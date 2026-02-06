# SSHKeyService Refactoring Summary

## Overview
This document outlines the comprehensive refactoring performed on the `SSHKeyService` class, with particular focus on the `Charge` method and security improvements.

---

## Critical Issues Fixed

### 1. Security Vulnerabilities

#### Hard-coded Credentials (RESOLVED)
**Problem:** Credentials were hard-coded throughout the service:
- SSH password: `"o^qw7^n3LdDhfs5O"`
- Panel username/password: `"master640"` / `"!Q@W3e4r"`

**Solution:**
- Created `SSHKeyServiceConfiguration` class to manage all configuration
- Moved credentials to configuration file
- Injected configuration via `IOptions<SSHKeyServiceConfiguration>`

**Files Modified:**
- [SSHKeyService.cs:30-55](../../V2Ray.Api/Services/SSHKeyServices/SSHKeyService.cs#L30-L55) - Added configuration injection
- [SSHKeyServiceConfiguration.cs](../../V2Ray.Api/Services/SSHKeyServices/SSHKeyServiceConfiguration.cs) - New configuration model

---

### 2. Charge Method Refactoring

#### Date Comparison Bug (Line 662)
**Problem:**
```csharp
if (key.CreatedAt.Date >= DateTime.UtcNow.AddDays(5).Date)
```
This checked if the account was created 5 days in the FUTURE (impossible), should check if it's OLDER than 5 days.

**Solution:**
```csharp
if (key.CreatedAt.Date <= DateTime.UtcNow.AddDays(-5).Date)
```
Now correctly checks if account is older than 5 days.

**Location:** [SSHKeyService.cs:779](../../V2Ray.Api/Services/SSHKeyServices/SSHKeyService.cs#L779)

---

#### Input Validation
**Problem:** No validation of input parameters, using `First()` which throws if not found.

**Solution:**
- Added `ValidateChargeParameters()` method
- Uses `FirstOrDefaultAsync()` with null check
- Validates keyId, userId, and durationId ranges

**Location:** [SSHKeyService.cs:644-655](../../V2Ray.Api/Services/SSHKeyServices/SSHKeyService.cs#L644-L655)

---

#### Division by Zero Protection
**Problem:**
```csharp
var month = durationId / 30;
```
If durationId < 30, month would be 0.

**Solution:**
- Created `CalculateMonths()` method with proper handling
- Uses `Math.Abs()` for negative durations
- Explicitly handles edge cases

**Location:** [SSHKeyService.cs:657-666](../../V2Ray.Api/Services/SSHKeyServices/SSHKeyService.cs#L657-L666)

---

#### Improved Code Organization
The Charge method was refactored into smaller, focused methods:

1. **ValidateChargeParameters** - Input validation
2. **CalculateMonths** - Duration to months conversion
3. **CalculateExpirationDate** - Expiration date logic
4. **UpdateDuration** - Handle duration updates
5. **SetTrafficLimits** - Configure traffic based on user/duration
6. **HandleAccountOperations** - V2Ray account operations
7. **HandleOrderManagement** - Order creation/updates

**Benefits:**
- Each method has a single responsibility
- Easier to test and maintain
- Better error handling at each step
- Clear separation of concerns

---

### 3. Logging Infrastructure

#### Problem
- Empty catch blocks throughout the code
- No visibility into errors or operations
- Difficult to debug production issues

#### Solution
- Added `ILogger<SSHKeyService>` dependency injection
- Added comprehensive logging throughout Charge method:
  - Information level: Operation start/completion, key operations
  - Debug level: Detailed state information
  - Warning level: Validation errors, unusual conditions
  - Error level: Exceptions with context

**Examples:**
```csharp
_logger.LogInformation("Starting charge operation for KeyId: {KeyId}, DurationId: {DurationId}, UserId: {UserId}",
    keyId, durationId, userId);

_logger.LogError(ex, "Unexpected error during charge operation for KeyId: {KeyId}, DurationId: {DurationId}",
    keyId, durationId);
```

**Location:** [SSHKeyService.cs:570-802](../../V2Ray.Api/Services/SSHKeyServices/SSHKeyService.cs#L570-L802)

---

### 4. Configuration-Based User Settings

#### Problem
Hard-coded user IDs with special treatment:
```csharp
if (month == 1)
{
    key.TotalTraffic = userId == 82 ? 40 : 55;
}
```

#### Solution
Created flexible configuration system:
```json
{
  "UserTraffic": {
    "DefaultOneMonthTraffic": 55,
    "OneMonthTraffic": {
      "82": 40
    }
  }
}
```

**Benefits:**
- Easy to add new users with custom limits
- No code changes needed for business rule updates
- Clear separation of logic and configuration

**Location:** [SSHKeyServiceConfiguration.cs:26-46](../../V2Ray.Api/Services/SSHKeyServices/SSHKeyServiceConfiguration.cs#L26-L46)

---

### 5. Exception Handling Improvements

#### Problem
```csharp
catch (Exception ex)
{
    throw new ApiException(ex.Message);  // Loses stack trace
}
```

#### Solution
```csharp
catch (ApiException)
{
    throw;  // Preserve original exception
}
catch (ChargeValidationException ex)
{
    _logger.LogWarning(ex, "Validation error...");
    throw new ApiException(ex.Message);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error...");
    throw new ApiException($"خطا در شارژ اکانت: {ex.Message}");
}
```

**Benefits:**
- Preserves stack traces
- Differentiates between validation and unexpected errors
- Logs all errors before re-throwing

---

## Configuration Setup

### 1. Add to your `appsettings.json`:
```json
{
  "SSHKeyService": {
    "Ssh": {
      "Port": 1027,
      "Username": "root",
      "Password": "YOUR_SSH_PASSWORD_HERE"
    },
    "Panel": {
      "Username": "YOUR_PANEL_USERNAME_HERE",
      "Password": "YOUR_PANEL_PASSWORD_HERE",
      "BaseDomain": "p.iransshvpn.com"
    },
    "NodeIps": [
      "199.247.28.162"
    ],
    "UserTraffic": {
      "DefaultOneMonthTraffic": 55,
      "DefaultTwoMonthTraffic": 110,
      "DefaultThreeMonthTraffic": 165,
      "OneMonthTraffic": {
        "82": 40
      },
      "TwoMonthTraffic": {
        "82": 80
      },
      "ThreeMonthTraffic": {
        "82": 120
      }
    }
  }
}
```

### 2. Register in Startup.cs or Program.cs:
```csharp
services.Configure<SSHKeyServiceConfiguration>(
    configuration.GetSection("SSHKeyService"));
```

---

## Security Best Practices

### Storing Credentials

**DO NOT** commit real credentials to version control. Use one of these approaches:

#### Option 1: User Secrets (Development)
```bash
dotnet user-secrets set "SSHKeyService:Ssh:Password" "your-password"
dotnet user-secrets set "SSHKeyService:Panel:Username" "your-username"
dotnet user-secrets set "SSHKeyService:Panel:Password" "your-password"
```

#### Option 2: Environment Variables (Production)
```bash
export SSHKeyService__Ssh__Password="your-password"
export SSHKeyService__Panel__Username="your-username"
export SSHKeyService__Panel__Password="your-password"
```

#### Option 3: Azure Key Vault / AWS Secrets Manager
Use cloud-based secret management for production environments.

---

## Testing Recommendations

### Unit Tests to Add

1. **Charge Method Validation**
   - Test with invalid keyId (≤ 0)
   - Test with invalid userId (≤ 0)
   - Test with out-of-range durationId
   - Test with non-existent keyId

2. **Date Calculations**
   - Test with expired keys
   - Test with future expiration dates
   - Test with negative durations

3. **Traffic Limits**
   - Test default traffic limits
   - Test user-specific traffic limits
   - Test with custom user configurations

4. **Order Management**
   - Test order creation for valid durations
   - Test order updates for negative durations
   - Test date restriction (5-day limit)

---

## Migration Checklist

- [ ] Copy `SSHKeyServiceConfiguration.cs` to your project
- [ ] Update `SSHKeyService.cs` with refactored code
- [ ] Add configuration section to `appsettings.json`
- [ ] Move credentials to secure storage (User Secrets/Environment Variables)
- [ ] Register configuration in Startup/Program.cs
- [ ] Update dependency injection to include ILogger and IOptions
- [ ] Test Charge method with various scenarios
- [ ] Monitor logs for any issues
- [ ] Remove old hard-coded credential references

---

## Breaking Changes

### Constructor Changes
The `SSHKeyService` constructor now requires two additional parameters:
```csharp
// Old
public SSHKeyService(IMapper mapper, DB db, IOutlineVpnManager outlineVpnManager,
    ICloudflareDnsUpdater cloudflareDnsUpdater, ISoftEather softEather)

// New
public SSHKeyService(IMapper mapper, DB db, IOutlineVpnManager outlineVpnManager,
    ICloudflareDnsUpdater cloudflareDnsUpdater, ISoftEather softEather,
    ILogger<SSHKeyService> logger,
    IOptions<SSHKeyServiceConfiguration> config)
```

**Action Required:** Ensure your DI container is configured to provide these dependencies.

---

## Performance Improvements

1. **Async/Await Consistency**
   - Changed `_db.SaveChanges()` to `await _db.SaveChangesAsync()`
   - More efficient database operations

2. **Database Queries**
   - Using `FirstOrDefaultAsync` instead of `First`
   - Better for async operations

---

## Future Recommendations

1. **Timezone Handling**
   - Standardize on UTC throughout
   - Only convert to local time for display
   - Replace mixed `DateTime.Now` and `DateTime.UtcNow` usage

2. **Empty Catch Blocks**
   - Replace remaining empty catch blocks with proper logging
   - Consider specific exception handling strategies

3. **Port Configuration**
   - Move hard-coded port numbers (27000, 26000, 25000) to configuration
   - Use similar pattern as UserTraffic configuration

4. **Transaction Management**
   - Consider wrapping Charge operations in database transactions
   - Ensures atomicity of multi-step operations

5. **Retry Logic**
   - Add retry logic for SSH connections
   - Add retry logic for HTTP panel operations

---

## Support

For questions or issues with this refactoring:
1. Review the code comments in the refactored methods
2. Check the example configuration file
3. Ensure all dependencies are properly registered
4. Review logs for detailed error information

---

**Document Version:** 1.0
**Last Updated:** 2026-01-05
**Author:** Code Review & Refactoring
