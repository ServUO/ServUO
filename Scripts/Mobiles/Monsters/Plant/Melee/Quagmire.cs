using System;

namespace Server.Mobiles
{
    [CorpseName("a quagmire corpse")]
    public class Quagmire : BaseCreature
    {
        [Constructable]
        public Quagmire()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8)
        {
            this.Name = "a quagmire";
            this.Body = 789;
            this.BaseSoundID = 352;

            this.SetStr(101, 130);
            this.SetDex(66, 85);
            this.SetInt(31, 55);

            this.SetHits(91, 105);

            this.SetDamage(10, 14);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.MagicResist, 65.1, 75.0);
            this.SetSkill(SkillName.Tactics, 50.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 32;
        }

        public Quagmire(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override double HitPoisonChance
        {
            get
            {
                return 0.1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public override int GetAngerSound()
        {
            return 353;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (this.BaseSoundID == -1)
                this.BaseSoundID = 352;
        }
    }
}