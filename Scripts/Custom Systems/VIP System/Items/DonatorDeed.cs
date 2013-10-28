namespace Server.Items
{
    public class DonatorDeed : Item
    {
        public override string DefaultName
        {
            get
            {
                return "Donator Deeds";
            }
        }
        public override bool DisplayLootType
        {
            get
            {
                return false;
            }
        }
        public override bool DisplayWeight
        {
            get
            {
                return false;
            }
        }
        public override double DefaultWeight
        {
            get
            {
                return 0.0;
            }
        }

        [Constructable]
        public DonatorDeed()
            : this(1)
        {
        }

        [Constructable]
        public DonatorDeed(int amount)
            : base(0x2D51)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.LootType = LootType.Blessed;
            this.Hue = 1153;
            this.Weight = 0.0;
        }

        public DonatorDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                    }
                    break;
            }
        }

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (dropped is DonatorDeed)
            {
                return this.StackWith(from, (DonatorDeed)dropped, playSound);
            }

            return base.StackWith(from, dropped, playSound);
        }

        public virtual bool StackWith(Mobile from, DonatorDeed dropped, bool playSound)
        {
            if (dropped.GetType() == this.GetType() && dropped.ItemID == this.ItemID && dropped.Hue == this.Hue && this.Amount + dropped.Amount <= 65535)
            {
                if (this.LootType != dropped.LootType)
                {
                    this.LootType = dropped.LootType;
                }

                this.Amount += dropped.Amount;
                dropped.Delete();

                if (playSound && from != null)
                {
                    int soundID = this.GetDropSound();

                    if (soundID == -1)
                    {
                        soundID = 0x42;
                    }

                    from.SendSound(soundID, this.GetWorldLocation());
                }

                return true;
            }

            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060738, this.Amount.ToString("#,#")); // Value: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            return;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region OPERATORS
        public static bool operator ==(DonatorDeed a, DonatorDeed b)
        {
            return (a.Amount == b.Amount);
        }

        public static bool operator !=(DonatorDeed a, DonatorDeed b)
        {
            return (a.Amount != b.Amount);
        }

        public static bool operator <(DonatorDeed a, DonatorDeed b)
        {
            return (a.Amount < b.Amount);
        }

        public static bool operator >(DonatorDeed a, DonatorDeed b)
        {
            return (a.Amount > b.Amount);
        }

        public static DonatorDeed operator -(DonatorDeed a, DonatorDeed b)
        {
            return new DonatorDeed(a.Amount - b.Amount);
        }

        public static DonatorDeed operator +(DonatorDeed a, DonatorDeed b)
        {
            return new DonatorDeed(a.Amount + b.Amount);
        }

        public static DonatorDeed operator *(DonatorDeed a, DonatorDeed b)
        {
            return new DonatorDeed(a.Amount * b.Amount);
        }

        public static DonatorDeed operator /(DonatorDeed a, DonatorDeed b)
        {
            return new DonatorDeed(a.Amount / b.Amount);
        }

        public static bool operator ==(DonatorDeed a, int b)
        {
            return (a.Amount == b);
        }

        public static bool operator !=(DonatorDeed a, int b)
        {
            return (a.Amount != b);
        }

        public static bool operator <(DonatorDeed a, int b)
        {
            return (a.Amount < b);
        }

        public static bool operator >(DonatorDeed a, int b)
        {
            return (a.Amount > b);
        }

        public static DonatorDeed operator -(DonatorDeed a, int b)
        {
            return new DonatorDeed(a.Amount - b);
        }

        public static DonatorDeed operator +(DonatorDeed a, int b)
        {
            return new DonatorDeed(a.Amount + b);
        }

        public static DonatorDeed operator *(DonatorDeed a, int b)
        {
            return new DonatorDeed(a.Amount * b);
        }

        public static DonatorDeed operator /(DonatorDeed a, int b)
        {
            return new DonatorDeed(a.Amount / b);
        }

        public static bool operator ==(int a, DonatorDeed b)
        {
            return (a == b.Amount);
        }

        public static bool operator !=(int a, DonatorDeed b)
        {
            return (a != b.Amount);
        }

        public static bool operator <(int a, DonatorDeed b)
        {
            return (a < b.Amount);
        }

        public static bool operator >(int a, DonatorDeed b)
        {
            return (a > b.Amount);
        }

        public static int operator -(int a, DonatorDeed b)
        {
            return a - b.Amount;
        }

        public static int operator +(int a, DonatorDeed b)
        {
            return a + b.Amount;
        }

        public static int operator *(int a, DonatorDeed b)
        {
            return a * b.Amount;
        }

        public static int operator /(int a, DonatorDeed b)
        {
            return a / b.Amount;
        }
        #endregion OPERATORS
    }
}