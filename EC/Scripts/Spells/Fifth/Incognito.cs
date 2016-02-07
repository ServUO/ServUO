using System;
using System.Collections;
using Server.Items;
using Server.Mobiles;
using Server.Spells.Seventh;

namespace Server.Spells.Fifth
{
    public class IncognitoSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Incognito", "Kal In Ex",
            206,
            9002,
            Reagent.Bloodmoss,
            Reagent.Garlic,
            Reagent.Nightshade);
        private static readonly Hashtable m_Timers = new Hashtable();
        private static readonly int[] m_HairIDs = new int[]
        {
            0x2044, 0x2045, 0x2046,
            0x203C, 0x203B, 0x203D,
            0x2047, 0x2048, 0x2049,
            0x204A, 0x0000
        };
        private static readonly int[] m_BeardIDs = new int[]
        {
            0x203E, 0x203F, 0x2040,
            0x2041, 0x204B, 0x204C,
            0x204D, 0x0000
        };
        public IncognitoSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Fifth;
            }
        }
        public static bool StopTimer(Mobile m)
        {
            Timer t = (Timer)m_Timers[m];

            if (t != null)
            {
                t.Stop();
                m_Timers.Remove(m);
                BuffInfo.RemoveBuff(m, BuffIcon.Incognito);
            }

            return (t != null);
        }

        public override bool CheckCast()
        {
            if (Factions.Sigil.ExistsOn(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1010445); // You cannot incognito if you have a sigil
                return false;
            }
            else if (!this.Caster.CanBeginAction(typeof(IncognitoSpell)))
            {
                this.Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
                return false;
            }
            else if (this.Caster.BodyMod == 183 || this.Caster.BodyMod == 184)
            {
                this.Caster.SendLocalizedMessage(1042402); // You cannot use incognito while wearing body paint
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            if (Factions.Sigil.ExistsOn(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1010445); // You cannot incognito if you have a sigil
            }
            else if (!this.Caster.CanBeginAction(typeof(IncognitoSpell)))
            {
                this.Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
            }
            else if (this.Caster.BodyMod == 183 || this.Caster.BodyMod == 184)
            {
                this.Caster.SendLocalizedMessage(1042402); // You cannot use incognito while wearing body paint
            }
            else if (DisguiseTimers.IsDisguised(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061631); // You can't do that while disguised.
            }
            else if (!this.Caster.CanBeginAction(typeof(PolymorphSpell)) || this.Caster.IsBodyMod)
            {
                this.DoFizzle();
            }
            else if (this.CheckSequence())
            {
                if (this.Caster.BeginAction(typeof(IncognitoSpell)))
                {
                    DisguiseTimers.StopTimer(this.Caster);

                    this.Caster.HueMod = this.Caster.Race.RandomSkinHue();
                    this.Caster.NameMod = this.Caster.Female ? NameList.RandomName("female") : NameList.RandomName("male");

                    PlayerMobile pm = this.Caster as PlayerMobile;

                    if (pm != null && pm.Race != null)
                    {
                        pm.SetHairMods(pm.Race.RandomHair(pm.Female), pm.Race.RandomFacialHair(pm.Female));
                        pm.HairHue = pm.Race.RandomHairHue();
                        pm.FacialHairHue = pm.Race.RandomHairHue();
                    }

                    this.Caster.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);
                    this.Caster.PlaySound(0x3BD);

                    BaseArmor.ValidateMobile(this.Caster);
                    BaseClothing.ValidateMobile(this.Caster);

                    StopTimer(this.Caster);

                    int timeVal = ((6 * this.Caster.Skills.Magery.Fixed) / 50) + 1;

                    if (timeVal > 144)
                        timeVal = 144;

                    TimeSpan length = TimeSpan.FromSeconds(timeVal);

                    Timer t = new InternalTimer(this.Caster, length);

                    m_Timers[this.Caster] = t;

                    t.Start();

                    BuffInfo.AddBuff(this.Caster, new BuffInfo(BuffIcon.Incognito, 1075819, length, this.Caster));
                }
                else
                {
                    this.Caster.SendLocalizedMessage(1079022); // You're already incognitoed!
                }
            }

            this.FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Owner;
            public InternalTimer(Mobile owner, TimeSpan length)
                : base(length)
            {
                this.m_Owner = owner;

                /*
                int val = ((6 * owner.Skills.Magery.Fixed) / 50) + 1;

                if ( val > 144 )
                val = 144;

                Delay = TimeSpan.FromSeconds( val );
                * */
                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (!this.m_Owner.CanBeginAction(typeof(IncognitoSpell)))
                {
                    if (this.m_Owner is PlayerMobile)
                        ((PlayerMobile)this.m_Owner).SetHairMods(-1, -1);

                    this.m_Owner.BodyMod = 0;
                    this.m_Owner.HueMod = -1;
                    this.m_Owner.NameMod = null;
                    this.m_Owner.EndAction(typeof(IncognitoSpell));

                    BaseArmor.ValidateMobile(this.m_Owner);
                    BaseClothing.ValidateMobile(this.m_Owner);
                }
            }
        }
    }
}