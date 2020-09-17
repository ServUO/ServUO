using Server.Gumps;
using Server.Network;
using System;
using System.Collections;

namespace Server.Items
{
    public class PowerGenerator : BaseAddon
    {
        [Constructable]
        public PowerGenerator()
            : this(Utility.RandomMinMax(3, 6))
        {
        }

        [Constructable]
        public PowerGenerator(int sideLength)
        {
            AddGeneratorComponent(0x73, 0, 0, 0);
            AddGeneratorComponent(0x76, -1, 0, 0);
            AddGeneratorComponent(0x75, 0, -1, 0);
            AddGeneratorComponent(0x37F4, 0, 0, 13);

            AddComponent(new ControlPanel(sideLength), 1, 0, -2);
        }

        public PowerGenerator(Serial serial)
            : base(serial)
        {
        }

        public override bool ShareHue => false;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        private void AddGeneratorComponent(int itemID, int x, int y, int z)
        {
            AddonComponent component = new AddonComponent(itemID)
            {
                Name = "a power generator",
                Hue = 0x451
            };

            AddComponent(component, x, y, z);
        }
    }

    public class ControlPanel : AddonComponent
    {
        private static readonly TimeSpan m_UseTimeout = TimeSpan.FromMinutes(2.0);
        private readonly Hashtable m_DamageTable = new Hashtable();
        private int m_SideLength;
        private Node[] m_Path;
        private Mobile m_User;
        private DateTime m_LastUse;
        public ControlPanel(int sideLength)
            : base(0xBDC)
        {
            Hue = 0x835;

            SideLength = sideLength;
        }

        public ControlPanel(Serial serial)
            : base(serial)
        {
        }

