using System;
using Server;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Craft;

namespace Server
{
    //This File Gets And Sets Exp
    //It Also Sets The Level For Creatures
    public class LevelCore
    {
        public static int GetExp(Mobile klr)
        {
            return 0; //Stats(klr) + (Skills(klr) / 10);
        }

        public static int Stats(Mobile m)
        {
            if (Cl.Enabled || En.Enabled)
                return 0;// ((int)Math.Round(11.111 * m.RawStatTotal));
            else
                return 0;
        }

        public static int Skills(Mobile m)
        {
            Configured c = new Configured();

            if ((Cl.Enabled) || (En.Enabled))
            {
                if (c.AdvancedSkillExp)
                {
                    if (Core.AOS)
                    { return (int)9.375 * AoS(m); }
                    else if (Core.SE || Core.ML)
                    { return (int)7.5 * SE(m); }
                    else
                    { return (int)6.25 * Pre(m); }
                }
                else
                    return ((int)1.388 * m.Skills.Total);
            }
            return 0;
        }

        public static int Pre(Mobile m)
        {
            return (int)(m.Skills[SkillName.Archery].Base + m.Skills[SkillName.Fencing].Base
                + m.Skills[SkillName.Macing].Base + m.Skills[SkillName.Magery].Base +
                m.Skills[SkillName.Parry].Base + m.Skills[SkillName.Swords].Base +
                m.Skills[SkillName.Tactics].Base + m.Skills[SkillName.Wrestling].Base);
        }

        public static int AoS(Mobile m)
        {
            return (int)(Pre(m) + m.Skills[SkillName.Chivalry].Base
                + m.Skills[SkillName.Necromancy].Base);
        }

        public static int SE(Mobile m)
        {
            return (int)(AoS(m) + m.Skills[SkillName.Ninjitsu].Base
                + m.Skills[SkillName.Bushido].Base);
        }

        public static int Base(Mobile kld)
        {
            Configured c = new Configured();

            double amnt;

            if (kld is BaseCreature)
            {
                BaseCreature bc = kld as BaseCreature;

                if (c.AdvancedExp)
                {
                    amnt = bc.HitsMax / 4;

                    #region Get Table Exp

                    foreach (Type creat in exp.NoExp)
                    {
                        if (creat == bc.GetType())
                            return 0;
                    }

                    foreach (Type creat in exp.ExtLowExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(5, 10));
                    }

                    foreach (Type creat in exp.VeryLowExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(10, 15));
                    }

                    foreach (Type creat in exp.LowExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(15, 25));
                    }

                    foreach (Type creat in exp.LowMedExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(25, 50));
                    }

