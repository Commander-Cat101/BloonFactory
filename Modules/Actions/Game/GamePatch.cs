using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppAssets.Scripts.Simulation.Simulation;

namespace BloonFactory.Modules.Actions.Game
{
    [HarmonyPatch(typeof(BuffBloonsInRadiusAction), nameof(BuffBloonsInRadiusAction.PerformAction))]
    public class GamePatch
    {
        public static bool Prefix(BuffBloonsInRadiusAction __instance)
        {
            var model = __instance.modl;

            int type = (int)model.radius;
            int value = (int)model.speedMultiplier;

            if (model.name.Contains(ModifyCashActionModule.BehaviorId))
            {
                if (type == 0)
                {
                    InGame.instance.bridge.AddCash(value, CashSource.Normal);
                }
                else if (type == 1)
                {
                    InGame.instance.bridge.SetCash(InGame.instance.bridge.GetCash() - value);
                }
                else if (type == 2)
                {
                    InGame.instance.bridge.SetCash(value);
                }
                return false;
            }
            if (model.name.Contains(ModifyLivesActionModule.BehaviorId))
            {
                if (type == 0)
                {
                    InGame.instance.bridge.simulation.health.Value += value;
                }
                else if (type == 1)
                {
                    InGame.instance.bridge.simulation.TakeDamage(value);
                }
                else if (type == 2)
                {
                    InGame.instance.SetHealth(value);
                }
                return false;
            }

            return true;
        }
    }
}
