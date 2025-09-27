using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;

namespace BloonFactory.Modules.Tags
{
    internal class BadTagModule : Module
    {
        public override string Name => "Bad Tag";

        public override void GetModuleProperties()
        {
            AddProperty(new SpaceModuleProperty(100));
        }
        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
        }
        public override void ProcessModule()
        {
            var bloon = GetInputValue<BloonModel>("Bloon");
            bloon.AddTag(BloonTag.Bad);
            bloon.AddBehavior(new BadImmunityModel("BadImmunityModel"));
        }
    }
}
