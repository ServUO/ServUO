using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using System.Collections.Generic;

namespace Server.Engines.Despise
{
    public static class DespiseRevampedSetup
    {
        public static void Initialize()
        {
            CommandSystem.Register("SetupDespise", AccessLevel.GameMaster, new CommandEventHandler(SetupDespise_OnCommand));
        }

        public static void SetupDespise_OnCommand(CommandEventArgs e)
        {
            if (DespiseController.Instance == null)
            {
                foreach (Region region in Region.Regions)
                {
                    if (region.Name == "Despise" && region.Map == Map.Trammel)
                    {
                        foreach (Sector sector in region.Sectors)
                        {
                            List<Item> list = new List<Item>(sector.Items);

                            foreach (Item item in list)
                            {
                                if (item is XmlSpawner)
                                    ((XmlSpawner)item).DoReset = true;
                            }

                            list.Clear();
                        }
                    }
                }

                CommandEventArgs args = new CommandEventArgs(e.Mobile, null, null, new string[] { @"Data\Monsters\NewDespise" });
                XmlSpawner.Load_OnCommand(args);

                DespiseController controller = new DespiseController();
                controller.MoveToWorld(new Point3D(5571, 626, 30), Map.Trammel);

                DespiseAnkh ankh = new DespiseAnkh(Alignment.Good);
                ankh.MoveToWorld(new Point3D(5474, 525, 79), Map.Trammel);

                ankh = new DespiseAnkh(Alignment.Evil);
                ankh.MoveToWorld(new Point3D(5472, 754, 10), Map.Trammel);

                Moongate gate1 = new Moongate(false);
                Moongate gate2 = new Moongate(false);

                //Gate1
                gate1.MoveToWorld(new Point3D(5475, 735, 5), Map.Trammel);
                gate2.MoveToWorld(new Point3D(5458, 610, 50), Map.Trammel);
                HueGates(gate1, gate2);
                LinkGates(gate1, gate2);

                gate1 = new Moongate(false);
                gate2 = new Moongate(false);

                //Gate2
                gate1.MoveToWorld(new Point3D(5459, 674, 20), Map.Trammel);
                gate2.MoveToWorld(new Point3D(5454, 522, 60), Map.Trammel);
                HueGates(gate1, gate2);
                LinkGates(gate1, gate2);

                gate1 = new Moongate(false);
                gate2 = new Moongate(false);

                //Gate3
                gate1.MoveToWorld(new Point3D(5388, 753, 5), Map.Trammel);
                gate2.MoveToWorld(new Point3D(5387, 628, 30), Map.Trammel);
                HueGates(gate1, gate2);
                LinkGates(gate1, gate2);

                //Teleporters
                IPooledEnumerable eable = Map.Trammel.GetItemsInRange(new Point3D(5588, 631, 30), 2);
                DespiseTeleporter tele = null;

                //Wisp
                MysteriousWisp wisp = new MysteriousWisp();
                wisp.MoveToWorld(new Point3D(1303, 1088, 0), Map.Trammel);

                foreach (Item item in eable)
                {
                    if (item is Teleporter)
                    {
                        Teleporter old = (Teleporter)item;

                        tele = new DespiseTeleporter();
                        tele.PointDest = old.PointDest;
                        tele.MapDest = old.MapDest;
                        tele.MoveToWorld(old.Location, old.Map);

                        old.Delete();
                    }
                }
                eable.Free();

                e.Mobile.SendMessage("Despise Revamped setup! Don't forget to setup mob spawners an activate it!");
            }
            else
                e.Mobile.SendMessage("This has already been setup.");
        }

        private static void HueGates(Moongate one, Moongate two)
        {
            one.Hue = 2715;
            two.Hue = 2677;
        }

        private static void LinkGates(Moongate one, Moongate two)
        {
            one.Target = two.Location;
            two.Target = one.Location;
            one.TargetMap = two.Map;
            two.TargetMap = one.Map;
        }
    }
}