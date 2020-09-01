using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a human corpse")]
    public class DupresChampion : BaseCreature
    {
        [Constructable]
        public DupresChampion() : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("male");
            Title = "the Champion";
            Body = 0x190;
            Hue = Utility.RandomSkinHue();
            Female = false;

            SetStr(190, 200);
            SetDex(50, 75);
            SetInt(150, 250);
            SetHits(3900, 4100);
            SetDamage(22, 28);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 70);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 50, 70);
            SetResistance(ResistanceType.Poison, 50, 70);
            SetResistance(ResistanceType.Energy, 50, 70);

            SetSkill(SkillName.EvalInt, 195.0, 220.0);
            SetSkill(SkillName.Magery, 195.0, 220.0);
            SetSkill(SkillName.Meditation, 195.0, 200.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 195.0, 220.0);
            SetSkill(SkillName.Wrestling, 195.0, 220.0);

            Item cutlass = new Cutlass();
            cutlass.LootType = LootType.Blessed;
            SetWearable(cutlass);

            Item ph = new PlateHelm();
            ph.LootType = LootType.Blessed;
            ph.Hue = 0x8A5; // gold
            SetWearable(ph);

            Item pa = new PlateArms();
            pa.LootType = LootType.Blessed;
            pa.Hue = 0x8A5; // gold
            SetWearable(pa);

            Item pg = new PlateGorget();
            pg.LootType = LootType.Blessed;
            pg.Hue = 0x8A5; // gold
            SetWearable(pg);

            Item pgl = new PlateGloves();
            pgl.LootType = LootType.Blessed;
            pgl.Hue = 0x8A5; // gold
            SetWearable(pgl);

            Item pl = new PlateLegs();
            pl.LootType = LootType.Blessed;
            pl.Hue = 0x8A5; // gold
            SetWearable(pl);

            Item pc = new PlateChest();
            pc.LootType = LootType.Blessed;
            pc.Hue = 0x8A5; // gold
            SetWearable(pc);

            Item mks = new MetalKiteShield();
            mks.LootType = LootType.Blessed;
            mks.Hue = 0x776;
            SetWearable(mks);

            Item bs = new BodySash(0x486); // dark purple
            bs.LootType = LootType.Blessed;
            SetWearable(bs);

            Item cloak = new Cloak(0x486); // dark purple
            cloak.LootType = LootType.Blessed;
            SetWearable(cloak);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootGold(400, 600));
        }

        public override bool CanBeParagon => false;

        public override Poison PoisonImmune => Poison.Lethal;

        public override int TreasureMapLevel => 5;

        public DupresChampion(Serial serial)
            : base(serial)
        {
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
