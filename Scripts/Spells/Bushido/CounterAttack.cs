using Server.Items;
using System;
using System.Collections;

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

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(0.25);
        public override double RequiredSkill => 40.0;
        public override int RequiredMana => 5;
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

            if (Caster.FindItemOnLayer(Layer.TwoHanded) as BaseShield != null)
                return true;

            if (Caster.FindItemOnLayer(Layer.OneHanded) as BaseWeapon != null)
                return true;

            if (Caster.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon != null)
                return true;

            Caster.SendLocalizedMessage(1062944); // You must have a weapon or a shield equipped to use this ability!
            return false;
        }

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            Caster.FixedEffect(0x37C4, 10, 7, 4, 3);
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Caster.SendLocalizedMessage(1063118); // You prepare to respond immediately to the next blocked blow.

                OnCastSuccessful(Caster);

                StartCountering(Caster);
            }

            FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public InternalTimer(Mobile m)
                : base(TimeSpan.FromSeconds(30.0))
            {
                m_Mobile = m;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                StopCountering(m_Mobile);
                m_Mobile.SendLocalizedMessage(1063119); // You return to your normal stance.
            }
        }
    }
}