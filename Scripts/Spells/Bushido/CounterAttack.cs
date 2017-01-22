using System;
using System.Collections;
using Server.Items;

namespace Server.Spells.Bushido
{
    public class CounterAttack : SamuraiSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "CounterAttack", null,
            -1,
            9002);
        private static readonly Hashtable m_Table = new Hashtable();
        public CounterAttack(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(0.25);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 40.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 5;
            }
        }
        public static bool IsCountering(Mobile m)
        {
            return m_Table.Contains(m);
        }

        public static void StartCountering(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            t = new InternalTimer(m);

            m_Table[m] = t;

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.CounterAttack, 1060598, 1063266, TimeSpan.FromSeconds(30), m));

            t.Start();
        }

        public static void StopCountering(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            m_Table.Remove(m);

            BuffInfo.RemoveBuff(m, BuffIcon.CounterAttack);

            OnEffectEnd(m, typeof(CounterAttack));
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (this.Caster.FindItemOnLayer(Layer.TwoHanded) as BaseShield != null)
                return true;

            if (this.Caster.FindItemOnLayer(Layer.OneHanded) as BaseWeapon != null)
                return true;

            if (this.Caster.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon != null)
                return true;

            this.Caster.SendLocalizedMessage(1062944); // You must have a weapon or a shield equipped to use this ability!
            return false;
        }

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            this.Caster.FixedEffect(0x37C4, 10, 7, 4, 3);
        }

        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                this.Caster.SendLocalizedMessage(1063118); // You prepare to respond immediately to the next blocked blow.

                this.OnCastSuccessful(this.Caster);

                StartCountering(this.Caster);
            }

            this.FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public InternalTimer(Mobile m)
                : base(TimeSpan.FromSeconds(30.0))
            {
                this.m_Mobile = m;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                StopCountering(this.m_Mobile);
                this.m_Mobile.SendLocalizedMessage(1063119); // You return to your normal stance.
            }
        }
    }
}