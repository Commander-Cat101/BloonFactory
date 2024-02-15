using BloonFactory.API.Bloons;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Knowledge;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Towers.Upgrades;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppInterop.Runtime;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System;

namespace BloonFactory.API.Patches
{
    [HarmonyPatch(typeof(GameModel), nameof(GameModel.CreateModded), typeof(GameModel), typeof(List<ModModel>), typeof(List<RelicKnowledgeItemBase>))]
    public static class CreateModdedPatch
    {
        //From modhelper, thanks doom
        [HarmonyPrefix]
        public static bool Prefix()
        {
            try
            {
                Game.instance.model.searchCache ??= new Dictionary<Il2CppSystem.Type, Dictionary<string, Model>>
                {
                    [Il2CppType.Of<BloonModel>()] = new(),
                };

                var bloonCache = Game.instance.model.searchCache[Il2CppType.Of<BloonModel>()];
                foreach (var (key, value) in CustomBloon.BloonCache)
                {
                    if (!bloonCache.ContainsKey(key))
                    {
                        bloonCache[key] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Log(ex);
            }
            

            return true;
        }
    }
}
