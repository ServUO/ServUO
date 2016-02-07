using System;

namespace Server.Items
{
    public class SamaritanRobe : Robe
    {
        [Constructable]
        public SamaritanRobe()
        {
            this.Hue = 0x2a3;
        }

        public SamaritanRobe(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094926;
            }
        }// Good Samaritan of Britannia [Replica]
        public override int BasePhysicalResistance
        {
            get
            {
                return 5;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}