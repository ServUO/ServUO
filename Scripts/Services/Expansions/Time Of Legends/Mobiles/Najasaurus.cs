using System;
using Server;

namespace Server.Mobiles
{
    [CorpseName("a najasaurus corpse")]
    public class Najasaurus : BaseCreature
    {
        public override bool AttacksFocus { get { return !Controlled; } }

        [Constructable]
        public Najasaurus()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, .2, .4)
        {
            this.Name = "a najasaurus";
            this.Body = 1289;
            this.BaseSoundID = 219;

            this.SetStr(160, 300);
            this.SetDex(160, 300);
            this.SetInt(20, 40);

            this.SetDamage(13, 24);
            this.SetHits(700, 900);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 45, 55);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 35, 45);

            this.SetSkill(SkillName.MagicResist, 150.0, 190.0);
            this.SetSkill(SkillName.Tactics, 80.0, 95.0);
            this.SetSkill(SkillName.Wrestling, 80.0, 100.0);
            this.SetSkill(SkillName.Poisoning, 90.0, 100.0);
            this.SetSkill(SkillName.DetectHidden, 45.0, 55.0);

            this.Fame = 17000;
            this.Karma = -17000;

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 102.0;
        }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
        }

        public Najasaurus(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}