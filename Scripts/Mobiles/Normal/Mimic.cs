using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a mimic corpse")]
    public class Mimic : BaseCreature
    {
        [Constructable]
        public Mimic()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a mimic";
            this.Body = 729;

            this.SetStr(281);
            this.SetDex(140);
            this.SetInt(261);

            this.SetHits(543);

            this.SetDamage(13, 20);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 63);
            this.SetResistance(ResistanceType.Fire, 43);
            this.SetResistance(ResistanceType.Cold, 36);
            this.SetResistance(ResistanceType.Poison, 37);
            this.SetResistance(ResistanceType.Energy, 42);

            this.SetSkill(SkillName.EvalInt, 100.0);
            this.SetSkill(SkillName.Magery, 107.5);
            this.SetSkill(SkillName.Meditation, 100.0);
            this.SetSkill(SkillName.MagicResist, 126.5);
            this.SetSkill(SkillName.Tactics, 98.5);
            this.SetSkill(SkillName.Wrestling, 92.2);

            this.PackReg(20);
        }

        public Mimic(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 4);
            this.AddLoot(LootPack.MedScrolls);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.03)            
                c.DropItem(new LuckyCoin());            
        }

        public override int GetIdleSound()
        {
            return 1561;
        }

        public override int GetAngerSound()
        {
            return 1558;
        }

        public override int GetHurtSound()
        {
            return 1560;
        }

        public override int GetDeathSound()
        {
            return 1559;
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