using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.JollyRoger
{
    public class BoxArray
    {
        public Mobile Mobile { get; set; }
        public bool Reward { get; set; }

        public BoxArray(Mobile m, bool r)
        {
            Mobile = m;
            Reward = r;
        }
    }

    public class SherryStrongBox : Container
    {
        private static List<BoxArray> Permission = new List<BoxArray>();
        private static string FilePath = Path.Combine("Saves/Misc", "SherryStrongBox.bin");

        [Constructable]
        public SherryStrongBox()
            : base(0xE80)
        {
            Weight = 0.0;
            Movable = false;
        }

        public override bool DisplaysContent => false;

        public static void AddPermission(Mobile from)
        {
            if (!Permission.Any(x => x.Mobile == from))
            {
                Permission.Add(new BoxArray(from, false));
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 2))
            {
                if (Permission != null)
                {
                    var p = Permission.FirstOrDefault(x => x.Mobile == from);

                    if (p != null)
                    {
                        if (!p.Reward)
                        {
                            Item item = new SheetMusicForStones();
                            from.AddToBackpack(item);
                            from.SendLocalizedMessage(1152339, "#1159343"); // A reward of ~1_ITEM~ has been placed in your backpack.
                            Permission.FirstOrDefault(x => x.Mobile == from).Reward = true;
                        }
                        else
                        {
                            base.OnDoubleClick(from);
                        }
                    }
                    else
                    {
                        PrivateOverheadMessage(MessageType.Regular, 0x47E, 500648,
                            from.NetState); // This chest seems to be locked.
                    }
                }
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    writer.Write(Permission.Count);

                    Permission.ForEach(s =>
                    {
                        writer.Write(s.Mobile);
                        writer.Write(s.Reward);
                    });
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();
                    int count = reader.ReadInt();

                    for (int i = count; i > 0; i--)
                    {
                        Mobile m = reader.ReadMobile();
                        bool r = reader.ReadBool();

                        if (m != null)
                        {
                            Permission.Add(new BoxArray(m, r));
                        }
                    }
                });
        }

        public SherryStrongBox(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SherryLute : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public string Note { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Sound { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SherryTheMouse Controller { get; set; }

        [Constructable]
        public SherryLute()
            : base(0xEB3)
        {
            Movable = false;
            Weight = 0.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (Controller != null)
            {
                PrivateOverheadMessage(MessageType.Regular, 0x47E, 1159341, from.NetState, Note); // *You strum the lute, it is tuned to ~1_NOTE~*

                if (Controller._List.ContainsKey(from))
                {
                    if (Controller._List.Any(x => x.Key == from && x.Value.Contains(Note)))
                    {
                        from.PlaySound(0x4C);
                        Controller._List.Remove(from);
                    }
                    else
                    {
                        var temp = Controller._List[from].ToList();
                        temp.Add(Note);

                        bool correct = false;

                        int i;

                        for (i = 0; i < temp.Count; i++)
                        {
                            if (temp[i] == Controller.Notes[i].Note)
                            {
                                correct = true;
                            }
                            else
                            {
                                correct = false;
                            }
                        }

                        if (correct)
                        {
                            Controller._List[from] = temp.ToArray();
                            from.PlaySound(Sound);
                        }
                        else
                        {
                            Controller._List.Remove(from);
                            from.PlaySound(0x4D);
                        }

                        if (i == 8)
                        {
                            from.PlaySound(511);
                            from.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1159342, from.NetState); // *You hear a click as the chest in the corner unlocks!*
                            from.PlaySound(1048);
                            SherryStrongBox.AddPermission(from);
                            Controller._List.Remove(from);
                            Controller.ChangeNotes();
                        }
                    }
                }
                else if (Controller.Notes[0].Note == Note)
                {
                    Controller._List.Add(from, new[] { Note });
                    from.PlaySound(Sound);
                }
                else
                {
                    from.PlaySound(0x4D);
                }
            }
        }

        public SherryLute(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(Note);
            writer.Write(Controller);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Note = reader.ReadString();
            Controller = reader.ReadMobile() as SherryTheMouse;
        }
    }

    public class NoteArray
    {
        public string Note { get; set; }
        public int Sound { get; set; }

        public NoteArray(string n, int s)
        {
            Note = n;
            Sound = s;
        }
    }

    public class SherryTheMouse : BaseQuester
    {
        public static SherryTheMouse InstanceTram { get; set; }
        public static SherryTheMouse InstanceFel { get; set; }

        public SherryStrongBox Box { get; set; }
        private List<Item> LuteList;
        public Dictionary<Mobile, string[]> _List = new Dictionary<Mobile, string[]>();
        private List<NoteArray> NoteList;

        [Constructable]
        public SherryTheMouse()
            : base("the Mouse")
        {
            LuteList = new List<Item>();

            NoteList = RandomNotes();

            Timer.DelayCall(TimeSpan.FromSeconds(5), () =>
            {
                SherryStrongBox b = new SherryStrongBox();
                Box = b;
                b.MoveToWorld(new Point3D(1347, 1642, 80), Map);

                for (int i = 0; i < LuteLocations.Length; i++)
                {
                    Point3D p = LuteLocations[i];

                    SherryLute sl = new SherryLute();
                    LuteList.Add(sl);
                    sl.Note = NoteList[i].Note;
                    sl.Sound = NoteList[i].Sound;
                    sl.Controller = this;

                    sl.MoveToWorld(p, Map);
                }
            });
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = "Sherry";

            Body = 0xEE;
        }

        public override void OnDelete()
        {
            if (Box != null)
            {
                Box.Delete();
            }

            if (LuteList != null)
            {
                LuteList.ForEach(f => f.Delete());
                LuteList.Clear();
            }            

            base.OnDelete();
        }

        public void ChangeNotes()
        {
            NoteList = RandomNotes();

            if (LuteList != null)
            {
                for (int i = 0; i < LuteList.Count; i++)
                {
                    if (LuteList[i] != null)
                    {
                        ((SherryLute)LuteList[i]).Note = NoteList[i].Note;
                        ((SherryLute)LuteList[i]).Sound = NoteList[i].Sound;
                    }
                }
            }
        }

        public List<NoteArray> RandomNotes()
        {
            return Notes.Select(x => new { n = x, rand = Utility.Random(Notes.Count) }).OrderBy(x => x.rand).Select(x => x.n).ToList();
        }

        public List<NoteArray> Notes = new List<NoteArray>()
        {
            new NoteArray( "C4", 0x404 ),
            new NoteArray( "D", 0x409 ),
            new NoteArray( "E", 0x40E ),
            new NoteArray( "F", 0x410 ),
            new NoteArray( "G", 0x414 ),
            new NoteArray( "A", 0x3FD ),
            new NoteArray( "B", 0x401 ),
            new NoteArray( "C5", 0x405 )
        };

        public readonly static Point3D[] LuteLocations = new Point3D[]
        {
            new Point3D(1350, 1646, 80), new Point3D(1350, 1650, 80), new Point3D(1350, 1655, 80), new Point3D(1350, 1659, 80),
            new Point3D(1355, 1646, 80), new Point3D(1355, 1650, 80), new Point3D(1355, 1655, 80), new Point3D(1355, 1659, 80)
        };

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 3))
            {
                if (FellowshipMedallion.IsDressed(from))
                {
                    Gump g = new Gump(100, 100);
                    g.AddBackground(0, 0, 570, 295, 0x2454);
                    g.AddImage(0, 0, 0x9CD6);
                    g.AddHtmlLocalized(335, 24, 223, 261, 1159381, 0xC63, false, true); // *squeak squeak* Have you come to play the instruments? *squeak squeak* I bet you are as good as Iolo! “Practice! Practice!” he would say, “Practice your musical scales!” *squeak squeak* My fingers were too tiny for the lute, but I could sing! Do Re Mi Fa So La Ti Do! *squeak squeak*  <br><br>Long ago, I was supposed to perform at the wedding of Shamino and Princess Beatrix. Ready to go at noon on the nose I would have been, but that was before...everything. *frowns* Sometimes I still visit Castle Sallé Dacil through a secret door in Ilshenar. Even though the castle has seen better days, I am still reminded of the Pure Love Princess Beatrix had for Shamino! *squeak squeak*

                    from.SendGump(g);
                    from.PlaySound(1664);
                }
                else
                {
                    PrivateOverheadMessage(MessageType.Regular, 0x47E, 1159380,
                        from.NetState); // * You attempt to understand the spirit but your connection to them is weak... *
                }
            }
        }

        public SherryTheMouse(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Box);

            if (LuteList != null)
            {
                writer.Write(LuteList.Count);

                LuteList.ForEach(x => writer.Write(x));
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            LuteList = new List<Item>();

            Box = (SherryStrongBox)reader.ReadItem();

            int lutecount = reader.ReadInt();

            for (int x = 0; x < lutecount; x++)
            {
                var l = reader.ReadItem();

                if (l != null)
                    LuteList.Add(l);
            }

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }

            Timer.DelayCall(TimeSpan.FromSeconds(10), () => { ChangeNotes(); });
        }
    }
}
