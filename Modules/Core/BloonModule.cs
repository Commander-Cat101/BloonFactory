using BloonFactory.LinkTypes;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Core
{
    internal class BloonModule : Module
    {
        public override string Name => "Bloon";

        public override void GetModuleProperties()
        {
            AddProperty(new StringModuleProperty("Name", "Custom Bloon", 20));
            AddProperty(new IntModuleProperty("Health", 1, 1, int.MaxValue));
            AddProperty(new FloatModuleProperty("Speed", 25, 1, float.MaxValue));
            AddProperty(new IntModuleProperty("Damage", 1, 1, int.MaxValue));
        }
        public override void GetLinkNodes()
        {
            AddOutput<Visuals>("Visuals", () => null);
            AddOutput<Bloon>("Bloon", () => null );
        }
        public override void ProcessModule()
        {
            
        }
    }
}
