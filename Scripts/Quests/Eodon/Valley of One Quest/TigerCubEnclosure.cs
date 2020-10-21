using Server.Engines.Quests;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class CubEnclosure : BaseAddon
    {
        public EnclosureDoor Door { get; set; }
        public TigerCub Cub { get; set; }

        public static readonly Point3D DoorOffset = new Point3D(2, 2, 0);
        public static readonly Point3D HomeLocation = new Point3D(354, 1894, 2);

        [Constructable]
        public CubEnclosure()
        {
            AddComponent(new AddonComponent(2103), 0, 0, 0);
            AddComponent(new AddonComponent(2103), 0, 1, 0);
            AddComponent(new AddonComponent(2103), 0, 2, 0);
            AddComponent(new AddonComponent(2102), 1, 2, 0);
            AddComponent(new AddonComponent(2101), 3, 2, 0);
            AddComponent(new AddonComponent(2103), 3, 1, 0);
            AddComponent(new AddonComponent(2103), 3, 0, 0);
            AddComponent(new AddonComponent(2102), 2, -1, 0);
            AddComponent(new AddonComponent(2102), 1, -1, 0);
            AddComponent(new AddonComponent(2102), 3, -1, 0);

            Door = new EnclosureDoor(this);
            Cub = new TigerCub
            {
                Blessed = true,
                RangeHome = 5,

                Location = new Point3D(Location.X + 1, Location.Y + 1, Location.Z)
            };
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            if (Door != null)
                Door.MoveToWorld(new Point3D(X + DoorOffset.X, Y + DoorOffset.Y, Z + DoorOffset.Z), Map);

            if (Cub != null)
            {
                Cub.MoveToWorld(new Point3D(X + 1, Y + 1, Z), Map);
                Cub.Home = Cub.Location;
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Door != null)
                Door.Map = Map;

            if (Cub != null)
                Cub.Map = Map;
        }

        public bool Contains(Point3D p)
        {
            foreach (AddonComponent c in Components)
            {
                if (c.Location == p)
                    return true;
            }

            return Door != null && Door.Location == p;
        }

        public void OnPuzzleCompleted(PlayerMobile pm)
        {
            if (Door != null)
            {
                pm.PrivateOverheadMessage(MessageType.Regular, 0x35, 1156501, pm.NetState); // *You watch as the Tiger Cub safely returns to the Kurak Tribe*

                Timer.DelayCall(TimeSpan.FromSeconds(.25), Delete);
                //1156499;*The enclosure unlocks!*  Is this used?

                if (Cub != null)
                {
                    Cub.Blessed = false;
                    Cub.Protector = pm;
                    Cub.Home = HomeLocation;
                    Cub.RangeHome = 1;
                }
            }
        }

        public void OnBadDirection(PlayerMobile pm)
        {
            if (Utility.RandomBool())
            {
                if (Door != null)
                    pm.PrivateOverheadMessage(MessageType.Regular, 0x35, 1156493, pm.NetState); //*A poisonous dart embeds itself into your neck!*

                Effects.PlaySound(pm.Location, pm.Map, 0x22E);

                pm.Damage(Utility.RandomMinMax(20, 40));
                pm.ApplyPoison(pm, Poison.Deadly);
            }
            else
            {
                if (Door != null)
                    pm.PrivateOverheadMessage(MessageType.Regular, 0x35, 1156494, pm.NetState); //*You are ambushed by attacking trappers!*

                SpawnTrappers(pm);
            }
        }

        public void SpawnTrappers(Mobile m)
        {
            if (m == null || !m.Alive)
                return;

            for (int i = 0; i < 3; i++)
            {
                Point3D p = m.Location;

                for (int j = 0; j < 25; j++)
                {
                    int x = p.X + Utility.RandomMinMax(p.X - 2, p.X + 2);
                    int y = p.Y + Utility.RandomMinMax(p.Y - 2, p.Y + 2);
                    int z = m.Map.GetAverageZ(x, y);

                    if (m.Map.CanSpawnMobile(x, y, z) && !Contains(new Point3D(x, y, z)))
                    {
                        p = new Point3D(x, y, z);
                        break;
                    }
                }

                BaseCreature bc = new Trapper();
                bc.MoveToWorld(p, m.Map);

                Timer.DelayCall(TimeSpan.FromSeconds(.25), mob => mob.Combatant = m, bc);
            }
        }

        public override void Delete()
        {
            base.Delete();

            if (Door != null)
                Door.Delete();

            if (Cub != null && Cub.Protector == null)
                Cub.Delete();
        }

        public CubEnclosure(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Door);
            writer.Write(Cub);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            Door = reader.ReadItem() as EnclosureDoor;
            Cub = reader.ReadMobile() as TigerCub;

            if (Door != null)
                Door.Enclosure = this;
        }
    }

    public class EnclosureDoor : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public CubEnclosure Enclosure { get; set; }

        public List<int> Path { get; set; }

        public EnclosureDoor(CubEnclosure enclosure) : base(2086)
        {
            Enclosure = enclosure;
            Path = GetRandomPath();

            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile && from.InRange(GetWorldLocation(), 2) && from.InLOS(this))
            {
                PrideOfTheAmbushQuest quest = QuestHelper.GetQuest((PlayerMobile)from, typeof(PrideOfTheAmbushQuest)) as PrideOfTheAmbushQuest;

                if (quest != null)
                {
                    from.SendGump(new LockingMechanismGump((PlayerMobile)from, this));
                }
            }
        }

        private readonly int[] m_Possibles =
        {
                0,   1,   2,   3,
                4,   5,   6,   7,
                8,   9,  10,  11,
                12, 13,  14,  15
        };

        private readonly int[][] _Paths =
        {
            new int[] { 0, 1, 2, 3, 7, 11, 15 },
            new int[] { 0, 4, 8, 12, 13, 9, 5, 1, 2, 6, 10, 14, 15 },
            new int[] { 0, 4, 5, 1, 2, 3, 7, 6, 10, 14, 15 },
            new int[] { 0, 4, 8, 12, 13, 14, 10, 6, 2, 3, 7, 11, 15 }, // 
            new int[] { 0, 4, 5, 9, 13, 14, 10, 11, 15 },
            new int[] { 0, 4, 5, 1, 2, 6, 10, 9, 13, 14, 15 },
            new int[] { 0, 1, 5, 4, 8, 12, 13, 14, 10, 11, 15 },
            new int[] { 0, 1, 2, 6, 5, 4, 8, 12, 13, 9, 10, 11, 15 },
            new int[] { 0, 1, 5, 6, 7, 11, 10, 9, 13, 14, 15 },
            new int[] { 0, 1, 5, 6, 7, 11, 15 },
            new int[] { 0, 1, 5, 6, 10, 9, 13, 14, 15 },
            new int[] { 0, 1, 2, 3, 7, 6, 5, 4, 8, 9, 1, 11, 12, 13, 14, 15 },
            new int[] { 0, 4, 8, 12, 13, 14, 15 },
            new int[] { 0, 4, 5, 9, 8, 12, 13, 14, 10, 6, 2, 3, 7, 11, 15 }
        };

        public List<int> GetRandomPath()
        {
            return new List<int>(_Paths[Utility.Random(_Paths.Length)]);
        }

        public class LockingMechanismGump : Gump
        {
            public EnclosureDoor Door { get; set; }
            public bool ShowNext { get; set; }
            public PlayerMobile User { get; set; }

            public List<int> Path => Door != null && !Door.Deleted ? Door.Path : null;
            public List<int> Progress { get; set; }

            public LockingMechanismGump(PlayerMobile pm, EnclosureDoor door)
                : base(25, 25)
            {
                Door = door;
                User = pm;

                Progress = new List<int>();
                Progress.Add(0);

                AddGumpLayout();
            }

            public void Refresh()
            {
                Entries.Clear();
                Entries.TrimExcess();
                AddGumpLayout();

                User.CloseGump(GetType());
                User.SendGump(this, false);
            }

            public void AddGumpLayout()
            {
                AddBackground(50, 50, 550, 440, 2600);
                AddBackground(100, 75, 450, 90, 2600);
                AddBackground(90, 175, 270, 270, 2600);
                AddBackground(100, 185, 250, 250, 5120);
                AddBackground(370, 175, 178, 200, 5120);

                AddImage(145, 95, 10451);
                AddImage(435, 95, 10451);
                AddImage(0, 50, 10400);
                AddImage(0, 200, 10401);
                AddImage(0, 360, 10402);

                AddImage(565, 50, 10410);
                AddImage(565, 200, 10411);
                AddImage(565, 360, 10412);

                AddImage(370, 175, 10452);
                AddImage(370, 360, 10452);

                AddImageTiled(125, 207, 130, 3, 5031);
                AddImageTiled(125, 247, 130, 3, 5031);
                AddImageTiled(125, 287, 130, 3, 5031);
                AddImageTiled(125, 327, 130, 3, 5031);

                AddImageTiled(123, 205, 3, 130, 5031);
                AddImageTiled(163, 205, 3, 130, 5031);
                AddImageTiled(203, 205, 3, 130, 5031);
                AddImageTiled(243, 205, 3, 130, 5031);

                AddImage(420, 250, 1417);
                AddImage(440, 270, 2642);

                AddHtmlLocalized(220, 90, 210, 16, 1156490, false, false); // <center>Enclosure Locking Mechanism</center>
                AddHtmlLocalized(220, 112, 210, 16, 1153748, false, false); // <center>Use the Directional Controls to</center>
                AddHtmlLocalized(220, 131, 210, 16, 1156491, false, false); // <center>Unlock the Enclosure</center>

                int x = 110;
                int y = 195;
                int xOffset = 0;
                int yOffset = 0;

                for (int i = 0; i < 16; i++)
                {
                    int itemID = Progress.Contains(i) ? 2152 : 9720;

                    if (i < 4)
                    {
                        xOffset = x + (40 * i);
                        yOffset = y;
                    }

                    else if (i < 8)
                    {
                        xOffset = x + (40 * (i - 4));
                        yOffset = y + 40;
                    }
                    else if (i < 12)
                    {
                        xOffset = x + (40 * (i - 8));
                        yOffset = y + 80;
                    }
                    else if (i < 16)
                    {
                        xOffset = x + (40 * (i - 12));
                        yOffset = y + 120;
                    }

                    AddImage(xOffset, yOffset, itemID);

                    if (i == Progress[Progress.Count - 1])
                        AddImage(xOffset + 8, yOffset + 8, 5032);

                    if (ShowNext && Progress.Count <= Path.Count && i == Path[Progress.Count])
                        AddImage(xOffset + 8, yOffset + 8, 2361);
                }

                ShowNext = false;

                if (User.Skills[SkillName.Lockpicking].Base >= 100)
                {
                    AddHtmlLocalized(410, 415, 150, 32, 1156492, false, false); // Attempt to pick the enclosure lock
                    AddButton(370, 415, 4005, 4005, 5, GumpButtonType.Reply, 0);
                }

                AddButton(453, 245, 10700, 10701, 1, GumpButtonType.Reply, 0); // up

                AddButton(478, 281, 10710, 10711, 2, GumpButtonType.Reply, 0); // right

                AddButton(453, 305, 10720, 10721, 3, GumpButtonType.Reply, 0); // down 

                AddButton(413, 281, 10730, 10731, 4, GumpButtonType.Reply, 0); // left
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (info.ButtonID > 0 && info.ButtonID <= 5)
                    HandleButton(info.ButtonID);
            }

            public void HandleButton(int id)
            {
                if (Door == null || Door.Deleted)
                    return;

                if (id > 0 && id < 5)
                {
                    int current = Progress[Progress.Count - 1];
                    int next = 15;
                    int pick;

                    if (Progress.Count >= 0 && Progress.Count < Path.Count)
                        next = Path[Progress.Count];

                    switch (id)
                    {
                        default:
                        case 1: pick = current - 4; break;
                        case 2: pick = current + 1; break;
                        case 3: pick = current + 4; break;
                        case 4: pick = current - 1; break;
                    }

                    if (Progress.Contains(pick) || pick < 0 || pick > 15)      //Off board or already chosen spot
                    {
                        User.PlaySound(0x5B6);
                        Refresh();
                    }
                    else if ((current == 10 || current == 11 || current == 14) && pick == 15)
                    {
                        User.PlaySound(0x3D);

                        if (Door.Enclosure != null)
                            Door.Enclosure.OnPuzzleCompleted(User);
                    }
                    else if (pick == next)                                          //Found next 
                    {
                        Progress.Add(pick);

                        User.PlaySound(0x1F5);
                        Refresh();
                    }
                    else // Wrong Mutha Fucka!
                    {
                        if (Door.Enclosure != null)
                            Door.Enclosure.OnBadDirection(User);

                        ClearProgress();
                    }
                }
                else if (id == 5)
                {
                    ShowNext = true;
                    Refresh();
                }
            }

            public void ClearProgress()
            {
                Progress.Clear();
                Progress.Add(0);
            }
        }

        public EnclosureDoor(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            Path = GetRandomPath();
        }
    }
}
