using System;
using Server;
using System.Collections.Generic;
using Server.Multis;
using Server.Regions;
using Server.Mobiles;
using Server.Engines.Quests;

namespace Server.Items
{
    public class FishInfo
    {
        public static void Configure()
        {
            //Rare Fish; 0 - 15
            m_FishInfos.Add(new FishInfo(2506, typeof(AutumnDragonfish),    1116090, "Ilshenar",                 false, RareChance, 105.4));//Confirmed
            m_FishInfos.Add(new FishInfo(2591, typeof(BlueLobster),         1116366, "Ice",                      false, RareChance, 103.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(1157, typeof(BullFish),            1116095, "Labyrinth",                false, RareChance, 105.4)); //Confirmed
            m_FishInfos.Add(new FishInfo(1167, typeof(CrystalFish),         1116092, "Prism of Light",           false, RareChance, 105.4)); //Confirmed
            m_FishInfos.Add(new FishInfo(2578, typeof(FairySalmon),         1116089, "TerMur",                   false, RareChance, 85.8)); //Confirmed
            m_FishInfos.Add(new FishInfo(1461, typeof(FireFish),            1116093, "Shame",                    false, RareChance, 95.8)); //Confirmed
            m_FishInfos.Add(new FishInfo(1257, typeof(GiantKoi),            1116088, "Tokuno",                   true,  RareChance, 95.8)); //Confirmed
            m_FishInfos.Add(new FishInfo(2579/*1287*/, typeof(GreatBarracuda),1116100, "Felucca",                true,  RareChance, 89.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2959, typeof(HolyMackerel),        1116087, "Gravewater Lake",          false, RareChance, 102.9)); //Confirmed
            m_FishInfos.Add(new FishInfo(2075, typeof(LavaFish),            1116096, "Abyss",                    false, RareChance, 110.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2075/*1152*/, typeof(ReaperFish),  1116094, "Doom",                     false, RareChance, 98.1));  //Confirmed
            m_FishInfos.Add(new FishInfo(2539, typeof(SpiderCrab),          1116367, "Terathan Keep",            false, RareChance, 103.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(2558, typeof(StoneCrab),           1116365, "T2A",                      true,  RareChance, 103.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(43,  typeof(SummerDragonfish),    1116091, "Destard",                   false, RareChance, 105.2));  //Confirmed
            m_FishInfos.Add(new FishInfo(1154, typeof(UnicornFish),         1116086, "Twisted Weald",            false, RareChance, 110.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2515, typeof(YellowtailBarracuda), 1116098, "Trammel",                  true,  RareChance, 81.9));//Confirmed

            //Legendary Fish ; 16-34
            m_FishInfos.Add(new FishInfo(2406, typeof(AbyssalDragonfish),   1116118, "Destard",                  false, LegendaryChance, 120.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2500, typeof(BlackMarlin),         1116099, "Felucca",                  true,  LegendaryChance, 110.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(2117, typeof(BloodLobster),        1116370, "Shame",                    false, LegendaryChance, 115.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(0,    typeof(BlueMarlin),          1116097, "Trammel",                  true,  LegendaryChance, 105.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(1777, typeof(DreadLobster),        1116371, "Doom",                     false, LegendaryChance, 115.1));
            m_FishInfos.Add(new FishInfo(1158, typeof(DungeonPike),         1116107, "Terathan Keep",            false, LegendaryChance, 105.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(2576, typeof(GiantSamuraiFish),    1116103, "Tokuno",                   true,  LegendaryChance, 110.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(1281, typeof(GoldenTuna),          1116102, "Tokuno",                   true,  LegendaryChance, 105.0));  //Confirmed
            m_FishInfos.Add(new FishInfo(2604, typeof(Kingfish),            1116085, "TrammelAndFelucca",        true,  LegendaryChance, 92.2));   //Confirmed
            m_FishInfos.Add(new FishInfo(1463, typeof(LanternFish),         1116106, "Prism of Light",           false, LegendaryChance, 105.1));  //Confirmed
            m_FishInfos.Add(new FishInfo(1283, typeof(RainbowFish),         1116108, "Twisted Weald",            false, LegendaryChance, 105.1));
            m_FishInfos.Add(new FishInfo(2076, typeof(SeekerFish),          1116109, "Labyrinth",                false, LegendaryChance, 105.1));
            m_FishInfos.Add(new FishInfo(0,    typeof(SpringDragonfish),    1116104, "Ilshenar",                 false, LegendaryChance, 120.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(1837, typeof(StoneFish),           1116110, "T2A",                      true,  LegendaryChance, 115.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(2600, typeof(TunnelCrab),          1116372, "Underworld",               false, LegendaryChance, 115.1)); //Confirmed
            m_FishInfos.Add(new FishInfo(2069, typeof(VoidCrab),            1116368, "TerMur",                   false, LegendaryChance, 120.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2071, typeof(VoidLobster),         1116369, "Abyss",                    false, LegendaryChance, 120.0));
            m_FishInfos.Add(new FishInfo(2499, typeof(WinterDragonfish),    1116105, "Ice",                      false, LegendaryChance, 120.0)); //Confirmed
            m_FishInfos.Add(new FishInfo(2551, typeof(ZombieFish),          1116101, "Gravewater Lake",          false, LegendaryChance, 115.1)); //Confirmed
            
            //Cannot fish up but used for bait
            m_FishInfos.Add(new FishInfo(1170, typeof(Charydbis),           1150208, "cannotfishup",     true,      LegendaryChance, 120.0));

            m_InvalidatedLocations = false;
            Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(InvalidateLocations));
        }

        public static List<FishInfo> FishInfos { get { return m_FishInfos; } }
        private static List<FishInfo> m_FishInfos = new List<FishInfo>();

        private int m_Hue;
        private Type m_Type;
        private object m_Label;
        private object m_Location;
        private bool m_RequiresDeepWater;
        private double m_BaseChance;
        private double m_MinSkill;

        public int Hue { get { return m_Hue; } }
        public Type Type { get { return m_Type; } }
        public object LabelNumber { get { return m_Label; } }
        public object Location { get { return m_Location; } set { m_Location = value; } }
        public bool RequiresDeepWater { get { return m_RequiresDeepWater; } }
        public double BaseChance { get { return m_BaseChance; } }
        public double MinSkill { get { return m_MinSkill; } }

        public static readonly double RareChance = 0.0075;
        public static readonly double LegendaryChance = 0.001;

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
            //Console.WriteLine("Invalidated {0} Map Locations in FishInfo.cs", c.ToString());
        }

        public static bool IsRareFish(Type type)
        {
            foreach (FishInfo info in m_FishInfos)
                if (info.Type == type)
                    return true;
            return false;
        }

        public static FishInfo GetInfo(Type type)
        {
            foreach (FishInfo info in m_FishInfos)
                if (info.Type == type) 
                    return info;
            return null;
        }

        public static FishInfo GetInfo(int hue)
        {
            foreach (FishInfo info in m_FishInfos) 
                if (info.Hue == hue)
                    return info;

            return null;
        }

        public static int GetIndexFromType(Type type)
        {
            if(type == null)
                return -1;

            for(int i = 0; i < m_FishInfos.Count; i++)
                if(m_FishInfos[i].Type == type)
                    return i;
            return -1;
        }

        public static Type GetTypeFromIndex(int index)
        {
            if(index < 0 || index >= m_FishInfos.Count)
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
            foreach (FishInfo info in m_FishInfos)
                if (info.Type == type)
                    return info.Hue;
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
            foreach (FishInfo info in FishInfos)
            {
                if (info.Type == type)
                    return info.LabelNumber;
            }
            return null;
        }

        public static string GetFishLocation(Type type)
        {
            foreach (FishInfo info in FishInfos)
            {
                if (info.Type == type)
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
            Point3D fromLoc = from.Location;

            Region reg = from.Region;

            if (reg.Parent != null)
                reg = reg.Parent;

            double skill = from.Skills[SkillName.Fishing].Value;
            bool fishing = harvestItem is FishingPole;

            Type bait = null;
            Type item = null;
            bool enhanced = false;

            if (harvestItem is FishingPole)
            {
                bait = ((FishingPole)harvestItem).BaitType;
                enhanced = ((FishingPole)harvestItem).EnhancedBait;
            }
            else if (harvestItem is LobsterTrap)
            {
                bait = ((LobsterTrap)harvestItem).BaitType;
                enhanced = ((LobsterTrap)harvestItem).EnhancedBait;
            }

            //insertion of baited type first to increase chances of fishing it up!
            List<FishInfo> infos = new List<FishInfo>(m_FishInfos);

            if (bait != null) {
                for (int i = 0; i < infos.Count; i++)
                {
                    FishInfo info = infos[i];
                    if (info.Type == bait)
                    {
                        infos.Remove(info);
                        infos.Insert(0, info);
                    }
                }
            }

            for (int i = 0; i < infos.Count; i++)
            {
                FishInfo info = infos[i];

                double baitStr = info.GetBaitStrength(bait, from, enhanced);

                if ((info.RequiresDeepWater && !IsDeepWater(pnt, map, reg)) || skill < info.MinSkill)
                    continue;

                if (fishing && info.Type.IsSubclassOf(typeof(BaseCrabAndLobster)))
                    continue;

                if (!fishing && !info.Type.IsSubclassOf(typeof(BaseCrabAndLobster)))
                    continue;

                if (info.Location is string)
                {
                    string loc = (string)info.Location;

                    if (loc.ToLower() == "cannotfishup")
                        continue;

                    if (loc.ToLower() == "t2a" && Server.Spells.SpellHelper.IsAnyT2A(map, fromLoc) && info.Roll(from, baitStr, bump))
                        item = info.Type;

                    if (loc.ToLower() == "trammelandfelucca" && (map == Map.Trammel || map == Map.Felucca) && !Server.Spells.SpellHelper.IsAnyT2A(map, fromLoc) && info.Roll(from, baitStr, bump))
                        item = info.Type;

                    if (loc.ToLower() == "fire island" && (map == Map.Felucca || map == Map.Trammel) && (from.X > 4559 && from.X < 4636 && from.Y > 3548 && from.Y < 3627
                        || from.X > 4465 && from.X < 4493 && from.Y > 4479 && from.Y < 3746) && info.Roll(from, baitStr, bump))
                        item = info.Type;

                    if (from.Region != null && from.Region.IsPartOf(loc) && info.Roll(from, baitStr, bump))
                        item = info.Type;

                }
                else if (info.Location is Map)
                {
                    Map locMap = (Map)info.Location;

                    if (map == locMap && info.Roll(from, baitStr, bump))
                        item = info.Type;
                }
            }

            if (!rareOnly && item == null && from is PlayerMobile)
            {
                double chance = ((double)skill + 20.0) / 100.0;

                bool dungeon = IsDungeon(pnt, map, reg);
                bool shore = IsShore(pnt, map, reg);
                bool deep = IsDeepWater(pnt, map, reg);

                if (fishing && chance >= Utility.RandomDouble())
                {
                    if (dungeon && skill >= 106.0)
                        item = BaseHighseasFish.DungeonFish[Utility.Random(BaseHighseasFish.DungeonFish.Length)];
                    else if (deep && skill >= 80.0)
                        item = BaseHighseasFish.DeepWaterFish[Utility.Random(BaseHighseasFish.DeepWaterFish.Length)];
                    else if(shore && skill >= 50.0)
                        item = BaseHighseasFish.ShoreFish[Utility.Random(BaseHighseasFish.ShoreFish.Length)];
                }
                else if (!fishing && chance >= Utility.RandomDouble() && skill >= 50.0)
                    item = BaseHighseasFish.LobstersAndCrabs[Utility.Random(BaseHighseasFish.LobstersAndCrabs.Length)];
            }

            return item;
        }

        public double GetBaitStrength(Type baitType, Mobile from, bool enhanced)
        {
            if (baitType != this.Type)
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
                            str += ((double)((IFishingAttire)item).SetBonus / 100) / (double)((ISetItem)item).Pieces;
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

        public bool Roll(Mobile from, double baitStr, double bump)
        {
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
            return region.IsPartOf(typeof(DungeonRegion)) || IsMondainDungeon(region) || Server.Spells.SpellHelper.IsTrammelWind(map, pnt) || Server.Spells.SpellHelper.IsFeluccaWind(map, pnt);
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

        public static Type[] SOSArtifacts { get { return m_SOSArtifacts; } }
        private static Type[] m_SOSArtifacts = new Type[]
        {
            typeof(AntiqueWeddingDress), typeof(GrapeVine),
            typeof(KelpWovenLeggings),   typeof(LargeFishingNet),
            typeof(RunedDriftwoodBow),   typeof(ValkyrieArmor)
        };

    }
}