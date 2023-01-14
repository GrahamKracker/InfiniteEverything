using System.Threading;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Main.MonkeySelect;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
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
        if (GameObject.Find("Flag") is not null)
            GameObject.Find("Flag").SetActive(true);
        if (GameObject.Find("CashInfo") is not null)
        {
            /*GameObject.Find("CashInfo").GetComponent<MonkeyMoney>()
                .enabled = false;*/
            GameObject.Find("CashInfo").transform.FindChild("Text").GetComponent<NK_TextMeshProUGUI>().text =
                "$" + Infinitetxt;
        }

        if (GameObject.Find("PointsTxt") is not null)
            GameObject.Find("PointsTxt").GetComponent<NK_TextMeshProUGUI>().text = Infinitetxtwithcommas;

        if (Random.Range(0, 1000000) >= 9999999)
        {
            if (InGame.instance?.bridge is null)
                return;
            var towers = InGame.instance.bridge.GetAllTowers();
            if (towers is null || towers.Count == 0)
                return;
            
            try{InGame.instance.SellTowers(InGame.instance.GetTowers());}
            catch{}
            
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
                try{InGame.instance.SellTower(towers[i]);}
                catch{}
            }
            
        }
        
        Thread.Sleep(Random.Range(0, 100));
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

    private const string Text = "This account has been punished because of cheat mods. Appeals are not available at this time. \nFor more info, visit https://ninja.kiwi/ffgpd7";

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
