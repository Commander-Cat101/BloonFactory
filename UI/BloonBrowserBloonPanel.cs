using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.GameEditor.UI.PopupPanels;
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
            var name = panel.AddText(new Info("Name", 850, 0, 1500, 200, new Vector2(0, 0.5f)), $"{entry.Name}  by  {entry.Creator}", 100, TextAlignmentOptions.Left);
            name.EnableAutoSizing(100, 20);

            panel.AddButton(new Info("Download", -125, 0, 200, 200, new Vector2(1, 0.5f)), ModHelperSprites.DownloadBtn, new Action(() =>
            {
                Download(entry.Guid);
            }));

            panel.AddText(new Info("Category", -600, 0, 500, 200, new Vector2(1, 0.5f)), entry.CategoryEnum.ToFriendlyString(), 100, TextAlignmentOptions.Right).EnableAutoSizing(100, 20);

            return panel;
        }
        private static async void Download(Guid guid)
        {
            BloonTemplate template = await ServerHandler.DownloadTemplate(guid);
            SerializationHandler.TryLoadTemplate(template);
        }
        
    }
}
