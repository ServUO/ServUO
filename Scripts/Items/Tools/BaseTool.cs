using System;
using Server.Engines.Craft;
using Server.Network;
using Server.ContextMenus;
using System.Collections.Generic;

namespace Server.Items
{
    public interface ITool : IUsesRemaining
    {
        CraftSystem CraftSystem { get; }
        bool BreakOnDepletion { get; }
        bool Deleted { get; }
        void Delete();
        bool CheckAccessible(Mobile from, ref int num);
    }

    public abstract class BaseTool : Item, ITool, IResource
    {
        private Mobile m_Crafter;
        private ItemQuality m_Quality;
        private int m_UsesRemaining;
        private bool m_RepairMode;
        private CraftResource _Resource;
        private bool _PlayerConstructed;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return _Resource; }
            set
            {
                _Resource = value;
                Hue = CraftResources.GetHue(_Resource);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set { m_Crafter = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality
        {
            get { return m_Quality; }
            set
            {
                UnscaleUses();
                m_Quality = value;
                InvalidateProperties();
                ScaleUses();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed
        {
            get { return _PlayerConstructed; }
            set
            {
                _PlayerConstructed = value; InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RepairMode
        {
            get { return m_RepairMode; }
            set { m_RepairMode = value; }
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
            get { return true; }
            set { }
        }

        public virtual bool BreakOnDepletion { get { return true; } }

        public abstract CraftSystem CraftSystem { get; }

        public BaseTool(int itemID)
            : this(Utility.RandomMinMax(25, 75), itemID)
        {
        }

        public BaseTool(int uses, int itemID)
            : base(itemID)
        {
            m_UsesRemaining = uses;
            m_Quality = ItemQuality.Normal;
        }

        public BaseTool(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Crafter != null)
                list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (m_Quality == ItemQuality.Exceptional)
                list.Add(1060636); // exceptional

            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public virtual void DisplayDurabilityTo(Mobile m)
        {
            LabelToAffix(m, 1017323, AffixType.Append, ": " + m_UsesRemaining.ToString()); // Durability
        }

        public virtual bool CheckAccessible(Mobile m, ref int num)
        {
            if (!IsChildOf(m) && Parent != m)
            {
                num = 1044263;
                return false;
            }

            return true;
        }

        public static bool CheckAccessible(Item tool, Mobile m)
        {
            return CheckAccessible(tool, m, false);
        }

        public static bool CheckAccessible(Item tool, Mobile m, bool message)
        {
            var num = 0;

            bool res;

            if (tool is ITool)
            {
                res = ((ITool)tool).CheckAccessible(m, ref num);
            }
            else
            {
                res = tool.IsChildOf(m) || tool.Parent == m;
            }

            if (num > 0 && message)
            {
                m.SendLocalizedMessage(num);
            }

            return res;
        }

        public static bool CheckTool(Item tool, Mobile m)
        {
            Item check = m.FindItemOnLayer(Layer.OneHanded);

            if (check is ITool && check != tool && !(check is AncientSmithyHammer))
                return false;

            check = m.FindItemOnLayer(Layer.TwoHanded);

            if (check is ITool && check != tool && !(check is AncientSmithyHammer))
                return false;

            return true;
        }

        public override void OnSingleClick(Mobile from)
        {
            DisplayDurabilityTo(from);

            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) || Parent == from)
            {
                CraftSystem system = CraftSystem;

                if (Core.TOL && m_RepairMode)
                {
                    Repair.Do(from, system, this);
                }
                else
                {
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
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)4); // version

            writer.Write(_PlayerConstructed);

            writer.Write((int)_Resource);
            writer.Write(m_RepairMode);
            writer.Write((Mobile)m_Crafter);
            writer.Write((int)m_Quality);
            writer.Write((int)m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                    {
                        _PlayerConstructed = reader.ReadBool();
                        goto case 3;
                    }
                case 3:
                    {
                        _Resource = (CraftResource)reader.ReadInt();
                        goto case 2;
                    }
                case 2:
                    {
                        m_RepairMode = reader.ReadBool();
                        goto case 1;
                    }
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
            PlayerConstructed = true;

            Quality = (ItemQuality)quality;

            if (makersMark)
                Crafter = from;

            /*if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                    typeRes = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(typeRes);
            }*/

            return quality;
        }
        #endregion
    }
}