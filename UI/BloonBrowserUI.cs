using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.ChallengeEditor;
using Il2CppNinjaKiwi.Common;
using MelonLoader;
using Newtonsoft.Json;
using Octokit.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static BloonFactory.UI.BloonBrowserBloonPanel;

namespace BloonFactory.UI
{
    internal class BloonBrowserUI : ModGameMenu<ContentBrowser>
    {
        public bool generatedContentReady = false;
        public BloonBrowserEntry[] currentPage;
        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            ModifyElements();
            AddElements();

            GenerateContentForPage();
            return false;
        }
        public override void OnMenuUpdate()
        {
            if (generatedContentReady)
            {
                generatedContentReady = false;
                foreach (var entry in currentPage)
                {
                    GameMenu.scrollRect.content.gameObject.AddModHelperComponent(CreateTemplate(entry));
                }
            }
        }
        public void ModifyElements()
        {
            GameMenu.GetComponentFromChildrenByName<RectTransform>("TopBar").gameObject.active = false;
            GameMenu.GetComponentFromChildrenByName<RectTransform>("Tabs").gameObject.active = false;

            var verticalLayoutGroup = GameMenu.scrollRect.content.GetComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.SetPadding(50);
            verticalLayoutGroup.spacing = 50;
            verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childControlHeight = false;
            GameMenu.scrollRect.rectTransform.sizeDelta += new Vector2(0, 200);
            GameMenu.scrollRect.rectTransform.localPosition += new Vector3(0, 100, 0);
        }
        public void AddElements()
        {
            var container = GameMenu.GetComponentFromChildrenByName<RectTransform>("Container").gameObject.AddModHelperPanel(new Info("SearchBar", 0, -475, new Vector2(0.5f, 1)));
            container.AddInputField(new Info("Search", 0, -425, 1500, 150), "Search...", VanillaSprites.BlueInsertPanel).InputField.textComponent.enableAutoSizing = true;
        }
        public void GenerateContentForPage()
        {
            GameMenu.scrollRect.content.transform.DestroyAllChildren();

            Task.Run(async () =>
            {
                PageUpdateRequest request = await ServerHandler.RequestPageUpdate();
                currentPage = request.Data.ToArray();
                generatedContentReady = true;
            });
        }
    }
}
