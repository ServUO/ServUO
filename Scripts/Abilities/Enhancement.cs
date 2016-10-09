using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class EnhancementAttributes
    {
        public string Title { get; set; }

        public AosAttributes Attributes { get; private set; }
        public AosWeaponAttributes WeaponAttributes { get; private set; }
        public AosArmorAttributes ArmorAttributes { get; private set; }
        public SAAbsorptionAttributes AbsorptionAttributes { get; private set; }

        public EnhancementAttributes(string title)
        {
            this.Title = title;
            this.Attributes = new AosAttributes(null);
            this.WeaponAttributes = new AosWeaponAttributes(null);
            this.ArmorAttributes = new AosArmorAttributes(null);
            this.AbsorptionAttributes = new SAAbsorptionAttributes(null);
        }
    }

    public class Enhancement
    {
        public static Dictionary<Mobile, List<EnhancementAttributes>> EnhancementList = new Dictionary<Mobile, List<EnhancementAttributes>>();

        public static bool AddMobile(Mobile m)
        {
            if (!EnhancementList.ContainsKey(m))
            {
                EnhancementList.Add(m, new List<EnhancementAttributes>());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the mobile and/or attributes from the dictionary
        /// </summary>
        /// <param name="m"></param>
        /// <param name="title">null or default value will remove the entire entry. Add the title arg to remove only that element from the list.</param>
        /// <returns></returns>
        public static bool RemoveMobile(Mobile m, string title = null)
        {
            if (EnhancementList.ContainsKey(m))
            {
                if (title != null)
                {
                    EnhancementAttributes match = EnhancementList[m].FirstOrDefault(attrs => attrs.Title == title);

                    if (match != null && EnhancementList[m].Contains(match))
                    {
                        if(match.Attributes.BonusStr > 0)
                            m.RemoveStatMod("MagicalEnhancementStr");

                        if (match.Attributes.BonusDex > 0)
                            m.RemoveStatMod("MagicalEnhancementDex");

                        if (match.Attributes.BonusInt > 0)
                            m.RemoveStatMod("MagicalEnhancementInt");

                        EnhancementList[m].Remove(match);
                    }
                }

                if(EnhancementList[m].Count == 0 || title == null)
                    EnhancementList.Remove(m);

                m.CheckStatTimers();
                m.UpdateResistances();
                m.Delta(MobileDelta.Stat | MobileDelta.WeaponDamage | MobileDelta.Hits | MobileDelta.Stam | MobileDelta.Mana);

                m.Items.ForEach(i => i.InvalidateProperties());

                return true;
            }

            return false;
        }

        public static int GetValue(Mobile m, AosAttribute att)
        {
            if (EnhancementList.ContainsKey(m))
            {
                int value = 0;
                EnhancementList[m].ForEach(attrs => value += attrs.Attributes[att]);
                return value;
            }

            return 0;
        }

        public static void SetValue(Mobile m, AosAttribute att, int value, string title)
        {
            if (!EnhancementList.ContainsKey(m))
                AddMobile(m);

            if (att == AosAttribute.BonusStr)
            {
                m.RemoveStatMod("MagicalEnhancementStr");
                m.AddStatMod(new StatMod(StatType.Str, "MagicalEnhancementStr", value, TimeSpan.Zero));
            }
            else if (att == AosAttribute.BonusDex)
            {
                m.RemoveStatMod("MagicalEnhancementDex");
                m.AddStatMod(new StatMod(StatType.Dex, "MagicalEnhancementDex", value, TimeSpan.Zero));
            }
            else if (att == AosAttribute.BonusInt)
            {
                m.RemoveStatMod("MagicalEnhancementInt");
                m.AddStatMod(new StatMod(StatType.Int, "MagicalEnhancementInt", value, TimeSpan.Zero));
            }

            EnhancementAttributes match = EnhancementList[m].FirstOrDefault(attrs => attrs.Title == title);

            if (match != null)
            {
                match.Attributes[att] = value;
            }
            else
            {
                match = new EnhancementAttributes(title);
                match.Attributes[att] = value;

                EnhancementList[m].Add(match);
            }

            m.CheckStatTimers();
            m.UpdateResistances();
            m.Delta(MobileDelta.Stat | MobileDelta.WeaponDamage | MobileDelta.Hits | MobileDelta.Stam | MobileDelta.Mana);
        }

        public static int GetValue(Mobile m, AosWeaponAttribute att)
        {
            if (EnhancementList.ContainsKey(m))
            {
                int value = 0;
                EnhancementList[m].ForEach(attrs => value += attrs.WeaponAttributes[att]);
                return value;
            }

            return 0;
        }

        public static void SetValue(Mobile m, AosWeaponAttribute att, int value, string title)
        {
            if (!EnhancementList.ContainsKey(m))
                AddMobile(m);

            EnhancementAttributes match = EnhancementList[m].FirstOrDefault(attrs => attrs.Title == title);

            if (match != null)
            {
                match.WeaponAttributes[att] = value;
            }
            else
            {
                match = new EnhancementAttributes(title);
                match.WeaponAttributes[att] = value;

                EnhancementList[m].Add(match);
            }

            m.CheckStatTimers();
            m.UpdateResistances();
            m.Delta(MobileDelta.Stat | MobileDelta.WeaponDamage | MobileDelta.Hits | MobileDelta.Stam | MobileDelta.Mana);
        }

        public static int GetValue(Mobile m, AosArmorAttribute att)
        {
            if (EnhancementList.ContainsKey(m))
            {
                int value = 0;
                EnhancementList[m].ForEach(attrs => value += attrs.ArmorAttributes[att]);
                return value;
            }

            return 0;
        }

        public static void SetValue(Mobile m, AosArmorAttribute att, int value, string title)
        {
            if (!EnhancementList.ContainsKey(m))
                AddMobile(m);

            EnhancementAttributes match = EnhancementList[m].FirstOrDefault(attrs => attrs.Title == title);

            if (match != null)
            {
                match.ArmorAttributes[att] = value;
            }
            else
            {
                match = new EnhancementAttributes(title);
                match.ArmorAttributes[att] = value;

                EnhancementList[m].Add(match);
            }

            m.CheckStatTimers();
            m.UpdateResistances();
            m.Delta(MobileDelta.Stat | MobileDelta.WeaponDamage | MobileDelta.Hits | MobileDelta.Stam | MobileDelta.Mana);
        }

        public static int GetValue(Mobile m, SAAbsorptionAttribute att)
        {
            if (EnhancementList.ContainsKey(m))
            {
                int value = 0;
                EnhancementList[m].ForEach(attrs => value += attrs.AbsorptionAttributes[att]);
                return value;
            }

            return 0;
        }

        public static void SetValue(Mobile m, SAAbsorptionAttribute att, int value, string title)
        {
            if (!EnhancementList.ContainsKey(m))
                AddMobile(m);

            EnhancementAttributes match = EnhancementList[m].FirstOrDefault(attrs => attrs.Title == title);

            if (match != null)
            {
                match.AbsorptionAttributes[att] = value;
            }
            else
            {
                match = new EnhancementAttributes(title);
                match.AbsorptionAttributes[att] = value;

                EnhancementList[m].Add(match);
            }

            m.CheckStatTimers();
            m.UpdateResistances();
            m.Delta(MobileDelta.Stat | MobileDelta.WeaponDamage | MobileDelta.Hits | MobileDelta.Stam | MobileDelta.Mana);
        }
    }
}
/*
AOS.cs
MagicalEnhancements.GetValue( m, attribute );
Usage of setting total (intended use)
MagicalEnhancements.SetValue( m, AosAttribute.Luck, 50 );
Example of a timed stackable Enhancement (supports to an extent)
private Mobile m_Mobile;

public void Luckboon()
{
MagicalEnhancements.AddMobile( m );
MagicalEnhancements.EnhancementList[m].Attributes.Luck += 200;
Timer.DelayCall( TimeSpan.FromSeconds( 30 ), new TimerCallback( Expire ) );
}

private void Expire()
{
if ( m_Mobile != null && MagicalEnhancements.EnhancementList.ContainsKey( m ) )
MagicalEnhancements.EnhancementList[m].Attributes.Luck -= 200;
}
*/