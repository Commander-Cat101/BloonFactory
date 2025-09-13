
using JetBrains.Annotations;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octokit;
using Octokit.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static BloonFactory.UI.BloonBrowserBloonPanel;

namespace BloonFactory
{
    internal static class ServerHandler
    {
        public const string URL = "http://localhost:8000/";
        private static HttpClient client = new HttpClient();
        internal static async Task Ping()
        {
            HttpResponseMessage response = await client.GetAsync(URL + "ping");
            response.EnsureSuccessStatusCode();
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            MelonLogger.Msg(Encoding.UTF8.GetString(bytes));
        }
        internal static async Task<PageUpdateRequest> RequestPageUpdate()
        {
            HttpResponseMessage response = await client.GetAsync(URL + "getPage");
            response.EnsureSuccessStatusCode();
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            var obj = JsonConvert.DeserializeObject<PageUpdateRequest>(Encoding.UTF8.GetString(bytes), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.None });
            return obj;
        }
        internal static async Task<BloonTemplate> DownloadTemplate(Guid guid)
        {
            HttpResponseMessage response = await client.GetAsync(URL + $"getTemplate={guid.ToString()}");
            response.EnsureSuccessStatusCode();
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            var obj = JsonConvert.DeserializeObject<BloonTemplate>(Encoding.UTF8.GetString(bytes), SerializationHandler.Settings);
            return obj;
        }
    }
    public class PageUpdateRequest
    {
        public List<BloonBrowserEntry> Data;
    }
    public class BloonBrowserEntry
    {
        public string Name;
        public Guid Guid;
        public int LikeCount;
        public int FileSize;
    }
}
