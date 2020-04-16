using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Engines.CityLoyalty
{
    public class CityDonationItem : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltySystem CitySystem { get { return CityLoyaltySystem.GetCityInstance(City); } set { } }

        public City City { get; protected set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public TradeMinister Minister { get; set; }

        public Dictionary<Type, int> Table { get; protected set; }

        public virtual bool Animals => false;

        public CityDonationItem(City city, TradeMinister minister, int itemid) : base(itemid)
        {
            City = city;
            Minister = minister;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!CityLoyaltySystem.IsSetup())
                return;

            if (Animals && Table != null && Table.Count > 0)
            {
                from.Target = new InternalTarget(this);
                from.SendLocalizedMessage(1152936); // Which animal do you wish to donate?
            }
            else if (!Animals)
            {
                SendMessageTo(from, 1152928); // If you wish to donate to the City simply drop the items into the crate.
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!CityLoyaltySystem.IsSetup())
                return false;

            if (!Animals && Table != null && Table.Count > 0)
            {
                int love = 0;

                if (dropped is Container)
                {
                    List<Item> items = new List<Item>(dropped.Items);

                    foreach (Item item in items)
                    {
                        love += CheckTurnin(item);
                    }

                    items.Clear();
                    items.TrimExcess();
                }
                else
                {
                    love = CheckTurnin(dropped);
                }

                if (love > 0)
                {
                    CityLoyaltySystem.GetCityInstance(City).AwardLove(from, love);
                    SendMessageTo(from, 1152926); // The City thanks you for your generosity!
                    return !(dropped is Container);
                }
                else
                {
                    SendMessageTo(from, 1152927); // Your generosity is appreciated but the City does not require this item.
                    return false;
                }
            }

            return false;
        }

        private int CheckTurnin(Item item)
        {
            Type dropType = item.GetType();
            int love = 0;

            foreach (KeyValuePair<Type, int> kvp in Table)
            {
                Type checkType = kvp.Key;

                if (checkType == dropType || dropType.IsSubclassOf(checkType))
                {
                    CityLoyaltySystem sys = CityLoyaltySystem.GetCityInstance(City);

                    if (sys != null)
                    {
                        // TODO: If anything adds to treasure, change this
                        if (item is MaritimeCargo)
                        {
                            AddToTreasury(sys, (MaritimeCargo)item);
                        }

                        item.Delete();
                        love = Table[checkType] * item.Amount;

                        break;
                    }
                }
            }

            return love;
        }

        private void AddToTreasury(CityLoyaltySystem system, MaritimeCargo cargo)
        {
            if (system != null)
            {
                if (cargo.City == City)
                {
                    system.AddToTreasury(null, cargo.GetAwardAmount() * 1000);
                }
                else
                {
                    system.AddToTreasury(null, cargo.GetAwardAmount() * 500);
                }
            }
        }

        private class InternalTarget : Target
        {
            public CityDonationItem Item { get; private set; }

            public InternalTarget(CityDonationItem item) : base(3, false, TargetFlags.None)
            {
                Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCreature)
                {
                    BaseCreature bc = targeted as BaseCreature;
                    Type t = bc.GetType();

                    if (bc.Controlled && !bc.Summoned && bc.GetMaster() == from)
                    {
                        if (Item.Table.ContainsKey(t))
                        {
                            CityLoyaltySystem sys = CityLoyaltySystem.GetCityInstance(Item.City);

                            if (sys != null)
                            {
                                bc.Delete();
                                sys.AwardLove(from, Item.Table[t]);

                                Item.SendMessageTo(from, 1152926); // The City thanks you for your generosity!
                            }
                        }
                        else
                            Item.SendMessageTo(from, 1152929); // That does not look like an animal the City is in need of.
                    }
                    else
                        Item.SendMessageTo(from, 1152930); // Erm. Uhh. I don't think that'd enjoy the stables much...
                }
                else
                    Item.SendMessageTo(from, 1152930); // Erm. Uhh. I don't think that'd enjoy the stables much...
            }
        }

        private void SendMessageTo(Mobile m, int message)
        {
            if (Minister != null)
                Minister.SayTo(m, message);
            else
                m.SendLocalizedMessage(message);
        }

        public CityDonationItem(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)City);
            writer.Write(Minister);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            City = (City)reader.ReadInt();
            Minister = reader.ReadMobile() as TradeMinister;
        }
    }

    public class CityItemDonation : CityDonationItem
    {
        [Constructable]
        public CityItemDonation(City city, TradeMinister minister) : base(city, minister, 0xE3C)
        {
            Table = ItemTable;

            if (CitySystem != null && CitySystem.Minister != null)
                CitySystem.Minister.DonationCrate = this;
        }

        public static Dictionary<Type, int> ItemTable { get; set; }

        public static void Configure()
        {
            ItemTable = new Dictionary<Type, int>();

            ItemTable.Add(typeof(BaseWoodBoard), 10);
            ItemTable.Add(typeof(BaseIngot), 10);
            ItemTable.Add(typeof(BaseHides), 10);
            ItemTable.Add(typeof(BaseLeather), 10);
            ItemTable.Add(typeof(RawRibs), 10);
            ItemTable.Add(typeof(BreadLoaf), 10);
            ItemTable.Add(typeof(RawFishSteak), 10);
            ItemTable.Add(typeof(BaseCrabAndLobster), 15);
            ItemTable.Add(typeof(BasePotion), 15);
            ItemTable.Add(typeof(Bow), 20);
            ItemTable.Add(typeof(Crossbow), 20);
            ItemTable.Add(typeof(MaritimeCargo), 50);
        }

        public CityItemDonation(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            Table = ItemTable;

            if (CitySystem != null && CitySystem.Minister != null)
                CitySystem.Minister.DonationCrate = this;
        }
    }

    public class CityPetDonation : CityDonationItem
    {
        public override bool Animals => true;

        [Constructable]
        public CityPetDonation(City city, TradeMinister minister) : base(city, minister, 0x14E7)
        {
            Table = PetTable;

            if (CitySystem != null && CitySystem.Minister != null)
                CitySystem.Minister.DonationPost = this;
        }

        public static Dictionary<Type, int> PetTable { get; set; }

        public static void Configure()
        {
            PetTable = new Dictionary<Type, int>();

            PetTable.Add(typeof(Dog), 10);
            PetTable.Add(typeof(Cat), 10);
            PetTable.Add(typeof(Cow), 10);
            PetTable.Add(typeof(Goat), 10);
            PetTable.Add(typeof(Horse), 10);
            PetTable.Add(typeof(Sheep), 10);
            PetTable.Add(typeof(Pig), 10);
            PetTable.Add(typeof(Chicken), 10);
        }

        public CityPetDonation(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            Table = PetTable;

            if (CitySystem != null && CitySystem.Minister != null)
                CitySystem.Minister.DonationPost = this;
        }
    }
}
