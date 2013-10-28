using System;

namespace Server.Mobiles
{
    [CorpseName("a trapdoor spider corpse")]
    public class TrapdoorSpider : BaseCreature
    {
        [Constructable]
        public TrapdoorSpider()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a trapdoor spider";
            this.Body = 737; 

            this.SetStr(100, 104);
            this.SetDex(162, 165);
            this.SetInt(29, 50);

            this.SetHits(125, 144);

            this.SetDamage(15, 18);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Poison, 80);

            this.SetResistance(ResistanceType.Physical, 0);
            this.SetResistance(ResistanceType.Fire, 30, 35);
            this.SetResistance(ResistanceType.Cold, 30, 35);
            this.SetResistance(ResistanceType.Poison, 40, 45);
            this.SetResistance(ResistanceType.Energy, 95, 100);

            this.SetSkill(SkillName.Anatomy, 2.0, 3.8);
            this.SetSkill(SkillName.MagicResist, 47.5, 57.9);
            this.SetSkill(SkillName.Poisoning, 70.5, 73.5);
            this.SetSkill(SkillName.Tactics, 73.3, 78.9);
            this.SetSkill(SkillName.Wrestling, 92.5, 94.6);
        }

        public TrapdoorSpider(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
        }

        public override int GetIdleSound()
        {
            return 1605;
        }

        public override int GetAngerSound()
        {
            return 1602;
        }

        public override int GetHurtSound()
        {
            return 1604;
        }

        public override int GetDeathSound()
        {
            return 1603;
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