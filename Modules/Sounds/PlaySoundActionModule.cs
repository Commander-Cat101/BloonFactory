using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace BloonFactory.Modules.Sounds
{
    internal class PlaySoundActionModule : Module
    {
        public override string Name => "Play Sound";

        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Delay", 0, 0, float.MaxValue));
            AddProperty(new SpaceModuleProperty(100));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
            AddInput<BloonSound>("Sound");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new CreateSoundOnActionModel("CreateSound_BloonFactory", Id.ToString(), new Il2CppReferenceArray<AudioClipReference>([GetInputValue<BloonSound>("Sound").sound]), GetValue<float>("Delay")));
        }
    }
}
