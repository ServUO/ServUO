using Server.Gumps;
using Server.Items;
using Server.Spells.Fifth;
using System.Collections;

namespace Server.Spells.Seventh
{
    public class PolymorphSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Polymorph", "Vas Ylem Rel",
            221,
            9002,
            Reagent.Bloodmoss,
            Reagent.SpidersSilk,
            Reagent.MandrakeRoot);

        private static readonly Hashtable m_Timers = new Hashtable();
        private readonly int m_NewBody;

        public PolymorphSpell(Mobile caster, Item scroll, int body)
            : base(caster, scroll, m_Info)
        {
            m_NewBody = body;
        }

        public PolymorphSpell(Mobile caster, Item scroll)
            : this(caster, scroll, 0)
        {
        }

        public override SpellCircle Circle => SpellCircle.Seventh;
        public static bool StopTimer(Mobile m)
        {
            Timer t = (Timer)m_Timers[m];

            if (t != null)
            {
                t.Stop();
                m_Timers.Remove(m);
            }

            return (t != null);
        }

        public override bool CheckCast()
        {
            if (Caster.Flying)
            {
                Caster.SendLocalizedMessage(1113415); // You cannot use this ability while flying.
                return false;
            }
            if (TransformationSpellHelper.UnderTransformation(Caster))
            {
                Caster.SendLocalizedMessage(1061633); // You cannot polymorph while in that form.
                return false;
            }
            if (DisguiseTimers.IsDisguised(Caster))
            {
                Caster.SendLocalizedMessage(502167); // You cannot polymorph while disguised.
                return false;
            }
            if (Caster.BodyMod == 183 || Caster.BodyMod == 184)
            {
                Caster.SendLocalizedMessage(1042512); // You cannot polymorph while wearing body paint
                return false;
            }
            if (!Caster.CanBeginAction(typeof(PolymorphSpell)))
            {
                EndPolymorph(Caster);
                return false;
            }
            if (m_NewBody == 0)
            {
                Gump gump = new PolymorphGump(Caster, Scroll);

                Caster.SendGump(gump);
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            if (Caster.Flying)
            {
                Caster.SendLocalizedMessage(1113415); // You cannot use this ability while flying.
            }
            else if (!Caster.CanBeginAction(typeof(PolymorphSpell)))
            {
                EndPolymorph(Caster);
            }
            else if (TransformationSpellHelper.UnderTransformation(Caster))
            {
                Caster.SendLocalizedMessage(1061633); // You cannot polymorph while in that form.
            }
            else if (DisguiseTimers.IsDisguised(Caster))
            {
                Caster.SendLocalizedMessage(502167); // You cannot polymorph while disguised.
            }
            else if (Caster.BodyMod == 183 || Caster.BodyMod == 184)
            {
                Caster.SendLocalizedMessage(1042512); // You cannot polymorph while wearing body paint
            }
            else if (!Caster.CanBeginAction(typeof(IncognitoSpell)) || Caster.IsBodyMod)
            {
                DoFizzle();
            }
            else if (CheckSequence())
            {
                if (Caster.BeginAction(typeof(PolymorphSpell)))
                {
                    if (m_NewBody != 0)
                    {
                        if (!((Body)m_NewBody).IsHuman)
                        {
                            Mobiles.IMount mt = Caster.Mount;

                            if (mt != null)
                                mt.Rider = null;
                        }

                        Caster.BodyMod = m_NewBody;

                        if (m_NewBody == 400 || m_NewBody == 401)
                            Caster.HueMod = Utility.RandomSkinHue();
                        else
                            Caster.HueMod = 0;

                        BaseArmor.ValidateMobile(Caster);
                        BaseClothing.ValidateMobile(Caster);
                    }
                }
                else
                {
                    Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
                }
            }

            FinishSequence();
        }

        public static void EndPolymorph(Mobile m)
        {
            if (!m.CanBeginAction(typeof(PolymorphSpell)))
            {
                m.BodyMod = 0;
                m.HueMod = -1;
                m.EndAction(typeof(PolymorphSpell));

                BaseArmor.ValidateMobile(m);
                BaseClothing.ValidateMobile(m);

                BuffInfo.RemoveBuff(m, BuffIcon.Polymorph);
            }
        }
    }
}
