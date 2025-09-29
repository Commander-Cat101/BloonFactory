using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Rounds;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace BloonFactory.Modules.Spawning
{
    internal class MultipleRoundsModule : Module
    {
        public override string Name => "Multiple Rounds";

        public override string Description => "Requires a connected bloon group to spawn.";

        [JsonIgnore]
        public RoundModel currentRound;
        public override void GetLinkNodes()
        {
            AddInput<RoundSetModel>("Roundset");
            AddOutput<RoundModel>("Round", () => currentRound);
        }
        public override void GetModuleProperties()
        {
            AddProperty(new IntModuleProperty("Start Round", 10, 0, 120));
            AddProperty(new IntModuleProperty("End Round", 20, 0, 120));
        }
        public override void ProcessModule()
        {
            int start = GetValue<int>("Start Round");
            int end = GetValue<int>("End Round");

            var modules = GetOutputsModules("Round");
            for (int i = start; i < end; i++)
            {
                currentRound = GetInputValue<RoundSetModel>("Roundset").rounds[i - 1];

                if (modules.Count == 0)
                    GetInputValue<RoundSetModel>("Roundset").rounds[GetValue<int>("Round") - 1].AddBloonGroup(((BloonTemplate)Template).TemplateId, 1, 0, 1);
                else
                    modules.ProcessAll();
            }
        }
    }
}
