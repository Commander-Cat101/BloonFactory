using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors.Actions;
using Il2CppAssets.Scripts.Simulation.Bloons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Actions.Stats
{
    internal class ModifyPropertyActionModule : Module
    {
        public static string BehaviorId => "ModifyPropertiesActionModule";
        public override string Name => "Modify Properties";

        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Type", ["Add", "Remove"], 0));


            AddProperty(new EnumModuleProperty("Property", ["Lead", "Black", "White", "Purple", "Frozen", "Immune", "Glass"], 0));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new SetPositionActionModel(BehaviorId, Id.ToString(), GetValue<int>("Type"), GetValue<int>("Property")));
        }
    }
}
