using Server.ContextMenus;
using Server.Misc;
using Server.Mobiles;
using Server.Services.Virtues;

using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
	public class BaseEscort : MondainQuester
	{
		private static readonly TimeSpan m_EscortDelay = TimeSpan.FromMinutes(5.0);

		private static readonly Dictionary<Mobile, Mobile> m_EscortTable = new Dictionary<Mobile, Mobile>();

		private bool m_Checked;

		public BaseQuest Quest { get; set; }
		public DateTime LastSeenEscorter { get; set; }

		public bool HasEscorter => m_EscortTable.ContainsValue(this);

		public bool IsDeleting => HasEscorter && DeleteTime > DateTime.MinValue;

		public override TimeSpan DeleteTimerDelay => HasEscorter ? TimeSpan.FromSeconds(45) : TimeSpan.FromMinutes(5);

		public override bool IsInvulnerable => false;
		public override bool Commandable => false;

		public override Type[] Quests => null;

		public override bool CanAutoStable => false;
		public override bool CanDetectHidden => false;

		public BaseEscort()
			: base()
		{
			AI = AIType.AI_Melee;
			FightMode = FightMode.Aggressor;
			RangePerception = 22;
			RangeFight = 1;
			ActiveSpeed = 0.2;
			PassiveSpeed = 1.0;

			ControlSlots = 0;
		}

		public BaseEscort(Serial serial)
			: base(serial)
		{
		}

		public override void OnTalk(PlayerMobile player)
		{
			if (AcceptEscorter(player))
			{
				base.OnTalk(player);
			}
		}

		public override bool CanBeRenamedBy(Mobile from)
		{
			return from.AccessLevel >= AccessLevel.GameMaster;
		}

		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (from.Alive && from == ControlMaster)
			{
				list.Add(new AbandonEscortEntry(this));
			}

			base.AddCustomContextEntries(from, list);
		}

		public override void OnAfterDelete()
		{
			if (Quest != null)
			{
				if (Quest.Owner != null)
				{
					m_EscortTable.Remove(Quest.Owner);
				}

				Quest.RemoveQuest();
			}

			base.OnAfterDelete();
		}

		public override void OnThink()
		{
			base.OnThink();

			CheckAtDestination();
		}

		public override bool CanBeDamaged()
		{
			return true;
		}

		public override void InitBody()
		{
			SetStr(90, 100);
			SetDex(90, 100);
			SetInt(15, 25);

			Race = Race.Human;
			Hue = Race.RandomSkinHue();
			Female = Utility.RandomBool();
			Name = NameList.RandomName(Female ? "female" : "male");

			Utility.AssignRandomHair(this);
			Utility.AssignRandomFacialHair(this);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write(IsDeleting);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			if (reader.ReadBool())
			{
				if (version == 0)
				{
					reader.ReadDeltaTime();
				}

				Delete();
			}
		}

		public void AddHash(PlayerMobile player)
		{
			m_EscortTable[player] = this;
		}

		public virtual void StartFollow()
		{
			StartFollow(ControlMaster);
		}

		public virtual void StartFollow(Mobile escorter)
		{
			CantWalk = false;
			CanMove = true;

			ActiveSpeed = 0.1;
			PassiveSpeed = 0.2;

			ControlOrder = OrderType.Follow;
			ControlTarget = escorter;

			CurrentSpeed = 0.1;
		}

		public virtual void StopFollow()
		{
			ActiveSpeed = 0.2;
			PassiveSpeed = 1.0;

			ControlOrder = OrderType.None;
			ControlTarget = null;

			CurrentSpeed = 1.0;

			SetControlMaster(null);
		}

		public virtual void BeginDelete(Mobile m)
		{
			StopFollow();

			if (m != null)
			{
				m_EscortTable.Remove(m);
			}

			BeginDeleteTimer();
		}

		public virtual bool AcceptEscorter(Mobile m)
		{
			if (!m.Alive)
			{
				return false;
			}

			if (IsDeleting)
			{
				Say(500898); // I am sorry, but I do not wish to go anywhere.
				return false;
			}

			if (Controlled)
			{
				if (m == ControlMaster)
				{
					m.SendGump(new MondainQuestGump(Quest, MondainQuestGump.Section.InProgress, false));
				}
				else
				{
					Say(500897); // I am already being led!
				}

				return false;
			}

			if (!m.InRange(Location, 5))
			{
				Say(500348); // I am too far away to do that.
				return false;
			}

			if (m_EscortTable.ContainsKey(m))
			{
				Say(500896); // I see you already have an escort.
				return false;
			}

			if (m is PlayerMobile p && p.LastEscortTime + m_EscortDelay >= DateTime.UtcNow)
			{
				var minutes = (int)Math.Ceiling((p.LastEscortTime + m_EscortDelay - DateTime.UtcNow).TotalMinutes);

				Say("You must rest {0} minute{1} before we set out on this journey.", minutes, minutes == 1 ? "" : "s");
				return false;
			}

			StopDeleteTimer();

			return true;
		}

		public virtual EscortObjective GetObjective()
		{
			if (Quest != null)
			{
				for (var i = 0; i < Quest.Objectives.Count; i++)
				{
					if (Quest.Objectives[i] is EscortObjective escort && !escort.Completed && !escort.Failed)
					{
						return escort;
					}
				}
			}

			return null;
		}

		public virtual Mobile GetEscorter()
		{
			var master = ControlMaster;

			if (master == null || !Controlled)
			{
				return master;
			}

			if (master.Map != Map || !master.InRange(Location, 30) || !master.Alive)
			{
				var lastSeenDelay = DateTime.UtcNow - LastSeenEscorter;

				if (lastSeenDelay >= TimeSpan.FromMinutes(2.0))
				{
					var escort = GetObjective();

					if (escort != null)
					{
						master.SendLocalizedMessage(1071194); // You have failed your escort quest…
						master.PlaySound(0x5B3);
						escort.Fail();
					}

					master.SendLocalizedMessage(1042473); // You have lost the person you were escorting.

					Say(1005653); // Hmmm.  I seem to have lost my master.

					StopFollow();

					m_EscortTable.Remove(master);

					BeginDeleteTimer();

					return null;
				}
				else
				{
					ControlOrder = OrderType.Stay;
				}
			}
			else
			{
				if (ControlOrder != OrderType.Follow)
				{
					StartFollow(master);
				}

				LastSeenEscorter = DateTime.UtcNow;
			}

			return master;
		}

		public virtual string GetDestination()
		{
			return null;
		}

		public virtual bool CheckAtDestination()
		{
			if (Quest != null)
			{
				var escort = GetObjective();

				if (escort == null)
				{
					return false;
				}

				var escorter = GetEscorter();

				if (escorter == null)
				{
					return false;
				}

				if (escort.Region != null && Region.IsPartOf(escort.Region))
				{
					Say(1042809, escorter.Name); // We have arrived! I thank thee, ~1_PLAYER_NAME~! I have no further need of thy services. Here is thy pay.

					escort.Complete();

					if (Quest.Completed)
					{
						escorter.SendLocalizedMessage(1046258, null, 0x23); // Your quest is complete.		

						if (QuestHelper.AnyRewards(Quest))
						{
							escorter.SendGump(new MondainQuestGump(Quest, MondainQuestGump.Section.Rewards, false, true));
						}
						else
						{
							Quest.GiveRewards();
						}

						escorter.PlaySound(Quest.CompleteSound);

						StopFollow();

						m_EscortTable.Remove(escorter);

						BeginDeleteTimer();

						// fame
						Titles.AwardFame(escorter, escort.Fame, true);

						// compassion
						var gainedPath = false;

						if (escorter is PlayerMobile pm)
						{
							if (pm.CompassionGains > 0 && DateTime.UtcNow > pm.NextCompassionDay)
							{
								pm.NextCompassionDay = DateTime.MinValue;
								pm.CompassionGains = 0;
							}

							if (pm.CompassionGains >= 5) // have already gained 5 times in one day, can gain no more
							{
								pm.SendLocalizedMessage(1053004); // You must wait about a day before you can gain in compassion again.
							}
							else if (VirtueHelper.Award(pm, VirtueName.Compassion, escort.Compassion, ref gainedPath))
							{
								pm.SendLocalizedMessage(1074949, null, 0x2A);  // You have demonstrated your compassion!  Your kind actions have been noted.

								if (gainedPath)
								{
									pm.SendLocalizedMessage(1053005); // You have achieved a path in compassion!
								}
								else
								{
									pm.SendLocalizedMessage(1053002); // You have gained in compassion.
								}

								pm.NextCompassionDay = DateTime.UtcNow + TimeSpan.FromDays(1.0); // in one day CompassionGains gets reset to 0

								++pm.CompassionGains;
							}
							else
							{
								pm.SendLocalizedMessage(1053003); // You have achieved the highest path of compassion and can no longer gain any further.
							}
						}
					}
					else
					{
						escorter.PlaySound(Quest.UpdateSound);
					}

					return true;
				}
			}
			else if (!m_Checked)
			{
				var region = GetDestination();

				if (region != null && Region.IsPartOf(region))
				{
					m_Checked = true;

					BeginDeleteTimer();
				}
			}

			return false;
		}

		private class AbandonEscortEntry : ContextMenuEntry
		{
			private readonly BaseEscort m_Mobile;

			public AbandonEscortEntry(BaseEscort m)
				: base(6102, 3)
			{
				m_Mobile = m;
			}

			public override void OnClick()
			{
				Owner.From.SendLocalizedMessage(1071194); // You have failed your escort quest…
				Owner.From.PlaySound(0x5B3);

				m_Mobile.Delete();
			}
		}

		public static void DeleteEscort(Mobile owner)
		{
			var pm = owner as PlayerMobile;

			foreach (var quest in pm.Quests)
			{
				if (quest.Quester is BaseEscort escort)
				{
					Timer.DelayCall(TimeSpan.FromSeconds(3), (e, o) =>
					{
						e.Say(500901); // Ack!  My escort has come to haunt me!

						o.SendLocalizedMessage(1071194); // You have failed your escort quest…
						o.PlaySound(0x5B3);

						e.Delete();
					}, escort, owner);
				}
			}
		}
	}
}
