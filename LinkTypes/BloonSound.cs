using Il2CppNinjaKiwi.Common.ResourceUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.LinkTypes
{
    internal class BloonSound
    {
        public AudioClipReference sound;

        public BloonSound(AudioClipReference sound)
            { this.sound = sound; }
    }
}
