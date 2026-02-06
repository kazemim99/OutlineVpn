# CRITICAL BUG: Charge Changes Being Reverted

## Problem Description

When you charge an SSHKeyInfo and enable it, the changes are being reverted after a few minutes.

## Root Cause

There's a background job (`UpdateUserUsageLockerState`) that runs every 10 hours and calls `DisableExpired()`. This method has a logic bug that disables accounts even when they shouldn't be disabled.

### The Bug (Line 1178)

```csharp
var keys = _db.SSHKeyInfos.OrderByDescending(c => c.ExpireDate)
    .Where(c => (c.ExpireDate <= DateTime.Now || c.TrefficExpired))
    .ToList();

// Then later (lines 1193-1196):
if (item.Enable)
{
    item.Enable = false;
    _db.Update(item);
}
```

### Why This Causes the Issue

1. You charge an account → extends `ExpireDate` to future → sets `TrefficExpired = false`
2. Background job runs (every 10 hours)
3. Query finds keys where `ExpireDate <= DateTime.Now` **OR** `TrefficExpired == true`
4. Even though you extended the date, if there's any condition that makes `TrefficExpired = true`, it disables the key again

### Race Condition

There's also a race condition:
1. User charges account at 10:55 AM
2. Background job starts at 11:00 AM (might have already loaded the data before the charge completed)
3. Background job disables the account using old data

## The Fix

The `DisableExpired()` method needs to be more careful about what it disables. It should only disable accounts that are BOTH expired AND enabled, not accounts that were just charged.

### Solution 1: Add a Grace Period After Charging

```csharp
public async Task DisableExpired()
{
    try
    {
        _logger.LogInformation("Starting DisableExpired background job");

        // Only disable keys that are expired AND haven't been charged recently
        var gracePeriodMinutes = 30; // 30 minute grace period after charging
        var graceTime = DateTime.UtcNow.AddMinutes(-gracePeriodMinutes);

        var keys = _db.SSHKeyInfos
            .OrderByDescending(c => c.ExpireDate)
            .Where(c =>
                // Expired by date AND not recently charged
                (c.ExpireDate <= DateTime.UtcNow && c.ChargeDate <= graceTime)
                ||
                // Traffic expired AND not recently charged AND date also expired
                (c.TrefficExpired && c.ExpireDate <= DateTime.UtcNow && c.ChargeDate <= graceTime)
            )
            .ToList();

        _logger.LogInformation("Found {Count} keys to potentially disable", keys.Count);

        // ... rest of the method
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in DisableExpired background job");
        throw;
    }
}
```

### Solution 2: Check Current State Before Disabling (Safer)

```csharp
public async Task DisableExpired()
{
    try
    {
        _logger.LogInformation("Starting DisableExpired background job");

        var keys = _db.SSHKeyInfos
            .OrderByDescending(c => c.ExpireDate)
            .Where(c => c.Enable) // Only look at enabled keys
            .ToList();

        var keysToDisable = new List<SSHKey>();

        foreach (var item in keys)
        {
            try
            {
                var shouldDisable = false;
                var reason = "";

                // Check if date expired
                if (item.ExpireDate.Date < DateTime.UtcNow.Date)
                {
                    shouldDisable = true;
                    reason = "Date expired";
                }
                // Only disable for traffic if date is also expired or close to expiring
                else if (item.TrefficExpired && item.ExpireDate.Date <= DateTime.UtcNow.AddDays(1).Date)
                {
                    shouldDisable = true;
                    reason = "Traffic expired and date expiring soon";
                }

                if (shouldDisable)
                {
                    // Double-check the account wasn't just charged
                    var wasRecentlyCharged = item.ChargeDate >= DateTime.UtcNow.AddMinutes(-30);

                    if (!wasRecentlyCharged)
                    {
                        _logger.LogInformation("Disabling key {KeyId} for user {UserId}. Reason: {Reason}",
                            item.Id, item.UserId, reason);

                        item.Enable = false;
                        keysToDisable.Add(item);
                    }
                    else
                    {
                        _logger.LogInformation("Skipping key {KeyId} - was charged recently at {ChargeDate}",
                            item.Id, item.ChargeDate);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing key {KeyId}", item.Id);
            }
        }

        // Now delete from V2Ray
        if (keysToDisable.Any())
        {
            _logger.LogInformation("Deleting {Count} keys from V2Ray panels", keysToDisable.Count);

            await CreateV2Ray(41, keysToDisable.Where(c => c.AccountType == AccountType.V2RAy && (c.UserId == 41 || c.UserId == 88)).ToList(),
                AccountType.V2RAy, AccountActionStatus.Delete);

            await CreateV2Ray(71, keysToDisable.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 71).ToList(),
                AccountType.V2RAy, AccountActionStatus.Delete);

            await CreateV2Ray(77, keysToDisable.Where(c => c.AccountType == AccountType.V2RAy && (c.UserId == 77 || c.UserId == 76 || c.UserId == 85 || c.UserId == 87)).ToList(),
                AccountType.V2RAy, AccountActionStatus.Delete);

            // Update database
            _db.UpdateRange(keysToDisable);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Successfully disabled {Count} expired keys", keysToDisable.Count);
        }
        else
        {
            _logger.LogInformation("No keys needed to be disabled");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Fatal error in DisableExpired background job");
        throw;
    }
}
```

