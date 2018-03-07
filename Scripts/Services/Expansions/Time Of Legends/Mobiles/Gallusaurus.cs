using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a gallusaurus corpse")]
    public class Gallusaurus : BaseCreature
    {
        public override bool AttacksFocus { get { return !Controlled; } }

        [Constructable]
        public Gallusaurus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            this.Name = "a gallusaurus";
            this.Body = 1286;
            this.BaseSoundID = 0x275;

            this.SetStr(477, 511);
            this.SetDex(155, 168);
            this.SetInt(221, 274);

            this.SetDamage(11, 17);

            this.SetHits(700, 900);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.MagicResist, 70.0, 80.0);
            this.SetSkill(SkillName.Tactics, 80.0, 90.0);
            this.SetSkill(SkillName.Wrestling, 80.0, 91.0);
            this.SetSkill(SkillName.Bushido, 110.0, 120.0);
            this.SetSkill(SkillName.DetectHidden, 25.0, 35.0);

            this.Fame = 8100;
            this.Karma = -8100;

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 102.0;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 1);
        }

        public override int Meat { get { return 3; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Block;
        }

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