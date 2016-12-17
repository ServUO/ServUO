using System;
using System.Collections;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Necromancy
{
    public class BloodOathSpell : NecromancerSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Blood Oath", "In Jux Mani Xen",
            203,
            9031,
            Reagent.DaemonBlood);
        private static readonly Hashtable m_OathTable = new Hashtable();
        private static readonly Hashtable m_Table = new Hashtable();
        public BloodOathSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.5);
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
                return 13;
            }
        }
        public static bool RemoveCurse(Mobile m)
        {
            ExpireTimer t = (ExpireTimer)m_Table[m];

            if (t == null)
                return false;

            t.DoExpire();
            return true;
        }

        public static Mobile GetBloodOath(Mobile m)
        {
            if (m == null)
                return null;

            Mobile oath = (Mobile)m_OathTable[m];

            if (oath == m)
                oath = null;

            return oath;
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (this.Caster == m)
            {
                this.Caster.SendLocalizedMessage(1060508); // You can't curse that.
            }
            else if (m_OathTable.Contains(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061607); // You are already bonded in a Blood Oath.
            }
            else if (m_OathTable.Contains(m))
            {
                if (m.Player)
                    this.Caster.SendLocalizedMessage(1061608); // That player is already bonded in a Blood Oath.
                else
                    this.Caster.SendLocalizedMessage(1061609); // That creature is already bonded in a Blood Oath.
            }
            else if (this.CheckHSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                /* Temporarily creates a dark pact between the caster and the target.
                * Any damage dealt by the target to the caster is increased, but the target receives the same amount of damage.
                * The effect lasts for ((Spirit Speak skill level - target's Resist Magic skill level) / 80 ) + 8 seconds.
                * 
                * NOTE: The above algorithm must be fixed point, it should be:
                * ((ss-rm)/8)+8
                */

                ExpireTimer timer = (ExpireTimer)m_Table[m];
                if (timer != null)
                    timer.DoExpire();

                m_OathTable[this.Caster] = this.Caster;
                m_OathTable[m] = this.Caster;

                if (m.Spell != null)
                    m.Spell.OnCasterHurt();
				
                this.Caster.PlaySound(0x175);

                this.Caster.FixedParticles(0x375A, 1, 17, 9919, 33, 7, EffectLayer.Waist);
                this.Caster.FixedParticles(0x3728, 1, 13, 9502, 33, 7, (EffectLayer)255);

                m.FixedParticles(0x375A, 1, 17, 9919, 33, 7, EffectLayer.Waist);
                m.FixedParticles(0x3728, 1, 13, 9502, 33, 7, (EffectLayer)255);

                TimeSpan duration = TimeSpan.FromSeconds(((this.GetDamageSkill(this.Caster) - this.GetResistSkill(m)) / 8) + 8);
                m.CheckSkill(SkillName.MagicResist, 0.0, 120.0);	//Skill check for gain

                timer = new ExpireTimer(this.Caster, m, duration);
                timer.Start();

                BuffInfo.AddBuff(this.Caster, new BuffInfo(BuffIcon.BloodOathCaster, 1075659, duration, this.Caster, m.Name.ToString()));
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.BloodOathCurse, 1075661, duration, m, this.Caster.Name.ToString()));

                m_Table[m] = timer;
                this.HarmfulSpell(m);
            }

            this.FinishSequence();
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Caster;
            private readonly Mobile m_Target;
            private readonly DateTime m_End;
            public ExpireTimer(Mobile caster, Mobile target, TimeSpan delay)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                this.m_Caster = caster;
                this.m_Target = target;
                this.m_End = DateTime.UtcNow + delay;

                this.Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                if (m_OathTable.Contains(this.m_Caster))
                {
                    this.m_Caster.SendLocalizedMessage(1061620); // Your Blood Oath has been broken.
                    m_OathTable.Remove(this.m_Caster);
                }

                if (m_OathTable.Contains(this.m_Target))
                {
                    this.m_Target.SendLocalizedMessage(1061620); // Your Blood Oath has been broken.
                    m_OathTable.Remove(this.m_Target);
                }

                this.Stop();

                BuffInfo.RemoveBuff(this.m_Caster, BuffIcon.BloodOathCaster);
                BuffInfo.RemoveBuff(this.m_Target, BuffIcon.BloodOathCurse);

                m_Table.Remove(this.m_Caster);
            }

            protected override void OnTick()
            {
                if (this.m_Caster.Deleted || this.m_Target.Deleted || !this.m_Caster.Alive || !this.m_Target.Alive || DateTime.UtcNow >= this.m_End)
                {
                    this.DoExpire();
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly BloodOathSpell m_Owner;
            public InternalTarget(BloodOathSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    this.m_Owner.Target((Mobile)o);
                else
                    from.SendLocalizedMessage(1060508); // You can't curse that.
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}