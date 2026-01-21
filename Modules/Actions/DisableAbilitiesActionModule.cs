using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;

namespace BloonFactory.Modules.Actions
{
    internal class DisableAbilitiesActionModule : Module
    {
        public override string Name => "Disable Abilities";

        public const string BehaviorName = "DisableAbilitiesActionModule";

        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Distance", 25, float.MinValue, float.MaxValue));
            AddProperty(new FloatModuleProperty("Duration", 3, float.MinValue, float.MaxValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new StunTowersInRadiusActionModel(BehaviorName, Id.ToString(), GetValue<float>("Distance"), GetValue<float>("Duration"), 0.3f, new PrefabReference("289f511b736a06a4c993b9e0e73d2b8a"), false));
            
        }

        /*public class DisableAbilitiesActionModulePatches
        {
            [HarmonyPatch(typeof(StunTowersInRadiusAction), nameof(StunTowersInRadiusAction.PerformAction))]
            public static bool Prefix(StunTowersInRadiusAction __instance)
            {
                if (__instance.modl.name == BehaviorName)
                {
                    foreach (var tower in __instance)
                    {
                        var ability = tower.GetAbilities().FirstOrDefault(a => a.isActivated);
                        if (ability != null)
                        {
                            ability.SetOnCooldown(__instance.modl.duration);
                            ability.DeactivateAbility();
                        }
                    }
                    return false;
                }
                return true;
            }
        }*/
    }
}
