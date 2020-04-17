using System;
using System.Linq;

namespace Server.Items
{
    public class FirstAidBelt : Container
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1158681;  // First Aid Belt

        public override int DefaultGumpID => 0x3C;
        public override int DefaultMaxItems => 1;
        public override int DefaultMaxWeight => 100;
        public override double DefaultWeight => 2.0;

        private int m_WeightReduction;
        private int m_HealingBonus;
        private AosAttributes m_Attributes;

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

        [CommandProperty(AccessLevel.GameMaster)]
        public int HealingBonus
        {
            get { return m_HealingBonus; }
            set
            {
                m_HealingBonus = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes { get { return m_Attributes; } set { } }

        public Item Bandage => Items.Count > 0 ? Items[0] : null;
        public int MaxBandage => DefaultMaxWeight * 10;

        [Constructable]
        public FirstAidBelt()
            : base(0xA1F6)
        {
            Weight = 2.0;
            Layer = Layer.Waist;
            m_Attributes = new AosAttributes(this);
        }

        public override void OnAfterDuped(Item newItem)
        {
            FirstAidBelt belt = newItem as FirstAidBelt;

            if (belt != null)
            {
                belt.m_Attributes = new AosAttributes(newItem, m_Attributes);
            }

            base.OnAfterDuped(newItem);
        }

        public FirstAidBelt(Serial serial)
            : base(serial)
        {
        }

        public override bool DisplaysContent => false;

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

        private static readonly Type[] m_Bandage = new Type[]
        {
            typeof(Bandage), typeof(EnhancedBandage)
        };

        public bool CheckType(Item item)
        {
            Type type = item.GetType();
            Item bandage = Bandage;

            if (bandage != null)
            {
                if (bandage.GetType() == type)
                    return true;
            }
            else
            {
                for (int i = 0; i < m_Bandage.Length; i++)
                {
                    if (type == m_Bandage[i])
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

                if (item.Amount + currentAmount <= MaxBandage)
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

            Item bandage = Bandage;

            if (bandage != null)
            {
                int currentAmount = Items.Sum(i => i.Amount);

                if (item.Amount + currentAmount <= MaxBandage)
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

            if (m_HealingBonus > 0)
                list.Add(1158679, m_HealingBonus.ToString()); // ~1_VALUE~% Bandage Healing Bonus

            int prop;

            if ((prop = m_Attributes.DefendChance) != 0)
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%

            if ((prop = m_Attributes.BonusDex) != 0)
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~

            if ((prop = m_Attributes.EnhancePotions) != 0)
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%

            if ((prop = m_Attributes.CastRecovery) != 0)
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~

            if ((prop = m_Attributes.CastSpeed) != 0)
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~

            if ((prop = m_Attributes.AttackChance) != 0)
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%

            if ((prop = m_Attributes.BonusHits) != 0)
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~

            if ((prop = m_Attributes.BonusInt) != 0)
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~

            if ((prop = m_Attributes.LowerManaCost) != 0)
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%

            if ((prop = m_Attributes.LowerRegCost) != 0)
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%	

            if ((prop = m_Attributes.Luck) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

            if ((prop = m_Attributes.BonusMana) != 0)
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~

            if ((prop = m_Attributes.RegenMana) != 0)
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~

            if ((prop = m_Attributes.NightSight) != 0)
                list.Add(1060441); // night sight

            if ((prop = m_Attributes.ReflectPhysical) != 0)
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%

            if ((prop = m_Attributes.RegenStam) != 0)
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~

            if ((prop = m_Attributes.RegenHits) != 0)
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~

            if ((prop = m_Attributes.SpellDamage) != 0)
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%

            if ((prop = m_Attributes.BonusStam) != 0)
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~

            if ((prop = m_Attributes.BonusStr) != 0)
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~

            if ((prop = m_Attributes.WeaponSpeed) != 0)
                list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%

            if ((prop = m_Attributes.LowerAmmoCost) > 0)
                list.Add(1075208, prop.ToString()); // Lower Ammo Cost ~1_Percentage~%

            double weight = 0;

            if (Bandage != null)
                weight = Bandage.Weight * Bandage.Amount;

            list.Add(1072241, "{0}\t{1}\t{2}\t{3}", Items.Count, DefaultMaxItems, (int)weight, DefaultMaxWeight); // Contents: ~1_COUNT~/~2_MAXCOUNT items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones

            if ((prop = m_WeightReduction) != 0)
                list.Add(1072210, prop.ToString()); // Weight reduction: ~1_PERCENTAGE~%
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            if (!m_Attributes.IsEmpty)
            {
                writer.Write(1);
                m_Attributes.Serialize(writer);
            }
            else
            {
                writer.Write(0);
            }

            writer.Write(m_HealingBonus);
            writer.Write(m_WeightReduction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        if (reader.ReadInt() == 1)
                        {
                            m_Attributes = new AosAttributes(this, reader);
                        }
                        else
                        {
                            m_Attributes = new AosAttributes(this);
                        }

                        m_HealingBonus = reader.ReadInt();
                        m_WeightReduction = reader.ReadInt();
                        break;
                    }
                case 0:
                    {
                        m_Attributes = new AosAttributes(this);
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
