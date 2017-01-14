using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using System.IO;
using System.Collections.Generic;
using Server.Targeting;
using Server.Engines.PartySystem;
using System.Data;
using System.Xml;
using Server.Engines.XmlSpawner2;

/*
** XmlQuestToken class
**
** Version 1.10
** updated 10/26/04
** ArteGordon
**
** Version 1.10
** updated 10/26/04
** - modified RewardString to only support item rewards and added the new AttachmentString property for specifying attachment rewards.
**
** Version 1.09
** updated 9/03/04
** - added the autoreward and rewarditem properties that allow rewards to be automatically created and given upon quest completion
**
** Version 1.08
** updated 7/31/04
** - added the Description1-5 property to allow the quest objective descriptions to be explicitly specified.  When this string is defined it will override the default
** description provided for the different types of quest objectives.
**
** Version 1.07
** updated 7/28/04
** - add support for an additional property test argument to the quest keywords
** - mobs no longer say "Thank you", or "I have no need for that." when given items for GIVE type quests.
**
** Version 1.06
** updated 7/07/04
** - Added the GIVE type quest objective.  Using an objective string of the type "GIVE,mobname,item[,count]" or "GIVENAMED,mobname,itemname[,count]" will produce an objective
** that is satisfied when the specified item(s) are dragged and dropped onto the named mob.
** - Added the ESCORT type quest objective.  Using an objective string of the type "ESCORT,mobname" will produce an objective
** that is satisfied when the specified TalkingBaseEscortable class mob is successfully escorted to its destination.
** Note, the destination is specified by setting the Destination property on the escort to a region name.
** - changed the rules for invalid XmlQuestTokens to eliminate hording or trading so that they can no longer be placed in containers and will automatically
** return to the player's pack if the player attempts to place them in any invalid location (such as another container, a bankbox, or another player).
** If they are placed on ground they will be automatically deleted.
** - added optimizations for KILL type quests in which only players that are flagged as carrying XmlQuestTokens are checked on kills.
**
** Version 1.05
** updated 4/30/04
** - Added the RewardString property
**
** Version 1.04
** updated 3/25/04
** - Added the LoadConfig and ConfigFile properties that allow XmlQuestTokens to read their configuration information from an XML file.
** - Moved the TitleString and NoteString properties from the QuestNote class to the XmlQuestToken class.
**
** Version 1.03
** updated 3/25/04
** - Added the PartyEnabled and PartyRange properties that allow party members to benefit from KILL and COLLECT keywords.
**
** Version 1.02
** updated 3/20/04
** - fixed the COLLECT and COLLECTNAMED keywords to remove non-stackable items after collecting.  Thanks to nix4 for pointing this out.
**
* Version 1.01
** updated 2/26/04
** - modified expiration time reporting format
** - added State properties to support more complex objective status information
** - added killtask support with special objective specification keywords KILL,mobtype,count and KILLNAMED,mobname,count
** - added collecttask support with special objective specification keywords COLLECT,itemtype,count and COLLECTNAMED,itemname,count
**
** Version 1.0
** updated 1/07/04
*/
namespace Server.Items
{

    public class XmlQuestTokenPack : Container
    {
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            return false;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            return false;
        }

#if(NEWPARENT)
		public override void OnAdded(IEntity target)
#else
		public override void OnAdded(object target)
#endif
        {
            base.OnAdded(target);

            UpdateTotal(this, TotalType.Weight, 0);
            UpdateTotal(this, TotalType.Gold, 0);
            UpdateTotal(this, TotalType.Items, 0);
        }

        public sealed class ForcedContainerContent : Packet
        {
            public ForcedContainerContent(Mobile beholder, Item beheld)
                : base(0x3C)
            {
                List<Item> items = beheld.Items;
                int count = items.Count;

                this.EnsureCapacity(5 + (count * 19));

                long pos = m_Stream.Position;

                int written = 0;

                m_Stream.Write((ushort)0);

                for (int i = 0; i < count; ++i)
                {
                    Item child = items[i];

                    if (!child.Deleted )
                    {
                        Point3D loc = child.Location;

                        ushort cid = (ushort)child.ItemID;

                        if (cid > 0x3FFF)
                            cid = 0x9D7;

                        m_Stream.Write((int)child.Serial);
                        m_Stream.Write((ushort)cid);
                        m_Stream.Write((byte)0); // signed, itemID offset
                        m_Stream.Write((ushort)child.Amount);
                        m_Stream.Write((short)loc.X);
                        m_Stream.Write((short)loc.Y);
                        m_Stream.Write((int)beheld.Serial);
                        m_Stream.Write((ushort)child.Hue);

                        ++written;
                    }
                }

                m_Stream.Seek(pos, SeekOrigin.Begin);
                m_Stream.Write((ushort)written);
            }
        }

