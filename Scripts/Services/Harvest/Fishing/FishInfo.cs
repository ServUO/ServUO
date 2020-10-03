using Server.Mobiles;
using Server.Regions;
using Server.Spells;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class FishInfo
    {
        public static void Configure()
        {
            //Rare Fish; 0 - 15
            m_FishInfos.Add(new FishInfo(2506, typeof(AutumnDragonfish), 1116090, "Ilshenar", false, RareChance, 105.4));//Confirmed
            m_FishInfos.Add(new FishInfo(2591, typeof(BlueLobster), 1116366, "Ice", false, RareChance, 103.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(1157, typeof(BullFish), 1116095, "Labyrinth", false, RareChance, 105.4)); //Confirmed
            m_FishInfos.Add(new FishInfo(1167, typeof(CrystalFish), 1116092, "Prism of Light", false, RareChance, 105.4)); //Confirmed
            m_FishInfos.Add(new FishInfo(2578, typeof(FairySalmon), 1116089, "TerMur", false, RareChance, 85.8)); //Confirmed
            m_FishInfos.Add(new FishInfo(1461, typeof(FireFish), 1116093, "Shame", false, RareChance, 95.8)); //Confirmed
            m_FishInfos.Add(new FishInfo(1257, typeof(GiantKoi), 1116088, "Tokuno", true, RareChance, 95.8)); //Confirmed
            m_FishInfos.Add(new FishInfo(2579/*1287*/, typeof(GreatBarracuda), 1116100, "Felucca", true, RareChance, 89.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2959, typeof(HolyMackerel), 1116087, "Gravewater Lake", false, RareChance, 102.9)); //Confirmed
            m_FishInfos.Add(new FishInfo(2075, typeof(LavaFish), 1116096, "Abyss", false, RareChance, 110.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2075/*1152*/, typeof(ReaperFish), 1116094, "Doom", false, RareChance, 98.1));  //Confirmed
            m_FishInfos.Add(new FishInfo(2539, typeof(SpiderCrab), 1116367, "Terathan Keep", false, RareChance, 103.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(2558, typeof(StoneCrab), 1116365, "T2A", true, RareChance, 103.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(43, typeof(SummerDragonfish), 1116091, "Destard", false, RareChance, 105.2));  //Confirmed
            m_FishInfos.Add(new FishInfo(1154, typeof(UnicornFish), 1116086, "Twisted Weald", false, RareChance, 110.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2515, typeof(YellowtailBarracuda), 1116098, "Trammel", true, RareChance, 81.9));//Confirmed

            //Legendary Fish ; 16-34
            m_FishInfos.Add(new FishInfo(2406, typeof(AbyssalDragonfish), 1116118, "Destard", false, LegendaryChance, 120.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2500, typeof(BlackMarlin), 1116099, "Felucca", true, LegendaryChance, 110.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(2117, typeof(BloodLobster), 1116370, "Shame", false, LegendaryChance, 115.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(0, typeof(BlueMarlin), 1116097, "Trammel", true, LegendaryChance, 105.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(1777, typeof(DreadLobster), 1116371, "Doom", false, LegendaryChance, 115.1));
            m_FishInfos.Add(new FishInfo(1158, typeof(DungeonPike), 1116107, "Terathan Keep", false, LegendaryChance, 105.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(2576, typeof(GiantSamuraiFish), 1116103, "Tokuno", true, LegendaryChance, 110.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(1281, typeof(GoldenTuna), 1116102, "Tokuno", true, LegendaryChance, 105.0));  //Confirmed
            m_FishInfos.Add(new FishInfo(2604, typeof(Kingfish), 1116085, "TrammelAndFelucca", true, LegendaryChance, 92.2));   //Confirmed
            m_FishInfos.Add(new FishInfo(1463, typeof(LanternFish), 1116106, "Prism of Light", false, LegendaryChance, 105.1));  //Confirmed
            m_FishInfos.Add(new FishInfo(1283, typeof(RainbowFish), 1116108, "Twisted Weald", false, LegendaryChance, 105.1));
            m_FishInfos.Add(new FishInfo(2076, typeof(SeekerFish), 1116109, "Labyrinth", false, LegendaryChance, 105.1));
            m_FishInfos.Add(new FishInfo(0, typeof(SpringDragonfish), 1116104, "Ilshenar", false, LegendaryChance, 120.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(1837, typeof(StoneFish), 1116110, "T2A", true, LegendaryChance, 115.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(2600, typeof(TunnelCrab), 1116372, "Underworld", false, LegendaryChance, 115.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(2069, typeof(VoidCrab), 1116368, "TerMur", false, LegendaryChance, 120.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2071, typeof(VoidLobster), 1116369, "Abyss", false, LegendaryChance, 120.0));
            m_FishInfos.Add(new FishInfo(2499, typeof(WinterDragonfish), 1116105, "Ice", false, LegendaryChance, 120.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2551, typeof(ZombieFish), 1116101, "Gravewater Lake", false, LegendaryChance, 115.1)); //Confirmed

            //Cannot fish up but used for bait
            m_FishInfos.Add(new FishInfo(1170, typeof(Charydbis), 1150208, "cannotfishup", true, LegendaryChance, 120.0));

            m_InvalidatedLocations = false;
            Timer.DelayCall(TimeSpan.FromSeconds(10), InvalidateLocations);
        }

        public static List<FishInfo> FishInfos => m_FishInfos;
        private static readonly List<FishInfo> m_FishInfos = new List<FishInfo>();

        private readonly int m_Hue;
        private readonly Type m_Type;
        private readonly object m_Label;
        private object m_Location;
        private readonly bool m_RequiresDeepWater;
        private readonly double m_BaseChance;
        private readonly double m_MinSkill;

        public int Hue => m_Hue;
        public Type Type => m_Type;
        public object LabelNumber => m_Label;
        public object Location { get { return m_Location; } set { m_Location = value; } }
        public bool RequiresDeepWater => m_RequiresDeepWater;
        public double BaseChance => m_BaseChance;
        public double MinSkill => m_MinSkill;

        public static readonly double RareChance = 0.008;
        public static readonly double LegendaryChance = 0.002;

        public static bool m_InvalidatedLocations;

        public FishInfo(int hue, Type type, object label, object found, bool deepwater, double basechance, double minskill)
        {
            m_Hue = hue;
            m_Type = type;
            m_Label = label;
            m_Location = found;
            m_RequiresDeepWater = deepwater;
            m_BaseChance = basechance;
            m_MinSkill = minskill;
        }

        public static void InvalidateLocations()
        {
            if (m_InvalidatedLocations)
                return;

            int c = 0;

            foreach (FishInfo info in m_FishInfos)
            {
                if (info.Location is string)
                {
                    string str = info.Location as string;

                    if (str == "Trammel")
                    {
                        info.Location = Map.Trammel;
                        c++;
                    }
                    else if (str == "Felucca")
                    {
                        info.Location = Map.Felucca;
                        c++;
                    }
                    else if (str == "Tokuno")
                    {
                        info.Location = Map.Tokuno;
                        c++;
                    }
                    else if (str == "Ilshenar")
                    {
                        info.Location = Map.Ilshenar;
                        c++;
                    }
                    else if (str == "Malas")
                    {
                        info.Location = Map.Malas;
                        c++;
                    }
                    else if (str == "TerMur")
                    {
                        info.Location = Map.TerMur;
                        c++;
                    }
                }
            }

            m_InvalidatedLocations = true;
        }

        public static bool IsRareFish(Type type)
        {
            return m_FishInfos.Any(info => info.Type == type);
        }

        public static FishInfo GetInfo(Type type)
        {
            return m_FishInfos.FirstOrDefault(info => info.Type == type);
        }

        public static FishInfo GetInfo(int hue)
        {
            return m_FishInfos.FirstOrDefault(info => info.Hue == hue);
        }

        public static int GetIndexFromType(Type type)
        {
            if (type == null)
                return -1;

            for (int i = 0; i < m_FishInfos.Count; i++)
                if (m_FishInfos[i].Type == type)
                    return i;
            return -1;
        }

        public static Type GetTypeFromIndex(int index)
        {
            if (index < 0 || index >= m_FishInfos.Count)
                return m_FishInfos[0].Type;

            return m_FishInfos[index].Type;
        }

        public static int GetFishHue(int index)
        {
            if (index < 0 || index >= m_FishInfos.Count)
                return 0;

            return m_FishInfos[index].Hue;
        }

        public static int GetFishHue(Type type)
        {
            var info = m_FishInfos.FirstOrDefault(i => i.Type == type);

            if (info != null)
            {
                return info.Hue;
            }

            return 0;
        }

        public static object GetFishLabel(int index)
        {
            if (index < 0 || index >= m_FishInfos.Count)
                return 0;

            return m_FishInfos[index].LabelNumber;
        }

        public static object GetFishLabel(Type type)
        {
            var info = m_FishInfos.FirstOrDefault(i => i.Type == type);

            if (info != null)
            {
                return info.LabelNumber;
            }

            return null;
        }

        public static string GetFishLocation(Type type)
        {
            var info = m_FishInfos.FirstOrDefault(i => i.Type == type);

            if (info != null)
            {
                return info.Location.ToString();
            }

            return null;
        }

        public static Type GetSpecialItem(Mobile from, Item harvestItem, Point3D pnt, bool rareOnly)
        {
            return GetSpecialItem(from, harvestItem, pnt, 0, rareOnly);
        }

        public static Type GetSpecialItem(Mobile from, Item harvestItem, Point3D pnt, double bump, bool rareOnly)
        {
            InvalidateLocations();

            Map map = from.Map;
            Region reg = from.Region;

            if (reg.Parent != null)
                reg = reg.Parent;

            double skill = from.Skills[SkillName.Fishing].Value;
            bool fishing = harvestItem is FishingPole;

            Type bait = null;
            Type item = null;
            bool enhanced = false;

            var baitable = harvestItem as IBaitable;

            if (baitable != null)
            {
                bait = baitable.BaitType;
                enhanced = baitable.EnhancedBait;
            }

            var infos = GetInfoFor(from);

            foreach (var info in infos.OrderByDescending(i => i.Type == bait))
            {
                if (info.Roll(from, bait, enhanced, bump))
                {
                    item = info.Type;
                    break;
                }
            }

            ColUtility.Free(infos);

            if (!rareOnly && item == null && from is PlayerMobile)
            {
                double chance = skill / 121.5;

                bool dungeon = IsDungeon(pnt, map, reg);
                bool shore = IsShore(pnt, map, reg);
                bool deep = IsDeepWater(pnt, map, reg);

                if (fishing && chance >= Utility.RandomDouble())
                {
                    if (dungeon && skill >= 106.0)
                        item = BaseHighseasFish.DungeonFish[Utility.Random(BaseHighseasFish.DungeonFish.Length)];
                    else if (deep && skill >= 80.0)
                        item = BaseHighseasFish.DeepWaterFish[Utility.Random(BaseHighseasFish.DeepWaterFish.Length)];
                    else if (shore && skill >= 50.0)
                        item = BaseHighseasFish.ShoreFish[Utility.Random(BaseHighseasFish.ShoreFish.Length)];
                }
                else if (!fishing && skill >= 50.0 && chance >= Utility.RandomDouble())
                {
                    item = BaseHighseasFish.LobstersAndCrabs[Utility.Random(BaseHighseasFish.LobstersAndCrabs.Length)];
                }
            }

            return item;
        }

        public static List<FishInfo> GetInfoFor(IEntity fisher)
        {
            var list = new List<FishInfo>();
            var fisherMap = fisher.Map;
            var fisherLoc = fisher.Location;

            for (int i = 0; i < m_FishInfos.Count; i++)
            {
                var info = m_FishInfos[i];

                if (info.Location is string)
                {
                    string loc = (string)info.Location;

                    if (loc.ToLower() == "cannotfishup")
                        continue;

                    switch (loc)
                    {
                        case "T2A":
                            if (SpellHelper.IsAnyT2A(fisherMap, fisherLoc))
                            {
                                list.Add(info);
                            }
                            break;
                        case "TrammelAndFelucca":
                            if ((fisherMap == Map.Trammel || fisherMap == Map.Felucca) && SpellHelper.IsAnyT2A(fisherMap, fisherLoc))
                            {
                                list.Add(info);
                            }
                            break;
                        case "Gravewater Lake":
                            if (IsGravewaterLake(fisherLoc, fisherMap))
                            {
                                list.Add(info);
                            }
                            break;
                        default:
                            if (Region.Find(fisherLoc, fisherMap).IsPartOf(loc))
                            {
                                list.Add(info);
                            }
                            break;
                    }
                }
                else if (info.Location is Map && fisherMap == (Map)info.Location)
                {
                    list.Add(info);
                }
            }

            return list;
        }

        public double GetBaitStrength(Type baitType, Mobile from, bool enhanced)
        {
            if (baitType != Type)
                return 1.0;

            double str = enhanced ? 3.0 : 2.0;

            for (int i = 0; i < from.Items.Count; i++)
            {
                Item item = from.Items[i];

                if (item is IFishingAttire)
                {
                    if (item is ISetItem)
                    {
                        if (((ISetItem)item).SetEquipped)
                        {
                            str += ((double)((IFishingAttire)item).SetBonus / 100) / ((ISetItem)item).Pieces;
                        }
                        else
                        {
                            str += (double)((IFishingAttire)item).BaitBonus / 100;
                        }
                    }
                    else
                    {
                        str += (double)((IFishingAttire)item).BaitBonus / 100;
                    }
                }
            }

            return str;
        }

        public bool Roll(Mobile from, Type bait, bool enhanced, double bump)
        {
            var baitStr = GetBaitStrength(bait, from, enhanced);
            double baseChance = MagicalFishFinder.HasSchool(from) ? BaseChance * 10 : BaseChance;

            return (baseChance + bump) * baitStr > Utility.RandomDouble();
        }

        public static bool IsDeepWater(Point3D pnt, Map map, Region region)
        {
            return SpecialFishingNet.FullValidation(map, pnt.X, pnt.Y) && !IsDungeon(pnt, map, region);
        }

        public static bool IsShore(Point3D pnt, Map map, Region region)
        {
            return !IsDeepWater(pnt, map, region) && !IsDungeon(pnt, map, region);
        }

        public static bool IsDungeon(Point3D pnt, Map map, Region region)
        {
            return region.IsPartOf<DungeonRegion>() || IsMondainDungeon(region) || SpellHelper.IsTrammelWind(map, pnt) || SpellHelper.IsFeluccaWind(map, pnt);
        }

        public static bool IsMondainDungeon(Region region)
        {
            if (region.IsPartOf("Twisted Weald"))
                return true;
            if (region.IsPartOf("Sanctuary"))
                return true;
            if (region.IsPartOf("Prism of Light"))
                return true;
            if (region.IsPartOf("Citadel"))
                return true;
            if (region.IsPartOf("Bedlam"))
                return true;
            if (region.IsPartOf("Blighted Grove"))
                return true;
            if (region.IsPartOf("Painted Caves"))
                return true;
            if (region.IsPartOf("Palace of Paroxysmus"))
                return true;
            if (region.IsPartOf("Labyrinth"))
                return true;
            return false;
        }

        public static bool IsFireIsland(Point3D p, Map map)
        {
            return (map == Map.Felucca || map == Map.Trammel) && (p.X > 4559 && p.X < 4636 && p.Y > 3548 && p.Y < 3627
                        || p.X > 4465 && p.X < 4493 && p.Y > 4479 && p.Y < 3746);
        }

        public static bool IsGravewaterLake(Point3D p, Map map)
        {
            return map == Map.Malas && ((p.X >= 1440 && p.X <= 1863 && p.Y >= 1527 && p.Y <= 1746) || (p.X >= 1381 && p.X <= 1596 && p.Y >= 1565 && p.Y <= 1789));
        }

        public static Type[] SOSArtifacts => m_SOSArtifacts;
        private static readonly Type[] m_SOSArtifacts = new Type[]
        {
            typeof(AntiqueWeddingDress), typeof(GrapeVine),
            typeof(KelpWovenLeggings),   typeof(LargeFishingNet),
            typeof(RunedDriftwoodBow),   typeof(ValkyrieArmor)
        };

    }
}
