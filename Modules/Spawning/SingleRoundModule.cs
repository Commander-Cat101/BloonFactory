using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Rounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Spawning
{
    internal class SingleRoundModule : Module
    {
        public override string Name => "Single Round";

        public override string Description => "Requires a connected bloon group to spawn.Requires a connected bloon group to spawn.";
        public override void GetLinkNodes()
        {
            AddInput<RoundSetModel>("Roundset");
            AddOutput<RoundModel>("Round", () => GetInputValue<RoundSetModel>("Roundset").rounds[GetValue<int>("Round") - 1]);
        }
        public override void GetModuleProperties()
        {
            AddProperty(new IntModuleProperty("Round", 20, 0, 120));
        }
        public override void ProcessModule()
        {
            GetOutputsModules("Round").ProcessAll();
        }
    }
}
