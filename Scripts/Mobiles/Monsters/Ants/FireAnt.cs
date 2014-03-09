using System;

namespace Server.Mobiles
{
    [CorpseName("a fire ant corpse")]
    public class FireAnt : BaseCreature
    {
        [Constructable]
        public FireAnt()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a fire ant";
            this.Body = 738; 

            this.SetStr(201, 246);
            this.SetDex(103, 115);
            this.SetInt(16, 29);

            this.SetHits(254, 289);
			this.SetMana(16, 29);
			this.SetStam(103, 121);

            this.SetDamage(15, 18);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Fire, 60);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 95, 97);
            this.SetResistance(ResistanceType.Cold, 36, 42);
            this.SetResistance(ResistanceType.Poison, 37, 45);
            this.SetResistance(ResistanceType.Energy, 36, 44);

            this.SetSkill(SkillName.Anatomy, 0);
            this.SetSkill(SkillName.MagicResist, 46.7, 58.2);
            this.SetSkill(SkillName.Tactics, 71.9, 82.8);
            this.SetSkill(SkillName.Wrestling, 71.5, 83.4);
        }

        public FireAnt(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average, 2);
        }

        public override int GetIdleSound()
        {
            return 846;
        }

        public override int GetAngerSound()
        {
            return 849;
        }

        public override int GetHurtSound()
        {
            return 852;
        }

        public override int GetDeathSound()
        {
            return 850;
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