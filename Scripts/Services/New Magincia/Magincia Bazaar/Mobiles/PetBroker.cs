using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
	public class PetBroker : BaseBazaarBroker
	{
		private List<PetBrokerEntry> m_BrokerEntries = new List<PetBrokerEntry>();
		public List<PetBrokerEntry> BrokerEntries { get { return m_BrokerEntries; } }
		
		public static readonly int MaxEntries = 10;
		
		public PetBroker(MaginciaBazaarPlot plot) : base(plot)
		{
            FollowersMax = 500;
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			if(from.InRange(this.Location, 4) && Plot != null)
			{
				if(Plot.Owner == from)
				{
					from.CloseGump(typeof(PetBrokerGump));
					from.SendGump(new PetBrokerGump(this, from));
				}
				else
				{
					from.CloseGump(typeof(PetInventoryGump));
					from.SendGump(new PetInventoryGump(this, from));
				}
			}
			else
				base.OnDoubleClick(from);
		}
		
		public bool TryAddEntry(BaseCreature bc, Mobile from, int cost)
		{
			if(bc == null || HasEntry(bc) || !bc.Alive || !bc.IsStabled)
				from.SendLocalizedMessage(1150342); // That pet is not in the stables. The pet must remain in the stables in order to be transferred to the broker's inventory.
			else if(m_BrokerEntries.Count >= MaxEntries)
				from.SendLocalizedMessage(1150631); // You cannot add more pets to this animal broker's inventory at this time, because the shop inventory is full.
			else if (!from.Stabled.Contains(bc))
				from.SendLocalizedMessage(1150344); // Transferring the pet from the stables to the animal broker's inventory failed for an unknown reason.
			else
			{
				m_BrokerEntries.Add(new PetBrokerEntry(bc, cost));
                return true;
			}

            return false;
		}
		
		public void RemoveEntry(PetBrokerEntry entry)
		{
			if(m_BrokerEntries.Contains(entry))
				m_BrokerEntries.Remove(entry);
		}

        public bool HasValidEntry()
        {
            foreach (PetBrokerEntry entry in m_BrokerEntries)
            {
                if (entry.Pet != null)
                    return true;
            }

            return BankBalance > 0;
        }
		
		public bool HasEntry(BaseCreature bc)
		{
			foreach(PetBrokerEntry entry in m_BrokerEntries)
			{
				if(entry.Pet == bc)
					return true;
			}
			
			return false;
		}

        public void CheckInventory()
        {
            List<PetBrokerEntry> entries = new List<PetBrokerEntry>(m_BrokerEntries);

            foreach(PetBrokerEntry entry in entries)
            {
                if (entry.Pet == null || entry.Pet.Deleted)
                    m_BrokerEntries.Remove(entry);
            }
        }
		
		public override int GetWeeklyFee()
		{
			int total = 0;
			
			foreach(PetBrokerEntry entry in m_BrokerEntries)
			{
				if(entry.SalePrice > 0)
					total += entry.SalePrice;
			}
			
			double perc = (double)total * .05;
			return (int)perc;
		}
		
		public bool TryBuyPet(Mobile from, PetBrokerEntry entry)
		{
			if(from == null || entry == null || entry.Pet == null)
				return false;
				
			int cost = entry.SalePrice;
			int toDeduct = cost + (int)((double)cost * ((double)ComissionFee / 100.0));
			BaseCreature pet = entry.Pet;
			
			if(!m_BrokerEntries.Contains(entry) || entry.Pet == null || entry.Pet.Deleted)
				from.SendLocalizedMessage(1150377); // Unable to complete the desired transaction at this time.
			else if(pet.GetControlChance(from) <= 0.0)
				from.SendLocalizedMessage(1150379); // Unable to transfer that pet to you because you have no chance at all of controlling it.
            else if (from.Stabled.Count >= AnimalTrainer.GetMaxStabled(from)/*from.Followers + pet.ControlSlots >= from.FollowersMax*/)
				from.SendLocalizedMessage(1150376); // You do not have any available stable slots. The Animal Broker can only transfer pets to your stables. Please make a stables slot available and try again.
			else if (!Banker.Withdraw(from, toDeduct))
				from.SendLocalizedMessage(1150252); // You do not have the funds needed to make this trade available in your bank box. Brokers are only able to transfer funds from your bank box. Please deposit the necessary funds into your bank box and try again.
			else
			{
                BankBalance += cost;
				pet.Blessed = false;
                EndViewTimer(pet);
                pet.ControlTarget = null;
                pet.ControlOrder = OrderType.Stay;
                pet.Internalize();
                pet.SetControlMaster(null);
                pet.SummonMaster = null;
                pet.IsStabled = true;
                pet.Loyalty = BaseCreature.MaxLoyalty;
                from.Stabled.Add(pet);

                from.SendLocalizedMessage(1150380, String.Format("{0}\t{1}", entry.TypeName, pet.Name)); // You have purchased ~1_TYPE~ named "~2_NAME~". The animal is now in the stables and you may retrieve it there.
                m_BrokerEntries.Remove(entry);
                return true;
            }

            return false;
		}
		
		private static Dictionary<BaseCreature, Timer> m_ViewTimer = new Dictionary<BaseCreature, Timer>();
		
		public static void AddToViewTimer(BaseCreature bc)
		{
			if(m_ViewTimer.ContainsKey(bc))
			{
				if(m_ViewTimer[bc] != null)
					m_ViewTimer[bc].Stop();
			}
			
			m_ViewTimer[bc] = new InternalTimer(bc);
			m_ViewTimer[bc].Start();
		}
		
		public static void EndViewTimer(BaseCreature bc)
		{
			if(m_ViewTimer.ContainsKey(bc))
			{
				if(m_ViewTimer[bc] != null)
					m_ViewTimer[bc].Stop();
					
				m_ViewTimer.Remove(bc);
			}
		}
		
		private class InternalTimer : Timer
		{
			BaseCreature m_Creature;
			
			public InternalTimer(BaseCreature bc) : base(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2))
			{
				m_Creature = bc;
				Priority = TimerPriority.OneMinute;
			}
			
			protected override void OnTick()
			{
                if (m_Creature is BaseMount)
                    ((BaseMount)m_Creature).Rider = null;

				m_Creature.ControlTarget = null;
				m_Creature.ControlOrder = OrderType.Stay;
				m_Creature.Internalize();

				m_Creature.SetControlMaster( null );
				m_Creature.SummonMaster = null;

				m_Creature.IsStabled = true;
				m_Creature.Loyalty = BaseCreature.MaxLoyalty;
				
				m_Creature.Home = Point3D.Zero;
				m_Creature.RangeHome = 10;
				m_Creature.Blessed = false;
				
				EndViewTimer(m_Creature);
			}
		}

		public PetBroker(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			
			writer.Write(m_BrokerEntries.Count);
			foreach(PetBrokerEntry entry in m_BrokerEntries)
				entry.Serialize(writer);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			int count = reader.ReadInt();
			for(int i = 0; i < count; i++)
				m_BrokerEntries.Add(new PetBrokerEntry(reader));
		}
	}
}