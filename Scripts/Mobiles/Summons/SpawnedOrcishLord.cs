using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an orcish corpse")]
    public class SpawnedOrcishLord : OrcishLord
    {
        [Constructable]
        public SpawnedOrcishLord()
        {
            Container pack = Backpack;

            if (pack != null)
                pack.Delete();

            NoKillAwards = true;
        }

        public SpawnedOrcishLord(Serial serial)
            : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
