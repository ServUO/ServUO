using System;
using System.Collections.Generic;
using System.IO;
using Server.Commands;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Vertex = Server.Mobiles.GuideHelper.GuideVertex;

namespace Server.Mobiles
{
    public static class GuideHelper
    {
        private static readonly Dictionary<string, List<GuideVertex>> m_GraphDefinitions = new Dictionary<string, List<GuideVertex>>();
        private static readonly List<int> m_ShopDefinitions = new List<int>();
        private static readonly char[] m_Separators = new char[] { '\t', ' ' };
        private static readonly string m_Delimiter = "--------------------------------------------------------------------------";
        public static void LogMessage(string line)
        {
            try
            {
                using (FileStream stream = new FileStream("Guide.log", FileMode.Append))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(line);
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static GuideVertex FindVertex(List<GuideVertex> list, int id)
        {
            foreach (GuideVertex v in list)
            {
                if (v.ID == id)
                    return v;
            }

            return null;
        }

        public static int FindShopName(int id)
        {
            if (id < 0 || id > m_ShopDefinitions.Count)
                return -1;

            return m_ShopDefinitions[id];
        }

        public static void Initialize()
        {
            CommandSystem.Register("GuideEdit", AccessLevel.GameMaster, new CommandEventHandler(VertexEdit_OnCommand));

            try
            {
                using (FileStream stream = File.OpenRead("Data\\Guide\\Definitions.cfg"))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();

                            if (!String.IsNullOrEmpty(line) && !line.StartsWith("#"))
                            {
                                string[] split = line.Split(m_Separators, StringSplitOptions.RemoveEmptyEntries);

                                if (split != null && split.Length > 1)
                                    m_ShopDefinitions.Add(Int32.Parse(split[1]));
                            }
                        }
                    }

                foreach (string file in Directory.GetFiles("Data\\Guide\\", "*.graph"))
                {
                    using (FileStream stream = File.OpenRead(file))
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            List<GuideVertex> list = new List<GuideVertex>();
                            GuideVertex current = null;
                            GuideVertex neighbour = null;

                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();

                                if (!String.IsNullOrEmpty(line))
                                {
                                    string[] split = line.Split(m_Separators, StringSplitOptions.RemoveEmptyEntries);
                                    int num;

                                    if (line.StartsWith("N:"))
                                    {
                                        if (current != null)
                                        {
                                            for (int i = 1; i < split.Length; i++)
                                            {
                                                num = Int32.Parse(split[i]);
                                                neighbour = FindVertex(list, num);

                                                if (neighbour == null)
                                                {
                                                    neighbour = new GuideVertex(num);
                                                    list.Add(neighbour);
                                                }

                                                current.Vertices.Add(neighbour);
                                            }
                                        }
                                    }
                                    else if (line.StartsWith("S:"))
                                    {
                                        if (current != null)
                                        {
                                            for (int i = 1; i < split.Length; i++)
                                            {
                                                num = Int32.Parse(split[i]);

                                                if (num >= 0 && num < m_ShopDefinitions.Count)
                                                    current.Shops.Add(num);
                                                else
                                                    throw new Exception(String.Format("Invalid shop ID: {0}", num));
                                            }
                                        }
                                    }
                                    else if (line.StartsWith("V:"))
                                    {
                                        if (split.Length > 5)
                                        {
                                            num = Int32.Parse(split[1]);
                                            neighbour = FindVertex(list, num);

                                            if (neighbour != null)
                                                current = neighbour;
                                            else
                                            {
                                                current = new GuideVertex(num);
                                                list.Add(current);
                                            }

                                            Point3D location = new Point3D();
                                            location.X = Int32.Parse(split[2]);
                                            location.Y = Int32.Parse(split[3]);
                                            location.Z = Int32.Parse(split[4]);
                                            current.Location = location;
                                            current.Teleporter = Boolean.Parse(split[5]);
                                        }
                                        else
                                            throw new Exception(String.Format("Incomplete vertex definition!"));
                                    }
                                }
                            }

                            m_GraphDefinitions.Add(Path.GetFileNameWithoutExtension(file), list);
                        }
                }
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
                LogMessage(ex.StackTrace);
                LogMessage(m_Delimiter);
            }
        }

        public static void VertexEdit_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            if (m.Region != null)
            {
                GuideVertex closest = ClosestVetrex(m.Region.Name, m.Location);

                if (closest != null)
                    m.SendGump(new GuideVertexEditGump(closest, m.Map, m.Region.Name));
                else
                    m.SendLocalizedMessage(1076113); // There are no shops nearby.  Please try again when you get to a town or city.
            }
            else
                m.SendLocalizedMessage(1076113); // There are no shops nearby.  Please try again when you get to a town or city.
        }

        public static GuideVertex ClosestVetrex(string town, Point3D location)
        {
            if (town == null || !m_GraphDefinitions.ContainsKey(town))
                return null;

            List<GuideVertex> vertices = m_GraphDefinitions[town];

            GuideVertex closest = null;
            double min = Int32.MaxValue;
            double distance;

            foreach (GuideVertex v in vertices)
            {
                distance = Math.Sqrt(Math.Pow(location.X - v.Location.X, 2) + Math.Pow(location.Y - v.Location.Y, 2));

                if (distance < min)
                {
                    closest = v;
                    min = distance;
                }
            }

            return closest;
        }

        public static Dictionary<int, GuideVertex> FindShops(string town, Point3D location)
        {
            if (town == null || !m_GraphDefinitions.ContainsKey(town))
                return null;
			
            List<GuideVertex> vertices = m_GraphDefinitions[town];
            Dictionary<int, GuideVertex> shops = new Dictionary<int, GuideVertex>();

            foreach (GuideVertex v in vertices)
            {
                foreach (int shop in v.Shops)
                {
                    if (shops.ContainsKey(shop))
                    {
                        GuideVertex d = shops[shop];

                        if (v.DistanceTo(location) < d.DistanceTo(location))
                            shops[shop] = v;
                    }
                    else
                        shops.Add(shop, v);
                }
            }

            if (shops.Count > 0)
                return shops;

            return null;
        }

        public static List<GuideVertex> Dijkstra(string town, GuideVertex source, GuideVertex destination)
        {
            if (town == null || !m_GraphDefinitions.ContainsKey(town))
                return null;

            Heap<GuideVertex> heap = new Heap<GuideVertex>();
            List<GuideVertex> path = new List<GuideVertex>();
            heap.Push(source);

            foreach (GuideVertex v in m_GraphDefinitions[town])
            {
                v.Distance = Int32.MaxValue;
                v.Previous = null;
                v.Visited = false;
                v.Removed = false;
            }
			
            source.Distance = 0;
            GuideVertex from ;
            int dist = 0;

            while (heap.Count > 0)
            {
                from = heap.Pop();
                from.Removed = true;

                if (from == destination)
                {
                    while (from != source)
                    {
                        path.Add(from);
                        from = from.Previous;
                    }

                    path.Add(source);
                    return path;
                }

                foreach (GuideVertex v in from.Vertices)
                {
                    if (!v.Removed)
                    {
                        dist = from.Distance + (from.Teleporter ? 1 : from.DistanceTo(v));

                        if (dist < v.Distance)
                        {
                            v.Distance = dist;
                            v.Previous = from;

                            if (!v.Visited)
                                heap.Push(v);
                            else
                                heap.Fix(v);
                        }
                    }
                }
            }

            return null;
        }

        public class GuideVertexEditGump : Gump
        {
            private readonly GuideVertex m_Vertex;
            private readonly Map m_Map;
            private readonly string m_Town;
            private readonly Item m_Item;
            public GuideVertexEditGump(GuideVertex vertex, Map map, string town)
                : base(50, 50)
            {
                this.m_Vertex = vertex;
                this.m_Map = map;
                this.m_Town = town;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                int size = m_ShopDefinitions.Count;
                bool on = false;

                this.AddPage(0);
                this.AddBackground(0, 0, 540, 35 + size * 30 / 2, 9200);
                this.AddAlphaRegion(15, 10, 510, 15 + size * 30 / 2);

                for (int i = 0; i < size; i += 2)
                {
                    on = this.m_Vertex.Shops.Contains(i);
                    this.AddButton(25, 25 + i * 30 / 2, on ? 2361 : 2360, on ? 2360 : 2361, i + 1, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(50, 20 + i * 30 / 2, 200, 20, m_ShopDefinitions[i], 0x7773, false, false);

                    if (i + 1 < size)
                    {
                        on = this.m_Vertex.Shops.Contains(i + 1);
                        this.AddButton(280, 25 + i * 30 / 2, on ? 2361 : 2360, on ? 2360 : 2361, i + 2, GumpButtonType.Reply, 0);
                        this.AddHtmlLocalized(305, 20 + i * 30 / 2, 200, 20, m_ShopDefinitions[i + 1], 0x7773, false, false);
                    }
                }

                this.m_Item = new Item(0x1183);
                this.m_Item.Visible = false;
                this.m_Item.MoveToWorld(this.m_Vertex.Location, map);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID > 0)
                {
                    if (this.m_Vertex.Shops.Contains(info.ButtonID - 1))
                        this.m_Vertex.Shops.Remove(info.ButtonID - 1);
                    else
                        this.m_Vertex.Shops.Add(info.ButtonID - 1);

                    sender.Mobile.SendGump(new GuideVertexEditGump(this.m_Vertex, this.m_Map, this.m_Town));
                }
                else if (this.m_Item != null && !this.m_Item.Deleted)
                {
                    this.m_Item.Delete();
                    this.Save(this.m_Town);
                }
            }

            private void Save(string town)
            {
                if (!m_GraphDefinitions.ContainsKey(town))
                    return;

                List<GuideVertex> list = m_GraphDefinitions[town];
                string path = Core.BaseDirectory + String.Format("\\Data\\Guide\\{0}.graph", town);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine("# Graph vertices");
                        writer.WriteLine("# {V:}VertexID{tab, }X{tab, }Y{tab, }Z{tab, }IsTeleporter");
                        writer.WriteLine("# {S:}ShopID{tab, }ShopID{tab, }...");
                        writer.WriteLine("# {N:}VertexID{tab, }VertexID{tab, }...");

                        foreach (GuideVertex v in list)
                        {
                            writer.WriteLine(String.Format("V:\t{0}\t{1}\t{2}\t{3}\t{4}", v.ID, v.Location.X, v.Location.Y, v.Location.Z, v.Teleporter.ToString()));

                            if (v.Shops.Count > 0)
                            {
                                writer.Write("S:");

                                foreach (int i in v.Shops)
                                    writer.Write(String.Format("\t{0}", i));

                                writer.WriteLine();
                            }

                            if (v.Vertices.Count > 0)
                            {
                                writer.Write("N:");

                                foreach (GuideVertex n in v.Vertices)
                                    writer.Write(String.Format("\t{0}", n.ID));

                                writer.WriteLine();
                            }
                        }
                    }
            }
        }

        public class GuideVertex : IComparable<GuideVertex>
        {
            public bool m_Visited;
            public bool m_Removed;
            private readonly int m_ID;
            private readonly List<GuideVertex> m_Vertices = new List<GuideVertex>();
            private readonly List<int> m_Shops = new List<int>();
            private Point3D m_Location;
            private bool m_Teleporter;
            private GuideVertex m_Previous;
            private int m_Distance;
            public GuideVertex(int id)
                : this(id, Point3D.Zero)
            {
            }

            public GuideVertex(int id, Point3D location)
            {
                this.m_ID = id;
                this.m_Location = location;
                this.m_Previous = null;
                this.m_Distance = Int32.MaxValue;
                this.m_Visited = false;
                this.m_Removed = false;
            }

            public int ID
            {
                get
                {
                    return this.m_ID;
                }
            }
            public Point3D Location
            {
                get
                {
                    return this.m_Location;
                }
                set
                {
                    this.m_Location = value;
                }
            }
            public List<GuideVertex> Vertices
            {
                get
                {
                    return this.m_Vertices;
                }
            }
            public List<int> Shops
            {
                get
                {
                    return this.m_Shops;
                }
            }
            public bool Teleporter
            {
                get
                {
                    return this.m_Teleporter;
                }
                set
                {
                    this.m_Teleporter = value;
                }
            }
            public GuideVertex Previous
            {
                get
                {
                    return this.m_Previous;
                }
                set
                {
                    this.m_Previous = value;
                }
            }
            public int Distance
            {
                get
                {
                    return this.m_Distance;
                }
                set
                {
                    this.m_Distance = value;
                }
            }
            public bool Visited
            {
                get
                {
                    return this.m_Visited;
                }
                set
                {
                    this.m_Visited = value;
                }
            }
            public bool Removed
            {
                get
                {
                    return this.m_Removed;
                }
                set
                {
                    this.m_Removed = value;
                }
            }
            public int DistanceTo(GuideVertex to)
            {
                return Math.Abs(this.m_Location.X - to.Location.X) + Math.Abs(this.m_Location.Y - to.Location.Y);
            }

            public int DistanceTo(Point3D to)
            {
                return Math.Abs(this.m_Location.X - to.X) + Math.Abs(this.m_Location.Y - to.Y);
            }

            public int CompareTo(GuideVertex o)
            {
                if (o != null)
                    return this.m_Distance - o.Distance;

                return 0;
            }
        }

        public class Heap<T> where T : IComparable<T>
        {
            private readonly List<T> m_List;
            public Heap()
            {
                this.m_List = new List<T>();
            }

            public int Count
            {
                get
                {
                    return this.m_List.Count;
                }
            }
            public T Top
            {
                get
                {
                    return this.m_List[0];
                }
            }
            public bool Contains(T item)
            {
                return this.m_List.Contains(item);
            }

            public void Push(T item)
            {
                this.m_List.Add(item);

                int child = this.m_List.Count - 1;
                int parent = (child - 1) / 2;
                T temp;

                while (item.CompareTo(this.m_List[parent]) < 0 && child > 0)
                {
                    temp = this.m_List[child];
                    this.m_List[child] = this.m_List[parent];
                    this.m_List[parent] = temp;

                    child = parent;
                    parent = (child - 1) / 2;
                }
            }

            public void Fix(T item)
            {
                int child = this.m_List.IndexOf(item);
                int parent = (child - 1) / 2;
                T temp;

                while (item.CompareTo(this.m_List[parent]) < 0 && child > 0)
                {
                    temp = this.m_List[child];
                    this.m_List[child] = this.m_List[parent];
                    this.m_List[parent] = temp;

                    child = parent;
                    parent = (child - 1) / 2;
                }
            }

            public T Pop()
            {
                if (this.m_List.Count == 0)
                    return default( T );

                T top = this.m_List[0];
                T temp;

                this.m_List[0] = this.m_List[this.m_List.Count - 1];
                this.m_List.RemoveAt(this.m_List.Count - 1);

                int parent = 0;
                int lchild;
                int rchild;
				
                bool ltl, ltr, cltc;

                do
                {
                    lchild = parent * 2 + 1;
                    rchild = parent * 2 + 2;
                    ltl = ltr = cltc = false;

                    if (this.m_List.Count > lchild)
                        ltl = (this.m_List[parent].CompareTo(this.m_List[lchild]) > 0);

                    if (this.m_List.Count > rchild)
                        ltr = (this.m_List[parent].CompareTo(this.m_List[rchild]) > 0);

                    if (ltl && ltr)
                        cltc = (this.m_List[lchild].CompareTo(this.m_List[rchild]) > 0);

                    if (ltl && !ltr)
                    {
                        temp = this.m_List[parent];
                        this.m_List[parent] = this.m_List[lchild];
                        this.m_List[lchild] = temp;

                        parent = lchild;
                    }
                    else if (!ltl && ltr)
                    {
                        temp = this.m_List[parent];
                        this.m_List[parent] = this.m_List[rchild];
                        this.m_List[rchild] = temp;

                        parent = rchild;
                    }
                    else if (ltl && ltr && cltc)
                    {
                        temp = this.m_List[parent];
                        this.m_List[parent] = this.m_List[rchild];
                        this.m_List[rchild] = temp;

                        parent = rchild;
                    }
                    else if (ltl && ltr)
                    {
                        temp = this.m_List[parent];
                        this.m_List[parent] = this.m_List[lchild];
                        this.m_List[lchild] = temp;

                        parent = lchild;
                    }
                }
                while (ltl || ltr);

                return top;
            }
        }
    }

    public class AttendantGuide : PersonalAttendant
    {
        private List<Vertex> m_Path;
        public AttendantGuide()
            : base("the Guide")
        {
            this.m_Path = null;
        }

        public AttendantGuide(Serial serial)
            : base(serial)
        {
        }

        public List<Vertex> Path 
        { 
            get
            {
                return this.m_Path;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (from.Alive && this.IsOwner(from))
            {
                Dictionary<int, Vertex> m_Shops = GuideHelper.FindShops(this.Region != null ? this.Region.Name : null, this.Location);

                if (m_Shops != null)
                {
                    from.CloseGump(typeof(InternalGump));
                    from.SendGump(new InternalGump(this, m_Shops));
                }
                else
                    from.SendLocalizedMessage(1076113); // There are no shops nearby.  Please try again when you get to a town or city.
            }
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && this.IsOwner(from))
                list.Add(new AttendantUseEntry(this, 6249));

            base.AddCustomContextEntries(from, list);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (this.ControlMaster != null && this.m_Path != null && this.m_Path.Count > 0)
            {
                Vertex v = this.m_Path[this.m_Path.Count - 1];
                Mobile m = this.ControlMaster;

                if (m.NetState != null)
                {
                    if (Math.Abs(v.DistanceTo(m.Location) - v.DistanceTo(this.Location)) > 10)
                        this.Frozen = true;
                    else
                        this.Frozen = false;

                    if (this.CurrentWayPoint == null)
                    {
                        this.CurrentWayPoint = new WayPoint();
                        this.CurrentWayPoint.MoveToWorld(v.Location, this.Map);
                    }

                    int dist = v.DistanceTo(this.Location);

                    if (dist < (v.Teleporter ? 1 : 3) || dist > 100)
                    {
                        this.m_Path.RemoveAt(this.m_Path.Count - 1);

                        if (this.m_Path.Count > 0)
                        {
                            if (this.CurrentWayPoint == null)
                                this.CurrentWayPoint = new WayPoint();

                            this.CurrentWayPoint.MoveToWorld(this.m_Path[this.m_Path.Count - 1].Location, this.Map);
                        }
                        else
                        {
                            Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(3), new TimerStateCallback<Mobile>(CommandFollow), m);
                            this.Say(1076051); // We have reached our destination
                            this.CommandStop(m);
                        }
                    }
                }
            }
        }

        public override void CommandFollow(Mobile by)
        {
            this.StopGuiding();

            base.CommandFollow(by);
        }

        public override void CommandStop(Mobile by)
        {
            this.StopGuiding();

            base.CommandStop(by);
        }

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

        public void StopGuiding()
        {
            this.CurrentSpeed = this.PassiveSpeed;

            if (this.CurrentWayPoint != null)
                this.CurrentWayPoint.Delete();

            if (this.m_Path != null)
                this.m_Path.Clear();

            this.Controlled = true;
            this.m_Path = null;
        }

        public void StartGuiding(List<Vertex> path)
        {
            this.m_Path = path;
			
            if (this.ControlMaster != null && this.m_Path != null && this.m_Path.Count > 0)
            {
                if (this.m_Path.Count > 1)
                    this.m_Path.RemoveAt(this.m_Path.Count - 1);
				
                if (this.CurrentWayPoint == null)
                    this.CurrentWayPoint = new WayPoint();

                this.CurrentWayPoint.MoveToWorld(this.m_Path[this.m_Path.Count - 1].Location, this.Map);

                this.AIObject.Action = ActionType.Wander;
                this.CurrentSpeed = this.ActiveSpeed;
                this.Controlled = false;
                this.Say(1076114); // Please follow me.
            }
        }

        private class InternalGump : Gump
        {
            private const int ShopsPerPage = 10;
            private const int ShopHeight = 24;
            private readonly AttendantGuide m_Guide;
            private readonly Dictionary<int, Vertex> m_Shops;
            public InternalGump(AttendantGuide guide, Dictionary<int, Vertex> shops)
                : base(60, 36)
            {
                this.m_Guide = guide;
                this.m_Shops = shops;

                this.AddPage(0);

                this.AddBackground(0, 0, 273, 84 + ShopsPerPage * ShopHeight, 0x13BE);
                this.AddImageTiled(10, 10, 253, 20, 0xA40);
                this.AddImageTiled(10, 40, 253, 4 + ShopsPerPage * ShopHeight, 0xA40);
                this.AddImageTiled(10, 54 + ShopsPerPage * ShopHeight, 253, 20, 0xA40);
                this.AddAlphaRegion(10, 10, 253, 64 + ShopsPerPage * ShopHeight);
                this.AddButton(10, 54 + ShopsPerPage * ShopHeight, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 54 + ShopsPerPage * ShopHeight, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
                this.AddHtmlLocalized(14, 12, 273, 20, 1076029, 0x7FFF, false, false); // What sort of shop do you seek?

                int i = 0;
                int page = 0;
                int iPage = 0;

                foreach (KeyValuePair<int, Vertex> kvp in shops)
                {
                    if (i % ShopsPerPage == 0)
                    {
                        if (page > 0)
                        {
                            this.AddButton(188, 54 + ShopsPerPage * ShopHeight, 0xFA5, 0xFA7, 0, GumpButtonType.Page, page + 1);
                            this.AddHtmlLocalized(228, 56 + ShopsPerPage * ShopHeight, 60, 20, 1043353, 0x7FFF, false, false); // Next
                        }

                        this.AddPage(page + 1);

                        if (page > 0)
                        {
                            this.AddButton(113, 54 + ShopsPerPage * ShopHeight, 0xFAE, 0xFB0, 0, GumpButtonType.Page, page);
                            this.AddHtmlLocalized(153, 56 + ShopsPerPage * ShopHeight, 60, 20, 1011393, 0x7FFF, false, false); // Back
                        }

                        iPage = 0;
                        page++;
                    }

                    this.AddButton(19, 49 + iPage * ShopHeight, 0x845, 0x846, 100 + kvp.Key, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(44, 47 + iPage * ShopHeight, 213, 20, GuideHelper.FindShopName(kvp.Key), 0x7FFF, false, false);

                    i++;
                    iPage++;
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                int shop = info.ButtonID - 100;

                if (this.m_Guide == null || this.m_Guide.Deleted || this.m_Guide.Region == null || info.ButtonID == 0)
                    return;

                Vertex source = GuideHelper.ClosestVetrex(this.m_Guide.Region.Name, this.m_Guide.Location);

                if (this.m_Shops.ContainsKey(shop))
                {
                    Vertex destination = this.m_Shops[shop];
                    List<Vertex> path = GuideHelper.Dijkstra(this.m_Guide.Region.Name, source, destination);
                    this.m_Guide.StartGuiding(path);
                }
            }
        }
    }

    public class AttendantMaleGuide : AttendantGuide
    {
        [Constructable]
        public AttendantMaleGuide()
            : base()
        {
        }

        public AttendantMaleGuide(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.SetStr(50, 60);
            this.SetDex(20, 30);
            this.SetInt(100, 110);

            this.Name = NameList.RandomName("male");
            this.Female = false;
            this.Race = Race.Human;
            this.Hue = this.Race.RandomSkinHue();

            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            this.AddItem(new SamuraiTabi());
            this.AddItem(new Kasa());
            this.AddItem(new MaleKimono(Utility.RandomGreenHue()));
            this.AddItem(new ShepherdsCrook());
        }

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
    }

    public class AttendantFemaleGuide : AttendantGuide
    {
        [Constructable]
        public AttendantFemaleGuide()
            : base()
        {
        }

        public AttendantFemaleGuide(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.SetStr(50, 60);
            this.SetDex(20, 30);
            this.SetInt(100, 110);

            this.Name = NameList.RandomName("female");
            this.Female = true;
            this.Race = Race.Human;
            this.Hue = this.Race.RandomSkinHue();

            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            this.AddItem(new Shoes(Utility.RandomBlueHue()));
            this.AddItem(new Shirt(0x8FD));
            this.AddItem(new FeatheredHat(Utility.RandomBlueHue()));
            this.AddItem(new Kilt(Utility.RandomBlueHue()));

            Item item = new Spellbook();
            item.Hue = Utility.RandomBlueHue();
            this.AddItem(item);
        }

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
    }
}