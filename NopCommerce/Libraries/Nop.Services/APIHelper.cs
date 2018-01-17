using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nop.Services
{
    public sealed class APIHelper
    {
        private const string API_PATTERN = "/api/{0}/{1}";
        private HttpClient _client;
        private string _clientId = "58aeb213-3887-40dd-928c-40a36b2d5ea3";
        private string _clientSecret = "c8dfe65e0ed657b49844f775dcd17341";
        private APISettings apiSettings = APISettings.Load();
        private APIHelper()
        {
            //var nopConfig = EngineContext.Current.Resolve<NopConfig>();

            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:49647/");
            _client.Timeout = TimeSpan.FromSeconds(20000);

            try
            {
                //APISettings apiSettings = APISettings.Load();
                if (apiSettings != null)
                {
                    apiSettings.ClientId = _clientId;
                    apiSettings.ClientSecret = _clientSecret;
                }
                if (DateTime.Now > apiSettings.ExpireAccessToken)
                {
                    AccessToken(apiSettings);
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiSettings.Access_Token);

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        private static APIHelper _instance = null;
        private static object syncRoot = new object();
        public static APIHelper Instance
        {
            get
            {

                lock (syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new APIHelper();

                    }

                    return _instance;
                }


            }
        }
        public static void Initilize()
        {
            _instance = new APIHelper();
        }
        private void AccessToken(APISettings apiSettings = null)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "token");
            request.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string,string>("grant_type","client_credentials"),
                    new KeyValuePair<string,string>("client_id",apiSettings.ClientId),
                    new KeyValuePair<string,string>("client_secret",apiSettings.ClientSecret)
            });


            var responseTask = _client.SendAsync(request);

            while (!responseTask.IsCompleted)
            {

            }
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var access_token = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
                apiSettings.Access_Token = access_token["access_token"];
                apiSettings.Refresh_Token = access_token["refresh_token"];
                apiSettings.ExpireAccessToken = DateTime.Parse(access_token[".expires"].ToString());
                apiSettings.SaveSettings();
            }
        }
        private void RefreshToken(APISettings apiSettings)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "token");
            request.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string,string>("grant_type","refresh_token"),
                    new KeyValuePair<string,string>("refresh_token",apiSettings.Refresh_Token)
            });


            _client.SendAsync(request)
                  .ContinueWith(responseTask =>
                  {
                      if (responseTask.IsCompleted)
                      {
                          var result = responseTask.Result;
                          if (result.IsSuccessStatusCode)
                          {
                              var access_token = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
                              apiSettings.Access_Token = access_token["access_token"];
                              apiSettings.ExpireAccessToken = DateTime.Parse(access_token[".expires"].ToString());
                              apiSettings.SaveSettings();
                          }
                          else if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                          {
                              AccessToken(apiSettings);
                          }
                      }
                  });
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiSettings.Access_Token);
        }
        private string GetAsync(string serviceName, string methodName, IDictionary<string, dynamic> parameters)
        {
            //APISettings apiSettings = APISettings.Load();
            if (apiSettings.ExpireAccessToken <= DateTime.Now)
            {
                RefreshToken(apiSettings);
            }
            string url = string.Format(API_PATTERN, serviceName, methodName);

            if (parameters != null)
            {
                url += string.Format("?{0}", string.Join("&",
                parameters.Select(kvp =>
                string.Format("{0}={1}", kvp.Key, kvp.Value))));
            }
            //try
            //{
            var getAsync = _client.GetStringAsync(url);
            while (!getAsync.IsCompleted) { }
            var getResult = getAsync.Result;
            return getResult;
            //}
            //catch (Exception ex)
            //{
            //    ILogger logger = EngineContext.Current.Resolve<ILogger>();
            //    logger.Error(ex.Message, ex);
            //    return ex.Message;
            //}
        }
        public T GetAsync<T>(string serviceName, string methodName, IDictionary<string, dynamic> parameters)
        {
            var result = GetAsync(serviceName, methodName, parameters);
            try
            {
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception ex)
            {
                //ILogger logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(ex.Message, ex);
            }
            return default(T);
        }
        public IList<T> GetListAsync<T>(string serviceName, string methodName, IDictionary<string, dynamic> parameters)
        {
            var result = GetAsync(serviceName, methodName, parameters);
            try
            {
                return JsonConvert.DeserializeObject<List<T>>(result);
            }
            catch (Exception ex)
            {
                //ILogger logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(ex.Message, ex);
            }
            return default(List<T>);
        }
        public IPagedList<T> GetPagedListAsync<T>(string serviceName, string methodName, IDictionary<string, dynamic> parameters)
        {
            var result = GetAsync(serviceName, methodName, parameters);

            try
            {
                var apiPagedList = JsonConvert.DeserializeObject<APIPagedList<T>>(result);
                return apiPagedList.ConvertAPIPagedListToPagedList();
            }
            catch (Exception ex)
            {
                //ILogger logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(ex.Message, ex);
            }
            return default(IPagedList<T>);
        }

        public bool PostAsync(string serviceName, string methodName, object body, IDictionary<string, dynamic> parameters = null)
        {
            //APISettings apiSettings = APISettings.Load();
            if (apiSettings.ExpireAccessToken <= DateTime.Now)
            {
                RefreshToken(apiSettings);
            }
            string url = string.Format(API_PATTERN, serviceName, methodName);

            if (parameters != null)
            {
                url += string.Format("?{0}", string.Join("&",
                parameters.Select(kvp =>
                string.Format("{0}={1}", kvp.Key, kvp.Value))));
            }
            //try
            //{
            var getResult = _client.PostAsJsonAsync(url, body).Result;
            return getResult.IsSuccessStatusCode;
            //}
            //catch (Exception ex)
            //{
            //    ILogger logger = EngineContext.Current.Resolve<ILogger>();
            //    logger.Error(ex.Message, ex);
            //    return false;
            //}
        }
        public T PostAsync<T>(string serviceName, string methodName, object body, IDictionary<string, dynamic> parameters = null)
        {
            //APISettings apiSettings = APISettings.Load();
            if (apiSettings.ExpireAccessToken <= DateTime.Now)
            {
                RefreshToken(apiSettings);
            }
            string url = string.Format(API_PATTERN, serviceName, methodName);

            if (parameters != null)
            {
                url += string.Format("?{0}", string.Join("&",
                parameters.Select(kvp =>
                string.Format("{0}={1}", kvp.Key, kvp.Value))));
            }
            //try
            //{
            var getResult = _client.PostAsJsonAsync(url, body).Result;
            return JsonConvert.DeserializeObject<T>(getResult.Content.ReadAsStringAsync().Result);
            //}
            //catch (Exception ex)
            //{
            //    ILogger logger = EngineContext.Current.Resolve<ILogger>();
            //    logger.Error(ex.Message, ex);
            //    return default(T);
            //}
        }
        public IList<T> PostListAsync<T>(string serviceName, string methodName, object body, IDictionary<string, dynamic> parameters = null)
        {
            //APISettings apiSettings = APISettings.Load();
            if (apiSettings.ExpireAccessToken <= DateTime.Now)
            {
                RefreshToken(apiSettings);
            }
            string url = string.Format(API_PATTERN, serviceName, methodName);

            if (parameters != null)
            {
                url += string.Format("?{0}", string.Join("&",
                parameters.Select(kvp =>
                string.Format("{0}={1}", kvp.Key, kvp.Value))));
            }
            //try
            //{
            var getResult = _client.PostAsJsonAsync(url, body).Result;
            return JsonConvert.DeserializeObject<List<T>>(getResult.Content.ReadAsStringAsync().Result);
            //}
            //catch (Exception ex)
            //{
            //    ILogger logger = EngineContext.Current.Resolve<ILogger>();
            //    logger.Error(ex.Message, ex);
            //    return default(List<T>);
            //}
        }

        public bool PutAsync(string serviceName, string methodName, object body, IDictionary<string, dynamic> parameters = null)
        {
            //APISettings apiSettings = APISettings.Load();
            if (apiSettings.ExpireAccessToken <= DateTime.Now)
            {
                RefreshToken(apiSettings);
            }
            string url = string.Format(API_PATTERN, serviceName, methodName);

            if (parameters != null)
            {
                url += string.Format("?{0}", string.Join("&",
                parameters.Select(kvp =>
                string.Format("{0}={1}", kvp.Key, kvp.Value))));
            }
            //try
            //{
            var getResult = _client.PutAsJsonAsync(url, body).Result;
            return getResult.IsSuccessStatusCode;
            //}
            //catch (Exception ex)
            //{
            //    ILogger logger = EngineContext.Current.Resolve<ILogger>();
            //    logger.Error(ex.Message, ex);
            //    return false;
            //}
        }
    }

    public class APISettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
        public DateTime ExpireAccessToken { get; set; }
        public DateTime ExpireRefreshToken { get; set; }
        public static APISettings Load()
        {
            APISettings apiSettings = new APISettings();

            try
            {
                var path = HttpContext.Current.Server.MapPath("~/App_Data/ApiSettings.txt");
                if (!System.IO.File.Exists(path))
                {
                    var file = System.IO.File.Create(path);
                    file.Dispose();
                }
                using (StreamReader reader = new StreamReader(path))
                {
                    var jsonSettings = reader.ReadToEnd().Trim();
                    if (!String.IsNullOrEmpty(jsonSettings))
                        apiSettings = JsonConvert.DeserializeObject<APISettings>(jsonSettings);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return apiSettings;
        }
        public void SaveSettings()
        {
            var jsonConfig = JsonConvert.SerializeObject(this);
            var path = HttpContext.Current.Server.MapPath("~/App_Data/ApiSettings.txt");
            if (!System.IO.File.Exists(path))
            {
                var file = System.IO.File.Create(path);
                file.Dispose();
            }
            System.IO.File.WriteAllText(path, jsonConfig);
        }
    }


    //public interface IAPIHelper
    //{
    //    IPagedList<T> GetPagedListAsync<T>(string serviceName, string methodName, IDictionary<string, dynamic> parameters);
    //    IList<T> GetListAsync<T>(string serviceName, string methodName, IDictionary<string, dynamic> parameters);
    //    T GetAsync<T>(string serviceName, string methodName, IDictionary<string, dynamic> parameters);
    //    bool PostAsync(string serviceName, string methodName, object body, IDictionary<string, dynamic> parameters = null);
    //}
}
