using Server.Mobiles;
using System;
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

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(0.5);

        public override double RequiredSkill => 45.0;
        public override int RequiredMana => 20;
        public override int RequiredTithing => 10;
        public override int MantraNumber => 1060723;  // Forul Solum
        public override bool BlocksMovement => false;

        public override TimeSpan GetCastDelay()
        {
            TimeSpan delay = base.GetCastDelay();

            if (UnderEffect(Caster))
            {
                double milliseconds = delay.TotalMilliseconds / 2;

                delay = TimeSpan.FromMilliseconds(milliseconds);
            }

            return delay;
        }

        public override void OnCast()
        {
            if (UnderEffect(Caster))
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
                int seconds = ComputePowerValue(1);
                Utility.FixMinMax(ref seconds, 67, 228);

                TimeSpan delay = TimeSpan.FromSeconds(seconds);

                Timer timer = Timer.DelayCall(delay, RemoveEffect, Caster);

                DateTime expire = DateTime.UtcNow + delay;

                EnemyOfOneContext context = new EnemyOfOneContext(Caster, timer, expire);
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

        public static bool UnderEffect(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        public static void RemoveEffect(Mobile m)
        {
            if (m_Table.ContainsKey(m))
            {
                EnemyOfOneContext context = m_Table[m];

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
        }

        public static string GetTypeName(Mobile defender)
        {
            if (defender is PlayerMobile || (defender is BaseCreature && ((BaseCreature)defender).GetMaster() is PlayerMobile))
            {
                return defender.Name;
            }

            Type t = defender.GetType();

            if (NameCache.ContainsKey(t))
            {
                return NameCache[t];
            }

            return AddNameToCache(t);
        }

        public static string AddNameToCache(Type t)
        {
            string name = t.Name;

            if (name != null)
            {
                for (int i = 0; i < name.Length; i++)
                {
                    if (i > 0 && char.IsUpper(name[i]))
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
        private readonly Mobile m_Owner;
        private Timer m_Timer;
        private DateTime m_Expire;
        private Type m_TargetType;
        private int m_DamageScalar;
        private string m_TypeName;

        private Mobile m_PlayerOrPet;

        public Mobile Owner => m_Owner;
        public Timer Timer => m_Timer;
        public Type TargetType => m_TargetType;
        public int DamageScalar => m_DamageScalar;
        public string TypeName => m_TypeName;

        public EnemyOfOneContext(Mobile owner, Timer timer, DateTime expire)
        {
            m_Owner = owner;
            m_Timer = timer;
            m_Expire = expire;
            m_TargetType = null;
            m_DamageScalar = 50;
        }

        public bool IsWaitingForEnemy => m_TargetType == null;

        public bool IsEnemy(Mobile m)
        {
            if (m is BaseCreature && ((BaseCreature)m).GetMaster() == Owner)
            {
                return false;
            }

            if (m_PlayerOrPet != null)
            {
                if (m_PlayerOrPet == m)
                {
                    return true;
                }
            }
            else if (m_TargetType == m.GetType())
            {
                return true;
            }

            return false;
        }

        public void OnCast()
        {
            UpdateBuffInfo();
        }

        private void UpdateDamage()
        {
            int chivalry = (int)m_Owner.Skills.Chivalry.Value;
            m_DamageScalar = 10 + ((chivalry - 40) * 9) / 10;

            if (m_PlayerOrPet != null)
                m_DamageScalar /= 2;
        }

        private void UpdateBuffInfo()
        {
            if (m_TypeName == null)
            {
                BuffInfo.AddBuff(m_Owner, new BuffInfo(BuffIcon.EnemyOfOne, 1075653, 1075902, m_Expire - DateTime.UtcNow, m_Owner, string.Format("{0}\t{1}", m_DamageScalar, "100"), true));
            }
            else
            {
                BuffInfo.AddBuff(m_Owner, new BuffInfo(BuffIcon.EnemyOfOne, 1075653, 1075654, m_Expire - DateTime.UtcNow, m_Owner, string.Format("{0}\t{1}\t{2}\t{3}", m_DamageScalar, TypeName, ".", "100"), true));
            }
        }

        public void OnHit(Mobile defender)
        {
            if (m_TargetType == null)
            {
                m_TypeName = EnemyOfOneSpell.GetTypeName(defender);

                if (defender is PlayerMobile || (defender is BaseCreature && ((BaseCreature)defender).GetMaster() is PlayerMobile))
                {
                    m_PlayerOrPet = defender;
                    TimeSpan duration = TimeSpan.FromSeconds(8);

                    if (DateTime.UtcNow + duration < m_Expire)
                    {
                        m_Expire = DateTime.UtcNow + duration;
                    }

                    if (m_Timer != null)
                    {
                        m_Timer.Stop();
                        m_Timer = null;
                    }

                    m_Timer = Timer.DelayCall(duration, EnemyOfOneSpell.RemoveEffect, m_Owner);
                }
                else
                {
                    m_TargetType = defender.GetType();
                }

                UpdateDamage();
                DeltaEnemies();
                UpdateBuffInfo();
            }
            else
            {
                // Odd but OSI recalculates when the target changes...
                UpdateDamage();
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
            IPooledEnumerable eable = m_Owner.GetMobilesInRange(18);

            foreach (Mobile m in eable)
            {
                if (m_PlayerOrPet != null)
                {
                    if (m == m_PlayerOrPet)
                    {
                        m.Delta(MobileDelta.Noto);
                    }
                }
                else if (m.GetType() == m_TargetType)
                {
                    m.Delta(MobileDelta.Noto);
                }
            }

            eable.Free();
        }
    }
}
