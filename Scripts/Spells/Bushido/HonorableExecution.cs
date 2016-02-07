using System;
using System.Collections;

namespace Server.Spells.Bushido
{
    public class HonorableExecution : SamuraiMove
    {
        private static readonly Hashtable m_Table = new Hashtable();
        public HonorableExecution()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 0;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 25.0;
            }
        }
        public override TextDefinition AbilityMessage
        {
            get
            {
                return new TextDefinition(1063122);
            }
        }// You better kill your enemy with your next hit or you'll be rather sorry...
        public static int GetSwingBonus(Mobile target)
        {
            HonorableExecutionInfo info = m_Table[target] as HonorableExecutionInfo;

            if (info == null)
                return 0;

            return info.m_SwingBonus;
        }

        public static bool IsUnderPenalty(Mobile target)
        {
            HonorableExecutionInfo info = m_Table[target] as HonorableExecutionInfo;

            if (info == null)
                return false;

            return info.m_Penalty;
        }

        public static void RemovePenalty(Mobile target)
        {
            HonorableExecutionInfo info = m_Table[target] as HonorableExecutionInfo;

            if (info == null || !info.m_Penalty)
                return;

            info.Clear();

            if (info.m_Timer != null)
                info.m_Timer.Stop();

            m_Table.Remove(target);
        }

        public override double GetDamageScalar(Mobile attacker, Mobile defender)
        {
            double bushido = attacker.Skills[SkillName.Bushido].Value;

            // TODO: 20 -> Perfection
            return 1.0 + (bushido * 20) / 10000;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            ClearCurrentMove(attacker);

            HonorableExecutionInfo info = m_Table[attacker] as HonorableExecutionInfo;

            if (info != null)
            {
                info.Clear();

                if (info.m_Timer != null)
                    info.m_Timer.Stop();
            }

            if (!defender.Alive)
            {
                attacker.FixedParticles(0x373A, 1, 17, 0x7E2, EffectLayer.Waist);

                double bushido = attacker.Skills[SkillName.Bushido].Value;

                attacker.Hits += 20 + (int)((bushido * bushido) / 480.0);

                int swingBonus = Math.Max(1, (int)((bushido * bushido) / 720.0));

                info = new HonorableExecutionInfo(attacker, swingBonus);
                info.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(20.0), new TimerStateCallback(EndEffect), info);

                m_Table[attacker] = info;
            }
            else
            {
                ArrayList mods = new ArrayList();

                mods.Add(new ResistanceMod(ResistanceType.Physical, -40));
                mods.Add(new ResistanceMod(ResistanceType.Fire, -40));
                mods.Add(new ResistanceMod(ResistanceType.Cold, -40));
                mods.Add(new ResistanceMod(ResistanceType.Poison, -40));
                mods.Add(new ResistanceMod(ResistanceType.Energy, -40));

                double resSpells = attacker.Skills[SkillName.MagicResist].Value;

                if (resSpells > 0.0)
                    mods.Add(new DefaultSkillMod(SkillName.MagicResist, true, -resSpells));

                info = new HonorableExecutionInfo(attacker, mods);
                info.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(7.0), new TimerStateCallback(EndEffect), info);

                m_Table[attacker] = info;
            }

            this.CheckGain(attacker);
        }

        public void EndEffect(object state)
        {
            HonorableExecutionInfo info = (HonorableExecutionInfo)state;

            RemovePenalty(info.m_Mobile);
        }

        private class HonorableExecutionInfo
        {
            public readonly Mobile m_Mobile;
            public readonly int m_SwingBonus;
            public readonly ArrayList m_Mods;
            public readonly bool m_Penalty;
            public Timer m_Timer;
            public HonorableExecutionInfo(Mobile from, int swingBonus)
                : this(from, swingBonus, null, false)
            {
            }

            public HonorableExecutionInfo(Mobile from, ArrayList mods)
                : this(from, 0, mods, true)
            {
            }

            public HonorableExecutionInfo(Mobile from, int swingBonus, ArrayList mods, bool penalty)
            {
                this.m_Mobile = from;
                this.m_SwingBonus = swingBonus;
                this.m_Mods = mods;
                this.m_Penalty = penalty;

                this.Apply();
            }

            public void Apply()
            {
                if (this.m_Mods == null)
                    return;

                for (int i = 0; i < this.m_Mods.Count; ++i)
                {
                    object mod = this.m_Mods[i];

                    if (mod is ResistanceMod)
                        this.m_Mobile.AddResistanceMod((ResistanceMod)mod);
                    else if (mod is SkillMod)
                        this.m_Mobile.AddSkillMod((SkillMod)mod);
                }
            }

            public void Clear()
            {
                if (this.m_Mods == null)
                    return;

                for (int i = 0; i < this.m_Mods.Count; ++i)
                {
                    object mod = this.m_Mods[i];

                    if (mod is ResistanceMod)
                        this.m_Mobile.RemoveResistanceMod((ResistanceMod)mod);
                    else if (mod is SkillMod)
                        this.m_Mobile.RemoveSkillMod((SkillMod)mod);
                }
            }
        }
    }
}