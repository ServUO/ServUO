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


/*
** XmlQuestBook class
**
*/
namespace Server.Items
{
    [Flipable( 0x1E5E, 0x1E5F )]
	public class PlayerQuestBoard : XmlQuestBook
   {

       public override bool IsDecoContainer
       {
           get { return false; }
       }

        public PlayerQuestBoard( Serial serial ) : base( serial )
        {
        }

        [Constructable]
        public PlayerQuestBoard() : base( 0x1e5e )
        {
            Movable = false;
            Name = "Player Quest Board";
            LiftOverride = true;    // allow players to store books in it
        }
        
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 ); // version

        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

        }
    }
    

	public class XmlQuestBook : Container
   {

        private PlayerMobile m_Owner;
        private bool m_Locked;

        [CommandProperty( AccessLevel.GameMaster )]
        public PlayerMobile Owner
        {   get{ return m_Owner; }
            set { m_Owner = value; }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public bool Locked
        {   get{ return m_Locked; }
            set { m_Locked = value; }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsCompleted
        {   get{
                Item [] questitems = this.FindItemsByType(typeof(IXmlQuest));

                if(questitems == null || questitems.Length <= 0)
                    return false;

				for ( int i = 0; i < questitems.Length; ++i )
				{
					IXmlQuest q = questitems[i] as IXmlQuest;

                    // check completion and validity status of all quests held in the book
                    if(q == null || q.Deleted || !q.IsValid || !q.IsCompleted) return false;

                }

                return true;
            }
        }


        public XmlQuestBook( Serial serial ) : base( serial )
        {
        }

        [Constructable]
        public XmlQuestBook(int itemid) : this( )
        {
            ItemID = itemid;
        }

        [Constructable]
        public XmlQuestBook() : base( 0x2259 )
        {
            //LootType = LootType.Blessed;
            Name = "QuestBook";
            Hue = 100;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if(!(from is PlayerMobile)) return;

            if(from.AccessLevel >= AccessLevel.GameMaster)
            {
                base.OnDoubleClick(from);
            }

            from.SendGump( new XmlQuestBookGump( (PlayerMobile)from, this ) );
        }
        
        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if(dropped is IXmlQuest  && !Locked)
            {
                return base.OnDragDrop(from,dropped);
            } else
            {
                return false;
            }
        }

        private void CheckOwnerFlag()
        {
            if(Owner != null && !Owner.Deleted)
            {
                // need to check to see if any other questtoken items are owned
                // search the Owners top level pack for an xmlquest
                List<Item> list = XmlQuest.FindXmlQuest(Owner);

                if(list == null || list.Count == 0)
                {
                    // if none remain then flag the ower as having none
                    Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag,false);
                }

            }
        }

        public virtual void Invalidate()
        {

            if(Owner != null)
                {
                    Owner.SendMessage(String.Format("{0} Quests invalidated - '{1}' removed", TotalItems,Name));
                }
            this.Delete();
        }
        
        public override void OnItemLifted(Mobile from, Item item)
        {
            base.OnItemLifted(from,item);

            if(from is PlayerMobile && Owner == null)
            {
                Owner = from as PlayerMobile;
                LootType = LootType.Blessed;
                // flag the owner as carrying a questtoken assuming the book contains quests and then confirm it with CheckOwnerFlag
                Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag,true);
                CheckOwnerFlag();
            }
        }


#if(NEWPARENT)
		public override void OnAdded(IEntity parent)
#else
		public override void OnAdded(object parent)
#endif
        {
            base.OnAdded(parent);
    
            if(parent != null && parent is Container)
            {
                // find the parent of the container
                // note, the only valid additions are to the player pack.  Anything else is invalid.  This is to avoid exploits involving storage or transfer of questtokens
                object from = ((Container)parent).Parent;

                // check to see if it can be added
                if(from != null && from is PlayerMobile)
                {
                    // if it was not owned then allow it to go anywhere
                    if(Owner == null)
                    {
                        Owner = from as PlayerMobile;
                        
                        LootType = LootType.Blessed;
                        // could also bless all of the quests inside as well but not actually necessary since blessed containers retain their
                        // contents whether blessed or not, and when dropped the questtokens will be blessed

                        // flag the owner as carrying a questtoken
                        Owner.SetFlag(XmlQuest.CarriedXmlQuestFlag,true);
                        CheckOwnerFlag();
                    } else
                    if(from as PlayerMobile != Owner || parent is BankBox)
                    {
                        // tried to give it to another player or placed it in the players bankbox. try to return it to the owners pack
                        Owner.AddToBackpack(this);
                    }
                } else
                {
                    if(Owner != null)
                    {
                        // try to return it to the owners pack
                        Owner.AddToBackpack(this);
                    }
                    // allow placement into npcs or drop on their corpses when owner is null
                    else 
                    if(!(from is Mobile) && !(parent is Corpse))
                    {
                        // in principle this should never be reached

                        // invalidate the token

                        CheckOwnerFlag();
    
                        Invalidate();
                    }
                }
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            CheckOwnerFlag();
        }

        public override bool OnDroppedToWorld(Mobile from,Point3D point)
        {

            bool returnvalue = base.OnDroppedToWorld(from,point);

            from.SendGump( new XmlConfirmDeleteGump(from,this));

            //CheckOwnerFlag();
    
            //Invalidate();
            return false;
            //return returnvalue;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 ); // version
            
            writer.Write( m_Owner);
            writer.Write( m_Locked);
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            this.m_Owner = reader.ReadMobile() as PlayerMobile;
            this.m_Locked = reader.ReadBool();
        }
   }
}
