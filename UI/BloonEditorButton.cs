using BTD_Mod_Helper.Api;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Unity.UI_New.Main;
using Il2CppAssets.Scripts.Unity.UI_New.Main.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Components;
using Il2CppAssets.Scripts.Unity.UI_New;

namespace BloonFactory.UI
{
    internal static class BloonEditorButton
    {
        public static void Create(MainMenu menu)
        {
            var mainMenuTransform = menu.transform.Cast<RectTransform>();
            var trophyStore = mainMenuTransform.FindChild("TrophyStore");
            var modsButton = trophyStore.Duplicate(mainMenuTransform);

            modsButton.name = "BloonEditor";
            modsButton.transform.FindChild("LimitedTime").Destroy();
            modsButton.GetComponentInChildren<Image>().SetSprite(VanillaSprites.AlternateBloonsBtn);
            modsButton.GetComponentInChildren<Button>().SetOnClick(() => ModGameMenu.Open<CustomBloonSelector>());

            var matchLocalPosition = modsButton.transform.gameObject.AddComponent<MatchLocalPosition>();
            matchLocalPosition.transformToCopy = trophyStore.transform;
            matchLocalPosition.offset = new Vector3(0 ,-325);

            var text = ModHelperText.Create(new Info("BloonEditor", 100, 150), "Bloon\nEditor");
            text.transform.GetComponent<NK_TextMeshProUGUI>().enableAutoSizing = true;
            text.transform.SetParent(modsButton.GetComponentInChildren<Button>().transform);
            
            text.transform.localPosition = new Vector3(150, -275, 0);
            text.transform.SetSiblingIndex(0);
        }
    }
}
