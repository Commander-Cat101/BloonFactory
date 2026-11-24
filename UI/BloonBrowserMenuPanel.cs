using BloonFactory.Handlers;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.GameEditor.UI.PopupPanels;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using Il2CppTMPro;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.RectTransform;

namespace BloonFactory.UI
{
    [RegisterTypeInIl2Cpp(false)]
    internal class BloonBrowserMenuPanel : ModHelperPanel
    {
        public static SpriteReference DownloadsIcon => ModContent.GetSpriteReference<BloonFactory>("DownloadsIcon");

        ModHelperButton downloadButton => GetDescendent<ModHelperButton>("DownloadButton");
        ModHelperImage downloadedImage => GetDescendent<ModHelperImage>("DownloadedImage");

        ModHelperPanel infoPanel => GetDescendent<ModHelperPanel>("InfoPanel");

        ModHelperButton infoButton => GetDescendent<ModHelperButton>("InfoButton");

        ModHelperText nameText => GetDescendent<ModHelperText>("Name");
        ModHelperText categoryText => GetDescendent<ModHelperText>("Category");
        ModHelperText descriptionText => GetDescendent<ModHelperText>("Description");
        ModHelperText downloadCountText => GetDescendent<ModHelperText>("DownloadLabel");
        ModHelperText likeCountText => GetDescendent<ModHelperText>("LikesLabel");

        public static VertexGradient regularGradient => new(new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255));
        public static TMP_ColorGradient verifiedGradient
        {
            get
            {
                var gradient = ScriptableObject.CreateInstance<TMP_ColorGradient>();
                gradient.topLeft = new Color32(255, 253, 184, 255);
                gradient.topRight = new Color32(255, 253, 184, 255);
                gradient.bottomLeft = new Color32(210, 141, 13, 255);
                gradient.bottomRight = new Color32(210, 141, 13, 255);
                return gradient;
            }
        }

        public static TMP_ColorGradient excellentGradient
        {
            get
            {
                var gradient = ScriptableObject.CreateInstance<TMP_ColorGradient>();
                gradient.topLeft = new Color32(1, 162, 244, 255);
                gradient.topRight = new Color32(1, 162, 244, 255);
                gradient.bottomLeft = new Color32(145, 220, 255, 255);
                gradient.bottomRight = new Color32(145, 220, 255, 255);
                return gradient;
            }
        }

        private bool downloading;
        public void SetEntry(BloonBrowserEntry? browserEntry)
        {
            if (browserEntry is null)
            {
                gameObject.SetActive(false);
                return;
            }
            BloonBrowserEntry entry = browserEntry.Value;

            nameText.SetText($"{entry.uploadData.Name ?? string.Empty}  by  {entry.uploadData.Author}");

            if (entry.VerificationStatus != VerificationStatus.Unverified)
                nameText.Text.SetVertexColorGradient(entry.VerificationStatus == VerificationStatus.Excellent ? excellentGradient : verifiedGradient);
            else
                nameText.Text.colorGradient = regularGradient;

            descriptionText.SetText(entry.uploadData.Description ?? string.Empty);
            categoryText.SetText(entry.uploadData.Category.ToFriendlyString());

            downloadButton.Button.onClick.RemoveAllListeners();
            downloadButton.Button.onClick.AddListener(new Action(() =>
            {
                Download(entry.uploadData.UUID);
                MenuManager.instance.buttonClick3Sound.Play("ClickSounds");
            }));

            infoButton.Button.onClick.RemoveAllListeners();
            infoButton.Button.onClick.AddListener(new Action(() =>
            {
                SetDescriptionOpen(!infoPanel.isActiveAndEnabled);
                MenuManager.instance.buttonClick2Sound.Play("ClickSounds");
            }));

            downloadCountText.SetText(GetDownloadString(entry.DownloadCount));
            
            likeCountText.SetText(GetDownloadString(entry.LikeCount));

            SetDescriptionOpen(false);
            SetDownloaded(SerializationHandler.IsUUIDLoaded(entry.uploadData.UUID));

            gameObject.SetActive(true);
        }
        public BloonBrowserMenuPanel(IntPtr ptr) : base(ptr)
        {
        }

