using BloonFactory.LinkTypes;
using BloonFactory.ModuleProperties;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Actions
{
    internal class SummonFireballsActionModule : Module
    {
        public override string Name => "Summon Fireballs";
        public override void GetModuleProperties()
        {
            AddProperty(new IntModuleProperty("Count", 3, 0, int.MaxValue));
            AddProperty(new FloatModuleProperty("Stun Duration", 2.5f, 0, float.MaxValue));

            AddProperty(new FloatModuleProperty("Magma Duration", 15f, 0, float.MaxValue));
            AddProperty(new FloatModuleProperty("Magma Radius", 15f, 0, float.MaxValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            try
            {
                var trigger = GetInputValue<Trigger>("Trigger");

                trigger.bloonModel.AddBehavior(new FireballActionModel("FireballActionModel", Id.ToString()
                    , new PrefabReference("eda503b848df15243b0d0cd1e6625582"), new PrefabReference("e5bbca84e860c9b418a7d6633dab441d"), new PrefabReference("4f01d8a0ba9df3148b8a2156cb8ee521")
                    , new Il2CppReferenceArray<AudioClipReference>([new AudioClipReference("e7a3d918fe2bddd4792b5ccba7fc8e26"), new AudioClipReference("a776ffad72d09614c8f9b08f925621ca")])
                    , GetValue<int>("Count"), 0.5f, GetValue<float>("Stun Duration"), GetValue<float>("Magma Duration"), GetValue<float>("Magma Radius"), 1.5f, -0.2f, "BlastFireball"
                ));
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to add action. {ex}");
            }

        }
    }
}
