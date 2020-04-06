using System;

namespace Server.Items
{
    [FlipableAttribute(0x13B8, 0x13B7)]
    public class ThinLongsword : BaseSword
    {
        [Constructable]
        public ThinLongsword()
            : base(0x13B8)
        {
            Weight = 1.0;
        }

        public ThinLongsword(Serial serial)
            : base(serial)
        {
        }

        public override int StrengthReq
        {
            get
            {
                return 35;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 15;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 16;
            }
        }
        public override float Speed
        {
            get
            {
                return 3.50f;
            }
        }
        
        public override int DefHitSound
        {
            get
            {
                return 0x237;
            }
        }
        public override int DefMissSound
        {
            get
            {
                return 0x23A;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 31;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 110;
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