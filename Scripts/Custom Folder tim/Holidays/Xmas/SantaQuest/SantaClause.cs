//Created by Milva
using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Network;
using Server.Spells;
using Server.Accounting;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "santa clause Corpse" )]
	public class SantaClause : Mobile
	{
        public virtual bool IsInvulnerable{ get{ return true; } }
		
		[Constructable]
		public SantaClause ()
		{
			Name = "SantaClause";
                        
			Body = 400;
            Hue = 1002;
			CantWalk = true;
			Blessed = true;
            
			
			
          
            Item longpants;
			longpants = new LongPants();
			longpants.Hue = 1109;
			AddItem( longpants );
                         

			Item tunic;
			tunic = new Tunic();
			tunic.Hue = 33;
			AddItem( tunic );


            Item leatherninjabelt;
            leatherninjabelt = new LeatherNinjaBelt();
            leatherninjabelt.Hue = 1109;
            AddItem(leatherninjabelt);
            
                        
            FacialHairItemID = 0x204C;
            FacialHairHue = 1153;

            Item JesterHat;
            JesterHat = new JesterHat();
            JesterHat.Hue = 33;
            AddItem(JesterHat);
            
          

              HairItemID = 0x203C;
              HairHue = 1153;

            Item boots;
            boots = new Boots();
            boots.Hue = 1109;
            AddItem( boots );


           
		}

        public SantaClause(Serial serial)
            : base(serial)
		{
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
	    { 
	        base.GetContextMenuEntries( from, list );
            list.Add(new SantaClauseEntry(from, this)); 
	    } 

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public class SantaClauseEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			private Mobile m_Giver;

            public SantaClauseEntry(Mobile from, Mobile giver)
                : base(6146, 3)
			{
				m_Mobile = from;
				m_Giver = giver;
			}

			public override void OnClick()
			{
				if( !( m_Mobile is PlayerMobile ) )
					return;
				
				PlayerMobile mobile = (PlayerMobile) m_Mobile;

                if (!mobile.HasGump(typeof(SantaQuestGump)))
				{
                    mobile.SendGump(new SantaQuestGump(mobile));
						
				} 
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
            Mobile m = from;

            PlayerMobile pm = from as PlayerMobile;
            Account acct = (Account)from.Account;
            bool RedPaintBarrelRecieved = Convert.ToBoolean(acct.GetTag("SantaReward"));

			if ( pm != null && dropped is RedPaintBarrel)
            {
               

				if (!RedPaintBarrelRecieved) //added account tag check
                {
					dropped.Delete(); //	
                    pm.AddToBackpack(new SantasGiftBox2013());
					SendMessage( "Thank you for your help!" );
                    acct.SetTag("SantaReward", "true");
					this.PrivateOverheadMessage( MessageType.Regular, 1153, false, "Thank you, this is exactly what I needed!", pm.NetState );
                    pm.SendMessage("You have been awarded a christmas gift for your quest completion!");
					return true;
                }
         		
				else
         		{
         			pm.SendMessage("You are so kind to have taken the time to help me obtain a Barrel Of Red Paint.");
                    dropped.Delete();
					return true;
				}
         	}
			else
         	{
				this.PrivateOverheadMessage( MessageType.Regular, 1153, false, "I did not ask for this item.", pm.NetState );
				return false;
     		}
		}
	}
}
