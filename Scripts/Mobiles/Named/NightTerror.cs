using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a night terror corpse")]
    public class NightTerror : BaseCreature
    {
        [Constructable]
        public NightTerror()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Night Terror";
            this.Body = 0x30c;
            this.Hue = 2963;

            this.SetStr(385, 467);
            this.SetDex(40, 70);
            this.SetInt(600, 800);

            this.SetHits(50000);

            this.SetDamage(10, 23);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 100);
            this.SetResistance(ResistanceType.Cold, 80, 90);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 70, 80);

            this.SetSkill(SkillName.MagicResist, 90.0, 100.0);
            this.SetSkill(SkillName.Tactics, 120.0);
            this.SetSkill(SkillName.Wrestling, 110.0);
            this.SetSkill(SkillName.Poisoning, 120.0);
            this.SetSkill(SkillName.DetectHidden, 120.0);
            this.SetSkill(SkillName.Parry, 60.0, 70.0);
            this.SetSkill(SkillName.Magery, 100.0);
            this.SetSkill(SkillName.EvalInt, 110.0);
            this.SetSkill(SkillName.Necromancy, 120.0);
            this.SetSkill(SkillName.SpiritSpeak, 120.0);

            this.Fame = 8000;
            this.Karma = -8000;

            this.VirtualArmor = 54;
        }

        public NightTerror(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override double HitPoisonChance { get { return 0.75; } }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
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
