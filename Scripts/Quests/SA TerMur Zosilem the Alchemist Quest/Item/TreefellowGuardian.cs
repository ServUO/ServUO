using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a treefellow guardian corpse")]
    public class TreefellowGuardian : BaseCreature
    {
        [Constructable]
        public TreefellowGuardian()
            : base(AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Treefellow Guardian";
            this.Body = 301;

            this.SetStr(511, 695);
            this.SetDex(30, 55);
            this.SetInt(403, 491);

            this.SetHits(724, 900);

            this.SetDamage(12, 16);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 80, 90);

            this.SetSkill(SkillName.MagicResist, 40.1, 55.0);
            this.SetSkill(SkillName.Tactics, 65.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 65.1, 85.0);

            this.Fame = 500;
            this.Karma = 1500;

            this.VirtualArmor = 24;
            this.PackItem(new Log(Utility.RandomMinMax(23, 34)));

            if (0.05 > Utility.RandomDouble())
                this.PackItem(new TreefellowWood()); 
        }

        public TreefellowGuardian(Serial serial)
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