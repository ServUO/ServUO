#region Header
// **********
// ServUO - BaseAI.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.IO;

using Server.ContextMenus;
using Server.Engines.Quests;
using Server.Engines.Quests.Necro;
using Server.Engines.XmlSpawner2;
using Server.Factions;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Regions;
using Server.Spells;
using Server.Spells.Spellweaving;
using Server.Targets;

using MoveImpl = Server.Movement.MovementImpl;
#endregion

namespace Server.Mobiles
{
	public enum AIType
	{
		AI_Use_Default,
		AI_Melee,
		AI_Animal,
		AI_Archer,
		AI_Healer,
		AI_Vendor,
		AI_Mage,
		AI_Berserk,
		AI_Predator,
		AI_Thief,
		AI_NecroMage,
		AI_OrcScout,
		AI_Spellbinder,
		AI_OmniAI,
        AI_Samurai,
        AI_Ninja,
        AI_Spellweaving,
        AI_Mystic
	}

	public enum ActionType
	{
		Wander,
		Combat,
		Guard,
		Flee,
		Backoff,
		Interact
	}

	public abstract class BaseAI
	{
		public Timer m_Timer;
		protected ActionType m_Action;
		private long m_NextStopGuard;

		public BaseCreature m_Mobile;

		public BaseAI(BaseCreature m)
		{
			m_Mobile = m;

			m_Timer = new AITimer(this);

			bool activate;

			if (!m.PlayerRangeSensitive)
			{
				activate = true;
			}
			else if (World.Loading)
			{
				activate = false;
			}
			else if (m.Map == null || m.Map == Map.Internal || !m.Map.GetSector(m).Active)
			{
				activate = false;
			}
			else
			{
				activate = true;
			}

			if (activate)
			{
				m_Timer.Start();
			}

			Action = ActionType.Wander;
		}

		public ActionType Action
		{
			get { return m_Action; }
			set
			{
				m_Action = value;
				OnActionChanged();
			}
		}

		public virtual bool WasNamed(string speech)
		{
			string name = m_Mobile.Name;

			return (name != null && Insensitive.StartsWith(speech, name));
		}

		private class InternalEntry : ContextMenuEntry
		{
			private readonly Mobile m_From;
			private readonly BaseCreature m_Mobile;
			private readonly BaseAI m_AI;
			private readonly OrderType m_Order;

			public InternalEntry(Mobile from, int number, int range, BaseCreature mobile, BaseAI ai, OrderType order)
				: base(number, range)
			{
				m_From = from;
				m_Mobile = mobile;
				m_AI = ai;
				m_Order = order;

				if (mobile.IsDeadPet &&
					(order == OrderType.Guard || order == OrderType.Attack || order == OrderType.Transfer || order == OrderType.Drop))
				{
					Enabled = false;
				}
			}

			public override void OnClick()
			{
				if (!m_Mobile.Deleted && m_Mobile.Controlled && m_From.CheckAlive())
				{
					if (m_Mobile.IsDeadPet &&
						(m_Order == OrderType.Guard || m_Order == OrderType.Attack || m_Order == OrderType.Transfer ||
						 m_Order == OrderType.Drop))
					{
						return;
					}

					bool isOwner = (m_From == m_Mobile.ControlMaster);
					bool isFriend = (!isOwner && m_Mobile.IsPetFriend(m_From));

					if (!isOwner && !isFriend)
					{
						return;
					}
					else if (isFriend && m_Order != OrderType.Follow && m_Order != OrderType.Stay && m_Order != OrderType.Stop)
					{
						return;
					}

					switch (m_Order)
					{
						case OrderType.Follow:
						case OrderType.Attack:
						case OrderType.Transfer:
						case OrderType.Friend:
						case OrderType.Unfriend:
							{
								if (m_Order == OrderType.Transfer && m_From.HasTrade)
								{
									m_From.SendLocalizedMessage(1010507); // You cannot transfer a pet with a trade pending
								}
								else if (m_Order == OrderType.Friend && m_From.HasTrade)
								{
									m_From.SendLocalizedMessage(1070947); // You cannot friend a pet with a trade pending
								}
								else
								{
									m_AI.BeginPickTarget(m_From, m_Order);
								}

								break;
							}
						case OrderType.Release:
							{
								if (m_Mobile.Summoned)
								{
									goto default;
								}
								else
								{
									m_From.SendGump(new ConfirmReleaseGump(m_From, m_Mobile));
								}

								break;
							}
						default:
							{
								if (m_Mobile.CheckControlChance(m_From))
								{
									m_Mobile.ControlOrder = m_Order;
								}

								break;
							}
					}
				}
			}
		}

		public virtual void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (from.Alive && m_Mobile.Controlled && from.InRange(m_Mobile, 14))
			{
				if (from == m_Mobile.ControlMaster)
				{
					list.Add(new InternalEntry(from, 6107, 14, m_Mobile, this, OrderType.Guard)); // Command: Guard
					list.Add(new InternalEntry(from, 6108, 14, m_Mobile, this, OrderType.Follow)); // Command: Follow

					if (m_Mobile.CanDrop)
					{
						list.Add(new InternalEntry(from, 6109, 14, m_Mobile, this, OrderType.Drop)); // Command: Drop
					}

					list.Add(new InternalEntry(from, 6111, 14, m_Mobile, this, OrderType.Attack)); // Command: Kill

					list.Add(new InternalEntry(from, 6112, 14, m_Mobile, this, OrderType.Stop)); // Command: Stop
					list.Add(new InternalEntry(from, 6114, 14, m_Mobile, this, OrderType.Stay)); // Command: Stay

					if (!m_Mobile.Summoned && !(m_Mobile is GrizzledMare))
					{
						list.Add(new InternalEntry(from, 6110, 14, m_Mobile, this, OrderType.Friend)); // Add Friend
						list.Add(new InternalEntry(from, 6099, 14, m_Mobile, this, OrderType.Unfriend)); // Remove Friend
						list.Add(new InternalEntry(from, 6113, 14, m_Mobile, this, OrderType.Transfer)); // Transfer
					}

					list.Add(new InternalEntry(from, 6118, 14, m_Mobile, this, OrderType.Release)); // Release
				}
				else if (m_Mobile.IsPetFriend(from))
				{
					list.Add(new InternalEntry(from, 6108, 14, m_Mobile, this, OrderType.Follow)); // Command: Follow
					list.Add(new InternalEntry(from, 6112, 14, m_Mobile, this, OrderType.Stop)); // Command: Stop
					list.Add(new InternalEntry(from, 6114, 14, m_Mobile, this, OrderType.Stay)); // Command: Stay
				}
			}
		}

		public virtual void BeginPickTarget(Mobile from, OrderType order)
		{
			if (m_Mobile.Deleted || !m_Mobile.Controlled || !from.InRange(m_Mobile, 14) || from.Map != m_Mobile.Map)
			{
				return;
			}

			bool isOwner = (from == m_Mobile.ControlMaster);
			bool isFriend = (!isOwner && m_Mobile.IsPetFriend(from));

			if (!isOwner && !isFriend)
			{
				return;
			}
			else if (isFriend && order != OrderType.Follow && order != OrderType.Stay && order != OrderType.Stop)
			{
				return;
			}

			if (from.Target == null)
			{
				if (order == OrderType.Transfer)
				{
					from.SendLocalizedMessage(502038); // Click on the person to transfer ownership to.
				}
				else if (order == OrderType.Friend)
				{
					from.SendLocalizedMessage(502020); // Click on the player whom you wish to make a co-owner.
				}
				else if (order == OrderType.Unfriend)
				{
					from.SendLocalizedMessage(1070948); // Click on the player whom you wish to remove as a co-owner.
				}

				from.Target = new AIControlMobileTarget(this, order);
			}
			else if (from.Target is AIControlMobileTarget)
			{
				AIControlMobileTarget t = (AIControlMobileTarget)from.Target;

				if (t.Order == order)
				{
					t.AddAI(this);
				}
			}
		}

		public virtual void OnAggressiveAction(Mobile aggressor)
		{
			Mobile currentCombat = m_Mobile.Combatant;

			if (currentCombat != null && !aggressor.Hidden && currentCombat != aggressor &&
				m_Mobile.GetDistanceToSqrt(currentCombat) > m_Mobile.GetDistanceToSqrt(aggressor))
			{
				m_Mobile.Combatant = aggressor;
			}
		}

		public virtual void EndPickTarget(Mobile from, Mobile target, OrderType order)
		{
			if (m_Mobile.Deleted || !m_Mobile.Controlled || !from.InRange(m_Mobile, 14) || from.Map != m_Mobile.Map ||
				!from.CheckAlive())
			{
				return;
			}

			bool isOwner = (from == m_Mobile.ControlMaster);
			bool isFriend = (!isOwner && m_Mobile.IsPetFriend(from));

			if (!isOwner && !isFriend)
			{
				return;
			}
			else if (isFriend && order != OrderType.Follow && order != OrderType.Stay && order != OrderType.Stop)
			{
				return;
			}

			if (order == OrderType.Attack)
			{
				if (target is BaseCreature && ((BaseCreature)target).IsScaryToPets && m_Mobile.IsScaredOfScaryThings)
				{
					m_Mobile.SayTo(from, "Your pet refuses to attack this creature!");
					return;
				}

				if ((SolenHelper.CheckRedFriendship(from) &&
					 (target is RedSolenInfiltratorQueen || target is RedSolenInfiltratorWarrior || target is RedSolenQueen ||
					  target is RedSolenWarrior || target is RedSolenWorker)) ||
					(SolenHelper.CheckBlackFriendship(from) &&
					 (target is BlackSolenInfiltratorQueen || target is BlackSolenInfiltratorWarrior || target is BlackSolenQueen ||
					  target is BlackSolenWarrior || target is BlackSolenWorker)))
				{
					from.SendAsciiMessage("You can not force your pet to attack a creature you are protected from.");
					return;
				}

				if (target is BaseFactionGuard)
				{
					m_Mobile.SayTo(from, "Your pet refuses to attack the guard.");
					return;
				}
			}

			if (m_Mobile.CheckControlChance(from))
			{
				m_Mobile.ControlTarget = target;
				m_Mobile.ControlOrder = order;
			}
		}

		public virtual bool HandlesOnSpeech(Mobile from)
		{
			if (from.AccessLevel >= AccessLevel.GameMaster)
			{
				return true;
			}

			if (from.Alive && m_Mobile.Controlled && m_Mobile.Commandable &&
				(from == m_Mobile.ControlMaster || m_Mobile.IsPetFriend(from)))
			{
				return true;
			}

			return (from.Alive && from.InRange(m_Mobile.Location, 3) && m_Mobile.IsHumanInTown());
		}

