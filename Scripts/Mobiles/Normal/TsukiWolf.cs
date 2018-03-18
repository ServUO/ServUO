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
            Name = "a tsuki wolf";
            Body = 250;

            switch( Utility.Random(3) )
            {
                case 0:
                    Hue = Utility.RandomNeutralHue();
                    break; //No, this really isn't accurate ;->
            }

            SetStr(401, 450);
            SetDex(151, 200);
            SetInt(66, 76);

            SetHits(376, 450);
            SetMana(40);

            SetDamage(14, 18);

            SetDamageType(ResistanceType.Physical, 90);
            SetDamageType(ResistanceType.Cold, 5);
            SetDamageType(ResistanceType.Energy, 5);

            SetResistance(ResistanceType.Physical, 40, 60);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 50, 70);
            SetResistance(ResistanceType.Poison, 50, 70);
            SetResistance(ResistanceType.Energy, 50, 70);

            SetSkill(SkillName.Anatomy, 65.1, 72.0);
            SetSkill(SkillName.MagicResist, 65.1, 70.0);
            SetSkill(SkillName.Tactics, 95.1, 110.0);
            SetSkill(SkillName.Wrestling, 97.6, 107.5);
            SetSkill(SkillName.Necromancy, 20.0);
            SetSkill(SkillName.SpiritSpeak, 20.0);
            SetSkill(SkillName.Anatomy, 40.0, 50.0);
            SetSkill(SkillName.DetectHidden, 100.0);
            SetSkill(SkillName.Parry, 90.0, 100.0);

            Fame = 8500;
            Karma = -8500;

            
            if (Core.ML && Utility.RandomDouble() < .33)
                PackItem(Engines.Plants.Seed.RandomPeculiarSeed(1));

            PackBodyPartOrBones();
            
            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 96.0;
        }

        public TsukiWolf(Serial serial)
            : base(serial)
        {
        }

        public override bool CanAngerOnTame { get { return true; } }
        public override int Meat { get { return 4; } }
        public override int Hides { get { return 25; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
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
                m_Mobile = m;
                m_From = from;
                Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                Stop();
                m_Table.Remove(m_Mobile);
            }

            public void DrainLife()
            {
                if (m_Mobile.Alive)
                    m_Mobile.Damage(2, m_From);
                else
                    DoExpire();
            }

            protected override void OnTick()
            {
                DrainLife();

                if (++m_Count >= 5)
                {
                    DoExpire();
                    m_Mobile.SendLocalizedMessage(1070824); // The creature's rage subsides.
                }
            }
        }
    }
}
