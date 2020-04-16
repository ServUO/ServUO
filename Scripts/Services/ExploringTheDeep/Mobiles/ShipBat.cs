namespace Server.Mobiles
{
    [CorpseName("a ship bat corpse")]
    public class ShipBat : VampireBat
    {
        [Constructable]
        public ShipBat()
            : base()
        {
            Name = "a ship bat";
            Body = 317;
            BaseSoundID = 0x270;

            SetStr(120);
            SetDex(120);
            SetInt(101);

            SetHits(85, 96);

            SetDamage(4, 12);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 10, 15);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 10, 15);

            SetSkill(SkillName.Tactics, 45.0, 55.0);
            SetSkill(SkillName.Wrestling, 90.0, 100.0);

            Fame = 1000;
            Karma = -1000;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
        }

        public override int GetIdleSound()
        {
            return 0x29B;
        }

        public ShipBat(Serial serial) : base(serial)
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
