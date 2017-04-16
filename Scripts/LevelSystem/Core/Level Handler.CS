using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.Craft;
using Server.Engines.PartySystem;

namespace Server
{
    //This File Sets All Main Properties
    //It Also Deals With Party Exp And
    //Player Leveling.
    public class LevelHandler
    {
        public ArrayList MemberCount = new ArrayList();

        public static void Set(Mobile killer, Mobile killed)
        {
            PlayerMobile klr = null;
            Party pty = null;
            Configured c = new Configured();
            LevelHandler lh = new LevelHandler();

            if (killer is BaseCreature)
            {
                BaseCreature bc = killer as BaseCreature;

                if (bc.Controlled && c.PetKillGivesExp)
                    klr = bc.GetMaster() as PlayerMobile;
            }
            else
            {
                if (killer is PlayerMobile) //double check ;)
                    klr = killer as PlayerMobile;
            }

            if (lh.MemberCount.Count > 0)
            {
                foreach (Mobile il in lh.MemberCount)
                {
                    lh.MemberCount.Remove(il);
                }
            }

            if (klr != null)
            {
                pty = Party.Get(klr);

                if (klr.charLevel < 1)
                    klr.charLevel = 1;

                if (klr.charToLevel < 50)
                    klr.charToLevel = 50;

                if (!(klr.charMaxLevel == c.StartMaxLvl && klr.charMaxLevel > c.EndMaxLvl))
                    klr.charMaxLevel = c.StartMaxLvl;

                AddExp(klr, killed, pty, new Configured());
            }
        }

        public static void AddExp(Mobile m, Mobile k, Party p, Configured c)
        {
            PlayerMobile pm = null;
            LevelHandler lh = new LevelHandler();

            int range = c.PartyRange;

            double orig = 0;  //Monster Xp
            double fig = 0;   //Party Xp
            double give = 0;  //Xp To Give

            if (k != null)
                orig = LevelCore.Base(k);

            if (p != null && c.PartySplitExp)
            {
                foreach (PartyMemberInfo mi in p.Members)
                {
                    pm = mi.Mobile as PlayerMobile;

                    if (pm.InRange(k, range) && lh.MemberCount.Count < 6)
                        lh.MemberCount.Add(pm);
                }

                if (lh.MemberCount.Count > 1)
                    fig = (orig / lh.MemberCount.Count);
            }
            else
            {
                pm = m as PlayerMobile;
                fig = orig;
            }

            if (fig > 0)
                give = LevelHandler.ExpFilter(pm, fig, p, false);

            if (give > 0)
            {
                if (p != null && c.PartyExpShare) //lots o party checks with this....
                {
                    foreach (PartyMemberInfo mi in p.Members)
                    {
                        if (pm.Alive && pm.InRange(k, range))
                        {
                            pm.SendMessage("You gained " + give + " exp for the kill!");
                            pm.charExp += (int)give;

                            if (pm.charExp >= pm.charToLevel && pm.charLevel < pm.charMaxLevel)
                                DoLevel(pm, new Configured());
                        }
                    }
                }
                else
                {
                    pm.SendMessage("You gained " + give + " exp for the kill!");
                    pm.charkxp += (int)give;

                    if (pm.charExp >= pm.charToLevel && pm.charLevel < pm.charMaxLevel)
                        DoLevel(pm, new Configured());
                }
            }
        }

        public static int ExpFilter(Mobile m, double o, Party p, bool craft)
        {
            PlayerMobile pm = null;
            Configured c = new Configured();

            double n;
            double New = 0;

            if (p != null && c.PartyExpShare)
            {
                #region Partied

                if (craft)
                    return 0;

                foreach (PartyMemberInfo mi in p.Members)
                {
                    pm = mi.Mobile as PlayerMobile;

                    if (pm.charExp + o > pm.charToLevel && pm.charLevel >= pm.charMaxLevel)
                    {
                        n = (o + pm.charExp) - pm.charToLevel;
                        New = (o - n);
                    }
                    else
                        New = o;
                }

                #endregion
            }
            else
            {
                pm = m as PlayerMobile;

                if (pm.charExp + o > pm.charToLevel && pm.charLevel >= pm.charMaxLevel)
                {
                    n = (o + pm.charExp) - pm.charToLevel;
                    New = (int)(o - n);
                }
                else
                    New = o;
            }

            return (int)New;
        }

        public static void DoLevel(Mobile klr, Configured c)
        {
            PlayerMobile pm = klr as PlayerMobile;
            LevelHandler lh = new LevelHandler();

            if (pm.charExp >= pm.charToLevel)
            {
                pm.charExp = 0;
                pm.charkxp = 0;
                pm.charLevel += 1;

                if (pm.charLevel <= 20)
                    pm.charToLevel = (int)(pm.charLevel * 100);
                else if (pm.charLevel <= 40)
                    pm.charToLevel = (int)(pm.charLevel * 200);
                else if (pm.charLevel <= 60)
                    pm.charToLevel = (int)(pm.charLevel * 400);
                else if (pm.charLevel <= 70)
                    pm.charToLevel = (int)(pm.charLevel * 700);
                else if (pm.charLevel <= 80)
                    pm.charToLevel = (int)(pm.charLevel * 900);
                else if (pm.charLevel <= 90)
                    pm.charToLevel = (int)(pm.charLevel * 1100);
                else if (pm.charLevel <= 100)
                    pm.charToLevel = (int)(pm.charLevel * 1500);


                if (pm.SkillsTotal < 120000)
                { pm.charSKPoints += 10; }

                int totalStats = pm.RawDex + pm.RawInt + pm.RawStr;
                if (totalStats < 300)
                { pm.charStatPoints += 3; }



            }
            
            if (c.RefreshOnLevel)
            {
                if (pm.Hits < pm.HitsMax)
                    pm.Hits = pm.HitsMax;

                if (pm.Mana < pm.ManaMax)
                    pm.Mana = pm.ManaMax;

                if (pm.Stam < pm.StamMax)
                    pm.Stam = pm.StamMax;
            }

            pm.PlaySound(0x20F);
            pm.FixedParticles(0x376A, 1, 31, 9961, 1160, 0, EffectLayer.Waist);
            pm.FixedParticles(0x37C4, 1, 31, 9502, 43, 2, EffectLayer.Waist);
            pm.SendMessage( "Your level has increased" );
            pm.charExp = 0;
            pm.charkxp = 0;
          
            
        }

        public static int Classic(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            int exp = LevelCore.GetExp(pm);//LevelCore.Stats(pm) + LevelCore.Skills(pm);
            int charToLevel = (int)(pm.charLevel * 100);
            int highest = 100;

            if (Cl.Enabled)
            {
                if (exp >= charToLevel && pm.charLevel != highest)
                {
                    pm.charLevel += 1;
                    pm.charToLevel = (int)(pm.charLevel * 100);

                    if (exp >= charToLevel)
                        LevelHandler.Classic(pm);

                    return exp;
                }
                return exp;
            }
            return 0;
        }
    }
}