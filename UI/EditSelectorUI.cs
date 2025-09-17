using BloonFactory.Modules.Core;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu.Powers;
using Il2CppAssets.Scripts.Unity.UI_New.Main.PowersSelect;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppAssets.Scripts.Unity.UI_New.Settings;
using Il2CppNinjaKiwi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloonFactory.UI
{
    internal class EditSelectorUI : ModGameMenu<SettingsScreen>
    {
        public ModHelperScrollPanel scrollPanel;
        public Animator bottomGroupAnimator;
        public Animator mainPanelAnimator;
        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            GameMenu.anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            GameMenu.transform.DestroyAllChildren();
            var panel = GameMenu.gameObject.AddModHelperPanel(new Info("Root", InfoPreset.FillParent));

            CreateMainContent(panel);
            CreateExtraContent(panel);
            return false;
        }
        public override void OnMenuClosed()
        {
            bottomGroupAnimator.Play("PopupSlideOut");
            mainPanelAnimator.Play("PopupScaleOut");
        }
        public void CreateMainContent(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("MainPanel", 0, 50, 3500, 1700), VanillaSprites.MainBGPanelBlue);
            scrollPanel = panel.AddScrollPanel(new Info("ScrollPanel", 0, 0, 3400, 1600), UnityEngine.RectTransform.Axis.Vertical, VanillaSprites.BlueInsertPanelRound, 50, 50);

            mainPanelAnimator = panel.AddComponent<Animator>();
            mainPanelAnimator.runtimeAnimatorController = Animations.PopupAnim;
            mainPanelAnimator.speed = .55f;
            mainPanelAnimator.Play("PopupSlideIn");
            AddContent();
        }
        public void CreateExtraContent(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("Panel", InfoPreset.FillParent));

            var newBloon = panel.AddButton(new Info("CreateNewBloon", 0, 250, 800, 300, new Vector2(0.5f, 0)), VanillaSprites.GreenBtnLong, new Action(() =>
            {
                MenuManager.instance.buttonClickSound.Play("ClickSounds");
                PopupScreen.instance.SafelyQueue(screen => screen.ShowSetNamePopup("Create Bloon", "Name of bloon to create.\n", new Action<string>(name =>
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        SerializationHandler.CreateTemplate(name);
                        AddContent();
                    }
                }), null));
                PopupScreen.instance.SafelyQueue(screen => screen.ModifyField(tmpInputField =>
                {
                    tmpInputField.textComponent.font = Fonts.Btd6FontBody;
                    tmpInputField.characterLimit = 20;
                }));
            }));
            newBloon.AddText(new Info("Text", 0, 0, 700, 250), "Create", 120);

            var openBrowser = panel.AddButton(new Info("BloonDownloader", 300, 300, 400, 400, new Vector2(0, 0)), VanillaSprites.AlternateBloonsBtn, new Action(() => ModGameMenu.Open<BloonBrowserUI>()));
            openBrowser.AddText(new Info("Text", 0, -150, 400, 200), "Bloon\nBrowser").EnableAutoSizing();

            bottomGroupAnimator = panel.AddComponent<Animator>();
            bottomGroupAnimator.runtimeAnimatorController = Animations.PopupAnim;
            bottomGroupAnimator.speed = .55f;
            bottomGroupAnimator.Play("PopupSlideIn");
        }
        public void AddContent()
        {
            scrollPanel.ScrollContent.transform.DestroyAllChildren();

            foreach (var template in SerializationHandler.Templates.Where(a => !a.IsQueueForDeletion))
            {
                scrollPanel.AddScrollContent(CreateContentPanel(template));
            }
        }
        public ModHelperPanel CreateContentPanel(BloonTemplate template)
        {
            var panel = ModHelperPanel.Create(new Info("Template", 3300, 300), VanillaSprites.MainBGPanelBlue);

            panel.AddText(new Info("Name", 600, 0, 1000, 150, new Vector2(0, 0.5f)), template.Name, 100, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing(150, 10);
            panel.AddButton(new Info("Edit", -150, 0, 200, 200, new Vector2(1, 0.5f)), VanillaSprites.EditBtn, new Action(() => { OpenEditorWithTemplate(template); }));
            panel.AddButton(new Info("Delete", -400, 0, 200, 200, new Vector2(1, 0.5f)), VanillaSprites.CloseBtn, new Action(() => { SerializationHandler.DeleteTemplate(template); AddContent(); }));

            if (!template.IsLoaded)
                panel.AddButton(new Info("NotLoaded", -20, -20, 100, 100, new Vector2(1, 1)), VanillaSprites.NoticeBtn,
                    new Action(() =>
                    {
                        PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "This Bloon is unloaded", "This bloon is unloaded and wont show up ingame, restart the game to load this bloon.", null, "Ok", null, null, Popup.TransitionAnim.Scale));
                    })
                );
            return panel;
        }
        public void OpenEditorWithTemplate(BloonTemplate template)
        {
            BloonEditorUI.Template = template;
            ModGameMenu.Open<BloonEditorUI>();
        }
        
    }
}
