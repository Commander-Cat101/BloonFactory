using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BloonFactory.Modules.Triggers
{
    internal class LivesChangeTriggerModule : TriggerModule
    {
        public override string Name => "Lives Change Trigger";

        public const string BehaviorName = "LivesChangeTrigger";
        public override void GetModuleProperties()
        {
            AddProperty(new BoolModuleProperty("Trigger On Increase", true));
            AddProperty(new BoolModuleProperty("Trigger On Decrease", true));
            AddProperty(new FloatModuleProperty("Cooldown", 0f, 0f, float.MaxValue));
        }

        public override void GetLinkNodes()
        {
            AddOutput<Trigger>("Trigger", () => new Trigger(currentModel));
        }

        public override void ProcessModule()
        {
            var guids = new Il2CppStringArray(GetOutputsModules("Trigger").AsGuids());

            currentModel.AddBehavior(new HealthPercentTriggerModel(BehaviorName, false, new Il2CppStructArray<float>([GetValue<float>("Cooldown"), Convert.ToSingle(GetValue<bool>("Trigger On Decrease")), Convert.ToSingle(GetValue<bool>("Trigger On Increase"))]), guids, true));

            GetOutputsModules("Trigger").ProcessAll();
        }
        public static IEnumerator UpdateLogic(HealthPercentTrigger trigger)
        {
            double lastLives = InGame.instance.GetHealth();
            bool triggerOnIncrease = Convert.ToBoolean(trigger.modl.percentageValues.ToArray()[2]);
            bool triggerOnDecrease = Convert.ToBoolean(trigger.modl.percentageValues.ToArray()[1]);
            float cooldown = trigger.modl.percentageValues.ToArray()[0];

            float timeSinceLastTrigger = 0;

            while (InGame.instance != null && trigger.bloon != null)
            {
                timeSinceLastTrigger -= Time.deltaTime;

                if (InGame.instance == null)
                    yield break;

                double currentLives = InGame.instance.GetHealth();

                if (currentLives == lastLives)
                {
                    yield return null;
                    continue;
                }

                if (timeSinceLastTrigger > 0)
                {
                    yield return null;
                    continue;
                }

                bool shouldTriggerForIncrease = (currentLives > lastLives) && triggerOnIncrease;
                bool shouldTriggerForDecrease = (currentLives < lastLives) && triggerOnDecrease;

                if (shouldTriggerForIncrease || shouldTriggerForDecrease)
                {
                    MelonLogger.Msg("Triggering Lives Change Trigger");
                    timeSinceLastTrigger = cooldown;
                    trigger.Trigger();
                }

                lastLives = currentLives;
                yield return null;
            }
        }
    }

    [HarmonyPatch(typeof(HealthPercentTrigger), nameof(HealthPercentTrigger.Damage))]
    public static class LivesChangeTrigger_HealthPercentTrigger_OnBloonDamaged_Patch
    {
        public static bool Prefix(HealthPercentTrigger __instance)
        {
            if (__instance.modl.name.EndsWith(LivesChangeTriggerModule.BehaviorName))
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(HealthPercentTrigger), nameof(HealthPercentTrigger.Attatched))]
    public static class LivesChangeTrigger_HealthPercentTrigger_OnSpawn_Patch
    {
        public static void Postfix(HealthPercentTrigger __instance)
        {
            if (__instance.modl.name.Contains(LivesChangeTriggerModule.BehaviorName))
            {
                MelonCoroutines.Start(LivesChangeTriggerModule.UpdateLogic(__instance));
            }
        }
    }
}
