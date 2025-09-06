using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using UnityEngine;

namespace BloonFactory.Modules.Display
{
    internal class SimpleDisplayModule : Module
    {
        public override string Name => "Simple Display";

        public override void GetLinkNodes()
        {
            AddInput<Visuals>("Visuals");
        }
        public override void GetModuleProperties()
        {
            AddProperty(new SpaceModuleProperty(100));
        }
        public override void ProcessModule()
        {
            Visuals visuals = GetInputValue<Visuals>("Visuals");
            visuals.bloonModel.disallowCosmetics = true;
            var display = new SimpleBloonDisplay(GenerateTexture, (BloonTemplate)Template);
            display.Apply(visuals.bloonModel);
        }
        public Texture2D GenerateTexture()
        {
            return ModContent.GetTexture<BloonFactory>("BaseBloon");
        }
    }
}
