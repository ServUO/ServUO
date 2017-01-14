using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using Server.Targeting;
using Server.Engines.PartySystem;
using System.Data;
using System.Xml;
using Server.Engines.XmlSpawner2;


namespace Server.Items
{
	public interface IXmlQuest
	{
		string Name { get; set; }

		string NoteString { get; set; }

		string TitleString { get; set; }

		string Objective1 { get; set; }

		string Objective2 { get; set; }

		string Objective3 { get; set; }

		string Objective4 { get; set; }

		string Objective5 { get; set; }

		string Description1 { get; set; }

		string Description2 { get; set; }

		string Description3 { get; set; }

		string Description4 { get; set; }

		string Description5 { get; set; }

		bool Completed1 { get; set; }

		bool Completed2 { get; set; }

		bool Completed3 { get; set; }

		bool Completed4 { get; set; }

		bool Completed5 { get; set; }

		string State1 { get; set; }

		string State2 { get; set; }

		string State3 { get; set; }

		string State4 { get; set; }

		string State5 { get; set; }

		bool PlayerMade { get; set; }

		bool PartyEnabled { get; set; }

		int PartyRange { get; set; }

		int Difficulty { get; set; }

		PlayerMobile Owner { get; set; }

		PlayerMobile Creator { get; set; }

		Container ReturnContainer { get; set; }

		Item RewardItem { get; set; }

		XmlAttachment RewardAttachment { get; set; }

		string Status { get; set; }

		string ExpirationString { get; }

		bool CanSeeReward { get; set; }

		bool AutoReward { get; set; }

		bool Repeatable { get; set; }

		bool IsValid { get; }

		bool AlreadyDone { get; }

		bool IsCompleted { get; }

		bool Deleted { get; }

		Container Pack { get; }

		bool HandlesOnSkillUse { get; }

		double Expiration { get; set; }

		DateTime TimeCreated { get; set; }

		void CheckAutoReward();

		void CheckRewardItem();

		void Invalidate();

		void OnSkillUse(Mobile m, Skill skill, bool success);

		List<XmlQuest.JournalEntry> Journal { get; set; }

		string AddJournalEntry { set;}

	}

	public interface ITemporaryQuestAttachment
	{
		Mobile QuestOwner { get; set; }
	}


	public abstract class XmlQuest
	{
		public const PlayerFlag CarriedXmlQuestFlag = (PlayerFlag)0x00100000;

		public const bool QuestPointsEnabled = true;

		public class JournalEntry
		{
			private string m_EntryID;
			private string m_EntryText;

			public string EntryID { get { return m_EntryID; } set { m_EntryID = value; } }
			public string EntryText { get { return m_EntryText; } set { m_EntryText = value; } }

			public JournalEntry(string ID, string text)
			{
				EntryID = ID;
				EntryText = text;
			}
		}

		public class GetCollectTarget : Target
		{
			IXmlQuest m_quest;

			public GetCollectTarget(IXmlQuest quest)
				: base(30, false, TargetFlags.None)
			{
				m_quest = quest;
			}
			protected override void OnTarget(Mobile from, object targeted)
			{
				if (targeted is Item && m_quest != null && !m_quest.Deleted)
				{
					XmlQuest.Collect(from, (Item)targeted, m_quest);
					from.CloseGump(typeof(XmlQuestStatusGump));
					from.SendGump(new XmlQuestStatusGump(m_quest, m_quest.TitleString));
				}
			}
		}

		public static void QuestButton(QuestGumpRequestArgs e)
		{
			if (e == null || e.Mobile == null) return;
			Mobile from = e.Mobile;

            from.CloseGump(typeof(XMLQuestLogGump));
			// bring up the quest status gump
            from.SendGump(new XMLQuestLogGump(from));

			// bring up the normal quest objectives gump
			//NormalQuestButton(from as PlayerMobile);
		}


		public static void QuestButton(NetState state, IEntity e, EncodedReader reader)
		{
			if (state == null || state.Mobile == null) return;
			Mobile from = state.Mobile;

            from.CloseGump(typeof(XMLQuestLogGump));
			// bring up the quest status gump
            from.SendGump(new XMLQuestLogGump(from));

			// bring up the normal quest objectives gump
			//NormalQuestButton(from as PlayerMobile);
		}

		// this just brings up the normal quest objectives gump 
		public static void NormalQuestButton(PlayerMobile from)
		{
			if (from == null || from.Quest == null) return;

			from.Quest.ShowQuestLog();
		}

		public static void RemoveTemporaryQuestObjects(Mobile questowner, string questname)
		{
			// find all TemporaryQuestObject attachments associated with the owner with the given name, and delete them

			List<XmlAttachment> list = new List<XmlAttachment>();

			foreach (XmlAttachment i in XmlAttach.Values)
			{
				// check for type
				if (i != null && !i.Deleted && i is ITemporaryQuestAttachment && ((ITemporaryQuestAttachment)i).QuestOwner == questowner && i.Name == questname)
				{
					list.Add(i);
				}
			}

			foreach (XmlAttachment i in list)
			{
				i.Delete();
			}
		}

