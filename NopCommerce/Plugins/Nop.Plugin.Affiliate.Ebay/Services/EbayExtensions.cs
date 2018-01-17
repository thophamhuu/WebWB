using Newtonsoft.Json;
using Nop.Plugin.Affiliate.Ebay.Domain;
using Nop.Plugin.Affiliate.Ebay.Models;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.Ebay.Services
{
    public static class EbayExtensions
    {
        /// <summary>
        /// Get formatted category ebay breadcrumb 
        /// </summary>
        /// <param name="category">Category ebay</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        public static string GetFormattedBreadCrumbEbay(this CategoryEbay category, IList<CategoryEbay> allCategories, string separator = ">>", int languageId = 0)
        {
            string result = string.Empty;

            var breadcrumb = GetCategoryBreadCrumbEbay(category, allCategories, true);
            for (int i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var categoryName = breadcrumb[i].GetLocalized(x => x.Name, languageId);
                result = String.IsNullOrEmpty(result)
                    ? categoryName
                    : string.Format("{0} {1} {2}", result, separator, categoryName);
            }

            return result;
        }

        /// <summary>
        /// Get category ebay breadcrumb 
        /// </summary>
        /// <param name="category">Category ebay</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Category breadcrumb </returns>
        public static IList<CategoryEbay> GetCategoryBreadCrumbEbay(this CategoryEbay category, IList<CategoryEbay> allCategories, bool showHidden = false)
        {
            if (category == null)
                throw new ArgumentNullException("CategoryEbayRecord");

            var result = new List<CategoryEbay>();

            //used to prevent circular references
            var alreadyProcessedCategoryIds = new List<int>();

            while (category != null && //not null
                !category.Deleted && //not deleted
                (showHidden || category.Published) && //published          
                !alreadyProcessedCategoryIds.Contains(category.Id)) //prevent circular references
            {
                result.Add(category);

                alreadyProcessedCategoryIds.Add(category.Id);

                category = (from c in allCategories
                            where c.Id == category.ParentCategoryId
                            select c).FirstOrDefault();
            }
            result.Reverse();
            return result;
        }

        public static string GetValueFromAPI(string url, string method)
        {
            string apiERM = url;
            //Lấy giá trị từ API
            HttpWebRequest request = CreateRequest(apiERM);
            request.ContentType = "application/json charset=utf-8"; //"text/xml; charset=utf-8";
            request.Method = method; //WebRequestMethods.Http.Post;
            //request.ContentLength = 0;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string apiResult = GetResponse(response);

            return apiResult;
        }

        private static HttpWebRequest CreateRequest(string uriString)
        {
            WebRequest request = WebRequest.Create(uriString);
            (request as HttpWebRequest).UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 3.5.30729; .NET CLR 3.0.30618)";
            (request as HttpWebRequest).CookieContainer = new CookieContainer();
            (request as HttpWebRequest).ReadWriteTimeout = 30 * 1000;
            (request as HttpWebRequest).Timeout = 30 * 1000;
            return (HttpWebRequest)request;
        }

        private static string GetResponse(HttpWebResponse response)
        {
            string result = string.Empty;
            System.IO.Stream stream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(stream);
            result = reader.ReadToEnd();
            response.Close();
            stream.Close();
            reader.Close();
            return result;
        }

        public static System.Drawing.Image DownloadImage(string imageUrl)
        {
            System.Drawing.Image image = null;

            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                image = System.Drawing.Image.FromStream(stream);

                webResponse.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            return image;
        }

        public static byte[] ImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        /// <summary>
        /// Sort Category Ebay for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted categories</returns>
        public static IList<CategoryEbay> SortCategoriesEbayForTree(this IList<CategoryEbay> source, int parentId = 0, bool ignoreCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var result = new List<CategoryEbay>();

            foreach (var cat in source.Where(c => c.ParentCategoryId == parentId).ToList())
            {
                result.Add(cat);
                result.AddRange(SortCategoriesEbayForTree(source, cat.Id, true));
            }
            if (!ignoreCategoriesWithoutExistingParent && result.Count != source.Count)
            {
                //find categories without parent in provided category source and insert them into result
                foreach (var cat in source)
                    if (result.FirstOrDefault(x => x.Id == cat.Id) == null)
                        result.Add(cat);
            }
            return result;
        }

        public static string GetToken()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.ebay.com/");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "UGh1b2NQaGEtd29ybGRidXktUFJELTI4ZTM1YzUzNS05NGIyYmI0YjpQUkQtOGUzNWM1MzU2ZjBmLTE0NDktNDI1YS1hNjM4LTYzMjI=");

            var request = new HttpRequestMessage(HttpMethod.Post, "identity/v1/oauth2/token");
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            keyValues.Add(new KeyValuePair<string, string>("refresh_token", "v^1.1#i^1#I^3#f^0#r^1#p^3#t^Ul4xMF8zOjk1Q0EzRkIxQkREQThCNERCMDRGOEMxNkZDNjk2RkM0XzFfMSNFXjI2MA=="));
            keyValues.Add(new KeyValuePair<string, string>("scope", "https://api.ebay.com/oauth/api_scope"));

            request.Content = new FormUrlEncodedContent(keyValues);
            HttpResponseMessage response = client.SendAsync(request).Result;
            var tokenResult = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<OAuth>(tokenResult);

            return result.access_token;
        }
    }
}
