using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an arcane daemon corpse")]
    public class ArcaneDaemon : BaseCreature
    {
        [Constructable]
        public ArcaneDaemon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an arcane daemon";
            this.Body = 0x310;
            this.BaseSoundID = 0x47D;

            this.SetStr(131, 150);
            this.SetDex(126, 145);
            this.SetInt(301, 350);

            this.SetHits(101, 115);

            this.SetDamage(12, 16);

            this.SetDamageType(ResistanceType.Physical, 80);
            this.SetDamageType(ResistanceType.Fire, 20);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 85.1, 95.0);
            this.SetSkill(SkillName.Tactics, 70.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);
            this.SetSkill(SkillName.Magery, 80.1, 90.0);
            this.SetSkill(SkillName.EvalInt, 70.1, 80.0);
            this.SetSkill(SkillName.Meditation, 70.1, 80.0);

            this.Fame = 7000;
            this.Karma = -10000;

            this.VirtualArmor = 55;
        }

        public ArcaneDaemon(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.ConcussionBlow;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average, 2);
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