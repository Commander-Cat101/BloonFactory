using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Sounds
{
    internal class SoundOnSpawnModule : Module
    {
        public override string Name => "Play Sound On Spawn";

        public override void GetModuleProperties()
        {
            AddProperty(new SpaceModuleProperty(200));
        }
        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
            AddInput<BloonSound>("Sound");
        }
        public override void ProcessModule()
        {
            var bloon = GetInputValue<BloonModel>("Bloon");

            bloon.AddBehavior(new CreateSoundOnSpawnBloonModel("CreateSoundOnSpawn", GetInputValue<BloonSound>("Sound").sound));
        }
    }
}
