using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.BloonMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Patches
{
    [HarmonyPatch(typeof(BloonMenu), nameof(BloonMenu.CreateBloonButtons))]
    public static class BloonMenu_CreateBloonButtons_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(BloonMenu __instance, Il2CppSystem.Collections.Generic.List<BloonModel> sortedBloons)
        {
            foreach (var bloon in CustomBloon.Bloons)
            {
                if (!sortedBloons.Any(a => a.id == bloon.BloonTemplate.TemplateId))
                {
                    sortedBloons.Add(Game.instance.model.GetBloon(bloon.BloonTemplate.TemplateId));
                }
            }
        }
    }
}
