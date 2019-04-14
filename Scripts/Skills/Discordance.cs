#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.Quests;
#endregion

namespace Server.SkillHandlers
{
	public class Discordance
	{
        private static readonly Dictionary<Mobile, DiscordanceInfo> m_Table = new Dictionary<Mobile, DiscordanceInfo>();

        public static bool UnderEffects(Mobile m)
        {
            return m != null && m_Table.ContainsKey(m) && !m_Table[m].m_PVP;
        }

        public static bool UnderPVPEffects(Mobile m)
        {
            return m != null && m_Table.ContainsKey(m) && m_Table[m].m_PVP;
        }

        public static void RemoveEffects(Mobile m)
        {
            if (m_Table.ContainsKey(m))
            {
                m_Table.Remove(m);
            }
        }

		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Discordance].Callback = OnUse;
		}

		public static TimeSpan OnUse(Mobile m)
		{
			m.RevealingAction();

			BaseInstrument.PickInstrument(m, OnPickedInstrument);

			return TimeSpan.FromSeconds(1.0); // Cannot use another skill for 1 second
		}

		public static void OnPickedInstrument(Mobile from, BaseInstrument instrument)
		{
			from.RevealingAction();
			from.SendLocalizedMessage(1049541); // Choose the target for your song of discordance.
			from.Target = new DiscordanceTarget(from, instrument);
			from.NextSkillTime = Core.TickCount + 6000;
		}

        public static bool GetEffect(Mobile targ, ref int effect)
        {
            return GetEffect(targ, ref effect, false);
        }

        public static bool GetEffect(Mobile targ, ref int effect, bool pvp)
		{
            if (!m_Table.ContainsKey(targ) || m_Table[targ].m_PVP != pvp)
            {
                return false;
            }

            DiscordanceInfo info = m_Table[targ];

			effect = info.m_Effect;
			return true;
		}

		private static void ProcessDiscordance(DiscordanceInfo info)
		{
			Mobile from = info.m_From;
			Mobile targ = info.m_Target;
			bool ends = false;

            if (info.m_PVP && info.m_Expires < DateTime.UtcNow)
            {
                DiscordanceInfo.RemoveDiscord(info);
            }
            else
            {
                // According to uoherald bard must remain alive, visible, and 
                // within range of the target or the effect ends in 15 seconds.
                if (!targ.Alive || targ.Deleted || !from.Alive || from.Hidden || targ.Hidden)
                {
                    ends = true;
                }
                else
                {
                    int range = (int)targ.GetDistanceToSqrt(from);
                    int maxRange = BaseInstrument.GetBardRange(from, SkillName.Discordance);
                    Map targetMap = targ.Map;

                    if (targ is BaseMount && ((BaseMount)targ).Rider != null)
                    {
                        Mobile rider = ((BaseMount)targ).Rider;

                        range = (int)rider.GetDistanceToSqrt(from);
                        targetMap = rider.Map;
                    }

                    if (from.Map != targetMap || range > maxRange)
                    {
                        ends = true;
                    }
                }

                if (ends && info.m_Ending && info.m_EndTime < DateTime.UtcNow)
                {
                    DiscordanceInfo.RemoveDiscord(info);
                }
                else
                {
                    if (ends && !info.m_Ending)
                    {
                        info.m_Ending = true;
                        info.m_EndTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
                    }
                    else if (!ends)
                    {
                        info.m_Ending = false;
                        info.m_EndTime = DateTime.UtcNow;
                    }

                    targ.FixedEffect(0x376A, 1, 32);
                }
            }
		}

		public class DiscordanceTarget : Target
		{
			private readonly BaseInstrument m_Instrument;

			public DiscordanceTarget(Mobile from, BaseInstrument inst)
				: base(BaseInstrument.GetBardRange(from, SkillName.Discordance), false, TargetFlags.None)
			{
				m_Instrument = inst;
			}

			protected override void OnTarget(Mobile from, object target)
			{
				from.RevealingAction();
				from.NextSkillTime = Core.TickCount + 1000;

				if (!m_Instrument.IsChildOf(from.Backpack))
				{
					from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!
				}
				else if (target is Mobile)
				{
					Mobile targ = (Mobile)target;

					if (targ == from ||
						(targ is BaseCreature && (((BaseCreature)targ).BardImmune || !from.CanBeHarmful(targ, false)) &&
						 ((BaseCreature)targ).ControlMaster != from))
					{
						from.SendLocalizedMessage(1049535); // A song of discord would have no effect on that.
					}
					else if (m_Table.ContainsKey(targ)) //Already discorded
					{
						from.SendLocalizedMessage(1049537); // Your target is already in discord.
					}
					else if (!targ.Player || (from is BaseCreature && ((BaseCreature)from).CanDiscord) || (Core.EJ && targ.Player && from.Player && CanDiscordPVP(from)))
					{
						double diff = m_Instrument.GetDifficultyFor(targ) - 10.0;
						double music = from.Skills[SkillName.Musicianship].Value;

                        if (from is BaseCreature)
                            music = 120.0;

                        int masteryBonus = 0;

						if (music > 100.0)
						{
							diff -= (music - 100.0) * 0.5;
						}

                        if (from is PlayerMobile)
                        {
                            masteryBonus = Spells.SkillMasteries.BardSpell.GetMasteryBonus((PlayerMobile)from, SkillName.Discordance);
                        }

                        if (masteryBonus > 0)
                        {
                            diff -= (diff * ((double)masteryBonus / 100));
                        }

						if (!BaseInstrument.CheckMusicianship(from))
						{
							from.SendLocalizedMessage(500612); // You play poorly, and there is no effect.
							m_Instrument.PlayInstrumentBadly(from);
							m_Instrument.ConsumeUse(from);
						}
						else if (from.CheckTargetSkill(SkillName.Discordance, target, diff - 25.0, diff + 25.0))
						{
							from.SendLocalizedMessage(1049539); // You play the song surpressing your targets strength
							m_Instrument.PlayInstrumentWell(from);
							m_Instrument.ConsumeUse(from);

                            DiscordanceInfo info;

                            if (Core.EJ && targ.Player && from.Player)
                            {
                                info = new DiscordanceInfo(from, targ, 0, null, true, from.Skills.CurrentMastery == SkillName.Discordance ? 6 : 4);
                                from.DoHarmful(targ);
                            }
                            else
                            {
                                ArrayList mods = new ArrayList();
                                int effect;
                                double scalar;

                                if (Core.AOS)
                                {
                                    double discord = from.Skills[SkillName.Discordance].Value;

                                    effect = (int)Math.Max(-28.0, (discord / -4.0));

                                    if (Core.SE && BaseInstrument.GetBaseDifficulty(targ) >= 160.0)
                                    {
                                        effect /= 2;
                                    }

                                    scalar = (double)effect / 100;

                                    mods.Add(new ResistanceMod(ResistanceType.Physical, effect));
                                    mods.Add(new ResistanceMod(ResistanceType.Fire, effect));
                                    mods.Add(new ResistanceMod(ResistanceType.Cold, effect));
                                    mods.Add(new ResistanceMod(ResistanceType.Poison, effect));
                                    mods.Add(new ResistanceMod(ResistanceType.Energy, effect));

                                    for (int i = 0; i < targ.Skills.Length; ++i)
                                    {
                                        if (targ.Skills[i].Value > 0)
                                        {
                                            mods.Add(new DefaultSkillMod((SkillName)i, true, targ.Skills[i].Value * scalar));
                                        }
                                    }
                                }
                                else
                                {
                                    effect = (int)(from.Skills[SkillName.Discordance].Value / -5.0);
                                    scalar = effect * 0.01;

                                    mods.Add(new StatMod(StatType.Str, "DiscordanceStr", (int)(targ.RawStr * scalar), TimeSpan.Zero));
                                    mods.Add(new StatMod(StatType.Int, "DiscordanceInt", (int)(targ.RawInt * scalar), TimeSpan.Zero));
                                    mods.Add(new StatMod(StatType.Dex, "DiscordanceDex", (int)(targ.RawDex * scalar), TimeSpan.Zero));

                                    for (int i = 0; i < targ.Skills.Length; ++i)
                                    {
                                        if (targ.Skills[i].Value > 0)
                                        {
                                            mods.Add(new DefaultSkillMod((SkillName)i, true, Math.Max(100, targ.Skills[i].Value) * scalar));
                                        }
                                    }
                                }

                                info = new DiscordanceInfo(from, targ, Math.Abs(effect), mods);

                                #region Bard Mastery Quest
                                if (from is PlayerMobile)
                                {
                                    BaseQuest quest = QuestHelper.GetQuest((PlayerMobile)from, typeof(WieldingTheSonicBladeQuest));

                                    if (quest != null)
                                    {
                                        foreach (BaseObjective objective in quest.Objectives)
                                            objective.Update(targ);
                                    }
                                }
                                #endregion
                            }

                            info.m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1.25), ProcessDiscordance, info);

                            m_Table[targ] = info;
                            from.NextSkillTime = Core.TickCount + (8000 - ((masteryBonus / 5) * 1000));
                        }
						else
						{
                            if (from is BaseCreature && PetTrainingHelper.Enabled)
                                from.CheckSkill(SkillName.Discordance, 0, from.Skills[SkillName.Discordance].Cap);

							from.SendLocalizedMessage(1049540); // You attempt to disrupt your target, but fail.
                            m_Instrument.PlayInstrumentBadly(from);
							m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + 5000;
                        }                        
                    }
					else
					{
						m_Instrument.PlayInstrumentBadly(from);
					}
				}
				else
				{
					from.SendLocalizedMessage(1049535); // A song of discord would have no effect on that.
				}
			}

            private bool CanDiscordPVP(Mobile m)
            {
                return !m_Table.Values.Any(info => info.m_From == m && info.m_PVP);
            }
		}

		private class DiscordanceInfo
		{
			public readonly Mobile m_From;
			public readonly Mobile m_Target;
			public readonly int m_Effect;
			public readonly ArrayList m_Mods;
			public DateTime m_EndTime;
			public bool m_Ending;
			public Timer m_Timer;

            // Pub 103 PVP Additions
            public DateTime m_Expires;
            public readonly bool m_PVP;

            public DiscordanceInfo(Mobile from, Mobile creature, int effect, ArrayList mods)
                : this(from, creature, effect, mods, false, 0)
            {
            }


            public DiscordanceInfo(Mobile from, Mobile creature, int effect, ArrayList mods, bool pvp, int duration)
			{
				m_From = from;
				m_Target = creature;
				m_EndTime = DateTime.UtcNow;
				m_Ending = false;
				m_Effect = effect;
				m_Mods = mods;
                m_PVP = pvp;

                if (m_PVP)
                {
                    m_Expires = DateTime.UtcNow + TimeSpan.FromSeconds(duration);
                }

				Apply();
			}

			public void Apply()
			{
                if (m_PVP)
                {
                    foreach (var item in m_Target.Items)
                    {
                        var bonuses = RunicReforging.GetAosSkillBonuses(item);

                        if (bonuses != null)
                        {
                            bonuses.Remove();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < m_Mods.Count; ++i)
                    {
                        object mod = m_Mods[i];

                        if (mod is ResistanceMod)
                        {
                            m_Target.AddResistanceMod((ResistanceMod)mod);
                        }
                        else if (mod is StatMod)
                        {
                            m_Target.AddStatMod((StatMod)mod);
                        }
                        else if (mod is SkillMod)
                        {
                            m_Target.AddSkillMod((SkillMod)mod);
                        }
                    }
                }
			}

			public void Clear()
			{
                if (m_PVP)
                {
                    Timer.DelayCall(() =>
                    {
                        foreach (var item in m_Target.Items)
                        {
                            var bonuses = RunicReforging.GetAosSkillBonuses(item);

                            if (bonuses != null)
                            {
                                bonuses.AddTo(m_Target);
                            }
                        }
                    });
                }
                else
                {
                    for (int i = 0; i < m_Mods.Count; ++i)
                    {
                        object mod = m_Mods[i];

                        if (mod is ResistanceMod)
                        {
                            m_Target.RemoveResistanceMod((ResistanceMod)mod);
                        }
                        else if (mod is StatMod)
                        {
                            m_Target.RemoveStatMod(((StatMod)mod).Name);
                        }
                        else if (mod is SkillMod)
                        {
                            m_Target.RemoveSkillMod((SkillMod)mod);
                        }
                    }
                }
			}

            public static void RemoveDiscord(DiscordanceInfo info)
            {
                if (info.m_Timer != null)
                {
                    info.m_Timer.Stop();
                }

                var targ = info.m_Target;
                info.Clear();

                Discordance.RemoveEffects(targ);
            }
        }
	}
}
