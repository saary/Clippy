using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net;
using System.Diagnostics;

namespace BingImageSearch
{
    public sealed class Bing
    {
        const string AZURE_KEY = "C60eZha7o/m9iAjFvWCKvS3oJIjgtD/aDCPqV0nBe38=";

        public static IAsyncOperation<string> ImageSearchAsync(string query)
        {
            return ImageSearchAsyncInternal(query, 10).AsAsyncOperation();
        }

        public static IAsyncOperation<JsonObject> ImageSearcJsonAsync(string query)
        {
            return ImageSearchJsonAsyncInternal(query, 10).AsAsyncOperation();
        }

        public static IAsyncOperation<string> ImageSearchAsync(string query, int limit)
        {
            return ImageSearchAsyncInternal(query, limit).AsAsyncOperation();
        }

        public static IAsyncOperation<JsonObject> ImageSearcJsonAsync(string query, int limit)
        {
            return ImageSearchJsonAsyncInternal(query, limit).AsAsyncOperation();
        }

        internal async static Task<string> ImageSearchAsyncInternal(string query, int limit)
        {
            string uri = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/v1/Image?Query=%27{0}%27&$top={1}&$format=Json";

            try
            {
                using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(AZURE_KEY, AZURE_KEY) })
                using (var http = new HttpClient(handler))
                {
                    return await http.GetStringAsync(new Uri(string.Format(uri, query, limit)));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return string.Empty;
            }
        }

        internal async static Task<JsonObject> ImageSearchJsonAsyncInternal(string query, int limit)
        {
            string result = await ImageSearchAsyncInternal(query, limit);
            return JsonObject.Parse(result);
        }
    }
}
