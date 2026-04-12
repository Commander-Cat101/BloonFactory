using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Objects;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace BloonFactory.Modules.Actions.Stats
{
    [HarmonyPatch(typeof(SetPositionAction), nameof(SetPositionAction.PerformAction))]
    public class HealBloonAction_PerformAction_Patch
    {
        public static bool Prefix(SetPositionAction __instance)
        {
            SetPositionActionModel healModel = __instance.setPosActionModel;

            var name = healModel.name;

            if (__instance.bloon == null || __instance.bloon.rootModel == null)
                return true;

            var rootModel = __instance.bloon.rootModel.Duplicate().Cast<BloonModel>();

            if (name.Contains(ModifyDamageActionModule.BehaviorId))
            {
                int type = (int)healModel.distance;
                int value = (int)healModel.speed;
                if (type == 0)
                {
                    rootModel.leakDamage += value;
                }
                else if (type == 1)
                {
                    rootModel.leakDamage -= value;
                }
                else if (type == 2)
                {
                    rootModel.leakDamage = value;
                }
                __instance.bloon.UpdateRootModel(rootModel);
                return false;
            }
            if (name.Contains(ModifySpeedActionModule.BehaviorId))
            {
                int type = (int)healModel.distance;
                int value = (int)healModel.speed;
                if (type == 0)
                {
                    rootModel.speed += value;
                }
                else if (type == 1)
                {
                    rootModel.speed -= value;
                }
                else if (type == 2)
                {
                    rootModel.speed = value;
                }
                __instance.bloon.UpdateRootModel(rootModel);
                return false;
            }
            if (name.Contains(ModifyHealthActionModule.BehaviorId))
            {
                int type = (int)healModel.distance;
                int value = (int)healModel.speed;
                if (type == 0)
                {
                    __instance.bloon.SetHealth(__instance.bloon.health + value);
                }
                else if (type == 1)
                {
                    __instance.bloon.SetHealth(__instance.bloon.health - value);
                }
                else if (type == 2)
                {
                    __instance.bloon.SetHealth(value);
                }

                return false;
            }
            if (name.Contains(ModifyPropertyActionModule.BehaviorId))
            {
                MelonLogger.Msg(rootModel.bloonProperties.ToString());
                int type = (int)healModel.distance;
                BloonProperties properties = (BloonProperties)MathF.Pow(2, (int)healModel.speed);

                if (type == 0)
                {
                    if (!rootModel.bloonProperties.HasFlag(properties))
                        rootModel.bloonProperties |= properties;
                }
                else if (type == 1)
                {
                    if (rootModel.bloonProperties.HasFlag(properties))
                        rootModel.bloonProperties &= ~properties;
                }
                __instance.bloon.UpdateRootModel(rootModel);
                return false;
            }

            return true;
        }
    }
}
