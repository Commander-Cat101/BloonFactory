using Il2CppNinjaKiwi.Common.ResourceUtils;
using Il2CppNinjaKiwi.NKMulti.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Sounds
{
    public static class Sounds
    {
        public static AudioClipReference[] CeramicHitSounds = { new AudioClipReference("f0e3f81cdc85b2944a4c51058d9edb06"), new AudioClipReference("b7e78628f52e58b48a0538146a56ba59"), new AudioClipReference("0b00b5a15a7b7984a9347cf2720c2997"), new AudioClipReference("a20bd33aeea430d4aaf9ce598c66b9e0") };

        public static AudioClipReference[] MoabHitSounds = { new AudioClipReference("a200b313c2cad4848930bd4e8ed8429c"), new AudioClipReference("c7edc0ecb57f35a408c5da2097c7fd2d"), new AudioClipReference("09b29cc02fd26c0478956b381d8c8b33"), new AudioClipReference("df27614540f8be443987da59abb2305c") };

        public static AudioClipReference[] GoldenHitSounds = { new AudioClipReference("d2fd3d179c6479a46a74fedd42e04f80"), new AudioClipReference("bd080a87aefb5844e9e5abfc29e8546a"), new AudioClipReference("bb7f9055e73798d498a2a342ed04e4a3"), new AudioClipReference("d85f4df8dc6ec1546bd096b43fcf46c0") };

        public static AudioClipReference[] BadPopSounds = { new AudioClipReference("b71f416964f488b4eb0421f59e2116d3"), new AudioClipReference("b71f416964f488b4eb0421f59e2116d3"), new AudioClipReference("ebc5a2e340f7d3e48881ffcb324f67c1"), new AudioClipReference("ebc5a2e340f7d3e48881ffcb324f67c1"), };

        public static AudioClipReference[] MoabPopSounds = { new AudioClipReference("ccc4e922217054c49b8fd726e5d8af66"), new AudioClipReference("9dce83e92d2e9c74b862e25794798860"), new AudioClipReference("88116454ca9162147af779c6ea77aab3"), new AudioClipReference("fae6126dba712a94298ac7ad9c58f152"), };

        public static AudioClipReference[] StandardPopSounds = { new AudioClipReference("cec997b36092a6642bfcf6335c614379"), new AudioClipReference("737a12e2912237f4585cefe0861c9b80"), new AudioClipReference("5471f323d7e5131489e071392ad4a981"), new AudioClipReference("6b0ba86314db40d4d9b3dd2d71d42be6"), };

        public static AudioClipReference[] CeramicPopSounds = { new AudioClipReference("c9baf5e9522aa4642948980a2dd31c7d"), new AudioClipReference("c30c29f32ed4c1b48822bf801277cd9d"), new AudioClipReference("287dffc5687e3eb41a2fcb8e818f1be7"), new AudioClipReference("c9baf5e9522aa4642948980a2dd31c7d"), };



        public static Dictionary<string, AudioClipReference[]> PopSoundsByName = new Dictionary<string, AudioClipReference[]> 
        {
            { "Standard", StandardPopSounds },
            { "Moab", MoabPopSounds },
            { "Bad", BadPopSounds },
            { "Ceramic", CeramicPopSounds }
        };
        public static Dictionary<string, AudioClipReference[]> HitSoundsByName = new Dictionary<string, AudioClipReference[]>
        {
            { "Moab", MoabHitSounds },
            { "Golden", GoldenHitSounds },
            { "Ceramic", CeramicHitSounds }
        };
        public static Dictionary<string, AudioClipReference> BloonSoundByName = new Dictionary<string, AudioClipReference>
        {
            { "Bloonarius Spawn", new AudioClipReference("36650f4e089da184185599675f236648") },
            { "Bloonarius Death", new AudioClipReference("9931ad4b5fb12d844bed36904d4377f4") },
            { "Blastpopulous Spawn", new AudioClipReference("07a3ced204856e54784c1a28ec23b929") },
            { "Blastpopulous Death", new AudioClipReference("0c0c1ec7e90c1094ea56ad28340548f9") },
            { "Fireball Hit 1", new AudioClipReference("e7a3d918fe2bddd4792b5ccba7fc8e26") },
            { "Fireball Hit 2", new AudioClipReference("a776ffad72d09614c8f9b08f925621ca") },
            { "Fireball Shoot", new AudioClipReference("c29f1d58288158040bfde36467af9cc5") },
            { "Create Rock", new AudioClipReference("a6a30ed0d2c88bf43a1e32ac62e5b546") },
            { "Diamondback Spawn", new AudioClipReference("fdf9d9806e126cf40bb366ba159a20c3") },
            { "Diamondback Death", new AudioClipReference("efb03539c8ec0fc4ca06b408327e1582") },
            { "Tail Shield Damaged", new AudioClipReference("129f9a053b67a1c4ca61ce0f6f1c21aa") },
            { "Tail Shield Broken", new AudioClipReference("080ff110d8c9f7a4e9a6c2f26eaeca70") },
            { "Dreadbloon Spawn", new AudioClipReference("be68f26ba9b64b449b883987c059bf8b") },
            { "Dreadbloon Death", new AudioClipReference("1f8d5242dbb1e4e409eb07f7012d135b") },
            { "Dreadbloon Rearmour", new AudioClipReference("ecf28438980e90e40978142e7c28c767") },
            { "Lych Spawn", new AudioClipReference("543a1758e782f6a47994018ab15f2e87") },
            { "Lych Death", new AudioClipReference("463fff76fb92d3b4fa583cff51415dc1") },
            { "Become Invincible", new AudioClipReference("6cc3ddba14673e545b495bf51cbb0a11") },
            { "Mini Lync Spawn", new AudioClipReference("543a1758e782f6a47994018ab15f2e87") },
            { "Mini Lync Death", new AudioClipReference("463fff76fb92d3b4fa583cff51415dc1") },
            { "Phayze Spawn", new AudioClipReference("5131b7b78cd9fb3428d54e5756153e57") },
            { "Phayze Death", new AudioClipReference("805f93aec87807443bc89390769c12bc") },
            { "Phayze Armour Break", new AudioClipReference("64b2df74300dd8747b7dd8dc9ba7facc") },
            { "Phayze Rearmour", new AudioClipReference("c0436da0bd4cc5045b21a7297932cab8") },
            { "Phayze Dash", new AudioClipReference("1b5680b2bf3d0db479522c4da974784b") },
            { "Vortex Spawn", new AudioClipReference("400dea73a631bd844aa5c48abbb6bb13") },
            { "Vortex Death", new AudioClipReference("364fd46ee6701b342a3981b5676d6e94") },
            { "Stun Towers", new AudioClipReference("71b13fdb7ea6b51458488483c2e956c6") },
            { "Tornado", new AudioClipReference("9f663945dc2878049a2aad540503eca2") },
            { "Fuse Ignited", new AudioClipReference("ecf28438980e90e40978142e7c28c767") },
            { "Explosion", new AudioClipReference("ec9a5a7531a750e4c82bf8429e0812e4") },
            { "Retribution Bloon Pop", new AudioClipReference("c0436da0bd4cc5045b21a7297932cab8") },
            { "Zap", new AudioClipReference("364fd46ee6701b342a3981b5676d6e94") },
            { "Cash from Bloon", new AudioClipReference("129f9a053b67a1c4ca61ce0f6f1c21aa") }
        };
    }
    public static class SoundExtentions
    {
        public static AudioClipReference PickRandom(this AudioClipReference[] clips)
        {
            return clips[Random.Shared.Next(clips.Length)];
        }
    }
}
