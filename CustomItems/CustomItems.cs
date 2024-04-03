using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Characters.Abilities;
using Characters.Abilities.CharacterStat;
using Characters.Gear.Items;
using Characters.Gear.Synergy.Inscriptions;
using CustomItems.CustomAbilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Characters.CharacterStatus;
using static Characters.Damage;

namespace CustomItems;

public class CustomItems
{
    public static readonly List<CustomItemReference> Items = InitializeItems();

    private static List<CustomItemReference> InitializeItems()
    {
        List<CustomItemReference> items = new();
        {
            var item = new CustomItemReference();
            item.name = "DecidedlyPhysical";
            item.rarity = Rarity.Legendary;

            item.itemName = "巨魔之王的贴身长棍";
            item.itemDescription = "<color=#F25D1C>物攻</color>加100%，\n不能造成<color=#1787D8>魔法伤害</color>。";
            item.itemLore = "The weapon of the strongest Orc. Once plated in gold, but he could never maintain it; his brute strength would shatter it.";

            item.prefabKeyword1 = Inscription.Key.Brave;
            item.prefabKeyword2 = Inscription.Key.Brave;

            item.stats = new Stat.Values(new Stat.Value[] { });

            StatBonus bonus = new();

            bonus._stat = new Stat.Values(new Stat.Value[]{
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.PhysicalAttackDamage, 1.0),
                new Stat.Value(Stat.Category.Percent, Stat.Kind.MagicAttackDamage, 0),
            });

            item.abilities = new Ability[] { bonus };


            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "PurelyMagical";
            item.rarity = Rarity.Legendary;

            item.itemName = "暗鸦君主徽章";
            item.itemDescription = "<color=#1787D8>魔攻</color>加100%，\n不能造成<color=#F25D1C>物理伤害</color>。";
            item.itemLore = "Able to control his army of raven souls, the Raven Lord has never faced a physical confrontation for the rest of his life.";

            item.prefabKeyword1 = Inscription.Key.Wisdom;
            item.prefabKeyword2 = Inscription.Key.Wisdom;

            item.stats = new Stat.Values(new Stat.Value[] { });

            StatBonus bonus = new();

            bonus._stat = new Stat.Values(new Stat.Value[]{
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.MagicAttackDamage, 1.0),
                new Stat.Value(Stat.Category.Percent, Stat.Kind.PhysicalAttackDamage, 0),
            });

            item.abilities = new Ability[] { bonus };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "WisdomAndCourage";
            item.rarity = Rarity.Rare;

            item.itemName = "对偶护符";
            item.itemDescription = "加40%<color=#F25D1C>物攻</color>和<color=#1787D8>魔攻</color>。";
            item.itemLore = "With a taste for knowledge, undecided warrior Haxa could never succumb to pure brutality.";

            item.prefabKeyword1 = Inscription.Key.Brave;
            item.prefabKeyword2 = Inscription.Key.Wisdom;