		private static void ReturnCollected(IXmlQuest quest, Item item)
		{
			if (item == null) return;

			// if this was player made, then return the item to the creator
			// dont allow players to return items to themselves.  This prevents possible exploits where quests are used as
			// item transporters
			if (quest != null && quest.PlayerMade && (quest.Creator != null) && !quest.Creator.Deleted && (quest.Creator != quest.Owner) && !item.QuestItem)
			{
				bool returned = false;
				if ((quest.ReturnContainer != null) && !quest.ReturnContainer.Deleted)
				{
					returned = quest.ReturnContainer.TryDropItem(quest.Creator, item, false);

					//ReturnContainer.DropItem(m_RewardItem);
				}
				if (!returned)
				{
					quest.Creator.AddToBackpack(item);
				}

                quest.Creator.SendMessage("You receive {0} from quest {1}", item.GetType().Name, quest.Name);
			}
			else
			{
				// just delete it
				item.Delete();
			}
		}

		private static void TakeGiven(Mobile to, IXmlQuest quest, Item item)
		{
			if (item == null) return;

			XmlSaveItem si = (XmlSaveItem)XmlAttach.FindAttachment(to, typeof(XmlSaveItem), "Given");

			if (si == null)
			{
				XmlAttach.AttachTo(to, new XmlSaveItem("Given", item, quest.Owner));
			}
			else
			{
				si.SavedItem = item;
				si.WasOwnedBy = quest.Owner;
			}

			// just delete it
			//item.Delete();
		}


		public static object CreateItem(IEntity from, string action, out string status_str, Type typerestrict)
		{
			status_str = null;

			if (action == null || action.Length <= 0 || from == null) return null;

			XmlSpawner.SpawnObject TheSpawn = new XmlSpawner.SpawnObject(null, 0);

			//BaseXmlSpawner.ApplyObjectStringProperties(null, action, m_TargetItem, m, m_TargetItem, out status_str);

			TheSpawn.TypeName = action;
			string substitutedtypeName = BaseXmlSpawner.ApplySubstitution(null, null, null, action);
			string typeName = BaseXmlSpawner.ParseObjectType(substitutedtypeName);

			if (BaseXmlSpawner.IsTypeOrItemKeyword(typeName))
			{
				BaseXmlSpawner.SpawnTypeKeyword(null, TheSpawn, typeName, substitutedtypeName, true, null, from.Location, Map.Internal, out status_str);
			}
			else
			{
				// its a regular type descriptor so find out what it is
				Type type = SpawnerType.GetType(typeName);

				// if a type restriction has been specified then test it
				if (typerestrict != null && type != null && type != typerestrict && !type.IsSubclassOf(typerestrict))
				{
					return null;
				}


				try
				{
					string[] arglist = BaseXmlSpawner.ParseString(substitutedtypeName, 3, "/");
					object o = XmlSpawner.CreateObject(type, arglist[0], true, true);

					if (o == null)
					{
						status_str = "invalid type specification: " + arglist[0];
					}
					else if (o is Mobile)
					{
						Mobile m = (Mobile)o;

						// dont do mobiles as rewards at this point
						m.Delete();
					}
					else if (o is Item)
					{
						Item item = (Item)o;
						BaseXmlSpawner.AddSpawnItem(null, from, TheSpawn, item, from.Location, Map.Internal, null, false, substitutedtypeName, out status_str);
					}
					else if (o is XmlAttachment)
					{
						BaseXmlSpawner.ApplyObjectStringProperties(null, substitutedtypeName, o, from as Mobile, o, out status_str);
						return o;
					}
				}
				catch { }
			}
			if (TheSpawn.SpawnedObjects.Count > 0)
			{
				if (TheSpawn.SpawnedObjects[0] is Item)
				{
					return ((Item)TheSpawn.SpawnedObjects[0]);
				}
				else if (TheSpawn.SpawnedObjects[0] is Mobile)
				{
					// dont do mobiles as rewards at this point
					((Mobile)(TheSpawn.SpawnedObjects[0])).Delete();
				}
			}

			return null;
		}

		public static List<Item> FindXmlQuest(PlayerMobile from)
		{

			if (from == null || from.Deleted) return null;

			if (from.Backpack == null) return null;

			List<Item> packlist = from.Backpack.Items;

			if (packlist == null) return null;

			List<Item> itemlist = new List<Item>();

			for (int i = 0; i < packlist.Count; ++i)
			{
				Item item = packlist[i];

				if (item != null && !item.Deleted && item is IXmlQuest)
				{
					//found it
					// add the item to the list
					itemlist.Add(item);

				}
				// is it an XmlQuestBook?
				if (item is XmlQuestBook)
				{
					XmlQuestBook book = item as XmlQuestBook;
					// search the book
					foreach (Item xi in book.Items)
					{
						if (xi != null && !xi.Deleted && xi is IXmlQuest)
						{
							itemlist.Add(xi);
						}
					}
				}
			}
			// now check any item that might be held
			Item held = from.Holding;
			if (held != null && !held.Deleted && held is IXmlQuest)
			{
				//found it
				// add the item to the list
				itemlist.Add(held);
			}
			return itemlist;
		}

		public static void CheckArgList(string[] arglist, int argstart, object propobj, out string typestr, out int targetcount, out bool checkprop, out string status_str)
		{
			targetcount = 1;
			checkprop = true;
			status_str = null;
			typestr = null;

			if (arglist.Length > argstart)
			{
				// go through the remaining args and determine what they are
				for (int i = argstart; i < arglist.Length; i++)
				{
					// is it a count arg or a prop arg
					string[] propargs = BaseXmlSpawner.ParseString(arglist[i], 2, "<>=!");
					if (propargs.Length > 1)
					{
						// its a prop arg
						checkprop = BaseXmlSpawner.CheckPropertyString(null, propobj, arglist[i], null, out status_str);
					}
					else if (arglist[i] != null && arglist[i].Length > 0 && arglist[i][0] >= '0' && arglist[i][0] <= '9')
					{
						// its a count arg
						if(!int.TryParse(arglist[i], out targetcount))
							targetcount=1;
					}
					else
					{
						// its a type arg
						typestr = arglist[i];
					}

				}
			}
		}


