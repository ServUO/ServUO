#define CLIENT6017

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

/*
** XmlQuestHolder class
**
**
** Version 1.0
** updated 9/17/04
** - based on the XmlQuestToken class, but derived from the Container instead of Item class in order to support reward holding and display
*/
namespace Server.Items
{
    public abstract class XmlQuestHolder : Container, IXmlQuest
    {
        //        public const PlayerFlag CarriedXmlQuestFlag = (PlayerFlag)0x00100000;
        private double m_ExpirationDuration;
        private DateTime m_TimeCreated;
        private string m_Objective1;
        private string m_Objective2;
        private string m_Objective3;
        private string m_Objective4;
        private string m_Objective5;
        private string m_Description1;
        private string m_Description2;
        private string m_Description3;
        private string m_Description4;
        private string m_Description5;
        private bool m_Completed1 = false;
        private bool m_Completed2 = false;
        private bool m_Completed3 = false;
        private bool m_Completed4 = false;
        private bool m_Completed5 = false;
        private string m_State1;
        private string m_State2;
        private string m_State3;
        private string m_State4;
        private string m_State5;
        private bool m_PartyEnabled = false;
        private int m_PartyRange = -1;
        private string m_ConfigFile;
        private string m_NoteString;
        private string m_TitleString;
        private string m_RewardString;
        private string m_AttachmentString;
        private PlayerMobile m_Owner;
        private readonly string m_SkillTrigger = null;
        private bool m_Repeatable = true;
        private TimeSpan m_NextRepeatable;

        private Item m_RewardItem;
        private XmlAttachment m_RewardAttachment;
        private int m_RewardAttachmentSerialNumber;
        private bool m_AutoReward = false;

        private bool m_CanSeeReward = true;
        private bool m_PlayerMade = false;
        private PlayerMobile m_Creator;
        private Container m_ReturnContainer;
        private string m_status_str;
        private int m_QuestDifficulty = 1;

        public static int JournalNotifyColor = 0;
        public static int JournalEchoColor = 6;

        public XmlQuestHolder(Serial serial)
            : base(serial)
        {
        }

        public XmlQuestHolder()
            : this(3643)
        {
        }

