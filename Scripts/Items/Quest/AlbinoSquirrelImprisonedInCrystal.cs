using Server.Mobiles;

namespace Server.Items
{
    public class AlbinoSquirrelImprisonedInCrystal : BaseImprisonedMobile
    {
        [Constructable]
        public AlbinoSquirrelImprisonedInCrystal()
            : base(0x1F1C)
        {
            Weight = 1.0;
            Hue = 0x482;
        }

        public AlbinoSquirrelImprisonedInCrystal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075004;// An Albino Squirrel Imprisoned in a Crystal
        public override BaseCreature Summon => new AlbinoSquirrel();
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

namespace Server.Mobiles
{
    public class AlbinoSquirrel : Squirrel
    {
        [Constructable]
        public AlbinoSquirrel()
            : base()
        {
            Hue = 0x482;
        }

        public AlbinoSquirrel(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteOnRelease => true;
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1049646); // (summoned)
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