		public static void ApplyCollected(Item target, IXmlQuest quest)
		{
			// check the quest objectives for special COLLECT keywords
			string newstatestr;
			bool collectstatus = false;

			if (!quest.Completed1 && CheckCollectObjective(quest, target, quest.Objective1, quest.State1, out newstatestr, out collectstatus))
			{
				quest.State1 = newstatestr;
				quest.Completed1 = collectstatus;
			}
			else if (!quest.Completed2 && CheckCollectObjective(quest, target, quest.Objective2, quest.State2, out newstatestr, out collectstatus))
			{
				quest.State2 = newstatestr;
				quest.Completed2 = collectstatus;
			}
			else if (!quest.Completed3 && CheckCollectObjective(quest, target, quest.Objective3, quest.State3, out newstatestr, out collectstatus))
			{
				quest.State3 = newstatestr;
				quest.Completed3 = collectstatus;
			}
			else if (!quest.Completed4 && CheckCollectObjective(quest, target, quest.Objective4, quest.State4, out newstatestr, out collectstatus))
			{
				quest.State4 = newstatestr;
				quest.Completed4 = collectstatus;
			}
			else if (!quest.Completed5 && CheckCollectObjective(quest, target, quest.Objective5, quest.State5, out newstatestr, out collectstatus))
			{
				quest.State5 = newstatestr;
				quest.Completed5 = collectstatus;

			}
			if (!quest.Deleted && quest.Owner != null && collectstatus)
			{
				quest.Owner.SendMessage("Quest objective completed.");

				// check to see if the quest has been completed and there is a reward to be automatically handed out
				quest.CheckAutoReward();

			}
		}

		public static void Collect(Mobile m, Item target, IXmlQuest quest)
		{
			if (quest == null || !quest.IsValid || m != quest.Owner) return;

			// check to see what was dropped onto this
			if (target != null && !target.Deleted)
			{
				// check for party collection
				Party p = null;
				if (m != null && !m.Deleted && m is PlayerMobile)
				{
					p = Party.Get(m);
				}

				if (quest.PartyEnabled && p != null)
				{
					// go through all of the party members to find the equivalent quest items and apply the collected target item
					// make a randomized order list
					List<PartyMemberInfo> startlist = new List<PartyMemberInfo>();
					List<PartyMemberInfo> randlist = new List<PartyMemberInfo>();

					foreach (PartyMemberInfo mi in p.Members)
					{
						startlist.Add(mi);
					}

					while (randlist.Count < p.Members.Count)
					{
						// pick a random member from the start list
						// then take them off the list
						int randindex = Utility.Random(startlist.Count);

						randlist.Add(startlist[randindex]);

						startlist.RemoveAt(randindex);
					}

					foreach (PartyMemberInfo mi in randlist)
					{
						Mobile member = mi.Mobile;

						// see if the member is in range
						if (quest.PartyRange < 0 || Utility.InRange(m.Location, member.Location, quest.PartyRange))
						{
							// find the quest item in their packs
							Item questitem = BaseXmlSpawner.SearchMobileForItem(member, quest.Name, "IXmlQuest", false);

							if (questitem != null && !questitem.Deleted && questitem is IXmlQuest)
							{
								ApplyCollected(target, (IXmlQuest)questitem);
							}
						}
					}
				}
				else
				{
					ApplyCollected(target, quest);
				}
			}
		}

		public static bool CheckCollectObjective(IXmlQuest quest, Item item, string objectivestr, string statestr, out string newstatestr, out bool collectstatus)
		{
			// format for the objective string will be COLLECT,itemtype[,count][,proptest] or COLLECTNAMED,itemname[,itemtype][,count][,proptest]
			newstatestr = statestr;
			collectstatus = false;
			if (objectivestr == null) return false;

			string[] arglist = BaseXmlSpawner.ParseString(objectivestr, 5, ",");
			int targetcount = 1;
			bool found = false;
			bool checkprop = true;
			string status_str = null;

			string typestr = null;

			CheckArgList(arglist, 2, item, out typestr, out targetcount, out checkprop, out status_str);

			if (status_str != null) quest.Status = status_str;

			if (arglist.Length > 1)
			{
				// collect task objective
				if (arglist[0] == "COLLECT")
				{
					//Type targettype = SpawnerType.GetType( arglist[1] );
					// test the collect requirements against the the collected item
					if (item != null && !item.Deleted && BaseXmlSpawner.CheckType(item, arglist[1])/*(item.GetType() == targettype)*/ && checkprop)
					{
						// found a match
						found = true;
					}
				}
				else if (arglist[0] == "COLLECTNAMED")
				{
					if (item != null && !item.Deleted && (arglist[1] == item.Name) && checkprop &&
					(typestr == null || BaseXmlSpawner.CheckType(item, typestr))
					)
					{
						// found a match
						found = true;
					}
				}
			}
			// update the objective state
			if (found)
			{
				int current = 0;
				int.TryParse(statestr, out current);
				// get the current collect count and update it
				int added = 0;
				if (item.Stackable)
				{
					if (targetcount - current < item.Amount)
					{
						added = targetcount - current;

						if (quest != null && quest.PlayerMade)
						{
							Item newitem = Mobile.LiftItemDupe(item, item.Amount - added);
							//Item newitem = item.Dupe(added);
							//if(newitem != null)
							//newitem.Amount = added;
							ReturnCollected(quest, newitem);
						}
						else
						{
							item.Amount -= added;
							ReturnCollected(quest, item);
						}
					}
					else
					{
						added = item.Amount;
						// if it is a playermade quest then give the item to the creator, otherwise just delete it
						ReturnCollected(quest, item);
						//item.Delete();
					}
				}
				else
				{
					if (targetcount - current > 0)
					{
						added = 1;
						//item.Delete();
					}
					ReturnCollected(quest, item);
				}

				int collected = current + added;

				newstatestr = String.Format("{0}", collected);

				if (collected >= targetcount)
				{
					// collecttask completed
					collectstatus = true;
				}
				return true;
			}
			else
				// not a collect task
				return false;
		}

