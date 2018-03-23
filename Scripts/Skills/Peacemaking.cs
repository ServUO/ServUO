#region Header
// **********
// ServUO - Peacemaking.cs
// **********
#endregion

#region References
using System;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.Quests;
#endregion

namespace Server.SkillHandlers
{
	public class Peacemaking
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Peacemaking].Callback = OnUse;
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
			from.SendLocalizedMessage(1049525); // Whom do you wish to calm?
			from.Target = new InternalTarget(from, instrument);
			from.NextSkillTime = Core.TickCount + 21600000;
		}

        public static bool UnderEffects(Mobile m)
        {
            return m is BaseCreature && ((BaseCreature)m).BardPacified;
        }

		public class InternalTarget : Target
		{
			private readonly BaseInstrument m_Instrument;
			private bool m_SetSkillTime = true;

			public InternalTarget(Mobile from, BaseInstrument instrument)
				: base(BaseInstrument.GetBardRange(from, SkillName.Peacemaking), false, TargetFlags.None)
			{
				m_Instrument = instrument;
			}

			protected override void OnTargetFinish(Mobile from)
			{
				if (m_SetSkillTime)
				{
					from.NextSkillTime = Core.TickCount;
				}
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				from.RevealingAction();

				if (!(targeted is Mobile))
				{
					from.SendLocalizedMessage(1049528); // You cannot calm that!
				}
				else if (!m_Instrument.IsChildOf(from.Backpack))
				{
					from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!
				}
				else
				{
					m_SetSkillTime = false;

                    int masteryBonus = 0;

                    if (from is PlayerMobile)
                        masteryBonus = Spells.SkillMasteries.BardSpell.GetMasteryBonus((PlayerMobile)from, SkillName.Peacemaking);

					if (targeted == from)
					{
						// Standard mode : reset combatants for everyone in the area
                        if (from.Player && !BaseInstrument.CheckMusicianship(from))
						{
							from.SendLocalizedMessage(500612); // You play poorly, and there is no effect.
							m_Instrument.PlayInstrumentBadly(from);
							m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + (10000 - ((masteryBonus / 5) * 1000));
						}
						else if (!from.CheckSkill(SkillName.Peacemaking, 0.0, 120.0))
						{
							from.SendLocalizedMessage(500613); // You attempt to calm everyone, but fail.
							m_Instrument.PlayInstrumentBadly(from);
							m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + (10000 - ((masteryBonus / 5) * 1000));
						}
						else
						{
							from.NextSkillTime = Core.TickCount + 5000;
							m_Instrument.PlayInstrumentWell(from);
							m_Instrument.ConsumeUse(from);

							Map map = from.Map;

							if (map != null)
							{
								int range = BaseInstrument.GetBardRange(from, SkillName.Peacemaking);

								bool calmed = false;
                                IPooledEnumerable eable = from.GetMobilesInRange(range);

								foreach (Mobile m in eable)
								{
									if ((m is BaseCreature && ((BaseCreature)m).Uncalmable) ||
										(m is BaseCreature && ((BaseCreature)m).AreaPeaceImmune) || m == from || !from.CanBeHarmful(m, false))
									{
										continue;
									}

									calmed = true;

									m.SendLocalizedMessage(500616); // You hear lovely music, and forget to continue battling!
									m.Combatant = null;
									m.Warmode = false;

									if (m is BaseCreature && !((BaseCreature)m).BardPacified)
									{
										((BaseCreature)m).Pacify(from, DateTime.UtcNow + TimeSpan.FromSeconds(1.0));
									}
								}
                                eable.Free();

								if (!calmed)
								{
									from.SendLocalizedMessage(1049648); // You play hypnotic music, but there is nothing in range for you to calm.
								}
								else
								{
									from.SendLocalizedMessage(500615); // You play your hypnotic music, stopping the battle.
								}
							}
						}
					}
					else
					{
						// Target mode : pacify a single target for a longer duration
						Mobile targ = (Mobile)targeted;

						if (!from.CanBeHarmful(targ, false))
						{
							from.SendLocalizedMessage(1049528);
							m_SetSkillTime = true;
						}
						else if (targ is BaseCreature && ((BaseCreature)targ).Uncalmable)
						{
							from.SendLocalizedMessage(1049526); // You have no chance of calming that creature.
							m_SetSkillTime = true;
						}
						else if (targ is BaseCreature && ((BaseCreature)targ).BardPacified)
						{
							from.SendLocalizedMessage(1049527); // That creature is already being calmed.
							m_SetSkillTime = true;
						}
                        else if (from.Player && !BaseInstrument.CheckMusicianship(from))
						{
							from.SendLocalizedMessage(500612); // You play poorly, and there is no effect.
							from.NextSkillTime = Core.TickCount + 5000;
							m_Instrument.PlayInstrumentBadly(from);
							m_Instrument.ConsumeUse(from);
						}
						else
						{
							double diff = m_Instrument.GetDifficultyFor(targ) - 10.0;
							double music = from.Skills[SkillName.Musicianship].Value;

							diff += XmlMobFactions.GetScaledFaction(from, targ, -25, 25, -0.001);

							if (music > 100.0)
							{
								diff -= (music - 100.0) * 0.5;
							}

                            if (masteryBonus > 0)
                                diff -= (diff * ((double)masteryBonus / 100));

							if (!from.CheckTargetSkill(SkillName.Peacemaking, targ, diff - 25.0, diff + 25.0))
							{
								from.SendLocalizedMessage(1049531); // You attempt to calm your target, but fail.
								m_Instrument.PlayInstrumentBadly(from);
								m_Instrument.ConsumeUse(from);

                                from.NextSkillTime = Core.TickCount + (10000 - ((masteryBonus / 5) * 1000));
							}
							else
							{
								m_Instrument.PlayInstrumentWell(from);
								m_Instrument.ConsumeUse(from);

                                from.NextSkillTime = Core.TickCount + (5000 - ((masteryBonus / 5) * 1000));

								if (targ is BaseCreature)
								{
									BaseCreature bc = (BaseCreature)targ;

									from.SendLocalizedMessage(1049532); // You play hypnotic music, calming your target.

									targ.Combatant = null;
									targ.Warmode = false;

									double seconds = 100 - (diff / 1.5);

									if (seconds > 120)
									{
										seconds = 120;
									}
									else if (seconds < 10)
									{
										seconds = 10;
									}

									bc.Pacify(from, DateTime.UtcNow + TimeSpan.FromSeconds(seconds));

                                    #region Bard Mastery Quest
                                    if (from is PlayerMobile)
                                    {
                                        BaseQuest quest = QuestHelper.GetQuest((PlayerMobile)from, typeof(TheBeaconOfHarmonyQuest));

                                        if (quest != null)
                                        {
                                            foreach (BaseObjective objective in quest.Objectives)
                                                objective.Update(bc);
                                        }
                                    }
                                    #endregion
								}
								else
								{
									from.SendLocalizedMessage(1049532); // You play hypnotic music, calming your target.

									targ.SendLocalizedMessage(500616); // You hear lovely music, and forget to continue battling!
									targ.Combatant = null;
									targ.Warmode = false;
								}
							}
						}
					}
				}
			}
		}
	}
}
