using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a kaze kemono corpse")]
    public class KazeKemono : BaseCreature
    {
        private static readonly Hashtable m_FlurryOfTwigsTable = new Hashtable();
        private static readonly Hashtable m_ChlorophylBlastTable = new Hashtable();
        [Constructable]
        public KazeKemono()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a kaze kemono";
            this.Body = 196;
            this.BaseSoundID = 655;

            this.SetStr(201, 275);
            this.SetDex(101, 155);
            this.SetInt(101, 105);

            this.SetHits(251, 330);

            this.SetDamage(15, 20);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Fire, 10);
            this.SetDamageType(ResistanceType.Cold, 10);
            this.SetDamageType(ResistanceType.Poison, 10);

            this.SetResistance(ResistanceType.Physical, 50, 70);
            this.SetResistance(ResistanceType.Fire, 30, 60);
            this.SetResistance(ResistanceType.Cold, 30, 60);
            this.SetResistance(ResistanceType.Poison, 50, 70);
            this.SetResistance(ResistanceType.Energy, 60, 80);

            this.SetSkill(SkillName.MagicResist, 110.1, 125.0);
            this.SetSkill(SkillName.Tactics, 55.1, 65.0);
            this.SetSkill(SkillName.Wrestling, 85.1, 95.0);
            this.SetSkill(SkillName.Anatomy, 25.1, 35.0);
            this.SetSkill(SkillName.Magery, 95.1, 105.0);

            this.Fame = 8000;
            this.Karma = -8000;
        }

        public KazeKemono(Serial serial)
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
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 3);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 > Utility.RandomDouble())
            {
                /* Flurry of Twigs
                * Start cliloc: 1070850
                * Effect: Physical resistance -15% for 5 seconds
                * End cliloc: 1070852
                * Effect: Type: "3" From: "0x57D4F5B" To: "0x0" ItemId: "0x37B9" ItemIdName: "glow" FromLocation: "(1048 779, 6)" ToLocation: "(1048 779, 6)" Speed: "10" Duration: "5" FixedDirection: "True" Explode: "False"
                */
                ExpireTimer timer = (ExpireTimer)m_FlurryOfTwigsTable[defender];

                if (timer != null)
                {
                    timer.DoExpire();
                    defender.SendLocalizedMessage(1070851); // The creature lands another blow in your weakened state.
                }
                else
                    defender.SendLocalizedMessage(1070850); // The creature's flurry of twigs has made you more susceptible to physical attacks!

                int effect = -(defender.PhysicalResistance * 15 / 100);

                ResistanceMod mod = new ResistanceMod(ResistanceType.Physical, effect);

                defender.FixedEffect(0x37B9, 10, 5);
                defender.AddResistanceMod(mod);

                timer = new ExpireTimer(defender, mod, m_FlurryOfTwigsTable, TimeSpan.FromSeconds(5.0));
                timer.Start();
                m_FlurryOfTwigsTable[defender] = timer;
            }
            else if (0.05 > Utility.RandomDouble())
            {
                /* Chlorophyl Blast
                * Start cliloc: 1070827
                * Effect: Energy resistance -50% for 10 seconds
                * End cliloc: 1070829
                * Effect: Type: "3" From: "0x57D4F5B" To: "0x0" ItemId: "0x37B9" ItemIdName: "glow" FromLocation: "(1048 779, 6)" ToLocation: "(1048 779, 6)" Speed: "10" Duration: "5" FixedDirection: "True" Explode: "False"
                */
                ExpireTimer timer = (ExpireTimer)m_ChlorophylBlastTable[defender];

                if (timer != null)
                {
                    timer.DoExpire();
                    defender.SendLocalizedMessage(1070828); // The creature continues to hinder your energy resistance!
                }
                else
                    defender.SendLocalizedMessage(1070827); // The creature's attack has made you more susceptible to energy attacks!

                int effect = -(defender.EnergyResistance / 2);

                ResistanceMod mod = new ResistanceMod(ResistanceType.Energy, effect);

                defender.FixedEffect(0x37B9, 10, 5);
                defender.AddResistanceMod(mod);

                timer = new ExpireTimer(defender, mod, m_ChlorophylBlastTable, TimeSpan.FromSeconds(10.0));
                timer.Start();
                m_ChlorophylBlastTable[defender] = timer;
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

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly ResistanceMod m_Mod;
            private readonly Hashtable m_Table;
            public ExpireTimer(Mobile m, ResistanceMod mod, Hashtable table, TimeSpan delay)
                : base(delay)
            {
                this.m_Mobile = m;
                this.m_Mod = mod;
                this.m_Table = table;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                this.m_Mobile.RemoveResistanceMod(this.m_Mod);
                this.Stop();
                this.m_Table.Remove(this.m_Mobile);
            }

            protected override void OnTick()
            {
                if (this.m_Mod.Type == ResistanceType.Physical)
                    this.m_Mobile.SendLocalizedMessage(1070852); // Your resistance to physical attacks has returned.
                else
                    this.m_Mobile.SendLocalizedMessage(1070829); // Your resistance to energy attacks has returned.

                this.DoExpire();
            }
        }
    }
}