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
using System.Web;
using static BloonFactory.UI.BloonBrowserMenuPanel;

namespace BloonFactory.Handlers
{
    internal static class ServerHandler
    {
        public const string URL = "http://localhost:8000/";
        private static HttpClient client = new();
        internal static async Task<BloonBrowserResponse> RequestBloonBrowserContent(bool includeUnverified)
        {
            HttpResponseMessage response = await client.GetAsync(URL + $"requestBrowser?{includeUnverified}");
            response.EnsureSuccessStatusCode();
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            var obj = JsonConvert.DeserializeObject<BloonBrowserResponse>(Encoding.UTF8.GetString(bytes));
            return obj;
        }
        internal static async Task<BloonTemplate> DownloadTemplate(string uuid)
        {
            HttpResponseMessage response = await client.GetAsync(URL + $"getTemplate?{uuid}");
            MelonLogger.Msg($"Requested template from server ({uuid})");
            response.EnsureSuccessStatusCode();
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            string jsonData = Encoding.UTF8.GetString(bytes);

            if (jsonData == string.Empty)
                return null;
            
            Console.WriteLine(jsonData);
            
            try
            {
                var obj = JsonConvert.DeserializeObject<BloonTemplate>(jsonData, SerializationHandler.Settings);
                return obj;
            }
            catch (Exception ex)
            {
                MelonLogger.Msg(ex);
            }
            return null;
        }

        internal static async Task<(bool success, string errorCode)> UploadTemplate(BloonTemplate template, BloonCategory category)
        {
            if (!Game.TryGetPlayerShortCode(out string code))
            {
                return (false, "Unable to get user code, are you signed into a NK account?");
            }
            
            UploadBloonRequest request = new() { shortCode = code, serializedTemplate = JsonConvert.SerializeObject(template, SerializationHandler.Settings), modVersion = ModHelperData.Version, category = category };
            
            string serializedRequest = JsonConvert.SerializeObject(request, SerializationHandler.Settings);

            (bool success, string errorCode) result = (false, "An error occured in processing the request.");
            
            try
            {
                HttpResponseMessage response = await client.PostAsync(URL + "uploadTemplate",
                    new StringContent(serializedRequest, Encoding.UTF8, "application/json"));
                    
                var serializedResponse = await response.Content.ReadAsByteArrayAsync();
                    
                var deserializedResponse =
                    JsonConvert.DeserializeObject<UploadBloonResponse>(
                        Encoding.UTF8.GetString(serializedResponse), SerializationHandler.Settings);
                    
                if (deserializedResponse.success && deserializedResponse.uploadData != null)
                {
                    template.uploadData = deserializedResponse.uploadData;
                    SerializationHandler.SaveTemplate(template);
                }
                result = (deserializedResponse.success, deserializedResponse.errorCode);
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
            }
            return result;
        }

        internal static async Task<(bool isLiked, string errorCode)> LikeTemplate(BloonTemplate template)
        {
            if (!Game.TryGetPlayerShortCode(out string code))
            {
                return (false, "Unable to get user code, are you signed into a NK account?");
            }

            var request = new LikeRequest()
            {
                shortCode = code,
                templateUUID = template.GetUUID()
            };
            
            string serializedRequest = JsonConvert.SerializeObject(request);

            try
            {
                HttpResponseMessage response = await client.PostAsync(URL + "likeTemplate",
                    new StringContent(serializedRequest, Encoding.UTF8, "application/json"));

                var serializedResponse = await response.Content.ReadAsByteArrayAsync();
                var deserializedResponse =
                    JsonConvert.DeserializeObject<(bool isLiked, string errorCode)>(
                        Encoding.UTF8.GetString(serializedResponse));

                return deserializedResponse;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
            }
            
            return (false, "An error occured in processing the request.");
        }
        internal static async Task<(bool success, string errorCode)> ReportTemplate(ReportFlags flags,
            BloonTemplate template)
        {
            if (!Game.TryGetPlayerShortCode(out string code))
            {
                return (false, "Unable to get user code, are you signed into a NK account?");
            }
            
            var request = new ReportRequest
            {
                ReportFlags = flags,
                ShortCode = code,
                TemplateUUID = template.GetUUID()
            };
            
            string serializedRequest = JsonConvert.SerializeObject(request, SerializationHandler.Settings);
            
            (bool success, string errorCode) result = (false, "An error occured in processing the request.");
            
            try
            {
                HttpResponseMessage response = await client.PostAsync(URL + "reportTemplate", 
                    new StringContent(serializedRequest, Encoding.UTF8, "application/json"));
                
                var serializedResponse = await response.Content.ReadAsByteArrayAsync();
                var deserializedResponse =
                    JsonConvert.DeserializeObject<(bool success, string errorCode)>(
                        Encoding.UTF8.GetString(serializedResponse), SerializationHandler.Settings);
                
                return deserializedResponse;
            }
            catch {}
            return result;
        }

