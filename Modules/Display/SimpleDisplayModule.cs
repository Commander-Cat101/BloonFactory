using BloonFactory.LinkTypes;
using BloonFactory.ModuleProperties;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Harmony;
using UnityEngine;

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
            AddProperty(new BloonTextureModuleProperty(GenerateTexture));
        }
        public override void ProcessModule()
        {
            Visuals visuals = GetInputValue<Visuals>("Visuals");
            visuals.bloonModel.disallowCosmetics = true;
            var display = new BloonDisplay(GenerateTexture, (BloonTemplate)Template, Id.ToString());
            display.Apply(visuals.bloonModel);
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
