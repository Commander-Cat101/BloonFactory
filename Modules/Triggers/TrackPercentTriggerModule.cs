using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Triggers
{
    internal class TrackPercentTriggerModule : TriggerModule
    {
        public override string Name => "Track Percent Trigger";

        public const string BehaviorName = "TrackPercentTrigger";
        public override void GetModuleProperties()
        {
            AddProperty(new IntSliderModuleProperty("Percentage", 50, 0, 100));
        }

        public override void GetLinkNodes()
        {
            AddOutput<Trigger>("Trigger", () => new Trigger(currentModel));
        }

        public override void ProcessModule()
        {
            var guids = new Il2CppStringArray(GetOutputsModules("Trigger").AsGuids());

            float value = (float)GetValue<int>("Percentage") / 100;
            currentModel.AddBehavior(new HealthPercentTriggerModel(BehaviorName, false, new float[] { value }, guids, true));

            GetOutputsModules("Trigger").ProcessAll();
        }
        public static IEnumerator UpdateLogic(HealthPercentTrigger trigger)
        {
            trigger.currentSkull = 0;
            float requiredPercThroughMap = trigger.modl.percentageValues.First();

            while (InGame.instance != null && trigger.bloon != null)
            {
                if (trigger.bloon.path == null)
                    yield return null;

                float percThroughMap = trigger.bloon.distanceTraveled / trigger.bloon.path.totalPathLength;

                if (percThroughMap >= requiredPercThroughMap)
                {
                    MelonLogger.Msg("Triggering.");
                    trigger.Trigger();
                    break;
                }
                yield return null;
            }
        }
    }
    [HarmonyPatch(typeof(HealthPercentTrigger), nameof(HealthPercentTrigger.Damage))]
    public static class HealthPercentTrigger_OnBloonDamaged_Patch
    {
        public static bool Prefix(HealthPercentTrigger __instance)
        {
            if (__instance.modl.name.EndsWith(TrackPercentTriggerModule.BehaviorName))
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(HealthPercentTrigger), nameof(HealthPercentTrigger.Attatched))]
    public static class HealthPercentTrigger_OnSpawn_Patch
    {
        public static void Postfix(HealthPercentTrigger __instance)
        {
            MelonLogger.Msg("Got spawned");
            if (__instance.modl.name.EndsWith(TrackPercentTriggerModule.BehaviorName))
            {
                MelonLogger.Msg("Started coroutine");
                MelonCoroutines.Start(TrackPercentTriggerModule.UpdateLogic(__instance));
            }
        }
    }
}
