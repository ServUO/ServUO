using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a black order master corpse")]
    public class TigersClawMaster : TigersClawThief
    {
        [Constructable]
        public TigersClawMaster()
            : base()
        {
            Name = "Black Order Master";
            Title = "of the Serpent's Fang Sect";
            SetStr(440, 460);
            SetDex(400, 415);
            SetInt(200, 215);

            SetHits(850, 875);

            SetDamage(15, 20);

            Fame = 25000;
            Karma = -25000;
        }

        public TigersClawMaster(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer => true;
        public override bool ShowFameTitle => false;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 6);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new TigerClawKey());

            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new TigerClawSectBadge());
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