using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using System;
using System.Linq;
using static Il2CppNinjaKiwi.GUTS.Models.BossRushRandomizerSettings;

namespace BloonFactory.Modules.Behaviors
{
    internal class AddChildrenModule : Module
    {
        public override string Name => "Add Children";
        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Bloon", Game.instance.model.bloons.Select(a => a.name).ToArray(), 0, new Action<int>((value) =>
            {
                SetValue(Game.instance.model.bloons[GetValue<int>("Bloon")].id, "BloonId");
            })));
            AddProperty(new IntModuleProperty("Count", 10, 0, int.MaxValue));

            if (!HasValue("BloonId"))
                SetValue("", "BloonId");
        }
        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
        }

        public override void ProcessModule()
        {
            try
            {
                string id = GetValue<string>("BloonId");
                if (Game.instance.model.bloons.Any(a => a.id == id))
                    GetInputValue<BloonModel>("Bloon").AddToChildren(id, GetValue<int>("Count"));
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to add action. {ex}");
            }

        }
    }
}
