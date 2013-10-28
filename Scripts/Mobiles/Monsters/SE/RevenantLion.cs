using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a revenant lion corpse")]
    public class RevenantLion : BaseCreature
    {
        [Constructable]
        public RevenantLion()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Revenant Lion";
            this.Body = 251;

            this.SetStr(276, 325);
            this.SetDex(156, 175);
            this.SetInt(76, 105);

            this.SetHits(251, 280);

            this.SetDamage(18, 24);

            this.SetDamageType(ResistanceType.Physical, 30);
            this.SetDamageType(ResistanceType.Cold, 30);
            this.SetDamageType(ResistanceType.Poison, 10);
            this.SetDamageType(ResistanceType.Energy, 30);

            this.SetResistance(ResistanceType.Physical, 40, 60);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.EvalInt, 80.1, 90.0);
            this.SetSkill(SkillName.Magery, 80.1, 90.0);
            this.SetSkill(SkillName.Poisoning, 120.1, 130.0);
            this.SetSkill(SkillName.MagicResist, 70.1, 90.0);
            this.SetSkill(SkillName.Tactics, 60.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 88.0);

            this.Fame = 4000;
            this.Karma = -4000;
            this.PackNecroReg(6, 8);
			
            switch ( Utility.Random(10))
            {
                case 0:
                    this.PackItem(new LeftArm());
                    break;
                case 1:
                    this.PackItem(new RightArm());
                    break;
                case 2:
                    this.PackItem(new Torso());
                    break;
                case 3:
                    this.PackItem(new Bone());
                    break;
                case 4:
                    this.PackItem(new RibCage());
                    break;
                case 5:
                    this.PackItem(new RibCage());
                    break;
                case 6:
                    this.PackItem(new BonePile());
                    break;
                case 7:
                    this.PackItem(new BonePile());
                    break;
                case 8:
                    this.PackItem(new BonePile());
                    break;
                case 9:
                    this.PackItem(new BonePile());
                    break;
            }
        }

        public RevenantLion(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
        }

        public override int GetAngerSound()
        {
            return 0x518;
        }

        public override int GetIdleSound()
        {
            return 0x517;
        }

        public override int GetAttackSound()
        {
            return 0x516;
        }

        public override int GetHurtSound()
        {
            return 0x519;
        }

        public override int GetDeathSound()
        {
            return 0x515;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 2);
            this.AddLoot(LootPack.MedScrolls, 2);
            // TODO: Bone Pile
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