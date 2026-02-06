# Quick Start Deployment Guide

## ⚡ Fast Track to Deploying the Fixes

This guide will get you up and running with the SSHKeyService fixes in **under 10 minutes**.

---

## Step 1: Add Configuration (2 minutes)

### 1.1 Add to your `appsettings.json`:

```json
{
  "SSHKeyService": {
    "Ssh": {
      "Port": 1027,
      "Username": "root",
      "Password": "REPLACE_WITH_YOUR_SSH_PASSWORD"
    },
    "Panel": {
      "Username": "REPLACE_WITH_YOUR_PANEL_USERNAME",
      "Password": "REPLACE_WITH_YOUR_PANEL_PASSWORD",
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

### 1.2 Replace the placeholder values:

- `REPLACE_WITH_YOUR_SSH_PASSWORD` → Your SSH password (was: `o^qw7^n3LdDhfs5O`)
- `REPLACE_WITH_YOUR_PANEL_USERNAME` → Your panel username (was: `master640`)
- `REPLACE_WITH_YOUR_PANEL_PASSWORD` → Your panel password (was: `!Q@W3e4r`)

**⚠️ IMPORTANT:** Do NOT commit real passwords to git!

---

## Step 2: Register Configuration (1 minute)

### Find your `Program.cs` or `Startup.cs` and add:

**For .NET 6+ (Program.cs):**
```csharp
// After builder.Services line
builder.Services.Configure<SSHKeyServiceConfiguration>(
    builder.Configuration.GetSection("SSHKeyService"));
```

**For older versions (Startup.cs):**
```csharp
// In ConfigureServices method
services.Configure<SSHKeyServiceConfiguration>(
    Configuration.GetSection("SSHKeyService"));
```

---

## Step 3: Build & Test (2 minutes)

```bash
# Build the project
dotnet build

# If build succeeds, run tests (if you have any)
dotnet test
```

**Expected:** Clean build with no errors.

**If you get build errors:**
- Ensure `SSHKeyServiceConfiguration.cs` file is in your project
- Ensure `SSHKeyService.cs` has been updated
- Check that `using Microsoft.Extensions.Options;` is at the top of SSHKeyService.cs

---

## Step 4: Deploy (5 minutes)

### 4.1 Backup Your Database
```bash
# Example for SQL Server
sqlcmd -S your-server -d your-database -Q "BACKUP DATABASE [YourDB] TO DISK = 'C:\Backup\YourDB.bak'"

# Or use your database management tool
```

### 4.2 Stop Your Application
```bash
# Stop the service/IIS/Docker container
```

### 4.3 Deploy New Code
```bash
# Copy new files to production
# Or use your deployment pipeline
```

### 4.4 Start Your Application
```bash
# Start the service/IIS/Docker container
```

---

## Step 5: Verify (5 minutes)

### 5.1 Check Application Starts
```bash
# Check logs for any startup errors
# Look for: "Starting DisableExpired background job"
```

### 5.2 Test Charge Operation
1. Charge an account through your admin panel
2. Verify the account is enabled
3. Wait 5 minutes
4. Verify the account is still enabled
5. ✅ If still enabled → Fix is working!

### 5.3 Monitor Logs
```bash
# Watch for these log messages:
# - "Starting charge operation for KeyId: X"
# - "Charge operation completed successfully for KeyId: X"
# - "Skipping key X - was charged recently at Y"
```

---

## Common Issues & Solutions

### Issue 1: Build Error - "IOptions not found"

**Solution:** Add this using statement to SSHKeyService.cs:
```csharp
using Microsoft.Extensions.Options;
```

### Issue 2: Runtime Error - "Unable to resolve service for type 'IOptions<SSHKeyServiceConfiguration>'"

**Solution:** Make sure you registered the configuration in Step 2.

### Issue 3: Runtime Error - "Configuration section 'SSHKeyService' not found"

**Solution:** Check your appsettings.json formatting - ensure proper JSON syntax.

### Issue 4: Accounts still being disabled

**Solution:**
1. Check that ChargeDate is being set correctly
2. Look for log message: "Skipping key X - was charged recently"
3. Ensure background job is using the new code
4. Restart the application to reload the background job

---

## Production Credentials (IMPORTANT!)

**DO NOT** store real passwords in `appsettings.json` in production!

### Option A: User Secrets (Development)
```bash
cd /path/to/V2Ray.Api
dotnet user-secrets init
dotnet user-secrets set "SSHKeyService:Ssh:Password" "your-real-password"
dotnet user-secrets set "SSHKeyService:Panel:Username" "your-real-username"
dotnet user-secrets set "SSHKeyService:Panel:Password" "your-real-password"
```

### Option B: Environment Variables (Production)
```bash
# Windows
set SSHKeyService__Ssh__Password=your-real-password
set SSHKeyService__Panel__Username=your-real-username
set SSHKeyService__Panel__Password=your-real-password

