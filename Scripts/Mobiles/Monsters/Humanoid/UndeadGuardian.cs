using System;

namespace Server.Mobiles
{
    [CorpseName("an undead guardian corpse")]
    public class UndeadGuardian : BaseCreature
    {
        [Constructable]
        public UndeadGuardian()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an undead guardian";
            this.Body = 722; 

            this.SetStr(212);
            this.SetDex(76);
            this.SetInt(56);

            this.SetHits(138);

            this.SetDamage(8, 18);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Cold, 60);

            this.SetResistance(ResistanceType.Physical, 38);
            this.SetResistance(ResistanceType.Fire, 24);
            this.SetResistance(ResistanceType.Cold, 58);
            this.SetResistance(ResistanceType.Poison, 28);
            this.SetResistance(ResistanceType.Energy, 38);

            this.SetSkill(SkillName.MagicResist, 66.6);
            this.SetSkill(SkillName.Tactics, 86.2);
            this.SetSkill(SkillName.Wrestling, 86.9);

            this.PackNecroReg(10, 15); /// Stratics didn't specify
        }

        public UndeadGuardian(Serial serial)
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