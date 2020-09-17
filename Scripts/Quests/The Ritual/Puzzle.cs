using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Engines.Quests.RitualQuest
{
    public class CrystalLotusPuzzle : Item
    {
        public static CrystalLotusPuzzle Instance { get; set; }

        public static void Initialize()
        {
            if (Instance == null)
            {
                Instance = new CrystalLotusPuzzle();
                Instance.MoveToWorld(new Point3D(978, 2876, 37), Map.TerMur);

                Static s = new Static(0x283B)
                {
                    Hue = 1152,
                    Name = "Pristine Crystal Lotus"
                };
                s.MoveToWorld(new Point3D(978, 2876, 47), Map.TerMur);
            }
        }

        public List<PuzzleTile> Tiles { get; set; } = new List<PuzzleTile>();
        public PuzzleTile[] Order { get; set; }
        public bool Sequencing { get; set; }
        public Region Region { get; set; }

        public Dictionary<Mobile, PuzzleTile[]> PlayerOrder { get; set; }

        public const int White = 1150;              // hue of white tile
        public const double WhiteLength = 1.2;      // length tile stays white
        public const double SequenceLength = 17.0;

        public CrystalLotusPuzzle()
            : base(0x1223)
        {
            Movable = false;
            Visible = false;
            LoadTiles();

            RegisterRegion();
            DoSequence();
        }

        private void DoSequence()
        {
            if (Region != null && Region.GetPlayerCount() == 0)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(10), DoSequence);
                return;
            }

            Sequencing = true;

            int seqCount = Utility.RandomMinMax(4, 7);
            Order = new PuzzleTile[seqCount];

            PlayerOrder = new Dictionary<Mobile, PuzzleTile[]>();

            for (int i = 0; i < seqCount; i++)
            {
                Order[i] = Tiles[Utility.Random(Tiles.Count)];

                Timer.DelayCall(TimeSpan.FromSeconds(i * WhiteLength), (tile, index) =>
                {
                    tile.Hue = White;

                    Timer.DelayCall(TimeSpan.FromSeconds(WhiteLength), t =>
                    {
                        t.Hue = tile.OriginalHue;
                    }, tile);

                    if (index == seqCount - 1)
                    {
                        Sequencing = false;

                        IPooledEnumerable eable = Map.TerMur.GetClientsInRange(Location, 20);

                        foreach (NetState ns in eable)
                        {
                            if (ns.Mobile != null && ns.Mobile is PlayerMobile && QuestHelper.HasQuest<PristineCrystalLotusQuest>((PlayerMobile)ns.Mobile))
                            {
                                ns.Mobile.SendLocalizedMessage(1151300, "", White); // Complete the puzzle to obtain a Pristine Crystal Lotus.
                            }
                        }

                        eable.Free();

                        Timer.DelayCall(TimeSpan.FromSeconds(Math.Max(SequenceLength, seqCount * 3)), DoSequence);
                    }
                }, Order[i], i);
            }
        }

        public void OnTileClicked(Mobile from, PuzzleTile tile)
        {
            if (Sequencing)
            {
                return;
            }

            if (from is PlayerMobile)
            {
                PristineCrystalLotusQuest quest = QuestHelper.GetQuest<PristineCrystalLotusQuest>((PlayerMobile)from);

                if (quest != null)
                {
                    if (!PlayerOrder.ContainsKey(from))
                    {
                        PlayerOrder[from] = new PuzzleTile[Order.Length];
                    }

                    PuzzleTile[] list = PlayerOrder[from];

                    for (int i = 0; i < Order.Length; i++)
                    {
                        PuzzleTile actual = Order[i];

                        if (list[i] == null)
                        {
                            list[i] = tile;

                            if (i == Order.Length - 1)
                            {
                                if (CheckMatch(list))
                                {
                                    from.SendLocalizedMessage(1151304); // You matched that pattern correctly.

                                    quest.PuzzlesComplete++;

                                    if (quest.PuzzlesComplete >= 5)
                                    {
                                        from.SendLocalizedMessage(1151306); // You may now retrieve a Pristine Crystal Lotus.
                                    }
                                }
                                else
                                {
                                    from.SendLocalizedMessage(1151305); // You did not complete the pattern correctly.
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }

        private bool CheckMatch(PuzzleTile[] list)
        {
            for (int i = 0; i < Order.Length; i++)
            {
                if (Order[i] != list[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool HandlesOnSpeech => true;

        public override void OnSpeech(SpeechEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            if (e.Speech.ToLower() == "i seek the lotus")
            {
                PristineCrystalLotusQuest quest = QuestHelper.GetQuest<PristineCrystalLotusQuest>(pm);

                if (quest != null)
                {
                    pm.SendLocalizedMessage(1151300); // Complete the puzzle to obtain a Pristine Crystal Lotus.
                }
                else
                {
                    pm.SendLocalizedMessage(1151301); // You marvel at the flashing tiles.
                }

                e.Handled = true;
            }
            else if (e.Speech.ToLower() == "give me the lotus")
            {
                PristineCrystalLotusQuest quest = QuestHelper.GetQuest<PristineCrystalLotusQuest>(pm);

                if (quest != null)
                {
                    if (quest.PuzzlesComplete < 5)
                    {
                        pm.SendLocalizedMessage(1151303); // You have not completed the puzzle.
                    }
                    else if (!quest.ReceivedLotus)
                    {
                        PristineCrystalLotus lotus = new PristineCrystalLotus();
                        pm.AddToBackpack(lotus);
                        pm.SendLocalizedMessage(1151302); // A Pristine Crystal Lotus has been placed in your backpack.

                        QuestHelper.CheckRewardItem(pm, lotus);

                        quest.ReceivedLotus = true;
                    }
                }

                e.Handled = true;
            }

        }

        private void LoadTiles()
        {
            Map map = Map.TerMur;

            //West
            PuzzleTile tile = new PuzzleTile(this, 33, 0);
            tile.MoveToWorld(new Point3D(971, 2876, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 14, 0);
            tile.MoveToWorld(new Point3D(971, 2878, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 1195, 0);
            tile.MoveToWorld(new Point3D(972, 2877, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 63, 0);
            tile.MoveToWorld(new Point3D(973, 2876, 37), map);
            Tiles.Add(tile);

            // NorthWest
            tile = new PuzzleTile(this, 63, 1);
            tile.MoveToWorld(new Point3D(978, 2868, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 1195, 1);
            tile.MoveToWorld(new Point3D(979, 2868, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 14, 1);
            tile.MoveToWorld(new Point3D(979, 2869, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 33, 1);
            tile.MoveToWorld(new Point3D(980, 2870, 37), map);
            Tiles.Add(tile);

            // NorthEast
            tile = new PuzzleTile(this, 33, 2);
            tile.MoveToWorld(new Point3D(985, 2870, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 1195, 2);
            tile.MoveToWorld(new Point3D(986, 2870, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 63, 2);
            tile.MoveToWorld(new Point3D(985, 2871, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 14, 2);
            tile.MoveToWorld(new Point3D(986, 2871, 37), map);
            Tiles.Add(tile);

            // East
            tile = new PuzzleTile(this, 14, 3);
            tile.MoveToWorld(new Point3D(985, 2876, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 33, 3);
            tile.MoveToWorld(new Point3D(986, 2877, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 63, 3);
            tile.MoveToWorld(new Point3D(987, 2878, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 1195, 3);
            tile.MoveToWorld(new Point3D(988, 2879, 37), map);
            Tiles.Add(tile);

            // SouthEast
            tile = new PuzzleTile(this, 1195, 4);
            tile.MoveToWorld(new Point3D(982, 2881, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 33, 4);
            tile.MoveToWorld(new Point3D(982, 2882, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 63, 4);
            tile.MoveToWorld(new Point3D(982, 2883, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 14, 4);
            tile.MoveToWorld(new Point3D(981, 2883, 37), map);
            Tiles.Add(tile);

            // SouthWest
            tile = new PuzzleTile(this, 33, 5);
            tile.MoveToWorld(new Point3D(975, 2882, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 14, 5);
            tile.MoveToWorld(new Point3D(976, 2883, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 63, 5);
            tile.MoveToWorld(new Point3D(975, 2884, 37), map);
            Tiles.Add(tile);

            tile = new PuzzleTile(this, 1195, 5);
            tile.MoveToWorld(new Point3D(976, 2885, 37), map);
            Tiles.Add(tile);

            Teleporter tele = new Teleporter(new Point3D(1050, 2940, 38), map, false);
            tele.MoveToWorld(new Point3D(1018, 2915, 38), map);
            Static sparkles = new Static(0x373A);
            sparkles.MoveToWorld(new Point3D(1018, 2915, 38), map);

            tele = new Teleporter(new Point3D(1018, 2915, 38), map, false);
            tele.MoveToWorld(new Point3D(1050, 2940, 38), map);
            sparkles = new Static(0x373A);
            sparkles.MoveToWorld(new Point3D(1050, 2940, 38), map);
        }

        public override void Delete()
        {
            base.Delete();

            Static s = Map.TerMur.FindItem<Static>(new Point3D(978, 2876, 47));
        }

        private void RegisterRegion()
        {
            Region = new Region("Crystal Lotus Puzzle Region", Map.TerMur, Region.DefaultPriority, new Rectangle2D(945, 2858, 66, 62));
            Region.Register();
        }

        public CrystalLotusPuzzle(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.WriteItemList(Tiles, true);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version

            Tiles = reader.ReadStrongItemList<PuzzleTile>();

            Instance = this;

            foreach (PuzzleTile tile in Tiles)
            {
                tile.Puzzle = this;
            }

            RegisterRegion();
            DoSequence();
        }
    }

    public class PuzzleTile : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public CrystalLotusPuzzle Puzzle { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int OriginalHue { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Group { get; private set; }

        public override bool ForceShowProperties => true;

        public PuzzleTile(CrystalLotusPuzzle puzzle, int hue, int group)
            : base(0x519)
        {
            Movable = false;
            Hue = hue;
            Group = group;

            OriginalHue = hue;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (Puzzle == null)
                return;

            if (m.InRange(GetWorldLocation(), 3))
            {
                Puzzle.OnTileClicked(m, this);
            }
            else
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public PuzzleTile(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(OriginalHue);
            writer.Write(Group);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version

            OriginalHue = reader.ReadInt();
            Group = reader.ReadInt();

            if (Hue != OriginalHue)
            {
                Hue = OriginalHue;
            }
        }
    }
}