		public static bool ApplyGiven(Mobile mob, Item target, IXmlQuest quest)
		{

			if (mob == null) return false;

			// check the quest objectives for special GIVE keywords
			string newstatestr;
			bool givestatus = false;
			bool found = false;

			if (!quest.Completed1 && CheckGiveObjective(quest, mob, target, quest.Objective1, quest.State1, out newstatestr, out givestatus))
			{
				quest.State1 = newstatestr;
				quest.Completed1 = givestatus;
				found = true;
			}
			else if (!quest.Completed2 && CheckGiveObjective(quest, mob, target, quest.Objective2, quest.State2, out newstatestr, out givestatus))
			{
				quest.State2 = newstatestr;
				quest.Completed2 = givestatus;
				found = true;
			}
			else if (!quest.Completed3 && CheckGiveObjective(quest, mob, target, quest.Objective3, quest.State3, out newstatestr, out givestatus))
			{
				quest.State3 = newstatestr;
				quest.Completed3 = givestatus;
				found = true;
			}
			else if (!quest.Completed4 && CheckGiveObjective(quest, mob, target, quest.Objective4, quest.State4, out newstatestr, out givestatus))
			{
				quest.State4 = newstatestr;
				quest.Completed4 = givestatus;
				found = true;
			}
			else if (!quest.Completed5 && CheckGiveObjective(quest, mob, target, quest.Objective5, quest.State5, out newstatestr, out givestatus))
			{
				quest.State5 = newstatestr;
				quest.Completed5 = givestatus;
				found = true;
			}

			/*
			if(found)
			{
				mob.Say("Thank you.");
			} else
			{
				mob.Say("I have no use for this.");
			}
			*/

			if (quest.Owner != null && found)
			{
				quest.Owner.SendMessage("Quest item accepted.");
			}

			if (!quest.Deleted && quest.Owner != null && givestatus)
			{
				quest.Owner.SendMessage("Quest objective completed.");
				// check to see if the quest has been completed and there is a reward to be automatically handed out
				quest.CheckAutoReward();
			}

			return found;
		}

		public static bool Give(Mobile from, Mobile to, Item target, IXmlQuest quest)
		{

			if (quest == null || !quest.IsValid) return false;

			bool found = false;

			// check to see what was dropped onto this
			if (target != null && !target.Deleted)
			{
				// check for party collection
				Party p = null;
				if (from != null && !from.Deleted && from is PlayerMobile)
				{
					p = Party.Get(from);
				}

				if (quest.PartyEnabled && p != null)
				{
					// go through all of the party members to find the equivalent quest items and apply the collected target item
					// make a randomized order list
					List<PartyMemberInfo> startlist = new List<PartyMemberInfo>();
					List<PartyMemberInfo> randlist = new List<PartyMemberInfo>();

					foreach (PartyMemberInfo mi in p.Members)
					{
						startlist.Add(mi);
					}

					while (randlist.Count < p.Members.Count)
					{
						// pick a random member from the start list
						// then take them off the list
						int randindex = Utility.Random(startlist.Count);

						randlist.Add(startlist[randindex]);

						startlist.RemoveAt(randindex);
					}

					foreach (PartyMemberInfo mi in randlist)
					{
						Mobile member = mi.Mobile;
						// see if the member is in range
						if (quest.PartyRange < 0 || Utility.InRange(from.Location, member.Location, quest.PartyRange))
						{
							// find the quest item in their packs
							Item questitem = BaseXmlSpawner.SearchMobileForItem(member, quest.Name, "IXmlQuest", false);

							if (questitem != null && !questitem.Deleted && questitem is IXmlQuest)
							{
								if (ApplyGiven(to, target, (IXmlQuest)questitem))
								{
									found = true;
								}
							}
						}
					}
				}
				else
				{
					found = ApplyGiven(to, target, quest);
				}
			}

			return found;
		}

		public static bool RegisterGive(Mobile from, Mobile to, Item item)
		{
			// check to see if this is a quest item that is to be collected
			// who is dropping it?

			bool found = false;

			if (item != null && !item.Deleted && from is PlayerMobile)
			{
				List<Item> questlist = FindXmlQuest((PlayerMobile)from);
				if (questlist != null)
				{
					// now go through the list and try to apply the dropped item
					for (int i = 0; i < questlist.Count; i++)
					{
						if (questlist[i] is IXmlQuest)
						{
							if (Give(from, to, item, (IXmlQuest)questlist[i]))
							{
								found = true;
							}
						}
					}
				}
			}

			return found;
		}


