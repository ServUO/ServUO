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

            this.SetStr(225);
            this.SetDex(108);
            this.SetInt(25);

            this.SetHits(299);

            this.SetDamage(15, 18);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Fire, 60);

            this.SetResistance(ResistanceType.Physical, 52);
            this.SetResistance(ResistanceType.Fire, 96);
            this.SetResistance(ResistanceType.Cold, 36);
            this.SetResistance(ResistanceType.Poison, 40);
            this.SetResistance(ResistanceType.Energy, 36);

            this.SetSkill(SkillName.Anatomy, 8.7);
            this.SetSkill(SkillName.MagicResist, 53.1);
            this.SetSkill(SkillName.Tactics, 77.2);
            this.SetSkill(SkillName.Wrestling, 75.4);
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