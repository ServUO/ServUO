using Server.Gumps;
using Server.Items;
using Server.Spells.Fifth;
using System;
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
            else if (TransformationSpellHelper.UnderTransformation(Caster))
            {
                Caster.SendLocalizedMessage(1061633); // You cannot polymorph while in that form.
                return false;
            }
            else if (DisguiseTimers.IsDisguised(Caster))
            {
                Caster.SendLocalizedMessage(502167); // You cannot polymorph while disguised.
                return false;
            }
            else if (Caster.BodyMod == 183 || Caster.BodyMod == 184)
            {
                Caster.SendLocalizedMessage(1042512); // You cannot polymorph while wearing body paint
                return false;
            }
            else if (!Caster.CanBeginAction(typeof(PolymorphSpell)))
            {
                EndPolymorph(Caster);
                return false;
            }
            else if (m_NewBody == 0)
            {
                Gump gump = new NewPolymorphGump(Caster, Scroll);

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

        private static TextDefinition GetArticleCliloc(int body)
        {
            if (body == 0x11 || body == 0x01)
                return "an";

            return "a";
        }

        private static TextDefinition GetFormCliloc(int body)
        {
            switch (body)
            {
                case 0xD9: return 1028476; // dog
                case 0xE1: return 1028482; // wolf
                case 0xD6: return 1028450; // panther
                case 0x1D: return 1028437; // gorilla
                case 0xD3: return 1028472; // black bear
                case 0xD4: return 1028478; // grizzly bear
                case 0xD5: return 1018276; // polar bear
                case 0x190: return 1028454; // human male
                case 0x191: return 1028455; // human female
                case 0x11: return 1018110; // orc
                case 0x21: return 1018128; // lizardman
                case 0x04: return 1018097; // gargoyle
                case 0x01: return 1018094; // ogre
                case 0x36: return 1018147; // troll
                case 0x02: return 1018111; // ettin
                case 0x09: return 1018103; // daemon
                default: return -1;
            }
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

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Owner;
            public InternalTimer(Mobile owner)
                : base(TimeSpan.FromSeconds(0))
            {
                m_Owner = owner;

                int val = (int)owner.Skills[SkillName.Magery].Value;

                if (val > 120)
                    val = 120;

                Delay = TimeSpan.FromSeconds(val);
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                EndPolymorph(m_Owner);
            }
        }
    }
}
