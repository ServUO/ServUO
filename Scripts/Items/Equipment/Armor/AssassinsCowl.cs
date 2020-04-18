namespace Server.Items
{
    public class AssassinsCowl : BaseHat
    {
        public override int LabelNumber => 1126024;  // assassin's cowl

        public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 4;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 2;

        public override int InitMinHits => 40;
        public override int InitMaxHits => 60;

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
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
