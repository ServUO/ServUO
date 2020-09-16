using Server.Commands;
using Server.Gumps;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ExodusNexus : NexusAddon
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenExodusNexus", AccessLevel.Administrator, Generate_ExodusNexus);
        }

        public static void Generate_ExodusNexus(CommandEventArgs e)
        {
            Decorate.Generate("exodus", "Data/Decoration/Exodus", Map.Ilshenar);
        }

        private static readonly TimeSpan m_UseTimeout = TimeSpan.FromMinutes(2.0);
        private readonly Dictionary<Mobile, DamageTimer> DamageTable = new Dictionary<Mobile, DamageTimer>();
        private int m_SideLength;
        private Node[] m_Path;
        private Mobile User { get; set; }
        private DateTime LastUse { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active => Hue == 0;

        [Constructable]
        public ExodusNexus()
            : this(Utility.RandomMinMax(3, 6))
        {
        }

        [Constructable]
        public ExodusNexus(int sideLength)
        {
            Hue = 1987;
            SideLength = sideLength;
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
                    ExodusInitPath();
                }
            }
        }
        public Node[] Path => m_Path;

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
                get { return m_X; }
                set { m_X = value; }
            }

            public int Y
            {
                get { return m_Y; }
                set { m_Y = value; }
            }
        }

        public enum PathDirection
        {
            Left,
            Up,
            Right,
            Down
        }

        public void ExodusInitPath()
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

            if (User != null)
            {
                User.CloseGump(typeof(NexusGameGump));
                User = null;
            }
        }

        public ExodusNexus(Serial serial) : base(serial)
        {
        }

        public void OpenGump(Mobile from)
        {
            if (!from.InRange(this, 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (User != null)
            {
                if (User == from)
                    return;

                if (User.Deleted || User.Map != Map || !User.InRange(this, 3) ||
                    User.NetState == null || DateTime.UtcNow - LastUse >= m_UseTimeout)
                {
                    User.CloseGump(typeof(NexusGameGump));
                }
                else
                {
                    from.SendLocalizedMessage(1152379); // Someone is currently working at the Nexus. 
                    return;
                }
            }

            User = from;
            LastUse = DateTime.UtcNow;

            from.SendGump(new NexusGameGump(this, from, 0, false));
        }

        public void DoDamage(Mobile to)
        {
            to.SendLocalizedMessage(1152372); // The Nexus shoots an arc of energy at you! 
            to.BoltEffect(0);
            to.LocalOverheadMessage(MessageType.Regular, 0x21, 1114443); // * Your body convulses from electric shock *
            to.NonlocalOverheadMessage(MessageType.Regular, 0x21, 1114443, to.Name); //  * ~1_NAME~ spasms from electric shock *

            AOS.Damage(to, to, 60, 0, 0, 0, 0, 100);

            if (!to.Alive)
                return;

            if (!DamageTable.ContainsKey(to))
            {
                DamageTimer timer = new DamageTimer(this, to);

                to.Frozen = true;
                DamageTable[to] = timer;
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

            from.SendLocalizedMessage(1152371); // You repair the mysterious Nexus! As it whirs to life it spits out a punch card! 
            from.AddToBackpack(new PunchCard());

            Hue = 0;
            Timer.DelayCall(TimeSpan.FromMinutes(10), delegate { Hue = 1987; });
        }

        public class DamageTimer : Timer
        {
            private readonly ExodusNexus m_Nexus;
            private readonly Mobile m_To;
            private int m_Step;

            public DamageTimer(ExodusNexus nexus, Mobile to)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                m_Nexus = nexus;
                m_To = to;
                m_Step = 0;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Nexus.Deleted || m_To.Deleted || !m_To.Alive)
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
                m_Nexus.DamageTable.Remove(m_To);
                m_To.Frozen = false;

                Stop();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(m_SideLength);
            writer.Write(m_Path.Length);

            for (int i = 0; i < m_Path.Length; i++)
            {
                Node cur = m_Path[i];

                writer.Write(cur.X);
                writer.Write(cur.Y);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_SideLength = reader.ReadInt();
            m_Path = new Node[reader.ReadInt()];

            for (int i = 0; i < m_Path.Length; i++)
            {
                m_Path[i] = new Node(reader.ReadInt(), reader.ReadInt());
            }

            if (Hue == 0)
                Timer.DelayCall(TimeSpan.FromMinutes(10), delegate { Hue = 1987; });
        }

        public class NexusGameGump : Gump
        {
            private readonly ExodusNexus m_Nexus;
            private readonly Mobile m_From;
            private readonly int m_Step;
            public NexusGameGump(ExodusNexus nexus, Mobile from, int step, bool hint)
                : base(5, 30)
            {
                m_Nexus = nexus;
                m_From = from;
                m_Step = step;

                int sideLength = nexus.SideLength;

                AddBackground(50, 0, 530, 410, 0xA28);

                AddImage(0, 0, 0x28C8);
                AddImage(547, 0, 0x28C9);

                AddBackground(95, 20, 442, 90, 0xA28);

                AddHtmlLocalized(165, 35, 300, 45, 1153747, false, false); // <center>GENERATOR CONTROL nexus</center>
                AddHtmlLocalized(165, 60, 300, 70, 1153748, false, false); // <center>Use the Directional Controls to</center>
                AddHtmlLocalized(165, 75, 300, 85, 1153749, false, false); // <center>Close the Grid Circuit</center>

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

                Node[] path = nexus.Path;

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
                    AddHtmlLocalized(405, 345, 140, 40, 1153750, false, false); // Attempt to Decipher the Circuit Path
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
                if (m_Nexus.Deleted || info.ButtonID == 0 || !m_From.CheckAlive())
                {
                    m_Nexus.User = null;
                    return;
                }

                if (m_From.Map != m_Nexus.Map || !m_From.InRange(m_Nexus, 3))
                {
                    m_From.SendLocalizedMessage(500446); // That is too far away.
                    m_Nexus.User = null;
                    return;
                }

                Node nextNode = m_Nexus.Path[m_Step + 1];

                if (info.ButtonID == 5) // Attempt to Decipher
                {
                    double lockpicking = m_From.Skills.Lockpicking.Value;

                    if (lockpicking < 65.0)
                        return;

                    m_From.PlaySound(0x241);

                    if (40.0 + Utility.RandomDouble() * 80.0 < lockpicking)
                    {
                        m_From.SendGump(new NexusGameGump(m_Nexus, m_From, m_Step, true));
                        m_Nexus.LastUse = DateTime.UtcNow;
                    }
                    else
                    {
                        m_Nexus.DoDamage(m_From);
                        m_Nexus.User = null;
                    }
                }
                else
                {
                    Node curNode = m_Nexus.Path[m_Step];

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
                        if (m_Step + 1 == m_Nexus.Path.Length - 1)
                        {
                            m_Nexus.Solve(m_From);
                            m_Nexus.User = null;
                        }
                        else
                        {
                            m_From.PlaySound(0x1F4);
                            m_From.SendGump(new NexusGameGump(m_Nexus, m_From, m_Step + 1, false));
                            m_Nexus.LastUse = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        m_Nexus.DoDamage(m_From);
                        m_Nexus.User = null;
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
    }
}
