namespace Server.Items
{
    public class PowderOfFortKeg : Item
    {
        private int _Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return _Charges; } set { _Charges = value; InvalidateProperties(); } }

        public override int LabelNumber => 1157221;  // A specially lined keg for powder of fortification.

        [Constructable]
        public PowderOfFortKeg()
            : this(0)
        {
        }

        [Constructable]
        public PowderOfFortKeg(int uses)
            : base(0x1940)
        {
            _Charges = uses;

            Hue = 2419;
            Weight = 15.0;
        }

        public override bool OnDragDrop(Mobile m, Item dropped)
        {
            if (dropped is PowderOfTemperament)
            {
                PowderOfTemperament powder = dropped as PowderOfTemperament;

                if (_Charges < 250)
                {
                    if (powder.UsesRemaining + _Charges > 250)
                    {
                        int add = 250 - _Charges;

                        powder.UsesRemaining -= add;

                        Charges = 250;
                    }
                    else
                    {
                        Charges += powder.UsesRemaining;
                        powder.Delete();
                    }

                    m.PlaySound(0x247);
                }
            }

            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Backpack != null && IsChildOf(from.Backpack) && Charges > 0)
            {
                PowderOfTemperament powder = from.Backpack.FindItemByType(typeof(PowderOfTemperament)) as PowderOfTemperament;

                if (powder != null)
                {
                    powder.UsesRemaining++;
                    Charges--;
                }
                else
                {
                    powder = new PowderOfTemperament(1);

                    if (!from.Backpack.TryDropItem(from, powder, false))
                    {
                        from.SendLocalizedMessage(1080016); // That container cannot hold more weight.
                        powder.Delete();

                        return;
                    }

                    Charges--;
                }

                from.PlaySound(0x247);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, _Charges.ToString());

            int perc = (int)(((double)_Charges / 250) * 100);
            int number = 0;

            if (perc <= 0)
                number = 502246; // The keg is empty.
            else if (perc < 5)
                number = 502248; // The keg is nearly empty.
            else if (perc < 20)
                number = 502249; // The keg is not very full.
            else if (perc < 30)
                number = 502250; // The keg is about one quarter full.
            else if (perc < 40)
                number = 502251; // The keg is about one third full.
            else if (perc < 47)
                number = 502252; // The keg is almost half full.
            else if (perc < 54)
                number = 502254; // The keg is approximately half full.
            else if (perc < 70)
                number = 502253; // The keg is more than half full.
            else if (perc < 80)
                number = 502255; // The keg is about three quarters full.
            else if (perc < 90)
                number = 502256; // The keg is very full.
            else if (perc < 100)
                number = 502257; // The liquid is almost to the top of the keg.
            else
                number = 502258; // The keg is completely full.

            list.Add(number);
        }

        public PowderOfFortKeg(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Charges = reader.ReadInt();

            if (version == 0)
                ItemID = 0x1940;
        }
    }
}
