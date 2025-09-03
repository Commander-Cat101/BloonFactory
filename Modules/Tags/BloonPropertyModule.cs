using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Bloons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Tags
{
    internal class BloonPropertyModule : Module
    {
        public override string Name => "Bloon Property";

        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
        }
        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Bloon Property", ["None", "Lead", "Black", "White", "Purple", "Frozen", "Immune"], 0));
        }
        public override void ProcessModule()
        {
            int value = GetValue<int>("Bloon Property");
            if (value == 0)
                return;
            GetInputValue<BloonModel>("Bloon").bloonProperties |= (BloonProperties)MathF.Pow(2, value - 1);
        }
    }
}
