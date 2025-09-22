
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
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
using static BloonFactory.UI.BloonBrowserMenuPanel;

namespace BloonFactory
{
    internal static class ServerHandler
    {
        public const string URL = "https://server.bloonfactory.org/";
        private static HttpClient client = new HttpClient();
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
            MelonLogger.Msg($"Requested template from server ({guid.ToString()})");
            response.EnsureSuccessStatusCode();
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            var obj = JsonConvert.DeserializeObject<BloonTemplate>(Encoding.UTF8.GetString(bytes), SerializationHandler.Settings);
            return obj;
        }
        internal static async Task<(bool success, string errorCode)> UploadTemplate(BloonTemplate template, BloonCategory category, string description)
        {
            string creator = Game.Player.LiNKAccount?.DisplayName;
            if (string.IsNullOrEmpty(creator))
            {
                MelonLogger.Msg("You must be logged into a NK account to upload a template.");
                return (false, "You must be logged into a NK account to upload a template.");
            }
            MelonLogger.Msg($"Uploading template to server ({template.Name})");
            UploadTemplateRequest request = new UploadTemplateRequest()
            {
                Name = template.Name,
                Guid = template.Guid,
                Creator = creator,
                Category = (byte)category,
                Description = description,
                Version = ModHelperData.Version,
                TemplateJson = JsonConvert.SerializeObject(template, SerializationHandler.Settings)
            };

            HttpResponseMessage response = await client.PostAsync(URL + "uploadTemplate", new StringContent(JsonConvert.SerializeObject(request)));
            
            if (!response.IsSuccessStatusCode)
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                return (false, Encoding.UTF8.GetString(bytes));
            }
            return (true, "");
        }
    }
    public class PageUpdateRequest
    {
        public List<BloonBrowserEntry> Data;
    }
    public class UploadTemplateRequest
    {
        public string Name;
        public Guid Guid;
        public string Creator;
        public byte Category;
        public string Description;
        public string Version = "1.0.0";

        public string TemplateJson;
    }
    public class BloonBrowserEntry
    {
        public string Name;
        public Guid Guid;
        public string Creator;
        public byte Category;
        public string Description;
        public string Version = "1.0.0";

        public DateTime UploadTime = DateTime.Now;
        public int Downloads = 0;

        [JsonIgnore]
        public BloonCategory CategoryEnum => (BloonCategory)Category;
    }
    public enum BloonCategory : byte
    {
        Boss,
        VanillaPlus,
        Modded
    }
    public static class CategoryExtensions
    {
        public static string[] BloonCategoryNames =>
        [
            "Boss",
            "Vanilla+",
            "Modded"
        ];
        public static string ToFriendlyString(this BloonCategory category)
        {
            return category switch
            {
                BloonCategory.Boss => "Boss",
                BloonCategory.VanillaPlus => "Vanilla+",
                BloonCategory.Modded => "Modded",
                _ => "Unknown",
            };
        }
    }
}
