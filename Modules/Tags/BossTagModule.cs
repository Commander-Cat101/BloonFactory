using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API.ModuleValues;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Boss;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.Races;
using MelonLoader;
using System;
using System.Linq;
using UnityEngine;

namespace BloonFactory.Modules.Tags
{
    internal class BossTagModule : Module
    {
        public override string Name => "Boss Tag";
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
            bloon.isBoss = true;
            bloon.AddTag(BloonTag.Boss);
        }
    }

    [HarmonyPatch(typeof(InGame), nameof(InGame.StartMatch))]
    internal static class InGame_StartMatch
    {
        [HarmonyPostfix]
        internal static void Postfix(InGame __instance)
        {
            if (InGameData.CurrentGame.bossData != null || InGameData.CurrentGame.bossRushData != null)
                return;

            __instance.InstantiateUiObject("BossUI", new System.Action<GameObject>(o =>
            {
                o.GetComponent<BossUI>().Initialise().StartCoroutine();
            })).StartCoroutine();
        }
    }

    [HarmonyPatch(typeof(UnityToSimulation), nameof(UnityToSimulation.GetBossBloon))]
    internal static class UnityToSimulation_GetBossBloon
    {
        [HarmonyPostfix]
        private static void Postfix(UnityToSimulation __instance, ref Bloon? __result)
        {
            if (InGameData.CurrentGame.bossData != null || InGameData.CurrentGame.bossRushData != null)
                return;

            __result ??= __instance.GetAllBloons().ToList().Where(b => b.GetBaseModel().isBoss).MaxBy(a => a.GetHealth())?.GetBloon();
        }
    }
    [HarmonyPatch(typeof(BossUI), nameof(BossUI.Update))]
    public static class  BossUI_OnUpdate
    {
        public static string currentGuid = "";
        [HarmonyPostfix]
        private static void Postfix(BossUI __instance)
        {
            if (InGameData.CurrentGame != null || InGameData.CurrentGame.bossData != null || InGameData.CurrentGame.bossRushData != null)
                return;

            if (__instance == null) return;

            __instance.noBossObj.transform.parent.gameObject.SetActive(false);

            if (__instance.TargetBloon != null && currentGuid != __instance.TargetBloon.bloonModel.icon.guidRef)
            {
                currentGuid = __instance.TargetBloon.bloonModel.icon.guidRef;
                __instance.bossImg.SetSprite(__instance.TargetBloon.bloonModel.icon);
            }
        }
    }
}