		private static readonly SkillName[] m_KeywordTable = new[]
		{
			SkillName.Parry, SkillName.Healing, SkillName.Hiding, SkillName.Stealing, SkillName.Alchemy, SkillName.AnimalLore,
			SkillName.ItemID, SkillName.ArmsLore, SkillName.Begging, SkillName.Blacksmith, SkillName.Fletching,
			SkillName.Peacemaking, SkillName.Camping, SkillName.Carpentry, SkillName.Cartography, SkillName.Cooking,
			SkillName.DetectHidden, SkillName.Discordance, //??
			SkillName.EvalInt, SkillName.Fishing, SkillName.Provocation, SkillName.Lockpicking, SkillName.Magery,
			SkillName.MagicResist, SkillName.Tactics, SkillName.Snooping, SkillName.RemoveTrap, SkillName.Musicianship,
			SkillName.Poisoning, SkillName.Archery, SkillName.SpiritSpeak, SkillName.Tailoring, SkillName.AnimalTaming,
			SkillName.TasteID, SkillName.Tinkering, SkillName.Veterinary, SkillName.Forensics, SkillName.Herding,
			SkillName.Tracking, SkillName.Stealth, SkillName.Inscribe, SkillName.Swords, SkillName.Macing, SkillName.Fencing,
			SkillName.Wrestling, SkillName.Lumberjacking, SkillName.Mining, SkillName.Meditation
		};

