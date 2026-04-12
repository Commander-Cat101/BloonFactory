using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Data.Gameplay;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using Newtonsoft.Json;
using static BloonFactory.Modules.Sounds.Sounds;

namespace BloonFactory.Modules.Core
{
    internal class BloonModule : Module
    {
        public override string Name => "Bloon";

        public override bool IsDuplicable => false;
        public override bool IsRemovable => false;
        public override string Description => "Adds the base functionality of your custom bloon.";

        [JsonIgnore]
        public BloonModel currentModel;
        [JsonIgnore]
        public RoundSetModel currentRoundSet;
        public override void GetModuleProperties()
        {
            AddProperty(new IntModuleProperty("Health", 1, 1, int.MaxValue));
            AddProperty(new FloatModuleProperty("Speed", 25, 1, float.MaxValue));
            AddProperty(new IntModuleProperty("Damage", 1, 0, int.MaxValue));
        }
        public override void GetLinkNodes()
        {
            AddOutput<Visuals>("Visuals", () => new Visuals(currentModel));
            AddOutput<BloonModel>("Bloon", () => currentModel);
            AddOutput<RoundSetModel>("Roundset", () => currentRoundSet);
        }
        public override void ProcessModule()
        {
            currentModel.speed = GetValue<float>("Speed");
            currentModel.maxHealth = GetValue<int>("Health");
            currentModel.leakDamage = GetValue<int>("Damage");

            currentModel.AddBehavior(new CreateSoundOnDamageBloonModel("SoundOnDamage", MoabHitSounds));

            GetOutputsModules("Bloon").ProcessAll();
            GetOutputsModules("Visuals").ProcessAll();
            GetOutputsModules("Roundset").ProcessAll();
        }
    }
}
