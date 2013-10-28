using System;
using System.Collections;
using Server.Targeting;

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
        private static readonly Hashtable m_Table = new Hashtable();
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
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (this.CheckHSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                //SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m ); //Irrelevent asfter AoS

                /* Temporarily causes intense physical pain to the target, dealing direct damage.
                * After 10 seconds the spell wears off, and if the target is still alive, 
                * some of the Hit Points lost through Pain Spike are restored.
                */

                m.FixedParticles(0x37C4, 1, 8, 9916, 39, 3, EffectLayer.Head);
                m.FixedParticles(0x37C4, 1, 8, 9502, 39, 4, EffectLayer.Head);
                m.PlaySound(0x210);

                double damage = ((this.GetDamageSkill(this.Caster) - this.GetResistSkill(m)) / 10) + (m.Player ? 18 : 30);
                m.CheckSkill(SkillName.MagicResist, 0.0, 120.0);	//Skill check for gain

                if (damage < 1)
                    damage = 1;

                TimeSpan buffTime = TimeSpan.FromSeconds(10.0);

                if (m_Table.Contains(m))
                {
                    damage = Utility.RandomMinMax(3, 7);
                    Timer t = m_Table[m] as Timer;

                    if (t != null)
                    {
                        t.Delay += TimeSpan.FromSeconds(2.0);

                        buffTime = t.Next - DateTime.UtcNow;
                    }
                }
                else
                {
                    new InternalTimer(m, damage).Start();
                }

                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.PainSpike, 1075667, buffTime, m, Convert.ToString((int)damage)));

                Misc.WeightOverloading.DFA = Misc.DFAlgorithm.PainSpike;
                m.Damage((int)damage, this.Caster);
                SpellHelper.DoLeech((int)damage, this.Caster, m);
                Misc.WeightOverloading.DFA = Misc.DFAlgorithm.Standard;

                //SpellHelper.Damage( this, m, damage, 100, 0, 0, 0, 0, Misc.DFAlgorithm.PainSpike );
                this.HarmfulSpell(m);
            }

            this.FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly int m_ToRestore;
            public InternalTimer(Mobile m, double toRestore)
                : base(TimeSpan.FromSeconds(10.0))
            {
                this.Priority = TimerPriority.OneSecond;

                this.m_Mobile = m;
                this.m_ToRestore = (int)toRestore;

                m_Table[m] = this;
            }

            protected override void OnTick()
            {
                m_Table.Remove(this.m_Mobile);

                if (this.m_Mobile.Alive && !this.m_Mobile.IsDeadBondedPet)
                    this.m_Mobile.Hits += this.m_ToRestore;

                BuffInfo.RemoveBuff(this.m_Mobile, BuffIcon.PainSpike);
            }
        }

        private class InternalTarget : Target
        {
            private readonly PainSpikeSpell m_Owner;
            public InternalTarget(PainSpikeSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    this.m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}