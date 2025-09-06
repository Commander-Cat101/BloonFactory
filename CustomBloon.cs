using BloonFactory.Modules;
using BloonFactory.Modules.Core;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using FactoryCore.API;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppNinjaKiwi.Common.ResourceUtils;
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

        public override SpriteReference IconReference => GetSpriteReference("BaseBloon");
        public CustomBloon()
        {

        }
        public CustomBloon(BloonTemplate template)
        {
            BloonTemplate = template;
        }
        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.name = BloonTemplate.Name;
            bloonModel.id = BloonTemplate.Guid.ToString();
        }
        public void ModifyExistingBloonModel(BloonModel model)
        {
            BloonTemplate.LoadModules();
            foreach (var module in BloonTemplate.GetModulesOfType<BloonModule>())
            {
                MelonLogger.Msg("Processing module");
                module.currentModel = model;
                module.ProcessModule();
            }

            foreach (var module in BloonTemplate.GetModulesOfType<TriggerModule>())
            {
                module.currentModel = model;
                module.ProcessModule();
            }
        }
        public override IEnumerable<ModContent> Load()
        {
            if (!SerializationHandler.HasLoaded)
                SerializationHandler.LoadAllTemplates();

            MelonLogger.Msg("STARTING LOAD");
            foreach (var template in SerializationHandler.Templates)
            {
                MelonLogger.Msg($"Loading {template.Name} bloon...");
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
