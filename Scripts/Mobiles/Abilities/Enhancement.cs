// Created by Peoharen
using System;
using System.Collections.Generic;

namespace Server
{
    public class EnhancementAttributes
    {
        public string Title;
        public AosAttributes Attributes;
        public AosWeaponAttributes WeaponAttributes;
        public AosArmorAttributes ArmorAttributes;
        public SAAbsorptionAttributes AbsorptionAttributes;
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
        public static Dictionary<Mobile, EnhancementAttributes> EnhancementList = new Dictionary<Mobile, EnhancementAttributes>();
        public static bool AddMobile(Mobile m, string title)
        {
            if (!EnhancementList.ContainsKey(m))
            {
                EnhancementList.Add(m, new EnhancementAttributes(title));
                return true;
            }

            return false;
        }

        public static bool RemoveMobile(Mobile m)
        {
            if (EnhancementList.ContainsKey(m))
            {
                EnhancementList.Remove(m);
                return true;
            }

            return false;
        }

        public static int GetValue(Mobile m, AosAttribute att)
        {
            if (EnhancementList.ContainsKey(m))
                return EnhancementList[m].Attributes[att];
            else
                return 0;
        }

        public static void SetValue(Mobile m, AosAttribute att, int value, string title)
        {
            if (!EnhancementList.ContainsKey(m))
                AddMobile(m, title);

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

            if (title != EnhancementList[m].Title)
                EnhancementList[m].Attributes[att] = value;
            else
                EnhancementList[m].Attributes[att] += value;
        }

        public static int GetValue(Mobile m, AosWeaponAttribute att)
        {
            if (EnhancementList.ContainsKey(m))
                return EnhancementList[m].WeaponAttributes[att];
            else
                return 0;
        }

        public static void SetValue(Mobile m, AosWeaponAttribute att, int value, string title)
        {
            if (!EnhancementList.ContainsKey(m))
                AddMobile(m, title);

            if (title != EnhancementList[m].Title)
                EnhancementList[m].WeaponAttributes[att] = value;
            else
                EnhancementList[m].WeaponAttributes[att] += value;
        }

        public static int GetValue(Mobile m, AosArmorAttribute att)
        {
            if (EnhancementList.ContainsKey(m))
                return EnhancementList[m].ArmorAttributes[att];
            else
                return 0;
        }

        public static void SetValue(Mobile m, AosArmorAttribute att, int value, string title)
        {
            if (!EnhancementList.ContainsKey(m))
                AddMobile(m, title);

            if (title != EnhancementList[m].Title)
                EnhancementList[m].ArmorAttributes[att] = value;
            else
                EnhancementList[m].ArmorAttributes[att] += value;
        }

        public static int GetValue(Mobile m, SAAbsorptionAttribute att)
        {
            if (EnhancementList.ContainsKey(m))
                return EnhancementList[m].AbsorptionAttributes[att];
            else
                return 0;
        }

        public static void SetValue(Mobile m, SAAbsorptionAttribute att, int value, string title)
        {
            if (!EnhancementList.ContainsKey(m))
                AddMobile(m, title);

            if (title != EnhancementList[m].Title)
                EnhancementList[m].AbsorptionAttributes[att] = value;
            else
                EnhancementList[m].AbsorptionAttributes[att] += value;
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