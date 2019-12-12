using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.Quests.Haven;

namespace Server.Items
{
    public static class GenerateUnderworldRooms
    {
        public static void Generate()
        {
            ExperimentalRoomController controller = new ExperimentalRoomController();
            controller.MoveToWorld(new Point3D(980, 1117, -42), Map.TerMur);

            //Room 0 to 1
            ExperimentalRoomDoor door = new ExperimentalRoomDoor(Room.RoomZero, DoorFacing.WestCCW);
            ExperimentalRoomBlocker blocker = new ExperimentalRoomBlocker(Room.RoomZero);
            door.Hue = 1109;
            door.MoveToWorld(new Point3D(984, 1116, -42), Map.TerMur);
            blocker.MoveToWorld(new Point3D(984, 1116, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", door);
            WeakEntityCollection.Add("sa", blocker);

            door = new ExperimentalRoomDoor(Room.RoomZero, DoorFacing.EastCW);
            blocker = new ExperimentalRoomBlocker(Room.RoomZero);
            door.Hue = 1109;
            door.MoveToWorld(new Point3D(985, 1116, -42), Map.TerMur);
            blocker.MoveToWorld(new Point3D(985, 1116, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", door);
            WeakEntityCollection.Add("sa", blocker);

            //Room 1 to 2
            door = new ExperimentalRoomDoor(Room.RoomOne, DoorFacing.WestCCW);
            blocker = new ExperimentalRoomBlocker(Room.RoomOne);
            door.Hue = 1109;
            door.MoveToWorld(new Point3D(984, 1102, -42), Map.TerMur);
            blocker.MoveToWorld(new Point3D(984, 1102, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", door);
            WeakEntityCollection.Add("sa", blocker);

            door = new ExperimentalRoomDoor(Room.RoomOne, DoorFacing.EastCW);
            blocker = new ExperimentalRoomBlocker(Room.RoomOne);
            door.Hue = 1109;
            door.MoveToWorld(new Point3D(985, 1102, -42), Map.TerMur);
            blocker.MoveToWorld(new Point3D(985, 1102, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", door);
            WeakEntityCollection.Add("sa", blocker);

            //Room 2 to 3
            door = new ExperimentalRoomDoor(Room.RoomTwo, DoorFacing.WestCCW);
            blocker = new ExperimentalRoomBlocker(Room.RoomTwo);
            door.Hue = 1109;
            door.MoveToWorld(new Point3D(984, 1090, -42), Map.TerMur);
            blocker.MoveToWorld(new Point3D(984, 1090, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", door);
            WeakEntityCollection.Add("sa", blocker);

            door = new ExperimentalRoomDoor(Room.RoomTwo, DoorFacing.EastCW);
            blocker = new ExperimentalRoomBlocker(Room.RoomTwo);
            door.Hue = 1109;
            door.MoveToWorld(new Point3D(985, 1090, -42), Map.TerMur);
            blocker.MoveToWorld(new Point3D(985, 1090, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", door);
            WeakEntityCollection.Add("sa", blocker);

            //Room 3 to 4
            door = new ExperimentalRoomDoor(Room.RoomTwo, DoorFacing.WestCCW);
            blocker = new ExperimentalRoomBlocker(Room.RoomThree);
            door.Hue = 1109;
            door.MoveToWorld(new Point3D(984, 1072, -42), Map.TerMur);
            blocker.MoveToWorld(new Point3D(984, 1072, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", door);
            WeakEntityCollection.Add("sa", blocker);

            door = new ExperimentalRoomDoor(Room.RoomTwo, DoorFacing.EastCW);
            blocker = new ExperimentalRoomBlocker(Room.RoomThree);
            door.Hue = 1109;
            door.MoveToWorld(new Point3D(985, 1072, -42), Map.TerMur);
            blocker.MoveToWorld(new Point3D(985, 1072, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", door);
            WeakEntityCollection.Add("sa", blocker);

            ExperimentalRoomChest chest = new ExperimentalRoomChest();
            chest.MoveToWorld(new Point3D(984, 1064, -37), Map.TerMur);
            WeakEntityCollection.Add("sa", chest);

            ExperimentalBook instr = new ExperimentalBook();
            instr.Movable = false;
            instr.MoveToWorld(new Point3D(995, 1114, -36), Map.TerMur);
            WeakEntityCollection.Add("sa", instr);

            SecretDungeonDoor dd = new SecretDungeonDoor(DoorFacing.NorthCCW);
            dd.ClosedID = 87;
            dd.OpenedID = 88;
            dd.MoveToWorld(new Point3D(1007, 1119, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", dd);

            LocalizedSign sign = new LocalizedSign(3026, 1113407);  // Experimental Room Access
            sign.Movable = false;
            sign.MoveToWorld(new Point3D(980, 1119, -37), Map.TerMur);
            WeakEntityCollection.Add("sa", sign);

            //Puzze Room
            PuzzleBox box = new PuzzleBox(PuzzleType.WestBox);
            box.MoveToWorld(new Point3D(1090, 1171, 11), Map.TerMur);
            WeakEntityCollection.Add("sa", box);

            box = new PuzzleBox(PuzzleType.EastBox);
            box.MoveToWorld(new Point3D(1104, 1171, 11), Map.TerMur);
            WeakEntityCollection.Add("sa", box);

            box = new PuzzleBox(PuzzleType.NorthBox);
            box.MoveToWorld(new Point3D(1097, 1163, 11), Map.TerMur);
            WeakEntityCollection.Add("sa", box);

            PuzzleBook book = new PuzzleBook();
            book.Movable = false;
            book.MoveToWorld(new Point3D(1109, 1153, -17), Map.TerMur);
            WeakEntityCollection.Add("sa", book);

            PuzzleRoomTeleporter tele = new PuzzleRoomTeleporter();
            tele.PointDest = new Point3D(1097, 1173, 1);
            tele.MapDest = Map.TerMur;
            tele.MoveToWorld(new Point3D(1097, 1175, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", tele);

            tele = new PuzzleRoomTeleporter();
            tele.PointDest = new Point3D(1098, 1173, 1);
            tele.MapDest = Map.TerMur;
            tele.MoveToWorld(new Point3D(1098, 1175, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", tele);

            MetalDoor2 door2 = new MetalDoor2(DoorFacing.WestCCW);
            door2.Locked = true;
            door2.KeyValue = 50000;
            door2.MoveToWorld(new Point3D(1097, 1174, 1), Map.TerMur);
            WeakEntityCollection.Add("sa", door2);

            door2 = new MetalDoor2(DoorFacing.EastCW);
            door2.Locked = true;
            door2.KeyValue = 50000;
            door2.MoveToWorld(new Point3D(1098, 1174, 1), Map.TerMur);
            WeakEntityCollection.Add("sa", door);

            Teleporter telep = new Teleporter();
            telep.PointDest = new Point3D(1097, 1175, 0);
            telep.MapDest = Map.TerMur;
            telep.MoveToWorld(new Point3D(1097, 1173, 1), Map.TerMur);
            WeakEntityCollection.Add("sa", telep);

            telep = new Teleporter();
            telep.PointDest = new Point3D(1098, 1175, 0);
            telep.MapDest = Map.TerMur;
            telep.MoveToWorld(new Point3D(1098, 1173, 1), Map.TerMur);
            WeakEntityCollection.Add("sa", telep);

            telep = new Teleporter();
            telep.PointDest = new Point3D(996, 1117, -42);
            telep.MapDest = Map.TerMur;
            telep.MoveToWorld(new Point3D(980, 1064, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", telep);

            Static sparkle = new Static(14138);
            sparkle.MoveToWorld(new Point3D(980, 1064, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", sparkle);

            //Maze of Death
            UnderworldPuzzleBox pBox = new UnderworldPuzzleBox();
            pBox.MoveToWorld(new Point3D(1068, 1026, -37), Map.TerMur);
            WeakEntityCollection.Add("sa", pBox);

            GoldenCompass compass = new GoldenCompass();
            compass.MoveToWorld(new Point3D(1070, 1055, -34), Map.TerMur);
            WeakEntityCollection.Add("sa", compass);

            Item map = new RolledMapOfTheUnderworld();
            map.MoveToWorld(new Point3D(1072, 1055, -36), Map.TerMur);
            map.Movable = false;
            WeakEntityCollection.Add("sa", map);

            FountainOfFortune f = new FountainOfFortune();
            f.MoveToWorld(new Point3D(1121, 957, -42), Map.TerMur);
            WeakEntityCollection.Add("sa", f);

            Item tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1121, 965, -41), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1122, 965, -40), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1123, 965, -41), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1124, 965, -41), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1122, 964, -41), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1123, 964, -41), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1123, 963, -40), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1123, 962, -40), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1123, 961, -41), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1122, 961, -41), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1122, 960, -41), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1121, 960, -41), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            tile = new InvisibleTile();
            tile.MoveToWorld(new Point3D(1121, 959, -41), Map.TerMur);
            WeakEntityCollection.Add("sa", tile);

            GenerateRevealTiles();
            CheckCannoneers();

            Console.WriteLine("Experimental Room, Puzzle Room and Maze of Death initialized.");
        }

        public static void GenerateRevealTiles()
        {
            Map map = Map.TerMur;

            for (int x = 1182; x <= 1192; x++)
            {
                for (int y = 1120; y <= 1134; y++)
                {
                    if (map != null && map.CanSpawnMobile(x, y, -42))
                    {
                        var t = new RevealTile();
                        t.MoveToWorld(new Point3D(x, y, -42), map);
                        WeakEntityCollection.Add("sa", t);
                    }
                }
            }

            var tile = new RevealTile();
            tile.MoveToWorld(new Point3D(1180, 883, 0), map);
            WeakEntityCollection.Add("sa", tile);

            tile = new RevealTile();
            tile.MoveToWorld(new Point3D(1180, 882, 0), map);
            WeakEntityCollection.Add("sa", tile);

            tile = new RevealTile();
            tile.MoveToWorld(new Point3D(1180, 881, 0), map);
            WeakEntityCollection.Add("sa", tile);

            tile = new RevealTile();
            tile.MoveToWorld(new Point3D(1180, 880, 0), map);
            WeakEntityCollection.Add("sa", tile);

            tile = new RevealTile();
            tile.MoveToWorld(new Point3D(1180, 879, 0), map);
            WeakEntityCollection.Add("sa", tile);
        }

        public static void CheckCannoneers()
        {
            Cannon cannon = Map.TerMur.FindItem<Cannon>(new Point3D(1126, 1200, -2));
            MilitiaCanoneer cannoneer = null;

            if (cannon == null)
            {
                cannon = new Cannon(CannonDirection.North);
                cannon.MoveToWorld(new Point3D(1126, 1200, -2), Map.TerMur);
                WeakEntityCollection.Add("sa", cannon);
            }

            cannoneer = Map.TerMur.FindMobile<MilitiaCanoneer>(new Point3D(1126, 1203, -2));

            if (cannoneer == null)
            {
                cannoneer = new MilitiaCanoneer();
                cannoneer.MoveToWorld(new Point3D(1126, 1203, -2), Map.TerMur);
                WeakEntityCollection.Add("sa", cannoneer);

            }

            cannon.Canoneer = cannoneer;

            cannon = Map.TerMur.FindItem<Cannon>(new Point3D(1131, 1200, -2));
            cannoneer = null;

            if (cannon == null)
            {
                cannon = new Cannon(CannonDirection.North);
                cannon.MoveToWorld(new Point3D(1131, 1200, -2), Map.TerMur);
                WeakEntityCollection.Add("sa", cannon);
            }

            cannoneer = Map.TerMur.FindMobile<MilitiaCanoneer>(new Point3D(1131, 1203, -2));

            if (cannoneer == null)
            {
                cannoneer = new MilitiaCanoneer();
                cannoneer.MoveToWorld(new Point3D(1131, 1203, -2), Map.TerMur);
                WeakEntityCollection.Add("sa", cannoneer);
            }

            cannon.Canoneer = cannoneer;
        }

        private static bool FindItem(Point3D p, Map map)
        {
            IPooledEnumerable eable = map.GetItemsInRange(p, 0);

            foreach (Item item in eable)
            {
                eable.Free();
                return true;
            }

            eable.Free();
            return false;
        }
    }
}