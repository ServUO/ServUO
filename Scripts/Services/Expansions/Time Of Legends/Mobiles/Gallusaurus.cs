using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a gallusaurus corpse")]
    public class Gallusaurus : BaseCreature
    {
        public override bool AttacksFocus { get { return true; } }

        [Constructable]
        public Gallusaurus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "an gallusaurus";
            Body = 1286;
            BaseSoundID = 0x275;

            SetStr(477, 511);
            SetDex(155, 168);
            SetInt(221, 274);

            SetDamage(11, 17);

            SetHits(2200);

            SetResistance(ResistanceType.Physical, 5, 6);
            SetResistance(ResistanceType.Fire, 2, 3);
            SetResistance(ResistanceType.Cold, 2);
            SetResistance(ResistanceType.Poison, 6, 7);
            SetResistance(ResistanceType.Energy, 2);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.MagicResist, 70, 80);
            SetSkill(SkillName.Tactics, 80, 90);
            SetSkill(SkillName.Wrestling, 80, 91);

            Fame = 8100;
            Karma = -8100;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 1);
        }

        public override int Meat { get { return 3; } }

        private static Dictionary<Mobile, ExpireTimer> _Table;

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 > Utility.RandomDouble())
            {
                /* Grasping Claw
                 * Start cliloc: 1070836
                 * Effect: Physical resistance -15% for 5 seconds
                 * End cliloc: 1070838
                 * Effect: Type: "3" - From: "0x57D4F5B" (player) - To: "0x0" - ItemId: "0x37B9" - ItemIdName: "glow" - FromLocation: "(1149 808, 32)" - ToLocation: "(1149 808, 32)" - Speed: "10" - Duration: "5" - FixedDirection: "True" - Explode: "False"
                 */

                if (_Table != null && _Table.ContainsKey(defender))
                {
                    _Table[defender].DoExpire();
                    defender.SendLocalizedMessage(1070837); // The creature lands another blow in your weakened state.
                }
                else
                    defender.SendLocalizedMessage(1070836); // The blow from the creature's claws has made you more susceptible to physical attacks.

                int effect = -(defender.PhysicalResistance * 15 / 100);

                ResistanceMod mod = new ResistanceMod(ResistanceType.Physical, effect);

                defender.FixedEffect(0x37B9, 10, 5);
                defender.AddResistanceMod(mod);

                ExpireTimer timer = new ExpireTimer(defender, mod, TimeSpan.FromSeconds(5.0));
                timer.Start();

                if (_Table == null)
                    _Table = new Dictionary<Mobile, ExpireTimer>();

                _Table[defender] = timer;
            }
        }

        private class ExpireTimer : Timer
        {
            private Mobile m_Mobile;
            private ResistanceMod m_Mod;

            public ExpireTimer(Mobile m, ResistanceMod mod, TimeSpan delay)
                : base(delay)
            {
                m_Mobile = m;
                m_Mod = mod;
                Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                m_Mobile.RemoveResistanceMod(m_Mod);
                Stop();
                _Table.Remove(m_Mobile);

                if (_Table.Count == 0)
                    _Table = null;
            }

            protected override void OnTick()
            {
                m_Mobile.SendLocalizedMessage(1070838); // Your resistance to physical attacks has returned.
                DoExpire();
            }
        }

        public Gallusaurus(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
	
}