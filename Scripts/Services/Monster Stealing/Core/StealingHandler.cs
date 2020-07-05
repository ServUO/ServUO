using Server.Engines.CannedEvil;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using System;

namespace Server.Engines.CreatureStealing
{
    class StealingHandler
    {
        private static readonly Type[] SpecialItemList =
        {
            typeof(SeedOfLife),
            typeof(BalmOfStrength),
            typeof(BalmOfWisdom),
            typeof(BalmOfSwiftness),
            typeof(ManaDraught),
            typeof(BalmOfProtection),
            typeof(StoneSkinLotion),
            typeof(GemOfSalvation),
            typeof(LifeShieldLotion),
            typeof(SmugglersLantern),
            typeof(SmugglersToolBox)
        };

        public static void HandleSteal(BaseCreature bc, PlayerMobile thief, ref Item stolen)
        {
            if (!CheckLocation(thief, bc))
            {
                return;
            }

            double stealing = thief.Skills.Stealing.Value;

            if (stealing >= 100)
            {
                int chance = GetStealingChance(thief, bc, stealing);

                if ((Utility.Random(100) + 1) <= chance)
                {
                    thief.SendLocalizedMessage(1094947);//You successfully steal a special item from the creature!

                    Item item;

                    if (bc is ExodusZealot)
                    {
                        item = Activator.CreateInstance(ExodusChest.RituelItem[Utility.Random(ExodusChest.RituelItem.Length)]) as Item;
                    }
                    else
                    {
                        item = Activator.CreateInstance(SpecialItemList[Utility.Random(SpecialItemList.Length - 2)]) as Item;
                    }

                    stolen = item;
                }
            }
        }

        public static void HandleSmugglersEdgeSteal(BaseCreature from, PlayerMobile thief)
        {
            if (from.HasBeenStolen || !CheckLocation(thief, from))
                return;

            if (0.05 > Utility.RandomDouble())
            {
                double tempSkill = Utility.RandomMinMax(80, 110);
                double realSkill = thief.Skills[SkillName.Stealing].Value;

                if (realSkill > tempSkill)
                    tempSkill = realSkill;

                if (tempSkill > 100)
                {
                    int chance = GetStealingChance(thief, from, tempSkill);

                    if (realSkill <= 109.9)
                        chance += 1;
                    else if (realSkill <= 114.9)
                        chance += 2;
                    else if (realSkill >= 115.0)
                        chance += 3;

                    if (chance >= Utility.Random(100))
                    {
                        Item item = Activator.CreateInstance(SpecialItemList[Utility.Random(SpecialItemList.Length)]) as Item;

                        if (item != null)
                        {
                            thief.AddToBackpack(item);

                            thief.SendLocalizedMessage(1094947);//You successfully steal a special item from the creature!
                        }
                    }
                    else
                    {
                        Container pack = from.Backpack;

                        if (pack != null && pack.Items.Count > 0)
                        {
                            int randomIndex = Utility.Random(pack.Items.Count);

                            Item stolen = TryStealItem(pack.Items[randomIndex], tempSkill);

                            if (stolen != null)
                            {
                                thief.AddToBackpack(stolen);

                                thief.SendLocalizedMessage(502724); // You succesfully steal the item.
                            }
                            else
                            {
                                thief.SendLocalizedMessage(502723); // You fail to steal the item.
                            }
                        }
                    }

                    from.HasBeenStolen = true;
                }
            }
        }

        private static bool CheckLocation(Mobile thief, Mobile from)
        {
            if (!((thief.Map == Map.Felucca && thief.Region is DungeonRegion) || thief.Region is ChampionSpawnRegion || from is ExodusZealot))
            {
                return false;
            }

            return true;
        }

        private static int GetStealingChance(Mobile thief, BaseCreature from, double stealing)
        {
            int fame = from.Fame;

            fame = Math.Max(1, fame);
            fame = Math.Min(30000, fame);

            int chance = 0;

            if (stealing == 120)
                chance += 10;
            else if (stealing >= 110.1)
                chance += 8;
            else if (stealing >= 100.1)
                chance += 5;
            else if (stealing == 100)
                chance += 2;

            int level = (int)(40.0 / 29999.0 * fame - 40.0 / 29999.0);

            if (level >= 40)
                chance += 5;
            else if (level >= 35)
                chance += 3;
            else if (level >= 30)
                chance += 2;
            else if (level >= 25)
                chance += 1;

            return chance;
        }

        private static Item TryStealItem(Item toSteal, double skill)
        {
            Item stolen = null;
            double w = toSteal.Weight + toSteal.TotalWeight;

            if (w <= 10)
            {
                if (toSteal.Stackable && toSteal.Amount > 1)
                {
                    int maxAmount = (int)((skill / 10.0) / toSteal.Weight);

                    if (maxAmount < 1)
                    {
                        maxAmount = 1;
                    }
                    else if (maxAmount > toSteal.Amount)
                    {
                        maxAmount = toSteal.Amount;
                    }

                    int amount = Utility.RandomMinMax(1, maxAmount);

                    if (amount >= toSteal.Amount)
                    {
                        int pileWeight = (int)Math.Ceiling(toSteal.Weight * toSteal.Amount);
                        pileWeight *= 10;

                        double chance = (skill - (pileWeight - 22.5)) / ((pileWeight + 27.5) - (pileWeight - 22.5));

                        if (chance >= Utility.RandomDouble())
                        {
                            stolen = toSteal;
                        }
                    }
                    else
                    {
                        int pileWeight = (int)Math.Ceiling(toSteal.Weight * amount);
                        pileWeight *= 10;

                        double chance = (skill - (pileWeight - 22.5)) / ((pileWeight + 27.5) - (pileWeight - 22.5));

                        if (chance >= Utility.RandomDouble())
                        {
                            stolen = Mobile.LiftItemDupe(toSteal, toSteal.Amount - amount);

                            if (stolen == null)
                            {
                                stolen = toSteal;
                            }
                        }
                    }
                }
                else
                {
                    int iw = (int)Math.Ceiling(w);
                    iw *= 10;

                    double chance = (skill - (iw - 22.5)) / ((iw + 27.5) - (iw - 22.5));

                    if (chance >= Utility.RandomDouble())
                    {
                        stolen = toSteal;
                    }
                }

                if (stolen != null)
                {
                    ItemFlags.SetTaken(stolen, true);
                    ItemFlags.SetStealable(stolen, false);
                    stolen.Movable = true;
                }
            }

            return stolen;
        }
    }
}
