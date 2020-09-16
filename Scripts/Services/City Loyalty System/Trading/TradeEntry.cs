using System;
using System.Collections.Generic;
using System.Linq;

using Server.Items;

namespace Server.Engines.CityLoyalty
{
    [PropertyObject]
    public class TradeEntry
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public City Origin { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public City Destination { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Kills { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Distance { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastAmbush { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string TurnIn
        {
            get
            {
                string str = "";
                Details.ForEach(d =>
                    {
                        str += d.ItemType.Name + ", ";
                    });
                return str;
            }
        }

        public List<TradeDetails> Details { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public TradeDetails Detail { get { return Details != null && Details.Count > 0 ? Details[0] : null; } set { } }

        public TradeEntry(City destination, City origin, int distance)
        {
            Origin = origin;
            Destination = destination;
            Distance = distance;

            Details = new List<TradeDetails>();
        }

        public override string ToString()
        {
            return "...";
        }

        public int CalculateGold()
        {
            if (Distance == 0)
                return 0;

            int gold = 0;

            Details.ForEach(d =>
                {
                    gold += d.Worth * 3;
                });

            return Math.Max(CityTradeSystem.TurnInGold, gold + (Distance * 10) + Math.Min(25000, (Kills * 500)));
        }

        [PropertyObject]
        public class TradeDetails
        {
            [CommandProperty(AccessLevel.GameMaster)]
            public Type ItemType { get; set; }

            [CommandProperty(AccessLevel.GameMaster)]
            public int Amount { get; set; }

            [CommandProperty(AccessLevel.GameMaster)]
            public int Worth { get; set; }

            [CommandProperty(AccessLevel.GameMaster)]
            public string Name { get; set; }

            public TradeDetails(Type type, int worth, int amount, string fallbackname)
            {
                ItemType = type;
                Worth = worth;
                Amount = amount;

                Name = CityLoyaltySystem.CityTrading.GetNameFor(type, fallbackname);
            }

            public TradeDetails(GenericReader reader)
            {
                int version = reader.ReadInt();

                ItemType = ScriptCompiler.FindTypeByName(reader.ReadString());
                Worth = reader.ReadInt();
                Amount = reader.ReadInt();
                Name = reader.ReadString();
            }

            public void Serialize(GenericWriter writer)
            {
                writer.Write(0);

                writer.Write(ItemType.Name);
                writer.Write(Worth);
                writer.Write(Amount);
                writer.Write(Name);
            }

            public override string ToString()
            {
                return "...";
            }

            public bool Match(Type type)
            {
                if (type == ItemType)
                {
                    return true;
                }

                var list = Interchangeables.FirstOrDefault(l => l.Any(t => t == type));

                if (list != null)
                {
                    return list.Any(t => t == ItemType);
                }

                return false;
            }

            public int Count(TradeOrderCrate crate)
            {
                return crate.Items.Where(i => Match(i.GetType())).Sum(item => item.Amount);
            }

            public static Type[][] Interchangeables => _Interchangeables;
            private static Type[][] _Interchangeables = new Type[][]
            {
                new Type[] { typeof(PewterBowlOfPeas), typeof(WoodenBowlOfPeas) },
                new Type[] { typeof(PewterBowlOfCarrots), typeof(WoodenBowlOfCarrots) },
                new Type[] { typeof(PewterBowlOfCorn), typeof(WoodenBowlOfCorn) },
                new Type[] { typeof(PewterBowlOfLettuce), typeof(WoodenBowlOfLettuce) },
            };
        }

        public TradeEntry(GenericReader reader)
        {
            int version = reader.ReadInt();

            Origin = (City)reader.ReadInt();
            Destination = (City)reader.ReadInt();

            LastAmbush = reader.ReadDateTime();

            Kills = reader.ReadInt();
            Distance = reader.ReadInt();

            Details = new List<TradeDetails>();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Details.Add(new TradeDetails(reader));
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write((int)Origin);
            writer.Write((int)Destination);

            writer.Write(LastAmbush);

            writer.Write(Kills);
            writer.Write(Distance);

            writer.Write(Details.Count);
            Details.ForEach(d => d.Serialize(writer));
        }
    }
}
