using Newtonsoft.Json;
using System;

namespace BloonFactory
{
    internal class BloonTemplate : Template
    {
        public Guid Guid;

        public string Name;
        public DateTime CreationTime;
        public string UUID;
        public string Description;

        public UploadData? uploadData;
        
        [JsonIgnore]
        public string TemplateId => Guid.ToString();
        [JsonIgnore]
        public bool IsLoaded = true;
        [JsonIgnore]
        public bool IsQueueForDeletion = false;
        [JsonIgnore]
        public bool IsDownloaded => uploadData != null;
        public string GetName() => !IsDownloaded ? Name : uploadData?.Name;
        public string GetDescription() => !IsDownloaded ? Description : uploadData?.Description;
        public string GetUUID() => !IsDownloaded ? UUID : uploadData?.UUID;
    }
    internal struct UploadData
    {
        public string Name { get; }   
        public string Author { get; }
        public DateTime UploadTime { get; }
        public string UUID { get; }
        public string Description { get; }
        
        public string ModVersion { get; }
    }
}
