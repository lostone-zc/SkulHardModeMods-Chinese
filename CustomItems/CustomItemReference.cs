using System;
using System.Reflection;
using Characters;
using Characters.Abilities;
using Characters.Abilities.CharacterStat;
using Characters.Gear.Items;
using GameResources;
using HarmonyLib;
using UnityEngine;

namespace CustomItems;

[Serializable]
public class CustomItemReference : ItemReference
{
    private string _originalName;
    public new string name
    {
        get { return base.name; }
        set
        {
            _originalName = value;
            base.name = "Custom-" + _originalName;
        }
    }

    internal string itemName;
    internal string itemDescription;
    internal string itemLore;
    internal Stat.Values stats;

    internal Ability[] abilities;

    internal string[] forbiddenDrops = new string[0];

    private Item item = null;
    private Sprite miniIcon;

    private static GameObject rootObj = null;

    public Item GetItem(Item baseItem)
    {
        if (item == null)
        {
            if (rootObj == null)
            {
                rootObj = new GameObject("CustomItems");
                rootObj.SetActive(false);
                UnityEngine.Object.DontDestroyOnLoad(rootObj);
            }

            item = UnityEngine.Object.Instantiate<Item>(baseItem, rootObj.transform);
            UnityEngine.Object.DontDestroyOnLoad(item);

            item.gameObject.name = name;

            item.keyword1 = prefabKeyword1;
            item.keyword2 = prefabKeyword2;
            item._stat = stats;
            item._rarity = rarity;
            item._gearTag = gearTag;

            LoadSprites();
            if (icon != null)
            {
                item.dropped.spriteRenderer.sprite = icon;
            }

            if (abilities != null && abilities.Length != 0)
            {
                GameObject attacherComponent = new("Ability Attacher");
                attacherComponent.transform.parent = item.gameObject.transform;

                var attacher = item._abilityAttacher = new();
                attacher._container = attacherComponent;

                attacher._components = new AbilityAttacher[abilities.Length];

                abilities[0]._defaultIcon = miniIcon;

                for (int i = 0; i < abilities.Length; i++)
                {
                    GameObject attacherObj = new GameObject("[" + i + "]", new Type[] { typeof(AlwaysAbilityAttacher) });
                    attacherObj.transform.parent = attacherComponent.transform;
                    AlwaysAbilityAttacher aa = attacherObj.GetComponent<AlwaysAbilityAttacher>();

                    aa._abilityComponent = CreateAbilityObject(attacherObj, abilities[i]);

                    attacher._components[i] = aa;
                }
            }

            //item.gameObject.SetActive(false);
        }

        // TODO: Clone, like we did with the wheel.
        return item;
    }

    private static AbilityComponent CreateAbilityObject(GameObject parent, Ability ability)
    {
        Type componentType;

        if (AbilityMap.Map.TryGetValue(ability.GetType(), out componentType))
        {
            GameObject abilityObj = new GameObject("Ability", new Type[] { componentType });
            abilityObj.transform.parent = parent.transform;

            FieldInfo field = AccessTools.Field(componentType, "_ability");
            Component component = abilityObj.GetComponent(componentType);
            field.SetValue(component, ability);

            return (AbilityComponent)component;
        }


        /*switch (ability)
        {
            case ApplyStatusOnGaveDamage:
                return MakeAbility<ApplyStatusOnGaveDamage, ApplyStatusOnGaveDamageComponent>(parent, ability);
            case StatBonusPerGearTag:
                return MakeAbility<StatBonusPerGearTag, StatBonusPerGearTagComponent>(parent, ability);
            case StatBonusByAirTime:
                return MakeAbility<StatBonusByAirTime, StatBonusByAirTimeComponent>(parent, ability);
            case AddAirJumpCount:
                return MakeAbility<AddAirJumpCount, AddAirJumpCountComponent>(parent, ability);
        }*/

        throw new NotImplementedException("Ability Type " + ability.GetType() + " not implemented yet.");
    }

    /*
    private static AbilityComponent MakeAbility<AbilityType, ComponentType>(GameObject parent, Ability ability)
    where AbilityType : Ability
    where ComponentType : AbilityComponent<AbilityType>
    {
        GameObject abilityObj = new GameObject("Ability", new Type[] { typeof(ComponentType) });
        abilityObj.transform.parent = parent.transform;
        ComponentType ab = abilityObj.GetComponent<ComponentType>();
        ab._ability = (AbilityType)ability;

        return ab;
    }
    */

    public void LoadSprites()
    {
        GetIcon();
        GetThumbnail();
        GetMiniIcon();
    }

    public Sprite GetIcon()
    {
        if (icon == null)
        {
            icon = LoadSprite("Icon", 0.85f);
        }
        return icon;
    }
    public Sprite GetThumbnail()
    {
        if (thumbnail == null)
        {
            thumbnail = LoadSprite("Thumbnail", 2.12f);

            if (thumbnail != null)
            {
                thumbnail.name = name;
            }
        }
        return thumbnail;
    }
    public Sprite GetMiniIcon()
    {
        if (miniIcon == null)
        {
            miniIcon = LoadSprite("AbilityIcon", 1.0f);

            if (miniIcon != null)
            {
                miniIcon.name = name;
            }
        }
        return miniIcon;
    }

    private Sprite LoadSprite(string type, float size)
    {
        Sprite sprite = null;

        try
        {
            var assembly = typeof(CustomItemReference).Assembly;
            var resource = assembly.GetManifestResourceStream("CustomItems.Sprites." + _originalName + "." + type + ".png");

            byte[] buf = new byte[resource.Length];
            resource.Read(buf, 0, (int)resource.Length);

            Texture2D texture = new(2, 2);
            texture.LoadImage(buf);
            texture.filterMode = FilterMode.Point;

            var side = Math.Max(texture.width, texture.height);

            sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), side / size);
        }
        catch (Exception)
        {
            Debug.LogWarning("[CustomItems] Could not load the " + type + " sprite for " + name);
        }

        return sprite;
    }
}
