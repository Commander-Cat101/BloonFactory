using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppAssets.Scripts.Utils.ObjectCache;

namespace BloonFactory.Modules.Conditionals
{
    internal class CompareActionModule : Module
    {
        public const string BehaviorId = "CompareActionModel";
        public override string Name => "Compare";
        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Value 1", ["Lives", "Cash", "Round", "Track Percentage", "Bloon Health"], 0));
            AddProperty(new EnumModuleProperty("Compare Type", ["Greater Than", "Less Than", "Equal"], 0));
            AddProperty(new IntModuleProperty("Value 2", 1, 0, int.MaxValue));

        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
            AddOutput<Trigger>("Trigger", () => GetInputValue<Trigger>("Trigger").With(this));
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            var guids = new Il2CppStringArray(GetOutputsModules("Trigger").AsGuids());

            trigger.bloonModel.AddBehavior(new WaitForSecondsActionModel(BehaviorId + $"{GetValue<int>("Value 1")}-{GetValue<int>("Compare Type")}", GetValue<int>("Value 2"), Id.ToString(), guids)); 
            GetOutputsModules("Trigger").Where(a => !trigger.TriggeredModules.Contains(a)).ProcessAll();
        }

        [HarmonyPatch(typeof(WaitForSecondsAction), nameof(WaitForSecondsAction.PerformAction))]
        public class WaitForSecondsAction_PerformAction
        {
            public static bool Prefix(WaitForSecondsAction __instance)
            {
                if (!__instance.waitForSecondsActionModel.name.Contains(BehaviorId))
                    return true;

                var step1 = __instance.waitForSecondsActionModel.name.Substring(__instance.waitForSecondsActionModel.name.IndexOf(BehaviorId) + BehaviorId.Length);
                var variables = step1.Split('-');

                int value1 = 0;

                switch (variables[0])
                {   case "0":
                        value1 = (int)InGame.instance.GetHealth();
                        break;
                    case "1":
                        value1 = (int)InGame.instance.GetCash();
                        break;
                    case "2":
                        value1 = (int)InGame.instance.bridge.GetCurrentRound();
                        break;
                    case "3":
                        float percThroughMap = __instance.bloon.distanceTraveled / __instance.bloon.path.totalPathLength;
                        value1 = (int)(percThroughMap * 100);
                        break;
                    case "4":
                        value1 = (int)__instance.bloon.Health;
                        break;
                }

                int value2 = (int)__instance.waitForSecondsActionModel.delayTime;

                bool result = false;

                if (variables[1] == "0") 
                {
                    result = value1 > value2;
                }
                else if (variables[1] == "1") 
                {
                    result = value1 < value2;
                }
                else if (variables[1] == "2")
                {
                    result = value1 == value2;
                }

                if (result)
                {
                    __instance.TriggerActions();
                }

                return false;
            }
        }
    }
}
