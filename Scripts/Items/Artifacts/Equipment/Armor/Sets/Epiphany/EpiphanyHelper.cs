using Server.Misc;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public interface IEpiphanyArmor
    {
        Alignment Alignment { get; }
        SurgeType Type { get; }
        int Frequency { get; }
        int Bonus { get; }
    }

    public static class EpiphanyHelper
    {
        public static Dictionary<Mobile, Dictionary<SurgeType, int>> Table { get; set; }

        public static readonly int MinTriggerDamage = 15; // TODO: Amount?

        public static int GetFrequency(Mobile m, IEpiphanyArmor armor)
        {
            if (m == null)
                return 1;

            return Math.Max(1, Math.Min(5, m.Items.Where(i => i is IEpiphanyArmor &&
                                      ((IEpiphanyArmor)i).Alignment == armor.Alignment &&
                                      ((IEpiphanyArmor)i).Type == armor.Type).Count()));
        }

        public static int GetBonus(Mobile m, IEpiphanyArmor armor)
        {
            if (m == null)
                return 0;

            switch (armor.Alignment)
            {
                default: return 0;
                case Alignment.Good:
                    if (m.Karma <= 0)
                        return 0;

                    return Math.Min(20, m.Karma / (Titles.MaxKarma / 20));
                case Alignment.Evil:
                    if (m.Karma >= 0)
                        return 0;

                    return Math.Min(20, -m.Karma / (Titles.MaxKarma / 20));
            }
        }

        public static void OnHit(Mobile m, int damage)
        {
            if (damage > MinTriggerDamage)
            {
                CheckHit(m, damage, SurgeType.Hits);
                CheckHit(m, damage, SurgeType.Stam);
                CheckHit(m, damage, SurgeType.Mana);
            }
        }

        public static void CheckHit(Mobile m, int damage, SurgeType type)
        {
            IEpiphanyArmor item = m.Items.OfType<IEpiphanyArmor>().FirstOrDefault(i => i.Type == type);

            if (item == null)
                return;

            if (Table == null)
            {
                Table = new Dictionary<Mobile, Dictionary<SurgeType, int>>();
            }

            if (!Table.ContainsKey(m))
            {
                Table[m] = new Dictionary<SurgeType, int>();
            }

            if (!Table[m].ContainsKey(type))
            {
                Table[m][type] = damage;
            }
            else
            {
                damage += Table[m][type];
            }

            int freq = GetFrequency(m, item);
            int bonus = GetBonus(m, item);

            if (freq > 0 && bonus > 0 && damage > Utility.Random(10000 / freq))
            {
                Table[m].Remove(type);

                if (Table[m].Count == 0)
                    Table.Remove(m);

                switch (type)
                {
                    case SurgeType.Hits: m.Hits = Math.Min(m.HitsMax, m.Hits + bonus); break;
                    case SurgeType.Stam: m.Hits = Math.Min(m.HitsMax, m.Hits + bonus); break;
                    default:
                    case SurgeType.Mana: m.Hits = Math.Min(m.HitsMax, m.Hits + bonus); break;
                }
            }
            else
            {
                Table[m][type] = damage;
            }
        }

        public static void OnKarmaChange(Mobile m)
        {
            foreach (Item item in m.Items.Where(i => i is IEpiphanyArmor))
            {
                item.InvalidateProperties();
            }
        }

        public static void AddProperties(IEpiphanyArmor item, ObjectPropertyList list)
        {
            if (item == null)
                return;

            switch (item.Type)
            {
                case SurgeType.Hits:
                    list.Add(1150830 + (int)item.Alignment + 1); // Set Ability: good healing burst
                    break;
                case SurgeType.Stam: // NOTE: This doesn't exist on EA, but put it in here anyways!
                    list.Add(1149953, string.Format("{0}\t{1}", "Set Ability", item.Alignment == Alignment.Evil ? "evil stamina burst" : "good stamina burst"));
                    break;
                default:
                case SurgeType.Mana:
                    list.Add(1150240 + (int)item.Alignment); // Set Ability: evil mana burst
                    break;
            }

            if (item is Item)
            {
                list.Add(1150240, GetFrequency(((Item)item).Parent as Mobile, item).ToString()); // Set Bonus: Frequency ~1_val~
                list.Add(1150243, GetBonus(((Item)item).Parent as Mobile, item).ToString()); // Karma Bonus: Burst level ~1_val~
            }
        }
    }
}