		public virtual void OnSpeech(SpeechEventArgs e)
		{
			if (e.Mobile.Alive && e.Mobile.InRange(m_Mobile.Location, 3) && m_Mobile.IsHumanInTown())
			{
				if (e.HasKeyword(0x9D) && WasNamed(e.Speech)) // *move*
				{
					if (m_Mobile.Combatant != null)
					{
						// I am too busy fighting to deal with thee!
						m_Mobile.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501482);
					}
					else
					{
						// Excuse me?
						m_Mobile.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501516);
						WalkRandomInHome(2, 2, 1);
					}
				}
				else if (e.HasKeyword(0x9E) && WasNamed(e.Speech)) // *time*
				{
					if (m_Mobile.Combatant != null)
					{
						// I am too busy fighting to deal with thee!
						m_Mobile.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501482);
					}
					else
					{
						int generalNumber;
						string exactTime;

						Clock.GetTime(m_Mobile, out generalNumber, out exactTime);

						m_Mobile.PublicOverheadMessage(MessageType.Regular, 0x3B2, generalNumber);
					}
				}
				else if (e.HasKeyword(0x6C) && WasNamed(e.Speech)) // *train
				{
					if (m_Mobile.Combatant != null)
					{
						// I am too busy fighting to deal with thee!
						m_Mobile.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501482);
					}
					else
					{
						bool foundSomething = false;

						Skills ourSkills = m_Mobile.Skills;
						Skills theirSkills = e.Mobile.Skills;

						for (int i = 0; i < ourSkills.Length && i < theirSkills.Length; ++i)
						{
							Skill skill = ourSkills[i];
							Skill theirSkill = theirSkills[i];

							if (skill != null && theirSkill != null && skill.Base >= 60.0 && m_Mobile.CheckTeach(skill.SkillName, e.Mobile))
							{
								double toTeach = skill.Base / 3.0;

								if (toTeach > 42.0)
								{
									toTeach = 42.0;
								}

								if (toTeach > theirSkill.Base)
								{
									int number = 1043059 + i;

									if (number > 1043107)
									{
										continue;
									}

									if (!foundSomething)
									{
										m_Mobile.Say(1043058); // I can train the following:
									}

									m_Mobile.Say(number);

									foundSomething = true;
								}
							}
						}

						if (!foundSomething)
						{
							m_Mobile.Say(501505); // Alas, I cannot teach thee anything.
						}
					}
				}
				else
				{
					SkillName toTrain = (SkillName)(-1);

					for (int i = 0; toTrain == (SkillName)(-1) && i < e.Keywords.Length; ++i)
					{
						int keyword = e.Keywords[i];

						if (keyword == 0x154)
						{
							toTrain = SkillName.Anatomy;
						}
						else if (keyword >= 0x6D && keyword <= 0x9C)
						{
							int index = keyword - 0x6D;

							if (index >= 0 && index < m_KeywordTable.Length)
							{
								toTrain = m_KeywordTable[index];
							}
						}
					}

					if (toTrain != (SkillName)(-1) && WasNamed(e.Speech))
					{
						if (m_Mobile.Combatant != null)
						{
							// I am too busy fighting to deal with thee!
							m_Mobile.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501482);
						}
						else
						{
							Skills skills = m_Mobile.Skills;
							Skill skill = skills[toTrain];

							if (skill == null || skill.Base < 60.0 || !m_Mobile.CheckTeach(toTrain, e.Mobile))
							{
								m_Mobile.Say(501507); // 'Tis not something I can teach thee of.
							}
							else
							{
								m_Mobile.Teach(toTrain, e.Mobile, 0, false);
							}
						}
					}
				}
			}

			if (m_Mobile.Controlled && m_Mobile.Commandable)
			{
				m_Mobile.DebugSay("Listening...");

				bool isOwner = (e.Mobile == m_Mobile.ControlMaster);
				bool isFriend = (!isOwner && m_Mobile.IsPetFriend(e.Mobile));

				if (e.Mobile.Alive && (isOwner || isFriend))
				{
					m_Mobile.DebugSay("It's from my master");

					var keywords = e.Keywords;
					string speech = e.Speech;

					// First, check the all*
					for (int i = 0; i < keywords.Length; ++i)
					{
						int keyword = keywords[i];

						switch (keyword)
						{
							case 0x164: // all come
								{
									if (!isOwner)
									{
										break;
									}

									if (m_Mobile.CheckControlChance(e.Mobile))
									{
										m_Mobile.ControlTarget = null;
										m_Mobile.ControlOrder = OrderType.Come;
									}

									return;
								}
							case 0x165: // all follow
								{
									BeginPickTarget(e.Mobile, OrderType.Follow);
									return;
								}
							case 0x166: // all guard
							case 0x16B: // all guard me
								{
									if (!isOwner)
									{
										break;
									}

									if (m_Mobile.CheckControlChance(e.Mobile))
									{
										
										m_Mobile.ControlOrder = OrderType.Guard;
                                        m_Mobile.ControlTarget = null;
									}
									return;
								}
							case 0x167: // all stop
								{
									if (m_Mobile.CheckControlChance(e.Mobile))
									{
										m_Mobile.ControlTarget = null;
										m_Mobile.ControlOrder = OrderType.Stop;
									}
									return;
								}
							case 0x168: // all kill
							case 0x169: // all attack
								{
									if (!isOwner)
									{
										break;
									}

									BeginPickTarget(e.Mobile, OrderType.Attack);
									return;
								}
							case 0x16C: // all follow me
								{
									if (m_Mobile.CheckControlChance(e.Mobile))
									{
										m_Mobile.ControlTarget = e.Mobile;
										m_Mobile.ControlOrder = OrderType.Follow;
									}
									return;
								}
							case 0x170: // all stay
								{
									if (m_Mobile.CheckControlChance(e.Mobile))
									{
										m_Mobile.ControlTarget = null;
										m_Mobile.ControlOrder = OrderType.Stay;
									}
									return;
								}
						}
					}

					// No all*, so check *command
					for (int i = 0; i < keywords.Length; ++i)
					{
						int keyword = keywords[i];

						switch (keyword)
						{
							case 0x155: // *come
								{
									if (!isOwner)
									{
										break;
									}

									if (WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
										m_Mobile.ControlTarget = null;
										m_Mobile.ControlOrder = OrderType.Come;
									}

									return;
								}
							case 0x156: // *drop
								{
									if (!isOwner)
									{
										break;
									}

									if (!m_Mobile.IsDeadPet && !m_Mobile.Summoned && WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
										m_Mobile.ControlTarget = null;
										m_Mobile.ControlOrder = OrderType.Drop;
									}

									return;
								}
							case 0x15A: // *follow
								{
									if (WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
										BeginPickTarget(e.Mobile, OrderType.Follow);
									}

									return;
								}
							case 0x15B: // *friend
								{
									if (!isOwner)
									{
										break;
									}

									if (WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
										if (m_Mobile.Summoned || (m_Mobile is GrizzledMare))
										{
											e.Mobile.SendLocalizedMessage(1005481); // Summoned creatures are loyal only to their summoners.
										}
										else if (e.Mobile.HasTrade)
										{
											e.Mobile.SendLocalizedMessage(1070947); // You cannot friend a pet with a trade pending
										}
										else
										{
											BeginPickTarget(e.Mobile, OrderType.Friend);
										}
									}

									return;
								}
							case 0x15C: // *guard
								{
									if (!isOwner)
									{
										break;
									}

									if (!m_Mobile.IsDeadPet && WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
                                        m_Mobile.ControlOrder = OrderType.Guard;
                                        m_Mobile.ControlTarget = null;
										
									}

									return;
								}
							case 0x15D: // *kill
							case 0x15E: // *attack
								{
									if (!isOwner)
									{
										break;
									}

									if (!m_Mobile.IsDeadPet && WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
										BeginPickTarget(e.Mobile, OrderType.Attack);
									}

									return;
								}
							case 0x15F: // *patrol
								{
									if (!isOwner)
									{
										break;
									}

									if (WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
										m_Mobile.ControlTarget = null;
										m_Mobile.ControlOrder = OrderType.Patrol;
									}

									return;
								}
							case 0x161: // *stop
								{
									if (WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
										m_Mobile.ControlTarget = null;
										m_Mobile.ControlOrder = OrderType.Stop;
									}

									return;
								}
							case 0x163: // *follow me
								{
									if (WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
										m_Mobile.ControlTarget = e.Mobile;
										m_Mobile.ControlOrder = OrderType.Follow;
									}

									return;
								}
							case 0x16D: // *release
								{
									if (!isOwner)
									{
										break;
									}

									if (WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
										if (!m_Mobile.Summoned)
										{
											e.Mobile.SendGump(new ConfirmReleaseGump(e.Mobile, m_Mobile));
										}
										else
										{
											m_Mobile.ControlTarget = null;
											m_Mobile.ControlOrder = OrderType.Release;
										}
									}

									return;
								}
							case 0x16E: // *transfer
								{
									if (!isOwner)
									{
										break;
									}

									if (!m_Mobile.IsDeadPet && WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
										if (m_Mobile.Summoned || (m_Mobile is GrizzledMare))
										{
											e.Mobile.SendLocalizedMessage(1005487); // You cannot transfer ownership of a summoned creature.
										}
										else if (e.Mobile.HasTrade)
										{
											e.Mobile.SendLocalizedMessage(1010507); // You cannot transfer a pet with a trade pending
										}
										else
										{
											BeginPickTarget(e.Mobile, OrderType.Transfer);
										}
									}

									return;
								}
							case 0x16F: // *stay
								{
									if (WasNamed(speech) && m_Mobile.CheckControlChance(e.Mobile))
									{
										m_Mobile.ControlTarget = null;
										m_Mobile.ControlOrder = OrderType.Stay;
									}

									return;
								}
						}
					}
				}
			}
			else
			{
				if (e.Mobile.AccessLevel >= AccessLevel.GameMaster)
				{
					m_Mobile.DebugSay("It's from a GM");

					if (m_Mobile.FindMyName(e.Speech, true))
					{
						var str = e.Speech.Split(' ');
						int i;

						for (i = 0; i < str.Length; i++)
						{
							string word = str[i];

							if (Insensitive.Equals(word, "obey"))
							{
								m_Mobile.SetControlMaster(e.Mobile);

								if (m_Mobile.Summoned)
								{
									m_Mobile.SummonMaster = e.Mobile;
								}

								return;
							}
						}
					}
				}
			}
		}

		public virtual bool Think()
		{
			if (m_Mobile.Deleted)
			{
				return false;
			}

			if (CheckFlee())
			{
				return true;
			}

			switch (Action)
			{
				case ActionType.Wander:
					m_Mobile.OnActionWander();
					return DoActionWander();

				case ActionType.Combat:
					m_Mobile.OnActionCombat();
					return DoActionCombat();

				case ActionType.Guard:
					m_Mobile.OnActionGuard();
					return DoActionGuard();

				case ActionType.Flee:
					m_Mobile.OnActionFlee();
					return DoActionFlee();

				case ActionType.Interact:
					m_Mobile.OnActionInteract();
					return DoActionInteract();

				case ActionType.Backoff:
					m_Mobile.OnActionBackoff();
					return DoActionBackoff();

				default:
					return false;
			}
		}

		public virtual void OnActionChanged()
		{
			switch (Action)
			{
				case ActionType.Wander:
					m_Mobile.Warmode = false;
					m_Mobile.Combatant = null;
					m_Mobile.FocusMob = null;
					m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
					break;
				case ActionType.Combat:
					m_Mobile.Warmode = true;
					m_Mobile.FocusMob = null;
					m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
					break;
				case ActionType.Guard:
					m_Mobile.Warmode = true;
					m_Mobile.FocusMob = null;
					m_Mobile.Combatant = null;
					m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
					m_NextStopGuard = Core.TickCount + 10000;
					m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
					break;
				case ActionType.Flee:
					m_Mobile.Warmode = true;
					m_Mobile.FocusMob = null;
					m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
					break;
				case ActionType.Interact:
					m_Mobile.Warmode = false;
					m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
					break;
				case ActionType.Backoff:
					m_Mobile.Warmode = false;
					m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
					break;
			}
		}

		public virtual bool OnAtWayPoint()
		{
			return true;
		}

		public virtual bool DoActionWander()
		{
			int followRange = m_Mobile.FollowRange;

			if (CheckHerding())
			{
				m_Mobile.DebugSay("Praise the shepherd!");
			}
			else if (m_Mobile.CurrentWayPoint != null)
			{
				WayPoint point = m_Mobile.CurrentWayPoint;
				if ((point.X != m_Mobile.Location.X || point.Y != m_Mobile.Location.Y) && point.Map == m_Mobile.Map &&
					point.Parent == null && !point.Deleted)
				{
					m_Mobile.DebugSay("I will move towards my waypoint.");
					DoMove(m_Mobile.GetDirectionTo(m_Mobile.CurrentWayPoint));
				}
				else if (OnAtWayPoint())
				{
					m_Mobile.DebugSay("I will go to the next waypoint");
					m_Mobile.CurrentWayPoint = point.NextPoint;
					if (point.NextPoint != null && point.NextPoint.Deleted)
					{
						m_Mobile.CurrentWayPoint = point.NextPoint = point.NextPoint.NextPoint;
					}
				}
			}
			else if (m_Mobile.IsAnimatedDead || m_Mobile.FollowRange > 0)
			{
				// animated dead follow their master
				Mobile master = m_Mobile.SummonMaster;

				if (master != null && master.Map == m_Mobile.Map && master.InRange(m_Mobile, m_Mobile.RangePerception + followRange))
				{
					MoveTo(master, false, followRange);
				}
				else
				{
					WalkRandomInHome(2, 2, 1);
				}
			}
			else if (CheckMove())
			{
				if (!m_Mobile.CheckIdle())
				{
					WalkRandomInHome(2, 2, 1);
				}
			}

			if (m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive &&
				!m_Mobile.Combatant.IsDeadBondedPet)
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo(m_Mobile.Combatant);
			}

			return true;
		}

		public virtual bool DoActionCombat()
		{
			if (Core.AOS && CheckHerding())
			{
				m_Mobile.DebugSay("Praise the shepherd!");
			}
			else
			{
				Mobile c = m_Mobile.Combatant;

				if (c == null || c.Deleted || c.Map != m_Mobile.Map || !c.Alive || c.IsDeadBondedPet)
				{
					Action = ActionType.Wander;
				}
				else
				{
					m_Mobile.Direction = m_Mobile.GetDirectionTo(c);
				}
			}

			return true;
		}

		public virtual bool DoActionGuard()
		{
			if (Core.AOS && CheckHerding())
			{
				m_Mobile.DebugSay("Praise the shepherd!");
			}
			else if (Core.TickCount < m_NextStopGuard)
			{
				m_Mobile.DebugSay("I am on guard");
				//m_Mobile.Turn( Utility.Random(0, 2) - 1 );
			}
			else
			{
				m_Mobile.DebugSay("I stopped being on guard");
				Action = ActionType.Wander;
			}

			return true;
		}

		public virtual bool DoActionFlee()
		{
			Mobile from = m_Mobile.FocusMob;

			if (from == null || from.Deleted || from.Map != m_Mobile.Map)
			{
				m_Mobile.DebugSay("I have lost him");
				Action = ActionType.Guard;
				return true;
			}

			if (WalkMobileRange(from, 1, true, m_Mobile.RangePerception * 2, m_Mobile.RangePerception * 3))
			{
				m_Mobile.DebugSay("I have fled");
				Action = ActionType.Guard;
				return true;
			}
			else
			{
				m_Mobile.DebugSay("I am fleeing!");
			}

			return true;
		}

		public virtual bool DoActionInteract()
		{
			return true;
		}

		public virtual bool DoActionBackoff()
		{
			return true;
		}

		public virtual bool Obey()
		{
			if (m_Mobile.Deleted)
			{
				return false;
			}

			switch (m_Mobile.ControlOrder)
			{
				case OrderType.None:
					return DoOrderNone();

				case OrderType.Come:
					return DoOrderCome();

				case OrderType.Drop:
					return DoOrderDrop();

				case OrderType.Friend:
					return DoOrderFriend();

				case OrderType.Unfriend:
					return DoOrderUnfriend();

				case OrderType.Guard:
					return DoOrderGuard();

				case OrderType.Attack:
					return DoOrderAttack();

				case OrderType.Patrol:
					return DoOrderPatrol();

				case OrderType.Release:
					return DoOrderRelease();

				case OrderType.Stay:
					return DoOrderStay();

				case OrderType.Stop:
					return DoOrderStop();

				case OrderType.Follow:
					return DoOrderFollow();

				case OrderType.Transfer:
					return DoOrderTransfer();

				default:
					return false;
			}
		}

		public virtual void OnCurrentOrderChanged()
		{
			if (m_Mobile.Deleted || m_Mobile.ControlMaster == null || m_Mobile.ControlMaster.Deleted)
			{
				return;
			}

			switch (m_Mobile.ControlOrder)
			{
				case OrderType.None:
					m_Mobile.ControlMaster.RevealingAction();
					m_Mobile.Home = m_Mobile.Location;
					m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
					m_Mobile.PlaySound(m_Mobile.GetIdleSound());
					m_Mobile.Warmode = false;
					m_Mobile.Combatant = null;
					break;
				case OrderType.Come:
					m_Mobile.ControlMaster.RevealingAction();
					m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
					m_Mobile.PlaySound(m_Mobile.GetIdleSound());
					m_Mobile.Warmode = false;
					m_Mobile.Combatant = null;
					break;
				case OrderType.Drop:
					m_Mobile.ControlMaster.RevealingAction();
					m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
					m_Mobile.PlaySound(m_Mobile.GetIdleSound());
					m_Mobile.Warmode = true;
					m_Mobile.Combatant = null;
					break;
				case OrderType.Friend:
				case OrderType.Unfriend:
					m_Mobile.ControlMaster.RevealingAction();
					break;
				case OrderType.Guard:
					m_Mobile.ControlMaster.RevealingAction();
					m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
					m_Mobile.PlaySound(m_Mobile.GetIdleSound());
					m_Mobile.Warmode = true;
					m_Mobile.Combatant = null;
                    m_Mobile.ControlTarget = null;
					string petname = String.Format("{0}", m_Mobile.Name);
					m_Mobile.ControlMaster.SendLocalizedMessage(1049671, petname); //~1_PETNAME~ is now guarding you.
					break;
				case OrderType.Attack:
					m_Mobile.ControlMaster.RevealingAction();
					m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
					m_Mobile.PlaySound(m_Mobile.GetIdleSound());

					m_Mobile.Warmode = true;
					m_Mobile.Combatant = null;
					break;
				case OrderType.Patrol:
					m_Mobile.ControlMaster.RevealingAction();
					m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
					m_Mobile.PlaySound(m_Mobile.GetIdleSound());
					m_Mobile.Warmode = false;
					m_Mobile.Combatant = null;
					break;
				case OrderType.Release:
					m_Mobile.ControlMaster.RevealingAction();
					m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
					m_Mobile.PlaySound(m_Mobile.GetIdleSound());
					m_Mobile.Warmode = false;
					m_Mobile.Combatant = null;
					break;
				case OrderType.Stay:
					m_Mobile.ControlMaster.RevealingAction();
					m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
					m_Mobile.PlaySound(m_Mobile.GetIdleSound());
					m_Mobile.Warmode = false;
					m_Mobile.Combatant = null;
					break;
				case OrderType.Stop:
					m_Mobile.ControlMaster.RevealingAction();
					m_Mobile.Home = m_Mobile.Location;
					m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
					m_Mobile.PlaySound(m_Mobile.GetIdleSound());
					m_Mobile.Warmode = false;
					m_Mobile.Combatant = null;
					break;
				case OrderType.Follow:
					m_Mobile.ControlMaster.RevealingAction();
					m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
					m_Mobile.PlaySound(m_Mobile.GetIdleSound());

					m_Mobile.Warmode = false;
					m_Mobile.Combatant = null;
					break;
				case OrderType.Transfer:
					m_Mobile.ControlMaster.RevealingAction();
					m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
					m_Mobile.PlaySound(m_Mobile.GetIdleSound());

					m_Mobile.Warmode = false;
					m_Mobile.Combatant = null;
					break;
			}
		}

		public virtual bool DoOrderNone()
		{
			m_Mobile.DebugSay("I have no order");

			WalkRandomInHome(3, 2, 1);

			if (m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive &&
				!m_Mobile.Combatant.IsDeadBondedPet)
			{
				m_Mobile.Warmode = true;
				m_Mobile.Direction = m_Mobile.GetDirectionTo(m_Mobile.Combatant);
			}
			else
			{
				m_Mobile.Warmode = false;
			}

			return true;
		}

		public virtual bool DoOrderCome()
		{
			if (m_Mobile.ControlMaster != null && !m_Mobile.ControlMaster.Deleted)
			{
				int iCurrDist = (int)m_Mobile.GetDistanceToSqrt(m_Mobile.ControlMaster);

				if (iCurrDist > m_Mobile.RangePerception)
				{
					m_Mobile.DebugSay("I have lost my master. I stay here");
					m_Mobile.ControlTarget = null;
					m_Mobile.ControlOrder = OrderType.None;
				}
				else
				{
					m_Mobile.DebugSay("My master told me come");

					// Not exactly OSI style, but better than nothing.
					bool bRun = (iCurrDist > 5);

					if (WalkMobileRange(m_Mobile.ControlMaster, 1, bRun, 0, 1))
					{
						if (m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive &&
							!m_Mobile.Combatant.IsDeadBondedPet)
						{
							m_Mobile.Warmode = true;
							m_Mobile.Direction = m_Mobile.GetDirectionTo(m_Mobile.Combatant);
						}
						else
						{
							m_Mobile.Warmode = false;
						}
					}
				}
			}

			return true;
		}

		public virtual bool DoOrderDrop()
		{
			if (m_Mobile.IsDeadPet || !m_Mobile.CanDrop)
			{
				return true;
			}

			m_Mobile.DebugSay("I drop my stuff for my master");

			Container pack = m_Mobile.Backpack;

			if (pack != null)
			{
				var list = pack.Items;

				for (int i = list.Count - 1; i >= 0; --i)
				{
					if (i < list.Count)
					{
						list[i].MoveToWorld(m_Mobile.Location, m_Mobile.Map);
					}
				}
			}

			m_Mobile.ControlTarget = null;
			m_Mobile.ControlOrder = OrderType.None;

			return true;
		}

		public virtual bool CheckHerding()
		{
			IPoint2D target = m_Mobile.TargetLocation;

			if (target == null)
			{
				return false; // Creature is not being herded
			}

			double distance = m_Mobile.GetDistanceToSqrt(target);

			if (distance < 1 || distance > 15)
			{
				if (distance < 1 && target.X == 1076 && target.Y == 450 && (m_Mobile is HordeMinionFamiliar))
				{
					PlayerMobile pm = m_Mobile.ControlMaster as PlayerMobile;

					if (pm != null)
					{
						QuestSystem qs = pm.Quest;

						if (qs is DarkTidesQuest)
						{
							QuestObjective obj = qs.FindObjective(typeof(FetchAbraxusScrollObjective));

							if (obj != null && !obj.Completed)
							{
								m_Mobile.AddToBackpack(new ScrollOfAbraxus());
								obj.Complete();
							}
						}
					}
				}

				m_Mobile.TargetLocation = null;
				return false; // At the target or too far away
			}

			DoMove(m_Mobile.GetDirectionTo(target));

			return true;
		}

		public virtual bool DoOrderFollow()
		{
			if (CheckHerding())
			{
				m_Mobile.DebugSay("Praise the shepherd!");
			}
			else if (m_Mobile.ControlTarget != null && !m_Mobile.ControlTarget.Deleted && m_Mobile.ControlTarget != m_Mobile)
			{
				int iCurrDist = (int)m_Mobile.GetDistanceToSqrt(m_Mobile.ControlTarget);

				if (iCurrDist > m_Mobile.RangePerception)
				{
					m_Mobile.DebugSay("I have lost the one to follow. I stay here");

					if (m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive &&
						!m_Mobile.Combatant.IsDeadBondedPet)
					{
						m_Mobile.Warmode = true;
						m_Mobile.Direction = m_Mobile.GetDirectionTo(m_Mobile.Combatant);
					}
					else
					{
						m_Mobile.Warmode = false;
					}
				}
				else
				{
					m_Mobile.DebugSay("My master told me to follow: {0}", m_Mobile.ControlTarget.Name);

					// Not exactly OSI style, but better than nothing.
					bool bRun = (iCurrDist > 5);

					if (WalkMobileRange(m_Mobile.ControlTarget, 1, bRun, 0, 1))
					{
						if (m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive &&
							!m_Mobile.Combatant.IsDeadBondedPet)
						{
							m_Mobile.Warmode = true;
							m_Mobile.Direction = m_Mobile.GetDirectionTo(m_Mobile.Combatant);
						}
						else
						{
							m_Mobile.Warmode = false;
							if (Core.AOS)
							{
								m_Mobile.CurrentSpeed = 0.1;
							}
						}
					}
				}
			}
			else
			{
				m_Mobile.DebugSay("I have nobody to follow");
				m_Mobile.ControlTarget = null;
				m_Mobile.ControlOrder = OrderType.None;
			}

			return true;
		}

		public virtual bool DoOrderFriend()
		{
			Mobile from = m_Mobile.ControlMaster;
			Mobile to = m_Mobile.ControlTarget;

			if (from == null || to == null || from == to || from.Deleted || to.Deleted || !to.Player)
			{
				m_Mobile.PublicOverheadMessage(MessageType.Regular, 0x3B2, 502039); // *looks confused*
			}
			else
			{
				bool youngFrom = from is PlayerMobile ? ((PlayerMobile)from).Young : false;
				bool youngTo = to is PlayerMobile ? ((PlayerMobile)to).Young : false;

				if (youngFrom && !youngTo)
				{
					from.SendLocalizedMessage(502040); // As a young player, you may not friend pets to older players.
				}
				else if (!youngFrom && youngTo)
				{
					from.SendLocalizedMessage(502041); // As an older player, you may not friend pets to young players.
				}
				else if (from.CanBeBeneficial(to, true))
				{
					NetState fromState = from.NetState, toState = to.NetState;

					if (fromState != null && toState != null)
					{
						if (from.HasTrade)
						{
							from.SendLocalizedMessage(1070947); // You cannot friend a pet with a trade pending
						}
						else if (to.HasTrade)
						{
							to.SendLocalizedMessage(1070947); // You cannot friend a pet with a trade pending
						}
						else if (m_Mobile.IsPetFriend(to))
						{
							from.SendLocalizedMessage(1049691); // That person is already a friend.
						}
						else if (!m_Mobile.AllowNewPetFriend)
						{
							from.SendLocalizedMessage(1005482); // Your pet does not seem to be interested in making new friends right now.
						}
						else
						{
							// ~1_NAME~ will now accept movement commands from ~2_NAME~.
							from.SendLocalizedMessage(1049676, String.Format("{0}\t{1}", m_Mobile.Name, to.Name));

							/* ~1_NAME~ has granted you the ability to give orders to their pet ~2_PET_NAME~.
                            * This creature will now consider you as a friend.
                            */
							to.SendLocalizedMessage(1043246, String.Format("{0}\t{1}", from.Name, m_Mobile.Name));

							m_Mobile.AddPetFriend(to);

							m_Mobile.ControlTarget = to;
							m_Mobile.ControlOrder = OrderType.Follow;

							return true;
						}
					}
				}
			}

			m_Mobile.ControlTarget = from;
			m_Mobile.ControlOrder = OrderType.Follow;

			return true;
		}

		public virtual bool DoOrderUnfriend()
		{
			Mobile from = m_Mobile.ControlMaster;
			Mobile to = m_Mobile.ControlTarget;

			if (from == null || to == null || from == to || from.Deleted || to.Deleted || !to.Player)
			{
				m_Mobile.PublicOverheadMessage(MessageType.Regular, 0x3B2, 502039); // *looks confused*
			}
			else if (!m_Mobile.IsPetFriend(to))
			{
				from.SendLocalizedMessage(1070953); // That person is not a friend.
			}
			else
			{
				// ~1_NAME~ will no longer accept movement commands from ~2_NAME~.
				from.SendLocalizedMessage(1070951, String.Format("{0}\t{1}", m_Mobile.Name, to.Name));

				/* ~1_NAME~ has no longer granted you the ability to give orders to their pet ~2_PET_NAME~.
                * This creature will no longer consider you as a friend.
                */
				to.SendLocalizedMessage(1070952, String.Format("{0}\t{1}", from.Name, m_Mobile.Name));

				m_Mobile.RemovePetFriend(to);
			}

			m_Mobile.ControlTarget = from;
			m_Mobile.ControlOrder = OrderType.Follow;

			return true;
		}

		public virtual bool DoOrderGuard()
		{
			if (m_Mobile.IsDeadPet)
			{
				return true;
			}

			Mobile controlMaster = m_Mobile.ControlMaster;

			if (controlMaster == null || controlMaster.Deleted)
			{
				return true;
			}

			Mobile combatant = m_Mobile.Combatant;

			var aggressors = controlMaster.Aggressors;

			if (aggressors.Count > 0)
			{
				for (int i = 0; i < aggressors.Count; ++i)
				{
					AggressorInfo info = aggressors[i];
					Mobile attacker = info.Attacker;

					if (attacker != null && !attacker.Deleted && attacker.GetDistanceToSqrt(m_Mobile) <= m_Mobile.RangePerception)
					{
						if (combatant == null || attacker.GetDistanceToSqrt(controlMaster) < combatant.GetDistanceToSqrt(controlMaster))
						{
							combatant = attacker;
						}
					}
				}

				if (combatant != null)
				{
					m_Mobile.DebugSay("Crap, my master has been attacked! I will attack one of those bastards!");
				}
			}

			if (combatant != null && combatant != m_Mobile && combatant != m_Mobile.ControlMaster && !combatant.Deleted &&
				combatant.Alive && !combatant.IsDeadBondedPet && m_Mobile.CanSee(combatant) &&
				m_Mobile.CanBeHarmful(combatant, false) && combatant.Map == m_Mobile.Map)
			{
				m_Mobile.DebugSay("Guarding from target...");

				m_Mobile.Combatant = combatant;
				m_Mobile.FocusMob = combatant;
				Action = ActionType.Combat;

				/*
                * We need to call Think() here or spell casting monsters will not use
                * spells when guarding because their target is never processed.
                */
				Think();
			}
			else
			{
				m_Mobile.DebugSay("Nothing to guard from");

				m_Mobile.Warmode = false;
				if (Core.AOS)
				{
					m_Mobile.CurrentSpeed = 0.1;
				}

				WalkMobileRange(controlMaster, 1, false, 0, 1);
			}

			return true;
		}

		public virtual bool DoOrderAttack()
		{
			if (m_Mobile.IsDeadPet)
			{
				return true;
			}

			if (m_Mobile.ControlTarget == null || m_Mobile.ControlTarget.Deleted || m_Mobile.ControlTarget.Map != m_Mobile.Map ||
				!m_Mobile.ControlTarget.Alive || m_Mobile.ControlTarget.IsDeadBondedPet)
			{
				m_Mobile.DebugSay(
					"I think he might be dead. He's not anywhere around here at least. That's cool. I'm glad he's dead.");

				if (Core.AOS)
				{
					m_Mobile.ControlTarget = m_Mobile.ControlMaster;
					m_Mobile.ControlOrder = OrderType.Follow;
				}
				else
				{
					m_Mobile.ControlTarget = null;
					m_Mobile.ControlOrder = OrderType.None;
				}

				if (m_Mobile.FightMode == FightMode.Closest || m_Mobile.FightMode == FightMode.Aggressor)
				{
					Mobile newCombatant = null;
					double newScore = 0.0;

					foreach (Mobile aggr in m_Mobile.GetMobilesInRange(m_Mobile.RangePerception))
					{
						if (!m_Mobile.CanSee(aggr) || aggr.Combatant != m_Mobile)
						{
							continue;
						}

						if (aggr.IsDeadBondedPet || !aggr.Alive)
						{
							continue;
						}

						double aggrScore = m_Mobile.GetFightModeRanking(aggr, FightMode.Closest, false);

						if ((newCombatant == null || aggrScore > newScore) && m_Mobile.InLOS(aggr))
						{
							newCombatant = aggr;
							newScore = aggrScore;
						}
					}

					if (newCombatant != null)
					{
						m_Mobile.ControlTarget = newCombatant;
						m_Mobile.ControlOrder = OrderType.Attack;
						m_Mobile.Combatant = newCombatant;
						m_Mobile.DebugSay("But -that- is not dead. Here we go again...");
						Think();
					}
				}
			}
			else
			{
				m_Mobile.DebugSay("Attacking target...");
				Think();
			}

			return true;
		}

		public virtual bool DoOrderPatrol()
		{
			m_Mobile.DebugSay("This order is not yet coded");
			return true;
		}

		public virtual bool DoOrderRelease()
		{
			m_Mobile.DebugSay("I have been released");

			m_Mobile.PlaySound(m_Mobile.GetAngerSound());

			m_Mobile.SetControlMaster(null);
			m_Mobile.SummonMaster = null;

			m_Mobile.BondingBegin = DateTime.MinValue;
			m_Mobile.OwnerAbandonTime = DateTime.MinValue;
			m_Mobile.IsBonded = false;

			SpawnEntry se = m_Mobile.Spawner as SpawnEntry;
			if (se != null && se.HomeLocation != Point3D.Zero)
			{
				m_Mobile.Home = se.HomeLocation;
				m_Mobile.RangeHome = se.HomeRange;
			}

			if (m_Mobile.DeleteOnRelease || m_Mobile.IsDeadPet)
			{
				m_Mobile.Delete();
			}

			m_Mobile.BeginDeleteTimer();
			m_Mobile.DropBackpack();

			return true;
		}

		public virtual bool DoOrderStay()
		{
			if (CheckHerding())
			{
				m_Mobile.DebugSay("Praise the shepherd!");
			}
			else
			{
				m_Mobile.DebugSay("My master told me to stay");
			}

			//m_Mobile.Direction = m_Mobile.GetDirectionTo( m_Mobile.ControlMaster );

			return true;
		}

		public virtual bool DoOrderStop()
		{
			if (m_Mobile.ControlMaster == null || m_Mobile.ControlMaster.Deleted)
			{
				return true;
			}

			m_Mobile.DebugSay("My master told me to stop.");

			m_Mobile.Direction = m_Mobile.GetDirectionTo(m_Mobile.ControlMaster);
			m_Mobile.Home = m_Mobile.Location;

			m_Mobile.ControlTarget = null;

			if (Core.ML)
			{
				WalkRandomInHome(3, 2, 1);
			}
			else
			{
				m_Mobile.ControlOrder = OrderType.None;
			}

			return true;
		}

		private class TransferItem : Item
		{
			public static bool IsInCombat(BaseCreature creature)
			{
				return (creature != null && (creature.Aggressors.Count > 0 || creature.Aggressed.Count > 0));
			}

			private readonly BaseCreature m_Creature;

			public TransferItem(BaseCreature creature)
				: base(ShrinkTable.Lookup(creature))
			{
				m_Creature = creature;

				Movable = false;

				if (!Core.AOS)
				{
					Name = creature.Name;
				}
				else if (ItemID == ShrinkTable.DefaultItemID || creature.GetType().IsDefined(typeof(FriendlyNameAttribute), false) ||
						 creature is Reptalon)
				{
					Name = FriendlyNameAttribute.GetFriendlyNameFor(creature.GetType()).ToString();
				}

				//(As Per OSI)No name.  Normally, set by the ItemID of the Shrink Item unless we either explicitly set it with an Attribute, or, no lookup found

				Hue = creature.Hue & 0x0FFF;
			}

			public TransferItem(Serial serial)
				: base(serial)
			{ }

			public override void Serialize(GenericWriter writer)
			{
				base.Serialize(writer);

				writer.Write(0); // version
			}

			public override void Deserialize(GenericReader reader)
			{
				base.Deserialize(reader);

				int version = reader.ReadInt();

				Delete();
			}

			public override void GetProperties(ObjectPropertyList list)
			{
				base.GetProperties(list);

				list.Add(1041603); // This item represents a pet currently in consideration for trade
				list.Add(1041601, m_Creature.Name); // Pet Name: ~1_val~

				if (m_Creature.ControlMaster != null)
				{
					list.Add(1041602, m_Creature.ControlMaster.Name); // Owner: ~1_val~
				}
			}

			public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
			{
				if (!base.AllowSecureTrade(from, to, newOwner, accepted))
				{
					return false;
				}

				if (Deleted || m_Creature == null || m_Creature.Deleted || m_Creature.ControlMaster != from || !from.CheckAlive() ||
					!to.CheckAlive())
				{
					return false;
				}

				if (from.Map != m_Creature.Map || !from.InRange(m_Creature, 14))
				{
					return false;
				}

				bool youngFrom = from is PlayerMobile ? ((PlayerMobile)from).Young : false;
				bool youngTo = to is PlayerMobile ? ((PlayerMobile)to).Young : false;

				if (accepted && youngFrom && !youngTo)
				{
					from.SendLocalizedMessage(502051); // As a young player, you may not transfer pets to older players.
				}
				else if (accepted && !youngFrom && youngTo)
				{
					from.SendLocalizedMessage(502052); // As an older player, you may not transfer pets to young players.
				}
				else if (accepted && !m_Creature.CanBeControlledBy(to))
				{
					string args = String.Format("{0}\t{1}\t ", to.Name, from.Name);

					from.SendLocalizedMessage(1043248, args);
					// The pet refuses to be transferred because it will not obey ~1_NAME~.~3_BLANK~
					to.SendLocalizedMessage(1043249, args);
					// The pet will not accept you as a master because it does not trust you.~3_BLANK~

					return false;
				}
				else if (accepted && !m_Creature.CanBeControlledBy(from))
				{
					string args = String.Format("{0}\t{1}\t ", to.Name, from.Name);

					from.SendLocalizedMessage(1043250, args);
					// The pet refuses to be transferred because it will not obey you sufficiently.~3_BLANK~
					to.SendLocalizedMessage(1043251, args);
					// The pet will not accept you as a master because it does not trust ~2_NAME~.~3_BLANK~
				}
				else if (accepted && (to.Followers + m_Creature.ControlSlots) > to.FollowersMax)
				{
					to.SendLocalizedMessage(1049607); // You have too many followers to control that creature.

					return false;
				}
				else if (accepted && IsInCombat(m_Creature))
				{
					from.SendMessage("You may not transfer a pet that has recently been in combat.");
					to.SendMessage("The pet may not be transfered to you because it has recently been in combat.");

					return false;
				}

				return true;
			}

			public override void OnSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
			{
				if (Deleted)
				{
					return;
				}

				Delete();

				if (m_Creature == null || m_Creature.Deleted || m_Creature.ControlMaster != from || !from.CheckAlive() ||
					!to.CheckAlive())
				{
					return;
				}

				if (from.Map != m_Creature.Map || !from.InRange(m_Creature, 14))
				{
					return;
				}

				if (accepted)
				{
					if (m_Creature.SetControlMaster(to))
					{
						if (m_Creature.Summoned)
						{
							m_Creature.SummonMaster = to;
						}

						m_Creature.ControlTarget = to;
						m_Creature.ControlOrder = OrderType.Follow;

						m_Creature.BondingBegin = DateTime.MinValue;
						m_Creature.OwnerAbandonTime = DateTime.MinValue;
						m_Creature.IsBonded = false;

						m_Creature.PlaySound(m_Creature.GetIdleSound());

						string args = String.Format("{0}\t{1}\t{2}", from.Name, m_Creature.Name, to.Name);

						from.SendLocalizedMessage(1043253, args); // You have transferred your pet to ~3_GETTER~.
						to.SendLocalizedMessage(1043252, args); // ~1_NAME~ has transferred the allegiance of ~2_PET_NAME~ to you.
					}
				}
			}
		}

		public virtual bool DoOrderTransfer()
		{
			if (m_Mobile.IsDeadPet)
			{
				return true;
			}

			Mobile from = m_Mobile.ControlMaster;
			Mobile to = m_Mobile.ControlTarget;

			if (from != to && from != null && !from.Deleted && to != null && !to.Deleted && to.Player)
			{
				m_Mobile.DebugSay("Begin transfer with {0}", to.Name);

				bool youngFrom = from is PlayerMobile ? ((PlayerMobile)from).Young : false;
				bool youngTo = to is PlayerMobile ? ((PlayerMobile)to).Young : false;

				if (youngFrom && !youngTo)
				{
					from.SendLocalizedMessage(502051); // As a young player, you may not transfer pets to older players.
				}
				else if (!youngFrom && youngTo)
				{
					from.SendLocalizedMessage(502052); // As an older player, you may not transfer pets to young players.
				}
				else if (!m_Mobile.CanBeControlledBy(to))
				{
					string args = String.Format("{0}\t{1}\t ", to.Name, from.Name);

					from.SendLocalizedMessage(1043248, args);
					// The pet refuses to be transferred because it will not obey ~1_NAME~.~3_BLANK~
					to.SendLocalizedMessage(1043249, args);
					// The pet will not accept you as a master because it does not trust you.~3_BLANK~
				}
				else if (!m_Mobile.CanBeControlledBy(from))
				{
					string args = String.Format("{0}\t{1}\t ", to.Name, from.Name);

					from.SendLocalizedMessage(1043250, args);
					// The pet refuses to be transferred because it will not obey you sufficiently.~3_BLANK~
					to.SendLocalizedMessage(1043251, args);
					// The pet will not accept you as a master because it does not trust ~2_NAME~.~3_BLANK~
				}
				else if (TransferItem.IsInCombat(m_Mobile))
				{
					from.SendMessage("You may not transfer a pet that has recently been in combat.");
					to.SendMessage("The pet may not be transfered to you because it has recently been in combat.");
				}
				else
				{
					NetState fromState = from.NetState, toState = to.NetState;

					if (fromState != null && toState != null)
					{
						if (from.HasTrade)
						{
							from.SendLocalizedMessage(1010507); // You cannot transfer a pet with a trade pending
						}
						else if (to.HasTrade)
						{
							to.SendLocalizedMessage(1010507); // You cannot transfer a pet with a trade pending
						}
						else
						{
							Container c = fromState.AddTrade(toState);
							c.DropItem(new TransferItem(m_Mobile));
						}
					}
				}
			}

			m_Mobile.ControlTarget = null;
			m_Mobile.ControlOrder = OrderType.Stay;

			return true;
		}

		public virtual bool DoBardPacified()
		{
			if (DateTime.UtcNow < m_Mobile.BardEndTime)
			{
				m_Mobile.DebugSay("I am pacified, I wait");
				m_Mobile.Combatant = null;
				m_Mobile.Warmode = false;
			}
			else
			{
				m_Mobile.DebugSay("I'm not pacified any longer");
				m_Mobile.BardPacified = false;
			}

			return true;
		}

		public virtual bool DoBardProvoked()
		{
			if (DateTime.UtcNow >= m_Mobile.BardEndTime &&
				(m_Mobile.BardMaster == null || m_Mobile.BardMaster.Deleted || m_Mobile.BardMaster.Map != m_Mobile.Map ||
				 m_Mobile.GetDistanceToSqrt(m_Mobile.BardMaster) > m_Mobile.RangePerception))
			{
				m_Mobile.DebugSay("I have lost my provoker");
				m_Mobile.BardProvoked = false;
				m_Mobile.BardMaster = null;
				m_Mobile.BardTarget = null;

				m_Mobile.Combatant = null;
				m_Mobile.Warmode = false;
			}
			else
			{
				if (m_Mobile.BardTarget == null || m_Mobile.BardTarget.Deleted || m_Mobile.BardTarget.Map != m_Mobile.Map ||
					m_Mobile.GetDistanceToSqrt(m_Mobile.BardTarget) > m_Mobile.RangePerception)
				{
					m_Mobile.DebugSay("I have lost my provoke target");
					m_Mobile.BardProvoked = false;
					m_Mobile.BardMaster = null;
					m_Mobile.BardTarget = null;

					m_Mobile.Combatant = null;
					m_Mobile.Warmode = false;
				}
				else
				{
					m_Mobile.Combatant = m_Mobile.BardTarget;
					m_Action = ActionType.Combat;

					m_Mobile.OnThink();
					Think();
				}
			}

			return true;
		}

		public virtual void WalkRandom(int iChanceToNotMove, int iChanceToDir, int iSteps)
		{
			if (m_Mobile.Deleted || m_Mobile.DisallowAllMoves)
			{
				return;
			}

			for (int i = 0; i < iSteps; i++)
			{
				if (Utility.Random(8 * iChanceToNotMove) <= 8)
				{
					int iRndMove = Utility.Random(0, 8 + (9 * iChanceToDir));

					switch (iRndMove)
					{
						case 0:
							DoMove(Direction.Up);
							break;
						case 1:
							DoMove(Direction.North);
							break;
						case 2:
							DoMove(Direction.Left);
							break;
						case 3:
							DoMove(Direction.West);
							break;
						case 5:
							DoMove(Direction.Down);
							break;
						case 6:
							DoMove(Direction.South);
							break;
						case 7:
							DoMove(Direction.Right);
							break;
						case 8:
							DoMove(Direction.East);
							break;
						default:
							DoMove(m_Mobile.Direction);
							break;
					}
				}
			}
		}

		public double TransformMoveDelay(double delay)
		{
			bool isPassive = (delay == m_Mobile.PassiveSpeed);
			bool isControlled = (m_Mobile.Controlled || m_Mobile.Summoned);

			if (delay == 0.2)
			{
				delay = 0.3;
			}
			else if (delay == 0.25)
			{
				delay = 0.45;
			}
			else if (delay == 0.3)
			{
				delay = 0.6;
			}
			else if (delay == 0.4)
			{
				delay = 0.9;
			}
			else if (delay == 0.5)
			{
				delay = 1.05;
			}
			else if (delay == 0.6)
			{
				delay = 1.2;
			}
			else if (delay == 0.8)
			{
				delay = 1.5;
			}

			if (isPassive)
			{
				delay += 0.2;
			}

			if (!isControlled)
			{
				delay += 0.1;
			}
			else if (m_Mobile.Controlled)
			{
				if (m_Mobile.ControlOrder == OrderType.Follow && m_Mobile.ControlTarget == m_Mobile.ControlMaster)
				{
					delay *= 0.5;
				}

				delay -= 0.075;
			}

			double speedfactor = 0.8;

			XmlValue a = (XmlValue)XmlAttach.FindAttachment(m_Mobile, typeof(XmlValue), "DamagedSpeedFactor");

			if (a != null)
			{
				speedfactor = a.Value / 100.0;
			}

			if (m_Mobile.ReduceSpeedWithDamage || m_Mobile.IsSubdued)
			{
				double offset = (double)m_Mobile.Hits / m_Mobile.HitsMax;

				if (offset < 0.0)
				{
					offset = 0.0;
				}
				else if (offset > 1.0)
				{
					offset = 1.0;
				}

				offset = 1.0 - offset;

				delay += (offset * speedfactor);
			}

			if (delay < 0.0)
			{
				delay = 0.0;
			}

			if (double.IsNaN(delay))
			{
				using (StreamWriter op = new StreamWriter("nan_transform.txt", true))
				{
					op.WriteLine(
						String.Format(
							"NaN in TransformMoveDelay: {0}, {1}, {2}, {3}",
							DateTime.UtcNow,
							GetType(),
							m_Mobile == null ? "null" : m_Mobile.GetType().ToString(),
							m_Mobile.HitsMax));
				}

				return 1.0;
			}

			return delay;
		}

		private long m_NextMove;

		public long NextMove { get { return m_NextMove; } set { m_NextMove = value; } }

		public virtual bool CheckMove()
		{
			return (Core.TickCount - m_NextMove >= 0);
		}

		public virtual bool DoMove(Direction d)
		{
			return DoMove(d, false);
		}

		public virtual bool DoMove(Direction d, bool badStateOk)
		{
			MoveResult res = DoMoveImpl(d);

			return (res == MoveResult.Success || res == MoveResult.SuccessAutoTurn || (badStateOk && res == MoveResult.BadState));
		}

		private static readonly Queue<Item> m_Obstacles = new Queue<Item>();

		public virtual MoveResult DoMoveImpl(Direction d)
		{
			if (m_Mobile.Deleted || m_Mobile.Frozen || m_Mobile.Paralyzed || (m_Mobile.Spell != null && m_Mobile.Spell.IsCasting) ||
				m_Mobile.DisallowAllMoves)
			{
				return MoveResult.BadState;
			}
			else if (!CheckMove())
			{
				return MoveResult.BadState;
			}

			// This makes them always move one step, never any direction changes
			m_Mobile.Direction = d;

			int delay = (int)(TransformMoveDelay(m_Mobile.CurrentSpeed) * 1000);

			m_NextMove += delay;

			if (Core.TickCount - m_NextMove > 0)
			{
				m_NextMove = Core.TickCount;
			}

			m_Mobile.Pushing = false;

			MoveImpl.IgnoreMovableImpassables = (m_Mobile.CanMoveOverObstacles && !m_Mobile.CanDestroyObstacles);

			if ((m_Mobile.Direction & Direction.Mask) != (d & Direction.Mask))
			{
				bool v = m_Mobile.Move(d);

				MoveImpl.IgnoreMovableImpassables = false;
				return (v ? MoveResult.Success : MoveResult.Blocked);
			}
			else if (!m_Mobile.Move(d))
			{
				bool wasPushing = m_Mobile.Pushing;

				bool blocked = true;

				bool canOpenDoors = m_Mobile.CanOpenDoors;
				bool canDestroyObstacles = m_Mobile.CanDestroyObstacles;

				if (canOpenDoors || canDestroyObstacles)
				{
					m_Mobile.DebugSay("My movement was blocked, I will try to clear some obstacles.");

					Map map = m_Mobile.Map;

					if (map != null)
					{
						int x = m_Mobile.X, y = m_Mobile.Y;
						Movement.Movement.Offset(d, ref x, ref y);

						int destroyables = 0;

						IPooledEnumerable eable = map.GetItemsInRange(new Point3D(x, y, m_Mobile.Location.Z), 1);

						foreach (Item item in eable)
						{
							if (canOpenDoors && item is BaseDoor && (item.Z + item.ItemData.Height) > m_Mobile.Z &&
								(m_Mobile.Z + 16) > item.Z)
							{
								if (item.X != x || item.Y != y)
								{
									continue;
								}

								BaseDoor door = (BaseDoor)item;

								if (!door.Locked || !door.UseLocks())
								{
									m_Obstacles.Enqueue(door);
								}

								if (!canDestroyObstacles)
								{
									break;
								}
							}
							else if (canDestroyObstacles && item.Movable && item.ItemData.Impassable &&
									 (item.Z + item.ItemData.Height) > m_Mobile.Z && (m_Mobile.Z + 16) > item.Z)
							{
								if (!m_Mobile.InRange(item.GetWorldLocation(), 1))
								{
									continue;
								}

								m_Obstacles.Enqueue(item);
								++destroyables;
							}
						}

						eable.Free();

						if (destroyables > 0)
						{
							Effects.PlaySound(new Point3D(x, y, m_Mobile.Z), m_Mobile.Map, 0x3B3);
						}

						if (m_Obstacles.Count > 0)
						{
							blocked = false; // retry movement
						}

						while (m_Obstacles.Count > 0)
						{
							Item item = m_Obstacles.Dequeue();

							if (item is BaseDoor)
							{
								m_Mobile.DebugSay("Little do they expect, I've learned how to open doors. Didn't they read the script??");
								m_Mobile.DebugSay("*twist*");

								((BaseDoor)item).Use(m_Mobile);
							}
							else
							{
								m_Mobile.DebugSay("Ugabooga. I'm so big and tough I can destroy it: {0}", item.GetType().Name);

								if (item is Container)
								{
									Container cont = (Container)item;

									for (int i = 0; i < cont.Items.Count; ++i)
									{
										Item check = cont.Items[i];

										if (check.Movable && check.ItemData.Impassable && (item.Z + check.ItemData.Height) > m_Mobile.Z)
										{
											m_Obstacles.Enqueue(check);
										}
									}

									cont.Destroy();
								}
								else
								{
									item.Delete();
								}
							}
						}

						if (!blocked)
						{
							blocked = !m_Mobile.Move(d);
						}
					}
				}

				if (blocked)
				{
					int offset = (Utility.RandomDouble() >= 0.6 ? 1 : -1);

					for (int i = 0; i < 2; ++i)
					{
						m_Mobile.TurnInternal(offset);

						if (m_Mobile.Move(m_Mobile.Direction))
						{
							MoveImpl.IgnoreMovableImpassables = false;
							return MoveResult.SuccessAutoTurn;
						}
					}

					MoveImpl.IgnoreMovableImpassables = false;
					return (wasPushing ? MoveResult.BadState : MoveResult.Blocked);
				}
				else
				{
					MoveImpl.IgnoreMovableImpassables = false;
					return MoveResult.Success;
				}
			}

			MoveImpl.IgnoreMovableImpassables = false;
			return MoveResult.Success;
		}

		public virtual void WalkRandomInHome(int iChanceToNotMove, int iChanceToDir, int iSteps)
		{
			if (m_Mobile.Deleted || m_Mobile.DisallowAllMoves)
			{
				return;
			}

			if (m_Mobile.Home == Point3D.Zero)
			{
				if (m_Mobile.Spawner is SpawnEntry)
				{
					Region region = ((SpawnEntry)m_Mobile.Spawner).Region;

					if (m_Mobile.Region.AcceptsSpawnsFrom(region))
					{
						m_Mobile.WalkRegion = region;
						WalkRandom(iChanceToNotMove, iChanceToDir, iSteps);
						m_Mobile.WalkRegion = null;
					}
					else
					{
						if (region.GoLocation != Point3D.Zero && Utility.Random(10) > 5)
						{
							DoMove(m_Mobile.GetDirectionTo(region.GoLocation));
						}
						else
						{
							WalkRandom(iChanceToNotMove, iChanceToDir, 1);
						}
					}
				}
				else
				{
					WalkRandom(iChanceToNotMove, iChanceToDir, iSteps);
				}
			}
			else
			{
				for (int i = 0; i < iSteps; i++)
				{
					if (m_Mobile.RangeHome != 0)
					{
						int iCurrDist = (int)m_Mobile.GetDistanceToSqrt(m_Mobile.Home);

						if (iCurrDist < m_Mobile.RangeHome * 2 / 3)
						{
							WalkRandom(iChanceToNotMove, iChanceToDir, 1);
						}
						else if (iCurrDist > m_Mobile.RangeHome)
						{
							DoMove(m_Mobile.GetDirectionTo(m_Mobile.Home));
						}
						else
						{
							if (Utility.Random(10) > 5)
							{
								DoMove(m_Mobile.GetDirectionTo(m_Mobile.Home));
							}
							else
							{
								WalkRandom(iChanceToNotMove, iChanceToDir, 1);
							}
						}
					}
					else
					{
						if (m_Mobile.Location != m_Mobile.Home)
						{
							DoMove(m_Mobile.GetDirectionTo(m_Mobile.Home));
						}
					}
				}
			}
		}

		public virtual bool CheckFlee()
		{
			if (m_Mobile.CheckFlee())
			{
				Mobile combatant = m_Mobile.Combatant;

				if (combatant == null)
				{
					WalkRandom(1, 2, 1);
				}
				else
				{
					Direction d = combatant.GetDirectionTo(m_Mobile);

					d = (Direction)((int)d + Utility.RandomMinMax(-1, +1));

					m_Mobile.Direction = d;
					m_Mobile.Move(d);
				}

				return true;
			}

			return false;
		}

		protected PathFollower m_Path;

		public virtual void OnTeleported()
		{
			if (m_Path != null)
			{
				m_Mobile.DebugSay("Teleported; repathing");
				m_Path.ForceRepath();
			}
		}

		public virtual bool MoveTo(Mobile m, bool run, int range)
		{
			if (m_Mobile.Deleted || m_Mobile.DisallowAllMoves || m == null || m.Deleted)
			{
				return false;
			}

			if (m_Mobile.InRange(m, range))
			{
				m_Path = null;
				return true;
			}

			if (m_Path != null && m_Path.Goal == m)
			{
				if (m_Path.Follow(run, 1))
				{
					m_Path = null;
					return true;
				}
			}
			else if (!DoMove(m_Mobile.GetDirectionTo(m), true))
			{
				m_Path = new PathFollower(m_Mobile, m);
				m_Path.Mover = DoMoveImpl;

				if (m_Path.Follow(run, 1))
				{
					m_Path = null;
					return true;
				}
			}
			else
			{
				m_Path = null;
				return true;
			}

			return false;
		}

		/*
        *  Walk at range distance from mobile
        * 
        *	iSteps : Number of steps
        *	bRun   : Do we run
        *	iWantDistMin : The minimum distance we want to be
        *  iWantDistMax : The maximum distance we want to be
        * 
        */

		public virtual bool WalkMobileRange(Mobile m, int iSteps, bool bRun, int iWantDistMin, int iWantDistMax)
		{
			if (m_Mobile.Deleted || m_Mobile.DisallowAllMoves)
			{
				return false;
			}

			if (m != null)
			{
				for (int i = 0; i < iSteps; i++)
				{
					// Get the curent distance
					int iCurrDist = (int)m_Mobile.GetDistanceToSqrt(m);

					if (iCurrDist < iWantDistMin || iCurrDist > iWantDistMax)
					{
						bool needCloser = (iCurrDist > iWantDistMax);
						bool needFurther = !needCloser;

						if (needCloser && m_Path != null && m_Path.Goal == m)
						{
							if (m_Path.Follow(bRun, 1))
							{
								m_Path = null;
							}
						}
						else
						{
							Direction dirTo;

							if (iCurrDist > iWantDistMax)
							{
								dirTo = m_Mobile.GetDirectionTo(m);
							}
							else
							{
								dirTo = m.GetDirectionTo(m_Mobile);
							}

							// Add the run flag
							if (bRun)
							{
								dirTo = dirTo | Direction.Running;
							}

							if (!DoMove(dirTo, true) && needCloser)
							{
								m_Path = new PathFollower(m_Mobile, m);
								m_Path.Mover = DoMoveImpl;

								if (m_Path.Follow(bRun, 1))
								{
									m_Path = null;
								}
							}
							else
							{
								m_Path = null;
							}
						}
					}
					else
					{
						return true;
					}
				}

				// Get the curent distance
				int iNewDist = (int)m_Mobile.GetDistanceToSqrt(m);

				if (iNewDist >= iWantDistMin && iNewDist <= iWantDistMax)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			return false;
		}

		/*
        * Here we check to acquire a target from our surronding
        * 
        *  iRange : The range
        *  acqType : A type of acquire we want (closest, strongest, etc)
        *  bPlayerOnly : Don't bother with other creatures or NPCs, want a player
        *  bFacFriend : Check people in my faction
        *  bFacFoe : Check people in other factions
        * 
        */

		public virtual bool AcquireFocusMob(int iRange, FightMode acqType, bool bPlayerOnly, bool bFacFriend, bool bFacFoe)
		{
			if (m_Mobile.Deleted)
			{
				return false;
			}

			if (m_Mobile.BardProvoked)
			{
				if (m_Mobile.BardTarget == null || m_Mobile.BardTarget.Deleted)
				{
					m_Mobile.FocusMob = null;
					return false;
				}
				else
				{
					m_Mobile.FocusMob = m_Mobile.BardTarget;
					return (m_Mobile.FocusMob != null);
				}
			}
			else if (m_Mobile.Controlled)
			{
				if (m_Mobile.ControlTarget == null || m_Mobile.ControlTarget.Deleted || m_Mobile.ControlTarget.Hidden ||
					!m_Mobile.ControlTarget.Alive || m_Mobile.ControlTarget.IsDeadBondedPet ||
					!m_Mobile.InRange(m_Mobile.ControlTarget, m_Mobile.RangePerception * 2))
				{
					if (m_Mobile.ControlTarget != null && m_Mobile.ControlTarget != m_Mobile.ControlMaster)
					{
						m_Mobile.ControlTarget = null;
					}

					m_Mobile.FocusMob = null;
					return false;
				}
				else
				{
					m_Mobile.FocusMob = m_Mobile.ControlTarget;
					return (m_Mobile.FocusMob != null);
				}
			}

			if (m_Mobile.ConstantFocus != null)
			{
				m_Mobile.DebugSay("Acquired my constant focus");
				m_Mobile.FocusMob = m_Mobile.ConstantFocus;
				return true;
			}

			if (acqType == FightMode.None)
			{
				m_Mobile.FocusMob = null;
				return false;
			}

			if (acqType == FightMode.Aggressor && m_Mobile.Aggressors.Count == 0 && m_Mobile.Aggressed.Count == 0 &&
				m_Mobile.FactionAllegiance == null && m_Mobile.EthicAllegiance == null)
			{
				m_Mobile.FocusMob = null;
				return false;
			}

			if (m_Mobile.NextReacquireTime > Core.TickCount)
			{
				m_Mobile.FocusMob = null;
				return false;
			}

			m_Mobile.NextReacquireTime = Core.TickCount + (int)m_Mobile.ReacquireDelay.TotalMilliseconds;

			m_Mobile.DebugSay("Acquiring...");

			Map map = m_Mobile.Map;

			if (map != null)
			{
				Mobile newFocusMob = null;
				double val = double.MinValue;
				double theirVal;

				var eable = map.GetMobilesInRange(m_Mobile.Location, iRange);

				foreach (Mobile m in eable)
				{
					if (m.Deleted || m.Blessed)
					{
						continue;
					}

					// Let's not target ourselves...
					if (m == m_Mobile || m is BaseFamiliar)
					{
						continue;
					}

					// Dead targets are invalid.
					if (!m.Alive || m.IsDeadBondedPet)
					{
						continue;
					}

					// Staff members cannot be targeted.
					if (m.IsStaff())
					{
						continue;
					}

					// Does it have to be a player?
					if (bPlayerOnly && !m.Player)
					{
						continue;
					}

					// Can't acquire a target we can't see.
					if (!m_Mobile.CanSee(m))
					{
						continue;
					}

					// Xmlspawner faction check
					//if (!Server.Engines.XmlSpawner2.XmlMobFactions.CheckAcquire(this.m_Mobile, m))
					//continue;

					if (Core.AOS && m is BaseCreature && (m as BaseCreature).Summoned && !(m as BaseCreature).Controlled)
					{
						continue;
					}

					if (m_Mobile.Summoned && m_Mobile.SummonMaster != null)
					{
						// If this is a summon, it can't target its controller.
						if (m == m_Mobile.SummonMaster)
						{
							continue;
						}

						// It also must abide by harmful spell rules.
						if (!SpellHelper.ValidIndirectTarget(m_Mobile.SummonMaster, m))
						{
							continue;
						}

						// Animated creatures cannot attack players directly.
						if (m is PlayerMobile && m_Mobile.IsAnimatedDead)
						{
							continue;
						}
					}

					// If we only want faction friends, make sure it's one.
					if (bFacFriend && !m_Mobile.IsFriend(m))
					{
						continue;
					}

					//Ignore anyone under EtherealVoyage
					if (TransformationSpellHelper.UnderTransformation(m, typeof(EtherealVoyageSpell)))
					{
						continue;
					}

					// Ignore players with activated honor
					if (m is PlayerMobile && ((PlayerMobile)m).HonorActive && !(m_Mobile.Combatant == m))
					{
						continue;
					}

					if (acqType == FightMode.Aggressor || acqType == FightMode.Evil ||
						(m is BaseCreature) && ((BaseCreature)m).Summoned || acqType == FightMode.Good)
					{
						bool bValid = IsHostile(m);

						if (!bValid)
						{
							bValid = (m_Mobile.GetFactionAllegiance(m) == BaseCreature.Allegiance.Enemy ||
									  m_Mobile.GetEthicAllegiance(m) == BaseCreature.Allegiance.Enemy);
						}

						if (acqType == FightMode.Evil && !bValid)
						{
							if (m is BaseCreature && ((BaseCreature)m).Controlled && ((BaseCreature)m).ControlMaster != null)
							{
								bValid = (((BaseCreature)m).ControlMaster.Karma < 0);
							}
							else
							{
								bValid = (m.Karma < 0);
							}
						}

                        if (acqType == FightMode.Good && !bValid)
                        {
                            if (m is BaseCreature && ((BaseCreature)m).Controlled && ((BaseCreature)m).ControlMaster != null)
                            {
                                bValid = (((BaseCreature)m).ControlMaster.Karma > 0);
                            }
                            else
                            {
                                bValid = (m.Karma > 0);
                            }
                        }

						if (!bValid)
						{
							continue;
						}
					}
					else
					{
						// Same goes for faction enemies.
						if (bFacFoe && !m_Mobile.IsEnemy(m))
						{
							continue;
						}

						// If it's an enemy factioned mobile, make sure we can be harmful to it.
						if (bFacFoe && !bFacFriend && !m_Mobile.CanBeHarmful(m, false))
						{
							continue;
						}
					}

					theirVal = m_Mobile.GetFightModeRanking(m, acqType, bPlayerOnly);

					if (theirVal > val && m_Mobile.InLOS(m))
					{
						newFocusMob = m;
						val = theirVal;
					}
				}

				eable.Free();

				m_Mobile.FocusMob = newFocusMob;
			}

			return (m_Mobile.FocusMob != null);
		}

		private bool IsHostile(Mobile from)
		{
			int count = Math.Max(m_Mobile.Aggressors.Count, m_Mobile.Aggressed.Count);

			if (m_Mobile.Combatant == from || from.Combatant == m_Mobile)
			{
				return true;
			}

			if (count > 0)
			{
				for (int a = 0; a < count; ++a)
				{
					if (a < m_Mobile.Aggressed.Count && m_Mobile.Aggressed[a].Attacker == from)
					{
						return true;
					}

					if (a < m_Mobile.Aggressors.Count && m_Mobile.Aggressors[a].Defender == from)
					{
						return true;
					}
				}
			}

			return false;
		}

		public virtual void DetectHidden()
		{
			if (m_Mobile.Deleted || m_Mobile.Map == null)
			{
				return;
			}

			m_Mobile.DebugSay("Checking for hidden players");

			double srcSkill = m_Mobile.Skills[SkillName.DetectHidden].Value;

			if (srcSkill <= 0)
			{
				return;
			}

			IPooledEnumerable eable = m_Mobile.GetMobilesInRange(m_Mobile.RangePerception);
			foreach (Mobile trg in eable)
			{
				if (trg != m_Mobile && trg.Player && trg.Alive && trg.Hidden && trg.IsPlayer() && m_Mobile.InLOS(trg))
				{
					m_Mobile.DebugSay("Trying to detect {0}", trg.Name);

					double trgHiding = trg.Skills[SkillName.Hiding].Value / 2.9;
					double trgStealth = trg.Skills[SkillName.Stealth].Value / 1.8;

					double chance = srcSkill / 1.2 - Math.Min(trgHiding, trgStealth);

					if (chance < srcSkill / 10)
					{
						chance = srcSkill / 10;
					}

					chance /= 100;

					if (chance > Utility.RandomDouble())
					{
						trg.RevealingAction();
						trg.SendLocalizedMessage(500814); // You have been revealed!
					}
				}
			}
			eable.Free();
		}

		public virtual void Deactivate()
		{
			if (m_Mobile.PlayerRangeSensitive)
			{
				m_Timer.Stop();

				SpawnEntry se = m_Mobile.Spawner as SpawnEntry;

				if (se != null && se.ReturnOnDeactivate && !m_Mobile.Controlled)
				{
					if (se.HomeLocation == Point3D.Zero)
					{
						if (!m_Mobile.Region.AcceptsSpawnsFrom(se.Region))
						{
							Timer.DelayCall(TimeSpan.Zero, ReturnToHome);
						}
					}
					else if (!m_Mobile.InRange(se.HomeLocation, se.HomeRange))
					{
						Timer.DelayCall(TimeSpan.Zero, ReturnToHome);
					}
				}
			}
		}

		private void ReturnToHome()
		{
			SpawnEntry se = m_Mobile.Spawner as SpawnEntry;

			if (se != null)
			{
				Point3D loc = se.RandomSpawnLocation(16, !m_Mobile.CantWalk, m_Mobile.CanSwim);

				if (loc != Point3D.Zero)
				{
					m_Mobile.MoveToWorld(loc, se.Region.Map);
					return;
				}
			}
		}

		public virtual void Activate()
		{
			if (!m_Timer.Running)
			{
				m_Timer.Delay = TimeSpan.Zero;
				m_Timer.Start();
			}
		}

		/*
        *  The mobile changed it speed, we must ajust the timer
        */

		public virtual void OnCurrentSpeedChanged()
		{
			m_Timer.Stop();
			m_Timer.Delay = TimeSpan.FromSeconds(Utility.RandomDouble());
			m_Timer.Interval = TimeSpan.FromSeconds(Math.Max(0.0, m_Mobile.CurrentSpeed));
			m_Timer.Start();
		}

		private long m_NextDetectHidden;

		public virtual bool CanDetectHidden { get { return m_Mobile.Skills[SkillName.DetectHidden].Value > 0; } }

		/*
        *  The Timer object
        */

		private class AITimer : Timer
		{
			private readonly BaseAI m_Owner;

			public AITimer(BaseAI owner)
				: base(
					TimeSpan.FromSeconds(Utility.RandomDouble()), TimeSpan.FromSeconds(Math.Max(0.0, owner.m_Mobile.CurrentSpeed)))
			{
				m_Owner = owner;

				m_Owner.m_NextDetectHidden = Core.TickCount;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if (m_Owner.m_Mobile.Deleted)
				{
					Stop();
					return;
				}
				else if (m_Owner.m_Mobile.Map == null || m_Owner.m_Mobile.Map == Map.Internal)
				{
					m_Owner.Deactivate();
					return;
				}
				else if (m_Owner.m_Mobile.PlayerRangeSensitive) //have to check this in the timer....
				{
					Sector sect = m_Owner.m_Mobile.Map.GetSector(m_Owner.m_Mobile);
					if (!sect.Active)
					{
						m_Owner.Deactivate();
						return;
					}
				}

				m_Owner.m_Mobile.OnThink();

				if (m_Owner.m_Mobile.Deleted)
				{
					Stop();
					return;
				}
				else if (m_Owner.m_Mobile.Map == null || m_Owner.m_Mobile.Map == Map.Internal)
				{
					m_Owner.Deactivate();
					return;
				}

				if (m_Owner.m_Mobile.BardPacified)
				{
					m_Owner.DoBardPacified();
				}
				else if (m_Owner.m_Mobile.BardProvoked)
				{
					m_Owner.DoBardProvoked();
				}
				else
				{
					if (!m_Owner.m_Mobile.Controlled)
					{
						if (!m_Owner.Think())
						{
							Stop();
							return;
						}
					}
					else
					{
						if (!m_Owner.Obey())
						{
							Stop();
							return;
						}
					}
				}

				if (m_Owner.CanDetectHidden && Core.TickCount - m_Owner.m_NextDetectHidden >= 0)
				{
					m_Owner.DetectHidden();

					// Not exactly OSI style, approximation.
					int delay = (15000 / m_Owner.m_Mobile.Int);

					if (delay > 60)
					{
						delay = 60;
					}

					int min = delay * (9 / 10); // 13s at 1000 int, 33s at 400 int, 54s at <250 int
					int max = delay * (10 / 9); // 16s at 1000 int, 41s at 400 int, 66s at <250 int

					m_Owner.m_NextDetectHidden = Core.TickCount +
												 (int)TimeSpan.FromSeconds(Utility.RandomMinMax(min, max)).TotalMilliseconds;
				}
			}
		}
	}
}