        public override void DisplayTo(Mobile to)
        {
            if (to == null) return;

            to.Send(new ContainerDisplay(this));
            to.Send(new ForcedContainerContent(to, this));

            if (ObjectPropertyList.Enabled)
            {
                List<Item> items = this.Items;

                for (int i = 0; i < items.Count; ++i)
                    to.Send(((Item)items[i]).OPLPacket);
            }
        }

        public XmlQuestTokenPack()
            : base(0x9B2)
        {
            Weight = 0;
        }

        public XmlQuestTokenPack(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public abstract class XmlQuestToken : Item, IXmlQuest
    {
        //public const PlayerFlag CarriedXmlQuestFlag = (PlayerFlag)0x00100000;

        private bool m_wasMoved = false;
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
        private string m_SkillTrigger = null;
        private bool m_Repeatable = true;
        private TimeSpan m_NextRepeatable;

        private Item m_RewardItem;
        private XmlAttachment m_RewardAttachment;
        private int m_RewardAttachmentSerialNumber;
        private bool m_AutoReward = false;
        private Container m_Pack;
        private bool m_CanSeeReward = false;
        private bool m_PlayerMade = false;
        private PlayerMobile m_Creator;
        private Container m_ReturnContainer;
        private string m_status_str;
        private int m_QuestDifficulty = 1;

        public XmlQuestToken(Serial serial)
            : base(serial)
        {
        }

        public XmlQuestToken()
        {
            //LootType = LootType.Blessed;
            TimeCreated = DateTime.UtcNow;
        }

        public XmlQuestToken(int itemID)
        {
            ItemID = itemID;
            //LootType = LootType.Blessed;
            TimeCreated = DateTime.UtcNow;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)14); // version
            // version 14
            if (m_Journal == null || m_Journal.Count == 0)
            {
                writer.Write((int)0);
            }
            else
            {
                writer.Write((int)m_Journal.Count);
                foreach (XmlQuest.JournalEntry e in m_Journal)
                {
                    writer.Write(e.EntryID);
                    writer.Write(e.EntryText);
                }
            }
            // version 13
            writer.Write(m_Repeatable);
            // version 12
            writer.Write(m_QuestDifficulty);
            // version 11
            writer.Write(m_AttachmentString);
            // version 10
            writer.Write(m_NextRepeatable);
            // version 9
            if (m_RewardAttachment != null)
                writer.Write(m_RewardAttachment.Serial.Value);
            else
                writer.Write((int)0);
            // version 8
            writer.Write(m_ReturnContainer);
            // version 7
            writer.Write(m_Pack);
            writer.Write(m_RewardItem);
            writer.Write(m_AutoReward);
            writer.Write(m_CanSeeReward);
            writer.Write(m_PlayerMade);
            writer.Write(m_Creator);
            // version 6
            writer.Write(m_Description1);
            writer.Write(m_Description2);
            writer.Write(m_Description3);
            writer.Write(m_Description4);
            writer.Write(m_Description5);
            // version 5
            writer.Write(m_Owner);
            // version 4
            writer.Write(m_RewardString);
            // version 3
            writer.Write(m_ConfigFile);
            writer.Write(m_NoteString);    // moved from the QuestNote class
            writer.Write(m_TitleString);   // moved from the QuestNote class

            // version 2
            writer.Write(m_PartyEnabled);
            writer.Write(m_PartyRange);
            // version 1
            writer.Write(m_State1);
            writer.Write(m_State2);
            writer.Write(m_State3);
            writer.Write(m_State4);
            writer.Write(m_State5);

            // version 0
            writer.Write(m_wasMoved);
            writer.Write(m_ExpirationDuration);
            writer.Write(m_TimeCreated);
            writer.Write(m_Objective1);
            writer.Write(m_Objective2);
            writer.Write(m_Objective3);
            writer.Write(m_Objective4);
            writer.Write(m_Objective5);
            writer.Write(m_Completed1);
            writer.Write(m_Completed2);
            writer.Write(m_Completed3);
            writer.Write(m_Completed4);
            writer.Write(m_Completed5);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 14:
                    {
                        int nentries = reader.ReadInt();

                        if (nentries > 0)
                        {
                            m_Journal = new List<XmlQuest.JournalEntry>();
                            for (int i = 0; i < nentries; i++)
                            {
                                string entryID = reader.ReadString();
                                string entryText = reader.ReadString();
                                m_Journal.Add(new XmlQuest.JournalEntry(entryID, entryText));
                            }
                        }

                        goto case 13;
                    }
                case 13:
                    {
                        m_Repeatable = reader.ReadBool();
                        goto case 12;
                    }
                case 12:
                    {
                        m_QuestDifficulty = reader.ReadInt();
                        goto case 11;
                    }
                case 11:
                    {
                        m_AttachmentString = reader.ReadString();
                        goto case 10;
                    }
                case 10:
                    {
                        m_NextRepeatable = reader.ReadTimeSpan();
                        goto case 9;
                    }
                case 9:
                    {
                        m_RewardAttachmentSerialNumber = reader.ReadInt();
                        goto case 8;
                    }
                case 8:
                    {
                        this.m_ReturnContainer = (Container)reader.ReadItem();
                        goto case 7;
                    }
                case 7:
                    {
                        this.m_Pack = (Container)reader.ReadItem();
                        this.m_RewardItem = reader.ReadItem();
                        this.m_AutoReward = reader.ReadBool();
                        this.m_CanSeeReward = reader.ReadBool();
                        this.m_PlayerMade = reader.ReadBool();
                        this.m_Creator = reader.ReadMobile() as PlayerMobile;
                        goto case 6;
                    }
                case 6:
                    {
                        this.m_Description1 = reader.ReadString();
                        this.m_Description2 = reader.ReadString();
                        this.m_Description3 = reader.ReadString();
                        this.m_Description4 = reader.ReadString();
                        this.m_Description5 = reader.ReadString();
                        goto case 5;
                    }
                case 5:
                    {
                        this.m_Owner = reader.ReadMobile() as PlayerMobile;
                        goto case 4;
                    }
                case 4:
                    {
                        this.m_RewardString = reader.ReadString();
                        goto case 3;
                    }
                case 3:
                    {
                        this.m_ConfigFile = reader.ReadString();
                        this.m_NoteString = reader.ReadString();
                        this.m_TitleString = reader.ReadString();
                        goto case 2;
                    }
                case 2:
                    {
                        this.m_PartyEnabled = reader.ReadBool();
                        this.m_PartyRange = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_State1 = reader.ReadString();
                        this.m_State2 = reader.ReadString();
                        this.m_State3 = reader.ReadString();
                        this.m_State4 = reader.ReadString();
                        this.m_State5 = reader.ReadString();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_wasMoved = reader.ReadBool();
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

        public static void Initialize()
        {
            foreach (Item item in World.Items.Values)
            {
                if (item is XmlQuestToken)
                {
                    XmlQuestToken t = item as XmlQuestToken;

                    if (t.Pack != null && !t.Pack.Deleted)
                    {

                        t.UpdateWeight();

                        t.UpdateTotal(t.Pack, TotalType.Weight, 0);
                        t.UpdateTotal(t.Pack, TotalType.Gold, 0);

                        t.RestoreRewardAttachment();

                    }
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!(from is PlayerMobile)) return;

            if (PlayerMade && (from == Creator) && (from == Owner))
            {
                from.SendGump(new XmlPlayerQuestGump((PlayerMobile)from, this));
            }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D point)
        {

            bool returnvalue = base.OnDroppedToWorld(from, point);

            from.SendGump(new XmlConfirmDeleteGump(from, this));

            //m_wasMoved  = true;

            //CheckOwnerFlag();

            //Invalidate();
            return false;
            //return returnvalue;
        }


        public override void OnDelete()
        {
            // remove any temporary quest attachments associated with this quest and quest owner
            XmlQuest.RemoveTemporaryQuestObjects(Owner, Name);

            base.OnDelete();

            // remove any reward items that might be attached to this
            ReturnReward();

            // and remove any pack
            if (Pack != null)
            {
                Pack.Delete();
            }
            CheckOwnerFlag();
        }

        public override void OnItemLifted(Mobile from, Item item)
        {
            base.OnItemLifted(from, item);

            if (from is PlayerMobile && PlayerMade && (Owner != null) && (Owner == Creator))
            {
                LootType = LootType.Regular;
            }
            else
                if (from is PlayerMobile && Owner == null)
                {
                    Owner = from as PlayerMobile;

                    LootType = LootType.Blessed;
                    // flag the owner as carrying a questtoken
                    Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag, true);
                }
        }

#if(NEWPARENT)
		public override void OnAdded(IEntity target)
#else
		public override void OnAdded(object target)
#endif
        {
            base.OnAdded(target);

            if ((target != null) && target is Container)
            {
                // find the parent of the container
                // note, the only valid additions are to the player pack or a questbook.  Anything else is invalid.  This is to avoid exploits involving storage or transfer of questtokens
                // make an exception for playermade quests that can be put on playervendors
                object parentOfTarget = ((Container)target).Parent;

                // if this is a QuestBook then allow additions if it is in a players pack or it is a player quest
                if ((parentOfTarget != null) && parentOfTarget is Container && target is XmlQuestBook)
                {
                    parentOfTarget = ((Container)parentOfTarget).Parent;
                }



                // check to see if it can be added.
                // allow playermade quests to be placed in playervendors or in xmlquestbooks that are in the world (supports the playerquestboards)
                if (PlayerMade && (((parentOfTarget != null) && parentOfTarget is PlayerVendor) ||
                    ((parentOfTarget == null) && target is XmlQuestBook)))
                {
                    CheckOwnerFlag();

                    Owner = null;

                    LootType = LootType.Regular;
                }
                else
                    if ((parentOfTarget != null) && (parentOfTarget is PlayerMobile) && PlayerMade && (Owner != null) && ((Owner == Creator) || (Creator == null)))
                    {
                        // check the old owner
                        CheckOwnerFlag();

                        Owner = parentOfTarget as PlayerMobile;

                        // first owner will become creator by default
                        if (Creator == null)
                            Creator = Owner;

                        LootType = LootType.Blessed;

                        // flag the new owner as carrying a questtoken
                        Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag, true);

                    }
                    else
                        if ((parentOfTarget != null) && (parentOfTarget is PlayerMobile))
                        {
                            if (Owner == null)
                            {
                                Owner = parentOfTarget as PlayerMobile;

                                LootType = LootType.Blessed;

                                // flag the owner as carrying a questtoken
                                Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag, true);
                            }
                            else
                                if ((parentOfTarget as PlayerMobile != Owner) || (target is BankBox))
                                {
                                    // tried to give it to another player or placed it in the players bankbox. try to return it to the owners pack
                                    Owner.AddToBackpack(this);
                                    /*
                                    // this has been added to a player who is not the owner so invalidate it
                                    WasMoved = true;

                                    CheckOwnerFlag();

                                    Invalidate();
                                    */

                                }
                        }
                        else
                        {
                            if (Owner != null)
                            {
                                // try to return it to the owners pack
                                Owner.AddToBackpack(this);
                            }
                            // allow placement into npcs or drop on their corpses when owner is null
                            else
                                if (!(parentOfTarget is Mobile) && !(target is Corpse) && parentOfTarget != null)
                                {

                                    // in principle this should never be reached

                                    // invalidate the token

                                    WasMoved = true;

                                    CheckOwnerFlag();

                                    Invalidate();
                                }
                        }
            }
        }

