using AutoWrapper.Wrappers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace V2Ray.Api.Test.Utils
{
    public static class ExtensionMethods
    {
        public static Task<T> GetAndDeserialize<T>(this HttpClient client, string requestUri)
        {
            return client.GetFromJsonAsync<T>(requestUri);
        }

        public static async Task<TOut> DeserializeApiResponse<TOut>(this HttpClient client, string requestUri)
        {
            var resp = await client.GetFromJsonAsync<ApiResponse>(requestUri);
            return JsonConvert.DeserializeObject<TOut>(resp.Result.ToString());
        }

        public static string GetQueryString(this object obj, string route, string baseRoute = "/api/")
        {
            var properties = from p in obj.GetType().GetProperties()
                             where !string.IsNullOrEmpty(p.GetValue(obj, null)?.ToString())
                             && p.PropertyType != typeof(int[])
                             select p.Name.ToLower() + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null)?.ToString());

            var queryString = string.Join("&", properties.ToArray());
            return $"{baseRoute}{route}?{queryString}";
        }

        public static async Task<T> ConvertToModel<T>(this HttpResponseMessage apiResponse)
        {
            var str = await apiResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(str);
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
}