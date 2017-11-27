using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace GFW
{
    [Serializable]
    public class CSVPathEditor : ScriptableObject
    {
        public string Path_ExcelInputDir;

        [NonSerialized]
        public List<TableConfig> tableList = new List<TableConfig>()
    {
        //new TableConfig("ActiveTaskReward","","\\Task\\ActiveTaskReward.xlsx"),
        //new TableConfig("PurchaseTimes","","\\PurchaseTimes\\PurchaseTimes.xlsx"),
        //new TableConfig("PurchasePrice","","\\PurchaseTimes\\PurchasePrice.xlsx"),
        //new TableConfig("Errorcode","","\\Resources\\Errorcode.xlsx"),
        //new TableConfig("LanguageSelection","","\\Resources\\LanguageSelection.xlsx"),
        //new TableConfig("LanguageLocal","","\\Resources\\LanguageLocal.xlsx"),
        //new TableConfig("Notice","","\\Notice\\Notice.xlsx"),
        //new TableConfig("Music","","\\Music\\Music.xlsx"),
        //new TableConfig("RandomName","","\\Role\\RandomName.xlsx"),
        //new TableConfig("Actor","","\\Map\\Actor.xlsx"),
        //new TableConfig("Dummy","","\\Resources\\Dummy.xlsx"),
        //new TableConfig("Language","","\\Resources\\Language.xlsx"),    
        //new TableConfig("ActivityBase","","\\Activity\\ActivityBase.xlsx"),
        //new TableConfig("ArenaBase", "", "\\Arena\\ArenaBase.xlsx"),
        //new TableConfig("ArmyBase", "", "\\Army\\ArmyBase.xlsx"),
        //new TableConfig("DefendCityBase", "", "\\Map\\DefendCityBase.xlsx"),
        //new TableConfig("EquipBase", "", "\\Equip\\EquipBase.xlsx"),
        //new TableConfig("FightBase", "", "\\Fight\\FightBase.xlsx"),
        //new TableConfig("FriendBase", "", "\\Friend\\FriendBase.xlsx"),
        //new TableConfig("GuideBase", "", "\\Task\\GuideBase.xlsx"),
        //new TableConfig("HeroBase", "", "\\Hero\\HeroBase.xlsx"),
        //new TableConfig("JumpUIBase", "", "\\Task\\JumpUIBase.xlsx"),
        //new TableConfig("LadderMatchBase", "", "\\LadderMatch\\LadderMatchBase.xlsx"),
        //new TableConfig("MonsterBase", "", "\\Fight\\MonsterBase.xlsx"),
        //new TableConfig("RoleBase", "", "\\Role\\RoleBase.xlsx"),
        //new TableConfig("RuneBase", "", "\\Rune\\RuneBase.xlsx"),
        //new TableConfig("TavernBase", "", "\\Tavern\\TavernBase.xlsx"),
        //new TableConfig("TrialBase", "", "\\Trial\\TrialBase.xlsx"),
        //new TableConfig("VipBase", "", "\\Vip\\VipBase.xlsx"),
        //new TableConfig("ChapterBase", "", "\\HeroChapter\\ChapterBase.xlsx"),
       
        //new TableConfig("MailCTable","MailCVO","\\Mail\\Mail.xlsx"),
        //new TableConfig("RuneCTable", "RuneCVO","\\Rune\\Rune.xlsx"),
        //new TableConfig("RuneOpenCTable", "RuneOpenCVO","\\Rune\\RuneOpen.xlsx"),
        //new TableConfig("ShopCTable", "ShopCVO","\\Shop\\Shop.xlsx"),
        //new TableConfig("Welfare_ActivityCTable", "Welfare_ActivityCVO","\\Activity\\ActivityList.xlsx"),
        //new TableConfig("Welfare_SevenDayGiftCTable", "Welfare_SevenDayGiftCVO","\\Activity\\Login.xlsx"),
        //new TableConfig("ShineTowerCTable", "ShineTowerCVO","\\Trial\\Trial.xlsx"),
        //new TableConfig("VipMoneyTreeCTable", "VipMoneyTreeCVO","\\Vip\\MoneyTree.xlsx"),
        //new TableConfig("VipDiamondCTable", "VipDiamondCVO","\\Vip\\VipDiamond.xlsx"),
        //new TableConfig("VipLevelCTable","VipLevelCVO","\\Vip\\VipLevel.xlsx"),
        //new TableConfig("RechargeCTable", "RechargeCVO","\\Vip\\Recharge.xlsx"),
        //new TableConfig("FightMapCTable", "FightMapCVO","\\Fight\\FightMap.xlsx"),
        //new TableConfig("FightMapGridCTable","FightMapGridCVO","\\Fight\\FightMapGrid.xlsx"),
        //new TableConfig("ShopItemCTable", "ShopItemCVO","\\Shop\\ShopExchange.xlsx"),
        //new TableConfig("CoinCTable","CoinCVO","\\Item\\Coin.xlsx"),
        //new TableConfig("GemThomasTable", "GemThomasCVO","\\Arena\\GemThomas.xlsx"),
        //new TableConfig("DialogCTable", "DialogCVO","\\Task\\Dialog.xlsx"),
        //new TableConfig("GuideCTable", "GuideCVO","\\Task\\Guide.xlsx"),
        //new TableConfig("GuideOpenCTable", "GuideOpenCVO","\\FunctionOpen\\GuideOpen.xlsx"),
        //new TableConfig("RankConfigCTable", "RankConfigCVO","\\Rank\\Rank.xlsx"),
        //new TableConfig("RankRewardCTable", "RankRewardCVO","\\Rank\\RankReward.xlsx"),
        //new TableConfig("Army_BulidCTable", "ArmyBulidCVO","\\Army\\ArmyBuilding.xlsx"),
        //new TableConfig("Army_DonateCTable", "Army_DonateCVO","\\Army\\ArmyDonate.xlsx"),
        //new TableConfig("Army_OfficalCTable", "Army_OfficalCVO","\\Army\\ArmyMember.xlsx"),
        //new TableConfig("Army_RedCTable", "Army_RedCVO","\\Army\\ArmyRed.xlsx"),
        //new TableConfig("Army_HelpCTable", "Army_HelpCVO","\\Army\\ArmyHelp.xlsx"),
        //new TableConfig("Army_LevelCTable", "Army_LevelCVO","\\Army\\ArmyLevel.xlsx"),
        //new TableConfig("RoleExpCTable", "RoleExpCVO","\\Role\\RoleExp.xlsx"),
        new TableConfig("ItemCTable", "ItemCVO","\\Item\\Item.xlsx"),
        //new TableConfig("MonsterCTable","MonsterCVO","\\Fight\\Monster.xlsx"),
        //new TableConfig("EquipStrengthenCTable", "EquipStrengthenCVO","\\Equip\\EquipStrengthen.xlsx"),
        //new TableConfig("EquipCTable", "EquipCVO","\\Equip\\Equip.xlsx"),
        //new TableConfig("HeroCTable", "HeroCVO","\\Hero\\Hero.xlsx"),
        //new TableConfig("HeroBreakCTable", "HeroBreakCVO","\\Hero\\HeroBreak.xlsx"),
        //new TableConfig("TaskCTable", "TaskCVO","\\Task\\Task.xlsx"),
        //new TableConfig("StoryBoxCTable", "StoryBoxCVO","\\Story\\StoryBox.xlsx"),
        //new TableConfig("StoryEventCTable", "StoryEventCVO","\\Story\\StoryEvent.xlsx"),
        //new TableConfig("SectioInfoCTable", "SectioInfoCVO","\\Story\\Story.xlsx"),
        //new TableConfig("LadderRewardCTable", "DanRewardCVO","\\LadderMatch\\LadderMatch.xlsx"),
        //new TableConfig("DanRewardCTable", "DanRewardCVO","\\Arena\\ArenaReward.xlsx"),
        //new TableConfig("ScenceCTable", "ScenceCVO","\\Map\\Scene.xlsx"),
        //new TableConfig("HeroTalentCTable", "HeroTalentCVO","\\Hero\\HeroTalent.xlsx"),
        //new TableConfig("HeroExpCTable", "HeroExpCVO","\\Hero\\HeroExp.xlsx"),
        //new TableConfig("SkillInfoCTable", "SkillInFoCVO","\\Skill\\Skill.xlsx"),
        //new TableConfig("SkillElementCTable", "SkillElementCVO","\\Skill\\SkillElement.xlsx"),
        //new TableConfig("SkillShowCTable", "SkillShowCVO","\\Skill\\SkillShow.xlsx"),
        //new TableConfig("SkillUpCTable", "SkillUpCVO","\\Skill\\SkillUp.xlsx"),
        //new TableConfig("BattleHallCTable", "BattleHallCVO","\\BattleHall\\BattleHall.xlsx"),
        //new TableConfig("FunctionOpenCTable", "FunctionOpenCVO","\\FunctionOpen\\FunctionOpen.xlsx"),
        //new TableConfig("ScenceNpcCTable", "ScenceNpcCVO","\\Map\\SceneNpc.xlsx"),
        //new TableConfig("SignCTable", "SignCVO","\\Activity\\Sign.xlsx"),
        //new TableConfig("Welfare_RechargeCTable", "Welfare_RechargeCVO","\\Activity\\TotalRecharge.xlsx"),
        //new TableConfig("CarnivalCTable", "CarnivalCVO","\\Activity\\Carnival.xlsx"),
        //new TableConfig("SuccinctLibraryCTable", "SuccinctLibraryCVO","\\Equip\\SuccinctLibrary.xlsx"),
        //new TableConfig("HeroTavernCTable", "HeroTavernCVO","\\Tavern\\TavernWeek.xlsx"),
        //new TableConfig("NoticeCTable", "NoticeCVO","\\Notice\\Announcement.xlsx"),
        //new TableConfig("TeamFightCTable", "TeamFightCVO","\\Team\\TeamFight.xlsx"),
        //new TableConfig("SecretFieldCTable", "SecretFieldCVO","\\Map\\SecretField.xlsx"),
        //new TableConfig("EventCTable", "EventCVO","\\Map\\Event.xlsx"),
        //new TableConfig("DailyTaskCVOCTable", "DailyTaskCVO","\\Task\\ActiveScore.xlsx"),
        //new TableConfig("ChapterCTable","chapterCVO","\\HeroChapter\\Chapter.xlsx"),
        //new TableConfig("ChapterRewardCTable","ChapterRewardCVO","\\HeroChapter\\ChapterReward.xlsx"),
        //new TableConfig("Welfare_PlayerLvCTable","Welfare_PlayerLvCVO","\\Activity\\LevelReward.xlsx"),
        //new TableConfig("Welfare_FirstRechargeCTable","Welfare_FirstRechargeCVO","\\Activity\\FirstCharge.xlsx"),
        //new TableConfig("Welfare_FianceCTable","Welfare_FianceCVO","\\Activity\\FiancePlan.xlsx"),
        //new TableConfig("Welfare_ConsumeCTable","Welfare_ConsumeCVO","\\Activity\\TotalConsume.xlsx")
    };

        private static CSVPathEditor _Instance;
        public static CSVPathEditor Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = (CSVPathEditor)AssetDatabase.LoadMainAssetAtPath(ConfigEditor.Path_EditorConfig);
                return _Instance;
            }
        }

    }

    [Serializable]
    public class TableConfig
    {
        public TableConfig(string tableCalssName, string rowClassName, string shortPath)
        {
            this.tableClassName = tableCalssName;
            this.rowClassName = rowClassName;
            this.shortPath = shortPath;
        }

        public string tableClassName;
        public string rowClassName;
        public string shortPath;
    }
}
