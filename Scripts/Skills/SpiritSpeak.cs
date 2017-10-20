#region Header
// **********
// ServUO - SpiritSpeak.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;

using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Mobiles;
#endregion

namespace Server.SkillHandlers
{
	internal class SpiritSpeak
	{
		public static void Initialize()
		{
			SkillInfo.Table[32].Callback = OnUse;
		}

        public static Dictionary<Mobile, Timer> _Table;

		public static TimeSpan OnUse(Mobile m)
		{
			if (Core.AOS)
			{
                if (m.Spell != null && m.Spell.IsCasting)
                {
                    m.SendLocalizedMessage(502642); // You are already casting a spell.
                }
                else if (BeginSpiritSpeak(m))
                {
                    return TimeSpan.FromSeconds(5.0);
                }

				return TimeSpan.Zero;
			}

			m.RevealingAction();

			if (m.CheckSkill(SkillName.SpiritSpeak, 0, 100))
			{
				if (!m.CanHearGhosts)
				{
					Timer t = new SpiritSpeakTimer(m);
					double secs = m.Skills[SkillName.SpiritSpeak].Base / 50;
					secs *= 90;
					if (secs < 15)
					{
						secs = 15;
					}

					t.Delay = TimeSpan.FromSeconds(secs); //15seconds to 3 minutes
					t.Start();
					m.CanHearGhosts = true;
				}

				m.PlaySound(0x24A);
				m.SendLocalizedMessage(502444); //You contact the neitherworld.
			}
			else
			{
				m.SendLocalizedMessage(502443); //You fail to contact the neitherworld.
				m.CanHearGhosts = false;
			}

			return TimeSpan.FromSeconds(1.0);
		}

		private class SpiritSpeakTimer : Timer
		{
			private readonly Mobile m_Owner;

			public SpiritSpeakTimer(Mobile m)
				: base(TimeSpan.FromMinutes(2.0))
			{
				m_Owner = m;
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				m_Owner.CanHearGhosts = false;
				m_Owner.SendLocalizedMessage(502445); //You feel your contact with the neitherworld fading.
			}
		}

        public static bool BeginSpiritSpeak(Mobile m)
        {
            if (_Table == null || !_Table.ContainsKey(m))
            {
                m.SendSpeedControl(SpeedControlType.NoMove);

                m.Animate(AnimationType.Spell, 1);
                m.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1062074, "", false); // Anh Mi Sah Ko
                m.PlaySound(0x24A);

                if (_Table == null)
                    _Table = new Dictionary<Mobile, Timer>();

                _Table[m] = new SpiritSpeakTimerNew(m);
                return true;
            }

            return false;
        }

        public static bool IsInSpiritSpeak(Mobile m)
        {
            return _Table != null && _Table.ContainsKey(m);
        }

        public static void Remove(Mobile m)
        {
            if (_Table != null && _Table.ContainsKey(m))
            {
                if(_Table[m] != null)
                    _Table[m].Stop();

                m.SendSpeedControl(SpeedControlType.Disable);
                _Table.Remove(m);

                if (_Table.Count == 0)
                    _Table = null;
            }
        }

        public static void CheckDisrupt(Mobile m)
        {
            if (!Core.AOS)
                return;

            if (_Table != null && _Table.ContainsKey(m))
            {
                if (m is PlayerMobile)
                {
                    m.SendLocalizedMessage(500641); // Your concentration is disturbed, thus ruining thy spell.
                }

                m.FixedEffect(0x3735, 6, 30);
                m.PlaySound(0x5C);

                m.NextSkillTime = Core.TickCount;

                Remove(m);
            }
        }

        private class SpiritSpeakTimerNew : Timer
        {
            public Mobile Caster { get; set; }

            public SpiritSpeakTimerNew(Mobile m)
                : base(TimeSpan.FromSeconds(1))
            {
                Start();
                Caster = m;
            }

            protected override void OnTick()
            {
                Corpse toChannel = null;

                foreach (Item item in Caster.GetItemsInRange(3))
                {
                    if (item is Corpse && !((Corpse)item).Channeled)
                    {
                        toChannel = (Corpse)item;
                        break;
                    }
                }

                int max, min, mana, number;

                if (toChannel != null)
                {
                    min = 1 + (int)(Caster.Skills[SkillName.SpiritSpeak].Value * 0.25);
                    max = min + 4;
                    mana = 0;
                    number = 1061287; // You channel energy from a nearby corpse to heal your wounds.
                }
                else
                {
                    min = 1 + (int)(Caster.Skills[SkillName.SpiritSpeak].Value * 0.25);
                    max = min + 4;
                    mana = 10;
                    number = 1061286; // You channel your own spiritual energy to heal your wounds.
                }

                if (Caster.Mana < mana)
                {
                    Caster.SendLocalizedMessage(1061285); // You lack the mana required to use this skill.
                }
                else
                {
                    Caster.CheckSkill(SkillName.SpiritSpeak, 0.0, 120.0);

                    if (Utility.RandomDouble() > (Caster.Skills[SkillName.SpiritSpeak].Value / 100.0))
                    {
                        Caster.SendLocalizedMessage(502443); // You fail your attempt at contacting the netherworld.
                    }
                    else
                    {
                        if (toChannel != null)
                        {
                            toChannel.Channeled = true;
                            toChannel.Hue = 0x835;
                        }

                        Caster.Mana -= mana;
                        Caster.SendLocalizedMessage(number);

                        if (min > max)
                        {
                            min = max;
                        }

                        Caster.Hits += Utility.RandomMinMax(min, max);

                        Caster.FixedParticles(0x375A, 1, 15, 9501, 2100, 4, EffectLayer.Waist);
                    }
                }

                SpiritSpeak.Remove(Caster);
                Stop();
            }
        }
	}
}