        public static BloonBrowserMenuPanel CreateTemplate()
        {
            var panel = Create<BloonBrowserMenuPanel>(new Info("Panel", 3600, 450));
            panel.FitContent(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize);
            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.spacing = 50;

            var mainPanel = panel.AddPanel(new Info("SubPanel", 3350 / 2, -125, 3350, 200, new Vector2(0f, 1f)), VanillaSprites.MainBGPanelBlue);

            var text = mainPanel.AddText(new Info("Name", 1050, 0, 1900, 200, new Vector2(0, 0.5f)), $"Test Bloon  by  JohnDoe123", 80, TextAlignmentOptions.Left);
            text.EnableAutoSizing(80, 20);
            text.Text.enableVertexGradient = true;

            mainPanel.AddButton(new Info("DownloadButton", 150, 100, 200, 200, new Vector2(1, 0f)), ModHelperSprites.DownloadBtn, new Action(() =>
            {
                
            }));

            mainPanel.AddImage(new Info("DownloadedImage", 150, 100, 200, 200, new Vector2(1, 0f)), VanillaSprites.TickGreenIcon);

            panel.AddPanel(new Info("InfoPanel", 0, 75, 3600, 150, new Vector2(0.5f, 0f)), VanillaSprites.BlueInsertPanel);

            var description = panel.infoPanel.AddText(new Info("Description", 0, 5, 3500, 100, new Vector2(0.5f, 0.5f)), "", 70, TextAlignmentOptions.Left);
            description.EnableAutoSizing(70, 20);
            description.Text.overflowMode = TextOverflowModes.Overflow;

            mainPanel.AddButton(new Info("InfoButton", -115, 0, 150, 150, new Vector2(1, 0.5f)), VanillaSprites.InfoBtn2, new Action(() =>
            {
                panel.SetDescriptionOpen(!panel.infoPanel.isActiveAndEnabled);
                MenuManager.instance.buttonClick2Sound.Play("ClickSounds");
            }));

            mainPanel.AddText(new Info("Category", -1150, 0, 500, 200, new Vector2(1, 0.5f)), "Boss", 100, TextAlignmentOptions.Right).EnableAutoSizing(80, 20);

            mainPanel.AddImage(new Info("DownloadIcon", -750, 0, 150, 150, new Vector2(1, 0.5f)), DownloadsIcon.guidRef);
            mainPanel.AddText(new Info("DownloadLabel", -600, 0, 100, 150, new Vector2(1, 0.5f)), GetDownloadString(0), 80, TextAlignmentOptions.Left).EnableAutoSizing(80);
            
            mainPanel.AddImage(new Info("LikesIcon", -450, 0, 150, 150, new Vector2(1, 0.5f)), VanillaSprites.ChallengeThumbsUpIcon);
            mainPanel.AddText(new Info("LikesLabel", -300, 0, 100, 150, new Vector2(1, 0.5f)), GetDownloadString(0), 80, TextAlignmentOptions.Left).EnableAutoSizing(80);

            panel.SetDescriptionOpen(false);

            return panel;
        }
        public static string GetDownloadString(int count)
        {
            if (count >= 1000000)
                return $"{(count / 1000000f).ToString("0.0")}M";
            if (count >= 1000)
                return $"{(count / 1000f).ToString("0.0")}K";
            return count.ToString();
        }
        public void SetDownloaded(bool downloaded)
        {
            downloadButton.gameObject.SetActive(!downloaded);
            downloadedImage.gameObject.SetActive(downloaded);
        }
        public void SetDescriptionOpen(bool active)
        {
            infoPanel.SetActive(active);
            LayoutRebuilder.ForceRebuildLayoutImmediate(this);
        }
        private async void Download(string uuid)
        {
            if (downloading)
                return;
            downloading = true;

            BloonTemplate template = await ServerHandler.DownloadTemplate(uuid);
            if (template == null)
            {
                PopupScreen.instance.SafelyQueue(screen =>
                {
                   screen.ShowPopup(PopupScreen.Placement.menuCenter, "Download Failed", "The template could not be downloaded from the server. Please try again later.", null, "Ok", null, null, Popup.TransitionAnim.Scale);
                });
                downloading = false;
                return;
            }
            if (SerializationHandler.TryLoadTemplate(template))
            {
                template.SetReferences();
                template.LoadModules();

                downloading = false;
                SetDownloaded(true);
                PopupScreen.instance.SafelyQueue(screen =>
                {
                    screen.ShowPopup(PopupScreen.Placement.menuCenter, "Download Success", "The template was successfully downloaded from the server", null, "Ok", null, null, Popup.TransitionAnim.Scale);
                });
            }
        }
        
    }
}
