using System;
using System.Collections.Generic;

namespace Server.Items
{
    /// <summary>
    /// Gain a defensive advantage over your primary opponent for a short time.
    /// </summary>
    public class Feint : WeaponAbility
	{
        private static Dictionary<Mobile, FeintTimer> m_Registry = new Dictionary<Mobile, FeintTimer>();
        public static Dictionary<Mobile, FeintTimer> Registry { get { return m_Registry; } }

		public Feint()
		{
		}

		public override int BaseMana { get { return 30; } }

		public override bool CheckSkills( Mobile from )
		{
			if( GetSkill( from, SkillName.Ninjitsu ) < 50.0  && GetSkill( from, SkillName.Bushido ) < 50.0 )
			{
				from.SendLocalizedMessage( 1063347, "50" ); // You need ~1_SKILL_REQUIREMENT~ Bushido or Ninjitsu skill to perform that attack!
				return false;
			}

			return base.CheckSkills( from );
		}

		public override void OnHit( Mobile attacker, Mobile defender, int damage )
		{
			if( !Validate( attacker ) || !CheckMana( attacker, true ) )
				return;

			if( Registry.ContainsKey( attacker ) )
			{
                if (m_Registry[attacker] != null)
                    m_Registry[attacker].Stop();

                Registry.Remove(attacker);
			}

			ClearCurrentAbility( attacker );

			attacker.SendLocalizedMessage( 1063360 ); // You baffle your target with a feint!
			defender.SendLocalizedMessage( 1063361 ); // You were deceived by an attacker's feint!

			attacker.FixedParticles( 0x3728, 1, 13, 0x7F3, 0x962, 0, EffectLayer.Waist );
            int bonus = (int)(20.0 + 3.0 * (Math.Max(attacker.Skills[SkillName.Ninjitsu].Value, attacker.Skills[SkillName.Bushido].Value) - 50.0) / 7.0);

			FeintTimer t = new FeintTimer( attacker, defender, bonus );	//20-50 % decrease
   
			t.Start();
			m_Registry[attacker] = t;

            string args = String.Format("{0}\t{1}", defender.Name, bonus);
            BuffInfo.AddBuff(attacker, new BuffInfo(BuffIcon.Feint, 1151308, 1151307, TimeSpan.FromSeconds(6), attacker, args));
		}

        public class FeintTimer : Timer
        {
            private Mobile m_Owner;
            private Mobile m_Enemy;
            private int m_DamageReduction;

            public Mobile Owner { get { return m_Owner; } }
            public Mobile Enemy { get { return m_Enemy; } }

            public int DamageReduction { get { return m_DamageReduction; } }

            public FeintTimer(Mobile owner, Mobile enemy, int DamageReduction)
                : base(TimeSpan.FromSeconds(6.0))
            {
                m_Owner = owner;
                m_Enemy = enemy;
                m_DamageReduction = DamageReduction;
                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                Registry.Remove(m_Owner);
            }
        }
    }
}