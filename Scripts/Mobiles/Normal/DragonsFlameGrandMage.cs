using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a black order grand mage corpse")]
    public class DragonsFlameGrandMage : DragonsFlameMage
    {
        [Constructable]
        public DragonsFlameGrandMage()
            : base()
        {
            Name = "Black Order Grand Mage";
            Title = "of the Dragon's Flame Sect";
            SetStr(340, 360);
            SetDex(200, 215);
            SetInt(500, 515);

            SetHits(800);

            SetDamage(15, 20);

            Fame = 25000;
            Karma = -25000;
        }

        public DragonsFlameGrandMage(Serial serial)
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

            c.DropItem(new DragonFlameKey());

            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new DragonFlameSectBadge());
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