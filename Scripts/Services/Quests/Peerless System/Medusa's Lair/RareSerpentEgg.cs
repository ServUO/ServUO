using System;

namespace Server.Items
{
    public class RareSerpentEgg1 : PeerlessKey
    {
        [Constructable]
        public RareSerpentEgg1()
            : base(0x41BF)
        {
            this.Weight = 1.0;
            this.Hue = 2207;// comes in either white, blue, yellow, or red
        }

        public RareSerpentEgg1(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112575;
            }
        }// a rare serpent egg
        public override int Lifespan
        {
            get
            {
                return 43200;
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

    public class RareSerpentEgg2 : PeerlessKey
    {
        [Constructable]
        public RareSerpentEgg2()
            : base(0x41BF)
        {
            this.Weight = 1.0;
            this.Hue = 1641;
        }

        public RareSerpentEgg2(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112575;
            }
        }// a rare serpent egg
        public override int Lifespan
        {
            get
            {
                return 43200;
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

    public class RareSerpentEgg3 : PeerlessKey
    {
        [Constructable]
        public RareSerpentEgg3()
            : base(0x41BF)
        {
            this.Weight = 1.0;
            this.Hue = 0;// plain hued one
        }

        public RareSerpentEgg3(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112575;
            }
        }// a rare serpent egg
        public override int Lifespan
        {
            get
            {
                return 43200;
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

    public class RareSerpentEgg4 : PeerlessKey
    {
        [Constructable]
        public RareSerpentEgg4()
            : base(0x41BF)
        {
            this.Weight = 1.0;
            this.Hue = 2121;
        }

        public RareSerpentEgg4(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112575;
            }
        }// a rare serpent egg
        public override int Lifespan
        {
            get
            {
                return 43200;
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
}