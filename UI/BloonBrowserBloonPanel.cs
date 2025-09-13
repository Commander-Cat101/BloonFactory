using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using Il2CppTMPro;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RectTransform;

namespace BloonFactory.UI
{
    internal class BloonBrowserBloonPanel
    {

        public static ModHelperPanel CreateTemplate(BloonBrowserEntry entry)
        {
            if (entry is null)
                return null;

            var panel = ModHelperPanel.Create(new Info("Panel", 3600, 250), VanillaSprites.MainBGPanelBlue);
            panel.AddText(new Info("Name", 575, 0, 1000, 200, new Vector2(0, 0.5f)), entry.Name, 100, TextAlignmentOptions.Left).EnableAutoSizing(120, 20);

            panel.AddButton(new Info("Download", -125, 0, 200, 200, new Vector2(1, 0.5f)), ModHelperSprites.DownloadBtn, new Action(() =>
            {
                Download(entry.Guid);
            }));
            panel.AddText(new Info("FileSize", -375, 0, 450, 200, new Vector2(1, 0.5f)), ByteArrayLengthToSizeText(entry.FileSize), 120, TextAlignmentOptions.Left).EnableAutoSizing(120, 20);

            /*panel.AddButton(new Info("Heart", -675, 0, 200, 200, new Vector2(1, 0.5f)), ModContent.GetSpriteReference<BloonFactory>("EmptyLikeIcon").guidRef, new Action(() =>
            {

            }));
            panel.AddText(new Info("HeartCount", -450, 0, 200, 200, new Vector2(1, 0.5f)), entry.LikeCount.ToString(), 120, TextAlignmentOptions.Left);*/

            return panel;
        }
        private static string ByteArrayLengthToSizeText(int length)
        {
            if (length >= 1_000_000)
            {
                return $"{Math.Round((double)(length / 1_000_000), 2)} MB";
            }
            if (length >= 1_000)
            {
                return $"{Math.Round((double)(length / 1_000), 2)} KB";
            }
            else
            {
                return $"{length} B";
            }
        }
        private static async void Download(Guid guid)
        {
            BloonTemplate template = await ServerHandler.DownloadTemplate(guid);
            SerializationHandler.TryLoadTemplate(template);
        }
        
    }
}
