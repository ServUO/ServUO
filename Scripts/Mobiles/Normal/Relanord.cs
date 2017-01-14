using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a relanord corpse")]
    public class Relanord : BaseCreature
    {
        [Constructable]
        public Relanord()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a Relanord";
            Body = 0x2F4;
            Hue = 2071;

            SetStr(561, 650);
            SetDex(76, 95);
            SetInt(61, 90);

            SetHits(331, 390);

            SetDamage(13, 19);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 40, 60);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 80.2, 98.0);
            SetSkill(SkillName.Tactics, 80.2, 98.0);
            SetSkill(SkillName.Wrestling, 80.2, 98.0);

            Fame = 10000;
            Karma = -10000;
            VirtualArmor = 50;

            QLPoints = 20;

            PackItem(new DaemonBone(5));
        }

        public Relanord(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get { return true; }
        }

        public override bool AutoDispel
        {
            get { return true; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 1);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new VoidOrb());
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
            var version = reader.ReadInt();
        }
    }
}