        internal static async Task<(bool success, string errorCode)> RateTemplate(RateRequest request)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync(URL + "rateTemplate",
                    new StringContent(JsonConvert.SerializeObject(request, SerializationHandler.Settings), Encoding.UTF8, "application/json"));
                
                var serializedResponse = await response.Content.ReadAsByteArrayAsync();
                var deserializedResponse = JsonConvert.DeserializeObject<(bool success, string errorCode)>(
                    Encoding.UTF8.GetString(serializedResponse), SerializationHandler.Settings);
                
                return deserializedResponse;
            }
            catch {}

            return (false, "An error occured in processing the request.");
        }
        internal static async Task<(bool success, string errorCode)> DeleteTemplate(DeleteRequest request)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync(URL + "deleteTemplate",
                    new StringContent(JsonConvert.SerializeObject(request, SerializationHandler.Settings), Encoding.UTF8, "application/json"));
                
                var serializedResponse = await response.Content.ReadAsByteArrayAsync();
                var deserializedResponse = JsonConvert.DeserializeObject<(bool success, string errorCode)>(
                    Encoding.UTF8.GetString(serializedResponse), SerializationHandler.Settings);
                
                return deserializedResponse;
            }
            catch {}

            return (false, "An error occured in processing the request.");
        }
        internal static async Task<(bool success, string errorCode)> BanTemplate(BanRequest request)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync(URL + "banTemplate",
                    new StringContent(JsonConvert.SerializeObject(request, SerializationHandler.Settings), Encoding.UTF8, "application/json"));
                
                var serializedResponse = await response.Content.ReadAsByteArrayAsync();
                var deserializedResponse = JsonConvert.DeserializeObject<(bool success, string errorCode)>(
                    Encoding.UTF8.GetString(serializedResponse), SerializationHandler.Settings);
                
                return deserializedResponse;
            }
            catch {}

            return (false, "An error occured in processing the request.");
        }
        internal struct BanRequest
        {
            public string loginKey;
            public string templateUUIDOrShortCode;
        }
        internal struct DeleteRequest
        {
            public string loginKey;
            public string templateUUID;
            public bool allByUser;
        }
        internal struct RateRequest
        {
            public string loginKey;
            public string templateUUID;
            public int verificationStatus;
        }
        private struct LikeRequest
        {
            public string shortCode;
            public string templateUUID;
        }
        private struct UploadBloonRequest
        {
            public string shortCode;
            public string modVersion;
            public BloonCategory category;
            public string serializedTemplate;
        }
        private struct UploadBloonResponse
        {
            public bool success;
            public string errorCode;
            public UploadData? uploadData;
        }

        private struct ReportRequest
        {
            public ReportFlags ReportFlags;
            public string ShortCode;
            public string TemplateUUID;
        }
        internal struct BloonBrowserResponse
        {
            public List<BloonBrowserEntry> Data;
        }
    }
    internal struct BloonBrowserEntry
    {
        public UploadData uploadData;
        public int LikeCount;
        public int DownloadCount;
        public VerificationStatus VerificationStatus;
    }
    public enum VerificationStatus : byte
    {
        Unverified,
        Verified,
        Excellent
    }
    [Flags]
    public enum ReportFlags : byte
    {
        None = 0,
        OffensiveText = 1,
        OffensiveAssets = 2,
        Spam = 4,
        Other = 8
    }
    public enum BloonCategory : byte
    {
        None,
        VanillaPlus,
        Boss,
        Modded
    }
    public static class CategoryExtensions
    {
        public static string[] BloonCategoryNames =>
        [
            BloonCategory.None.ToFriendlyString(),
            BloonCategory.VanillaPlus.ToFriendlyString(),
            BloonCategory.Boss.ToFriendlyString(),
            BloonCategory.Modded.ToFriendlyString()
        ];
        public static string ToFriendlyString(this BloonCategory category)
        {
            return category switch
            {
                BloonCategory.Boss => "Boss",
                BloonCategory.VanillaPlus => "Vanilla+",
                BloonCategory.Modded => "Modded",
                BloonCategory.None => "None",
            };
        }
    }
}
