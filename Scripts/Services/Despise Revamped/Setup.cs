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
            CommandSystem.Register("DeleteDespise", AccessLevel.GameMaster, new CommandEventHandler(DeleteDespise_OnCommand));
        }

        private static void DeleteDespise_OnCommand(CommandEventArgs e)
        {
            WeakEntityCollection.Delete("despise");
            DespiseController.Instance = null;
        }

        public static void SetupDespise_OnCommand(CommandEventArgs e)
        {
            if (DespiseController.Instance == null)
            {
                DespiseController controller = new DespiseController();
                WeakEntityCollection.Add("despise", controller);
                controller.MoveToWorld(new Point3D(5571, 626, 30), Map.Trammel);

                DespiseAnkh ankh = new DespiseAnkh(Alignment.Good);
                WeakEntityCollection.Add("despise", ankh);
                ankh.MoveToWorld(new Point3D(5474, 525, 79), Map.Trammel);

                ankh = new DespiseAnkh(Alignment.Evil);
                WeakEntityCollection.Add("despise", ankh);
                ankh.MoveToWorld(new Point3D(5472, 754, 10), Map.Trammel);

                Moongate gate1 = new Moongate(false);
                Moongate gate2 = new Moongate(false);
                WeakEntityCollection.Add("despise", gate1);
                WeakEntityCollection.Add("despise", gate2);

                //Gate1
                gate1.MoveToWorld(new Point3D(5475, 735, 5), Map.Trammel);
                gate2.MoveToWorld(new Point3D(5458, 610, 50), Map.Trammel);
                HueGates(gate1, gate2);
                LinkGates(gate1, gate2);

                gate1 = new Moongate(false);
                gate2 = new Moongate(false);
                WeakEntityCollection.Add("despise", gate1);
                WeakEntityCollection.Add("despise", gate2);

                //Gate2
                gate1.MoveToWorld(new Point3D(5459, 674, 20), Map.Trammel);
                gate2.MoveToWorld(new Point3D(5454, 522, 60), Map.Trammel);
                HueGates(gate1, gate2);
                LinkGates(gate1, gate2);

                gate1 = new Moongate(false);
                gate2 = new Moongate(false);
                WeakEntityCollection.Add("despise", gate1);
                WeakEntityCollection.Add("despise", gate2);

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
                WeakEntityCollection.Add("despise", wisp);
                wisp.MoveToWorld(new Point3D(1303, 1088, 0), Map.Trammel);

                foreach (Item item in eable)
                {
                    if (item is Teleporter)
                    {
                        Teleporter old = (Teleporter)item;

                        tele = new DespiseTeleporter();
                        WeakEntityCollection.Add("despise", tele);
                        tele.PointDest = old.PointDest;
                        tele.MapDest = old.MapDest;
                        tele.MoveToWorld(old.Location, old.Map);

                        old.Delete();
                    }
                }
                eable.Free();

                e.Mobile.SendMessage("Despise setup complete");
            }
            else
                e.Mobile.SendMessage("Despise appears to already be setup");
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