### Solution 3: Immediate Fix (Minimal Change)

Simply add a check to not disable recently charged accounts:

```csharp
// At line 1178, change to:
var keys = _db.SSHKeyInfos
    .OrderByDescending(c => c.ExpireDate)
    .Where(c =>
        (c.ExpireDate <= DateTime.Now || c.TrefficExpired)
        && c.ChargeDate <= DateTime.UtcNow.AddMinutes(-30) // Not charged in last 30 min
    )
    .ToList();
```

## Additional Issues Found

### 1. Using DateTime.Now instead of DateTime.UtcNow
Line 1178 uses `DateTime.Now` which can cause timezone issues. Should use `DateTime.UtcNow` consistently.

### 2. Race Condition in UpdateUserTraffic
The `UpdateUserTraffic` method (runs every 6 hours) also updates keys and could interfere with charging.

### 3. No Locking Mechanism
Multiple background jobs and user actions can modify the same records simultaneously without any locking.

## Recommended Implementation

I recommend **Solution 2** because:
1. ✅ Most explicit about what should be disabled and why
2. ✅ Includes grace period check
3. ✅ Adds comprehensive logging
4. ✅ Handles edge cases properly
5. ✅ Makes debugging easier

## Testing the Fix

### Test Case 1: Charge and Verify Not Disabled
```csharp
1. Charge an expired account
2. Wait for background job to run (or trigger manually)
3. Verify account is still enabled
4. Check logs to confirm it was skipped due to recent charge
```

### Test Case 2: Old Expired Account
```csharp
1. Create an account with ExpireDate in the past
2. Don't charge it
3. Run background job
4. Verify it gets disabled
5. Check logs to confirm reason
```

### Test Case 3: Traffic Expired But Date Valid
```csharp
1. Create account with valid date but traffic expired
2. Run background job
3. Should NOT be disabled (date is still valid)
4. Check logs
```

## Deployment Steps

1. **Backup Database** - Before deploying the fix
2. **Deploy Fix** - Update DisableExpired method
3. **Monitor Logs** - Watch for 24 hours
4. **Verify** - Check that charged accounts stay enabled
5. **Monitor Metrics** - Ensure expired accounts are still being disabled correctly

## Alternative: Disable Background Job Temporarily

If you need an immediate workaround:

1. Stop the background job service
2. Manually run DisableExpired when needed
3. Deploy the proper fix
4. Re-enable background job

In Program.cs or Startup.cs, comment out:
```csharp
// services.AddHostedService<UpdateUserUsageLockerState>();
```

**WARNING:** This will prevent automatic expiration until the job is re-enabled.

---

**Priority:** 🔴 CRITICAL
**Impact:** High - Affects all charge operations
**Effort:** Low - Single method fix
**Risk:** Low - Adds safety check, doesn't change existing logic for truly expired accounts
