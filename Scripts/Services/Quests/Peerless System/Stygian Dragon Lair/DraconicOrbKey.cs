using System;

namespace Server.Items
{
    public class DraconicOrbKey : PeerlessKey
    { 
        [Constructable]
        public DraconicOrbKey()
            : base(0x573F)
        {
            this.Weight = 1;
            this.Hue = 0x35; // TODO check
        }

        public DraconicOrbKey(Serial serial)
            : base(serial)
        {
        }

        public override int Lifespan
        {
            get
            {
                return 21600;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1113515;
            }
        }// Draconic Orb
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

    public class DraconicOrbKeyRed : PeerlessKey
    {
        [Constructable]
        public DraconicOrbKeyRed()
            : base(0x573F)
        {
            this.Weight = 1;
            this.Hue = 33; // TODO check
        }

        public DraconicOrbKeyRed(Serial serial)
            : base(serial)
        {
        }

        public override int Lifespan
        {
            get
            {
                return 21600;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1113515;
            }
        }// Draconic Orb
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

    public class DraconicOrbKeyOrange : PeerlessKey
    {
        [Constructable]
        public DraconicOrbKeyOrange()
            : base(0x573F)
        {
            this.Weight = 1;
            this.Hue = 1260; // TODO check
        }

        public DraconicOrbKeyOrange(Serial serial)
            : base(serial)
        {
        }

        public override int Lifespan
        {
            get
            {
                return 21600;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1113515;
            }
        }// Draconic Orb
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

    public class DraconicOrbKeyBlue : PeerlessKey
    {
        [Constructable]
        public DraconicOrbKeyBlue()
            : base(0x573F)
        {
            this.Weight = 1;
            this.Hue = 5; // TODO check
        }

        public DraconicOrbKeyBlue(Serial serial)
            : base(serial)
        {
        }

        public override int Lifespan
        {
            get
            {
                return 21600;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1113515;
            }
        }// Draconic Orb
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