using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Spells.SkillMasteries;

namespace Server.Spells.Necromancy
{
    public class PainSpikeSpell : NecromancerSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Pain Spike", "In Sar",
            203,
            9031,
            Reagent.GraveDust,
            Reagent.PigIron);

        private static readonly Dictionary<Mobile, InternalTimer> m_Table = new Dictionary<Mobile, InternalTimer>();

        public PainSpikeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.0);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 20.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 5;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return false;
            }
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

                ApplyEffects(m);
                ConduitSpell.CheckAffected(Caster, m, ApplyEffects);
            }

            FinishSequence();
        }

        public void ApplyEffects(Mobile m, double strength = 1.0)
        {
            //SpellHelper.CheckReflect( (int)Circle, Caster, ref m ); //Irrelevent asfter AoS

            /* Temporarily causes intense physical pain to the target, dealing direct damage.
             * After 10 seconds the spell wears off, and if the target is still alive, 
             * some of the Hit Points lost through Pain Spike are restored.
             */

            m.FixedParticles(0x37C4, 1, 8, 9916, 39, 3, EffectLayer.Head);
            m.FixedParticles(0x37C4, 1, 8, 9502, 39, 4, EffectLayer.Head);
            m.PlaySound(0x210);

            double damage = (((GetDamageSkill(Caster) - GetResistSkill(m)) / 10) + (m.Player ? 18 : 30)) * strength;
            m.CheckSkill(SkillName.MagicResist, 0.0, 120.0);	//Skill check for gain

            if (damage < 1)
                damage = 1;

            TimeSpan buffTime = TimeSpan.FromSeconds(10.0 * strength);
            InternalTimer t;

            if (m_Table.ContainsKey(m))
            {
                damage = Utility.RandomMinMax(3, 7);
                t = m_Table[m];

                if (t != null)
                {
                    t.Expires += TimeSpan.FromSeconds(2);
                }
            }
            else
            {
                t = new InternalTimer(m, damage);
                t.Start();
            }

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.PainSpike, 1075667, t.Expires - DateTime.UtcNow, m, Convert.ToString((int)damage)));

            Misc.WeightOverloading.DFA = Misc.DFAlgorithm.PainSpike;
            AOS.Damage(m, Caster, (int)damage, 0, 0, 0, 0, 0, 0, 100);
            AOS.DoLeech((int)damage, Caster, m);
            Misc.WeightOverloading.DFA = Misc.DFAlgorithm.Standard;

            HarmfulSpell(m);
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly int m_ToRestore;

            public DateTime Expires { get; set; }

            public InternalTimer(Mobile m, double toRestore)
                : base(TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250))
            {
                Priority = TimerPriority.FiftyMS;

                m_Mobile = m;
                m_ToRestore = (int)toRestore;

                Expires = DateTime.UtcNow + TimeSpan.FromSeconds(10);

                m_Table[m] = this;
            }

            protected override void OnTick()
            {
                if (DateTime.UtcNow >= Expires)
                {
                    if (m_Table.ContainsKey(m_Mobile))
                        m_Table.Remove(m_Mobile);

                    if (m_Mobile.Alive && !m_Mobile.IsDeadBondedPet)
                        m_Mobile.Hits += m_ToRestore;

                    BuffInfo.RemoveBuff(m_Mobile, BuffIcon.PainSpike);
                    
                    Stop();
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly PainSpikeSpell m_Owner;
            public InternalTarget(PainSpikeSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
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
