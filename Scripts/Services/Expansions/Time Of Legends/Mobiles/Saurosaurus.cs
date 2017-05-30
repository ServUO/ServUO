using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a saurosaurus corpse")]
    public class Saurosaurus : BaseCreature
    {
        public override bool AttacksFocus { get { return !Controlled; } }

        [Constructable]
        public Saurosaurus() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, .2, .4)
        {
            this.Name = "a saurosaurus";
            this.Body = 1291;
            this.BaseSoundID = 362;

            this.SetStr(802, 824);
            this.SetDex(201, 220);
            this.SetInt(403, 440);

            this.SetDamage(21, 28);

            this.SetHits(1321, 1468);

            this.SetResistance(ResistanceType.Physical, 75, 85);
            this.SetResistance(ResistanceType.Fire, 80, 90);
            this.SetResistance(ResistanceType.Cold, 45, 55);
            this.SetResistance(ResistanceType.Poison, 35, 45);
            this.SetResistance(ResistanceType.Energy, 45, 55);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetSkill(SkillName.MagicResist, 70.0, 90.0);
            this.SetSkill(SkillName.Tactics, 110.0, 120.0);
            this.SetSkill(SkillName.Wrestling, 110.0, 130.0);
            this.SetSkill(SkillName.Anatomy, 50.0, 60.0);
            this.SetSkill(SkillName.DetectHidden, 80.0);
            this.SetSkill(SkillName.Parry, 80.0, 90);
            this.SetSkill(SkillName.Focus, 115.0, 125.0);

            this.Fame = 11000;
            this.Karma = -11000;

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 102.0;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
        }

        // Missing: Life Leech, Tail Swipe ability

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.ConcussionBlow;
        }

        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }
        public override int DragonBlood { get { return 8; } }
        public override int Meat { get { return 5; } }
        public override int Hides { get { return 11; } }

        public Saurosaurus(Serial serial) : base(serial)
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