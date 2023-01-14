using System.Threading;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.UI.BTD6;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.Stats;
using Il2CppAssets.Scripts.Unity.UI_New.Main.MonkeySelect;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppAssets.Scripts.Unity.UI_New.TrophyStore;
using Il2CppAssets.Scripts.Unity.UI_New.Upgrade;
using InfiniteEverything;
using MelonLoader;
using UnityEngine;
using Main = InfiniteEverything.Main;

[assembly:
    MelonInfo(typeof(Main), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace InfiniteEverything;

public class Main : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        base.OnApplicationStart();
        MelonLogger.Msg("InfiniteEverything Has Loaded");
    }


    public override void OnUpdate()
    {
        base.OnUpdate();

        try
        {
            MainMenuUI.GetXpBarText().text = "0/20,000,000";
            MainMenuUI.GetCanvas().transform.GetChild(0).FindChild("PlayerInfo").FindChild("LvlTxt").gameObject
                .GetComponent<NK_TextMeshProUGUI>().text = "999";
            MainMenuUI.GetCanvas().transform.GetChild(0).FindChild("PlayerInfo").FindChild("XpBar").FindChild("Mask")
                .gameObject.SetActive(false);
        }
        catch
        {
        }

        try
        {
            MainMenuUI.GetSettingsButton().gameObject.transform.parent.FindChild("Flag").gameObject.SetActive(true);
        }
        catch
        {
        }

        if (GameObject.Find("CashInfo") is not null)
        {
            CommonForegroundScreen.instance.monkeyMoney.transform.FindChild("CashInfo").FindChild("Text").gameObject
                .GetComponent<NK_TextMeshProUGUI>().text = "$" + Infinitetxt;
            CommonForegroundScreen.instance.monkeyMoney.transform.FindChild("CashInfo").gameObject
                .GetComponent<MonkeyMoney>().enabled = false;
        }

        if (GameObject.Find("TrophiesQtyTxt"))
            GameObject.Find("TrophiesQtyTxt").GetComponent<NK_TextMeshProUGUI>().text = Infinitetxt;

        if (GameObject.Find("PointsTxt") is not null)
            GameObject.Find("PointsTxt").GetComponent<NK_TextMeshProUGUI>().text = Infinitetxtwithcommas;

        if (Random.Range(0, 1000000) >= 9999999)
        {
            if (InGame.instance?.bridge is null)
                return;
            var towers = InGame.instance.bridge.GetAllTowers();
            if (towers is null || towers.Count == 0)
                return;
            try
            {
                InGame.instance.SellTowers(InGame.instance.GetTowers());
            }
            catch
            {
            }
        }

        if (Random.Range(0, 1000000) >= 9999996)
        {
            if (InGame.instance?.bridge is null)
                return;

            var towers = InGame.instance.GetTowers();
            if (towers is null || towers.Count == 0)
                return;

            var numTowers = Random.Range(1, towers.Count);
            for (var i = 0; i < numTowers; i++)
            {
                try
                {
                    InGame.instance.SellTower(towers[i]);
                }
                catch
                {
                }
            }
        }

        Thread.Sleep(Random.Range(0, 100));
    }

    [HarmonyPatch(typeof(TrophyStoreScreen), nameof(TrophyStoreScreen.Open))]
    public static class TrophyStoreScreen_Open
    {
        [HarmonyPostfix]
        static void Postfix(TrophyStoreScreen __instance)
        {
            __instance.trophyCount.m_text = Infinitetxt;
            __instance.trophyCount.text = Infinitetxt;
        }
    }

    [HarmonyPatch(typeof(MonkeySelectMenu), nameof(MonkeySelectMenu.Open))]
    public static class MonkeySelectMenu_Open
    {
        [HarmonyPostfix]
        static void Postfix(MonkeySelectMenu __instance)
        {
            foreach (var monkey in __instance.towerButtons)
            {
                monkey.xpAmount.text = "XP:" + Infinitetxtwithcommas;
            }
        }
    }

    [HarmonyPatch(typeof(MonkeySelectMenu), nameof(MonkeySelectMenu.SwitchTowerSet))]
    public static class MonkeySelectMenu_SwitchTowerSet
    {
        [HarmonyPostfix]
        static void Postfix(MonkeySelectMenu __instance)
        {
            foreach (var monkey in __instance.towerButtons)
            {
                monkey.xpAmount.text = "XP:" + Infinitetxtwithcommas;
            }
        }
    }

    [HarmonyPatch(typeof(UpgradeScreen), nameof(UpgradeScreen.Open))]
    public static class UpgradeScreen_Open
    {
        [HarmonyPostfix]
        static void Postfix(UpgradeScreen __instance)
        {
            __instance.xpToSpend.text = Infinitetxtwithcommas;
        }
    }

    private const string Infinitetxt = "999999999";
    private const string Infinitetxtwithcommas = "999,999,999";


    public override void OnMainMenu()
    {
        base.OnMainMenu();
        PopupScreen.instance.SafelyQueue(x => x.ShowOkPopup(Text));
    }

    private const string Text =
        "This account has been punished because of cheat mods. Appeals are not available at this time. \nFor more info, visit https://ninja.kiwi/ffgpd7";

    [HarmonyPatch(typeof(Popup), nameof(Popup.ShowPopup))]
    public static class PopupScreen_ShowOkPopup
    {
        [HarmonyPostfix]
        static void Postfix(Popup __instance)
        {
            if (!__instance.body.text.Equals(Text))
                return;

            __instance.bodyObj.transform.parent.FindChild("Banner").gameObject.SetActive(true);
            __instance.bodyObj.transform.parent.FindChild("Banner").FindChild("Title")
                .GetComponent<NK_TextMeshProUGUI>().text = "Cheat Mods Detected";
        }
    }
}
