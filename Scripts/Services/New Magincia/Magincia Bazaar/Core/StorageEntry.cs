using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
    public enum StorageType
    {
        None,
        Commodity, 
        Pet
    }

	public class StorageEntry 
	{
        private StorageType m_StorageType;
        private int m_Funds;
        private DateTime m_Expires;
        private Dictionary<Type, int> m_CommodityTypes = new Dictionary<Type, int>();
        private List<BaseCreature> m_Creatures = new List<BaseCreature>();

        public StorageType StorageType { get { return m_StorageType; } }
        public int Funds { get { return m_Funds; } set { m_Funds = value; } }
        public DateTime Expires { get { return m_Expires; } }
        public Dictionary<Type, int> CommodityTypes { get { return m_CommodityTypes; } }
        public List<BaseCreature> Creatures { get { return m_Creatures; } }

        public StorageEntry(List<CommodityBrokerEntry> list, int funds)
        {
            m_Funds = funds;
            m_StorageType = StorageType.Commodity;

            foreach (CommodityBrokerEntry entry in list)
            {
                if(entry.Stock > 0)
                    m_CommodityTypes[entry.CommodityType] = entry.Stock;
            }

            m_Expires = DateTime.UtcNow + TimeSpan.FromDays(7);
        }

        public StorageEntry(List<PetBrokerEntry> list, int funds)
        {
            m_Funds = funds;
            m_StorageType = StorageType.Pet;
            m_Expires = DateTime.UtcNow + TimeSpan.FromDays(7);

            foreach (PetBrokerEntry entry in list)
                m_Creatures.Add(entry.Pet);
        }

        public void RemoveCommodity(Type type, int amount)
        {
            if (m_CommodityTypes.ContainsKey(type))
            {
                m_CommodityTypes[type] -= amount;

                if(m_CommodityTypes[type] <= 0)
                    m_CommodityTypes.Remove(type);
            }
        }

        public void RemovePet(BaseCreature pet)
        {
            if (m_Creatures.Contains(pet))
                m_Creatures.Remove(pet);
        }

        public StorageEntry(GenericReader reader)
		{
			int version = reader.ReadInt();
			
            m_StorageType = (StorageType)reader.ReadInt();
            m_Funds = reader.ReadInt();
            m_Expires = reader.ReadDateTime();

            switch(m_StorageType)
            {
                case StorageType.None: break;
                case StorageType.Commodity:
                    {
                        int count = reader.ReadInt();
                        for(int i = 0; i < count; i++)
                        {
                            Type cType = ScriptCompiler.FindTypeByName(reader.ReadString());
                            int amount = reader.ReadInt();

                            if (cType != null)
                                m_CommodityTypes[cType] = amount;
                        }
                        break;
                    }
                case StorageType.Pet:
                    {
                        int c = reader.ReadInt();
                        for (int i = 0; i < c; i++)
                        {
                            BaseCreature bc = reader.ReadMobile() as BaseCreature;

                            if (bc != null)
                                m_Creatures.Add(bc);
                        }
                        break;
                    }
            }
		}
		
		public void Serialize(GenericWriter writer)
		{
			writer.Write((int)0);

            writer.Write((int)m_StorageType);
            writer.Write(m_Funds);
            writer.Write(m_Expires);

            switch(m_StorageType)
            {
                case StorageType.None: break;
                case StorageType.Commodity:
                    {
                        writer.Write(m_CommodityTypes.Count);
                        foreach (KeyValuePair<Type, int> kvp in m_CommodityTypes)
                        {
                            writer.Write(kvp.Key.Name);
                            writer.Write(kvp.Value);
                        }
                        break;
                    }
                case StorageType.Pet:
                    {
                        writer.Write(m_Creatures.Count);
                        foreach(BaseCreature bc in m_Creatures)
                            writer.Write(bc);
                        break;
                    }
            }
		}
	}
}