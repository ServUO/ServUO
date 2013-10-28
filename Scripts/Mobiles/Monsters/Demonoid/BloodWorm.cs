using System;

namespace Server.Mobiles
{
    public interface IBloodCreature
    {
    }

    [CorpseName("a bloodworm corpse")]
    public class BloodWorm : BaseCreature, IBloodCreature
    {
        [Constructable]
        public BloodWorm()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a bloodworm";
            this.Body = 287;

            this.SetStr(420);
            this.SetDex(80);
            this.SetInt(18);

            this.SetHits(365);

            this.SetDamage(11, 17);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 49);
            this.SetResistance(ResistanceType.Fire, 50);
            this.SetResistance(ResistanceType.Cold, 35);
            this.SetResistance(ResistanceType.Poison, 69);
            this.SetResistance(ResistanceType.Energy, 26);

            this.SetSkill(SkillName.MagicResist, 35.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0);
        }

        public BloodWorm(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Average);
        }

        public override int GetIdleSound()
        {
            return 1503;
        }

        public override int GetAngerSound()
        {
            return 1500;
        }

        public override int GetHurtSound()
        {
            return 1502;
        }

        public override int GetDeathSound()
        {
            return 1501;
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