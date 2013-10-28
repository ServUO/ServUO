using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a tsuki wolf corpse")]
    public class TsukiWolf : BaseCreature
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public TsukiWolf()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a tsuki wolf";
            this.Body = 250;

            switch( Utility.Random(3) )
            {
                case 0:
                    this.Hue = Utility.RandomNeutralHue();
                    break; //No, this really isn't accurate ;->
            }

            this.SetStr(401, 450);
            this.SetDex(151, 200);
            this.SetInt(66, 76);

            this.SetHits(376, 450);
            this.SetMana(40);

            this.SetDamage(14, 18);

            this.SetDamageType(ResistanceType.Physical, 90);
            this.SetDamageType(ResistanceType.Cold, 5);
            this.SetDamageType(ResistanceType.Energy, 5);

            this.SetResistance(ResistanceType.Physical, 40, 60);
            this.SetResistance(ResistanceType.Fire, 50, 70);
            this.SetResistance(ResistanceType.Cold, 50, 70);
            this.SetResistance(ResistanceType.Poison, 50, 70);
            this.SetResistance(ResistanceType.Energy, 50, 70);

            this.SetSkill(SkillName.Anatomy, 65.1, 72.0);
            this.SetSkill(SkillName.MagicResist, 65.1, 70.0);
            this.SetSkill(SkillName.Tactics, 95.1, 110.0);
            this.SetSkill(SkillName.Wrestling, 97.6, 107.5);

            this.Fame = 8500;
            this.Karma = -8500;

            if (Core.ML && Utility.RandomDouble() < .33)
                this.PackItem(Engines.Plants.Seed.RandomPeculiarSeed(1));

            switch( Utility.Random(10) )
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

        public TsukiWolf(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 4;
            }
        }
        public override int Hides
        {
            get
            {
                return 25;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Rich);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 > Utility.RandomDouble())
            {
                /* Blood Bath
                * Start cliloc 1070826
                * Sound: 0x52B
                * 2-3 blood spots
                * Damage: 2 hps per second for 5 seconds
                * End cliloc: 1070824
                */
                ExpireTimer timer = (ExpireTimer)m_Table[defender];

                if (timer != null)
                {
                    timer.DoExpire();
                    defender.SendLocalizedMessage(1070825); // The creature continues to rage!
                }
                else
                    defender.SendLocalizedMessage(1070826); // The creature goes into a rage, inflicting heavy damage!

                timer = new ExpireTimer(defender, this);
                timer.Start();
                m_Table[defender] = timer;
            }
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

        public override int GetAngerSound()
        {
            return 0x52D;
        }

        public override int GetIdleSound()
        {
            return 0x52C;
        }

        public override int GetAttackSound()
        {
            return 0x52B;
        }

        public override int GetHurtSound()
        {
            return 0x52E;
        }

        public override int GetDeathSound()
        {
            return 0x52A;
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly Mobile m_From;
            private int m_Count;
            public ExpireTimer(Mobile m, Mobile from)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                this.m_Mobile = m;
                this.m_From = from;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                this.Stop();
                m_Table.Remove(this.m_Mobile);
            }

            public void DrainLife()
            {
                if (this.m_Mobile.Alive)
                    this.m_Mobile.Damage(2, this.m_From);
                else
                    this.DoExpire();
            }

            protected override void OnTick()
            {
                this.DrainLife();

                if (++this.m_Count >= 5)
                {
                    this.DoExpire();
                    this.m_Mobile.SendLocalizedMessage(1070824); // The creature's rage subsides.
                }
            }
        }
    }
}