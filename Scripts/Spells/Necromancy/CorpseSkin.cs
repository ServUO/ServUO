using Server.Spells.SkillMasteries;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Spells.Necromancy
{
    public class CorpseSkinSpell : NecromancerSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Corpse Skin", "In Agle Corp Ylem",
            203,
            9051,
            Reagent.BatWing,
            Reagent.GraveDust);

        private static readonly Dictionary<Mobile, ExpireTimer> m_Table = new Dictionary<Mobile, ExpireTimer>();

        public CorpseSkinSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(1.75);
        public override double RequiredSkill => 20.0;
        public override int RequiredMana => 11;
        public static bool RemoveCurse(Mobile m)
        {
            if (m_Table.ContainsKey(m))
            {
                m_Table[m].DoExpire();
                return true;
            }

            return false;
        }

        public static bool IsUnderEffects(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        public static int GetResistMalus(Mobile m)
        {
            if (m_Table.ContainsKey(m))
            {
                return 70 - m_Table[m].Malus;
            }

            return 70;
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (CheckHSequence(m))
            {
                SpellHelper.Turn(Caster, m);

                SpellHelper.CheckReflect(this, Caster, ref m);

                ApplyEffects(m);
                ConduitSpell.CheckAffected(Caster, m, ApplyEffects);
            }

            FinishSequence();
        }

        public void ApplyEffects(Mobile m, double strength = 1.0)
        {
            /* Transmogrifies the flesh of the target creature or player to resemble rotted corpse flesh,
                * making them more vulnerable to Fire and Poison damage,
                * but increasing their resistance to Physical and Cold damage.
                * 
                * The effect lasts for ((Spirit Speak skill level - target's Resist Magic skill level) / 25 ) + 40 seconds.
                * 
                * NOTE: Algorithm above is fixed point, should be:
                * ((ss-mr)/2.5) + 40
                * 
                * NOTE: Resistance is not checked if targeting yourself
                */

            if (m_Table.ContainsKey(m))
            {
                m_Table[m].DoExpire(false);
            }

            m.SendLocalizedMessage(1061689); // Your skin turns dry and corpselike.

            if (m.Spell != null)
                m.Spell.OnCasterHurt();

            m.FixedParticles(0x373A, 1, 15, 9913, 67, 7, EffectLayer.Head);
            m.PlaySound(0x1BB);

            double ss = GetDamageSkill(Caster);
            double mr = GetResistSkill(m);
            m.CheckSkill(SkillName.MagicResist, 0.0, m.Skills[SkillName.MagicResist].Cap);	//Skill check for gain

            TimeSpan duration = TimeSpan.FromSeconds((((ss - mr) / 2.5) + 40.0) * strength);

            int malus = (int)Math.Min(15, (Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) * 0.075);

            ResistanceMod[] mods = new ResistanceMod[4]
                    {
                        new ResistanceMod( ResistanceType.Fire, (int)(-malus * strength) ),
                        new ResistanceMod( ResistanceType.Poison, (int)(-malus * strength) ),
                        new ResistanceMod( ResistanceType.Cold, (int)(+10.0 * strength) ),
                        new ResistanceMod( ResistanceType.Physical, (int)(+10.0 * strength) )
                    };

            ExpireTimer timer = new ExpireTimer(m, mods, malus, duration);
            timer.Start();

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.CorpseSkin, 1075663, duration, m));

            m_Table[m] = timer;

            m.UpdateResistances();

            for (int i = 0; i < mods.Length; ++i)
                m.AddResistanceMod(mods[i]);

            HarmfulSpell(m);
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly ResistanceMod[] m_Mods;
            private readonly int m_Malus;

            public int Malus => m_Malus;

            public ExpireTimer(Mobile m, ResistanceMod[] mods, int malus, TimeSpan delay)
                : base(delay)
            {
                m_Mobile = m;
                m_Mods = mods;
                m_Malus = malus;
            }

            public void DoExpire(bool message = true)
            {
                for (int i = 0; i < m_Mods.Length; ++i)
                    m_Mobile.RemoveResistanceMod(m_Mods[i]);

                Stop();
                BuffInfo.RemoveBuff(m_Mobile, BuffIcon.CorpseSkin);

                if (m_Table.ContainsKey(m_Mobile))
                    m_Table.Remove(m_Mobile);

                m_Mobile.UpdateResistances();

                if (message)
                    m_Mobile.SendLocalizedMessage(1061688); // Your skin returns to normal.
            }

            protected override void OnTick()
            {
                DoExpire();
            }
        }

        private class InternalTarget : Target
        {
            private readonly CorpseSkinSpell m_Owner;
            public InternalTarget(CorpseSkinSpell owner)
                : base(10, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
