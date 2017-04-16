using System;
using Server;
using Server.Mobiles;

namespace Server
{
    //This File Checks ALL MOUNTS To See
    //If You Have A Required Level Setup
    //In The Configuration File, If You Do
    //It Does The Appropriate Checks.
    public class NeedLevel
    {
        public static bool MountCheck(Mobile m, BaseMount mnt)
        {
            Configured c = new Configured();
            PlayerMobile pm = m as PlayerMobile;

            if (c.ToMountLevel > 0)
            {
                if (!(pm.charLevel >= c.ToMountLevel))
                {
                    mnt.OnDisallowedRider(pm);
                    pm.SendMessage("You are not level {0}!", c.ToMountLevel);
                    return false;
                }
                return true;
            }
            else
            {
                if (mnt is Beetle)
                {
                    if (c.Beetle > 0 && !(pm.charLevel >= c.Beetle))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.Beetle);
                        return false;
                    }
                    return true;
                }
                else if (mnt is DesertOstard)
                {
                    if (c.DesertOstard > 0 && !(pm.charLevel >= c.DesertOstard))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.DesertOstard);
                        return false;
                    }
                    return true;
                }
                else if (mnt is FireSteed)
                {
                    if (c.FireSteed > 0 && !(pm.charLevel >= c.FireSteed))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.FireSteed);
                        return false;
                    }
                    return true;
                }
                else if (mnt is ForestOstard)
                {
                    if (c.ForestOstard > 0 && !(pm.charLevel >= c.ForestOstard))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.ForestOstard);
                        return false;
                    }
                    return true;
                }
                else if (mnt is FrenziedOstard)
                {
                    if (c.FrenziedOstard > 0 && !(pm.charLevel >= c.FrenziedOstard))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.FrenziedOstard);
                        return false;
                    }
                    return true;
                }
                else if (mnt is HellSteed)
                {
                    if (c.HellSteed > 0 && !(pm.charLevel >= c.HellSteed))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.HellSteed);
                        return false;
                    }
                    return true;
                }
                else if (mnt is Hiryu)
                {
                    if (c.Hiryu > 0 && !(pm.charLevel >= c.Hiryu))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.Hiryu);
                        return false;
                    }
                    return true;
                }
                else if (mnt is Horse)
                {
                    if (c.Horse > 0 && !(pm.charLevel >= c.Horse))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.Horse);
                        return false;
                    }
                    return true;
                }
                else if (mnt is Kirin)
                {
                    if (c.Kirin > 0 && !(pm.charLevel >= c.Kirin))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.Kirin);
                        return false;
                    }
                    return true;
                }
                else if (mnt is LesserHiryu)
                {
                    if (c.LesserHiryu > 0 && !(pm.charLevel >= c.LesserHiryu))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.LesserHiryu);
                        return false;
                    }
                    return true;
                }
                else if (mnt is Nightmare)
                {
                    if (c.NightMare > 0 && !(pm.charLevel >= c.NightMare))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.NightMare);
                        return false;
                    }
                    return true;
                }
                else if (mnt is RidableLlama)
                {
                    if (c.RidableLlama > 0 && !(pm.charLevel >= c.RidableLlama))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.RidableLlama);
                        return false;
                    }
                    return true;
                }
                else if (mnt is Ridgeback)
                {
                    if (c.Ridgeback > 0 && !(pm.charLevel >= c.Ridgeback))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.Ridgeback);
                        return false;
                    }
                    return true;
                }
                else if (mnt is SavageRidgeback)
                {
                    if (c.SavageRidgeback > 0 && !(pm.charLevel >= c.SavageRidgeback))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.SavageRidgeback);
                        return false;
                    }
                    return true;
                }
                else if (mnt is ScaledSwampDragon)
                {
                    if (c.ScaledSwampDragon > 0 && !(pm.charLevel >= c.ScaledSwampDragon))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.ScaledSwampDragon);
                        return false;
                    }
                    return true;
                }
                else if (mnt is SeaHorse)
                {
                    if (c.SeaHorse > 0 && !(pm.charLevel >= c.SeaHorse))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.SeaHorse);
                        return false;
                    }
                    return true;
                }
                else if (mnt is SilverSteed)
                {
                    if (c.SilverSteed > 0 && !(pm.charLevel >= c.SilverSteed))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.SilverSteed);
                        return false;
                    }
                    return true;
                }
                else if (mnt is SkeletalMount)
                {
                    if (c.SkeletalMount > 0 && !(pm.charLevel >= c.SkeletalMount))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.SkeletalMount);
                        return false;
                    }
                    return true;
                }
                else if (mnt is SwampDragon)
                {
                    if (c.SwampDragon > 0 && !(pm.charLevel >= c.SwampDragon))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.SwampDragon);
                        return false;
                    }
                    return true;
                }
                else if (mnt is Unicorn)
                {
                    if (c.Unicorn > 0 && !(pm.charLevel >= c.Unicorn))
                    {
                        mnt.OnDisallowedRider(pm);
                        pm.SendMessage("You are not level {0}!", c.Unicorn);
                        return false;
                    }
                    return true;
                }
                return true;
            }
        }
    }
}