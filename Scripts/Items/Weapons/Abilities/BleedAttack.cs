using System;
using System.Collections;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Necromancy;

namespace Server.Items
{
    /// <summary>
    /// Make your opponent bleed profusely with this wicked use of your weapon.
    /// When successful, the target will bleed for several seconds, taking damage as time passes for up to ten seconds.
    /// The rate of damage slows down as time passes, and the blood loss can be completely staunched with the use of bandages. 
    /// </summary>
    public class BleedAttack : WeaponAbility
    {
        private static readonly Hashtable m_Table = new Hashtable();
        public BleedAttack()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 30;
            }
        }
		
		public static bool IsBleeding(Mobile m)
        {
            return m_Table.Contains(m);
        }
		
		public static void BeginBleed(Mobile m, Mobile from, bool blooddrinker)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            t = new InternalTimer(from, m, blooddrinker);
            m_Table[m] = t;

            t.Start();
        }

        public static void DoBleed(Mobile m, Mobile from, int level, bool blooddrinker)
        {
            if (m.Alive)
            {
                int damage = Utility.RandomMinMax(level, level * 2);

                if (!m.Player)
                    damage *= 2;

                m.PlaySound(0x133);
                m.Damage(damage, from);
				
				if (blooddrinker)
				{
					from.Heal(damage, from, true); // Check for OSI message accuracy instead of standard heal message
				}

                Blood blood = new Blood();

                blood.ItemID = Utility.Random(0x122A, 5);

                blood.MoveToWorld(m.Location, m.Map);
            }
            else
            {
                EndBleed(m, false);
            }
        }

        public static void EndBleed(Mobile m, bool message)
        {
            Timer t = (Timer)m_Table[m];

            if (t == null)
                return;

            t.Stop();
            m_Table.Remove(m);

            if (message)
                m.SendLocalizedMessage(1060167); // The bleeding wounds have healed, you are no longer bleeding!
        }
		
		public static bool CheckBloodDrink(Mobile attacker)
		{
			Item onehand = attacker.FindItemOnLayer( Layer.OneHanded );
			Item twohand = attacker.FindItemOnLayer( Layer.TwoHanded );

			BaseWeapon bw = null;
			if (onehand is BaseWeapon)
				bw = onehand as BaseWeapon;
			else if (twohand is BaseWeapon)
				bw = twohand as BaseWeapon;

		    if (bw !=null)
                return (bw.WeaponAttributes.BloodDrinker != 0);

		    return false;
		}

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            // Necromancers under Lich or Wraith Form are immune to Bleed Attacks.
            TransformContext context = TransformationSpellHelper.GetContext(defender);

            if ((context != null && (context.Type == typeof(LichFormSpell) || context.Type == typeof(WraithFormSpell))) ||
                (defender is BaseCreature && ((BaseCreature)defender).BleedImmune))
            {
                attacker.SendLocalizedMessage(1062052); // Your target is not affected by the bleed attack!
                return;
            }

            attacker.SendLocalizedMessage(1060159); // Your target is bleeding!
            defender.SendLocalizedMessage(1060160); // You are bleeding!

            if (defender is PlayerMobile)
            {
                defender.LocalOverheadMessage(MessageType.Regular, 0x21, 1060757); // You are bleeding profusely
                defender.NonlocalOverheadMessage(MessageType.Regular, 0x21, 1060758, defender.Name); // ~1_NAME~ is bleeding profusely
            }

            bool blooddrinker = CheckBloodDrink(attacker);

            defender.PlaySound(0x133);
            defender.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist);
			
			BeginBleed(defender, attacker, blooddrinker);
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;
            private readonly Mobile m_Mobile;
            private int m_Count;
			private readonly bool m_blooddrinker;
            public InternalTimer(Mobile from, Mobile m, bool blooddrinker)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_From = from;
                m_Mobile = m;
                Priority = TimerPriority.TwoFiftyMS;
				m_blooddrinker = blooddrinker;
			}

            protected override void OnTick()
            {
                DoBleed(m_Mobile, m_From, 5 - m_Count, m_blooddrinker);

                if (++m_Count == 5)
                    EndBleed(m_Mobile, true);
            }
        }
    }
}