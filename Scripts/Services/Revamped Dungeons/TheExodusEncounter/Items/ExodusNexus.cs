using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Commands;

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
        private Dictionary<Mobile, DamageTimer> DamageTable = new Dictionary<Mobile, DamageTimer>();
        private int m_SideLength;
        private Node[] m_Path;
        private Mobile User { get; set; }
        private DateTime LastUse { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active { get { return this.Hue == 0; } }

        [Constructable]
        public ExodusNexus()
            : this(Utility.RandomMinMax(3, 6))
        {
        }

        [Constructable]
        public ExodusNexus(int sideLength)
        {
            this.Hue = 1987;
            this.SideLength = sideLength;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SideLength
        {
            get
            {
                return this.m_SideLength;
            }
            set
            {
                if (value < 3)
                    value = 3;
                else if (value > 6)
                    value = 6;

                if (this.m_SideLength != value)
                {
                    this.m_SideLength = value;
                    this.ExodusInitPath();
                }
            }
        }
        public Node[] Path { get { return this.m_Path; } }

        public struct Node
        {
            private int m_X;
            private int m_Y;

            public Node(int x, int y)
            {
                this.m_X = x;
                this.m_Y = y;
            }

            public int X
            {
                get { return this.m_X; }
                set { this.m_X = value; }
            }

            public int Y
            {
                get { return this.m_Y; }
                set { this.m_Y = value; }
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
            int totalNodes = this.SideLength * this.SideLength;

            Node[] stack = new Node[totalNodes];
            Node current = stack[0] = new Node(0, 0);
            int stackSize = 1;

            bool[,] visited = new bool[this.SideLength, this.SideLength];
            visited[0, 0] = true;

            while (true)
            {
                PathDirection[] choices = new PathDirection[4];
                int count = 0;

                if (current.X > 0 && !visited[current.X - 1, current.Y])
                    choices[count++] = PathDirection.Left;

                if (current.Y > 0 && !visited[current.X, current.Y - 1])
                    choices[count++] = PathDirection.Up;

                if (current.X < this.SideLength - 1 && !visited[current.X + 1, current.Y])
                    choices[count++] = PathDirection.Right;

                if (current.Y < this.SideLength - 1 && !visited[current.X, current.Y + 1])
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

                    if (current.X == this.SideLength - 1 && current.Y == this.SideLength - 1)
                        break;

                    visited[current.X, current.Y] = true;
                }
                else
                {
                    current = stack[--stackSize - 1];
                }
            }

            this.m_Path = new Node[stackSize];

            for (int i = 0; i < stackSize; i++)
            {
                this.m_Path[i] = stack[i];
            }

            if (this.User != null)
            {
                this.User.CloseGump(typeof(NexusGameGump));
                this.User = null;
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

            if (this.User != null)
            {
                if (this.User == from)
                    return;

                if (this.User.Deleted || this.User.Map != this.Map || !this.User.InRange(this, 3) ||
                    this.User.NetState == null || DateTime.UtcNow - this.LastUse >= m_UseTimeout)
                {
                    this.User.CloseGump(typeof(NexusGameGump));
                }
                else
                {
                    from.SendLocalizedMessage(1152379); // Someone is currently working at the Nexus. 
                    return;
                }
            }

            this.User = from;
            this.LastUse = DateTime.UtcNow;

            from.SendGump(new NexusGameGump(this, from, 0, false));
        }

        public void DoDamage(Mobile to)
        {
            to.SendLocalizedMessage(1152372); // The Nexus shoots an arc of energy at you! 
            to.BoltEffect(0);
            to.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x21, 1114443); // * Your body convulses from electric shock *
            to.NonlocalOverheadMessage(Server.Network.MessageType.Regular, 0x21, 1114443, to.Name); //  * ~1_NAME~ spasms from electric shock *

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
            Effects.PlaySound(this.Location, this.Map, 0x211);
            Effects.PlaySound(this.Location, this.Map, 0x1F3);

            Effects.SendLocationEffect(this.Location, this.Map, 0x36B0, 4, 4);
            Effects.SendLocationEffect(new Point3D(this.X - 1, this.Y - 1, this.Z + 2), this.Map, 0x36B0, 4, 4);
            Effects.SendLocationEffect(new Point3D(this.X - 2, this.Y - 1, this.Z + 2), this.Map, 0x36B0, 4, 4);

            from.SendLocalizedMessage(1152371); // You repair the mysterious Nexus! As it whirs to life it spits out a punch card! 
            from.AddToBackpack(new PunchCard());

            this.Hue = 0;
            Timer.DelayCall(TimeSpan.FromMinutes(10), new TimerCallback(delegate { this.Hue = 1987; }));
        }

        public class DamageTimer : Timer
        {
            private readonly ExodusNexus m_Nexus;
            private readonly Mobile m_To;
            private int m_Step;

            public DamageTimer(ExodusNexus nexus, Mobile to)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                this.m_Nexus = nexus;
                this.m_To = to;
                this.m_Step = 0;

                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (this.m_Nexus.Deleted || this.m_To.Deleted || !this.m_To.Alive)
                {
                    this.End();
                    return;
                }

                this.m_To.PlaySound(0x28);

                this.m_To.LocalOverheadMessage(MessageType.Regular, 0xC9, true, "* Your body convulses from electric shock *");
                this.m_To.NonlocalOverheadMessage(MessageType.Regular, 0xC9, true, string.Format("* {0} spasms from electric shock *", this.m_To.Name));

                AOS.Damage(this.m_To, this.m_To, 20, 0, 0, 0, 0, 100);

                if (++this.m_Step >= 3 || !this.m_To.Alive)
                {
                    this.End();
                }
            }

            private void End()
            {
                m_Nexus.DamageTable.Remove(m_To);
                this.m_To.Frozen = false;

                this.Stop();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write((int)this.m_SideLength);
            writer.Write((int)this.m_Path.Length);

            for (int i = 0; i < this.m_Path.Length; i++)
            {
                Node cur = this.m_Path[i];

                writer.Write(cur.X);
                writer.Write(cur.Y);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_SideLength = reader.ReadInt();
            this.m_Path = new Node[reader.ReadInt()];

            for (int i = 0; i < this.m_Path.Length; i++)
            {
                this.m_Path[i] = new Node(reader.ReadInt(), reader.ReadInt());
            }

            if (this.Hue == 0)
                Timer.DelayCall(TimeSpan.FromMinutes(10), new TimerCallback(delegate { this.Hue = 1987; }));
        }

        public class NexusGameGump : Gump
        {
            private readonly ExodusNexus m_Nexus;
            private readonly Mobile m_From;
            private readonly int m_Step;
            public NexusGameGump(ExodusNexus nexus, Mobile from, int step, bool hint)
                : base(5, 30)
            {
                this.m_Nexus = nexus;
                this.m_From = from;
                this.m_Step = step;

                int sideLength = nexus.SideLength;

                this.AddBackground(50, 0, 530, 410, 0xA28);

                this.AddImage(0, 0, 0x28C8);
                this.AddImage(547, 0, 0x28C9);

                this.AddBackground(95, 20, 442, 90, 0xA28);
                
                this.AddHtmlLocalized(165, 35, 300, 45, 1153747, false, false); // <center>GENERATOR CONTROL nexus</center>
                this.AddHtmlLocalized(165, 60, 300, 70, 1153748, false, false); // <center>Use the Directional Controls to</center>
                this.AddHtmlLocalized(165, 75, 300, 85, 1153749, false, false); // <center>Close the Grid Circuit</center>

                this.AddImage(140, 40, 0x28D3);
                this.AddImage(420, 40, 0x28D3);

                this.AddBackground(365, 120, 178, 210, 0x1400);

                this.AddImage(365, 115, 0x28D4);
                this.AddImage(365, 288, 0x28D4);

                this.AddImage(414, 189, 0x589);
                this.AddImage(435, 210, 0xA52);

                this.AddButton(408, 222, 0x29EA, 0x29EC, 1, GumpButtonType.Reply, 0); // Left
                this.AddButton(448, 185, 0x29CC, 0x29CE, 2, GumpButtonType.Reply, 0); // Up
                this.AddButton(473, 222, 0x29D6, 0x29D8, 3, GumpButtonType.Reply, 0); // Right
                this.AddButton(448, 243, 0x29E0, 0x29E2, 4, GumpButtonType.Reply, 0); // Down

                this.AddBackground(90, 115, 30 + 40 * sideLength, 30 + 40 * sideLength, 0xA28);
                this.AddBackground(100, 125, 10 + 40 * sideLength, 10 + 40 * sideLength, 0x1400);

                for (int i = 0; i < sideLength; i++)
                {
                    for (int j = 0; j < sideLength - 1; j++)
                    {
                        this.AddImage(120 + 40 * i, 162 + 40 * j, 0x13F9);
                    }
                }

                for (int i = 0; i < sideLength - 1; i++)
                {
                    for (int j = 0; j < sideLength; j++)
                    {
                        this.AddImage(138 + 40 * i, 147 + 40 * j, 0x13FD);
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
                        this.AddNode(110 + 40 * i, 135 + 40 * j, hues[i, j]);
                    }
                }

                Node curNode = path[step];
                this.AddImage(118 + 40 * curNode.X, 143 + 40 * curNode.Y, 0x13A8);

                if (hint)
                {
                    Node nextNode = path[step + 1];
                    this.AddImage(119 + 40 * nextNode.X, 143 + 40 * nextNode.Y, 0x939);
                }

                if (from.Skills.Lockpicking.Value >= 65.0)
                {
                    this.AddButton(365, 350, 0xFA6, 0xFA7, 5, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(405, 345, 140, 40, 1153750, false, false); // Attempt to Decipher the Circuit Path
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
                if (this.m_Nexus.Deleted || info.ButtonID == 0 || !this.m_From.CheckAlive())
                {
                    this.m_Nexus.User = null;
                    return;
                }

                if (this.m_From.Map != this.m_Nexus.Map || !this.m_From.InRange(this.m_Nexus, 3))
                {
                    this.m_From.SendLocalizedMessage(500446); // That is too far away.
                    this.m_Nexus.User = null;
                    return;
                }

                Node nextNode = this.m_Nexus.Path[this.m_Step + 1];

                if (info.ButtonID == 5) // Attempt to Decipher
                {
                    double lockpicking = this.m_From.Skills.Lockpicking.Value;

                    if (lockpicking < 65.0)
                        return;

                    this.m_From.PlaySound(0x241);

                    if (40.0 + Utility.RandomDouble() * 80.0 < lockpicking)
                    {
                        this.m_From.SendGump(new NexusGameGump(this.m_Nexus, this.m_From, this.m_Step, true));
                        this.m_Nexus.LastUse = DateTime.UtcNow;
                    }
                    else
                    {
                        this.m_Nexus.DoDamage(this.m_From);
                        this.m_Nexus.User = null;
                    }
                }
                else
                {
                    Node curNode = this.m_Nexus.Path[this.m_Step];

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
                        if (this.m_Step + 1 == this.m_Nexus.Path.Length - 1)
                        {
                            this.m_Nexus.Solve(this.m_From);
                            this.m_Nexus.User = null;
                        }
                        else
                        {
                            this.m_From.PlaySound(0x1F4);
                            this.m_From.SendGump(new NexusGameGump(this.m_Nexus, this.m_From, this.m_Step + 1, false));
                            this.m_Nexus.LastUse = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        this.m_Nexus.DoDamage(this.m_From);
                        this.m_Nexus.User = null;
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

                this.AddImage(x, y, id);
            }
        }
    }
}