        public XmlQuestHolder(int itemID)
            : base(itemID)
        {
            this.Weight = 0;
            this.Hue = 500;
            //LootType = LootType.Blessed;
            this.TimeCreated = DateTime.UtcNow;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)6); // version
            // version 6
            if (this.m_Journal == null || this.m_Journal.Count == 0)
            {
                writer.Write((int)0);
            }
            else
            {
                writer.Write((int)this.m_Journal.Count);
                foreach (XmlQuest.JournalEntry e in this.m_Journal)
                {
                    writer.Write(e.EntryID);
                    writer.Write(e.EntryText);
                }
            }
            // version 5
            writer.Write(this.m_Repeatable);
            // version 4
            writer.Write(this.m_QuestDifficulty);
            // version 3
            writer.Write(this.m_AttachmentString);
            // version 2
            writer.Write(this.m_NextRepeatable);
            // version 1
            if (this.m_RewardAttachment != null)
                writer.Write(this.m_RewardAttachment.Serial.Value);
            else
                writer.Write((int)0);
            // version 0
            writer.Write(this.m_ReturnContainer);
            writer.Write(this.m_RewardItem);
            writer.Write(this.m_AutoReward);
            writer.Write(this.m_CanSeeReward);
            writer.Write(this.m_PlayerMade);
            writer.Write(this.m_Creator);
            writer.Write(this.m_Description1);
            writer.Write(this.m_Description2);
            writer.Write(this.m_Description3);
            writer.Write(this.m_Description4);
            writer.Write(this.m_Description5);
            writer.Write(this.m_Owner);
            writer.Write(this.m_RewardString);
            writer.Write(this.m_ConfigFile);
            writer.Write(this.m_NoteString);    // moved from the QuestNote class
            writer.Write(this.m_TitleString);   // moved from the QuestNote class
            writer.Write(this.m_PartyEnabled);
            writer.Write(this.m_PartyRange);
            writer.Write(this.m_State1);
            writer.Write(this.m_State2);
            writer.Write(this.m_State3);
            writer.Write(this.m_State4);
            writer.Write(this.m_State5);
            writer.Write(this.m_ExpirationDuration);
            writer.Write(this.m_TimeCreated);
            writer.Write(this.m_Objective1);
            writer.Write(this.m_Objective2);
            writer.Write(this.m_Objective3);
            writer.Write(this.m_Objective4);
            writer.Write(this.m_Objective5);
            writer.Write(this.m_Completed1);
            writer.Write(this.m_Completed2);
            writer.Write(this.m_Completed3);
            writer.Write(this.m_Completed4);
            writer.Write(this.m_Completed5);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 6:
                    {
                        int nentries = reader.ReadInt();

                        if (nentries > 0)
                        {
                            this.m_Journal = new ArrayList();
                            for (int i = 0; i < nentries; i++)
                            {
                                string entryID = reader.ReadString();
                                string entryText = reader.ReadString();
                                this.m_Journal.Add(new XmlQuest.JournalEntry(entryID, entryText));
                            }
                        }

                        goto case 5;
                    }
                case 5:
                    {
                        this.m_Repeatable = reader.ReadBool();

                        goto case 4;
                    }
                case 4:
                    {
                        this.m_QuestDifficulty = reader.ReadInt();

                        goto case 3;
                    }
                case 3:
                    {
                        this.m_AttachmentString = reader.ReadString();

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_NextRepeatable = reader.ReadTimeSpan();

                        goto case 1;
                    }
                case 1:
                    {
                        this.m_RewardAttachmentSerialNumber = reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        this.m_ReturnContainer = (Container)reader.ReadItem();
                        this.m_RewardItem = reader.ReadItem();
                        this.m_AutoReward = reader.ReadBool();
                        this.m_CanSeeReward = reader.ReadBool();
                        this.m_PlayerMade = reader.ReadBool();
                        this.m_Creator = reader.ReadMobile() as PlayerMobile;
                        this.m_Description1 = reader.ReadString();
                        this.m_Description2 = reader.ReadString();
                        this.m_Description3 = reader.ReadString();
                        this.m_Description4 = reader.ReadString();
                        this.m_Description5 = reader.ReadString();
                        this.m_Owner = reader.ReadMobile() as PlayerMobile;
                        this.m_RewardString = reader.ReadString();
                        this.m_ConfigFile = reader.ReadString();
                        this.m_NoteString = reader.ReadString();
                        this.m_TitleString = reader.ReadString();
                        this.m_PartyEnabled = reader.ReadBool();
                        this.m_PartyRange = reader.ReadInt();
                        this.m_State1 = reader.ReadString();
                        this.m_State2 = reader.ReadString();
                        this.m_State3 = reader.ReadString();
                        this.m_State4 = reader.ReadString();
                        this.m_State5 = reader.ReadString();
                        this.Expiration = reader.ReadDouble();
                        this.m_TimeCreated = reader.ReadDateTime();
                        this.m_Objective1 = reader.ReadString();
                        this.m_Objective2 = reader.ReadString();
                        this.m_Objective3 = reader.ReadString();
                        this.m_Objective4 = reader.ReadString();
                        this.m_Objective5 = reader.ReadString();
                        this.m_Completed1 = reader.ReadBool();
                        this.m_Completed2 = reader.ReadBool();
                        this.m_Completed3 = reader.ReadBool();
                        this.m_Completed4 = reader.ReadBool();
                        this.m_Completed5 = reader.ReadBool();
                    }
                    break;
            }
        }

        private static Item PlaceHolderItem = null;

        public static void Initialize()
        {
            // create a temporary placeholder item used to force allocation empty Items lists used to hold hidden rewards.
            PlaceHolderItem = new Item(1);

            foreach (Item item in World.Items.Values)
            {
                if (item is XmlQuestHolder)
                {
                    XmlQuestHolder t = item as XmlQuestHolder;

                    t.UpdateWeight();

                    t.RestoreRewardAttachment();
                }
            }

            // remove the temporary placeholder item
            PlaceHolderItem.Delete();
        }

        private void HideRewards()
        {
            if (this.m_RewardItem != null)
            {
                // remove the item from the containers item list
                if (this.Items.Contains(this.m_RewardItem))
                {
                    this.Items.Remove(this.m_RewardItem);
                }
            }
        }

        private void UnHideRewards()
        {
            if (this.m_RewardItem == null)
                return;

            Item tmpitem = null;

            if (this.Items == Item.EmptyItems)
            {
                tmpitem = PlaceHolderItem;

                if (tmpitem == null || tmpitem.Deleted)
                {
                    tmpitem = new Item(1);
                }

                // need to get it to allocate a new list by adding an item
                this.DropItem(tmpitem);
            }

            if (!this.Items.Contains(this.m_RewardItem))
            {
                this.m_RewardItem.Parent = this;
                this.m_RewardItem.Map = this.Map;

                // restore the item to the containers item list
                this.Items.Add(this.m_RewardItem);
            }

            // remove the placeholder
            if (tmpitem != null && this.Items.Contains(tmpitem))
            {
                this.Items.Remove(tmpitem);
                tmpitem.Map = Map.Internal;
            }

            if (tmpitem != null && tmpitem != PlaceHolderItem)
            {
                tmpitem.Delete();
            }
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (!(item is Container))
                return false;
            else
                return base.CheckItemUse(from, item);
        }

        public override void DisplayTo(Mobile to)
        {
            if (to == null)
                return;

            // add the reward item back into the container list for display
            this.UnHideRewards();

            to.Send(new ContainerDisplay(this));

            #if(CLIENT6017)
            // add support for new client container packets
            if (to.NetState != null && to.NetState.ContainerGridLines)
                to.Send(new ContainerContent6017(to, this));
            else
            #endif
                to.Send(new ContainerContent(to, this));

            if (ObjectPropertyList.Enabled)
            {
                List<Item> items = this.Items;

                for (int i = 0; i < items.Count; ++i)
                    to.Send(((Item)items[i]).OPLPacket);
            }
            // move the reward item out of container to protect it from use
            this.HideRewards();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            list.Add(this.Name);
            if (this.LootType == LootType.Blessed)
            {
                list.Add(1038021);
            }
            if (this.PlayerMade && this.Owner != null && !(this.RootParent is PlayerVendor))
            {
                list.Add(1050044, "{0}\t{1}", this.TotalItems, this.TotalWeight); // ~1_COUNT~items,~2_WEIGHT~stones
            }

            // add any playervendor price/description information
            if (this.RootParent is PlayerVendor)
            {
                ((PlayerVendor)this.RootParent).GetChildProperties(list, this);
            }
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (m.IsPlayer())
                return false;

            return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            if (from.IsPlayer())
                return false;

            return base.TryDropItem(from, dropped, sendFullMessage);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            return false;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            return false;
        }

        public override bool CheckTarget(Mobile from, Server.Targeting.Target targ, object targeted)
        {
            if (from.IsPlayer())
                return false;

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            //base.OnDoubleClick(from);
            if (!(from is PlayerMobile))
                return;

            if (this.PlayerMade && (from == this.Creator) && (from == this.Owner))
            {
                from.SendGump(new XmlPlayerQuestGump((PlayerMobile)from, this));
            }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D point)
        {
            bool returnvalue = base.OnDroppedToWorld(from, point);

            from.SendGump(new XmlConfirmDeleteGump(from, this));

            return false;
        }

        public override void OnDelete()
        {
            // remove any temporary quest attachments associated with this quest and quest owner
            XmlQuest.RemoveTemporaryQuestObjects(this.Owner, this.Name);

            base.OnDelete();

            // remove any reward items that might be attached to this
            this.ReturnReward();

            // determine whether the owner needs to be flagged with a quest attachment indicating completion of this quest
            this.QuestCompletionAttachment();

            this.CheckOwnerFlag();
        }

        public override void OnItemLifted(Mobile from, Item item)
        {
            base.OnItemLifted(from, item);

            if (from is PlayerMobile && this.PlayerMade && (this.Owner != null) && (this.Owner == this.Creator))
            {
                this.LootType = LootType.Regular;
            }
            else if (from is PlayerMobile && this.Owner == null)
            {
                this.Owner = from as PlayerMobile;

                this.LootType = LootType.Blessed;
                // flag the owner as carrying a questtoken
                this.Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag, true);
            }
        }

        public override void OnAdded(object target)
        {
            base.OnAdded(target);

            if ((target != null) && target is Container)
            {
                // find the parent of the container
                // note, the only valid additions are to the player pack or a questbook.  Anything else is invalid.  
                // This is to avoid exploits involving storage or transfer of questtokens
                // make an exception for playermade quests that can be put on playervendors
                object parentOfTarget = ((Container)target).Parent;

                // if this is a QuestBook then allow additions if it is in a players pack or it is a player quest
                if ((parentOfTarget != null) && parentOfTarget is Container && target is XmlQuestBook)
                {
                    parentOfTarget = ((Container)parentOfTarget).Parent;
                }

                // check to see if it can be added.
                // allow playermade quests to be placed in playervendors or in xmlquestbooks that are in the world (supports the playerquestboards)
                if (this.PlayerMade && (((parentOfTarget != null) && parentOfTarget is PlayerVendor) ||
                                        ((parentOfTarget == null) && target is XmlQuestBook)))
                {
                    this.CheckOwnerFlag();

                    this.Owner = null;

                    this.LootType = LootType.Regular;
                }
                else if ((parentOfTarget != null) && (parentOfTarget is PlayerMobile) && this.PlayerMade && (this.Owner != null) && ((this.Owner == this.Creator) || (this.Creator == null)))
                {
                    // check the old owner
                    this.CheckOwnerFlag();

                    this.Owner = parentOfTarget as PlayerMobile;

                    // first owner will become creator by default
                    if (this.Creator == null)
                        this.Creator = this.Owner;

                    this.LootType = LootType.Blessed;

                    // flag the new owner as carrying a questtoken
                    this.Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag, true);
                }
                else if ((parentOfTarget != null) && (parentOfTarget is PlayerMobile))
                {
                    if (this.Owner == null)
                    {
                        this.Owner = parentOfTarget as PlayerMobile;

                        this.LootType = LootType.Blessed;

                        // flag the owner as carrying a questtoken
                        this.Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag, true);
                    }
                    else if ((parentOfTarget as PlayerMobile != this.Owner) || (target is BankBox))
                    {
                        // tried to give it to another player or placed it in the players bankbox. try to return it to the owners pack
                        this.Owner.AddToBackpack(this);
                    }
                }
                else
                {
                    if (this.Owner != null)
                    {
                        // try to return it to the owners pack
                        this.Owner.AddToBackpack(this);
                    }
                    // allow placement into containers in the world, npcs or drop on their corpses when owner is null
                    else if (!(parentOfTarget is Mobile) && !(target is Corpse) && parentOfTarget != null)
                    {
                        // invalidate the token
                        this.CheckOwnerFlag();

                        this.Invalidate();
                    }
                }
            }
        }

        private ArrayList m_Journal;
        public ArrayList Journal
        {
            get
            {
                return this.m_Journal;
            }
            set
            {
                this.m_Journal = value;
            }
        }
        private static readonly char[] colondelim = new char[1] { ':' };

        public string EchoAddJournalEntry
        {
            set
            {
                // notify and echo journal text
                this.VerboseAddJournalEntry(value, true, true);
            }
        }

        public string NotifyAddJournalEntry
        {
            set
            {
                // notify
                this.VerboseAddJournalEntry(value, true, false);
            }
        }

        public string AddJournalEntry
        {
            set
            {
                // silent
                this.VerboseAddJournalEntry(value, false, false);
            }
        }

        private void VerboseAddJournalEntry(string entrystring, bool notify, bool echo)
        {
            if (entrystring == null)
                return;

            // parse the value
            string[] args = entrystring.Split(colondelim, 2);

            if (args == null)
                return;

            string entryID = null;
            string entryText = null;
            if (args.Length > 0)
            {
                entryID = args[0].Trim();
            }

            if (entryID == null || entryID.Length == 0)
                return;

            if (args.Length > 1)
            {
                entryText = args[1].Trim();
            }

            // allocate a new journal if none exists
            if (this.m_Journal == null)
                this.m_Journal = new ArrayList();

            // go through the existing journal to find a matching ID
            XmlQuest.JournalEntry foundEntry = null;

            foreach (XmlQuest.JournalEntry e in this.m_Journal)
            {
                if (e.EntryID == entryID)
                {
                    foundEntry = e;
                    break;
                }
            }

            if (foundEntry != null)
            {
                // modify an existing entry
                if (entryText == null || entryText.Length == 0)
                {
                    // delete the entry
                    this.m_Journal.Remove(foundEntry);
                }
                else
                {
                    // just replace the text
                    foundEntry.EntryText = entryText;

                    Mobile holder = this.RootParent as Mobile;

                    if (holder != null)
                    {
                        if (notify)
                        {
                            // notify the player holding the questholder                       
                            holder.SendMessage(JournalNotifyColor, "Journal entry '{0}' of quest '{1}' has been modified.", entryID, this.Name);
                        }
                        if (echo)
                        {
                            // echo the journal text to the player holding the questholder                       
                            holder.SendMessage(JournalEchoColor, "{0}", entryText);
                        }
                    }
                }
            }
            else
            {
                // add a new entry
                if (entryText != null && entryText.Length != 0)
                {
                    // add the new entry
                    this.m_Journal.Add(new XmlQuest.JournalEntry(entryID, entryText));

                    Mobile holder = this.RootParent as Mobile;

                    if (holder != null)
                    {
                        if (notify)
                        {
                            // notify the player holding the questholder                       
                            holder.SendMessage(JournalNotifyColor, "Journal entry '{0}' has been added to quest '{1}'.", entryID, this.Name);
                        }
                        if (echo)
                        {
                            // echo the journal text to the player holding the questholder                       
                            holder.SendMessage(JournalEchoColor, "{0}", entryText);
                        }
                    }
                }
            }
        }

        private void QuestCompletionAttachment()
        {
            bool complete = this.IsCompleted;

            // is this quest repeatable
            if ((!this.Repeatable || this.NextRepeatable > TimeSpan.Zero) && complete)
            {
                double expiresin = this.Repeatable ? this.NextRepeatable.TotalMinutes : 0;

                // then add an attachment indicating that it has already been done
                XmlAttach.AttachTo(this.Owner, new XmlQuestAttachment(this.Name, expiresin));
            }

            // have quest points been enabled?
            if (XmlQuest.QuestPointsEnabled && complete && !this.PlayerMade)
            {
                XmlQuestPoints.GiveQuestPoints(this.Owner, this);
            }
        }

        private void PackItem(Item item)
        {
            if (item != null)
            {
                this.DropItem(item);
            }

            this.PackItemsMovable(this, false);

            // make sure the weight and gold of the questtoken is updated to reflect the weight of added rewards in playermade quests to avoid
            // exploits where quests are used as zero weight containers

            this.UpdateWeight();
        }

        private void CalculateWeight(Item target)
        {
            if (target is Container)
            {
                int gold = 0;
                int weight = 0;
                int nitems = 0;

                foreach (Item i in ((Container)target).Items)
                {
                    // make sure gold amount is consistent with totalgold
                    if (i is Gold)
                    {
                        this.UpdateTotal(i, TotalType.Gold, i.Amount);
                    }

                    if (i is Container)
                    {
                        this.CalculateWeight(i);
                        weight += i.TotalWeight + (int)i.Weight;
                        gold += i.TotalGold;
                        nitems += i.TotalItems + 1;
                    }
                    else
                    {
                        weight += (int)(i.Weight * i.Amount);
                        gold += i.TotalGold;
                        nitems += 1;
                    }
                }

                this.UpdateTotal((Container)target, TotalType.Weight, weight);
                this.UpdateTotal((Container)target, TotalType.Gold, gold);
                this.UpdateTotal((Container)target, TotalType.Items, nitems);
            }
        }

        private void UpdateWeight()
        {
            // decide whether to hide the weight, gold, and number of the reward from the totals calculation
            if (this.PlayerMade)
            {
                this.UnHideRewards();
            }
            else
            {
                this.HideRewards();
            }

            // update the container totals
            this.UpdateTotals();

            // and the parent totals
            if (this.RootParent is Mobile)
            {
                ((Mobile)this.RootParent).UpdateTotals();
            }

            // hide the reward item
            this.HideRewards();
        }

        private void ReturnReward()
        {
            if (this.m_RewardItem != null)
            {
                this.CheckRewardItem();

                // if this was player made, then return the item to the creator
                if (this.PlayerMade && (this.Creator != null) && !this.Creator.Deleted)
                {
                    this.m_RewardItem.Movable = true;

                    // make sure all of the items in the pack are movable as well
                    this.PackItemsMovable(this, true);

                    bool returned = false;

                    if ((this.ReturnContainer != null) && !this.ReturnContainer.Deleted)
                    {
                        returned = this.ReturnContainer.TryDropItem(this.Creator, this.m_RewardItem, false);
                        //ReturnContainer.DropItem(m_RewardItem);
                    }
                    if (!returned)
                    {
                        returned = this.Creator.AddToBackpack(this.m_RewardItem);
                    }
                    if (returned)
                    {
                        this.Creator.SendMessage("Your reward {0} was returned from quest {1}", this.m_RewardItem.GetType().Name, this.Name);
                        //AddMobileWeight(Creator, m_RewardItem);
                    }
                    else
                    {
                        this.Creator.SendMessage("Attempted to return reward {0} from quest {1} : containers full.", this.m_RewardItem.GetType().Name, this.Name);
                    }
                }
                else
                {
                    // just delete it
                    this.m_RewardItem.Delete();
                }
                this.m_RewardItem = null;
                this.UpdateWeight();
            }
            if (this.m_RewardAttachment != null)
            {
                // delete any remaining attachments
                this.m_RewardAttachment.Delete();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public new string Name
        {
            get
            {
                if (this.PlayerMade)
                {
                    return "PQ: " + base.Name;
                }
                else
                {
                    return base.Name;
                }
            }
            set
            {
                base.Name = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Creator
        {
            get
            {
                return this.m_Creator;
            }
            set
            {
                this.m_Creator = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Difficulty
        {
            get
            {
                return this.m_QuestDifficulty;
            }
            set
            {
                this.m_QuestDifficulty = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Status
        {
            get
            {
                return this.m_status_str;
            }
            set
            {
                this.m_status_str = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string NoteString
        {
            get
            {
                return this.m_NoteString;
            }
            set
            {
                this.m_NoteString = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AutoReward
        {
            get
            {
                return this.m_AutoReward;
            }
            set
            {
                this.m_AutoReward = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanSeeReward
        {
            get
            {
                return this.m_CanSeeReward;
            }
            set
            {
                this.m_CanSeeReward = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerMade
        {
            get
            {
                return this.m_PlayerMade;
            }
            set
            {
                this.m_PlayerMade = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Container ReturnContainer
        {
            get
            {
                return this.m_ReturnContainer;
            }
            set
            {
                this.m_ReturnContainer = value;
            }
        }

        private void PackItemsMovable(Container pack, bool canmove)
        {
            if (pack == null)
                return;
            this.UnHideRewards();
            Item[] itemlist = pack.FindItemsByType(typeof(Item), true);
            if (itemlist != null)
            {
                for (int i = 0; i < itemlist.Length; i++)
                {
                    itemlist[i].Movable = canmove;
                }
            }
        }

        private void RestoreRewardAttachment()
        {
            this.m_RewardAttachment = XmlAttach.FindAttachmentBySerial(this.m_RewardAttachmentSerialNumber);
        }

        public XmlAttachment RewardAttachment
        {
            get
            {
                // if the reward item is not set, and the reward string is specified, then use the reward string to construct and assign the
                // reward item
                // dont allow player made quests to use the rewardstring creation feature
                if (this.m_RewardAttachment != null && this.m_RewardAttachment.Deleted)
                    this.m_RewardAttachment = null;

                if ((this.m_RewardAttachment == null || this.m_RewardAttachment.Deleted) &&
                    (this.m_AttachmentString != null) && !this.PlayerMade)
                {
                    object o = XmlQuest.CreateItem(this, this.m_AttachmentString, out this.m_status_str, typeof(XmlAttachment));
                    if (o is Item)
                    {
                        ((Item)o).Delete();
                    }
                    else if (o is XmlAttachment)
                    {
                        this.m_RewardAttachment = o as XmlAttachment;
                        this.m_RewardAttachment.OwnedBy = this;
                    }
                }

                return this.m_RewardAttachment;
            }
            set
            {
                // get rid of any existing attachment
                if (this.m_RewardAttachment != null && !this.m_RewardAttachment.Deleted)
                {
                    this.m_RewardAttachment.Delete();
                }

                this.m_RewardAttachment = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Item RewardItem
        {
            get
            {
                // if the reward item is not set, and the reward string is specified, then use the reward string to construct and assign the
                // reward item
                // dont allow player made quests to use the rewardstring creation feature
                if ((this.m_RewardItem == null || this.m_RewardItem.Deleted) &&
                    (this.m_RewardString != null) && !this.PlayerMade)
                {
                    object o = XmlQuest.CreateItem(this, this.m_RewardString, out this.m_status_str, typeof(Item));
                    if (o is Item)
                    {
                        this.m_RewardItem = o as Item;
                        this.PackItem(this.m_RewardItem);
                    }
                    else if (o is XmlAttachment)
                    {
                        ((XmlAttachment)o).Delete();
                    }
                }

                return this.m_RewardItem;
            }
            set
            {
                // get rid of any existing reward item if it has been assigned
                if (this.m_RewardItem != null && !this.m_RewardItem.Deleted)
                {
                    this.ReturnReward();
                }

                // and assign the new item
                this.m_RewardItem = value;

                /*
                // is this currently carried by a mobile?
                if(m_RewardItem.RootParent != null && m_RewardItem.RootParent is Mobile)
                {
                // if so then remove it
                ((Mobile)(m_RewardItem.RootParent)).RemoveItem(m_RewardItem);

                }
                */

                // and put it in the pack
                if (this.m_RewardItem != null && !this.m_RewardItem.Deleted)
                {
                    this.PackItem(this.m_RewardItem);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string TitleString
        {
            get
            {
                return this.m_TitleString;
            }
            set
            {
                this.m_TitleString = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string RewardString
        {
            get
            {
                return this.m_RewardString;
            }
            set
            {
                this.m_RewardString = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string AttachmentString
        {
            get
            {
                return this.m_AttachmentString;
            }
            set
            {
                this.m_AttachmentString = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ConfigFile
        {
            get
            {
                return this.m_ConfigFile;
            }
            set
            {
                this.m_ConfigFile = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LoadConfig
        {
            get
            {
                return false;
            }
            set
            {
                if (value == true)
                    this.LoadXmlConfig(this.ConfigFile);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PartyEnabled
        {
            get
            {
                return this.m_PartyEnabled;
            }
            set
            {
                this.m_PartyEnabled = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PartyRange
        {
            get
            {
                return this.m_PartyRange;
            }
            set
            {
                this.m_PartyRange = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string State1
        {
            get
            {
                return this.m_State1;
            }
            set
            {
                this.m_State1 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string State2
        {
            get
            {
                return this.m_State2;
            }
            set
            {
                this.m_State2 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string State3
        {
            get
            {
                return this.m_State3;
            }
            set
            {
                this.m_State3 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string State4
        {
            get
            {
                return this.m_State4;
            }
            set
            {
                this.m_State4 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string State5
        {
            get
            {
                return this.m_State5;
            }
            set
            {
                this.m_State5 = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Description1
        {
            get
            {
                return this.m_Description1;
            }
            set
            {
                this.m_Description1 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Description2
        {
            get
            {
                return this.m_Description2;
            }
            set
            {
                this.m_Description2 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Description3
        {
            get
            {
                return this.m_Description3;
            }
            set
            {
                this.m_Description3 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Description4
        {
            get
            {
                return this.m_Description4;
            }
            set
            {
                this.m_Description4 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Description5
        {
            get
            {
                return this.m_Description5;
            }
            set
            {
                this.m_Description5 = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Objective1
        {
            get
            {
                return this.m_Objective1;
            }
            set
            {
                this.m_Objective1 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Objective2
        {
            get
            {
                return this.m_Objective2;
            }
            set
            {
                this.m_Objective2 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Objective3
        {
            get
            {
                return this.m_Objective3;
            }
            set
            {
                this.m_Objective3 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Objective4
        {
            get
            {
                return this.m_Objective4;
            }
            set
            {
                this.m_Objective4 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Objective5
        {
            get
            {
                return this.m_Objective5;
            }
            set
            {
                this.m_Objective5 = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed1
        {
            get
            {
                return this.m_Completed1;
            }
            set
            {
                this.m_Completed1 = value;
                this.CheckAutoReward();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed2
        {
            get
            {
                return this.m_Completed2;
            }
            set
            {
                this.m_Completed2 = value;
                this.CheckAutoReward();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed3
        {
            get
            {
                return this.m_Completed3;
            }
            set
            {
                this.m_Completed3 = value;
                this.CheckAutoReward();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed4
        {
            get
            {
                return this.m_Completed4;
            }
            set
            {
                this.m_Completed4 = value;
                this.CheckAutoReward();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed5
        {
            get
            {
                return this.m_Completed5;
            }
            set
            {
                this.m_Completed5 = value;
                this.CheckAutoReward();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeCreated
        {
            get
            {
                return this.m_TimeCreated;
            }
            set
            {
                this.m_TimeCreated = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Expiration
        {
            get
            {
                return this.m_ExpirationDuration;
            }
            set
            {
                // cap the max value at 100 years
                if (value > 876000)
                {
                    this.m_ExpirationDuration = 876000;
                }
                else
                {
                    this.m_ExpirationDuration = value;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ExpiresIn
        {
            get
            {
                if (this.m_ExpirationDuration > 0)
                {
                    // if this is a player created quest, then refresh the expiration time until it is in someone elses possession
                    /*
                    if(PlayerMade && ((Owner == Creator) || (Owner == null)))
                    {
                    m_TimeCreated = DateTime.UtcNow;
                    }
                    */
                    return (this.m_TimeCreated + TimeSpan.FromHours(this.m_ExpirationDuration) - DateTime.UtcNow);
                }
                else
                {
                    return TimeSpan.FromHours(0);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsExpired
        {
            get
            {
                if (((this.m_ExpirationDuration > 0) && (this.ExpiresIn <= TimeSpan.FromHours(0))))
                {
                    return true;
                }
                else
                    return false;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Repeatable
        {
            get
            {
                return this.m_Repeatable;
            }
            set
            {
                this.m_Repeatable = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual TimeSpan NextRepeatable
        {
            get
            {
                return this.m_NextRepeatable;
            }
            set
            {
                this.m_NextRepeatable = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool AlreadyDone
        {
            get
            {
                // look for a quest attachment with the current quest name
                if (XmlAttach.FindAttachment(this.Owner, typeof(XmlQuestAttachment), this.Name) == null)
                    return false;

                return true;
            }
        }

        public virtual string ExpirationString
        {
            get
            {
                if (this.AlreadyDone)
                {
                    return "Already done";
                }
                else if (this.m_ExpirationDuration <= 0)
                {
                    return "Never expires";
                }
                else if (this.IsExpired)
                {
                    return "Expired";
                }
                else
                {
                    TimeSpan ts = this.ExpiresIn;

                    int days = (int)ts.TotalDays;
                    int hours = (int)(ts - TimeSpan.FromDays(days)).TotalHours;
                    int minutes = (int)(ts - TimeSpan.FromHours(hours)).TotalMinutes;
                    int seconds = (int)(ts - TimeSpan.FromMinutes(minutes)).TotalSeconds;

                    if (days > 0)
                    {
                        return String.Format("Expires in {0} days {1} hrs", days, hours);
                    }
                    else if (hours > 0)
                    {
                        return String.Format("Expires in {0} hrs {1} mins", hours, minutes);
                    }
                    else
                    {
                        return String.Format("Expires in {0} mins {1} secs", minutes, seconds);
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsValid
        {
            get
            {
                if (this.IsExpired)
                {
                    // eliminate reward definitions
                    this.RewardString = null;
                    this.AttachmentString = null;

                    // return any reward items
                    this.ReturnReward();

                    return false;
                }
                else if (this.AlreadyDone)
                {
                    return false;
                }
                else
                    return true;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsCompleted
        {
            get
            {
                if (this.IsValid &&
                    (this.Completed1 || this.Objective1 == null || (this.Objective1.Length == 0)) &&
                    (this.Completed2 || this.Objective2 == null || (this.Objective2.Length == 0)) &&
                    (this.Completed3 || this.Objective3 == null || (this.Objective3.Length == 0)) &&
                    (this.Completed4 || this.Objective4 == null || (this.Objective4.Length == 0)) &&
                    (this.Completed5 || this.Objective5 == null || (this.Objective5.Length == 0))
                )
                    return true;
                else
                    return false;
            }
        }

        public Container Pack
        {
            get
            {
                return this;
            }
        }

        // this is the handler for skill use
        // not yet implemented, just a hook for now
        public void OnSkillUse(Mobile m, Skill skill, bool success)
        {
            if (m == this.m_Owner && this.IsValid)
            {
                //m_skillTriggerActivated  = false;
                // do a location test for the skill use
                /*
                if ( !Utility.InRange( m.Location, this.Location, m_ProximityRange ) )
                return;
                */
                int testskill = -1;

                // check the skill trigger conditions, Skillname,min,max
                try
                {
                    testskill = (int)Enum.Parse(typeof(SkillName), this.m_SkillTrigger);
                }
                catch
                {
                }

                if (this.m_SkillTrigger != null && (int)skill.SkillName == testskill)
                {
                    // have a skill trigger so flag it and test it
                    //m_skillTriggerActivated  = true;
                }
            }
        }

        public bool HandlesOnSkillUse
        {
            get
            {
                return (this.IsValid && this.m_SkillTrigger != null && this.m_SkillTrigger.Length > 0);
            }
        }

        private void CheckOwnerFlag()
        {
            if (this.Owner != null && !this.Owner.Deleted)
            {
                // need to check to see if any other questtoken items are owned
                // search the Owners top level pack for an xmlquest
                ArrayList list = XmlQuest.FindXmlQuest(this.Owner);

                if (list == null || list.Count == 0)
                {
                    // if none remain then flag the ower as having none
                    this.Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag, false);
                }
            }
        }

        public virtual void Invalidate()
        {
            //Hue = 32;
            //LootType = LootType.Regular;
            if (this.Owner != null)
            {
                this.Owner.SendMessage(String.Format("Quest invalidated - '{0}' removed", this.Name));
            }
            this.Delete();
        }

        public void CheckRewardItem()
        {
            // go through all reward items and delete anything that is movable.  This blocks any exploits where players might
            // try to add items themselves
            if (this.m_RewardItem != null && !this.m_RewardItem.Deleted && this.m_RewardItem is Container)
            {
                foreach (Item i in ((Container)this.m_RewardItem).FindItemsByType(typeof(Item), true))
                {
                    if (i.Movable)
                    {
                        i.Delete();
                    }
                }
            }
        }

        public void CheckAutoReward()
        {
            if (!this.Deleted && this.AutoReward && this.IsCompleted && this.Owner != null &&
                ((this.RewardItem != null && !this.m_RewardItem.Deleted) || (this.RewardAttachment != null && !this.m_RewardAttachment.Deleted)))
            {
                if (this.RewardItem != null)
                {
                    // make sure nothing has been added to the pack other than the original reward items
                    this.CheckRewardItem();

                    this.m_RewardItem.Movable = true;

                    // make sure all of the items in the pack are movable as well
                    this.PackItemsMovable(this, true);

                    this.Owner.AddToBackpack(this.m_RewardItem);
                    //AddMobileWeight(Owner,m_RewardItem);

                    this.m_RewardItem = null;
                }
                if (this.RewardAttachment != null)
                {
                    Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(AttachToCallback), new object[] { this.Owner, this.m_RewardAttachment });

                    this.m_RewardAttachment = null;
                }

                this.Owner.SendMessage(String.Format("{0} completed. You receive the quest reward!", this.Name));
                this.Delete();
            }
        }

        public void AttachToCallback(object state)
        {
            object[] args = (object[])state;

            XmlAttach.AttachTo(args[0], (XmlAttachment)args[1]);
        }

        private const string XmlTableName = "Properties";
        private const string XmlDataSetName = "XmlQuestHolder";

        public void LoadXmlConfig(string filename)
        {
            if (filename == null || filename.Length <= 0)
                return;
            // Check if the file exists
            if (System.IO.File.Exists(filename) == true)
            {
                FileStream fs = null;
                try
                {
                    fs = File.Open(filename, FileMode.Open, FileAccess.Read);
                }
                catch
                {
                }

                if (fs == null)
                {
                    this.Status = String.Format("Unable to open {0} for loading", filename);
                    return;
                }

                // Create the data set
                DataSet ds = new DataSet(XmlDataSetName);

                // Read in the file
                //ds.ReadXml( e.Arguments[0].ToString() );
                bool fileerror = false;
                try
                {
                    ds.ReadXml(fs);
                }
                catch
                {
                    fileerror = true;
                }

                // close the file
                fs.Close();
                if (fileerror)
                {
                    Console.WriteLine("XmlQuestHolder: Error in XML config file '{0}'", filename);
                    return;
                }
                // Check that at least a single table was loaded
                if (ds.Tables != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[XmlTableName] != null && ds.Tables[XmlTableName].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[XmlTableName].Rows)
                        {
                            bool valid_entry;
                            string strEntry = null;
                            bool boolEntry = true;
                            double doubleEntry = 0;
                            int intEntry = 0;
                            TimeSpan timespanEntry = TimeSpan.Zero;

                            valid_entry = true;
                            try
                            {
                                strEntry = (string)dr["Name"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Name = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Title"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.TitleString = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Note"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.NoteString = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Reward"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.RewardString = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Attachment"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.AttachmentString = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Objective1"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Objective1 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Objective2"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Objective2 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Objective3"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Objective3 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Objective4"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Objective4 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Objective5"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Objective5 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Description1"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Description1 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Description2"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Description2 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Description3"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Description3 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Description4"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Description4 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try
                            {
                                strEntry = (string)dr["Description5"];
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Description5 = strEntry;
                            }

                            valid_entry = true;
                            boolEntry = false;
                            try
                            {
                                boolEntry = bool.Parse((string)dr["PartyEnabled"]);
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.PartyEnabled = boolEntry;
                            }

                            valid_entry = true;
                            boolEntry = false;
                            try
                            {
                                boolEntry = bool.Parse((string)dr["AutoReward"]);
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.AutoReward = boolEntry;
                            }

                            valid_entry = true;
                            boolEntry = true;
                            try
                            {
                                boolEntry = bool.Parse((string)dr["CanSeeReward"]);
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.CanSeeReward = boolEntry;
                            }

                            valid_entry = true;
                            boolEntry = true;
                            try
                            {
                                boolEntry = bool.Parse((string)dr["Repeatable"]);
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.m_Repeatable = boolEntry;
                            }

                            valid_entry = true;
                            timespanEntry = TimeSpan.Zero;
                            try
                            {
                                timespanEntry = TimeSpan.Parse((string)dr["NextRepeatable"]);
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.m_NextRepeatable = timespanEntry;
                            }

                            valid_entry = true;
                            boolEntry = false;
                            try
                            {
                                boolEntry = bool.Parse((string)dr["PlayerMade"]);
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.PlayerMade = boolEntry;
                            }

                            valid_entry = true;
                            intEntry = 0;
                            try
                            {
                                intEntry = int.Parse((string)dr["PartyRange"]);
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.PartyRange = intEntry;
                            }

                            valid_entry = true;
                            doubleEntry = 0;
                            try
                            {
                                doubleEntry = double.Parse((string)dr["Expiration"]);
                            }
                            catch
                            {
                                valid_entry = false;
                            }
                            if (valid_entry)
                            {
                                this.Expiration = doubleEntry;
                            }
                        }
                    }
                }
            }
        }
    }
}