		public static bool CheckGiveObjective(IXmlQuest quest, Mobile mob, Item item, string objectivestr, string statestr, out string newstatestr, out bool givestatus)
		{
			// format for the objective string will be GIVE,mobname,itemtype[,count][,proptest] or GIVENAMED,mobname,itemname[,type][,count][,proptest]
			newstatestr = statestr;
			givestatus = false;
			if (objectivestr == null) return false;

			if (mob == null || mob.Name == null) return false;

			string[] arglist = BaseXmlSpawner.ParseString(objectivestr, 6, ",");
			int targetcount = 1;
			bool found = false;
			bool checkprop = true;
			string status_str = null;
			string typestr = null;

			CheckArgList(arglist, 3, item, out typestr, out targetcount, out checkprop, out status_str);

			if (status_str != null) quest.Status = status_str;

			if (arglist.Length > 1)
			{
				// the name of the mob must match the specified mobname
				if (mob.Name != arglist[1]) return false;
			}


			if (arglist.Length > 2)
			{
				// collect task objective
				if (arglist[0] == "GIVE")
				{
					//Type targettype = SpawnerType.GetType( arglist[2] );

					// test the requirements against the the given item
					if (item != null && !item.Deleted && BaseXmlSpawner.CheckType(item, arglist[2]) /*(item.GetType() == targettype)*/ && checkprop)
					{
						// found a match
						found = true;
					}
				}
				else if (arglist[0] == "GIVENAMED")
				{
					if (item != null && !item.Deleted && (arglist[2] == item.Name) && checkprop &&
					(typestr == null || BaseXmlSpawner.CheckType(item, typestr))
					)
					{
						// found a match
						found = true;
					}
				}
			}
			// update the objective state
			if (found)
			{

				int current = 0;
				int.TryParse(statestr, out current);

				// get the current given count and update it
				int added = 0;

				if (item.Stackable)
				{
					if (targetcount - current < item.Amount)
					{
						added = targetcount - current;

						if (quest != null && quest.PlayerMade)
						{
							//Item newitem = item.Dupe(added);
							Item newitem = Mobile.LiftItemDupe(item, added);
							//if(newitem != null)
							//newitem.Amount = added;
							TakeGiven(mob, quest, newitem);
						}
						else
						{
							item.Amount -= added;
						}
					}
					else
					{
						added = item.Amount;
						TakeGiven(mob, quest, item);
						//item.Delete();
					}
				}
				else
				{
					if (targetcount - current > 0)
					{
						added = 1;
						TakeGiven(mob, quest, item);
						//item.Delete();
					}
				}

				int collected = current + added;

				newstatestr = String.Format("{0}", collected);

				if (collected >= targetcount)
				{
					// givetask completed
					givestatus = true;
				}

				return (added > 0);

			}
			else
			{
				// not a give task
				return false;
			}
		}

		public static bool CheckKillObjective(IXmlQuest quest, Mobile m_killed, Mobile m_killer, string objectivestr, string statestr, out string newstatestr, out bool killstatus)
		{
			newstatestr = statestr;
			killstatus = false;
			if (objectivestr == null) return false;

			// format for the objective string will be KILL,mobtype[,count][,proptest] or KILLNAMED,mobname[,type][,count][,proptest]
			string[] arglist = BaseXmlSpawner.ParseString(objectivestr, 5, ",");
			int targetcount = 1;
			bool found = false;
			bool checkprop = true;
			string status_str = null;

			string typestr = null;

			CheckArgList(arglist, 2, m_killed, out typestr, out targetcount, out checkprop, out status_str);

			if (status_str != null) quest.Status = status_str;

			if (arglist.Length > 1)
			{
				// kill task objective
				if (arglist[0] == "KILL")
				{
					//Type targettype = SpawnerType.GetType( arglist[1] );

					// test the kill requirements against the the killed mobile
					if (m_killed != null && !m_killed.Deleted && BaseXmlSpawner.CheckType(m_killed, arglist[1])/*(m_killed.GetType() == targettype)*/ && checkprop)
					{
						// found a match
						found = true;
					}
				}
				else if (arglist[0] == "KILLNAMED")
				{
					if (m_killed != null && !m_killed.Deleted && (arglist[1] == m_killed.Name) && checkprop &&
					(typestr == null || BaseXmlSpawner.CheckType(m_killed, typestr))
					)
					{
						// found a match
						found = true;
					}
				}
			}
			// update the objective state
			if (found)
			{
				// get the current kill count and update it
				int current = 0;
				int.TryParse(statestr, out current);

				int killed = current + 1;
				newstatestr = String.Format("{0}", killed);

				if (killed >= targetcount)
				{
					// killtask completed
					killstatus = true; ;
				}
				return true;
			}
			else
				// not a kill task
				return false;
		}

