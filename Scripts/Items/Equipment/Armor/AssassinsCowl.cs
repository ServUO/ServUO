using System;

namespace Server.Items
{
    public class AssassinsCowl : BaseHat
    {
        public override int LabelNumber { get { return 1126024; } } // assassin's cowl

        public override int BasePhysicalResistance { get { return 2; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 2; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        [Constructable]
        public AssassinsCowl()
            : this(0)
        {
        }

        [Constructable]
        public AssassinsCowl(int hue)
            : base(0xA410, hue)
        {
            Weight = 3.0;
            StrRequirement = 45;
        }

        public AssassinsCowl(Serial serial)
            : base(serial)
        {
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
