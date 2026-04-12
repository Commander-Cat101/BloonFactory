using BloonFactory.LinkTypes;
using BloonFactory.ModuleProperties;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Api.Internal;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API.ModuleValues;
using HarmonyLib;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

namespace BloonFactory.Modules.Display
{
    internal class SimpleDisplayModule : Module
    {
        public override string Name => "Simple Display";

        internal BloonTexture bloonTexture;
        public override void GetLinkNodes()
        {
            AddInput<Visuals>("Visuals");
            AddOutput<BloonTexture>("Texture", () => bloonTexture);
        }
        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Scale", 1f, 0.01f, 10f));
            AddProperty(new BloonTextureModuleProperty(GenerateTexture));
        }
        public override void ProcessModule()
        {
            Visuals visuals = GetInputValue<Visuals>("Visuals");
            visuals.bloonModel.disallowCosmetics = true;
            var display = new BloonDisplay(GenerateTexture, (BloonTemplate)Template, Guid.NewGuid().ToString(), GetValue<float>("Scale"));
            display.Apply(visuals.bloonModel);

            if (visuals.bloonModel.icon.guidRef == "")
            {
                Texture2D texture = GenerateTexture();
                Guid guid = Guid.NewGuid();

                ResourceHandler.AddTexture(guid.ToString(), texture);

                visuals.bloonModel.icon = new SpriteReference() { guidRef = $"Ui[{guid.ToString()}]" };

                MelonLogger.Msg($"Added icon for bloon {visuals.bloonModel.name} with GUID {guid}");

            }
        }
        public Texture2D GenerateTexture()
        {
            var outputs = GetOutputsModules("Texture");

            bloonTexture = new BloonTexture(); 
            
            var baseBloon = ModContent.GetTexture<BloonFactory>("BaseBloon");
            bloonTexture.texture = baseBloon;
            outputs.ProcessAll();
            return bloonTexture.texture;
        }
    }
}

