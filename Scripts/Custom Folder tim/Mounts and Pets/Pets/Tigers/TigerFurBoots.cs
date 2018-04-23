//Created by DelBoy aka Fury on 18/02/14

using Server;
using Server.Items;
using System;

namespace Server.Items
{
    public class TigerFurBoots : FurBoots
    {
        public string SkillBonus;
        private SkillMod m_SkillMod0;

        [Constructable]
        public TigerFurBoots()
        {
            Name = "Tiger Fur Boots";
            this.Hue = 1359;
            base.Attributes.Luck = 134;

            DefineMods();
        }

        private void DefineMods()
        {
            int value = Utility.RandomMinMax(1, 10);
            switch (Utility.Random(58))
            {
                case 0:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.ArmsLore, true, value);
                        SkillBonus = String.Format("Increases Arms Lore by {0}%", value);
                        break;
                    }

                case 1:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Begging, true, value);
                        SkillBonus = String.Format("Increases Begging by {0}%", value);
                        break;
                    }

                case 2:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Camping, true, value);
                        SkillBonus = String.Format("Increases Camping by {0}%", value);
                        break;
                    }

                case 3:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Cartography, true, value);
                        SkillBonus = String.Format("Increases Cartography by {0}%", value);
                        break;
                    }

                case 4:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Forensics, true, value);
                        SkillBonus = String.Format("Increases Forensics by {0}%", value);
                        break;
                    }

                case 5:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.ItemID, true, value);
                        SkillBonus = String.Format("Increases Item ID by {0}%", value);
                        break;
                    }

                case 6:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.TasteID, true, value);
                        SkillBonus = String.Format("Increases Taste ID by {0}%", value);
                        break;
                    }

                case 7:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Anatomy, true, value);
                        SkillBonus = String.Format("Increases Anatomy by {0}%", value);
                        break;
                    }

                case 8:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Archery, true, value);
                        SkillBonus = String.Format("Increases Archery by {0}%", value);
                        break;
                    }

                case 9:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Fencing, true, value);
                        SkillBonus = String.Format("Increases Fencing by {0}%", value);
                        break;
                    }

                case 10:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Focus, true, value);
                        SkillBonus = String.Format("Increases Focus by {0}%", value);
                        break;
                    }

                case 11:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Healing, true, value);
                        SkillBonus = String.Format("Increases Healing by {0}%", value);
                        break;
                    }

                case 12:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Macing, true, value);
                        SkillBonus = String.Format("Increases Mace Fighting by {0}%", value);
                        break;
                    }

                case 13:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Parry, true, value);
                        SkillBonus = String.Format("Increases Parry by {0}%", value);
                        break;
                    }

                case 14:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Swords, true, value);
                        SkillBonus = String.Format("Increases Swordsmanship by {0}%", value);
                        break;
                    }

                case 15:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Tactics, true, value);
                        SkillBonus = String.Format("Increases Tactics by {0}%", value);
                        break;
                    }

                case 16:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Throwing, true, value);
                        SkillBonus = String.Format("Increases Throwing by {0}%", value);
                        break;
                    }

                case 17:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Wrestling, true, value);
                        SkillBonus = String.Format("Increases Wrestling by {0}%", value);
                        break;
                    }

                case 18:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Alchemy, true, value);
                        SkillBonus = String.Format("Increases Alchemy by {0}%", value);
                        break;
                    }

                case 19:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Blacksmith, true, value);
                        SkillBonus = String.Format("Increases Blacksmithy by {0}%", value);
                        break;
                    }

                case 20:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Fletching, true, value);
                        SkillBonus = String.Format("Increases Bowcraft/Fletching by {0}%", value);
                        break;
                    }

                case 21:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Carpentry, true, value);
                        SkillBonus = String.Format("Increases Carpentry by {0}%", value);
                        break;
                    }

                case 22:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Cooking, true, value);
                        SkillBonus = String.Format("Increases Cooking by {0}%", value);
                        break;
                    }

                case 23:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Inscribe, true, value);
                        SkillBonus = String.Format("Increases Inscription by {0}%", value);
                        break;
                    }

                case 24:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Lumberjacking, true, value);
                        SkillBonus = String.Format("Increases Lumberjacking by {0}%", value);
                        break;
                    }

                case 25:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Mining, true, value);
                        SkillBonus = String.Format("Increases Mining by {0}%", value);
                        break;
                    }

                case 26:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Tailoring, true, value);
                        SkillBonus = String.Format("Increases Tailoring by {0}%", value);
                        break;
                    }

                case 27:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Tinkering, true, value);
                        SkillBonus = String.Format("Increases Tinkering by {0}%", value);
                        break;
                    }

                case 28:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Bushido, true, value);
                        SkillBonus = String.Format("Increases Bushido by {0}%", value);
                        break;
                    }

                case 29:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Chivalry, true, value);
                        SkillBonus = String.Format("Increases Chivalry by {0}%", value);
                        break;
                    }

                case 30:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.EvalInt, true, value);
                        SkillBonus = String.Format("Increases Evaluating Intelligence by {0}%", value);
                        break;
                    }

                case 31:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Imbuing, true, value);
                        SkillBonus = String.Format("Increases Imbuing by {0}%", value);
                        break;
                    }

                case 32:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Magery, true, value);
                        SkillBonus = String.Format("Increases Magery by {0}%", value);
                        break;
                    }

                case 33:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Meditation, true, value);
                        SkillBonus = String.Format("Increases Meditation by {0}%", value);
                        break;
                    }

                case 34:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Mysticism, true, value);
                        SkillBonus = String.Format("Increases Mysticism by {0}%", value);
                        break;
                    }

                case 35:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Necromancy, true, value);
                        SkillBonus = String.Format("Increases Necromancy by {0}%", value);
                        break;
                    }

                case 36:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Ninjitsu, true, value);
                        SkillBonus = String.Format("Increases Ninjitsu by {0}%", value);
                        break;
                    }

                case 37:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.MagicResist, true, value);
                        SkillBonus = String.Format("Increases Resisting Spells by {0}%", value);
                        break;
                    }

                case 38:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Spellweaving, true, value);
                        SkillBonus = String.Format("Increases Spellweaving by {0}%", value);
                        break;
                    }

                case 39:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.SpiritSpeak, true, value);
                        SkillBonus = String.Format("Increases Spirit Speak by {0}%", value);
                        break;
                    }

                case 40:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.AnimalLore, true, value);
                        SkillBonus = String.Format("Increases Animal Lore by {0}%", value);
                        break;
                    }

                case 41:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.AnimalTaming, true, value);
                        SkillBonus = String.Format("Increases Animal Taming by {0}%", value);
                        break;
                    }

                case 42:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Fishing, true, value);
                        SkillBonus = String.Format("Increases Fishing by {0}%", value);
                        break;
                    }

                case 43:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Herding, true, value);
                        SkillBonus = String.Format("Increases Herding by {0}%", value);
                        break;
                    }

                case 44:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Tracking, true, value);
                        SkillBonus = String.Format("Increases Tracking by {0}%", value);
                        break;
                    }

                case 45:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Veterinary, true, value);
                        SkillBonus = String.Format("Increases Veterinary by {0}%", value);
                        break;
                    }

                case 46:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.DetectHidden, true, value);
                        SkillBonus = String.Format("Increases Detecting Hidden by {0}%", value);
                        break;
                    }

                case 47:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Hiding, true, value);
                        SkillBonus = String.Format("Increases Hiding by {0}%", value);
                        break;
                    }

                case 48:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Lockpicking, true, value);
                        SkillBonus = String.Format("Increases Lockpicking by {0}%", value);
                        break;
                    }

                case 49:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Poisoning, true, value);
                        SkillBonus = String.Format("Increases Poisoning by {0}%", value);
                        break;
                    }

                case 50:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.RemoveTrap, true, value);
                        SkillBonus = String.Format("Increases Remove Trap by {0}%", value);
                        break;
                    }

                case 51:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Snooping, true, value);
                        SkillBonus = String.Format("Increases Snooping by {0}%", value);
                        break;
                    }

                case 52:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Stealing, true, value);
                        SkillBonus = String.Format("Increases Stealing by {0}%", value);
                        break;
                    }

                case 53:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Stealth, true, value);
                        SkillBonus = String.Format("Increases Stealth by {0}%", value);
                        break;
                    }

                case 54:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Discordance, true, value);
                        SkillBonus = String.Format("Increases Discordance by {0}%", value);
                        break;
                    }

                case 55:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Musicianship, true, value);
                        SkillBonus = String.Format("Increases Musicianship by {0}%", value);
                        break;
                    }

                case 56:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Peacemaking, true, value);
                        SkillBonus = String.Format("Increases Peacemaking by {0}%", value);
                        break;
                    }

                case 57:
                    {
                        m_SkillMod0 = new DefaultSkillMod(SkillName.Provocation, true, value);
                        SkillBonus = String.Format("Increases Provocation by {0}%", value);
                        break;
                    }
            }
        }

        private void SetMods(Mobile wearer)
        {
            wearer.AddSkillMod(m_SkillMod0);
        }

        public override bool OnEquip(Mobile from)
        {
            SetMods(from);
            return true;
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile m = (Mobile)parent;
                m.RemoveStatMod("TigerFurBoots");

                if (m_SkillMod0 != null)
                    m_SkillMod0.Remove();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(SkillBonus);
        }

        public TigerFurBoots(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

    }
}

