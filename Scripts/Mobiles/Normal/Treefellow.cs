using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a treefellow corpse")]
    public class Treefellow : BaseCreature
    {
        [Constructable]
        public Treefellow()
            : base(AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            this.Name = "a treefellow";
            this.Body = 301;

            this.SetStr(196, 220);
            this.SetDex(31, 55);
            this.SetInt(66, 90);

            this.SetHits(118, 132);

            this.SetDamage(12, 16);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 30, 35);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.MagicResist, 40.1, 55.0);
            this.SetSkill(SkillName.Tactics, 65.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 65.1, 85.0);

            this.Fame = 500;
            this.Karma = 1500;

            this.VirtualArmor = 24;
            this.PackItem(new Log(Utility.RandomMinMax(23, 34)));
        }

        public Treefellow(Serial serial)
            : base(serial)
        {
        }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Dismount;
        }

        public override int GetIdleSound()
        {
            return 443;
        }

        public override int GetDeathSound()
        {
            return 31;
        }

        public override int GetAttackSound()
        {
            return 672;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
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

            if (this.BaseSoundID == 442)
                this.BaseSoundID = -1;
        }
    }
}