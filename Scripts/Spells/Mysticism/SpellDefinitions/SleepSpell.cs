using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;
using System.Collections.Generic;
using Server.Network;

namespace Server.Spells.Mystic
{
	public class SleepSpell : MysticSpell
	{
        public override SpellCircle Circle { get { return SpellCircle.Third; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Sleep", "In Zu",
				230,
				9022,
				Reagent.Bloodmoss,
				Reagent.Garlic,
				Reagent.SulfurousAsh,
				Reagent.DragonBlood
			);

		public SleepSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new MysticSpellTarget( this, TargetFlags.Harmful );
		}

		public override void OnTarget( Object o )
		{
			Mobile target = o as Mobile;

			if ( target == null )
			{
				return;
			}
            else if (target.Paralyzed)
            {
                Caster.SendLocalizedMessage(1080134); //Your target is already immobilized and cannot be slept.
            }
            else if (m_ImmunityList.Contains(target))
            {
                Caster.SendLocalizedMessage(1080135); //Your target cannot be put to sleep.
            }
            else if (CheckHSequence(target))
            {
                double duration = ((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 20) + 3;
                duration -= target.Skills[SkillName.MagicResist].Value / 10;

                if (duration <= 0 || Server.Spells.Mystic.StoneFormSpell.CheckImmunity(target))
                {
                    Caster.SendLocalizedMessage(1080136); //Your target resists sleep.
                    target.SendLocalizedMessage(1080137); //You resist sleep.
                }
                else
                    DoSleep(Caster, target, TimeSpan.FromSeconds(duration));
            }

			FinishSequence();
		}

        private static Dictionary<Mobile, SleepTimer> m_Table = new Dictionary<Mobile, SleepTimer>();
        private static List<Mobile> m_ImmunityList = new List<Mobile>();

        public static void DoSleep(Mobile caster, Mobile target, TimeSpan duration)
        {
            target.Combatant = null;
            target.Send(SpeedControl.WalkSpeed);

            caster.PlaySound(0x657);
            target.FixedParticles(0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255);
            target.FixedParticles(0x376A, 1, 15, 9502, 97, 3, (EffectLayer)255);

            
            BuffInfo.AddBuff(target, new BuffInfo(BuffIcon.Sleep, 1080139));

            if (m_Table.ContainsKey(target))
                m_Table[target].Stop();

            m_Table[target] = new SleepTimer(target, duration);

            target.Delta(MobileDelta.WeaponDamage);
        }

        public static void AddToSleepTable(Mobile from, TimeSpan duration)
        {
            m_Table.Add(from, new SleepTimer(from, duration));
        }

        public static bool IsUnderSleepEffects(Mobile from)
        {
            return m_Table.ContainsKey(from);
        }

        public static void OnDamage(Mobile from)
        {
            if (m_Table.ContainsKey(from))
                EndSleep(from);
        }

        public class SleepTimer : Timer
        {
            private Mobile m_Target;

            public  SleepTimer(Mobile target, TimeSpan duration) : base(duration)
            {
                m_Target = target;
                this.Start();
            }

            protected override void OnTick()
            {
                EndSleep(m_Target);
                this.Stop();
            }
        }

        public static void EndSleep(Mobile target)
        {
            if (m_Table.ContainsKey(target))
            {
                target.Send(SpeedControl.Disable);

                m_Table[target].Stop();
                m_Table.Remove(target);

                BuffInfo.RemoveBuff(target, BuffIcon.Sleep);

                double immduration = target.Skills[SkillName.MagicResist].Value / 10;

                m_ImmunityList.Add(target);
                Timer.DelayCall(TimeSpan.FromSeconds(immduration), new TimerStateCallback(RemoveImmunity_Callback), target);

                target.Delta(MobileDelta.WeaponDamage);
            }
        }

        public static void RemoveImmunity_Callback(object state)
        {
            Mobile m = (Mobile)state;

            if (m_ImmunityList.Contains(m))
                m_ImmunityList.Remove(m);
        }
	}
}