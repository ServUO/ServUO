using System;

namespace Server.Mobiles
{
    [CorpseName("a vampire bat corpse")]
    public class VampireBat : BaseCreature
    {
        [Constructable]
        public VampireBat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a vampire bat";
            this.Body = 317;
            this.BaseSoundID = 0x270;

            this.SetStr(91, 110);
            this.SetDex(91, 115);
            this.SetInt(26, 50);

            this.SetHits(55, 66);

            this.SetDamage(7, 9);

            this.SetDamageType(ResistanceType.Physical, 80);
            this.SetDamageType(ResistanceType.Poison, 20);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 15, 25);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.MagicResist, 70.1, 95.0);
            this.SetSkill(SkillName.Tactics, 55.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 30.1, 55.0);

            this.Fame = 1000;
            this.Karma = -1000;

            this.VirtualArmor = 14;
        }

        public VampireBat(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Poor);
        }

        public override int GetIdleSound()
        {
            return 0x29B;
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