        private List<XmlQuest.JournalEntry> m_Journal;
        public List<XmlQuest.JournalEntry> Journal { get { return m_Journal; } set { m_Journal = value; } }
        private static char[] colondelim = new char[1] { ':' };

        public string AddJournalEntry
        {
            set
            {
                if (value == null) return;

                // parse the value
                string[] args = value.Split(colondelim, 2);

                if (args == null) return;

                string entryID = null;
                string entryText = null;
                if (args.Length > 0)
                {
                    entryID = args[0].Trim();
                }

                if (entryID == null || entryID.Length == 0) return;

                if (args.Length > 1)
                {
                    entryText = args[1].Trim();
                }


                // allocate a new journal if none exists
                if (m_Journal == null) m_Journal = new List<XmlQuest.JournalEntry>();

                // go through the existing journal to find a matching ID
                XmlQuest.JournalEntry foundEntry = null;

                foreach (XmlQuest.JournalEntry e in m_Journal)
                {
                    if (e.EntryID == entryID)
                    {
                        foundEntry = e;
                        break;
                    }
                }

                if (foundEntry != null)
                {

                    if (entryText == null || entryText.Length == 0)
                    {
                        // delete the entry
                        m_Journal.Remove(foundEntry);
                    }
                    else
                    {
                        // just replace the text
                        foundEntry.EntryText = entryText;
                    }
                }
                else
                {
                    if (entryText != null && entryText.Length != 0)
                    {
                        // add the new entry
                        m_Journal.Add(new XmlQuest.JournalEntry(entryID, entryText));
                    }
                }
            }
        }


