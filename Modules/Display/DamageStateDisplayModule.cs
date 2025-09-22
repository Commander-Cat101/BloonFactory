using BloonFactory.LinkTypes;
using BloonFactory.ModuleProperties;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Internal;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloonFactory.Modules.Display
{
    internal class DamageStateDisplayModule : Module
    {
        public override string Name => "Damage State Display";

        internal BloonTexture bloonTexture;
        public override void GetLinkNodes()
        {
            AddInput<Visuals>("Visuals");
            AddOutput<BloonTexture>("Texture", () => bloonTexture);
        }
        public override void GetModuleProperties()
        {
            AddProperty(new FloatSliderModuleProperty("Percent", 50, 0, 100, 1));
            AddProperty(new FloatModuleProperty("Scale", 1f, 0.01f, 10f));
            AddProperty(new BloonTextureModuleProperty(GenerateTexture));
        }
        public override void ProcessModule()
        {
            Visuals visuals = GetInputValue<Visuals>("Visuals");
            visuals.bloonModel.disallowCosmetics = true;
            var display = new BloonDisplay(GenerateTexture, (BloonTemplate)Template, Id.ToString(), GetValue<float>("Scale"));
            MelonLogger.Msg(display.Id);

            if (visuals.bloonModel.damageDisplayStates == null)
            {
                visuals.bloonModel.damageDisplayStates = new List<DamageStateModel>().ToIl2CppReferenceArray();
            }
            var list = visuals.bloonModel.damageDisplayStates.ToList();
            list.Add(new DamageStateModel("DamageState", ModContent.CreatePrefabReference(display.Id), GetValue<float>("Percent") / 100));
            visuals.bloonModel.damageDisplayStates = list.ToIl2CppReferenceArray();
        }
        public Texture2D GenerateTexture()
        {
            bloonTexture = new BloonTexture();

            var outputs = GetOutputsModules("Texture");
            if (outputs.Count != 0)
            {
                outputs.ProcessAll();
            }
            else
            {
                var baseBloon = ModContent.GetTexture<BloonFactory>("BaseBloon");
                bloonTexture.texture = baseBloon;
            }
            return bloonTexture.texture;
        }

        public static void DamageStateFix(BloonModel model, BloonTemplate template)
        {
            if (model.damageDisplayStates != null)
            {
                var list = model.damageDisplayStates.ToList();
                var max = list.MaxBy(a => a.healthPercent);
                if (max != null)
                    max.healthPercent = 1;
                model.damageDisplayStates = list.ToIl2CppReferenceArray();
            }
            if (model.icon.guidRef == "")
            {
                var modules = template.GetModulesOfType<DamageStateDisplayModule>();
                if (modules == null || modules.Count == 0)
                    return;

                Texture2D texture = modules.MaxBy(a => a.GetValue<float>("Percent")).GenerateTexture();

                Guid guid = Guid.NewGuid();
                ResourceHandler.AddTexture(guid.ToString(), texture);
                model.icon = new SpriteReference() { guidRef = $"Ui[{guid.ToString()}]" };
            }
        }
    }
}
