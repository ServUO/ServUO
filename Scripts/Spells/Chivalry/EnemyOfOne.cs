using System;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Spells.Chivalry
{
    public class EnemyOfOneSpell : PaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Enemy of One", "Forul Solum",
            -1,
            9002);

        public EnemyOfOneSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(0.5); } }

        public override double RequiredSkill { get { return 45.0; } }
        public override int RequiredMana { get { return 20; } }
        public override int RequiredTithing { get { return 10; } }
        public override int MantraNumber { get { return 1060723; } } // Forul Solum
        public override bool BlocksMovement { get { return false; } }

        public override TimeSpan GetCastDelay()
        {
            TimeSpan delay = base.GetCastDelay();

            if (Core.SA && UnderEffect(Caster))
            {
                double milliseconds = delay.TotalMilliseconds / 2;

                delay = TimeSpan.FromMilliseconds(milliseconds);
            }

            return delay;
        }

        public override void OnCast()
        {
            if (Core.SA && UnderEffect(Caster))
            {
                PlayEffects();

                // As per Pub 71, Enemy of one has now been changed to a Spell Toggle. You can remove the effect
                // before the duration expires by recasting the spell.
                RemoveEffect(Caster);
            }
            else if (CheckSequence())
            {
                PlayEffects();

                // TODO: validate formula
                var seconds = ComputePowerValue(1);
                Utility.FixMinMax(ref seconds, 67, 228);

                var delay = TimeSpan.FromSeconds(seconds);

                var timer = Timer.DelayCall(delay, RemoveEffect, Caster);

                var expire = DateTime.UtcNow + delay;

                var context = new EnemyOfOneContext(Caster, timer, expire);
                context.OnCast();
                m_Table[Caster] = context;
            }

            FinishSequence();
        }

        private void PlayEffects()
        {
            Caster.PlaySound(0x0F5);
            Caster.PlaySound(0x1ED);

            Caster.FixedParticles(0x375A, 1, 30, 9966, 33, 2, EffectLayer.Head);
            Caster.FixedParticles(0x37B9, 1, 30, 9502, 43, 3, EffectLayer.Head);
        }

        private static readonly Dictionary<Mobile, EnemyOfOneContext> m_Table = new Dictionary<Mobile, EnemyOfOneContext>();

        public static EnemyOfOneContext GetContext(Mobile m)
        {
            if (!m_Table.ContainsKey(m))
                return null;

            return m_Table[m];
        }

        private static bool UnderEffect(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        private static void RemoveEffect(Mobile m)
        {
            if (m_Table.ContainsKey(m))
            {
                var context = m_Table[m];

                m_Table.Remove(m);

                context.OnRemoved();

                m.PlaySound(0x1F8);
            }
        }

        public static Dictionary<Type, string> NameCache { get; set; }

        public static void Configure()
        {
            if (NameCache == null)
                NameCache = new Dictionary<Type, string>();

            NameCache[typeof(PlayerMobile)] = "players";

            EventSink.MobileCreated += CacheName;
        }

        public static string GetTypeName(Mobile defender)
        {
            Type t = defender.GetType();

            if (NameCache.ContainsKey(t))
            {
                return NameCache[t];
            }

            return AddNameToCache(t);
        }

        public static void CacheName(MobileCreatedEventArgs e)
        {
            if(NameCache.ContainsKey(e.Mobile.GetType()))
                return;

            BaseCreature bc = e.Mobile as BaseCreature;

            if (bc != null && !bc.Blessed && !bc.IsInvulnerable && (bc.Owners == null || bc.Owners.Count == 0))
            {
                AddNameToCache(bc.GetType());
            }
        }

        public static string AddNameToCache(Type t)
        {
            string name = t.Name;

            if (name != null)
            {
                for (int i = 0; i < name.Length; i++)
                {
                    if (i > 0 && Char.IsUpper(name[i]))
                    {
                        name = name.Insert(i, " ");
                        i++;
                    }
                }

                if (name.EndsWith("y"))
                {
                    name = name.Substring(0, name.Length - 1);
                    name = name + "ies";
                }
                else if (!name.EndsWith("s"))
                {
                    name = name + "s";
                }


                NameCache[t] = name.ToLower();
            }

            return name;
        }
    }

	public class EnemyOfOneContext
	{
		private Mobile m_Owner;
		private Timer m_Timer;
		private DateTime m_Expire;
		private Type m_TargetType;
		private int m_DamageScalar;
        private string m_TypeName;

		public Mobile Owner { get { return m_Owner; } }
		public Timer Timer { get { return m_Timer; } }
		public Type TargetType { get { return m_TargetType; } }
		public int DamageScalar { get { return m_DamageScalar; } }
        public string TypeName { get { return m_TypeName; } }

		public EnemyOfOneContext(Mobile owner, Timer timer, DateTime expire)
		{
			m_Owner = owner;
			m_Timer = timer;
			m_Expire = expire;
			m_TargetType = null;
			m_DamageScalar = 50;
		}

		public bool IsWaitingForEnemy { get { return m_TargetType != null; } }

		public bool IsEnemy(Mobile m)
		{
			return m_TargetType == m.GetType();
		}

		public void OnCast()
		{
			UpdateBuffInfo();
		}

		private void UpdateBuffInfo()
		{
			// TODO: display friendly name attribute when target is not null.
            if (m_TargetType == null || TypeName == null)
            {
                BuffInfo.AddBuff(m_Owner, new BuffInfo(BuffIcon.EnemyOfOne, 1075653, 1075902, m_Expire - DateTime.UtcNow, m_Owner, string.Format("{0}\t{1}", m_DamageScalar, "100")));
            }
            else
            {
                BuffInfo.AddBuff(m_Owner, new BuffInfo(BuffIcon.EnemyOfOne, 1075653, 1075654, m_Expire - DateTime.UtcNow, m_Owner, string.Format("{0}\t{1}\t{2}\t{3}", m_DamageScalar, TypeName, ".", "100")));
                //+~1_PERCENT~% damage to ~2_TEMPLATES~~3_EXTRA~<br>+~4_PERCENT~% damage to you from anything except ~2_TEMPLATES~~3_EXTRA~
            }
		}

		public void OnHit(Mobile defender)
		{
			if (m_TargetType == null)
			{
				m_TargetType = defender.GetType();
                m_TypeName = EnemyOfOneSpell.GetTypeName(defender);

				if (Core.SA)
				{
					// Odd but OSI recalculates when the target changes...
					var chivalry = (int)m_Owner.Skills.Chivalry.Value;
					m_DamageScalar = 10 + ((chivalry - 40) * 9) / 10;
				}

				DeltaEnemies();
				UpdateBuffInfo();
			}
		}

		public void OnRemoved()
		{
			if (m_Timer != null)
				m_Timer.Stop();

			DeltaEnemies();

			BuffInfo.RemoveBuff(m_Owner, BuffIcon.EnemyOfOne);
		}

		private void DeltaEnemies()
		{
			foreach (var m in m_Owner.GetMobilesInRange(18))
			{
				if (m.GetType() == m_TargetType)
					m.Delta(MobileDelta.Noto);
			}
		}
	}
}
