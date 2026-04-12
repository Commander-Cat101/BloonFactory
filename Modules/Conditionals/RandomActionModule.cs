using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API.ModuleValues;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Linq;
namespace BloonFactory.Modules.Conditionals
{
    internal class RandomActionModule : Module
    {
        public const string BehaviorId = "RandomActionModel";
        public override string Name => "Random";

        public override string Description => "Generates a random number between 0 and Max Value (inclusive) and if the number is over Threshold will trigger the action";
        public override void GetModuleProperties()
        {
            AddProperty(new IntModuleProperty("Max Value", 2, 1, int.MaxValue));
            AddProperty(new IntModuleProperty("Threshold", 1, 0, int.MaxValue));
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

            trigger.bloonModel.AddBehavior(new WaitForSecondsActionModel(BehaviorId + $"{GetValue<int>("Threshold")}", GetValue<int>("Max Value"), Id.ToString(), guids)); 
            GetOutputsModules("Trigger").Where(a => !trigger.TriggeredModules.Contains(a)).ProcessAll();
        }

        [HarmonyPatch(typeof(WaitForSecondsAction), nameof(WaitForSecondsAction.PerformAction))]
        public class WaitForSecondsAction_PerformAction
        {
            public static bool Prefix(WaitForSecondsAction __instance)
            {
                if (!__instance.waitForSecondsActionModel.name.Contains(BehaviorId))
                    return true;

                var variable = __instance.waitForSecondsActionModel.name.Substring(__instance.waitForSecondsActionModel.name.IndexOf(BehaviorId) + BehaviorId.Length);

                int threshold = int.Parse(variable);
                int maxValue = (int)__instance.waitForSecondsActionModel.delayTime;

                Random rand = new Random();

                if (rand.Next(maxValue + 1) > threshold)
                {
                    __instance.TriggerActions();
                }
                return false;
            }
        }
    }
}
