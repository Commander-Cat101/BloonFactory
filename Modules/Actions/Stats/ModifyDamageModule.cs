using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Actions.Stats
{
    internal class ModifyDamageActionModule : Module
    {
        public static string BehaviorId => "ModifyDamageActionModule";
        public override string Name => "Modify Damage";

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
            trigger.bloonModel.AddBehavior(new SetPositionActionModel(BehaviorId, Id.ToString(), GetValue<int>("Type"), GetValue<int>("Value")));
        }
    }
}
