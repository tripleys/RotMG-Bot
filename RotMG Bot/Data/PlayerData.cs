using RotMG_Net_Lib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot.Data
{
    public class PlayerData
    {

        public int ObjectId;

        public WorldPosData Position;

        public string Name;

        public int Level;

        public int Exp;

        public int CurrentFame;

        public int Stars;

        public string AccountId;

        public int AccountFame;

        public bool NameChosen;

        public string GuildName;

        public int GuildRank;

        public int Gold;

        public int ClassType;

        public int MaxHP;

        public int MaxMP;

        public int Hp;

        public int Mp;

        public int Attack;

        public int Defense;

        public int Speed;

        public int Dexterity;

        public int Wisdom;

        public int Vitality;

        public int Condition;

        public int Skin;

        public int HpPots;

        public int MpPots;

        public bool HasBackpack;

        public int[] Inventory;

        public static PlayerData processObjectStatus(ObjectStatusData osd)
        {
            PlayerData playerData = processStatData(osd.Stats, new PlayerData());
            playerData.Position = osd.Pos;
            playerData.ObjectId = osd.ObjectId;
            return playerData;
        }

        public static PlayerData processStatData(StatData[] statData, PlayerData playerData)
        {
            if (playerData.Inventory == null)
            {
                playerData.Inventory = new int[20];
                for (int i = 0; i < playerData.Inventory.Length; i++)
                    playerData.Inventory[i] = -1;
            }
            foreach (StatData stat in statData)
            {
                switch (stat.StatType)
                {
                    case StatData.NAME_STAT:
                        playerData.Name = stat.StringValue;
                        continue;
                    case StatData.LEVEL_STAT:
                        playerData.Level = stat.StatValue;
                        continue;
                    case StatData.EXP_STAT:
                        playerData.Exp = stat.StatValue;
                        continue;
                    case StatData.CURR_FAME_STAT:
                        playerData.CurrentFame = stat.StatValue;
                        continue;
                    case StatData.NUM_STARS_STAT:
                        playerData.Stars = stat.StatValue;
                        continue;
                    case StatData.ACCOUNT_ID_STAT:
                        playerData.AccountId = stat.StringValue;
                        continue;
                    case StatData.FAME_STAT:
                        playerData.AccountFame = stat.StatValue;
                        continue;
                    case StatData.CREDITS_STAT:
                        playerData.Gold = stat.StatValue;
                        continue;
                    case StatData.TEXTURE_STAT:
                        playerData.Skin = stat.StatValue;
                        continue;
                    case StatData.MAX_HP_STAT:
                        playerData.MaxHP = stat.StatValue;
                        continue;
                    case StatData.MAX_MP_STAT:
                        playerData.MaxMP = stat.StatValue;
                        continue;
                    case StatData.HP_STAT:
                        playerData.Hp = stat.StatValue;
                        continue;
                    case StatData.MP_STAT:
                        playerData.Mp = stat.StatValue;
                        continue;
                    case StatData.ATTACK_STAT:
                        playerData.Attack = stat.StatValue;
                        continue;
                    case StatData.DEFENSE_STAT:
                        playerData.Defense = stat.StatValue;
                        continue;
                    case StatData.SPEED_STAT:
                        playerData.Speed = stat.StatValue;
                        continue;
                    case StatData.DEXTERITY_STAT:
                        playerData.Dexterity = stat.StatValue;
                        continue;
                    case StatData.VITALITY_STAT:
                        playerData.Vitality = stat.StatValue;
                        continue;
                    case StatData.WISDOM_STAT:
                        playerData.Wisdom = stat.StatValue;
                        continue;
                    case StatData.CONDITION_STAT:
                        playerData.Condition = stat.StatValue;
                        continue;
                    case StatData.HEALTH_POTION_STACK_STAT:
                        playerData.HpPots = stat.StatValue;
                        continue;
                    case StatData.MAGIC_POTION_STACK_STAT:
                        playerData.MpPots = stat.StatValue;
                        continue;
                    case StatData.HASBACKPACK_STAT:
                        playerData.HasBackpack = stat.StatValue == 1;
                        continue;
                    case StatData.NAME_CHOSEN_STAT:
                        playerData.NameChosen = stat.StatValue != 0;
                        continue;
                    case StatData.GUILD_NAME_STAT:
                        playerData.GuildName = stat.StringValue;
                        continue;
                    case StatData.GUILD_RANK_STAT:
                        playerData.GuildRank = stat.StatValue;
                        continue;
                    default:
                        if (stat.StatType >= StatData.INVENTORY_0_STAT && stat.StatType <= StatData.INVENTORY_11_STAT)
                        {
                            playerData.Inventory[stat.StatType - 8] = stat.StatValue;
                        }
                        else if (stat.StatType >= StatData.BACKPACK_0_STAT && stat.StatType <= StatData.BACKPACK_7_STAT)
                        {
                            playerData.Inventory[stat.StatType - 59] = stat.StatValue;
                        }
                        continue;
                }
            }
            return playerData;
        }
    }
}
