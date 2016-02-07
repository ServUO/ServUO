using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lady of the snow corpse")]
    public class LadyOfTheSnow : BaseCreature
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public LadyOfTheSnow()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a lady of the snow";
            this.Body = 252;
            this.BaseSoundID = 0x482;

            this.SetStr(276, 305);
            this.SetDex(106, 125);
            this.SetInt(471, 495);

            this.SetHits(596, 625);

            this.SetDamage(13, 20);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Cold, 80);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 40, 55);
            this.SetResistance(ResistanceType.Cold, 70, 90);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 65, 85);

            this.SetSkill(SkillName.Magery, 95.1, 110.0);
            this.SetSkill(SkillName.MagicResist, 90.1, 105.0);
            this.SetSkill(SkillName.Tactics, 80.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 100.0);
            this.SetSkill(SkillName.Necromancy, 90, 110.0);
            this.SetSkill(SkillName.SpiritSpeak, 90.0, 110.0);

            this.Fame = 15200;
            this.Karma = -15200;

            this.PackReg(3);
            this.PackItem(new Necklace());

            if (0.25 > Utility.RandomDouble())
                this.PackItem(Engines.Plants.Seed.RandomBonsaiSeed());
        }

        public LadyOfTheSnow(Serial serial)
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
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 4;
            }
        }
        public override int GetDeathSound()
        {
            return 0x370;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
        }

        // TODO: Snowball
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 > Utility.RandomDouble())
            {
                /* Cold Wind
                * Graphics: Message - Type: "3" From: "0x57D4F5B" To: "0x0" ItemId: "0x37B9" ItemIdName: "glow" FromLocation: "(928 164, 34)" ToLocation: "(928 164, 34)" Speed: "10" Duration: "5" FixedDirection: "True" Explode: "False"
                * Start cliloc: 1070832
                * Damage: 1hp per second for 5 seconds
                * End cliloc: 1070830
                * Reset cliloc: 1070831
                */
                ExpireTimer timer = (ExpireTimer)m_Table[defender];

                if (timer != null)
                {
                    timer.DoExpire();
                    defender.SendLocalizedMessage(1070831); // The freezing wind continues to blow!
                }
                else
                    defender.SendLocalizedMessage(1070832); // An icy wind surrounds you, freezing your lungs as you breathe!

                timer = new ExpireTimer(defender, this);
                timer.Start();
                m_Table[defender] = timer;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
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
                    this.m_Mobile.SendLocalizedMessage(1070830); // The icy wind dissipates.
                }
            }
        }
    }
}