		public static void ApplyKilled(Mobile m_killed, Mobile m_killer, IXmlQuest quest)
		{
			if (quest == null || !quest.IsValid) return;

			string newstatestr;
			bool killstatus = false;
			if (!quest.Completed1 && CheckKillObjective(quest, m_killed, m_killer, quest.Objective1, quest.State1, out newstatestr, out killstatus))
			{
				quest.State1 = newstatestr;
				quest.Completed1 = killstatus;
			}
			else if (!quest.Completed2 && CheckKillObjective(quest, m_killed, m_killer, quest.Objective2, quest.State2, out newstatestr, out killstatus))
			{
				quest.State2 = newstatestr;
				quest.Completed2 = killstatus;
			}
			else if (!quest.Completed3 && CheckKillObjective(quest, m_killed, m_killer, quest.Objective3, quest.State3, out newstatestr, out killstatus))
			{
				quest.State3 = newstatestr;
				quest.Completed3 = killstatus;
			}
			else if (!quest.Completed4 && CheckKillObjective(quest, m_killed, m_killer, quest.Objective4, quest.State4, out newstatestr, out killstatus))
			{
				quest.State4 = newstatestr;
				quest.Completed4 = killstatus;
			}
			else if (!quest.Completed5 && CheckKillObjective(quest, m_killed, m_killer, quest.Objective5, quest.State5, out newstatestr, out killstatus))
			{
				quest.State5 = newstatestr;
				quest.Completed5 = killstatus;
			}
			if (!quest.Deleted && quest.Owner != null && killstatus)
			{
				quest.Owner.SendMessage("Quest objective completed.");
				// check to see if the quest has been completed and there is a reward to be automatically handed out
				quest.CheckAutoReward();
			}
		}

		public static void CheckKilled(Mobile m_killed, Mobile m_killer, Mobile member)
		{
			if (!(member is PlayerMobile)) return;

			// search the player for IXmlQuest objects
			List<Item> mobitems = FindXmlQuest((PlayerMobile)member);

			if (mobitems == null) return;

			for (int i = 0; i < mobitems.Count; i++)
			{
				// search the objects for kill requirements
				if (mobitems[i] is IXmlQuest)
				{
					IXmlQuest quest = (IXmlQuest)mobitems[i];

					if (quest != null && !quest.Deleted && quest.PartyEnabled)
					{
						if (member != null && !member.Deleted)
						{
							if (quest.PartyRange < 0 || Utility.InRange(m_killer.Location, member.Location, quest.PartyRange))
								ApplyKilled(m_killed, member, quest);
						}
					}
					else if (member != null && !member.Deleted && member == m_killer && quest != null && !quest.Deleted)
					{
						ApplyKilled(m_killed, m_killer, quest);
					}
				}
			}
		}




		public static void RegisterKill(Mobile m_killed, Mobile m_killer)
		{

			// check for any attachments that might support the OnBeforeKill method
			XmlAttach.CheckOnBeforeKill(m_killed, m_killer);

			// check for any attachments that might support the OnKill method
			XmlAttach.CheckOnKill(m_killed, m_killer);

			// go through all of the party members to to try to fill killquest objectives
			Party p = Party.Get(m_killer);
			if (p != null)
			{
				foreach (PartyMemberInfo mi in p.Members)
				{
					Mobile member = mi.Mobile;
					if (member != null && member is PlayerMobile && ((PlayerMobile)member).GetFlag(CarriedXmlQuestFlag))
					{

						CheckKilled(m_killed, m_killer, member);

					}

				}
			}
			else
			{
				if (m_killer != null && m_killer is PlayerMobile && ((PlayerMobile)m_killer).GetFlag(CarriedXmlQuestFlag))
				{
					CheckKilled(m_killed, m_killer, m_killer);

				}

			}
		}

		public static bool CheckEscortObjective(IXmlQuest quest, Mobile m_escorted, Mobile m_escorter, string objectivestr, string statestr, out string newstatestr, out bool escortstatus)
		{
			newstatestr = statestr;
			escortstatus = false;
			if (objectivestr == null) return false;
			// format for the objective string will be ESCORT[,mobname][,proptest]
			string[] arglist = BaseXmlSpawner.ParseString(objectivestr, 3, ",");

			if (arglist.Length > 0)
			{
				// is it an escort task?
				if (arglist[0] != "ESCORT")
					return false;
			}
			else
			{
				return false;
			}

			bool found = false;

			int targetcount = 1;

			bool checkprop = true;
			string status_str = null;

			if (arglist.Length > 2)
			{
				checkprop = BaseXmlSpawner.CheckPropertyString(null, m_escorted, arglist[2], null, out status_str);
			}

			if (status_str != null) quest.Status = status_str;

			if (arglist.Length > 1)
			{
				// check the mobname, allow for empty names to match any escort

				if (m_escorted != null && !m_escorted.Deleted && (arglist[1] == m_escorted.Name || (arglist[1] == null || arglist[1] == String.Empty)) && checkprop)
				{
					// found a match
					found = true;
				}

			}
			else
			{
				// no mobname so any escort will do
				if (m_escorted != null && !m_escorted.Deleted && checkprop)
				{
					// found a match
					found = true;
				}
			}

			// update the objective state
			if (found)
			{
				// get the current escort count and update it
				int current = 0;
				int.TryParse(statestr, out current);

				int escorted = current + 1;

				newstatestr = String.Format("{0}", escorted);

				if (escorted >= targetcount)
				{
					// escort completed
					escortstatus = true; ;
				}
				return true;
			}
			else
				// not an escort task
				return false;
		}

