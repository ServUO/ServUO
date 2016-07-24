using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a wild tiger corpse")]
    public class Wild_Tiger : BaseCreature
    {
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
        }

        private static int m_MinTime = 4;
        private static int m_MaxTime = 8;
        private DateTime m_NextAbilityTime;
        private ExpireTimer m_Timer;

        [Constructable]
        public Wild_Tiger()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a wild tiger";

            switch (Utility.Random(2))
            {
                case 0:
                    {
                        BodyValue = 1254;
                        break;
                    }
                case 1:
                    {
                        BodyValue = 1255;
                        break;
                    }
            }

            //Add a low chance of the tiger being a white tiger
            int hueValue = Utility.Random(500);

            if (hueValue <= 1)
            {
                Hue = 0x481;
            }

            SetStr(500, 555);
            SetDex(85, 125);
            SetInt(85, 165);

            SetHits(350, 450);

            SetDamage(18, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 75);
            SetResistance(ResistanceType.Fire, 20, 40);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.Anatomy, 0, 0);
            SetSkill(SkillName.MagicResist, 91.4, 93.4);
            SetSkill(SkillName.Tactics, 108.1, 110.0);
            SetSkill(SkillName.Wrestling, 97.3, 98.2);

            Fame = 14000;
            Karma = -14000;

            VirtualArmor = 60;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 107.1;
        }

        public Wild_Tiger(Serial serial)
            : base(serial)
        {
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

        public override bool CanAngerOnTame { get { return true; } }
        //public override int Pelt { get { return 30; } }
        //public override PeltType PeltType
        //{
        //    get
         //   {
         //       return PeltType.Orange;
         //   }
       // } Not emplanted in Servuo will be soon as i get the okay.
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override int Meat { get { return 16; } }

        /*
        public override void OnThink()
        {
            if (DateTime.UtcNow >= m_NextAbilityTime)
            {
                BaseAttackHelperSE.HiryuAbilitiesAttack(this, ref m_Timer);

                m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(m_MinTime, m_MaxTime));
            }

            base.OnThink();
        }
     */ // will add these if i get the okay from servuo.

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (BaseSoundID == 357)
            {
                BaseSoundID = 0x451;
            }
        }
    }
}