        private enum PathDirection
        {
            Left,
            Up,
            Right,
            Down
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int SideLength
        {
            get
            {
                return m_SideLength;
            }
            set
            {
                if (value < 3)
                    value = 3;
                else if (value > 6)
                    value = 6;

                if (m_SideLength != value)
                {
                    m_SideLength = value;
                    InitPath();
                }
            }
        }
        public Node[] Path => m_Path;
        public override string DefaultName => "a control panel";
        public void InitPath()
        {
            // Depth-First Search algorithm
            int totalNodes = SideLength * SideLength;

            Node[] stack = new Node[totalNodes];
            Node current = stack[0] = new Node(0, 0);
            int stackSize = 1;

            bool[,] visited = new bool[SideLength, SideLength];
            visited[0, 0] = true;

            while (true)
            {
                PathDirection[] choices = new PathDirection[4];
                int count = 0;

                if (current.X > 0 && !visited[current.X - 1, current.Y])
                    choices[count++] = PathDirection.Left;

                if (current.Y > 0 && !visited[current.X, current.Y - 1])
                    choices[count++] = PathDirection.Up;

                if (current.X < SideLength - 1 && !visited[current.X + 1, current.Y])
                    choices[count++] = PathDirection.Right;

                if (current.Y < SideLength - 1 && !visited[current.X, current.Y + 1])
                    choices[count++] = PathDirection.Down;

                if (count > 0)
                {
                    PathDirection dir = choices[Utility.Random(count)];

                    switch (dir)
                    {
                        case PathDirection.Left:
                            current = new Node(current.X - 1, current.Y);
                            break;
                        case PathDirection.Up:
                            current = new Node(current.X, current.Y - 1);
                            break;
                        case PathDirection.Right:
                            current = new Node(current.X + 1, current.Y);
                            break;
                        default:
                            current = new Node(current.X, current.Y + 1);
                            break;
                    }

                    stack[stackSize++] = current;

                    if (current.X == SideLength - 1 && current.Y == SideLength - 1)
                        break;

                    visited[current.X, current.Y] = true;
                }
                else
                {
                    current = stack[--stackSize - 1];
                }
            }

            m_Path = new Node[stackSize];

            for (int i = 0; i < stackSize; i++)
            {
                m_Path[i] = stack[i];
            }

            if (m_User != null)
            {
                m_User.CloseGump(typeof(GameGump));
                m_User = null;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (m_User != null)
            {
                if (m_User == from)
                    return;

                if (m_User.Deleted || m_User.Map != Map || !m_User.InRange(this, 3) ||
                    m_User.NetState == null || DateTime.UtcNow - m_LastUse >= m_UseTimeout)
                {
                    m_User.CloseGump(typeof(GameGump));
                }
                else
                {
                    from.SendMessage("Someone is currently using the control panel.");
                    return;
                }
            }

            m_User = from;
            m_LastUse = DateTime.UtcNow;

            from.SendGump(new GameGump(this, from, 0, false));
        }

        public void DoDamage(Mobile to)
        {
            to.Send(new UnicodeMessage(Serial, ItemID, MessageType.Regular, 0x3B2, 3, "", "", "The generator shoots an arc of electricity at you!"));
            to.BoltEffect(0);
            to.LocalOverheadMessage(MessageType.Regular, 0xC9, true, "* Your body convulses from electric shock *");
            to.NonlocalOverheadMessage(MessageType.Regular, 0xC9, true, string.Format("* {0} spasms from electric shock *", to.Name));

            AOS.Damage(to, to, 60, 0, 0, 0, 0, 100);

            if (!to.Alive)
                return;

            if (m_DamageTable[to] == null)
            {
                to.Frozen = true;

                DamageTimer timer = new DamageTimer(this, to);
                m_DamageTable[to] = timer;

                timer.Start();
            }
        }

        public void Solve(Mobile from)
        {
            Effects.PlaySound(Location, Map, 0x211);
            Effects.PlaySound(Location, Map, 0x1F3);

            Effects.SendLocationEffect(Location, Map, 0x36B0, 4, 4);
            Effects.SendLocationEffect(new Point3D(X - 1, Y - 1, Z + 2), Map, 0x36B0, 4, 4);
            Effects.SendLocationEffect(new Point3D(X - 2, Y - 1, Z + 2), Map, 0x36B0, 4, 4);

            from.SendMessage("You scrounge some gems from the wreckage.");

            for (int i = 0; i < SideLength; i++)
            {
                from.AddToBackpack(new ArcaneGem());
            }

            from.AddToBackpack(new Diamond(SideLength));

            Item ore = new ShadowIronOre(9);
            ore.MoveToWorld(new Point3D(X - 1, Y, Z + 2), Map);

            ore = new ShadowIronOre(14);
            ore.MoveToWorld(new Point3D(X - 2, Y - 1, Z + 2), Map);

            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteEncodedInt(m_SideLength);

            writer.WriteEncodedInt(m_Path.Length);
            for (int i = 0; i < m_Path.Length; i++)
            {
                Node cur = m_Path[i];

                writer.WriteEncodedInt(cur.X);
                writer.WriteEncodedInt(cur.Y);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_SideLength = reader.ReadEncodedInt();

            m_Path = new Node[reader.ReadEncodedInt()];
            for (int i = 0; i < m_Path.Length; i++)
            {
                m_Path[i] = new Node(reader.ReadEncodedInt(), reader.ReadEncodedInt());
            }
        }

        public struct Node
        {
            private int m_X;
            private int m_Y;
            public Node(int x, int y)
            {
                m_X = x;
                m_Y = y;
            }

            public int X
            {
                get
                {
                    return m_X;
                }
                set
                {
                    m_X = value;
                }
            }
            public int Y
            {
                get
                {
                    return m_Y;
                }
                set
                {
                    m_Y = value;
                }
            }
        }

        private class GameGump : Gump
        {
            private readonly ControlPanel m_Panel;
            private readonly Mobile m_From;
            private readonly int m_Step;
            public GameGump(ControlPanel panel, Mobile from, int step, bool hint)
                : base(5, 30)
            {
                m_Panel = panel;
                m_From = from;
                m_Step = step;

                int sideLength = panel.SideLength;

                AddBackground(50, 0, 530, 410, 0xA28);

                AddImage(0, 0, 0x28C8);
                AddImage(547, 0, 0x28C9);

                AddBackground(95, 20, 442, 90, 0xA28);

                AddHtml(229, 35, 300, 45, "GENERATOR CONTROL PANEL", false, false);

                AddHtml(223, 60, 300, 70, "Use the Directional Controls to", false, false);
                AddHtml(253, 75, 300, 85, "Close the Grid Circuit", false, false);

                AddImage(140, 40, 0x28D3);
                AddImage(420, 40, 0x28D3);

                AddBackground(365, 120, 178, 210, 0x1400);

                AddImage(365, 115, 0x28D4);
                AddImage(365, 288, 0x28D4);

                AddImage(414, 189, 0x589);
                AddImage(435, 210, 0xA52);

                AddButton(408, 222, 0x29EA, 0x29EC, 1, GumpButtonType.Reply, 0); // Left
                AddButton(448, 185, 0x29CC, 0x29CE, 2, GumpButtonType.Reply, 0); // Up
                AddButton(473, 222, 0x29D6, 0x29D8, 3, GumpButtonType.Reply, 0); // Right
                AddButton(448, 243, 0x29E0, 0x29E2, 4, GumpButtonType.Reply, 0); // Down

                AddBackground(90, 115, 30 + 40 * sideLength, 30 + 40 * sideLength, 0xA28);
                AddBackground(100, 125, 10 + 40 * sideLength, 10 + 40 * sideLength, 0x1400);

                for (int i = 0; i < sideLength; i++)
                {
                    for (int j = 0; j < sideLength - 1; j++)
                    {
                        AddImage(120 + 40 * i, 162 + 40 * j, 0x13F9);
                    }
                }

                for (int i = 0; i < sideLength - 1; i++)
                {
                    for (int j = 0; j < sideLength; j++)
                    {
                        AddImage(138 + 40 * i, 147 + 40 * j, 0x13FD);
                    }
                }

                Node[] path = panel.Path;

                NodeHue[,] hues = new NodeHue[sideLength, sideLength];

                for (int i = 0; i <= step; i++)
                {
                    Node n = path[i];
                    hues[n.X, n.Y] = NodeHue.Blue;
                }

                Node lastNode = path[path.Length - 1];
                hues[lastNode.X, lastNode.Y] = NodeHue.Red;

                for (int i = 0; i < sideLength; i++)
                {
                    for (int j = 0; j < sideLength; j++)
                    {
                        AddNode(110 + 40 * i, 135 + 40 * j, hues[i, j]);
                    }
                }

                Node curNode = path[step];
                AddImage(118 + 40 * curNode.X, 143 + 40 * curNode.Y, 0x13A8);

                if (hint)
                {
                    Node nextNode = path[step + 1];
                    AddImage(119 + 40 * nextNode.X, 143 + 40 * nextNode.Y, 0x939);
                }

                if (from.Skills.Lockpicking.Value >= 65.0)
                {
                    AddButton(365, 350, 0xFA6, 0xFA7, 5, GumpButtonType.Reply, 0);
                    AddHtml(405, 345, 140, 40, "Attempt to Decipher the Circuit Path", false, false);
                }
            }

            private enum NodeHue
            {
                Gray,
                Blue,
                Red
            }
            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Panel.Deleted || info.ButtonID == 0 || !m_From.CheckAlive())
                {
                    m_Panel.m_User = null;
                    return;
                }

                if (m_From.Map != m_Panel.Map || !m_From.InRange(m_Panel, 3))
                {
                    m_From.SendLocalizedMessage(500446); // That is too far away.
                    m_Panel.m_User = null;
                    return;
                }

                Node nextNode = m_Panel.Path[m_Step + 1];

                if (info.ButtonID == 5) // Attempt to Decipher
                {
                    double lockpicking = m_From.Skills.Lockpicking.Value;

                    if (lockpicking < 65.0)
                        return;

                    m_From.PlaySound(0x241);

                    if (40.0 + Utility.RandomDouble() * 80.0 < lockpicking)
                    {
                        m_From.SendGump(new GameGump(m_Panel, m_From, m_Step, true));
                        m_Panel.m_LastUse = DateTime.UtcNow;
                    }
                    else
                    {
                        m_Panel.DoDamage(m_From);
                        m_Panel.m_User = null;
                    }
                }
                else
                {
                    Node curNode = m_Panel.Path[m_Step];

                    int newX, newY;
                    switch (info.ButtonID)
                    {
                        case 1: // Left
                            newX = curNode.X - 1;
                            newY = curNode.Y;
                            break;
                        case 2: // Up
                            newX = curNode.X;
                            newY = curNode.Y - 1;
                            break;
                        case 3: // Right
                            newX = curNode.X + 1;
                            newY = curNode.Y;
                            break;
                        case 4: // Down
                            newX = curNode.X;
                            newY = curNode.Y + 1;
                            break;
                        default:
                            return;
                    }

                    if (nextNode.X == newX && nextNode.Y == newY)
                    {
                        if (m_Step + 1 == m_Panel.Path.Length - 1)
                        {
                            m_Panel.Solve(m_From);
                            m_Panel.m_User = null;
                        }
                        else
                        {
                            m_From.PlaySound(0x1F4);
                            m_From.SendGump(new GameGump(m_Panel, m_From, m_Step + 1, false));
                            m_Panel.m_LastUse = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        m_Panel.DoDamage(m_From);
                        m_Panel.m_User = null;
                    }
                }
            }

            private void AddNode(int x, int y, NodeHue hue)
            {
                int id;
                switch (hue)
                {
                    case NodeHue.Gray:
                        id = 0x25F8;
                        break;
                    case NodeHue.Blue:
                        id = 0x868;
                        break;
                    default:
                        id = 0x9A8;
                        break;
                }

                AddImage(x, y, id);
            }
        }

        private class DamageTimer : Timer
        {
            private readonly ControlPanel m_Panel;
            private readonly Mobile m_To;
            private int m_Step;
            public DamageTimer(ControlPanel panel, Mobile to)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                m_Panel = panel;
                m_To = to;
                m_Step = 0;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Panel.Deleted || m_To.Deleted || !m_To.Alive)
                {
                    End();
                    return;
                }

                m_To.PlaySound(0x28);

                m_To.LocalOverheadMessage(MessageType.Regular, 0xC9, true, "* Your body convulses from electric shock *");
                m_To.NonlocalOverheadMessage(MessageType.Regular, 0xC9, true, string.Format("* {0} spasms from electric shock *", m_To.Name));

                AOS.Damage(m_To, m_To, 20, 0, 0, 0, 0, 100);

                if (++m_Step >= 3 || !m_To.Alive)
                {
                    End();
                }
            }

            private void End()
            {
                m_Panel.m_DamageTable.Remove(m_To);
                m_To.Frozen = false;

                Stop();
            }
        }
    }
}