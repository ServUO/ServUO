using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a relanord corpse")]
    public class Relanord : BaseVoidCreature
    {
        public override VoidEvolution Evolution => VoidEvolution.Survival;
        public override int Stage => 2;

        [Constructable]
        public Relanord()
            : base(AIType.AI_Melee, 10, 1, 0.2, 0.4)
        {
            Name = "relanord";
            Body = 0x2F4;
            Hue = 2071;

            SetStr(700, 800);
            SetDex(60, 100);
            SetInt(60, 100);

            SetHits(400, 500);

            SetDamage(10, 15);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 45, 60);
            SetResistance(ResistanceType.Fire, 40, 60);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 30, 60);

            SetSkill(SkillName.MagicResist, 30.2, 50.0);
            SetSkill(SkillName.Tactics, 40.2, 60.0);
            SetSkill(SkillName.Wrestling, 50.2, 70.0);

            Fame = 10000;
            Karma = -10000;
        }

        public Relanord(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel => true;

        public override bool BardImmune => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 1);
            AddLoot(LootPack.LootItem<DaemonBone>(15, true));
        }

        public override int GetIdleSound()
        {
            return 0xFD;
        }

        public override int GetAngerSound()
        {
            return 0x26C;
        }

        public override int GetDeathSound()
        {
            return 0x211;
        }

        public override int GetAttackSound()
        {
            return 0x23B;
        }

        public override int GetHurtSound()
        {
            return 0x140;
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
