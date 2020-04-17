using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
    public class StorageEntry
    {
        private int m_Funds;
        private DateTime m_Expires;
        private readonly Dictionary<Type, int> m_CommodityTypes = new Dictionary<Type, int>();
        private readonly List<BaseCreature> m_Creatures = new List<BaseCreature>();

        public int Funds { get { return m_Funds; } set { m_Funds = value; } }
        public DateTime Expires => m_Expires;
        public Dictionary<Type, int> CommodityTypes => m_CommodityTypes;
        public List<BaseCreature> Creatures => m_Creatures;

        public StorageEntry(Mobile m, BaseBazaarBroker broker)
        {
            AddInventory(m, broker);
        }

        public void AddInventory(Mobile m, BaseBazaarBroker broker)
        {
            m_Funds += broker.BankBalance;
            m_Expires = DateTime.UtcNow + TimeSpan.FromDays(7);

            if (broker is CommodityBroker)
            {
                foreach (CommodityBrokerEntry entry in ((CommodityBroker)broker).CommodityEntries)
                {
                    if (entry.Stock > 0)
                    {
                        m_CommodityTypes[entry.CommodityType] = entry.Stock;
                    }
                }
            }
            else if (broker is PetBroker)
            {
                foreach (PetBrokerEntry entry in ((PetBroker)broker).BrokerEntries)
                {
                    if (entry.Pet.Map != Map.Internal || !entry.Pet.IsStabled)
                    {
                        entry.Internalize();
                    }

                    m_Creatures.Add(entry.Pet);
                }
            }
        }

        public void RemoveCommodity(Type type, int amount)
        {
            if (m_CommodityTypes.ContainsKey(type))
            {
                m_CommodityTypes[type] -= amount;

                if (m_CommodityTypes[type] <= 0)
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

            switch (version)
            {
                case 1:
                    m_Funds = reader.ReadInt();
                    m_Expires = reader.ReadDateTime();

                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Type cType = ScriptCompiler.FindTypeByName(reader.ReadString());
                        int amount = reader.ReadInt();

                        if (cType != null)
                            m_CommodityTypes[cType] = amount;
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        BaseCreature bc = reader.ReadMobile() as BaseCreature;

                        if (bc != null)
                            m_Creatures.Add(bc);
                    }
                    break;
                case 0:
                    int type = reader.ReadInt();
                    m_Funds = reader.ReadInt();
                    m_Expires = reader.ReadDateTime();

                    switch (type)
                    {
                        case 0: break;
                        case 1:
                            {
                                int c1 = reader.ReadInt();
                                for (int i = 0; i < c1; i++)
                                {
                                    Type cType = ScriptCompiler.FindTypeByName(reader.ReadString());
                                    int amount = reader.ReadInt();

                                    if (cType != null)
                                        m_CommodityTypes[cType] = amount;
                                }
                                break;
                            }
                        case 2:
                            {
                                int c2 = reader.ReadInt();
                                for (int i = 0; i < c2; i++)
                                {
                                    BaseCreature bc = reader.ReadMobile() as BaseCreature;

                                    if (bc != null)
                                    {
                                        m_Creatures.Add(bc);
                                    }
                                }
                                break;
                            }
                    }
                    break;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(1);

            writer.Write(m_Funds);
            writer.Write(m_Expires);

            writer.Write(m_CommodityTypes.Count);
            foreach (KeyValuePair<Type, int> kvp in m_CommodityTypes)
            {
                writer.Write(kvp.Key.Name);
                writer.Write(kvp.Value);
            }

            writer.Write(m_Creatures.Count);
            foreach (BaseCreature bc in m_Creatures)
            {
                writer.Write(bc);
            }
        }
    }
}