using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a ballem corpse")]
    public class Ballem : BaseVoidCreature
    {
        public override VoidEvolution Evolution => VoidEvolution.Killing;
        public override int Stage => 2;

        [Constructable]
        public Ballem()
            : base(AIType.AI_Melee, 10, 1, 0.2, 0.4)
        {
            Name = "ballem";
            Body = 304;
            Hue = 2071;
            BaseSoundID = 684;

            SetStr(991);
            SetDex(1001);
            SetInt(243);

            SetHits(500, 600);

            SetDamage(10, 15);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 30, 50);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 70.0, 80.0);
            SetSkill(SkillName.Tactics, 50.1, 60.0);
            SetSkill(SkillName.Wrestling, 70.1, 80.0);
            SetSkill(SkillName.Anatomy, 0.0, 10.0);

            Fame = 1800;
            Karma = -1800;

            SetWeaponAbility(WeaponAbility.CrushingBlow);
        }

        public Ballem(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Lethal;

        public override bool Unprovokable => true;

        public override bool BardImmune => true;

        public override bool CanRummageCorpses => true;

        public override bool BleedImmune => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LootItem<DaemonBone>(15, true));
            AddLoot(LootPack.LootItem<AncientPotteryFragments>(10.0));
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
