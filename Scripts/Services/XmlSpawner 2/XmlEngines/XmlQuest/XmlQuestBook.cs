using System;
using System.Collections;
using Server.Gumps;
using Server.Mobiles;

/*
** XmlQuestBook class
**
*/
namespace Server.Items
{
    [Flipable(0x1E5E, 0x1E5F)]
    public class PlayerQuestBoard : XmlQuestBook
    {
        public PlayerQuestBoard(Serial serial)
            : base(serial)
        {
        }

        [Constructable]
        public PlayerQuestBoard()
            : base(0x1e5e)
        {
            this.Movable = false;
            this.Name = "Player Quest Board";
            this.LiftOverride = true;    // allow players to store books in it
        }

        public override bool IsDecoContainer
        {
            get
            {
                return false;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class XmlQuestBook : Container
    {
        private PlayerMobile m_Owner;
        private bool m_Locked;
        public XmlQuestBook(Serial serial)
            : base(serial)
        {
        }

        [Constructable]
        public XmlQuestBook(int itemid)
            : this()
        {
            this.ItemID = itemid;
        }

        [Constructable]
        public XmlQuestBook()
            : base(0x2259)
        {
            //LootType = LootType.Blessed;
            this.Name = "QuestBook";
            this.Hue = 100;
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
        public bool Locked
        {
            get
            {
                return this.m_Locked;
            }
            set
            {
                this.m_Locked = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsCompleted
        {
            get
            {
                Item[] questitems = this.FindItemsByType(typeof(IXmlQuest));

                if (questitems == null || questitems.Length <= 0)
                    return false;

                for (int i = 0; i < questitems.Length; ++i)
                {
                    IXmlQuest q = questitems[i] as IXmlQuest;

                    // check completion and validity status of all quests held in the book
                    if (q == null || q.Deleted || !q.IsValid || !q.IsCompleted)
                        return false;
                }

                return true;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile))
                return;

            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                base.OnDoubleClick(from);
            }

            from.SendGump(new XmlQuestBookGump((PlayerMobile)from, this));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is IXmlQuest && !this.Locked)
            {
                return base.OnDragDrop(from, dropped);
            }
            else
            {
                return false;
            }
        }

        public virtual void Invalidate()
        {
            if (this.Owner != null)
            {
                this.Owner.SendMessage(String.Format("{0} Quests invalidated - '{1}' removed", this.TotalItems, this.Name));
            }
            this.Delete();
        }

        public override void OnItemLifted(Mobile from, Item item)
        {
            base.OnItemLifted(from, item);

            if (from is PlayerMobile && this.Owner == null)
            {
                this.Owner = from as PlayerMobile;
                this.LootType = LootType.Blessed;
                // flag the owner as carrying a questtoken assuming the book contains quests and then confirm it with CheckOwnerFlag
                this.Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag, true);
                this.CheckOwnerFlag();
            }
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);
    
            if (parent != null && parent is Container)
            {
                // find the parent of the container
                // note, the only valid additions are to the player pack.  Anything else is invalid.  This is to avoid exploits involving storage or transfer of questtokens
                object from = ((Container)parent).Parent;

                // check to see if it can be added
                if (from != null && from is PlayerMobile)
                {
                    // if it was not owned then allow it to go anywhere
                    if (this.Owner == null)
                    {
                        this.Owner = from as PlayerMobile;
                        
                        this.LootType = LootType.Blessed;
                        // could also bless all of the quests inside as well but not actually necessary since blessed containers retain their
                        // contents whether blessed or not, and when dropped the questtokens will be blessed

                        // flag the owner as carrying a questtoken
                        this.Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag, true);
                        this.CheckOwnerFlag();
                    }
                    else if (from as PlayerMobile != this.Owner || parent is BankBox)
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
                    // allow placement into npcs or drop on their corpses when owner is null
                    else if (!(from is Mobile) && !(parent is Corpse))
                    {
                        // in principle this should never be reached
                        // invalidate the token
                        this.CheckOwnerFlag();
    
                        this.Invalidate();
                    }
                }
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            this.CheckOwnerFlag();
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D point)
        {
            bool returnvalue = base.OnDroppedToWorld(from, point);

            from.SendGump(new XmlConfirmDeleteGump(from,this));

            //CheckOwnerFlag();
    
            //Invalidate();
            return false;
            //return returnvalue;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            
            writer.Write(this.m_Owner);
            writer.Write(this.m_Locked);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Owner = reader.ReadMobile() as PlayerMobile;
            this.m_Locked = reader.ReadBool();
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
    }
}