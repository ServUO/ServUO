using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a treefellow corpse")]
    public class FeralTreefellow : BaseCreature
    {
        [Constructable]
        public FeralTreefellow()
            : base(AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            this.Name = "a feral treefellow";
            this.Body = 301;

            this.SetStr(1351, 1600);
            this.SetDex(301, 550);
            this.SetInt(651, 900);

            this.SetHits(1170, 1320);

            this.SetDamage(26, 35);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Cold, 70, 80);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 40, 60);

            this.SetSkill(SkillName.MagicResist, 40.1, 55.0);// Unknown
            this.SetSkill(SkillName.Tactics, 65.1, 90.0);// Unknown
            this.SetSkill(SkillName.Wrestling, 65.1, 85.0);// Unknown

            this.Fame = 1000;  //Unknown
            this.Karma = -3000;  //Unknown

            this.VirtualArmor = 24;
            this.PackItem(new Log(Utility.RandomMinMax(23, 34)));
        }

        public FeralTreefellow(Serial serial)
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
            this.AddLoot(LootPack.Average); //Unknown
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

    [CorpseName("a treefellow corpse")]
    public class FerelTreefellow : BaseCreature
    {
        [Constructable]
        public FerelTreefellow()
            : base(AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            this.Name = "a feral treefellow";
            this.Body = 301;

            this.SetStr(1351, 1600);
            this.SetDex(301, 550);
            this.SetInt(651, 900);

            this.SetHits(1170, 1320);

            this.SetDamage(26, 35);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Cold, 70, 80);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 40, 60);

            this.SetSkill(SkillName.MagicResist, 40.1, 55.0);// Unknown
            this.SetSkill(SkillName.Tactics, 65.1, 90.0);// Unknown
            this.SetSkill(SkillName.Wrestling, 65.1, 85.0);// Unknown

            this.Fame = 12500;  //Unknown
            this.Karma = 12500;  //Unknown

            this.VirtualArmor = 24;
            this.PackItem(new Log(Utility.RandomMinMax(23, 34)));
        }

        public FerelTreefellow(Serial serial)
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
            this.AddLoot(LootPack.Average); //Unknown
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