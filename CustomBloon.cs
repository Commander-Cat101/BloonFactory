using BloonFactory.Modules;
using BloonFactory.Modules.Core;
using BloonFactory.Modules.Display;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using FactoryCore.API;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        public void ModifyExistingBloonModel(BloonModel model, RoundSetModel roundset)
        {
            BloonTemplate.LoadModules();
            model.icon = new SpriteReference() { guidRef = "" };
            foreach (var module in BloonTemplate.GetModulesOfType<BloonModule>())
            {
                try
                {
                    module.currentModel = model;
                    module.currentRoundSet = roundset;
                    module.ProcessModule();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                }
            }

            foreach (var module in BloonTemplate.GetModulesOfType<TriggerModule>())
            {
                try
                {
                    module.currentModel = model;
                    module.ProcessModule();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                }
            }

            DamageStateDisplayModule.DamageStateFix(model, BloonTemplate);
            
            if (model.icon.guidRef == "")
            {
                MelonLogger.Msg("No icon setting to default");
                model.icon = GetSpriteReference("BaseBloon");
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
