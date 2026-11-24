using Newtonsoft.Json;
using System;
using BloonFactory.Handlers;

namespace BloonFactory
{
    internal class BloonTemplate : Template
    { 
        public string Name;
        public DateTime CreationTime;
        public string UUID;
        public bool HasBeenLiked;
        public string Description;

        public UploadData? uploadData;
        
        [JsonIgnore]
        public string TemplateId => UUID;
        [JsonIgnore]
        public bool IsDownloaded => uploadData != null;
        public string GetName() => !IsDownloaded ? Name : uploadData?.Name;
        public string GetDescription() => !IsDownloaded ? Description : uploadData?.Description;
        public string GetUUID() => !IsDownloaded ? UUID : uploadData?.UUID;
    }
    [JsonObject(MemberSerialization.OptOut)]
    internal struct UploadData
    {
        [JsonProperty] public string Name;
        [JsonProperty] public string Author;
        [JsonProperty] public DateTime UploadTime;
        [JsonProperty] public string UUID;
        [JsonProperty] public string Description;
        [JsonProperty] public string ModVersion;
        [JsonProperty] public BloonCategory Category;
        public override string ToString()
        {
            return $"Name: {Name}, Author: {Author}, UploadTime: {UploadTime}, UUID: {UUID}, Description: {Description}, ModVersion: {ModVersion}";
        }
    }
}
