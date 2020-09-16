using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class EnhancedBandage : Bandage, ICommodity
    {
        [Constructable]
        public EnhancedBandage()
            : this(1)
        {
        }

        [Constructable]
        public EnhancedBandage(int amount)
            : base(amount)
        {
            Hue = 0x8A5;
        }

        public EnhancedBandage(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public static int HealingBonus => 10;
        public override int LabelNumber => 1152441;// enhanced bandage
        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1075216); // these bandages have been enhanced
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    [Flipable(0x2AC0, 0x2AC3)]
    public class FountainOfLife : BaseAddonContainer
    {
        private int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return m_Charges;
            }
            set
            {
                m_Charges = Math.Min(value, 10);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextRecharge { get; set; }

        [Constructable]
        public FountainOfLife()
            : this(10)
        {
        }

        [Constructable]
        public FountainOfLife(int charges)
            : base(0x2AC0)
        {
            m_Charges = charges;
        }

        public FountainOfLife(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed => new FountainOfLifeDeed(m_Charges);
        public virtual TimeSpan RechargeTime => TimeSpan.FromDays(1);
        public override int LabelNumber => 1075197;// Fountain of Life
        public override int DefaultGumpID => 0x484;
        public override int DefaultDropSound => 66;
        public override int DefaultMaxItems => 125;

        public override bool OnDragLift(Mobile from)
        {
            return false;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is Bandage)
            {
                bool allow = base.OnDragDrop(from, dropped);

                if (allow)
                    Enhance(from);

                return allow;
            }
            else
            {
                from.SendLocalizedMessage(1075209); // Only bandages may be dropped into the fountain.
                return false;
            }
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (item is Bandage)
            {
                bool allow = base.OnDragDropInto(from, item, p);

                if (allow)
                    Enhance(from);

                return allow;
            }
            else
            {
                from.SendLocalizedMessage(1075209); // Only bandages may be dropped into the fountain.
                return false;
            }
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1075217, m_Charges.ToString()); // ~1_val~ charges remaining
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); //version

            writer.Write(NextRecharge);
            writer.Write(m_Charges);

            if (DateTime.UtcNow > NextRecharge)
            {
                ToProcess.Add(this);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    NextRecharge = reader.ReadDateTime();
                    goto case 0;
                case 0:
                    m_Charges = reader.ReadInt();

                    if (version < 1)
                    {
                        NextRecharge = reader.ReadDateTime();
                    }
                    break;
            }
        }

        public void Recharge()
        {
            NextRecharge = DateTime.UtcNow + RechargeTime;

            m_Charges = 10;

            Enhance();
        }

        public void Enhance()
        {
            Enhance(null);
        }

        public void Enhance(Mobile from)
        {
            EnhancedBandage existing = null;

            foreach (Item item in Items)
            {
                if (item is EnhancedBandage)
                {
                    existing = item as EnhancedBandage;
                    break;
                }
            }

            for (int i = Items.Count - 1; i >= 0 && m_Charges > 0; --i)
            {
                if (Items[i] is EnhancedBandage)
                    continue;

                Bandage bandage = Items[i] as Bandage;

                if (bandage != null)
                {
                    Item enhanced;

                    if (bandage.Amount > m_Charges)
                    {
                        bandage.Amount -= m_Charges;
                        enhanced = new EnhancedBandage(m_Charges);
                        m_Charges = 0;
                    }
                    else
                    {
                        enhanced = new EnhancedBandage(bandage.Amount);
                        m_Charges -= bandage.Amount;
                        bandage.Delete();
                    }

                    // try stacking first
                    if (from == null || !TryDropItem(from, enhanced, false))
                    {
                        if (existing != null)
                            existing.StackWith(from, enhanced);
                        else
                            DropItem(enhanced);
                    }
                }
            }

            InvalidateProperties();
        }

        public static List<FountainOfLife> ToProcess { get; set; } = new List<FountainOfLife>();

        public static void Initialize()
        {
            EventSink.AfterWorldSave += CheckRecharge;
        }

        public static void CheckRecharge(AfterWorldSaveEventArgs e)
        {
            for (int i = 0; i < ToProcess.Count; i++)
            {
                ToProcess[i].Recharge();
            }

            ToProcess.Clear();
        }
    }

    public class FountainOfLifeDeed : BaseAddonContainerDeed
    {
        private int m_Charges;
        [Constructable]
        public FountainOfLifeDeed()
            : this(10)
        {
        }

        [Constructable]
        public FountainOfLifeDeed(int charges)
            : base()
        {
            LootType = LootType.Blessed;
            m_Charges = charges;
        }

        public FountainOfLifeDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075197;// Fountain of Life
        public override BaseAddonContainer Addon => new FountainOfLife(m_Charges);
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return m_Charges;
            }
            set
            {
                m_Charges = Math.Min(value, 10);
                InvalidateProperties();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); //version

            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_Charges = reader.ReadInt();
        }
    }
}
