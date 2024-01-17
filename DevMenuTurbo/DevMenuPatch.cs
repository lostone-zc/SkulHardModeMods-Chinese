using TMPro;
using UI.TestingTool;
using HarmonyLib;
using UnityEngine;
using GameResources;
using Singletons;
using Services;
using Level;
using Characters.Gear.Items;
using UnityEngine.UI;

namespace DevMenu;

[HarmonyPatch]
public class DevMenuTurboPatch
{
    [HarmonyPatch(typeof(Panel), "canUse", MethodType.Getter)]
    static bool Prefix(ref Panel __instance, ref bool __result)
    {
        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(Panel), "Awake")]
    [HarmonyPostfix]
    static void TranslateToEnglish(ref Panel __instance)
    {
        ref var self = ref __instance;

        SetText(self._openMapList, "地图");
        SetText(self._nextStage, "下一章节");
        SetText(self._nextMap, "下一地图");
        SetText(self._openGearList, "装备清单");
        SetText(self._hideUI, "隐藏UI");
        SetText(self._getGold, "+10000 金币");
        SetText(self._getDarkquartz, "+1000 魔石");
        SetText(self._getBone, "+100 碎骨");
        SetText(self._getHeartQuartz, "+100 魔石核心");
        SetText(self._testMap, "测试地图");
        SetText(self._awake, "升级头骨");
        SetText(self._damageBuff, "伤害Buff");
        SetText(self._hp10k, "+10000 HP");
        SetText(self._noCooldown, "无冷却");
        SetText(self._shield10, "+10 护盾");
        SetText(self._hardmodeToggle, "魔镜");
        SetText(self._rerollSkill, "重置技能");
        SetText(self._timeScaleReset, "重置");
        SetText(self._infiniteRevive, "不朽");
        SetText(self._verification, "血量显示");
        SetText(self._right3, "所有Buff ->");

        SetText2(self._hardmodeLevelSlider, "魔镜等级");
        SetText2(self._hardmodeClearedLevelSlider, "魔镜解锁");
        SetText2(self._timeScaleSlider, "加速");

        SetText(self._gearList.transform, "返回");

        // Disable showing your time zone, just in case
        self._localNow.gameObject.SetActive(false);
        self._utcNow.gameObject.SetActive(false);
    }

    private static void SetText(Component obj, string text)
    {
        obj.GetComponentInChildren<TextMeshProUGUI>(true)?.SetText(text);
    }

    private static void SetText2(Component obj, string text)
    {
        var texts = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
        if (texts != null && texts.Length > 1)
        {
            texts[1].SetText(text);
        }
    }

    [HarmonyPatch(typeof(GearListElement), "Set")]
    [HarmonyPostfix]
    static void FillInventoryOnClick(ref GearListElement __instance, GearReference gearReference)
    {
        ref var self = ref __instance;

        if (gearReference.type != Characters.Gear.Gear.Type.Item)
            return;

        var handler = self.gameObject.AddComponent<ButtonRightClickHandler>();
        handler.OnRightClick += delegate
        {
            GearRequest request = gearReference.LoadAsync();
            request.WaitForCompletion();

            LevelManager manager = Singleton<Service>.Instance.levelManager;
            var inventory = manager.player.playerComponents.inventory.item;

            Item item = null;

            for (int i = 0; i < inventory.items.Count; i++)
            {
                if (inventory.items[i] == null)
                {
                    if (item == null)
                    {
                        item = (Item)manager.DropGear(request, Vector3.zero);
                        inventory.EquipAt(item, i);
                    }
                    else
                    {
                        Item clone = manager.DropItem(item, Vector3.zero);
                        inventory.EquipAt(clone, i);
                    }
                }
            }
        };
    }

    [HarmonyPatch(typeof(GearList), "Awake")]
    [HarmonyPostfix]
    static void FixDarkAbilitySearch(ref GearList __instance)
    {
        var self = __instance;
        foreach (var button in self._upgradeListElements)
        {
            Text name = button.GetComponentInChildren<Text>();
            button.name = name.text;
            name.fontSize = 29;
        }

        self._inputField.onValueChanged.RemoveAllListeners();
        self._inputField.onValueChanged.AddListener(delegate
        {
            if (self._currentFilter == GearList.Filter.Upgrade)
                self.FilterUpgrade();
            else
                self.FilterGearList();
        });
    }
}
