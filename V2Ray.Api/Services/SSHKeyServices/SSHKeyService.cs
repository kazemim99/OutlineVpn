using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using System.Text;
using V2Ray.Api.Database;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.V2Keys.Dto;
using Renci.SshNet;
using V2Ray.Api.Services.SSHKeyServices.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json.Serialization;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace V2Ray.Api.Services.SSHKeyServices
{
    public class SSHKeyService : BaseService<SSHKey,
        int,
        UpdateSSHKeyInput,
        CreateSSHKeyInput,
        GetSSHKeyOutput,
        GetSSHKeyListOutput,
        SSHKeyFilterInput>,
        ISSHKeyService
    {
        private readonly DB _db;
        private readonly ICloudflareDnsUpdater _cloudflareDnsUpdater;
        private readonly IMapper _mapper;
        private readonly ILogger<SSHKeyService> _logger;

        public SSHKeyService(
            IMapper mapper,
            DB db,
            IOutlineVpnManager outlineVpnManager,
            ICloudflareDnsUpdater cloudflareDnsUpdater,
            ILogger<SSHKeyService> logger) : base(mapper, db)
        {
            _db = db;
            _mapper = mapper;
            _cloudflareDnsUpdater = cloudflareDnsUpdater;
            _logger = logger;
        }


        public async Task GenerateSshFromAdmin(CreateSSHKeyInput input)
        {
            try
            {
                if (input.Count > 10)
                    throw new ApiException("امکان ساخت بیشتر از ده اکانت همزمان  وجود ندارد");


                var user = _db.Users.Include(c => c.SSHKeyInfos).First(c => c.Id == input.UserId);

                var ceiling = user.SSHKeyInfos.Count(c => c.Enable) + input.Count;

                if (user.AccountLimit > 0 && user.AccountLimit < ceiling)
                {
                    throw new ApiException($"امکان ساخت بیش از {user.AccountLimit} برای شما وجود ندارد");
                }






                for (int i = 0; i < input.Count; i++)
                {
                    var keys = new List<SSHKey>();

                    input.UserName = input.UserName.IsNullOrEmpty() ? GenerateUser(i) : input.UserName;
                    input.ExpireDate = DateTime.UtcNow.AddDays(input.DurationId + 1 + input.ExtraDayId).ToPeString("yyyy/MM/dd");
                    input.ChargeDate = DateTime.UtcNow;
                    var key = new SSHKey
                    {
                        UserName = input.UserName,
                        Password = "pass",
                        Port = input.Port,
                        Name = input.Name,
                        ChargeDate = input.ChargeDate,
                        Server = "server",
                        DurationId = input.DurationId,
                        ExpireDate = input.ExpireDate.ToGeo(),
                        MultiUser = input.MultiUser,
                        UserId = input.UserId.Value,
                        Enable = true,
                        AccountType = input.AccountType,
                    };

                    if (input.DurationId == 30)
                    {
                        key.TotalTraffic = 45;
                    }
                    else if (input.DurationId == 60)
                    {
                        key.TotalTraffic = 90;
                    }
                    else if (input.DurationId == 90)
                    {
                        key.TotalTraffic = 135;
                    }

                    keys.Add(key);
                    _db.Add(key);

                    int id = 0;


                    if (input.AccountType == AccountType.V2RAy)
                    {
                        id = await CreateV2Ray(input.UserId.Value, keys, input.AccountType, AccountActionStatus.Create);
                    }

                    input.ChargeDate = DateTime.UtcNow;
                    if (input.DurationId != 1)
                    {
                        var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
                        var hasRecentOrder = _db.Orders
                            .Include(c => c.SSHKey)
                            .Any(c => c.SSHKey.UserName == input.UserName && c.CreatedAt >= oneWeekAgo);

                        if (!hasRecentOrder)
                        {
                            _db.Orders.Add(new Order
                            {
                                SSHKeyId = id,
                                CreatedAt = DateTime.UtcNow.Date,
                                DurationId = input.DurationId,
                                CreatorUserId = input.UserId,
                                UserId = input.UserId.Value,
                            });
                        }
                    }
                    input.UserName = string.Empty;
                    input.Password = string.Empty;
                }
                _db.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public override async Task UpdateAsync(int id, UpdateSSHKeyInput input, params string[] include)
        {

            var key = _db.SSHKeyInfos.Include(new[] { "Orders" }).First(a => a.Id == id);

            input.DurationId = key.DurationId;
            input.UserId = key.UserId;
            input.Enable = key.Enable;
            input.ChargeDate = key.ChargeDate;
            input.ExpireDate = key.ExpireDate.AddDays(input.ExtraDayId).ToPeString("yyyy/MM/dd");
            input.Server = key.Server;
            input.V2Guid = key.V2Guid;
            input.V2Id = key.V2Id;
            input.Code = key.Code;
            input.SSHCode = key.SSHCode.IsNullOrEmpty() ? $"ssh://{key.UserName}:{key.Password}@{key.Server}:{key.Port}?LCHepgjuVVy6UQRcXWdT8MFUMaAm31Xu8huIC93UZkqH92e6+WtSSbKYEp0PHKy5#${key.SSHCode}" : key.SSHCode; ;
            key.ExpireDate = input.ExpireDate.ToGeo().AddDays(input.ExtraDayId);
            var keys = new List<SSHKey>() { key };
            if (input.AccountType != key.AccountType)
            {
                key.AccountType = input.AccountType;

                //await CreateV2Ray(input.UserId.Value, keys, key.AccountType, AccountActionStatus.Delete);
                await CreateV2Ray(input.UserId.Value, keys, input.AccountType, AccountActionStatus.Create);
                return;
            }

            await base.UpdateAsync(id, input, include);

        }


        public override async Task<GetSSHKeyOutput> GetById(int id, params string[] include)
        {
            var result = await base.GetById(id, include);


            return result;
        }








        private int? GetV2Port(int userId, AccountType accountType)
        {
            var subId = 27000;
            if (userId == 71)
            {
                subId = 26000;
            }
            if (userId == 41)
            {
                subId = 25000;
            }

            return subId;
        }






        private string ConnectPanel(int userId, AccountType accountType)
        {
            var baseUrls = "p.iransshvpn.com";
            //if (userId == 71 || userId == 88 || userId == 41)//danial
            //{
            //    baseUrls = "v.iransshvpn.com";
            //}
            if(userId == 41)
            {
                baseUrls = "v5.iransshvpn.com";

            }
            var url = baseUrls.Split("/");
            IPAddress addresses = Dns.GetHostAddresses(url[0])[0];
            return $"https://{addresses}/FhFNjd6Q9p";
        }

        public async Task Charge(int keyId, int durationId, int userId)
        {
            try
            {
                _logger.LogInformation("Starting charge operation for KeyId: {KeyId}, DurationId: {DurationId}, UserId: {UserId}",
                    keyId, durationId, userId);

                // Validate input parameters
                ValidateChargeParameters(keyId, durationId, userId);

                // Calculate months from duration (handle edge cases)
                var month = CalculateMonths(durationId);

                // Fetch key with better error handling
                var key = await _db.SSHKeyInfos
                    .Include(k => k.User)
                    .FirstOrDefaultAsync(a => a.Id == keyId);

                if (key == null)
                {
                    _logger.LogError("SSH Key not found with Id: {KeyId}", keyId);
                    throw new ApiException($"کلید SSH با شناسه {keyId} یافت نشد");
                }

                _logger.LogDebug("Found key {KeyId} for user {UserId}, Account Type: {AccountType}",
                    keyId, key.UserId, key.AccountType);

                // Calculate new expiration date
                var expireDate = CalculateExpirationDate(key.ExpireDate, month);

                // Update key properties
                key.TrefficExpired = false;
                key.UsedTraffic = 0;
                key.LastPanelTraffic = 0;
                key.Enable = true;
                key.ChargeDate = DateTime.UtcNow;
                key.ExpireDate = expireDate;
                if (durationId < 0)
                {
                    key.ExpireDate = DateTime.Now.AddDays(-1);
                    key.Enable = false;
                }
                // Update duration
                UpdateDuration(key, durationId);

                // Set traffic limits based on duration and user
                SetTrafficLimits(key, month, userId);

                var keys = new List<SSHKey> { key };

                // Handle account operations based on duration
                await HandleAccountOperations(userId, key, keys, durationId);

                // Handle order management
                await HandleOrderManagement(key, keyId, userId, durationId);

                // Save all changes

                _db.Update(key);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Charge operation completed successfully for KeyId: {KeyId}", keyId);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (ChargeValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error during charge operation for KeyId: {KeyId}", keyId);
                throw new ApiException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during charge operation for KeyId: {KeyId}, DurationId: {DurationId}",
                    keyId, durationId);
                throw new ApiException($"خطا در شارژ اکانت: {ex.Message}");
            }
        }

        private void ValidateChargeParameters(int keyId, int durationId, int userId)
        {
            if (keyId <= 0)
                throw new ChargeValidationException("شناسه کلید نامعتبر است");

            if (userId <= 0)
                throw new ChargeValidationException("شناسه کاربر نامعتبر است");

            // Allow negative durations for adjustments, but validate range
            if (durationId < -365 || durationId > 91)
                throw new ChargeValidationException("مدت زمان نامعتبر است");
        }

        private int CalculateMonths(int durationId)
        {
            // Handle negative durations (for adjustments)
            var absDuration = Math.Abs(durationId);

            // Calculate months, ensuring at least 0
            var month = absDuration / 30;

            return month;
        }

        private DateTime CalculateExpirationDate(DateTime currentExpireDate, int months)
        {
            DateTime expireDate;

            // If key is already expired, start from now
            if (currentExpireDate.Date <= DateTime.UtcNow.Date)
            {
                expireDate = DateTime.UtcNow.AddMonths(months);
            }
            else
            {
                // Add to existing expiration date
                expireDate = currentExpireDate.AddMonths(months);
            }

            // Ensure expiration is not in the past
            if (expireDate.Date < DateTime.UtcNow.Date)
            {
                expireDate = DateTime.UtcNow;
            }

            return expireDate;
        }

        private void UpdateDuration(SSHKey key, int durationId)
        {
            if (durationId < 0)
            {
                // For negative durations, adjust the existing duration
                key.DurationId += durationId;

                // Ensure duration doesn't go negative
                if (key.DurationId < 0)
                {
                    _logger.LogWarning("Duration adjusted to negative value for KeyId: {KeyId}, setting to 0", key.Id);
                    key.DurationId = 0;
                }
            }
            else
            {
                // Replace with new duration
                key.DurationId = durationId;
            }
        }

        private void SetTrafficLimits(SSHKey key, int months, int userId)
        {
            if (months <= 0)
            {
                _logger.LogDebug("No traffic limit set for 0 months duration");
                return;
            }
            var config = new UserTrafficConfiguration();
            // Use configuration for traffic limits
            key.TotalTraffic = config.GetTrafficForDuration(userId, months);

            _logger.LogDebug("Set traffic limit for KeyId: {KeyId}, Months: {Months}, Traffic: {Traffic}GB",
                key.Id, months, key.TotalTraffic);
        }

        private async Task HandleAccountOperations(int userId, SSHKey key, List<SSHKey> keys, int durationId)
        {
            if (durationId <= 0)
            {
                // Disable account
                keys.First().Enable = false;

                if (key.AccountType == AccountType.V2RAy)
                {
                    _logger.LogInformation("Deleting V2Ray account for KeyId: {KeyId}", key.Id);
                    await CreateV2Ray(userId, keys, key.AccountType, AccountActionStatus.Delete);
                }
            }
            else
            {
                // Enable/Update account
                if (key.AccountType == AccountType.V2RAy)
                {
                    keys.First().UsedTraffic = 0;
                    _logger.LogInformation("Updating V2Ray account for KeyId: {KeyId}", key.Id);
                    await CreateV2Ray(userId, keys, key.AccountType, AccountActionStatus.Update);
                }
            }
        }

        private async Task HandleOrderManagement(SSHKey key, int keyId, int userId, int durationId)
        {
            if (key.User == null)
            {
                _logger.LogDebug("No user associated with key {KeyId}, skipping order management", keyId);
                return;
            }

            if (durationId >= 30)
            {
                // Create new order for valid durations
                _db.Orders.Add(new Order
                {
                    SSHKeyId = key.Id,
                    DurationId = durationId,
                    CreatedAt = DateTime.UtcNow,
                    CreatorUserId = userId,
                    UserId = userId,
                });

                _logger.LogInformation("Created order for KeyId: {KeyId}, Duration: {Duration}", keyId, durationId);
            }
            else if (durationId <= 0)
            {


                //var orders = await _db.Orders.Where(c => c.SSHKeyId == keyId).ToListAsync();
                //if (orders != null && orders.Any())
                //{
                //    // Adjust order durations
                //    orders.ForEach(c => c.DurationId += durationId);

                //    if (orders.Any(c => c.DurationId <= 0))
                //    {
                //        _logger.LogInformation("Removing {Count} orders for KeyId: {KeyId}", orders.Count, keyId);
                //        _db.Orders.RemoveRange(orders);
                //    }
                //    else
                //    {
                //        _logger.LogInformation("Updating {Count} orders for KeyId: {KeyId}", orders.Count, keyId);
                //        _db.Orders.UpdateRange(orders);
                //    }
                //}
            }
        }

        public async Task ChangeState(int id, int currentUserId, bool fromCharge = false)
        {
            var keyInfo = await _db.SSHKeyInfos.FirstOrDefaultAsync(a => a.Id == id);

            keyInfo.Enable = !keyInfo.Enable;

            if (fromCharge)
                keyInfo.Enable = true;


            var keys = new List<SSHKey>();

            if (!keyInfo.Enable)
            {
                //if (keyInfo.AccountType == AccountType.L2TP)
                //{
                //    keys.Add(keyInfo);
                //    _softEather.CreateSoftEather(keys, AccountActionStatus.Delete);
                //}
                if (keyInfo.AccountType == AccountType.V2RAy)
                {


                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    { return true; };

                    using var httpClient = new HttpClient(clientHandler)
                    {

                        Timeout = TimeSpan.FromSeconds(360)
                    };

                    var baseUrls = ConnectPanel(keyInfo.UserId, keyInfo.AccountType);
                    var loginData = new
                    {
                        username = "master640",
                        password = "!Q@W3e4r"
                    };

                    //var loginResponse = await httpClient.PostAsJsonAsync($"{baseUrls}/login", loginData);
                    //loginResponse.EnsureSuccessStatusCode();
                    //var sessionCookie = loginResponse.Headers.GetValues("Set-Cookie").ToString();
                    //httpClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);

                    //httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
                    //var panelresult = await httpClient.PostAsJsonAsync<Root>($"{baseUrls}/panel/api/inbounds/list", null);

                    //var contents = await panelresult.Content.ReadAsStringAsync();
                    //var accounts = JsonConvert.DeserializeObject<Root>(contents);
                    //var inbounds = accounts.obj.Where(c => c.enable).SelectMany(c => c.clientStats).FirstOrDefault(c => c.email == keyInfo.UserName);
                    //if (inbounds != null)
                    //{
                    //    //keyInfo.UsedTraffic = BytesToGigabytes(inbounds.down);
                    //    //_db.Update(keyInfo);
                    //    //_db.SaveChanges();
                    //}
                    await CreateV2Ray(currentUserId, new List<SSHKey> { keyInfo }, keyInfo.AccountType, AccountActionStatus.Delete, true);

                }


                //else if (keyInfo.AccountType == AccountType.SSH)
                //{
                //    await BulkDeleteServer(new List<SSHKey> { keyInfo });
                //    _db.Update(keyInfo);

                //}
                //else if (keyInfo.AccountType == AccountType.Outline)
                //{
                //    _outlineVpnManager.DeleteAccessKey(keyInfo.UserName);
                //    _db.Update(keyInfo);
                //}
            }
            else
            {
                if (keyInfo.AccountType == AccountType.V2RAy)
                {

                    if (keyInfo.DurationId == 30)
                    {
                        keyInfo.TotalTraffic =  45;
                    }
                    else if (keyInfo.DurationId == 60)
                    {
                        keyInfo.TotalTraffic = 90;
                    }
                    else if (keyInfo.DurationId == 90)
                    {
                        keyInfo.TotalTraffic =135;
                    }
                    await CreateV2Ray(currentUserId, new List<SSHKey> { keyInfo }, keyInfo.AccountType, AccountActionStatus.Create, true);
                }

                //else if (keyInfo.AccountType == AccountType.SSH)
                //{
                //    await BulkAddUserToServer(new List<SSHKey> { keyInfo });

                //    _db.Update(keyInfo);

                //}

                //else if (keyInfo.AccountType == AccountType.L2TP)
                //{
                //    keys.Add(keyInfo);
                //    _softEather.CreateSoftEather(keys);
                //}

            }
            _db.Update(keyInfo);

            _db.SaveChanges();



        }

        public override async Task Delete(int id)
        {
            var keyInfo = await _db.SSHKeyInfos.FirstAsync(a => a.Id == id);


            var keys = new List<SSHKey>
            {
                keyInfo
            };

            //if (keyInfo.AccountType == AccountType.SSH)
            //{
            //    await BulkDeleteServer(keys);
            //}


            if (keyInfo.AccountType == AccountType.V2RAy)
            {
                await CreateV2Ray(keyInfo.UserId, keys, keyInfo.AccountType, AccountActionStatus.Delete);
            }

            //if (keyInfo.AccountType == AccountType.L2TP)
            //{
            //    _softEather.CreateSoftEather(keys, AccountActionStatus.Delete);
            //}

            if (keyInfo.ExpireDate.Date >= DateTime.Now.AddDays(6))
            {
                var order = await _db.Orders.Where(c => c.SSHKeyId == keyInfo.Id).ToListAsync();
                if (order != null)
                {
                    _db.RemoveRange(order);
                    _db.SaveChanges();
                }

            }
            _db.Remove(keyInfo);
            _db.SaveChanges();
            //await base.Delete(id);
        }


        //public async Task Recreate(string name)
        //{
        //    var keys = _db.SSHKeyInfos.Where(a => a.UserName.Contains(name)).ToList();
        //    foreach (var item in keys)
        //    {
        //        Thread.Sleep(1000);
        //        await ChangeState(item.Id, item.UserId);
        //    }
        //}


        public async Task AdjustV2()
        {
            try
            {



                var items = _db.SSHKeyInfos.Where(c => c.ExpireDate.Date > DateTime.Now.Date && c.Enable);



                await CreateV2Ray(41, items.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 41).ToList(), AccountType.V2RAy);
                await CreateV2Ray(41, items.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 88).ToList(), AccountType.V2RAy);

                await CreateV2Ray(71, items.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 71).ToList(), AccountType.V2RAy);

                await CreateV2Ray(77, items.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 77).ToList(), AccountType.V2RAy);

                await CreateV2Ray(77, items.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 76).ToList(), AccountType.V2RAy);
                await CreateV2Ray(77, items.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 85).ToList(), AccountType.V2RAy);
                await CreateV2Ray(77, items.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 87).ToList(), AccountType.V2RAy);

            }
            catch (Exception ex)
            {


            }
        }
        public async Task UpdateUserTraffic()
        {
            var remarks = new List<string> { "M", "D", "R" };

            foreach (var item in remarks)
            {
                try
                {
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    { return true; };

                    using var httpClient = new HttpClient(clientHandler)
                    {
                        Timeout = TimeSpan.FromSeconds(360)
                    };

                    var userid = item == "D" ? 71 : 1;
                    var baseUrls = ConnectPanel(userid, AccountType.V2RAy);

                    var loginData = new
                    {
                        username = "master640",
                        password = "!Q@W3e4r"
                    };

                    var loginResponse = await httpClient.PostAsJsonAsync($"{baseUrls}/login", loginData);
                    loginResponse.EnsureSuccessStatusCode();
                    var sessionCookie = loginResponse.Headers.GetValues("Set-Cookie").ToString();
                    httpClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);

                    httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
                    var panelresult = await httpClient.GetFromJsonAsync<Root>($"{baseUrls}/panel/api/inbounds/list");

                    var inbounds = panelresult.obj.Where(c => c.enable);

                    foreach (var cliens in inbounds.Where(c => c.remark == item).Select(d => d.clientStats))
                    {
                        foreach (var item2 in cliens)
                        {
                            var key = await _db.SSHKeyInfos.FirstOrDefaultAsync(c => c.UserName == item2.email);

                            if (key != null)
                            {
                                try
                                {
                                    var currentPanelTraffic = BytesToGigabytes(item2.down);

                                    // Delta-based tracking:
                                    // - If panel traffic >= last known panel value: no reset, add only the delta
                                    // - If panel traffic < last known panel value: panel was reset, add the new value
                                    if (currentPanelTraffic < key.LastPanelTraffic)
                                    {
                                        // Panel was reset - add new panel traffic on top of stored usage
                                        key.UsedTraffic += currentPanelTraffic;
                                    }
                                    else
                                    {
                                        // No reset - add only the difference since last check
                                        key.UsedTraffic += (currentPanelTraffic - key.LastPanelTraffic);
                                    }

                                    // Remember the current panel value for next comparison
                                    key.LastPanelTraffic = currentPanelTraffic;

                                    // Check if traffic limit exceeded
                                    if (key.TotalTraffic > 0 && key.UsedTraffic >= key.TotalTraffic)
                                    {
                                        key.TrefficExpired = true;
                                    }

                                    _db.Update(key);
                                    _db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error updating traffic for user {UserName}", item2.email);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating traffic for remark {Remark}", item);
                }
            }
        }

        static double BytesToGigabytes(long bytes)
        {
            return Math.Round((double)bytes / 1_000_000_000, 2);
        }

        public async Task DisableExpired()
        {
            try
            {
                _logger.LogInformation("Starting DisableExpired background job");

                // Only look at currently enabled keys
                var keys = _db.SSHKeyInfos
                    .OrderByDescending(c => c.ExpireDate)
                    .Where(c => (c.ExpireDate.Date < DateTime.Now.Date || c.TrefficExpired))
                    .ToList();

                _logger.LogInformation("Found {Count} enabled keys to check for expiration", keys.Count);


                // Now delete from V2Ray panels
                if (keys.Any())
                {
                    await CreateV2Ray(41, keys.Where(c => c.AccountType == AccountType.V2RAy && (c.UserId == 41 || c.UserId == 88)).ToList(),
                        AccountType.V2RAy, AccountActionStatus.Delete);

                    //await CreateV2Ray(71, keys.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 71).ToList(),
                    //    AccountType.V2RAy, AccountActionStatus.Delete);

                    await CreateV2Ray(77, keys.Where(c => c.AccountType == AccountType.V2RAy && (c.UserId == 77 || c.UserId == 76 || c.UserId == 85 || c.UserId == 87)).ToList(),
                        AccountType.V2RAy, AccountActionStatus.Delete);
                    keys.ForEach(c => c.Enable = false);

                    // Update database
                    _db.UpdateRange(keys);
                    await _db.SaveChangesAsync();

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in DisableExpired background job");
                throw;
            }
        }


        public async Task<GenerateSSHOutput> GetKeyDetails(int userId)
        {
            var userKeyInfo = _db.SSHKeyInfos.FirstOrDefault(c => c.UserId == userId);
            if (userKeyInfo == null)
            {
                return new GenerateSSHOutput();
            }
            return new GenerateSSHOutput
            {
                ExpireDate = userKeyInfo.ExpireDate.ToPeString("yyyy/MM/dd"),
                Password = userKeyInfo.Password,
                Port = userKeyInfo.Port,
                UserName = userKeyInfo.UserName
            };
        }

        public override IQueryable<SSHKey> Filter(SSHKeyFilterInput filter)
        {
            try
            {
                var query = _db.SSHKeyInfos.AsQueryable();

                if (!filter.IsAdmin)
                {
                    query = query.Where(c => c.UserId == filter.UserId);
                }

                if (filter.UserName != null && filter.UserName.Length >= 3)
                    query = query.Where(a => a.UserName.Contains(filter.UserName));

                if (filter.Password != null && filter.Password.Length > 5)
                    query = query.Where(a => a.Password.Contains(filter.Password));

                if (filter.Name != null && filter.Name.Length > 4)
                    query = query.Where(a => a.Name.Contains(filter.Name));

                if (filter.Expired)
                {
                    query = query.Where(a => a.ExpireDate.Date <= DateTime.UtcNow.Date);
                }

                if (!filter.CodeFil.IsNullOrEmpty())
                {
                    query = query.Where(a => a.Code == filter.CodeFil);
                }

                return query.OrderByDescending(c => c.Id);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task SetUser(int userId, SetPasswordModel model)
        {
            var key = _db.SSHKeyInfos.FirstOrDefault(a => a.UserName == model.UserName && a.Password == model.Password);
            if (key == null)
                throw new ApiException("رمز عبور و نام کاربری اشتباه است");

            key.UserId = userId;
            _db.Update(key);
            _db.SaveChanges();
        }



        private string GenerateUser(int i)
        {
            var user = _db.SSHKeyInfos.Max(c => c.Id);
            if (user < 100)
                user += 100;

            return $"u{user + 300 + i}";
        }




        public async Task<int> CreateV2Ray(int currenUserId, List<SSHKey> sSHKeys, AccountType accountType, AccountActionStatus status = AccountActionStatus.Create, bool isSync = false)
        {
            try
            {
                if (!sSHKeys.Any())
                    return 0;


                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                { return true; };

                using var httpClient = new HttpClient(clientHandler)
                {

                    Timeout = TimeSpan.FromSeconds(7)
                };

                var panelUrl = ConnectPanel(currenUserId, accountType);




                var loginData = new
                {
                    username = "master640",
                    password = "!Q@W3e4r"
                };

                StringContent queryString = new StringContent(JsonConvert.SerializeObject(loginData), UnicodeEncoding.UTF8, "application/json");


                EntityEntry<SSHKey>? entity = null;



                var loginResponse = await httpClient.PostAsJsonAsync($"{panelUrl}/login", loginData);
                loginResponse.EnsureSuccessStatusCode();
                var sessionCookie = loginResponse.Headers.GetValues("Set-Cookie").ToString();
                httpClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);
                var number = new ConfigDateOutput();
                foreach (var item in sSHKeys)
                {


                    var formData = new Dictionary<string, string>();
                    if(number.SubId == 0)
                    {
                     number =await GetUserNumber(item.UserId);
                    }

                    if (status == AccountActionStatus.Delete)
                    {

                        var secSub = number.SubId;
                        var secUrl = panelUrl;


                        var url = $"{secUrl}/panel/api/inbounds/{secSub}/delClient/{item.V2Guid}";
                        var postResponse = await httpClient.PostAsync($"{url}", null);
                        //postResponse.EnsureSuccessStatusCode();

                    }
                    else
                    {
                        item.V2Port = GetV2Port(item.UserId, accountType);

                        if (item.V2Guid.IsNullOrEmpty())
                            item.V2Guid = Guid.NewGuid().ToString();


                        if (accountType == AccountType.V2RAy)
                        {
                            number =await GetUserNumber(currenUserId);
                            item.Code = $"vless://{item.V2Guid}@v{number.Domain}.iransshvpn.com:{number.Port}?type=ws&path=%2F&host=&security=none#{item.UserName}";

                            formData = new Dictionary<string, string>
        {
            { "id", number.SubId.ToString() },
            { "settings", "{\"clients\":" +
            "[" +
            "{\"flow\":\"\"," +
            "\"id\":\"" + item.V2Guid + "\"," +
            "\"email\":\"" + item.UserName + "\"," +
            "\"limitIp\":0," +
            "\"totalGB\":\"" +(item.DurationId/30) * 52212254720 + "\"," +
            "\"expiryTime\":0," +
            "\"enable\":true," +
            "\"tgId\":0," +

            "\"reset\":0" +
            "}" +
            "]" +
            "}"
                            },

        };
                        }


                        // Encode the form data
                        var content = new FormUrlEncodedContent(formData);
                        try
                        {

                            var url = $"{panelUrl}/panel/api/inbounds/addClient";
                            // Perform POST request to /panel/api/inbounds/add
                            var postResponse = await httpClient.PostAsync($"{url}", content);
                            postResponse.EnsureSuccessStatusCode();
                            var contents = await postResponse.Content.ReadAsStringAsync();

                            var jsonObject = JObject.Parse(contents);

                            var success = (bool)jsonObject["success"];

                            if (!((string)jsonObject["msg"]).Contains("Duplicate") && !success)
                            {
                                throw new ApiException((string)jsonObject["msg"]);
                            }
                            else
                            {
                                //item.AccountType = accountType;
                                //if (_db.SSHKeyInfos.Any(c => c.Id == item.Id))
                                //{
                                //    entity = _db.SSHKeyInfos.Update(item);
                                //    _db.SaveChanges();
                                //}
                                //else
                                //{
                                //    entity = _db.SSHKeyInfos.Add(item);
                                //    _db.SaveChanges();
                                //}
                                //try
                                //{
                                //    var result = _db.SaveChanges();
                                //    item.Id = entity.Entity.Id;
                                //}
                                //catch (Exception ex)
                                //{

                                //    throw;
                                //}
                            }
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }


                }


                number = new ConfigDateOutput();
                return sSHKeys.First().Id;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task RemoveOrphanPanelClients()
        {
            var remarkUserMap = new Dictionary<string, int>
            {
                { "M", 77 },
                { "D", 71 },
                { "R", 41 }
            };

          

            var dbUserNames = await _db.SSHKeyInfos
                .Where(c => c.AccountType == AccountType.V2RAy && c.Enable)
                .Select(c => c.UserName)
                .ToListAsync();

            foreach (var (remark, userId) in remarkUserMap)
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using var httpClient = new HttpClient(clientHandler) { Timeout = TimeSpan.FromSeconds(30) };

                var panelUrl = ConnectPanel(userId, AccountType.V2RAy);

                var loginData = new { username = "master640", password = "!Q@W3e4r" };
                var loginResponse = await httpClient.PostAsJsonAsync($"{panelUrl}/login", loginData);
                loginResponse.EnsureSuccessStatusCode();
                var sessionCookie = loginResponse.Headers.GetValues("Set-Cookie").First();
                httpClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);

                var panelResult = await httpClient.GetFromJsonAsync<Root>($"{panelUrl}/panel/api/inbounds/list");
                if (panelResult?.obj == null)
                    return;
                try
                {
                    var orphanKeys = new List<SSHKey>();

                    foreach (var inbound in panelResult.obj.Where(c => c.remark == remark && c.enable))
                    {
                        if (string.IsNullOrEmpty(inbound.settings) || inbound.clientStats == null)
                            continue;

                        var settingsJson = JObject.Parse(inbound.settings);
                        var clients = settingsJson["clients"] as JArray;
                        if (clients == null)
                            continue;

                        var clientGuidMap = clients
                            .Where(c => c["email"] != null)
                            .ToDictionary(c => c["email"]!.ToString(), c => c["id"]!.ToString());

                        foreach (var clientStat in inbound.clientStats)
                        {
                            if (!string.IsNullOrEmpty(clientStat.email)
                                && !dbUserNames.Contains(clientStat.email)
                                && clientGuidMap.TryGetValue(clientStat.email, out var guid))
                            {
                                orphanKeys.Add(new SSHKey
                                {
                                    UserName = clientStat.email,
                                    V2Guid = guid,
                                    UserId = userId,
                                    AccountType = AccountType.V2RAy
                                });
                            }
                        }
                    }

                    if (orphanKeys.Any())
                    {
                        _logger.LogInformation("Removing {Count} orphan clients for remark {Remark}", orphanKeys.Count, remark);
                        await CreateV2Ray(userId, orphanKeys, AccountType.V2RAy, AccountActionStatus.Delete);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error removing orphan clients for remark {Remark}", remark);
                }
            }
        }

        private async Task<ConfigDateOutput> GetUserNumber(int userId)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            { return true; };

            using var httpClient = new HttpClient(clientHandler)
            {
                Timeout = TimeSpan.FromSeconds(360)
            };

            var baseUrls = ConnectPanel(userId, AccountType.V2RAy);

            var loginData = new
            {
                username = "master640",
                password = "!Q@W3e4r"
            };

            var loginResponse = await httpClient.PostAsJsonAsync($"{baseUrls}/login", loginData);
            loginResponse.EnsureSuccessStatusCode();
            var sessionCookie = loginResponse.Headers.GetValues("Set-Cookie").ToString();
            httpClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);

            httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
            var panelresult = await httpClient.GetFromJsonAsync<Root>($"{baseUrls}/panel/api/inbounds/list");


            var data = new ConfigDateOutput()
            {
                Domain = 7,
                Port = 27000,
                SubId = panelresult.obj.First(c => c.port == 27000).id,
            };

            if (userId == 71 || userId == 88)//danial
            {
                var danial = panelresult.obj.First(c => c.port == 26000);
                data.Port = 26000;
                data.Domain = 6;
                data.SubId = danial.id;
            }
            if (userId == 41)//ramin
            {
                var ramin = panelresult.obj.First(c => c.port == 25000);

                data.Port = 25000;
                data.SubId = ramin.id;
                data.Domain = 8;
            }

            return data;
        }

    }
    public enum AccountActionStatus
    {
        Create,
        Update,
        Delete,
    }

    public class ClientStat
    {
        public int id { get; set; }
        public int inboundId { get; set; }
        public bool enable { get; set; }
        public string email { get; set; }
        public object up { get; set; }
        public long down { get; set; }
        public long expiryTime { get; set; }
        public object total { get; set; }
        public int reset { get; set; }
    }

    public class Obj
    {
        public int id { get; set; }
        public long up { get; set; }
        public long down { get; set; }
        public long total { get; set; }
        public string remark { get; set; }
        public bool enable { get; set; }
        public long expiryTime { get; set; }
        public List<ClientStat> clientStats { get; set; }
        public string listen { get; set; }
        public int port { get; set; }
        public string protocol { get; set; }
        public string settings { get; set; }
        public string streamSettings { get; set; }
        public string tag { get; set; }
        public string sniffing { get; set; }
    }

    public class Root
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public List<Obj> obj { get; set; }
    }






    public class AccessKey
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("accessUrl")]
        public string AccessUrl { get; set; }
    }

    public class AccessKeyListOutput
    {
        [JsonPropertyName("accessKeys")]
        public List<AccessKey> AccessKeys { get; set; }
    }


}

