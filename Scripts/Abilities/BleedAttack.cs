using System;
using System.Collections.Generic;
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
        private static readonly Dictionary<Mobile, BleedTimer> m_BleedTable = new Dictionary<Mobile, BleedTimer>();

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
            return m_BleedTable.ContainsKey(m);
        }
		
		public static void BeginBleed(Mobile m, Mobile from, bool splintering = false)
        {
            EndBleed(m, true);
            bool blooddrinker = CheckBloodDrink(from);
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Bleed, 1075829, 1075830, TimeSpan.FromSeconds(10), m, String.Format("{0}\t{1}\t{2}", "1", "10", "2")));

            BleedTimer timer;

            if (m_BleedTable.ContainsKey(m))
            {
                timer = m_BleedTable[m];

                if (timer.Splintering != splintering)
                {
                    timer.Stop();
                }
            }

            timer = new BleedTimer(from, m, blooddrinker, splintering);
            m_BleedTable[m] = timer;
            timer.Start();
        }

        public static void DoBleed(Mobile m, Mobile from, int damage, bool blooddrinker)
        {
            if (m.Alive && !m.IsDeadBondedPet)
            {
                if (!m.Player)
                    damage *= 2;

                m.PlaySound(0x133);
                AOS.Damage(m, from, damage, false, 0, 0, 0, 0, 0, 0, 100, false, false, false);

                if (blooddrinker && from.Hits < from.HitsMax)
                {
                    from.SendLocalizedMessage(1113606); //The blood drinker effect heals you.
                    from.Heal(damage);
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
            Timer t = null;

            if (m_BleedTable.ContainsKey(m))
            {
                t = m_BleedTable[m];
                m_BleedTable.Remove(m);
            }

            if (t == null)
                return;

            t.Stop();
            BuffInfo.RemoveBuff(m, BuffIcon.Bleed);

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
                (defender is BaseCreature && ((BaseCreature)defender).BleedImmune) || Server.Spells.Mysticism.StoneFormSpell.CheckImmunity(defender))
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

            defender.PlaySound(0x133);
            defender.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist);

			BeginBleed(defender, attacker);
        }

        private class BleedTimer : Timer
        {
            private readonly Mobile m_From;
            private readonly Mobile m_Mobile;
            private int m_Count;
            private int m_MaxCount;
            private readonly bool m_BloodDrinker;

            public bool Splintering { get; private set; }

            public BleedTimer(Mobile from, Mobile m, bool blooddrinker, bool splintering)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_From = from;
                m_Mobile = m;
                Priority = TimerPriority.TwoFiftyMS;
				m_BloodDrinker = blooddrinker;
                Splintering = splintering;

                m_MaxCount = Spells.SkillMasteries.BardSpell.GetSpellForParty(m, typeof(Spells.SkillMasteries.ResilienceSpell)) != null ? 3 : 5;
			}

            protected override void OnTick()
            {
                if (!m_Mobile.Alive || m_Mobile.Deleted)
                {
                    EndBleed(m_Mobile, true);
                }
                else
                {
                    int damage = 0;

                    if (!Server.Spells.SkillMasteries.WhiteTigerFormSpell.HasBleedMod(m_From, out damage))
                        damage = Math.Max(1, Utility.RandomMinMax(1 + (m_MaxCount - m_Count), 10 - m_Count));

                    DoBleed(m_Mobile, m_From, damage, m_BloodDrinker);

                    if (++m_Count >= m_MaxCount)
                        EndBleed(m_Mobile, true);
                }
            }
        }
    }
}