                    foreach (Type creat in exp.MedExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(50, 75));
                    }

                    foreach (Type creat in exp.HighMedExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(75, 100));
                    }

                    foreach (Type creat in exp.HighExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(100, 150));
                    }

                    foreach (Type creat in exp.VeryHighExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(150, 200));
                    }

                    foreach (Type creat in exp.ExtHighExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(175, 225));
                    }

                    foreach (Type creat in exp.ExtHigh)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(225, 300));
                    }

                    #endregion
                }
                else
                {
                    amnt = bc.HitsMax + bc.RawStatTotal;

                    if (IsMageryCreature(bc))
                        amnt += 10;

                    if (IsFireBreathingCreature(bc))
                        amnt += 15;

                    if (kld is VampireBat || kld is VampireBatFamiliar)
                        amnt += 5;

                    amnt += GetPoisonLevel(bc) * 10;
                }
            }
            else
            {
                PlayerMobile pm = kld as PlayerMobile;

                amnt = ((pm.Str * pm.Hits) + (pm.Dex * pm.Stam)
                    + (pm.Int * pm.Mana)) / 5;

                if (pm.Skills.Total >= 100)
                    amnt += pm.Skills.Total;
                else
                    amnt += 100;
            }
            
            return (int)amnt / 10;       
        }

        private static bool IsMageryCreature(BaseCreature bc)
        {
            return (bc != null && bc.AI == AIType.AI_Mage && bc.Skills[SkillName.Magery].Base > 5.0);
        }

        private static bool IsParagon(BaseCreature bc)
        {
            if (bc == null)
                return false;

            return bc.IsParagon;
        }

        private static bool IsFireBreathingCreature(BaseCreature bc)
        {
            if (bc == null)
                return false;

            return bc.HasBreath;
        }

        private static bool IsPoisonImmune(BaseCreature bc)
        {
            return (bc != null && bc.PoisonImmune != null);
        }

        private static int GetPoisonLevel(BaseCreature bc)
        {
            if (bc == null)
                return 0;

            Poison p = bc.HitPoison;

            if (p == null)
                return 0;

            return p.Level + 1;
        }

        public static int CreatureLevel(Mobile m, Configured c)
        {
            BaseCreature bc = m as BaseCreature;

            int Lvl = ((bc.HitsMax + bc.RawStatTotal) / 40) + ((bc.DamageMax * bc.DamageMin) / 30);

            if (bc is BaseVendor && !(c.VendorLevels))
                return 0;
            else
            {
                if (Lvl < 1)
                    return 1;
                else if (IsParagon(bc))
                    return Lvl += Utility.RandomMinMax(3, 7);
                else
                    return Lvl;
            }
        }

        public static string Display(Mobile m, Configured c)
        {
            PlayerMobile pm = m as PlayerMobile;

            string dsp;

            if (pm.AccessLevel > AccessLevel.Player && !(c.StaffHasLevel))
            {
                dsp = "";
            }
            else
            {
                if (c.MaxLevel)
                {
                    dsp = "" + pm.charLevel + "/" + pm.charMaxLevel;
                }
                else
                {
                    dsp = "" + pm.charLevel;
                }
            }

            return dsp;
        }

        public static int TExp(Mobile m)
        {
            PlayerMobile pm = m as PlayerMobile;

            if (Cl.Enabled)
                return LevelHandler.Classic(pm);
            else if (En.Enabled)
                return pm.charkxp + LevelCore.GetExp(pm);
            else
                return pm.charkxp;
        }

        public static void Taming(Mobile m, Configured c)
        {
            PlayerMobile pm = m as PlayerMobile;
            LevelHandler lh = new LevelHandler();

            
            if (pm.charExp < pm.charToLevel)
            {

                 pm.SendMessage("You have gained 5 Experience for taming a wild creature");
                 pm.charkxp += 5;

                 if (pm.charkxp >= pm.charToLevel && pm.charLevel < pm.charMaxLevel)
                     LevelHandler.DoLevel(pm, new Configured());



            }
        }

        public static void Quest(Mobile m, Configured c)
        {
            PlayerMobile pm = m as PlayerMobile;
            LevelHandler lh = new LevelHandler();


            if (pm.charExp < pm.charToLevel)
            {

                pm.SendMessage("You have gained 50 Experience for Completing this quest");
                pm.charkxp += 51;

                if (pm.charkxp >= pm.charToLevel && pm.charLevel < pm.charMaxLevel)
                    LevelHandler.DoLevel(pm, new Configured());



            }
        }





        public static void Harvest(Item i, Mobile m, Configured c)
        {
            PlayerMobile pm = m as PlayerMobile;
            LevelHandler lh = new LevelHandler();

            if (pm.charExp < pm.charToLevel)
            {
                int xp = (int)Math.Round(2 + (LevelCore.HarvestExp(i, pm)));
                int give = LevelHandler.ExpFilter(pm, xp, null, true);

                if (give > 0)
                {
                    pm.SendMessage("You gained {0} experience for harvesting some Resources.", give);
                    pm.charkxp += (int)give;
               
                if (pm.charkxp >= pm.charToLevel && pm.charLevel < pm.charMaxLevel)
                    LevelHandler.DoLevel(pm, new Configured());
                }
            }
        }

        public static double HarvestExp(Item i, Mobile m)
        {
            double rtn = 0;
            PlayerMobile pm = m as PlayerMobile;

            if (i is BaseOre)
            {
                BaseOre bo = i as BaseOre;

                if (bo.Resource == CraftResource.Iron)
                    rtn += 1;
                else if (bo.Resource == CraftResource.DullCopper)
                    rtn += 2;
                else if (bo.Resource == CraftResource.ShadowIron)
                    rtn += 3;
                else if (bo.Resource == CraftResource.Copper)
                    rtn += 4;
                else if (bo.Resource == CraftResource.Bronze)
                    rtn += 5;
                else if (bo.Resource == CraftResource.Gold)
                    rtn += 6;
                else if (bo.Resource == CraftResource.Agapite)
                    rtn += 7;
                else if (bo.Resource == CraftResource.Verite)
                    rtn += 8;
                else if (bo.Resource == CraftResource.Valorite)
                    rtn += 9;
            }
            if (i is Fish)
            {
                rtn += 0.2;
            }

            if (i is Log)
            {
                Log lo = i as Log;
                if (lo.Resource == CraftResource.RegularWood)
                     rtn += 1;
                else if (lo.Resource == CraftResource.OakWood)
                    rtn += 2;
                else if (lo.Resource == CraftResource.AshWood)
                    rtn += 3;
                else if (lo.Resource == CraftResource.YewWood)
                    rtn += 4;
                else if (lo.Resource == CraftResource.Heartwood)
                    rtn += 5;
                else if (lo.Resource == CraftResource.Bloodwood)
                    rtn += 6;
                else if (lo.Resource == CraftResource.Frostwood)
                    rtn += 7;

            }

            return (rtn);

        }




        public static void Craft(Item i, int q, double ch, double e, Mobile m, Configured c)
        {
            PlayerMobile pm = m as PlayerMobile;
            LevelHandler lh = new LevelHandler();

            if (pm.charExp < pm.charToLevel)
            {
                int xp = (int)Math.Round(3.6 + (LevelCore.CraftExp(i, q, ch, e, pm)));
                int give = LevelHandler.ExpFilter(pm, xp, null, true);

                if (give > 0)
                {
                    pm.SendMessage("You gained {0} experience for crafting an item.", give);
                    pm.charkxp += (int)give;


                    if (pm.charkxp >= pm.charToLevel && pm.charLevel < pm.charMaxLevel)
                        LevelHandler.DoLevel(pm, new Configured());
                }
            }
        }

        public static double CraftExp(Item i, int q, double c, double e, Mobile m)
        {
            PlayerMobile pm = m as PlayerMobile;

            double rtn = 0;

            if (i is BaseWeapon)
            {
                #region Weapon Resource Settings

                BaseWeapon bw = i as BaseWeapon;

                if (bw.Resource == CraftResource.Iron)
                    rtn += 0.2;
                else if (bw.Resource == CraftResource.DullCopper)
                    rtn += 0.4;
                else if (bw.Resource == CraftResource.ShadowIron)
                    rtn += 0.8;
                else if (bw.Resource == CraftResource.Copper)
                    rtn += 1.6;
                else if (bw.Resource == CraftResource.Bronze)
                    rtn += 1.8;
                else if (bw.Resource == CraftResource.Gold)
                    rtn += 2.2;
                else if (bw.Resource == CraftResource.Agapite)
                    rtn += 3.0;
                else if (bw.Resource == CraftResource.Verite)
                    rtn += 4.6;
                else if (bw.Resource == CraftResource.Valorite)
                    rtn += 4.8;
                else if (bw.Resource == CraftResource.RegularWood)
                    rtn += 0.2;
                else if (bw.Resource == CraftResource.OakWood)
                    rtn += 0.4;
                else if (bw.Resource == CraftResource.AshWood)
                    rtn += 0.8;
                else if (bw.Resource == CraftResource.YewWood)
                    rtn += 1.4;
                else if (bw.Resource == CraftResource.Heartwood)
                    rtn += 2.0;
                else if (bw.Resource == CraftResource.Bloodwood)
                    rtn += 3.8;
                else if (bw.Resource == CraftResource.Frostwood)
                    rtn += 4.8;


                #endregion
            }
            else if (i is BaseArmor)
            {
                #region Armour Resource Settings

                BaseArmor ba = i as BaseArmor;

                #region Armour Type (Ring/Chain/Plate)

                foreach (Type t in armours.Ringmail)
                {
                    if (t == ba.GetType())
                        rtn += 0.6;
                }

                foreach (Type t in armours.Chainmail)
                {
                    if (t == ba.GetType())
                        rtn += 1.3;
                }

                foreach (Type t in armours.Platemail)
                {
                    if (t == ba.GetType())
                        rtn += 2.1;
                }

                #endregion

                #region Metals

                if (ba.Resource == CraftResource.Iron)
                    rtn += 0.2;
                else if (ba.Resource == CraftResource.DullCopper)
                    rtn += 0.4;
                else if (ba.Resource == CraftResource.ShadowIron)
                    rtn += 0.8;
                else if (ba.Resource == CraftResource.Copper)
                    rtn += 1.6;
                else if (ba.Resource == CraftResource.Bronze)
                    rtn += 1.8;
                else if (ba.Resource == CraftResource.Gold)
                    rtn += 2.2;
                else if (ba.Resource == CraftResource.Agapite)
                    rtn += 3.0;
                else if (ba.Resource == CraftResource.Verite)
                    rtn += 4.6;
                else if (ba.Resource == CraftResource.Valorite)
                    rtn += 4.8;

                #endregion

                #region Wood
                else if (ba.Resource == CraftResource.RegularWood)
                    rtn += 0.2;
                else if (ba.Resource == CraftResource.OakWood)
                    rtn += 0.4;
                else if (ba.Resource == CraftResource.AshWood)
                    rtn += 0.8;
                else if (ba.Resource == CraftResource.YewWood)
                    rtn += 1.4;
                else if (ba.Resource == CraftResource.Heartwood)
                    rtn += 2.0;
                else if (ba.Resource == CraftResource.Bloodwood)
                    rtn += 3.8;
                else if (ba.Resource == CraftResource.Frostwood)
                    rtn += 4.8;
                #endregion

                #region Leathers

                else if (ba.Resource == CraftResource.RegularLeather)
                    rtn += 0.8;
                else if (ba.Resource == CraftResource.SpinedLeather)
                    rtn += 1.6;
                else if (ba.Resource == CraftResource.HornedLeather)
                    rtn += 2.6;
                else if (ba.Resource == CraftResource.BarbedLeather)
                    rtn += 3.8;

                #endregion

                #region Scales 
                else if (ba.Resource == CraftResource.RedScales)
                    rtn += 1.0;
                else if (ba.Resource == CraftResource.YellowScales)
                    rtn += 1.0;
                else if (ba.Resource == CraftResource.GreenScales)
                    rtn += 1.0;
                else if (ba.Resource == CraftResource.BlueScales)
                    rtn += 1.0;
                else if (ba.Resource == CraftResource.WhiteScales)
                    rtn += 1.0;
                else if (ba.Resource == CraftResource.BlackScales)
                    rtn += 1.0;


                #endregion

                #endregion
            }
           

            double chance = 100 * c;

            if (chance > 0 && chance < 20)
                rtn += 6.0;
            else if (chance > 20 && chance < 40)
                rtn += 4.4;
            else if (chance > 40 && chance < 60)
                rtn += 3.2;
            else if (chance > 60 && chance < 80)
                rtn += 2.0;
            else
                rtn += 0.3;

            if (q > 1)
                rtn += 3.2;

            return (rtn - e);
        }
    }
}