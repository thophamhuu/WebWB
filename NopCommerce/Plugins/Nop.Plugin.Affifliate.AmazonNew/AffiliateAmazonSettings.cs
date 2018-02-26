using Newtonsoft.Json;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Web;

namespace Nop.Plugin.Affiliate.Amazon
{
    public class AffiliateAmazonSettings : ISettings
    {
        public DateTime? RunTaskTime { get; set; }
        public string Service { get; set; }
        public string Endpoint { get; set; }
        public string Version { get; set; }
        public string Accounts { get; set; }
        public int Durations { get; set; }
        public string Folder { get; set; }
    }
    public class AffiliateAmazonAccount
    {
        public int Id { get; set; }
        public string AssociateTag { get; set; }
        public string AccessKeyID { get; set; }
        public string SecretKey { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class AffAmazonContext
    {
        private static volatile AffAmazonContext instance;
        private static object syncRoot = new Object();
        private AffAmazonContext()
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            AffiliateAmazonSettings amazonSettings = settingService.LoadSetting<AffiliateAmazonSettings>();
            if (amazonSettings != null)
            {
                accounts = new List<AffAmazonAccount>();
                var list = !string.IsNullOrEmpty(amazonSettings.Accounts) && !string.IsNullOrWhiteSpace(amazonSettings.Accounts) ? JsonConvert.DeserializeObject<List<AffiliateAmazonAccount>>(amazonSettings.Accounts) : new List<AffiliateAmazonAccount>();
                if (list != null)
                {
                    list.ForEach(x =>
                    {
                        if (x.IsActive)
                        {
                            var account = new AffAmazonAccount
                            {
                                AccessKeyID = x.AccessKeyID,
                                AssociateTag = x.AssociateTag,
                                SecretKey = x.SecretKey,
                            };
                            accounts.Add(account);
                        }
                    });
                }
                service = amazonSettings.Service;
                endpoint = amazonSettings.Endpoint;
                version = amazonSettings.Version;
                durations = amazonSettings.Durations;
                var context = HttpContext.Current;
                folder = amazonSettings.Folder;
            }
        }
        public static void Resolve()
        {
            instance = new AffAmazonContext();
        }
        public static AffAmazonContext Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AffAmazonContext();
                    }
                }
                return instance;
            }
        }
        private string service;
        private string endpoint;
        private string version;
        private int durations;
        private string folder;
        private List<AffAmazonAccount> accounts;
        public string Service { get { return service; } }
        public string Endpoint { get { return endpoint; } }
        public string Version { get { return version; } }
        public int Durations { get { return durations; } }
        public string Folder { get { return folder; } }

        public List<AffAmazonAccount> Accounts
        {
            get
            {
                return accounts;
            }
        }
        public int Count
        {
            get
            {
                return accounts != null ? accounts.Count : 0;
            }
        }
        public bool IsUsed(int index)
        {
            var account = accounts[index];
            return account.UsedTime.HasValue && (Durations - (int)(DateTime.Now - account.UsedTime.Value).TotalMilliseconds) > 0;
        }
        public int Wait(int index)
        {
            var account = accounts[index];
            if (!account.UsedTime.HasValue)
                return 0;
            int spent = (int)(DateTime.Now - account.UsedTime.Value).TotalMilliseconds;
            return Durations - spent > 0 ? Durations - spent : 0;
        }
        public void UpdateAccount(int index, DateTime usedTime)
        {
            accounts[index].UsedTime = usedTime;
        }
    }
    public class AffAmazonAccount
    {
        public string AssociateTag { get; set; }
        public string AccessKeyID { get; set; }
        public string SecretKey { get; set; }
        public DateTime? UsedTime { get; set; }
    }
}