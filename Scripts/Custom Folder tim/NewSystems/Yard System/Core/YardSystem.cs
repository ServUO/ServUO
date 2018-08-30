using System.Collections.Generic;
using System;
using Server.Commands;
using Server.Items;

namespace Server.ACC.YS
{
    public class YardSystem
    {
        public static void Initialize()
        {
            EventSink.WorldSave += new WorldSaveEventHandler(StartTimer);
            CommandSystem.Register("UpdateYardGates", AccessLevel.GameMaster, new CommandEventHandler(UpdateOldGates));
        }

        //This command should not need to be used, but I left it in just in case.
        [Usage("UpdateYardGates")]
        [Description("Updates the old version of yard gates to the new version.")]
        public static void UpdateOldGates(CommandEventArgs e)
        {
            List<Item> toDelete = new List<Item>();

            foreach (Item item in World.Items.Values)
            {
                if (item.GetType() == typeof(YardIronGate) ||
                    item.GetType() == typeof(YardShortIronGate) ||
                    item.GetType() == typeof(YardLightWoodGate) ||
                    item.GetType() == typeof(YardDarkWoodGate))
                {
                    toDelete.Add(item);
                }
            }

            for (int i = 0; i < toDelete.Count; i++)
            {
                if (toDelete[i].GetType() == typeof(YardIronGate))
                {
                    YardIronGate gate = (YardIronGate)toDelete[i];
                    if (gate != null)
                    {
                        YardGate newGate = new YardGate(2084, gate.Placer, gate.Price, null, gate.Location, (DoorFacing)((gate.ClosedID - 2084) / 2));
                    }
                }
                else if (toDelete[i].GetType() == typeof(YardShortIronGate))
                {
                    YardShortIronGate gate = (YardShortIronGate)toDelete[i];
                    if (gate != null)
                    {
                        YardGate newGate = new YardGate(2124, gate.Placer, gate.Price, null, gate.Location, (DoorFacing)((gate.ClosedID - 2124) / 2));
                    }
                }
                else if (toDelete[i].GetType() == typeof(YardLightWoodGate))
                {
                    YardLightWoodGate gate = (YardLightWoodGate)toDelete[i];
                    if (gate != null)
                    {
                        YardGate newGate = new YardGate(2105, gate.Placer, gate.Price, null, gate.Location, (DoorFacing)((gate.ClosedID - 2105) / 2));
                    }
                }
                else if (toDelete[i].GetType() == typeof(YardDarkWoodGate))
                {
                    YardDarkWoodGate gate = (YardDarkWoodGate)toDelete[i];
                    if (gate != null)
                    {
                        YardGate newGate = new YardGate(2150, gate.Placer, gate.Price, null, gate.Location, (DoorFacing)((gate.ClosedID - 2150) / 2));
                    }
                }

                toDelete[i].Delete();
            }

            World.Save();
        }

        public static List<Item> OrphanedYardItems = new List<Item>();

        public static void AddOrphanedItem(Item item)
        {
            if (OrphanedYardItems == null)
            {
                OrphanedYardItems = new List<Item>();
            }

            if (item == null)
            {
                return;
            }

            OrphanedYardItems.Add(item);
        }

        public static void StartTimer(WorldSaveEventArgs args)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(YardSettings.SecondsToCleanup), CleanYards);
        }

        public static void CleanYards()
        {
            if (OrphanedYardItems == null || OrphanedYardItems.Count <= 0)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine(String.Format("Cleaning {0} Orphaned Yard Items...", OrphanedYardItems.Count));
            for (int i = 0; i < OrphanedYardItems.Count; i++)
            {
                if (OrphanedYardItems[i] is YardItem)
                {
                    YardItem item = (YardItem)OrphanedYardItems[i];
                    if (item == null)
                    {
                        continue;
                    }
                    item.FindHouseOfPlacer();
                    if (item.House == null)
                    {
                        item.Refund();
                    }
                }
                else if (OrphanedYardItems[i] is YardGate)
                {
                    YardGate item = (YardGate)OrphanedYardItems[i];
                    if (item == null)
                    {
                        continue;
                    }
                    item.FindHouseOfPlacer();
                    if (item.House == null)
                    {
                        item.Refund();
                    }
                }
            }

            OrphanedYardItems.Clear();
        }
    }
}
