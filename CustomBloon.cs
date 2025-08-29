using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using Il2CppAssets.Scripts.Models.Bloons;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory
{
    internal class CustomBloon : ModBloon
    {
        public static List<CustomBloon> Bloons = new List<CustomBloon>(); 
        public override bool UseIconAsDisplay => false;
        public override string Name => $"{BloonTemplate.Guid}";
        public override string BaseBloon => "Red";

        public BloonTemplate BloonTemplate;
        public CustomBloon(BloonTemplate template)
        {
            BloonTemplate = template;
        }
        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.name = BloonTemplate.Name;
            bloonModel.id = BloonTemplate.Guid.ToString();
        }
        public override IEnumerable<ModContent> Load()
        {
            if (!SerializationHandler.HasLoaded)
                SerializationHandler.LoadAllTemplates();
            MelonLogger.Error("STARTING LOAD");
            foreach (var template in SerializationHandler.Templates)
            {
                MelonLogger.Error($"Loading {template.Name} bloon...");
                yield return new CustomBloon(template);
            }
        }
        public override void Register()
        {
            base.Register();
            Bloons.Add(this);
        }
    }
}
