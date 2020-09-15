using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class PsychicAttack : WeaponAbility
    {
        public override int BaseMana => 30;

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1074383); // Your shot sends forth a wave of psychic energy.
            defender.SendLocalizedMessage(1074384); // Your mind is attacked by psychic force!

            defender.FixedParticles(0x3789, 10, 25, 5032, EffectLayer.Head);
            defender.PlaySound(0x1F8);

            if (m_Registry.ContainsKey(defender))
            {
                if (!m_Registry[defender].DoneIncrease)
                {
                    m_Registry[defender].SpellDamageMalus *= 2;
                    m_Registry[defender].ManaCostMalus *= 2;
                }
            }
            else
                m_Registry[defender] = new PsychicAttackTimer(defender);

            BuffInfo.RemoveBuff(defender, BuffIcon.PsychicAttack);

            string args = string.Format("{0}\t{1}", m_Registry[defender].SpellDamageMalus, m_Registry[defender].ManaCostMalus);
            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.PsychicAttack, 1151296, 1151297, args));
        }

        private static readonly Dictionary<Mobile, PsychicAttackTimer> m_Registry = new Dictionary<Mobile, PsychicAttackTimer>();
        public static Dictionary<Mobile, PsychicAttackTimer> Registry => m_Registry;

        public static void RemoveEffects(Mobile defender)
        {
            if (defender == null)
                return;

            BuffInfo.RemoveBuff(defender, BuffIcon.PsychicAttack);

            if (m_Registry.ContainsKey(defender))
                m_Registry.Remove(defender);

            defender.SendLocalizedMessage(1150292); // You recover from the effects of the psychic attack.
        }

        public class PsychicAttackTimer : Timer
        {
            private readonly Mobile m_Defender;
            private int m_SpellDamageMalus;
            private int m_ManaCostMalus;
            private bool m_DoneIncrease;

            public int SpellDamageMalus { get { return m_SpellDamageMalus; } set { m_SpellDamageMalus = value; m_DoneIncrease = true; } }
            public int ManaCostMalus { get { return m_ManaCostMalus; } set { m_ManaCostMalus = value; m_DoneIncrease = true; } }
            public bool DoneIncrease => m_DoneIncrease;

            public PsychicAttackTimer(Mobile defender)
                : base(TimeSpan.FromSeconds(10))
            {
                m_Defender = defender;
                m_SpellDamageMalus = 15;
                m_ManaCostMalus = 15;
                m_DoneIncrease = false;
                Start();
            }

            protected override void OnTick()
            {
                RemoveEffects(m_Defender);
                Stop();
            }
        }
    }
}
