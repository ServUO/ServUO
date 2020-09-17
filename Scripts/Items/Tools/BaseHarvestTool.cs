using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Engines.Harvest;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public enum MiningType
    {
        OreOnly,
        OreAndStone,
        OreAndGems,
        StoneOnly,
    }

    public abstract class BaseHarvestTool : Item, IUsesRemaining, ICraftable, IHarvestTool
    {
        private Mobile m_Crafter;
        private ItemQuality m_Quality;
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return m_Crafter;
            }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality
        {
            get
            {
                return m_Quality;
            }
            set
            {
                UnscaleUses();
                m_Quality = value;
                InvalidateProperties();
                ScaleUses();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return m_UsesRemaining;
            }
            set
            {
                m_UsesRemaining = value;
                InvalidateProperties();
            }
        }

        public void ScaleUses()
        {
            m_UsesRemaining = (m_UsesRemaining * GetUsesScalar()) / 100;
            InvalidateProperties();
        }

        public void UnscaleUses()
        {
            m_UsesRemaining = (m_UsesRemaining * 100) / GetUsesScalar();
        }

        public int GetUsesScalar()
        {
            if (m_Quality == ItemQuality.Exceptional)
                return 200;

            return 100;
        }

        public bool ShowUsesRemaining
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public virtual bool BreakOnDepletion => true;

        public abstract HarvestSystem HarvestSystem { get; }

        public BaseHarvestTool(int itemID)
            : this(50, itemID)
        {
        }

        public BaseHarvestTool(int usesRemaining, int itemID)
            : base(itemID)
        {
            m_UsesRemaining = usesRemaining;
            m_Quality = ItemQuality.Normal;
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (m_Quality == ItemQuality.Exceptional)
                list.Add(1060636); // exceptional
        }

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public virtual void DisplayDurabilityTo(Mobile m)
        {
            LabelToAffix(m, 1017323, AffixType.Append, ": " + m_UsesRemaining.ToString()); // Durability
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) || Parent == from)
                HarvestSystem.BeginHarvesting(from, this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            AddContextMenuEntries(from, this, list, HarvestSystem);
        }

        public static void AddContextMenuEntries(Mobile from, Item item, List<ContextMenuEntry> list, HarvestSystem system)
        {
            if (system != Mining.System)
                return;

            if (!item.IsChildOf(from.Backpack) && item.Parent != from)
                return;

            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return;

            int typeentry = 0;

            if (pm.ToggleMiningStone)
                typeentry = 6179;
            if (pm.ToggleMiningGem)
                typeentry = 1112239;
            if (pm.ToggleStoneOnly)
                typeentry = 1156865;
            if (!pm.ToggleMiningStone && !pm.ToggleMiningGem && !pm.ToggleStoneOnly)
                typeentry = 6178;

            ContextMenuEntry miningEntry = new ContextMenuEntry(typeentry)
            {
                Color = 0x421F
            };
            list.Add(miningEntry);

            list.Add(new ToggleMiningStoneEntry(pm, MiningType.OreOnly, 6176));         // Set To Ore
            list.Add(new ToggleMiningStoneEntry(pm, MiningType.OreAndStone, 6177));     // Set To Ore and Stone
            list.Add(new ToggleMiningStoneEntry(pm, MiningType.OreAndGems, 1112237));   // Set To Ore and Gems
            list.Add(new ToggleMiningStoneEntry(pm, MiningType.StoneOnly, 1156864));    // Set To Stone
        }

        public class ToggleMiningStoneEntry : ContextMenuEntry
        {
            private readonly PlayerMobile m_Mobile;
            private readonly MiningType MiningType;
            //private bool m_Valuestone;
            //private bool m_Valuegem;

            public ToggleMiningStoneEntry(PlayerMobile mobile, MiningType type, int number)
                : base(number)
            {
                m_Mobile = mobile;
                MiningType = type;
                //m_Valuestone = valuestone;
                //m_Valuegem = valuegem;

                bool canMineStone = (mobile.StoneMining && mobile.Skills[SkillName.Mining].Base >= 100.0);
                bool canMineGems = (mobile.GemMining && mobile.Skills[SkillName.Mining].Base >= 100.0);

                switch (type)
                {
                    case MiningType.OreOnly:
                        if (!mobile.ToggleMiningStone && !mobile.ToggleMiningGem && !mobile.ToggleStoneOnly)
                            Flags |= CMEFlags.Disabled;
                        break;
                    case MiningType.OreAndStone:
                        if (mobile.ToggleMiningStone || !canMineStone)
                            Flags |= CMEFlags.Disabled;
                        break;
                    case MiningType.OreAndGems:
                        if (mobile.ToggleMiningGem || !canMineGems)
                            Flags |= CMEFlags.Disabled;
                        break;
                    case MiningType.StoneOnly:
                        if (mobile.ToggleStoneOnly || !canMineStone)
                            Flags |= CMEFlags.Disabled;
                        break;
                }

                /*if (valuestone && mobile.ToggleMiningStone == valuestone || (valuestone && !stoneMining))
                    Flags |= CMEFlags.Disabled;
                else if (valuegem && mobile.ToggleMiningGem == valuegem || (valuegem && !gemMining))
                    Flags |= CMEFlags.Disabled;
                else if (!valuestone && !valuegem && !mobile.ToggleMiningStone && !mobile.ToggleMiningGem)
                    Flags |= CMEFlags.Disabled;*/
            }

            public override void OnClick()
            {
                bool oldValuestoneandore = m_Mobile.ToggleMiningStone;
                bool oldValuegem = m_Mobile.ToggleMiningGem;
                bool oldValuestone = m_Mobile.ToggleStoneOnly;

                switch (MiningType)
                {
                    case MiningType.OreAndStone:
                        {
                            if (oldValuestoneandore)
                            {
                                m_Mobile.SendLocalizedMessage(1054023); // You are already set to mine both ore and stone!
                            }
                            else if (!m_Mobile.StoneMining || m_Mobile.Skills[SkillName.Mining].Base < 100.0)
                            {
                                m_Mobile.SendLocalizedMessage(1054024); // You have not learned how to mine stone or you do not have enough skill!
                            }
                            else
                            {
                                m_Mobile.ToggleMiningStone = true;
                                m_Mobile.ToggleMiningGem = false;
                                m_Mobile.ToggleStoneOnly = false;
                                m_Mobile.SendLocalizedMessage(1054022); // You are now set to mine both ore and stone.
                            }
                        }
                        break;
                    case MiningType.OreAndGems:
                        {
                            if (oldValuegem)
                            {
                                m_Mobile.SendLocalizedMessage(1112235); // You are already set to mine both ore and gems!
                            }
                            else if (!m_Mobile.GemMining || m_Mobile.Skills[SkillName.Mining].Base < 100.0)
                            {
                                m_Mobile.SendLocalizedMessage(1112234); // You have not learned how to mine gems or you do not have enough skill!
                            }
                            else
                            {
                                m_Mobile.ToggleMiningGem = true;
                                m_Mobile.ToggleMiningStone = false;
                                m_Mobile.ToggleStoneOnly = false;
                                m_Mobile.SendLocalizedMessage(1112236); // You are now set to mine both ore and gems.
                            }
                        }
                        break;
                    case MiningType.StoneOnly:
                        {
                            if (oldValuestone)
                            {
                                m_Mobile.SendLocalizedMessage(1156866); // You are already mining only stone.
                            }
                            else if (!m_Mobile.StoneMining || m_Mobile.Skills[SkillName.Mining].Base < 100.0)
                            {
                                m_Mobile.SendLocalizedMessage(1054024); // You have not learned how to mine stone or you do not have enough skill!
                            }
                            else
                            {
                                m_Mobile.ToggleMiningStone = false;
                                m_Mobile.ToggleMiningGem = false;
                                m_Mobile.ToggleStoneOnly = true;
                                m_Mobile.SendLocalizedMessage(1156867); // You are now set to mine only stone.
                            }
                        }
                        break;
                    default:
                        {
                            if (oldValuestoneandore || oldValuegem || oldValuestone)
                            {
                                m_Mobile.ToggleMiningStone = false;
                                m_Mobile.ToggleMiningGem = false;
                                m_Mobile.ToggleStoneOnly = false;
                                m_Mobile.SendLocalizedMessage(1054020); // You are now set to mine only ore.
                            }
                            else
                            {
                                m_Mobile.SendLocalizedMessage(1054021); // You are already set to mine only ore!
                            }
                        }
                        break;
                }
            }
        }

        public BaseHarvestTool(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(m_Crafter);
            writer.Write((int)m_Quality);

            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_Crafter = reader.ReadMobile();
                        m_Quality = (ItemQuality)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        m_UsesRemaining = reader.ReadInt();
                        break;
                    }
            }
        }

        #region ICraftable Members
        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (makersMark)
                Crafter = from;

            return quality;
        }
        #endregion
    }
}
