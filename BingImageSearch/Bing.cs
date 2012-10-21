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

namespace BingImageSearch
{
    public sealed class Bing
    {
        const string AZURE_KEY = "C60eZha7o/m9iAjFvWCKvS3oJIjgtD/aDCPqV0nBe38=";

        public static IAsyncOperation<string> ImageSearchAsync(string query)
        {
            return ImageSearchAsyncInternal(query).AsAsyncOperation();
        }

        public static IAsyncOperation<JsonObject> ImageSearcJsonAsync(string query)
        {
            return ImageSearchJsonAsyncInternal(query).AsAsyncOperation();
        }

        internal async static Task<string> ImageSearchAsyncInternal(string query)
        {
            string uri = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/v1/Image?Query=%27{0}%27&$top=10&$format=Json";


            try
            {
                using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(AZURE_KEY, AZURE_KEY) })
                using (var http = new HttpClient(handler))
                {
                    return await http.GetStringAsync(new Uri(string.Format(uri, query)));
                }
            }
            catch (Exception e)
            {              
                return string.Empty;
            }
        }

        internal async static Task<JsonObject> ImageSearchJsonAsyncInternal(string query)
        {
            string result = await ImageSearchAsyncInternal(query);
            return JsonObject.Parse(result);
        }
    }
}
