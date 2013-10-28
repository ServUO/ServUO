using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Engines.Harvest;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public interface IUsesRemaining
    {
        int UsesRemaining { get; set; }
        bool ShowUsesRemaining { get; set; }
    }

    public abstract class BaseHarvestTool : Item, IUsesRemaining, ICraftable
    {
        private Mobile m_Crafter;
        private ToolQuality m_Quality;
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return this.m_Crafter;
            }
            set
            {
                this.m_Crafter = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ToolQuality Quality
        {
            get
            {
                return this.m_Quality;
            }
            set
            {
                this.UnscaleUses();
                this.m_Quality = value;
                this.InvalidateProperties();
                this.ScaleUses();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return this.m_UsesRemaining;
            }
            set
            {
                this.m_UsesRemaining = value;
                this.InvalidateProperties();
            }
        }

        public void ScaleUses()
        {
            this.m_UsesRemaining = (this.m_UsesRemaining * this.GetUsesScalar()) / 100;
            this.InvalidateProperties();
        }

        public void UnscaleUses()
        {
            this.m_UsesRemaining = (this.m_UsesRemaining * 100) / this.GetUsesScalar();
        }

        public int GetUsesScalar()
        {
            if (this.m_Quality == ToolQuality.Exceptional)
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

        public abstract HarvestSystem HarvestSystem { get; }

        public BaseHarvestTool(int itemID)
            : this(50, itemID)
        {
        }

        public BaseHarvestTool(int usesRemaining, int itemID)
            : base(itemID)
        {
            this.m_UsesRemaining = usesRemaining;
            this.m_Quality = ToolQuality.Regular;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            // Makers mark not displayed on OSI
            //if ( m_Crafter != null )
            //	list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

            if (this.m_Quality == ToolQuality.Exceptional)
                list.Add(1060636); // exceptional

            list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public virtual void DisplayDurabilityTo(Mobile m)
        {
            this.LabelToAffix(m, 1017323, AffixType.Append, ": " + this.m_UsesRemaining.ToString()); // Durability
        }

        public override void OnSingleClick(Mobile from)
        {
            this.DisplayDurabilityTo(from);

            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack) || this.Parent == from)
                this.HarvestSystem.BeginHarvesting(from, this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            AddContextMenuEntries(from, this, list, this.HarvestSystem);
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
            if (!pm.ToggleMiningStone && !pm.ToggleMiningGem)
                typeentry = 6178;

            ContextMenuEntry miningEntry = new ContextMenuEntry(typeentry);
            miningEntry.Color = 0x421F;
            list.Add(miningEntry);

            list.Add(new ToggleMiningStoneEntry(pm, false, false, 6176));
            list.Add(new ToggleMiningStoneEntry(pm, true, false, 6177));
            list.Add(new ToggleMiningStoneEntry(pm, false, true, 1112237));
        }

        private class ToggleMiningStoneEntry : ContextMenuEntry
        {
            private readonly PlayerMobile m_Mobile;
            private bool m_Valuestone;
            private bool m_Valuegem;

            public ToggleMiningStoneEntry(PlayerMobile mobile, bool valuestone, bool valuegem, int number)
                : base(number)
            {
                this.m_Mobile = mobile;
                this.m_Valuestone = valuestone;
                this.m_Valuegem = valuegem;

                bool stoneMining = (mobile.StoneMining && mobile.Skills[SkillName.Mining].Base >= 100.0);
                bool gemMining = (mobile.GemMining && mobile.Skills[SkillName.Mining].Base >= 100.0);

                if (valuestone && mobile.ToggleMiningStone == valuestone || (valuestone && !stoneMining))
                    this.Flags |= CMEFlags.Disabled;
                else if (valuegem && mobile.ToggleMiningGem == valuegem || (valuegem && !gemMining))
                    this.Flags |= CMEFlags.Disabled;
                else if (!valuestone && !valuegem && !mobile.ToggleMiningStone && !mobile.ToggleMiningGem)
                    this.Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                bool oldValuestone = this.m_Mobile.ToggleMiningStone;
                bool oldValuegem = this.m_Mobile.ToggleMiningGem;

                if (this.m_Valuestone)
                {
                    if (oldValuestone)
                    {
                        this.m_Mobile.SendLocalizedMessage(1054023); // You are already set to mine both ore and stone!
                    }
                    else if (!this.m_Mobile.StoneMining || this.m_Mobile.Skills[SkillName.Mining].Base < 100.0)
                    {
                        this.m_Mobile.SendLocalizedMessage(1054024); // You have not learned how to mine stone or you do not have enough skill!
                    }
                    else
                    {
                        this.m_Mobile.ToggleMiningStone = true;
                        this.m_Mobile.ToggleMiningGem = false;
                        this.m_Mobile.SendLocalizedMessage(1054022); // You are now set to mine both ore and stone.
                    }
                }
                else if (this.m_Valuegem)
                {
                    if (oldValuegem)
                    {
                        this.m_Mobile.SendLocalizedMessage(1112235); // You are already set to mine both ore and gems!
                    }
                    else if (!this.m_Mobile.GemMining || this.m_Mobile.Skills[SkillName.Mining].Base < 100.0)
                    {
                        this.m_Mobile.SendLocalizedMessage(1112234); // You have not learned how to mine gems or you do not have enough skill!
                    }
                    else
                    {
                        this.m_Mobile.ToggleMiningGem = true;
                        this.m_Mobile.ToggleMiningStone = false;
                        this.m_Mobile.SendLocalizedMessage(1112236); // You are now set to mine both ore and gems.
                    }
                }
                else
                {
                    if (oldValuestone || oldValuegem)
                    {
                        this.m_Mobile.ToggleMiningStone = false;
                        this.m_Mobile.ToggleMiningGem = false;
                        this.m_Mobile.SendLocalizedMessage(1054020); // You are now set to mine only ore.
                    }
                    else
                    {
                        this.m_Mobile.SendLocalizedMessage(1054021); // You are already set to mine only ore!
                    }
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

            writer.Write((int)1); // version

            writer.Write((Mobile)this.m_Crafter);
            writer.Write((int)this.m_Quality);

            writer.Write((int)this.m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Crafter = reader.ReadMobile();
                        this.m_Quality = (ToolQuality)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_UsesRemaining = reader.ReadInt();
                        break;
                    }
            }
        }

        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            this.Quality = (ToolQuality)quality;

            if (makersMark)
                this.Crafter = from;

            return quality;
        }
        #endregion
    }
}