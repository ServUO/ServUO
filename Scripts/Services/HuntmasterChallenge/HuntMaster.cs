using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;

namespace Server.Engines.HuntsmasterChallenge
{
	public class HuntMaster : BaseVendor
	{
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }
		public override bool IsActiveVendor { get { return false; } }

        public override void InitSBInfo()
        {
        }
		
        [Constructable]
		public HuntMaster() : base ( "the huntmaster" )
		{
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			if(from.InRange(this.Location, 4) && HuntingSystem.Instance != null && HuntingSystem.Instance.Active)
			{
				if(!HuntingSystem.Instance.CheckUnclaimedEntry(from))
				{
					from.CloseGump(typeof(BasicInfoGump));
					from.SendGump(new BasicInfoGump(1155750, 1155726));
					
					/*Greetings! Only the most brave Hunters dare take my challenge! To participate, 
					simply purchase a hunting permit from me for 5,000gp.  When you are ready to to 
					hunt seek any species of prey represented by the trophies in this hall.  When you 
					have bested your quarry use the deed on the corpse to document your kill.  Return 
					to me and hand me the permit and I will process it.  If your kill beats or ties the 
					current best record for that species, you will be eligible for rewards when the 
					contest finishes on the first day of the next month!    Return and speak to me on 
					the first day of the next month to claim your rewards should your record hold up 
					during the month long contest! You may also use a taxidermy kit, purchasable from a 
					tanner, to create a trophy of your kill like the ones you see here. Happy Hunting!*/
				}
			}
		}
		
		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
            if(HuntingSystem.Instance != null && HuntingSystem.Instance.Active)
			    list.Add(new BuyPermitEntry(this));
		
			base.AddCustomContextEntries(from, list);
		}
		
		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if(dropped is HuntingPermit)
			{
				HuntingPermit permit = dropped as HuntingPermit;
				HuntingSystem sys = HuntingSystem.Instance;
				
				if(sys == null || !sys.Active)
					return false;
				
				if(!permit.HasDocumentedKill)
					SayTo(from, "You can't submit a kill you haven't documented yet!");
				else if (permit.KillEntry.DateKilled < sys.SeasonBegins)
					SayTo(from, 1155720); // This permit was documented in a different month or year than the current month and year. I only accept permits documented in the current month and year.
				else if (permit.HasSubmitted)
					SayTo(from, 1155719); // This permit has already been submitted.
				else
					sys.TrySubmitKill(this, from, permit);
			}
			
			return false;
		}
		
		private class BuyPermitEntry : ContextMenuEntry
		{
			private HuntMaster m_HuntMaster;
			
			public BuyPermitEntry(HuntMaster master) : base(1155701, 3) // Get Hunting Permit
			{
				m_HuntMaster = master;
			}
			
			public override void OnClick()
			{
				Mobile from = this.Owner.From;
				
				if(HuntingPermit.HasPermit(from))
					from.SendLocalizedMessage(1155702); // You already have a hunting permit.
				else if(Banker.Withdraw(from, 5000))
				{
					Banker.Withdraw(from, 5000);
					HuntingPermit permit = new HuntingPermit(from);
					
					if(from.Backpack == null || !from.Backpack.TryDropItem(from, permit, false))
					{
						from.SendLocalizedMessage(1155703); // Your backpack was too full so the permit was deleted. Empty your backpack and try again.
						permit.Delete();
					}
					//TODO: Message???
				}
                else
                    from.SendLocalizedMessage(500382); // Thou dost not have sufficient funds in thy account to withdraw that much.
			}
		}
		
		public HuntMaster(Serial serial) : base(serial)
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
			int v = reader.ReadInt();
		}
	}
}	