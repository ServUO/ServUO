using System;
using Server.Gumps;
using Server.Multis;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    interface IEngravable
    {
        string EngravedText { get; set; }
    }

    public class BaseEngravingTool : Item, IUsesRemaining
    { 
        private int m_UsesRemaining;

        [Constructable]
        public BaseEngravingTool(int itemID)
            : this(itemID, 1)
        {
        }

        [Constructable]
        public BaseEngravingTool(int itemID, int uses)
            : base(itemID)
        {
            Weight = 1.0;
            Hue = 0x48D;

            LootType = LootType.Blessed;

            m_UsesRemaining = uses;
        }

        public BaseEngravingTool(Serial serial)
            : base(serial)
        {
        }

        public virtual Type[] Engraves { get { return null; } }
        public virtual int GumpTitle { get { return 1072359; } }

        public virtual int SuccessMessage { get { return 1072361; } } // // You engraved the object.
        public virtual int TargetMessage { get { return 1072357; } } // Select an object to engrave.
        public virtual int RemoveMessage { get { return 1072362; } } // You remove the engraving from the object.
        public virtual int OutOfChargesMessage { get { return 1042544; } } // This item is out of charges.
        public virtual int NotAccessibleMessage { get { return 1072310; } } // The selected item is not accessible to engrave.
        public virtual int CannotEngraveMessage { get { return 1072309; } } // The selected item cannot be engraved by this engraving tool.

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

        public virtual bool ShowUsesRemaining
        { 
            get
            {
                return true;
            }
            set
            {
            }
        }

        public virtual bool CheckType(IEntity entity)
        {
            if (Engraves == null || entity == null)
                return false;

            Type type = entity.GetType();
				
            for (int i = 0; i < Engraves.Length; i ++)
            { 
                if (type == Engraves[i] || type.IsSubclassOf(Engraves[i]))
                    return true;
            }
			
            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
			
            if (!from.NetState.SupportsExpansion(Expansion.ML))
            {
                from.SendLocalizedMessage(1072791); // You must upgrade to Mondain's Legacy in order to use that item.				
                return;
            }
			
            if (m_UsesRemaining > 0)
            {
                from.SendLocalizedMessage(TargetMessage);
                from.Target = new InternalTarget(this);
            }
            else
                from.SendLocalizedMessage(OutOfChargesMessage);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (ShowUsesRemaining)
                list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~			
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)1); // version
			
            writer.Write((int)m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
			
            m_UsesRemaining = reader.ReadInt();

            if (version == 0)
                LootType = LootType.Blessed;
        }

        private class InternalTarget : Target
        {
            private readonly BaseEngravingTool m_Tool;

            public InternalTarget(BaseEngravingTool tool)
                : base(2, true, TargetFlags.None)
            {
                m_Tool = tool;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Tool == null || m_Tool.Deleted)
                    return;

                if (targeted is IEntity)
                {
                    IEntity entity = (IEntity)targeted;

                    if (IsValid(entity, from))
                    {
                        if (entity is IEngravable && m_Tool.CheckType(entity))
                        {
                            from.CloseGump(typeof(InternalGump));
                            from.SendGump(new InternalGump(m_Tool, entity));
                        }
                        else
                            from.SendLocalizedMessage(m_Tool.CannotEngraveMessage);
                    }
                    else
                        from.SendLocalizedMessage(m_Tool.CannotEngraveMessage);
                }
                else
                    from.SendLocalizedMessage(m_Tool.CannotEngraveMessage);
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                from.SendLocalizedMessage(m_Tool.NotAccessibleMessage);
            }

            private bool IsValid(IEntity entity, Mobile m)
            {
                if (entity is Item)
                {
                    Item item = entity as Item;

                    if (BaseHouse.CheckAccessible(m, (Item)item))
                        return true;
                    else if (item.Movable && !item.IsLockedDown && !item.IsSecure)
                        return true;
                }
                else if (entity is BaseCreature)
                {
                    BaseCreature bc = entity as BaseCreature;

                    if (bc.Controlled && bc.ControlMaster == m)
                        return true;
                }
					
                return false;				
            }
        }

        private class InternalGump : Gump
        {
            private readonly BaseEngravingTool m_Tool;
            private readonly IEntity m_Target;

            public InternalGump(BaseEngravingTool tool, IEntity target)
                : base(0, 0)
            {
                m_Tool = tool;
                m_Target = target;
			
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;
			
                AddPage(0);
				
                AddBackground(50, 50, 400, 300, 0xA28);
                AddHtmlLocalized(50, 70, 400, 20, m_Tool.GumpTitle, 0x0, false, false);
                AddHtmlLocalized(75, 95, 350, 145, 1072360, 0x0, true, true);				
				
                AddButton(125, 300, 0x81A, 0x81B, (int)Buttons.Okay, GumpButtonType.Reply, 0);
                AddButton(320, 300, 0x819, 0x818, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
				
                AddImageTiled(75, 245, 350, 40, 0xDB0);
                AddImageTiled(76, 245, 350, 2, 0x23C5);
                AddImageTiled(75, 245, 2, 40, 0x23C3);
                AddImageTiled(75, 285, 350, 2, 0x23C5);
                AddImageTiled(425, 245, 2, 42, 0x23C3);
				
                AddTextEntry(78, 245, 345, 40, 0x0, (int)Buttons.Text, "");
            }

            private enum Buttons
            {
                Cancel,
                Okay,
                Text
            }

            public override void OnResponse(Server.Network.NetState state, RelayInfo info)
            { 
                if (m_Tool == null || m_Tool.Deleted || m_Target == null || m_Target.Deleted)
                    return;
			
                if (info.ButtonID == (int)Buttons.Okay)
                {
                    TextRelay relay = info.GetTextEntry((int)Buttons.Text);
					
                    if (relay != null)
                    {
                        if (relay.Text == null || relay.Text.Equals(""))
                        {
                            ((IEngravable)m_Target).EngravedText = null;
                            state.Mobile.SendLocalizedMessage(m_Tool.RemoveMessage);
                        }
                        else
                        {
                            if (relay.Text.Length > 40)
                                ((IEngravable)m_Target).EngravedText = relay.Text.Substring(0, 40);
                            else
                                ((IEngravable)m_Target).EngravedText = relay.Text;

                            state.Mobile.SendLocalizedMessage(m_Tool.SuccessMessage);

                            m_Tool.UsesRemaining -= 1;

                            if (m_Tool.UsesRemaining < 1)
                            {
                                m_Tool.Delete();
                                state.Mobile.SendLocalizedMessage(1044038); // You have worn out your tool!
                            }
                        }
                    }
                }
            }
        }
    }

    public class LeatherContainerEngraver : BaseEngravingTool
    {
        [Constructable]
        public LeatherContainerEngraver()
            : base(0xF9D, 1)
        { 
        }

        public LeatherContainerEngraver(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072152;
            }
        }// leather container engraving tool
        public override Type[] Engraves
        {
            get
            {
                return new Type[]
                {
                    typeof(Pouch), typeof(Backpack), typeof(Bag)
                };
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    public class WoodenContainerEngraver : BaseEngravingTool
    {
        [Constructable]
        public WoodenContainerEngraver()
            : base(0x1026, 1)
        { 
        }

        public WoodenContainerEngraver(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072153;
            }
        }// wooden container engraving tool
        public override Type[] Engraves
        {
            get
            {
                return new Type[]
                {
                    typeof(WoodenBox), typeof(LargeCrate), typeof(MediumCrate),
                    typeof(SmallCrate), typeof(WoodenChest), typeof(EmptyBookcase),
                    typeof(Armoire), typeof(FancyArmoire), typeof(PlainWoodenChest),
                    typeof(OrnateWoodenChest), typeof(GildedWoodenChest), typeof(WoodenFootLocker),
                    typeof(FinishedWoodenChest), typeof(TallCabinet), typeof(ShortCabinet),
                    typeof(RedArmoire), typeof(CherryArmoire), typeof(MapleArmoire),
                    typeof(ElegantArmoire), typeof(Keg), typeof(SimpleElvenArmoire),
                    typeof(DecorativeBox), typeof(FancyElvenArmoire), typeof(RarewoodChest),
                    typeof(RewardSign), typeof(GargoyleWoodenChest)
                };
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    public class MetalContainerEngraver : BaseEngravingTool
    {
        [Constructable]
        public MetalContainerEngraver()
            : base(0x1EB8, 1)
        { 
        }

        public MetalContainerEngraver(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072154;
            }
        }// metal container engraving tool
        public override Type[] Engraves
        {
            get
            {
                return new Type[]
                {
                    typeof(ParagonChest), typeof(MetalChest)
                };
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    public class FoodEngraver : BaseEngravingTool
    {
        [Constructable]
        public FoodEngraver()
            : base(0x1BD1, 1)
        { 
        }

        public FoodEngraver(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072951;
            }
        }// food decoration tool
        public override Type[] Engraves
        {
            get
            {
                return new Type[]
                {
                    typeof(Cake), typeof(CheesePizza), typeof(SausagePizza),
                    typeof(Cookies)
                };
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    public class SpellbookEngraver : BaseEngravingTool
    {
        [Constructable]
        public SpellbookEngraver()
            : base(0xFBF, 1)
        { 
        }

        public SpellbookEngraver(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072151;
            }
        }// spellbook engraving tool
        public override Type[] Engraves
        {
            get
            {
                return new Type[]
                {
                    typeof(Spellbook)
                };
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    public class StatuetteEngravingTool : BaseEngravingTool
    {
        [Constructable]
        public StatuetteEngravingTool()
            : base(0x12B3, 10)
        {
            Hue = 0;
        }

        public StatuetteEngravingTool(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080201;
            }
        }// Statuette Engraving Tool
        public override Type[] Engraves
        {
            get
            {
                return new Type[]
                {
                    typeof(MonsterStatuette)
                };
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ArmorEngravingTool : BaseEngravingTool
    {
        [Constructable]
        public ArmorEngravingTool()
            : base(0x32F8, 30)
        {
            Hue = 0x490;
        }

        public ArmorEngravingTool(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber { get { return 1080547; } }// Armor Engraving Tool
        public override int GumpTitle { get { return 1071163; } } // <center>Armor Engraving Tool</center>

        public override Type[] Engraves { get { return new Type[] { typeof(BaseArmor) }; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
