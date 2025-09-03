using FactoryCore.API;
using Il2CppAssets.Scripts.Models.Bloons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules
{
    internal abstract class TriggerModule : Module
    {
        [JsonIgnore]
        public BloonModel currentModel;
    }
}
