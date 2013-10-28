using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Fairy dragon corpse")]
    public class FairyDragon : BaseCreature
    {
        [Constructable]
        public FairyDragon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Fairy Dragon";
            this.Body = 718;
            this.BaseSoundID = 362;

            this.SetStr(512, 558);
            this.SetDex(95, 105);
            this.SetInt(455, 501);

            this.SetHits(398, 403);

            this.SetDamage(15, 18);

            //SetDamageType( ResistanceType.Physical, 100 );
            this.SetDamageType(ResistanceType.Fire, 20, 25);
            this.SetDamageType(ResistanceType.Cold, 20, 25);
            this.SetDamageType(ResistanceType.Poison, 20, 25);
            this.SetDamageType(ResistanceType.Energy, 20, 25);

            this.SetResistance(ResistanceType.Physical, 16, 30);
            this.SetResistance(ResistanceType.Fire, 41, 44);
            this.SetResistance(ResistanceType.Cold, 40, 49);
            this.SetResistance(ResistanceType.Poison, 40, 49);
            this.SetResistance(ResistanceType.Energy, 45, 47);

            this.SetSkill(SkillName.EvalInt, 30.1, 40.0);
            this.SetSkill(SkillName.Magery, 30.1, 40.0);
            this.SetSkill(SkillName.MagicResist, 99.1, 100.0);
            this.SetSkill(SkillName.Tactics, 60.6, 68.2);
            this.SetSkill(SkillName.Wrestling, 90.1, 92.5);

            this.Fame = 15000;
            this.Karma = -15000;

            this.VirtualArmor = 39;
			
            this.PackItem(new FaeryDust());
            // edit for arti drop?		
            if (Utility.Random(100) < 25)
            {
                switch ( Utility.Random(2))
                {
                    case 0:
                        this.PackItem(new FeyWings());
                        break;
                    case 1:
                        this.PackItem(new FairyDragonWing());
                        break;
                    case 3:
                        this.PackItem(new SignOfChaos());
                        break;
                }
            }

            this.Tamable = false;
            this.ControlSlots = 3;
            this.MinTameSkill = 93.9;
        }

        public FairyDragon(Serial serial)
            : base(serial)
        {
        }

        public override bool ReacquireOnMovement
        {
            get
            {
                return !this.Controlled;
            }
        }
        //public override bool HasBreath{ get{ return true; } } // fire breath enabled
        public override bool AutoDispel
        {
            get
            {
                return !this.Controlled;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 4;
            }
        }
        public override int Meat
        {
            get
            {
                return 9;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override double HitPoisonChance
        {
            get
            {
                return 0.75;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override bool CanAngerOnTame
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.MedScrolls, 2);
        }

        public override int GetAttackSound()
        {
            return 0x5E9;
        }

        //public override int GetAngerSound()
        //{
        //	return 718;
        //}
        public override int GetDeathSound()
        {
            return 0x5EA;
        }

        public override int GetHurtSound()
        {
            return 0x5EB;
        }

        public override int GetIdleSound()
        {
            return 0x5EC;
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