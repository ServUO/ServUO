namespace Server.Items
{
    public class Meteorite : Item
    {
        private bool _Polished;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Polished
        {
            get
            {
                return _Polished;
            }
            set
            {
                _Polished = value;

                if (_Polished)
                {
                    Polish();
                }
                else
                {
                    UnPolish();
                }

                InvalidateProperties();
            }
        }


        public override int LabelNumber => _Polished ? 1125446 : 1158640;  // Polished/Rough Meteorite

        [Constructable]
        public Meteorite()
            : base(Utility.Random(0x1364, 3))
        {
            Hue = 1910;
        }

        public void TryPolish(Mobile from)
        {
            if (0.75 > Utility.RandomDouble())
            {
                Polished = true;
            }
            else
            {
                from.SendLocalizedMessage(1158694); // You attempt to polish the meteorite, but its delicate structure crumbles at the attempt.
                Delete();
            }
        }

        public void Polish()
        {
            Hue = 0;
            ItemID = Utility.Random(0xA1CE, 12);
        }

        public void UnPolish()
        {
            Hue = 1910;
            ItemID = Utility.Random(0x1364, 3);
        }

        public Meteorite(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(_Polished);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Polished = reader.ReadBool();
        }
    }
}