        private void QuestCompletionAttachment()
        {
            bool complete = IsCompleted;

            // is this quest repeatable
            if ((!Repeatable || NextRepeatable > TimeSpan.Zero) && complete)
            {
                double expiresin = Repeatable ? NextRepeatable.TotalMinutes : 0;

                // then add an attachment indicating that it has already been done
                XmlAttach.AttachTo(Owner, new XmlQuestAttachment(this.Name, expiresin));
            }

            // have quest points been enabled?
            if (XmlQuest.QuestPointsEnabled && complete && !PlayerMade)
            {
                XmlQuestPoints.GiveQuestPoints(Owner, this);
            }
        }

        private void PackItem(Item item)
        {
            if ((m_Pack == null || m_Pack.Deleted) && (Owner != null) /* && (this.RootParent is Mobile) */)
            {
                m_Pack = new XmlQuestTokenPack();

                m_Pack.Layer = Layer.Invalid;

                //m_Pack.Parent = Owner;
                m_Pack.Parent = this;
                //m_Pack.Map = Owner.Map;
                m_Pack.Map = this.Map;
                m_Pack.Location = this.Location;

            }

            if ((m_Pack != null) && !m_Pack.Deleted)
            {

                m_Pack.DropItem(item);
                PackItemsMovable(m_Pack, false);

                UpdateTotal(m_Pack, TotalType.Weight, 0);
                UpdateTotal(m_Pack, TotalType.Gold, 0);

            }
            // make sure the weight and gold of the questtoken is updated to reflect the weight of added rewards in playermade quests to avoid
            // exploits where quests are used as zero weight containers

            UpdateWeight();
        }


