using System;

namespace Server.Items
{
    public class EnhancedBandage : Bandage
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
            this.Hue = 0x8A5;
        }

        public EnhancedBandage(Serial serial)
            : base(serial)
        {
        }

        public static int HealingBonus
        {
            get
            {
                return 10;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1152441;
            }
        }// enhanced bandage
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

    [FlipableAttribute(0x2AC0, 0x2AC3)]
    public class FountainOfLife : BaseAddonContainer
    {
        private int m_Charges;
        private Timer m_Timer;
        [Constructable]
        public FountainOfLife()
            : this(10)
        {
        }

        [Constructable]
        public FountainOfLife(int charges)
            : base(0x2AC0)
        {
            this.m_Charges = charges;

            this.m_Timer = Timer.DelayCall(this.RechargeTime, this.RechargeTime, new TimerCallback(Recharge));
        }

        public FountainOfLife(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed
        {
            get
            {
                return new FountainOfLifeDeed(this.m_Charges);
            }
        }
        public virtual TimeSpan RechargeTime
        {
            get
            {
                return TimeSpan.FromDays(1);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1075197;
            }
        }// Fountain of Life
        public override int DefaultGumpID
        {
            get
            {
                return 0x484;
            }
        }
        public override int DefaultDropSound
        {
            get
            {
                return 66;
            }
        }
        public override int DefaultMaxItems
        {
            get
            {
                return 125;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return this.m_Charges;
            }
            set
            {
                this.m_Charges = Math.Min(value, 10);
                this.InvalidateProperties();
            }
        }
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
                    this.Enhance(from);

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
                    this.Enhance(from);

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

            list.Add(1075217, this.m_Charges.ToString()); // ~1_val~ charges remaining
        }

        public override void OnDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); //version

            writer.Write(this.m_Charges);
            writer.Write((DateTime)this.m_Timer.Next);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Charges = reader.ReadInt();

            DateTime next = reader.ReadDateTime();

            if (next < DateTime.UtcNow)
                this.m_Timer = Timer.DelayCall(TimeSpan.Zero, this.RechargeTime, new TimerCallback(Recharge));
            else
                this.m_Timer = Timer.DelayCall(next - DateTime.UtcNow, this.RechargeTime, new TimerCallback(Recharge));
        }

        public void Recharge()
        {
            this.m_Charges = 10;

            this.Enhance(null);
        }

        public void Enhance(Mobile from)
        {
			EnhancedBandage existing = null;

			foreach(Item item in Items)
			{
				if(item is EnhancedBandage)
				{
					existing = item as EnhancedBandage;
					break;
				}
			}

            for (int i = this.Items.Count - 1; i >= 0 && this.m_Charges > 0; --i)
            {
                if (this.Items[i] is EnhancedBandage)
                    continue;

                Bandage bandage = this.Items[i] as Bandage;

                if (bandage != null)
                {
                    Item enhanced;

                    if (bandage.Amount > this.m_Charges)
                    {
                        bandage.Amount -= this.m_Charges;
                        enhanced = new EnhancedBandage(this.m_Charges);
                        this.m_Charges = 0;
                    }
                    else
                    {
                        enhanced = new EnhancedBandage(bandage.Amount);
                        this.m_Charges -= bandage.Amount;
                        bandage.Delete();
                    }

					// try stacking first
					if (from == null || !this.TryDropItem(from, enhanced, false))
					{
						if (existing != null)
							existing.StackWith(from, enhanced);
						else
							this.DropItem(enhanced);
					}
                }
            }

            this.InvalidateProperties();
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
            this.LootType = LootType.Blessed;
            this.m_Charges = charges;
        }

        public FountainOfLifeDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075197;
            }
        }// Fountain of Life
        public override BaseAddonContainer Addon
        {
            get
            {
                return new FountainOfLife(this.m_Charges);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return this.m_Charges;
            }
            set
            {
                this.m_Charges = Math.Min(value, 10);
                this.InvalidateProperties();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); //version

            writer.Write(this.m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Charges = reader.ReadInt();
        }
    }
}