		public static void ApplyEscorted(Mobile m_escorted, Mobile m_escorter, IXmlQuest quest)
		{
			if (quest == null || !quest.IsValid) return;

			string newstatestr;
			bool escortstatus = false;
			if (!quest.Completed1 && CheckEscortObjective(quest, m_escorted, m_escorter, quest.Objective1, quest.State1, out newstatestr, out escortstatus))
			{
				quest.State1 = newstatestr;
				quest.Completed1 = escortstatus;
			}
			else if (!quest.Completed2 && CheckEscortObjective(quest, m_escorted, m_escorter, quest.Objective2, quest.State2, out newstatestr, out escortstatus))
			{
				quest.State2 = newstatestr;
				quest.Completed2 = escortstatus;
			}
			else if (!quest.Completed3 && CheckEscortObjective(quest, m_escorted, m_escorter, quest.Objective3, quest.State3, out newstatestr, out escortstatus))
			{
				quest.State3 = newstatestr;
				quest.Completed3 = escortstatus;
			}
			else if (!quest.Completed4 && CheckEscortObjective(quest, m_escorted, m_escorter, quest.Objective4, quest.State4, out newstatestr, out escortstatus))
			{
				quest.State4 = newstatestr;
				quest.Completed4 = escortstatus;
			}
			else if (!quest.Completed5 && CheckEscortObjective(quest, m_escorted, m_escorter, quest.Objective5, quest.State5, out newstatestr, out escortstatus))
			{
				quest.State5 = newstatestr;
				quest.Completed5 = escortstatus;
			}
			if (!quest.Deleted && quest.Owner != null && escortstatus)
			{
				quest.Owner.SendMessage("Quest objective completed.");
				// check to see if the quest has been completed and there is a reward to be automatically handed out
				quest.CheckAutoReward();
			}
		}

		public static void CheckEscorted(Mobile m_escorted, Mobile m_escorter, Mobile member)
		{
			if (!(member is PlayerMobile)) return;

			// search the player for IXmlQuest objects
			List<Item> mobitems = FindXmlQuest((PlayerMobile)member);

			if (mobitems == null) return;

			for (int i = 0; i < mobitems.Count; i++)
			{

				if (mobitems[i] is IXmlQuest)
				{
					// search the objects for escort requirements
					IXmlQuest quest = (IXmlQuest)mobitems[i];

					if (quest != null && !quest.Deleted && quest.PartyEnabled)
					{
						if (member != null && !member.Deleted)
						{
							if (quest.PartyRange < 0 || Utility.InRange(m_escorter.Location, member.Location, quest.PartyRange))
							{
								ApplyEscorted(m_escorted, member, quest);
							}
						}
					}
					else if (member != null && !member.Deleted && member == m_escorter && quest != null && !quest.Deleted)
					{
						ApplyEscorted(m_escorted, m_escorter, quest);
					}
				}
			}
		}

		public static void RegisterEscort(Mobile m_escorted, Mobile m_escorter)
		{

			// go through all of the party members to to try to fill escort objectives
			Party p = Party.Get(m_escorter);
			if (p != null)
			{
				foreach (PartyMemberInfo mi in p.Members)
				{
					Mobile member = mi.Mobile;
					if (member != null && member is PlayerMobile && ((PlayerMobile)member).GetFlag(CarriedXmlQuestFlag))
					{

						CheckEscorted(m_escorted, m_escorter, member);
					}

				}
			}
			else
			{
				if (m_escorter != null && m_escorter is PlayerMobile && ((PlayerMobile)m_escorter).GetFlag(CarriedXmlQuestFlag))
				{
					CheckEscorted(m_escorted, m_escorter, m_escorter);
				}

			}
		}

		// Unused...was it meant to do anything?!
		/*public static Hashtable VisitSectorList = new Hashtable();

		public static void RegisterMove(PlayerMobile m_player)
		{

			if (m_player == null || m_player.Map == null) return;

			// check for any attachments that might support the OnMove method
			//XmlAttach.CheckOnMove(m_player);

			// check to see if the current sector that the player is in, is registered in the VISIT sector list
			Sector newSector = m_player.Map.GetSector(m_player.Location);

			if (VisitSectorList != null && VisitSectorList.Contains(newSector))
			{
				// check to see if the player has a quest with a VISIT type objective
				if (m_player.GetFlag(CarriedXmlQuestFlag))
				{
					CheckVisited(m_player);
				}
			}
		}*/

		public static bool CheckVisitObjective(IXmlQuest quest, PlayerMobile m_player, string objectivestr, string statestr, out string newstatestr, out bool visitstatus)
		{
			newstatestr = statestr;
			visitstatus = false;

			if (objectivestr == null) return false;

			// format for the objective string will be VISIT,x,y,range[,duration]
			string[] arglist = BaseXmlSpawner.ParseString(objectivestr, 5, ",");

			bool found = false;

			int targetcount = 1;

			string status_str = null;

			if (status_str != null) quest.Status = status_str;

			if (arglist.Length > 3)
			{
				// escort task objective
				if (arglist[0] == "VISIT")
				{

					double duration = 0; // duration in minutes

					// get the coords
					int x = 0;
					if(!int.TryParse(arglist[1], out x))
						status_str = "invalid VISIT x";

					int y = 0;
					if(!int.TryParse(arglist[2], out y))
						status_str = "invalid VISIT y";

					int range = 0;
					if(!int.TryParse(arglist[3], out range))
						status_str = "invalid VISIT range";

					if (arglist.Length > 4)
					{
						if(!double.TryParse(arglist[4], NumberStyles.Any, CultureInfo.InvariantCulture, out duration))
							status_str = "invalid VISIT duration";
					}

					// check them against the players current location

					if (m_player != null && m_player.InRange(new Point2D(x, y), range))
					{
						if (duration > 0)
						{
							// is there already a timer started on the quest object?
						}
						else
						{
							found = true;
						}
						// if it is in range, then start the timer
					}
				}
			}

			// update the objective state
			if (found)
			{
				// get the current visitation count and update it
				int current = 0;
				int.TryParse(statestr, out current);

				int visited = current + 1;

				newstatestr = String.Format("{0}", visited);

				if (visited >= targetcount)
				{
					// visitation completed
					visitstatus = true; ;
				}
				return true;
			}
			else
				// not a visitation task
				return false;
		}

