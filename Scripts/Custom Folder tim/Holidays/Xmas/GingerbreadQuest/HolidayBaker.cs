//Created by Milva
using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Gumps;
using Server.Misc;
using Server.ContextMenus;
using Server.Network;
using Server.Spells;
using Server.Accounting;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "holiday baker' Corpse" )]
	public class HolidayBaker : Mobile
	{
        public virtual bool IsInvulnerable{ get{ return true; } }
		
		[Constructable]
		public HolidayBaker ()
		{
			Name = "Tilly";
                        
			Body = 401;
            Hue = 1002;
			CantWalk = true;
			Blessed = true;
			
			Item skirt;
			skirt = new Skirt();
			skirt.Hue = 1368;
			AddItem( skirt );
          

			Item shirt;
			shirt = new Shirt();
            skirt.Hue = 1368;
			AddItem( shirt );
          

			Item shoes;
			shoes = new Shoes();
			shoes.Hue = 1368;
			AddItem( shoes );
            

			Item JesterHat;
            JesterHat = new JesterHat();
            JesterHat.Hue = 1368;
            AddItem(JesterHat);
            

            Item FullApron;
            FullApron = new FullApron();
            FullApron .Hue = 1368;
            AddItem(FullApron);

            Item LongHair = new LongHair(8252);
            LongHair.Movable = false;
            LongHair.Hue = 1153;
            AddItem(LongHair);

           
		}

        public HolidayBaker(Serial serial)
            : base(serial)
		{
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
	    { 
	        base.GetContextMenuEntries( from, list );
            list.Add(new HolidayBakerEntry(from, this)); 
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

		public class HolidayBakerEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			private Mobile m_Giver;

            public HolidayBakerEntry(Mobile from, Mobile giver)
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

                if (!mobile.HasGump(typeof(Gingerbreadquestgump)))
				{
                    mobile.SendGump(new Gingerbreadquestgump(mobile));
						
				} 
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
            Mobile m = from;

            PlayerMobile pm = from as PlayerMobile;
            Account acct = (Account)from.Account;
            bool GingerbreadDoughRecieved = Convert.ToBoolean(acct.GetTag("HolidayReward"));

			if ( pm != null && dropped is GingerbreadDough)
            {
               

				if (!GingerbreadDoughRecieved) //added account tag check
                {
					dropped.Delete(); //	
                    pm.AddToBackpack(new SantaStatuette());
					SendMessage( "Thank you for your help!" );
                    acct.SetTag("HolidayReward", "true");
					this.PrivateOverheadMessage( MessageType.Regular, 1153, false, "Thank you, this is exactly what I needed!", pm.NetState );
                    pm.SendMessage("You have been awarded a christmas gift for your quest completion!");
					return true;
                }
         		
				else
         		{
         			pm.SendMessage("You are so kind to have taken the time to help me obtain a Gingerbread Dough.");
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
