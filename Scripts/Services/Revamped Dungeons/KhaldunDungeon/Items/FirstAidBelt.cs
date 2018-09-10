using System;
using System.Linq;

namespace Server.Items
{
    public class FirstAidBelt : Container
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1158681; } } // First Aid Belt

        public override int DefaultGumpID { get { return 0x3C; } }
        public override int DefaultMaxItems { get { return 1; } }
        public override int DefaultMaxWeight { get { return 100; } }
        public override double DefaultWeight { get { return 2.0; } }

        private int m_WeightReduction;

        [CommandProperty(AccessLevel.GameMaster)]
        public int WeightReduction
        {
            get { return m_WeightReduction; }
            set
            {
                m_WeightReduction = value;
                InvalidateProperties();
            }
        }

        public Item Ammo { get { return Items.Count > 0 ? Items[0] : null; } }
        public int MaxAmmo { get { return DefaultMaxWeight * 10; } }

        [Constructable]
        public FirstAidBelt()
            : this(0xA1F6)
        {
        }

        public FirstAidBelt(int itemID)
            : base(itemID)
        {
            Weight = 2.0;
            LootType = LootType.Blessed;
            Layer = Layer.Waist;
            WeightReduction = 50;
        }

        public FirstAidBelt(Serial serial)
            : base(serial)
        {
        }

        public override bool DisplaysContent { get { return false; } }

        public override void UpdateTotal(Item sender, TotalType type, int delta)
        {
            InvalidateProperties();

            base.UpdateTotal(sender, type, delta);
        }

        public override int GetTotal(TotalType type)
        {
            int total = base.GetTotal(type);

            if (type == TotalType.Weight)
                total -= total * m_WeightReduction / 100;

            return total;
        }

        private static readonly Type[] m_Ammo = new Type[]
        {
            typeof(Bandage)
        };

        public bool CheckType(Item item)
        {
            Type type = item.GetType();
            Item ammo = Ammo;

            if (ammo != null)
            {
                if (ammo.GetType() == type)
                    return true;
            }
            else
            {
                for (int i = 0; i < m_Ammo.Length; i++)
                {
                    if (type == m_Ammo[i])
                        return true;
                }
            }

            return false;
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (!Movable)
                return false;

            if (!CheckType(item))
            {
                if (message)
                    m.SendLocalizedMessage(1074836); // The container can not hold that type of object.

                return false;
            }

            if (!checkItems || Items.Count < DefaultMaxItems)
            {
                int currentAmount = 0;

                Items.ForEach(i => currentAmount += i.Amount);

                if (item.Amount + currentAmount <= MaxAmmo)
                    return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
                else
                    m.SendLocalizedMessage(1080017); // That container cannot hold more items.
            }

            return false;
        }

        public override bool CheckStack(Mobile from, Item item)
        {
            if (!CheckType(item))
            {
                return false;
            }

            Item ammo = Ammo;

            if (ammo != null)
            {
                int currentAmount = Items.Sum(i => i.Amount);

                if (item.Amount + currentAmount <= MaxAmmo)
                    return base.CheckStack(from, item);
            }

            return false;
        }

        public override void AddItem(Item dropped)
        {
            base.AddItem(dropped);

            InvalidateWeight();
        }

        public override void RemoveItem(Item dropped)
        {
            base.RemoveItem(dropped);

            InvalidateWeight();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            Item ammo = Ammo;

            int prop;

            double weight = 0;

            if (ammo != null)
                weight = ammo.Weight * ammo.Amount;

            list.Add(1072241, "{0}\t{1}\t{2}\t{3}", Items.Count, DefaultMaxItems, (int)weight, DefaultMaxWeight); // Contents: ~1_COUNT~/~2_MAXCOUNT items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones

            if ((prop = m_WeightReduction) != 0)
                list.Add(1072210, prop.ToString()); // Weight reduction: ~1_PERCENTAGE~%
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write((int)m_WeightReduction);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_WeightReduction = reader.ReadInt();

                        break;
                    }
            }
        }

        public void InvalidateWeight()
        {
            if (RootParent is Mobile)
            {
                Mobile m = (Mobile)RootParent;

                m.UpdateTotals();
            }
        }
    }
}