            item.stats = new Stat.Values(new Stat.Value[]{
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.MagicAttackDamage, 0.4),
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.PhysicalAttackDamage, 0.4),
            });

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "ElementalMess";
            item.rarity = Rarity.Legendary;

            item.itemName = "五等分的哀伤";
            item.itemDescription = "攻击和技能有10%概率附上五种异常状态的一种（毒血冰晕火)。\n"
                                + "(冰和晕有内置4秒CD。)";
            item.itemLore = "The Demonlord Beelz becomes stronger by inflicting pain and suffering on everyone around him.";

            item.prefabKeyword1 = Inscription.Key.Execution;
            item.prefabKeyword2 = Inscription.Key.Misfortune;

            item.stats = new Stat.Values(new Stat.Value[] { });

            item.abilities = new Ability[5];

            for (int i = 0; i < 5; i++)
            {
                var ability = new ApplyStatusOnGaveDamage();
                var status = (Kind)i;
                ability._cooldownTime = (status == Kind.Freeze || status == Kind.Stun) ? 4.0f : 0.1f;
                ability._chance = 10;
                ability._attackTypes = new();
                ability._attackTypes[MotionType.Basic] = true;
                ability._attackTypes[MotionType.Skill] = true;

                ability._types = new();
                ability._types[AttackType.Melee] = true;
                ability._types[AttackType.Ranged] = true;
                ability._types[AttackType.Projectile] = true;

                ability._status = new CharacterStatus.ApplyInfo(status);

                item.abilities[i] = ability;
            }

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "FireAndIce";
            item.rarity = Rarity.Rare;

            item.itemName = "元素师法杖";
            item.itemDescription = "<color=#1787D8>魔攻</color>加25%，\n"
                                + "技能有5%概率附上冰冻或者燃烧。\n"
                                + "(冰冻有额外2秒CD。)\n"
                                + "如果<u>所有</u>敌人都处于燃烧状态，<color=#1787D8>魔攻</color>增幅20%。";
            item.itemLore = "Only a true master of the elements could learn how to set ice on fire.";

            item.prefabKeyword1 = Inscription.Key.Arson;
            item.prefabKeyword2 = Inscription.Key.AbsoluteZero;

            item.stats = new Stat.Values(new Stat.Value[] {
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.MagicAttackDamage, 0.25),
            });

            Kind[] statuses = { Kind.Freeze, Kind.Burn };

            item.abilities = new Ability[statuses.Length + 1];

            item.abilities[0] = new StatBonusIfAllEnemiesAreAffectedByStatus
            {
                _stats = new Stat.Values(new Stat.Value[] { new Stat.Value(Stat.Category.Percent, Stat.Kind.MagicAttackDamage, 1.20) }),
                _status = Kind.Burn
            };

            for (int i = 0; i < statuses.Length; i++)
            {
                var ability = new ApplyStatusOnGaveDamage();
                var status = statuses[i];
                ability._cooldownTime = (status == Kind.Freeze) ? 2.0f : 0.1f;
                ability._chance = 10;
                ability._attackTypes = new();
                ability._attackTypes[MotionType.Skill] = true;

                ability._types = new();
                ability._types[AttackType.Melee] = true;
                ability._types[AttackType.Ranged] = true;
                ability._types[AttackType.Projectile] = true;

                ability._status = new CharacterStatus.ApplyInfo(status);

                item.abilities[i + 1] = ability;
            }

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "PoisonAndBleed";
            item.rarity = Rarity.Rare;

            item.itemName = "淬毒飞镖";
            item.itemDescription = "<color=#F25D1C>物攻</color>加25%，\n"
                                + "技能有5%概率附上中毒或者流血。\n"
                                + "如果<u>所有</u>敌人都处于中毒状态，<color=#F25D1C>物攻</color>增幅20%。";
            item.itemLore = "In skilled hands, this kunai brings death as swiftly as a Scorpion's sting.";

            item.prefabKeyword1 = Inscription.Key.Poisoning;
            item.prefabKeyword2 = Inscription.Key.ExcessiveBleeding;

            item.stats = new Stat.Values(new Stat.Value[] {
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.PhysicalAttackDamage, 0.25),
            });

            Kind[] statuses = { Kind.Wound, Kind.Poison };

            item.abilities = new Ability[statuses.Length + 1];

            item.abilities[0] = new StatBonusIfAllEnemiesAreAffectedByStatus
            {
                _stats = new Stat.Values(new Stat.Value[] { new Stat.Value(Stat.Category.Percent, Stat.Kind.PhysicalAttackDamage, 1.20) }),
                _status = Kind.Poison
            };

            for (int i = 0; i < statuses.Length; i++)
            {
                var ability = new ApplyStatusOnGaveDamage();
                var status = statuses[i];
                ability._cooldownTime = 0.1f;
                ability._chance = 10;
                ability._attackTypes = new();
                ability._attackTypes[MotionType.Skill] = true;

                ability._types = new();
                ability._types[AttackType.Melee] = true;
                ability._types[AttackType.Ranged] = true;
                ability._types[AttackType.Projectile] = true;

                ability._status = new CharacterStatus.ApplyInfo(status);

                item.abilities[i + 1] = ability;
            }

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "CarleonHeritage";
            item.rarity = Rarity.Common;

            item.itemName = "卡利恩旗帜";
            item.itemDescription = "每拥有一件卡利恩装备，加15%<color=#F25D1C>物攻</color>和<color=#1787D8>魔攻</color>。";
            item.itemLore = "The symbol of Carleon's people brought strength during times of need.";

            item.prefabKeyword1 = Inscription.Key.Spoils;
            item.prefabKeyword2 = Inscription.Key.Heritage;

            item.gearTag = Characters.Gear.Gear.Tag.Carleon;

            item.stats = new Stat.Values(new Stat.Value[] { });

            StatBonusPerGearTag ability = new();

            ability._tag = Characters.Gear.Gear.Tag.Carleon;

            ability._statPerGearTag = new Stat.Values(new Stat.Value[] {
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.PhysicalAttackDamage, 0.15),
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.MagicAttackDamage, 0.15),
            });

            item.abilities = new Ability[] { ability };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "BoneOfJumps";
            item.rarity = Rarity.Common;

            item.itemName = "骨翼";
            item.itemDescription = "加一次跳跃，获得50%的缓降。\n"
                                 + "(使用石像鬼时在空中攻击或者释放技能降低下落速度)\n"
                                 + "在空中加25%<color=#F25D1C>物攻</color>和<color=#1787D8>魔攻</color>。";
            item.itemLore = "Float like a butterfly, sting like a bee!";

            item.prefabKeyword1 = Inscription.Key.Bone;
            item.prefabKeyword2 = Inscription.Key.Soar;

            item.stats = new Stat.Values(new Stat.Value[] { });

            AddAirJumpCount jumpAbility = new();

            StatBonusByAirTime bonus = new();

            bonus._timeToMaxStat = 0.01f;
            bonus._remainTimeOnGround = 0.0f;
            bonus._maxStat = new Stat.Values(new Stat.Value[] {
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.PhysicalAttackDamage, 0.25),
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.MagicAttackDamage, 0.25),
            });

            GravityScale gravityAbility = new() { amount = 0.5f };

            item.abilities = new Ability[] { bonus, jumpAbility, gravityAbility };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "BoneOfJumps_BoneUpgrade";
            item.rarity = Rarity.Common;

            item.obtainable = false;

            item.itemName = "永恒骨头：翼";
            item.itemDescription = "加两次跳跃，获得60%的缓降。\n"
                                 + "(使用石像鬼时在空中攻击或者释放技能降低下落速度)\n"
                                 + "在空中加50%<color=#F25D1C>物攻</color>和<color=#1787D8>魔攻</color>。";
            item.itemLore = "Float like a cloud, sting like a swarm of bees!";

            item.prefabKeyword1 = Inscription.Key.Bone;
            item.prefabKeyword2 = Inscription.Key.Soar;

            item.stats = new Stat.Values(new Stat.Value[] { });

            AddAirJumpCount jumpAbility = new() { _count = 2 };

            StatBonusByAirTime bonus = new();

            bonus._timeToMaxStat = 0.01f;
            bonus._remainTimeOnGround = 0.0f;
            bonus._maxStat = new Stat.Values(new Stat.Value[] {
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.PhysicalAttackDamage, 0.5),
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.MagicAttackDamage, 0.5),
            });

            GravityScale gravityAbility = new() { amount = 0.4f };

            item.abilities = new Ability[] { bonus, jumpAbility, gravityAbility };

            item.forbiddenDrops = new[] { "BoneOfJumps" };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "TalariaOfMercury";
            item.rarity = Rarity.Unique;

            item.itemName = "水星足立健";
            item.itemDescription = "加一次疾驰次数，获得50%移速。";
            item.itemLore = "Mortals could never achieve Hermes's speed. The key to his swiftness was his winged sandals.";

            item.prefabKeyword1 = Inscription.Key.Chase;
            item.prefabKeyword2 = Inscription.Key.Rapidity;

            item.stats = new Stat.Values(new Stat.Value[] {
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.MovementSpeed, 0.5),
            });

            item.abilities = new Ability[] {
                new ExtraDash(),
            };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "TheTreasureOmen";
            item.rarity = Rarity.Unique;

            item.gearTag = Characters.Gear.Gear.Tag.Omen;
            item.obtainable = false; // Omens should be unobtainable

            item.itemName = "预兆：万恶之根";
            item.itemDescription = "每拥有100金币加1%<color=#F25D1C>物攻</color>和<color=#1787D8>魔攻</color>，\n"
                                 + "每拥有1个宝物刻印加20%<color=#F25D1C>物攻</color>和<color=#1787D8>魔攻</color>。";
            item.itemLore = "Wealth is the true root of all power, for greed knows no bounds.";

            item.prefabKeyword1 = Inscription.Key.Omen;
            item.prefabKeyword2 = Inscription.Key.Treasure;

            item.stats = new Stat.Values(new Stat.Value[] { });

            item.abilities = new Ability[] {
                new StatBonusByGoldAmount(){
                    _statsPerStack = new Stat.Values(new Stat.Value[] {
                        new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.PhysicalAttackDamage, 0.01),
                        new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.MagicAttackDamage, 0.01),
                    }),
                    _goldPerStack = 100
                },
                new StatBonusByInscriptionCount(){
                    _keys = new[]{Inscription.Key.Treasure},
                    _type = StatBonusByInscriptionCount.Type.Count,
                    _statPerStack = new Stat.Values(new Stat.Value[] {
                        new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.PhysicalAttackDamage, 0.2),
                        new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.MagicAttackDamage, 0.2),
                    })
                },
            };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "PandorasBox";
            item.rarity = Rarity.Legendary;

            item.itemName = "水番多拉魔盒";
            item.itemDescription = "拾起时会用高等级装备替换身上所有装备，\n每替换一件给500块钱。";
            item.itemLore = "\"This is my own special gift to you. Don't ever open it.\"";

            item.prefabKeyword1 = Inscription.Key.Heirloom;
            item.prefabKeyword2 = Inscription.Key.Treasure;

            item.stats = new Stat.Values(new Stat.Value[] { });

            item.abilities = new Ability[] {
                new PandorasBoxAbility(),
            };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "QuintDamageBuff";
            item.rarity = Rarity.Unique;

            item.itemName = "猛禽之爪";
            item.itemDescription = "增加15%暴击伤害，\n精华伤害可以暴击。";
            item.itemLore = "No one has ever seen the monster that possessed such powerful claws and survived to tell the story.";

            item.prefabKeyword1 = Inscription.Key.Heritage;
            item.prefabKeyword2 = Inscription.Key.Misfortune;

            item.stats = new Stat.Values(new Stat.Value[]{
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.CriticalDamage, 0.15),
            });

            item.abilities = new Ability[] {
                new AdditionalCritReroll() {
                    motionType = MotionType.Quintessence
                }
            };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "MisfortuneBrawl";
            item.rarity = Rarity.Common;

            item.itemName = "黄铜指虎";
            item.itemDescription = "增加10%暴击伤害，\n增加5%暴击率。";
            item.itemLore = "There is no such thing as a fair fight. Only a fight you win.";

            item.prefabKeyword1 = Inscription.Key.Misfortune;
            item.prefabKeyword2 = Inscription.Key.Brawl;

            item.stats = new Stat.Values(new Stat.Value[]{
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.CriticalDamage, 0.10),
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.CriticalChance, 0.05),
            });

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "CommonMasterpiece";
            item.rarity = Rarity.Common;

            item.itemName = "装饰斧";
            item.itemDescription = "增加20%<color=#F25D1C>物攻</color>。";
            item.itemLore = "During times of distress, anything can be used as a weapon.";

            item.prefabKeyword1 = Inscription.Key.Masterpiece;
            item.prefabKeyword2 = Inscription.Key.Execution;

            item.stats = new Stat.Values(new Stat.Value[]{
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.PhysicalAttackDamage, 0.20)
            });

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "CommonMasterpiece_2";
            item.obtainable = false; // Evolutions should not be obtainable by default
            item.rarity = Rarity.Common;

            item.itemName = "荣耀战斧";
            item.itemDescription = "增幅20%<color=#F25D1C>物攻</color>。";
            item.itemLore = "A weapon reforged by the fire of its warrior's soul.";

            item.prefabKeyword1 = Inscription.Key.Masterpiece;
            item.prefabKeyword2 = Inscription.Key.Execution;

            item.forbiddenDrops = new[] { "CommonMasterpiece" };

            item.stats = new Stat.Values(new Stat.Value[]{
                new Stat.Value(Stat.Category.Percent, Stat.Kind.PhysicalAttackDamage, 1.20)
            });

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "TheManatechOmen";
            item.rarity = Rarity.Unique;

            item.gearTag = Characters.Gear.Gear.Tag.Omen;
            item.obtainable = false; // Omens should be unobtainable

            item.itemName = "预兆：魔工学奇迹";
            item.itemDescription = "5秒内每捡起一个零件增幅5%技能伤害，最多50%。";
            item.itemLore = "A paradoxical machine that never seems to stop running.";

            item.prefabKeyword1 = Inscription.Key.Omen;
            item.prefabKeyword2 = Inscription.Key.Manatech;

            item.stats = new Stat.Values(new Stat.Value[] { });

            StatBonusPerManatechPart bonus = new();

            bonus._timeout = 5.0f;
            bonus._maxStack = 10;
            bonus._statPerStack = new Stat.Values(new Stat.Value[] {
                new Stat.Value(Stat.Category.Percent, Stat.Kind.SkillAttackDamage, 0.05),
            });

            item.abilities = new Ability[] { bonus };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "AdventurerKiller";
            item.rarity = Rarity.Unique;

            item.itemName = "灵魂提取器";
            item.itemDescription = "杀死冒险家必定掉落专属装备，掉落一次后该物品消失。";
            item.itemLore = "One's soul is the source of their most desirable posessions.";

            item.prefabKeyword1 = Inscription.Key.Duel;
            item.prefabKeyword2 = Inscription.Key.Execution;

            item.stats = new Stat.Values(new Stat.Value[] { });

            item.abilities = new Ability[] {
                new AdventurerWeaponSteal(){
                    baseItem = item.name
                }
            };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "SymbolOfConfidence";
            item.rarity = Rarity.Unique;

            item.itemName = "自信的象征";
            item.itemDescription = "减伤25%。\n"
                                 + "每有1%血量加1%<color=#F25D1C>物攻</color>和<color=#1787D8>魔攻</color>，最多100%。";
            item.itemLore = "The blistering confidence brought by a strong defense makes the most powerful warriors.";

            item.prefabKeyword1 = Inscription.Key.Antique;
            item.prefabKeyword2 = Inscription.Key.Fortress;

            item.stats = new Stat.Values(new Stat.Value[]{
                new Stat.Value(Stat.Category.Percent, Stat.Kind.TakingDamage, 0.75)
            });

            StatBonusPerHPPercent bonus = new();

            bonus._maxStat = new Stat.Values(new Stat.Value[] {
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.PhysicalAttackDamage, 1.0),
                new Stat.Value(Stat.Category.PercentPoint, Stat.Kind.MagicAttackDamage, 1.0),
            });

            item.abilities = new Ability[] { bonus };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "OmenClone";
            item.rarity = Rarity.Unique;

            item.gearTag = Characters.Gear.Gear.Tag.Omen;
            item.obtainable = false; // Omens should be unobtainable

            item.itemName = "预兆：神秘藏品";
            item.itemDescription = "有一随机刻印，\n"
                                 + "当有14种刻印时升级。";
            item.itemLore = "The heart of a true collector desires what it desires. Even when it doesn't know what lies inside.";

            item.prefabKeyword1 = Inscription.Key.Omen;
            item.prefabKeyword2 = Inscription.Key.None;

            item.stats = new Stat.Values(new Stat.Value[] { });
            item.abilities = new Ability[] {
                new InscriptionCountAbility(),
            };

            item.extraComponents = new[] {
                typeof(OmenKeywordRandomizer),
                typeof(UpgradeOnInscriptionCount),
            };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "OmenClone_2";
            item.obtainable = false; // Omens are not obtainable, and neither should be evolutions.
            item.rarity = Rarity.Unique;

            item.itemName = "预兆：疯狂神像";
            item.itemDescription = "有一随机刻印，\n"
                                 + "每个持有刻印增加1个，\n"
                                 + "收集癖对刻印需求增加1。";
            item.itemLore = "I DON'T DESIRE ANYTHING ANYMORE! I HAVE EVERYTHING I'VE EVER WANTED!";

            // Omens are unobtainable, so they are found by their tag.
            // So we don't put the Omen tag or inscription here, and leave for the evolution process to copy them.
            item.prefabKeyword1 = Inscription.Key.None;
            item.prefabKeyword2 = Inscription.Key.None;

            item.stats = new Stat.Values(new Stat.Value[] { });

            item.abilities = new Ability[] {
                new NerfCollectionDesire(){
                    _count = 1
                }
            };

            item.extraComponents = new[] {
                typeof(OmenKeywordRandomizer), // Allows dropping the item with DevMenu or Machine
                typeof(CloneCloneClone),
            };

            item.forbiddenDrops = new[] { "OmenClone" };

            items.Add(item);
        }

        {
            var item = new CustomItemReference();
            item.name = "OmenClone_2_BoneUpgrade";
            item.obtainable = false; // Omens are not obtainable, and neither should be evolutions.
            item.rarity = Rarity.Unique;

            item.itemName = "预兆：骨粉";
            item.itemDescription = "每个持有刻印增加1个，\n"
                                 + "收集癖对刻印需求增加1，\n"
                                 + "骨头刻印的替换次数需求减1。";
            item.itemLore = "Yo dawg, I heard you like Bone! So I put a Bone inside this Bone item so you can use more Bone against Evil Little Bone!";

            // Omens are unobtainable, so they are found by their tag.
            // So we don't put the Omen tag or inscription here, and leave for the evolution process to copy them.
            // The Bone upgrade process works the same, copying the item's inscription. So we're fine here.
            item.prefabKeyword1 = Inscription.Key.None;
            item.prefabKeyword2 = Inscription.Key.Bone;

            item.stats = new Stat.Values(new Stat.Value[] { });

            item.abilities = new Ability[] {
                new NerfCollectionDesire(){
                    _count = 1
                },
                new ReduceBoneInscriptionRequirement(){
                    _count = 1
                }
            };

            item.extraComponents = new[] {
                typeof(DelayedOmenAssigner), // To allow people dropping it from the DevMenu
                typeof(CloneCloneClone),
            };

            item.forbiddenDrops = new[] { "OmenClone", "OmenClone_2" };

            items.Add(item);
        }

        return items;
    }

    internal static void LoadSprites()
    {
        Items.ForEach(item => item.LoadSprites());
    }

    internal static Dictionary<string, string> MakeStringDictionary()
    {
        Dictionary<string, string> strings = new(Items.Count * 8);

        foreach (var item in Items)
        {
            strings.Add("item/" + item.name + "/name", item.itemName);
            strings.Add("item/" + item.name + "/desc", item.itemDescription);
            strings.Add("item/" + item.name + "/flavor", item.itemLore);
        }

        return strings;
    }

    internal static List<Masterpiece.EnhancementMap> ListMasterpieces()
    {
        var masterpieces = Items.Where(i => (i.prefabKeyword1 == Inscription.Key.Masterpiece) || (i.prefabKeyword2 == Inscription.Key.Masterpiece))
                                .ToDictionary(i => i.name);

        return masterpieces.Where(item => masterpieces.ContainsKey(item.Key + "_2"))
                           .Select(item => new Masterpiece.EnhancementMap()
                           {
                               _from = new AssetReference(item.Value.guid),
                               _to = new AssetReference(masterpieces[item.Key + "_2"].guid),
                           })
                           .ToList();
    }

    internal static List<Bone.SuperAbility.EnhancementMap> ListUpgradableBones()
    {
        var items = Items.ToDictionary(i => i.name);

        return items.Where(item => items.ContainsKey(item.Key + "_BoneUpgrade"))
                    .Select(item => new Bone.SuperAbility.EnhancementMap()
                    {
                        _from = new AssetReference(item.Value.guid),
                        _to = new AssetReference(items[item.Key + "_BoneUpgrade"].guid),
                    })
                    .ToList();
    }
}
