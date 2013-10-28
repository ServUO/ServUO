using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Sixth
{
    public class InvisibilitySpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Invisibility", "An Lor Xen",
            206,
            9002,
            Reagent.Bloodmoss,
            Reagent.Nightshade);
        private static readonly Hashtable m_Table = new Hashtable();
        public InvisibilitySpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Sixth;
            }
        }
        public static bool HasTimer(Mobile m)
        {
            return m_Table[m] != null;
        }

        public static void RemoveTimer(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
            {
                t.Stop();
                m_Table.Remove(m);
            }
        }

        public override bool CheckCast()
        {
            if (Engines.ConPVP.DuelContext.CheckSuddenDeath(this.Caster))
            {
                this.Caster.SendMessage(0x22, "You cannot cast this spell when in sudden death.");
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (m is Mobiles.BaseVendor || m is Mobiles.PlayerVendor || m is Mobiles.PlayerBarkeeper || m.AccessLevel > this.Caster.AccessLevel)
            {
                this.Caster.SendLocalizedMessage(501857); // This spell won't work on that!
            }
            else if (this.CheckBSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                Effects.SendLocationParticles(EffectItem.Create(new Point3D(m.X, m.Y, m.Z + 16), this.Caster.Map, EffectItem.DefaultDuration), 0x376A, 10, 15, 5045);
                m.PlaySound(0x3C4);

                m.Hidden = true;
                m.Combatant = null;
                m.Warmode = false;

                RemoveTimer(m);

                TimeSpan duration = TimeSpan.FromSeconds(((1.2 * this.Caster.Skills.Magery.Fixed) / 10));

                Timer t = new InternalTimer(m, duration);

                BuffInfo.RemoveBuff(m, BuffIcon.HidingAndOrStealth);
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Invisibility, 1075825, duration, m));	//Invisibility/Invisible

                m_Table[m] = t;

                t.Start();
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly InvisibilitySpell m_Owner;
            public InternalTarget(InvisibilitySpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Beneficial)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    this.m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public InternalTimer(Mobile m, TimeSpan duration)
                : base(duration)
            {
                this.Priority = TimerPriority.OneSecond;
                this.m_Mobile = m;
            }

            protected override void OnTick()
            {
                this.m_Mobile.RevealingAction();
                RemoveTimer(this.m_Mobile);
            }
        }
    }
}