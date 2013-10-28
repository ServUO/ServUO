using System;

namespace Server.Mobiles
{
    [CorpseName("an putrid undead guardian corpse")]
    public class PutridUndeadGuardian : BaseCreature
    {
        [Constructable]
        public PutridUndeadGuardian()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an putrid undead guardian";
            this.Body = 722; 

            this.SetStr(79);
            this.SetDex(63);
            this.SetInt(187);

            this.SetHits(553);

            this.SetDamage(3, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40);
            this.SetResistance(ResistanceType.Fire, 23);
            this.SetResistance(ResistanceType.Cold, 57);
            this.SetResistance(ResistanceType.Poison, 29);
            this.SetResistance(ResistanceType.Energy, 39);

            this.SetSkill(SkillName.MagicResist, 62.7);
            this.SetSkill(SkillName.Tactics, 45.4);
            this.SetSkill(SkillName.Wrestling, 50.7);

            this.PackNecroReg(10, 15); /// Stratics didn't specify
        }

        public PutridUndeadGuardian(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
        }

        public override int GetIdleSound()
        {
            return 1609;
        }

        public override int GetAngerSound()
        {
            return 1606;
        }

        public override int GetHurtSound()
        {
            return 1608;
        }

        public override int GetDeathSound()
        {
            return 1607;
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
        }
    }
}