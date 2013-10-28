using System;
using Server.Engines.Craft;
using Server.Network;

namespace Server.Items
{
    public enum ToolQuality
    {
        Low,
        Regular,
        Exceptional
    }

    public abstract class BaseTool : Item, IUsesRemaining, ICraftable
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

        public abstract CraftSystem CraftSystem { get; }

        public BaseTool(int itemID)
            : this(Utility.RandomMinMax(25, 75), itemID)
        {
        }

        public BaseTool(int uses, int itemID)
            : base(itemID)
        {
            this.m_UsesRemaining = uses;
            this.m_Quality = ToolQuality.Regular;
        }

        public BaseTool(Serial serial)
            : base(serial)
        {
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

        public static bool CheckAccessible(Item tool, Mobile m)
        {
            return (tool.IsChildOf(m) || tool.Parent == m);
        }

        public static bool CheckTool(Item tool, Mobile m)
        {
            Item check = m.FindItemOnLayer(Layer.OneHanded);

            if (check is BaseTool && check != tool && !(check is AncientSmithyHammer))
                return false;

            check = m.FindItemOnLayer(Layer.TwoHanded);

            if (check is BaseTool && check != tool && !(check is AncientSmithyHammer))
                return false;

            return true;
        }

        public override void OnSingleClick(Mobile from)
        {
            this.DisplayDurabilityTo(from);

            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack) || this.Parent == from)
            {
                CraftSystem system = this.CraftSystem;

                int num = system.CanCraft(from, this, null);

                if (num > 0 && (num != 1044267 || !Core.SE)) // Blacksmithing shows the gump regardless of proximity of an anvil and forge after SE
                {
                    from.SendLocalizedMessage(num);
                }
                else
                {
                    CraftContext context = system.GetContext(from);

                    from.SendGump(new CraftGump(from, system, this, null));
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
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