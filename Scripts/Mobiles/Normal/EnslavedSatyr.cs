using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an enslaved satyr corpse")]
    public class EnslavedSatyr : Satyr
    {
        [Constructable]
        public EnslavedSatyr()
            : base()
        {
            Name = "an enslaved satyr";

            Fame = 10000;
            Karma = -10000;

            SetSpecialAbility(SpecialAbility.AngryFire);
        }

        public EnslavedSatyr(Serial serial)
            : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.1)
                c.DropItem(new ParrotItem());
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