        private void CalculateWeight(Item target)
        {
            if (target is Container)
            {
                int gold = 0;
                int weight = 0;
                int nitems = 0;
                foreach (Item i in ((Container)target).FindItemsByType(typeof(Item), false))
                {
                    // make sure gold amount is consistent with totalgold
                    if (i is Gold)
                    {
                        UpdateTotal(i, TotalType.Gold, i.Amount);
                    }

                    if (i is Container)
                    {
                        CalculateWeight(i);
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

                UpdateTotal((Container)target, TotalType.Weight, weight);
                UpdateTotal((Container)target, TotalType.Gold, gold);
                UpdateTotal((Container)target, TotalType.Items, nitems);
            }
        }


        private void UpdateWeight()
        {


            // update the weight of the reward item itself
            CalculateWeight(m_RewardItem);

            // make sure the weight and gold of the questtoken is updated to reflect the weight of added rewards
            if (m_RewardItem != null && !m_RewardItem.Deleted && PlayerMade)
            {
                if (m_RewardItem is Container)
                {
                    UpdateTotal(this, TotalType.Weight, m_RewardItem.TotalWeight + (int)m_RewardItem.Weight);
                    UpdateTotal(this, TotalType.Gold, m_RewardItem.TotalGold);
                }
                else
                {
                    UpdateTotal(this, TotalType.Weight, (int)(m_RewardItem.Weight * m_RewardItem.Amount));
                    UpdateTotal(this, TotalType.Gold, m_RewardItem.TotalGold);

                }

            }
            else
            {
                UpdateTotal(this, TotalType.Weight, 0);
                UpdateTotal(this, TotalType.Gold, 0);
            }

        }

        private void ReturnReward()
        {
            if (m_RewardItem != null)
            {

                CheckRewardItem();

                // if this was player made, then return the item to the creator
                if (PlayerMade && (Creator != null) && !Creator.Deleted)
                {
                    m_RewardItem.Movable = true;
                    if (m_Pack != null && !m_Pack.Deleted)
                    {
                        // make sure all of the items in the pack are movable as well
                        PackItemsMovable(m_Pack, true);
                    }
                    bool returned = false;

                    //RefreshPackLocation(Creator, false);

                    if ((ReturnContainer != null) && !ReturnContainer.Deleted)
                    {
                        returned = ReturnContainer.TryDropItem(Creator, m_RewardItem, false);
                        //ReturnContainer.DropItem(m_RewardItem);
                    }
                    if (!returned)
                    {

                        returned = Creator.AddToBackpack(m_RewardItem);

                    }
                    if (returned)
                    {
                        Creator.SendMessage("Your reward {0} was returned from quest {1}", m_RewardItem.GetType().Name, Name);
                        //AddMobileWeight(Creator, m_RewardItem);
                    }
                    else
                    {
                        Creator.SendMessage("Attempted to return reward {0} from quest {1} : containers full.", m_RewardItem.GetType().Name, Name);
                    }
                }
                else
                {
                    // just delete it
                    m_RewardItem.Delete();

                }
                m_RewardItem = null;
                UpdateWeight();
            }
            if (m_RewardAttachment != null)
            {
                // delete any remaining attachments
                m_RewardAttachment.Delete();
            }

        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public new string Name
        {
            get
            {
                if (PlayerMade)
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
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Creator
        {
            get { return m_Creator; }
            set { m_Creator = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Difficulty
        {
            get { return m_QuestDifficulty; }
            set { m_QuestDifficulty = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string NoteString
        {
            get { return m_NoteString; }
            set { m_NoteString = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AutoReward
        {
            get { return m_AutoReward; }
            set { m_AutoReward = value; }
        }
        
        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanSeeReward
        {
            // dont allow rewards to be seen on xmlquesttokens
            get { return false; }
            set { }
        }
        /*
                [CommandProperty( AccessLevel.GameMaster )]
                public bool CanSeeReward
                {
                    get{ return m_CanSeeReward; }
                    set { m_CanSeeReward = value; }
                }
        */
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerMade
        {
            get { return m_PlayerMade; }
            set { m_PlayerMade = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Container ReturnContainer
        {
            get { return m_ReturnContainer; }
            set { m_ReturnContainer = value; }
        }

        public Container Pack
        {
            get { return m_Pack; }
        }

        private void PackItemsMovable(Container pack, bool canmove)
        {
            if (pack == null) return;

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
            m_RewardAttachment = XmlAttach.FindAttachmentBySerial(m_RewardAttachmentSerialNumber);
        }

        public XmlAttachment RewardAttachment
        {
            get
            {
                // if the reward item is not set, and the reward string is specified, then use the reward string to construct and assign the
                // reward item
                // dont allow player made quests to use the rewardstring creation feature
                if (m_RewardAttachment != null && m_RewardAttachment.Deleted) m_RewardAttachment = null;

                if ((m_RewardAttachment == null || m_RewardAttachment.Deleted) &&
                    (m_AttachmentString != null) && !PlayerMade)
                {
                    object o = XmlQuest.CreateItem(this, m_AttachmentString, out m_status_str, typeof(XmlAttachment));
                    if (o is Item)
                    {
                        // should never get here
                        ((Item)o).Delete();
                    }
                    else
                        if (o is XmlAttachment)
                        {
                            m_RewardAttachment = o as XmlAttachment;
                            m_RewardAttachment.OwnedBy = this;
                        }
                }

                return m_RewardAttachment;
            }
            set
            {
                // get rid of any existing attachment
                if (m_RewardAttachment != null && !m_RewardAttachment.Deleted)
                {
                    m_RewardAttachment.Delete();
                }

                m_RewardAttachment = value;
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
                if ((m_RewardItem == null || m_RewardItem.Deleted) &&
                    (m_RewardString != null) && !PlayerMade)
                {
                    string status_str;
                    object o = XmlQuest.CreateItem(this, m_RewardString, out status_str, typeof(Item));
                    if (o is Item)
                    {
                        m_RewardItem = o as Item;
                    }
                    else
                        if (o is XmlAttachment)
                        {
                            // should never get here
                            ((XmlAttachment)o).Delete();
                        }
                }

                // place it in the xmlquesttoken pack if it isnt already there
                if ((m_RewardItem != null && !m_RewardItem.Deleted) && ((Pack == null) || Pack.Deleted || (m_RewardItem.Parent != Pack)))
                {
                    PackItem(m_RewardItem);
                }

                return m_RewardItem;
            }
            set
            {
                // get rid of any existing reward item if it has been assigned
                if (m_RewardItem != null && !m_RewardItem.Deleted)
                {

                    ReturnReward();
                }

                // and assign the new item
                m_RewardItem = value;

                // and put it in the xmlquesttoken pack
                if (m_RewardItem != null && !m_RewardItem.Deleted)
                {
                    PackItem(m_RewardItem);
                }

                // is this currently carried by a mobile?
                if (m_RewardItem.RootParent != null && m_RewardItem.RootParent is Mobile)
                {
                    // if so then remove it
                    ((Mobile)(m_RewardItem.RootParent)).RemoveItem(m_RewardItem);

                }


            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string TitleString
        {
            get { return m_TitleString; }
            set { m_TitleString = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string RewardString
        {
            get { return m_RewardString; }
            set { m_RewardString = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string AttachmentString
        {
            get { return m_AttachmentString; }
            set { m_AttachmentString = value; }
        }


        [CommandProperty(AccessLevel.GameMaster)]
        public string ConfigFile
        {
            get { return m_ConfigFile; }
            set { m_ConfigFile = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LoadConfig
        {
            get { return false; }
            set { if (value == true) LoadXmlConfig(ConfigFile); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PartyEnabled
        {
            get { return m_PartyEnabled; }
            set { m_PartyEnabled = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PartyRange
        {
            get { return m_PartyRange; }
            set { m_PartyRange = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string State1
        {
            get { return m_State1; }
            set { m_State1 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string State2
        {
            get { return m_State2; }
            set { m_State2 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string State3
        {
            get { return m_State3; }
            set { m_State3 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string State4
        {
            get { return m_State4; }
            set { m_State4 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string State5
        {
            get { return m_State5; }
            set { m_State5 = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Description1
        {
            get { return m_Description1; }
            set { m_Description1 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Description2
        {
            get { return m_Description2; }
            set { m_Description2 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Description3
        {
            get { return m_Description3; }
            set { m_Description3 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Description4
        {
            get { return m_Description4; }
            set { m_Description4 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Description5
        {
            get { return m_Description5; }
            set { m_Description5 = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Objective1
        {
            get { return m_Objective1; }
            set { m_Objective1 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Objective2
        {
            get { return m_Objective2; }
            set { m_Objective2 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Objective3
        {
            get { return m_Objective3; }
            set { m_Objective3 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Objective4
        {
            get { return m_Objective4; }
            set { m_Objective4 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Objective5
        {
            get { return m_Objective5; }
            set { m_Objective5 = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed1
        {
            get { return m_Completed1; }
            set
            {
                m_Completed1 = value;
                CheckAutoReward();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed2
        {
            get { return m_Completed2; }
            set
            {
                m_Completed2 = value;
                CheckAutoReward();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed3
        {
            get { return m_Completed3; }
            set
            {
                m_Completed3 = value;
                CheckAutoReward();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed4
        {
            get { return m_Completed4; }
            set
            {
                m_Completed4 = value;
                CheckAutoReward();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed5
        {
            get { return m_Completed5; }
            set
            {
                m_Completed5 = value;
                CheckAutoReward();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Status
        {
            get { return m_status_str; }
            set { m_status_str = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool WasMoved
        {
            get { return m_wasMoved; }
            set { m_wasMoved = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeCreated
        {
            get { return m_TimeCreated; }
            set { m_TimeCreated = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Expiration
        {
            get
            {
                return m_ExpirationDuration;
            }
            set
            {
                // cap the max value at 100 years
                if (value > 876000)
                {
                    m_ExpirationDuration = 876000;
                }
                else
                {
                    m_ExpirationDuration = value;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ExpiresIn
        {
            get
            {
                if (m_ExpirationDuration > 0)
                {
                    // if this is a player created quest, then refresh the expiration time until it is in someone elses possession
                    /*
                     if(PlayerMade && ((Owner == Creator) || (Owner == null)))
                     {
                         m_TimeCreated = DateTime.UtcNow;
                     }
                     */
                    return (m_TimeCreated + TimeSpan.FromHours(m_ExpirationDuration) - DateTime.UtcNow);
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
                if (((m_ExpirationDuration > 0) && (ExpiresIn <= TimeSpan.FromHours(0))))
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
                return m_Repeatable;
            }
            set
            {
                m_Repeatable = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual TimeSpan NextRepeatable
        {
            get
            {
                return m_NextRepeatable;
            }
            set
            {
                m_NextRepeatable = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool AlreadyDone
        {
            get
            {
                // look for a quest attachment with the current quest name
                if (XmlAttach.FindAttachment(Owner, typeof(XmlQuestAttachment), Name) == null)
                    return false;

                return true;

            }
        }

        public virtual string ExpirationString
        {
            get
            {
                if (AlreadyDone)
                {
                    return "Already done";
                }
                else
                    if (m_ExpirationDuration <= 0)
                    {
                        return "Never expires";
                    }
                    else
                        if (IsExpired)
                        {
                            return "Expired";
                        }
                        else
                        {
                            TimeSpan ts = ExpiresIn;

                            int days = (int)ts.TotalDays;
                            int hours = (int)(ts - TimeSpan.FromDays(days)).TotalHours;
                            int minutes = (int)(ts - TimeSpan.FromHours(hours)).TotalMinutes;
                            int seconds = (int)(ts - TimeSpan.FromMinutes(minutes)).TotalSeconds;

                            if (days > 0)
                            {
                                return String.Format("Expires in {0} days {1} hrs", days, hours);
                            }
                            else
                                if (hours > 0)
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
                if (WasMoved || IsExpired)
                {
                    // eliminate reward definitions
                    RewardString = null;
                    AttachmentString = null;

                    // return any reward items
                    ReturnReward();

                    // and get rid of the pack
                    if (Pack != null)
                        Pack.Delete();

                    return false;
                }
                else
                    if (AlreadyDone)
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
                if (IsValid &&
                    (Completed1 || Objective1 == null || (Objective1.Length == 0)) &&
                    (Completed2 || Objective2 == null || (Objective2.Length == 0)) &&
                    (Completed3 || Objective3 == null || (Objective3.Length == 0)) &&
                    (Completed4 || Objective4 == null || (Objective4.Length == 0)) &&
                    (Completed5 || Objective5 == null || (Objective5.Length == 0))
                    )
                    return true;
                else
                    return false;
            }
        }


        public bool HandlesOnSkillUse { get { return (IsValid && m_SkillTrigger != null && m_SkillTrigger.Length > 0); } }

        public void OnSkillUse(Mobile m, Skill skill, bool success)
        {
        }

        private void CheckOwnerFlag()
        {
            if (Owner != null && !Owner.Deleted)
            {
                // need to check to see if any other questtoken items are owned
                // search the Owners top level pack for an xmlquesttoken
                List<Item> list = XmlQuest.FindXmlQuest(Owner);

                if (list == null || list.Count == 0)
                {

                    // if none remain then flag the ower as having none
                    Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag, false);
                }
            }


        }

        public virtual void Invalidate()
        {
            //Hue = 32;
            //LootType = LootType.Regular;
            if (Owner != null)
            {
                Owner.SendMessage(String.Format("Quest invalidated - '{0}' removed", Name));
            }
            this.Delete();
        }


        public void CheckRewardItem()
        {
            // go through all reward items and delete anything that is movable.  This blocks any exploits where players might
            // try to add items themselves
            if (m_RewardItem != null && !m_RewardItem.Deleted && m_RewardItem is Container)
            {
                foreach (Item i in ((Container)m_RewardItem).FindItemsByType(typeof(Item), true))
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
            if (!this.Deleted && AutoReward && IsCompleted && Owner != null &&
                ((RewardItem != null && !m_RewardItem.Deleted) || (RewardAttachment != null && !m_RewardAttachment.Deleted)))
            {
                if (RewardItem != null)
                {
                    // make sure nothing has been added to the pack other than the original reward items
                    CheckRewardItem();

                    m_RewardItem.Movable = true;
                    if (m_Pack != null)
                    {
                        // make sure all of the items in the pack are movable as well
                        PackItemsMovable(m_Pack, true);
                    }
                    //RefreshPackLocation(Owner, false);


                    Owner.AddToBackpack(m_RewardItem);
                    //AddMobileWeight(Owner,m_RewardItem);

                    m_RewardItem = null;
                }

                if (RewardAttachment != null)
                {
                    Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(AttachToCallback), new object[] { Owner, m_RewardAttachment });

                    m_RewardAttachment = null;
                }

                Owner.SendMessage(String.Format("{0} completed. You receive the quest reward!", Name));
                this.Delete();
            }
        }

        public void AttachToCallback(object state)
        {
            object[] args = (object[])state;

            XmlAttach.AttachTo(args[0], (XmlAttachment)args[1]);
        }

        private const string XmlTableName = "Properties";
        private const string XmlDataSetName = "XmlQuestToken";

        public void LoadXmlConfig(string filename)
        {
            if (filename == null || filename.Length <= 0) return;

            // Check if the file exists
            if (System.IO.File.Exists(filename) == true)
            {
                FileStream fs = null;
                try
                {
                    fs = File.Open(filename, FileMode.Open, FileAccess.Read);
                }
                catch { }

                if (fs == null)
                {
                    Status = String.Format("Unable to open {0} for loading", filename);
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
                catch { fileerror = true; }
                // close the file
                fs.Close();
                if (fileerror)
                {
                    Console.WriteLine("XmlQuestToken: Error in XML config file '{0}'", filename);
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
                            try { strEntry = (string)dr["Name"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.Name = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Title"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.TitleString = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Note"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.NoteString = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Reward"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.RewardString = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Attachment"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.AttachmentString = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Objective1"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.Objective1 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Objective2"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.Objective2 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Objective3"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.Objective3 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Objective4"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.Objective4 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Objective5"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.Objective5 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Description1"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.Description1 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Description2"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.Description2 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Description3"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.Description3 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Description4"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.Description4 = strEntry;
                            }

                            valid_entry = true;
                            strEntry = null;
                            try { strEntry = (string)dr["Description5"]; }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.Description5 = strEntry;
                            }

                            valid_entry = true;
                            boolEntry = false;
                            try { boolEntry = bool.Parse((string)dr["PartyEnabled"]); }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.PartyEnabled = boolEntry;
                            }

                            valid_entry = true;
                            boolEntry = false;
                            try { boolEntry = bool.Parse((string)dr["AutoReward"]); }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.AutoReward = boolEntry;
                            }

                            valid_entry = true;
                            boolEntry = true;
                            try { boolEntry = bool.Parse((string)dr["CanSeeReward"]); }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.CanSeeReward = boolEntry;
                            }

                            valid_entry = true;
                            boolEntry = true;
                            try { boolEntry = bool.Parse((string)dr["Repeatable"]); }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.m_Repeatable = boolEntry;
                            }

                            valid_entry = true;
                            timespanEntry = TimeSpan.Zero;
                            try { timespanEntry = TimeSpan.Parse((string)dr["NextRepeatable"]); }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.m_NextRepeatable = timespanEntry;
                            }

                            valid_entry = true;
                            boolEntry = false;
                            try { boolEntry = bool.Parse((string)dr["PlayerMade"]); }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.PlayerMade = boolEntry;
                            }

                            valid_entry = true;
                            intEntry = 0;
                            try { intEntry = int.Parse((string)dr["PartyRange"]); }
                            catch { valid_entry = false; }
                            if (valid_entry)
                            {
                                this.PartyRange = intEntry;
                            }

                            valid_entry = true;
                            doubleEntry = 0;
                            try { doubleEntry = double.Parse((string)dr["Expiration"]); }
                            catch { valid_entry = false; }
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