		public static void ApplyVisited(PlayerMobile m_player, IXmlQuest quest)
		{
			if (quest == null || !quest.IsValid) return;

			string newstatestr;
			bool visitstatus = false;
			if (!quest.Completed1 && CheckVisitObjective(quest, m_player, quest.Objective1, quest.State1, out newstatestr, out visitstatus))
			{
				quest.State1 = newstatestr;
				quest.Completed1 = visitstatus;
			}
			else if (!quest.Completed2 && CheckVisitObjective(quest, m_player, quest.Objective2, quest.State2, out newstatestr, out visitstatus))
			{
				quest.State2 = newstatestr;
				quest.Completed2 = visitstatus;
			}
			else if (!quest.Completed3 && CheckVisitObjective(quest, m_player, quest.Objective2, quest.State2, out newstatestr, out visitstatus))
			{
				quest.State3 = newstatestr;
				quest.Completed3 = visitstatus;
			}
			else if (!quest.Completed4 && CheckVisitObjective(quest, m_player, quest.Objective4, quest.State4, out newstatestr, out visitstatus))
			{
				quest.State4 = newstatestr;
				quest.Completed4 = visitstatus;
			}
			else if (!quest.Completed5 && CheckVisitObjective(quest, m_player, quest.Objective5, quest.State5, out newstatestr, out visitstatus))
			{
				quest.State5 = newstatestr;
				quest.Completed5 = visitstatus;
			}
			if (!quest.Deleted && quest.Owner != null && visitstatus)
			{
				quest.Owner.SendMessage("Quest objective completed.");
				// check to see if the quest has been completed and there is a reward to be automatically handed out
				quest.CheckAutoReward();
			}
		}

		public static void CheckVisited(PlayerMobile m_player)
		{

			// search the player for IXmlQuest objects
			List<Item> mobitems = FindXmlQuest(m_player);

			if (mobitems == null) return;

			for (int i = 0; i < mobitems.Count; i++)
			{

				if (mobitems[i] is IXmlQuest)
				{
					// search the objects for visitation requirements
					IXmlQuest quest = (IXmlQuest)mobitems[i];

					if (quest != null && !quest.Deleted)
					{
						ApplyVisited(m_player, quest);
					}
				}
			}
		}


		public static bool VerifyObjective(string[] arglist, out string status_str)
		{
			status_str = null;

			if (arglist == null || arglist.Length < 1)
			{
				return true;
			}

			bool checkprop;
			int targetcount;
			string typestr = null;

			switch (arglist[0])
			{
				case "COLLECT":
				case "KILL":
					XmlQuest.CheckArgList(arglist, 2, null, out typestr, out targetcount, out checkprop, out status_str);
					if (arglist.Length > 1)
					{
						if (SpawnerType.GetType(arglist[1]) == null)
						{
							status_str = "Invalid type: " + arglist[1];
							return false;
						}
					}
					else
					{
						status_str = arglist[0] + "missing args";
						return false;
					}
					break;
				case "COLLECTNAMED":
				case "KILLNAMED":
					XmlQuest.CheckArgList(arglist, 2, null, out typestr, out targetcount, out checkprop, out status_str);
					if (arglist.Length < 1)
					{
						status_str = arglist[0] + "missing args";
						return false;
					}
					break;
				case "GIVENAMED":
					XmlQuest.CheckArgList(arglist, 3, null, out typestr, out targetcount, out checkprop, out status_str);
					if (arglist.Length < 1)
					{
						status_str = arglist[0] + "missing args";
						return false;
					}
					break;
				case "GIVE":
					XmlQuest.CheckArgList(arglist, 3, null, out typestr, out targetcount, out checkprop, out status_str);
					if (arglist.Length > 2)
					{
						if (SpawnerType.GetType(arglist[2]) == null)
						{
							status_str = "Invalid type: " + arglist[2];
							return false;
						}
					}
					else
					{
						status_str = arglist[0] + "missing args";
						return false;
					}
					break;
			}


			// check the validity of the typestr
			if (typestr != null)
			{
				if (SpawnerType.GetType(typestr) == null)
				{
					status_str = "Invalid type: " + typestr;
					return false;
				}
			}

			return true;
		}

		public static void VerifyObjectives(IXmlQuest quest)
		{
			string status_str;

			// go through each objective and test the args
			VerifyObjective(BaseXmlSpawner.ParseString(quest.Objective1, 6, ","), out status_str);
			if (status_str != null) quest.Status = status_str;
			VerifyObjective(BaseXmlSpawner.ParseString(quest.Objective2, 6, ","), out status_str);
			if (status_str != null) quest.Status = status_str;
			VerifyObjective(BaseXmlSpawner.ParseString(quest.Objective3, 6, ","), out status_str);
			if (status_str != null) quest.Status = status_str;
			VerifyObjective(BaseXmlSpawner.ParseString(quest.Objective4, 6, ","), out status_str);
			if (status_str != null) quest.Status = status_str;
			VerifyObjective(BaseXmlSpawner.ParseString(quest.Objective5, 6, ","), out status_str);
			if (status_str != null) quest.Status = status_str;

		}
	}
}
