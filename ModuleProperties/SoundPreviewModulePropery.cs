using BloonFactory.Modules.Sounds;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Audio;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TaskScheduler = BTD_Mod_Helper.Api.TaskScheduler;

namespace BloonFactory.ModuleProperties
{
    internal class SoundPreviewProperty : ModuleProperty
    {
        public Func<AudioClipReference> GetSound;

        public object CoroutineHandle;

        public string uniqueId = Guid.NewGuid().ToString();

        public SoundPreviewProperty(Func<AudioClipReference> getSound)
        {
            GetSound = getSound;
            Name = Guid.NewGuid().ToString();
        }
        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            //Play a dummy sound otherwise causes an error on first play
            Game.instance.audioFactory.PlaySound(Sounds.GoldenHitSounds[0], "FX", uniqueId: uniqueId, volume: 0);

            var panel = root.AddPanel(new Info("BloonTextureModuleValue", 0, 0, 1000, 150));

            var slider = panel.AddSlider(new Info("Slider", 425, 0, 700, 70, new Vector2(0, 0.5f)), 0, 0, 1, 0, new Vector2(0, 0));
            slider.Slider.interactable = false;
            slider.DefaultNotch.DeleteObject();

            var button = panel.AddButton(new Info("Play", -125, 0, 100, 100, new Vector2(1, 0.5f)), VanillaSprites.ArrowHideBtn, new Action(() =>
            {

                var audioClip = GetSound.Invoke();

                TaskScheduler.ScheduleTask(() =>
                {
                    Game.instance.audioFactory.PlaySound(audioClip, "FX", uniqueId: uniqueId);

                    AudioSource source = Game.instance.audioFactory.FindSourceByUniqueId(uniqueId);

                    if (CoroutineHandle != null)
                    {
                        MelonCoroutines.Stop(CoroutineHandle);
                    }

                    CoroutineHandle = MelonCoroutines.Start(HandleSoundClip(source, slider));
                });
            }));
            button.transform.rotation = Quaternion.Euler(0, 0, 90);

            return panel;
        }
        public IEnumerator HandleSoundClip(AudioSource source, ModHelperSlider slider)
        {
            slider.Slider.maxValue = source.clip.length;

            while (true)
            {
                try
                {
                    if (!source.isPlaying)
                        break;

                    slider.SetCurrentValue(source.time);
                }
                catch
                {
                    break;
                }
                yield return null;
            }

            slider.SetCurrentValue(slider.Slider.maxValue);
        }
        public override void LoadData()
        {

        }
    }
}