# Linux
export SSHKeyService__Ssh__Password="your-real-password"
export SSHKeyService__Panel__Username="your-real-username"
export SSHKeyService__Panel__Password="your-real-password"
```

### Option C: Azure Key Vault (Recommended for Production)
```csharp
builder.Configuration.AddAzureKeyVault(/* ... */);
```

---

## What This Fixes

✅ **Main Issue:** Accounts no longer disabled after charging
✅ **Security:** No more hard-coded credentials in source code
✅ **Reliability:** Better error handling and logging
✅ **Bugs:** Fixed date comparison bug
✅ **Maintainability:** Configuration-based user settings

---

## Monitoring After Deployment

### Day 1: Monitor Closely
- [ ] Check logs every 2 hours
- [ ] Test charge operations multiple times
- [ ] Verify background job doesn't disable charged accounts
- [ ] Monitor error rates

### Week 1: Regular Monitoring
- [ ] Check logs daily
- [ ] Review any errors or warnings
- [ ] Verify user reports (should be positive!)

### Week 2+: Normal Monitoring
- [ ] Weekly log review
- [ ] Monitor for any anomalies

---

## Rollback (If Needed)

If you encounter critical issues:

```bash
# 1. Stop the application
# 2. Restore previous version from backup
# 3. Restore database from backup (if needed)
# 4. Restart application
# 5. Contact support with error logs
```

**Note:** If rolling back, you MUST keep:
- The configuration setup (security requirement)
- The DisableExpired fix (critical bug)

---

## Success Criteria

Your deployment is successful when:

✅ Application starts without errors
✅ Charge operations complete successfully
✅ Charged accounts stay enabled after 30+ minutes
✅ Background job logs show "Skipping - was charged recently"
✅ Expired accounts are still being disabled correctly
✅ No credential-related errors in logs

---

## Getting Help

If you encounter issues:

1. **Check Logs** - Look for error messages and stack traces
2. **Review Documentation:**
   - [COMPLETE_REVIEW_SUMMARY.md](./COMPLETE_REVIEW_SUMMARY.md)
   - [CRITICAL_BUG_FIX.md](./CRITICAL_BUG_FIX.md)
   - [CHARGE_METHOD_IMPROVEMENTS.md](./CHARGE_METHOD_IMPROVEMENTS.md)
3. **Verify Configuration** - Ensure all config values are correct
4. **Test Locally** - Try reproducing the issue in development

---

## Next Steps After Successful Deployment

1. **Document the changes** - Update your team documentation
2. **Train team members** - Explain the new configuration
3. **Set up monitoring** - Configure alerts for errors
4. **Plan for improvements** - Review future recommendations
5. **Add tests** - Write unit tests for critical paths

---

**Deployment Time:** ~15 minutes
**Difficulty:** Easy
**Risk Level:** Low (fixes are defensive and additive)
**Impact:** High (solves critical user-reported issue)

---

**Last Updated:** 2026-01-05
**Version:** 1.0
**Status:** ✅ Ready for Deployment
