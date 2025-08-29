using FactoryCore.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory
{
    internal class BloonTemplate : Template
    {
        public Guid Guid;

        public string Name;

        [JsonIgnore]
        public string TemplateId => Guid.ToString();
        [JsonIgnore]
        public bool IsLoaded = true;
        [JsonIgnore]
        public bool IsQueueForDeletion = false;
    }
}
