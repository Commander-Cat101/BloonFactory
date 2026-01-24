using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Actions.Game
{
    internal class ModifyCashActionModule : Module
    {
        public static string BehaviorId => "ModifyCashActionModule";
        public override string Name => "Modify Cash";

        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Type", ["Add", "Subtract", "Set"], 0));
            AddProperty(new IntModuleProperty("Value", 0, int.MinValue, int.MaxValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new BuffBloonsInRadiusActionModel(BehaviorId, Id.ToString(), GetValue<int>("Type"), GetValue<int>("Value")));
        }
    }
}
