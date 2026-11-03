using BloonFactory.Handlers;
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
        public Guid BloonGUID;
        public override bool UseIconAsDisplay => false;
        public override string Name => BloonGUID.ToString();
        public override string BaseBloon => "Red";
        public CustomBloon()
        {

        }
        public CustomBloon(Guid guid)
        {
            BloonGUID = guid;
        }
        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.id = BloonGUID.ToString();
        }
        public void ModifyExistingBloonModel(BloonTemplate template, BloonModel model, RoundSetModel roundset)
        {
            template.LoadModules();
            model.icon = new SpriteReference() { guidRef = "" };
            foreach (var module in template.GetModulesOfType<BloonModule>())
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

            foreach (var module in template.GetModulesOfType<TriggerModule>())
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

            DamageStateDisplayModule.DamageStateFix(model, template);
            model.dontShowInSandbox = true;
        }
        public override IEnumerable<ModContent> Load()
        {
            List<CustomBloon> bloons = new List<CustomBloon>();
            for (int i = 0; i < BloonFactory.CustomBloonAmount; i++)
            {
                var bloon = new CustomBloon(Guid.NewGuid());
                bloons.Add(bloon);
                yield return bloon;
            }
            BloonHandler.Initialize(bloons.ToArray());
        }
    }
}
