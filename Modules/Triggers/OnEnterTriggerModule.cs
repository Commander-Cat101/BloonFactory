using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloonFactory.Modules.Triggers
{
    internal class OnEnterTriggerModule : TriggerModule
    {
        public override string Name => "On Enter Trigger";

        public override string Description => "Triggers when the bloon enters.";

        public const string BehaviorName = "OnEnterTriggerModel";
        public override void GetModuleProperties()
        {
            AddProperty(new SpaceModuleProperty(100));
        }

        public override void GetLinkNodes()
        {
            AddOutput<Trigger>("Trigger", () => new Trigger(currentModel));
        }

        public override void ProcessModule()
        {
            var guids = new Il2CppStringArray(GetOutputsModules("Trigger").AsGuids());
            currentModel.AddBehavior(new HealthPercentTriggerModel(BehaviorName, false, new Il2CppStructArray<float>([]), guids, true));

            GetOutputsModules("Trigger").ProcessAll();
        }
    }
    [HarmonyPatch(typeof(HealthPercentTrigger), nameof(HealthPercentTrigger.Damage))]
    public class HealthPercentTrigger_Damage_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(HealthPercentTrigger __instance)
        {
            if (__instance.modl.name.EndsWith(OnEnterTriggerModule.BehaviorName))
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(HealthPercentTrigger), nameof(HealthPercentTrigger.Attatched))]
    public class HealthPercentTrigger_Attached_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(HealthPercentTrigger __instance)
        {
            if (__instance.modl.name.EndsWith(OnEnterTriggerModule.BehaviorName))
            {
                MelonCoroutines.Start(Logic(__instance));
            }
        }

        public static IEnumerator Logic(HealthPercentTrigger __instance)
        {
            yield return new WaitForSeconds(0.5f);
            __instance.Trigger();
        }
    }
}
