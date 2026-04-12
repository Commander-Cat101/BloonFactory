using BloonFactory.LinkTypes;
using BloonFactory.ModuleProperties;
using FactoryCore.API.ModuleProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Sounds
{
    internal class SoundModule : Module
    {
        public override string Name => "Sound Module";

        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Sound", Sounds.BloonSoundByName.Select(a => a.Key).ToArray(), 0));
            AddProperty(new SoundPreviewProperty(() => Sounds.BloonSoundByName.ElementAt(GetValue<int>("Sound")).Value));
        }
        public override void GetLinkNodes()
        {
            AddOutput<BloonSound>("Sound", GetSound);
        }

        public BloonSound GetSound()
        {
            return new BloonSound(Sounds.BloonSoundByName.ElementAt(GetValue<int>("Sound")).Value);
        }
    }
}
