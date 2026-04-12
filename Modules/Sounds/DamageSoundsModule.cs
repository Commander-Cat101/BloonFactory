using BloonFactory.ModuleProperties;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Sounds
{
    internal class DamageSoundsModule : Module
    {
        public override string Name => "Damage Sounds";

        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Hit Sound", Sounds.HitSoundsByName.Select(a => a.Key).ToArray(), 0));
            AddProperty(new SoundPreviewProperty(() => Sounds.HitSoundsByName.ElementAt(GetValue<int>("Hit Sound")).Value.PickRandom()));
            AddProperty(new EnumModuleProperty("Pop Sound", Sounds.PopSoundsByName.Select(a => a.Key).ToArray(), 0));
            AddProperty(new SoundPreviewProperty(() => Sounds.PopSoundsByName.ElementAt(GetValue<int>("Pop Sound")).Value.PickRandom()));
        }
        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
        }
        public override void ProcessModule()
        {
            var bloon = GetInputValue<BloonModel>("Bloon");

            bloon.GetBehavior<CreateSoundOnDamageBloonModel>().sounds = Sounds.HitSoundsByName.ElementAt(GetValue<int>("Hit Sound")).Value;

            var popSounds = Sounds.PopSoundsByName.ElementAt(GetValue<int>("Pop Sound")).Value;
            var popEffectModel = bloon.GetBehavior<PopEffectModel>();

            popEffectModel.soundEffect1Id = popSounds[0];
            popEffectModel.soundEffect2Id = popSounds[1];
            popEffectModel.soundEffect3Id = popSounds[2];
            popEffectModel.soundEffect4Id = popSounds[3];
        }
    }
}
