using Server.Commands;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;
using System.IO;
using Vertex = Server.Mobiles.GuideHelper.GuideVertex;

namespace Server.Mobiles
{
    public static class GuideHelper
    {
        private static readonly Dictionary<string, List<Vertex>> m_GraphDefinitions = new Dictionary<string, List<Vertex>>();
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
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
            }
        }

        public static Vertex FindVertex(List<Vertex> list, int id)
        {
            foreach (Vertex v in list)
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
            CommandSystem.Register("GuideEdit", AccessLevel.GameMaster, VertexEdit_OnCommand);

            try
            {
                using (FileStream stream = File.OpenRead(Path.Combine("Data", "Guide", "Definitions.cfg")))
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                        {
                            string[] split = line.Split(m_Separators, StringSplitOptions.RemoveEmptyEntries);

                            if (split != null && split.Length > 1)
                                m_ShopDefinitions.Add(int.Parse(split[1]));
                        }
                    }
                }

                foreach (string file in Directory.GetFiles(Path.Combine("Data", "Guide"), "*.graph"))
                {
                    using (FileStream stream = File.OpenRead(file))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        List<Vertex> list = new List<Vertex>();
                        Vertex current = null;
                        Vertex neighbour = null;

                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();

                            if (!string.IsNullOrEmpty(line))
                            {
                                string[] split = line.Split(m_Separators, StringSplitOptions.RemoveEmptyEntries);
                                int num;

                                if (line.StartsWith("N:"))
                                {
                                    if (current != null)
                                    {
                                        for (int i = 1; i < split.Length; i++)
                                        {
                                            num = int.Parse(split[i]);
                                            neighbour = FindVertex(list, num);

                                            if (neighbour == null)
                                            {
                                                neighbour = new Vertex(num);
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
                                            num = int.Parse(split[i]);

                                            if (num >= 0 && num < m_ShopDefinitions.Count)
                                                current.Shops.Add(num);
                                            else
                                                throw new Exception(string.Format("Invalid shop ID: {0}", num));
                                        }
                                    }
                                }
                                else if (line.StartsWith("V:"))
                                {
                                    if (split.Length > 5)
                                    {
                                        num = int.Parse(split[1]);
                                        neighbour = FindVertex(list, num);

                                        if (neighbour != null)
                                            current = neighbour;
                                        else
                                        {
                                            current = new Vertex(num);
                                            list.Add(current);
                                        }

                                        Point3D location = new Point3D
                                        {
                                            X = int.Parse(split[2]),
                                            Y = int.Parse(split[3]),
                                            Z = int.Parse(split[4])
                                        };
                                        current.Location = location;
                                        current.Teleporter = bool.Parse(split[5]);
                                    }
                                    else
                                        throw new Exception(string.Format("Incomplete vertex definition!"));
                                }
                            }
                        }

                        m_GraphDefinitions.Add(Path.GetFileNameWithoutExtension(file), list);
                    }
                }
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
                LogMessage(m_Delimiter);
            }
        }

        public static void VertexEdit_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            if (m.Region != null)
            {
                Vertex closest = ClosestVetrex(m.Region.Name, m.Location);

                if (closest != null)
                    m.SendGump(new GuideVertexEditGump(closest, m.Map, m.Region.Name));
                else
                    m.SendLocalizedMessage(1076113); // There are no shops nearby.  Please try again when you get to a town or city.
            }
            else
                m.SendLocalizedMessage(1076113); // There are no shops nearby.  Please try again when you get to a town or city.
        }

        public static Vertex ClosestVetrex(string town, Point3D location)
        {
            if (town == null || !m_GraphDefinitions.ContainsKey(town))
                return null;

            List<Vertex> vertices = m_GraphDefinitions[town];

            Vertex closest = null;
            double min = int.MaxValue;
            double distance;

            foreach (Vertex v in vertices)
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

        public static Dictionary<int, Vertex> FindShops(string town, Point3D location)
        {
            if (town == null || !m_GraphDefinitions.ContainsKey(town))
                return null;

            List<Vertex> vertices = m_GraphDefinitions[town];
            Dictionary<int, Vertex> shops = new Dictionary<int, Vertex>();

            foreach (Vertex v in vertices)
            {
                foreach (int shop in v.Shops)
                {
                    if (shops.ContainsKey(shop))
                    {
                        Vertex d = shops[shop];

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

        public static List<Vertex> Dijkstra(string town, Vertex source, Vertex destination)
        {
            if (town == null || !m_GraphDefinitions.ContainsKey(town))
                return null;

            Heap<Vertex> heap = new Heap<Vertex>();
            List<Vertex> path = new List<Vertex>();
            heap.Push(source);

            foreach (Vertex v in m_GraphDefinitions[town])
            {
                v.Distance = int.MaxValue;
                v.Previous = null;
                v.Visited = false;
                v.Removed = false;
            }

            source.Distance = 0;
            Vertex from;
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

                foreach (Vertex v in from.Vertices)
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
            private readonly Vertex m_Vertex;
            private readonly Map m_Map;
            private readonly string m_Town;
            private readonly Item m_Item;
            public GuideVertexEditGump(Vertex vertex, Map map, string town)
                : base(50, 50)
            {
                m_Vertex = vertex;
                m_Map = map;
                m_Town = town;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                int size = m_ShopDefinitions.Count;
                bool on = false;

                AddPage(0);
                AddBackground(0, 0, 540, 35 + size * 30 / 2, 9200);
                AddAlphaRegion(15, 10, 510, 15 + size * 30 / 2);

                for (int i = 0; i < size; i += 2)
                {
                    on = m_Vertex.Shops.Contains(i);
                    AddButton(25, 25 + i * 30 / 2, on ? 2361 : 2360, on ? 2360 : 2361, i + 1, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(50, 20 + i * 30 / 2, 200, 20, m_ShopDefinitions[i], 0x7773, false, false);

                    if (i + 1 < size)
                    {
                        on = m_Vertex.Shops.Contains(i + 1);
                        AddButton(280, 25 + i * 30 / 2, on ? 2361 : 2360, on ? 2360 : 2361, i + 2, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(305, 20 + i * 30 / 2, 200, 20, m_ShopDefinitions[i + 1], 0x7773, false, false);
                    }
                }

                m_Item = new Item(0x1183)
                {
                    Visible = false
                };
                m_Item.MoveToWorld(m_Vertex.Location, map);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID > 0)
                {
                    if (m_Vertex.Shops.Contains(info.ButtonID - 1))
                        m_Vertex.Shops.Remove(info.ButtonID - 1);
                    else
                        m_Vertex.Shops.Add(info.ButtonID - 1);

                    sender.Mobile.SendGump(new GuideVertexEditGump(m_Vertex, m_Map, m_Town));
                }
                else if (m_Item != null && !m_Item.Deleted)
                {
                    m_Item.Delete();
                    Save(m_Town);
                }
            }

            private void Save(string town)
            {
                if (!m_GraphDefinitions.ContainsKey(town))
                    return;

                List<Vertex> list = m_GraphDefinitions[town];
                string path = Core.BaseDirectory + string.Format("\\Data\\Guide\\{0}.graph", town);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine("# Graph vertices");
                    writer.WriteLine("# {V:}VertexID{tab, }X{tab, }Y{tab, }Z{tab, }IsTeleporter");
                    writer.WriteLine("# {S:}ShopID{tab, }ShopID{tab, }...");
                    writer.WriteLine("# {N:}VertexID{tab, }VertexID{tab, }...");

                    foreach (Vertex v in list)
                    {
                        writer.WriteLine(string.Format("V:\t{0}\t{1}\t{2}\t{3}\t{4}", v.ID, v.Location.X, v.Location.Y, v.Location.Z, v.Teleporter.ToString()));

                        if (v.Shops.Count > 0)
                        {
                            writer.Write("S:");

                            foreach (int i in v.Shops)
                                writer.Write(string.Format("\t{0}", i));

                            writer.WriteLine();
                        }

                        if (v.Vertices.Count > 0)
                        {
                            writer.Write("N:");

                            foreach (Vertex n in v.Vertices)
                                writer.Write(string.Format("\t{0}", n.ID));

                            writer.WriteLine();
                        }
                    }
                }
            }
        }

        public class GuideVertex : IComparable<Vertex>
        {
            public bool m_Visited;
            public bool m_Removed;
            private readonly int m_ID;
            private readonly List<Vertex> m_Vertices = new List<Vertex>();
            private readonly List<int> m_Shops = new List<int>();
            private Point3D m_Location;
            private bool m_Teleporter;
            private Vertex m_Previous;
            private int m_Distance;
            public GuideVertex(int id)
                : this(id, Point3D.Zero)
            {
            }

            public GuideVertex(int id, Point3D location)
            {
                m_ID = id;
                m_Location = location;
                m_Previous = null;
                m_Distance = int.MaxValue;
                m_Visited = false;
                m_Removed = false;
            }

            public int ID => m_ID;
            public Point3D Location
            {
                get
                {
                    return m_Location;
                }
                set
                {
                    m_Location = value;
                }
            }
            public List<Vertex> Vertices => m_Vertices;
            public List<int> Shops => m_Shops;
            public bool Teleporter
            {
                get
                {
                    return m_Teleporter;
                }
                set
                {
                    m_Teleporter = value;
                }
            }
            public Vertex Previous
            {
                get
                {
                    return m_Previous;
                }
                set
                {
                    m_Previous = value;
                }
            }
            public int Distance
            {
                get
                {
                    return m_Distance;
                }
                set
                {
                    m_Distance = value;
                }
            }
            public bool Visited
            {
                get
                {
                    return m_Visited;
                }
                set
                {
                    m_Visited = value;
                }
            }
            public bool Removed
            {
                get
                {
                    return m_Removed;
                }
                set
                {
                    m_Removed = value;
                }
            }
            public int DistanceTo(Vertex to)
            {
                return Math.Abs(m_Location.X - to.Location.X) + Math.Abs(m_Location.Y - to.Location.Y);
            }

            public int DistanceTo(Point3D to)
            {
                return Math.Abs(m_Location.X - to.X) + Math.Abs(m_Location.Y - to.Y);
            }

            public int CompareTo(Vertex o)
            {
                if (o != null)
                    return m_Distance - o.Distance;

                return 0;
            }
        }

        public class Heap<T> where T : IComparable<T>
        {
            private readonly List<T> m_List;
            public Heap()
            {
                m_List = new List<T>();
            }

            public int Count => m_List.Count;
            public T Top => m_List[0];
            public bool Contains(T item)
            {
                return m_List.Contains(item);
            }

            public void Push(T item)
            {
                m_List.Add(item);

                int child = m_List.Count - 1;
                int parent = (child - 1) / 2;
                T temp;

                while (item.CompareTo(m_List[parent]) < 0 && child > 0)
                {
                    temp = m_List[child];
                    m_List[child] = m_List[parent];
                    m_List[parent] = temp;

                    child = parent;
                    parent = (child - 1) / 2;
                }
            }

            public void Fix(T item)
            {
                int child = m_List.IndexOf(item);
                int parent = (child - 1) / 2;
                T temp;

                while (item.CompareTo(m_List[parent]) < 0 && child > 0)
                {
                    temp = m_List[child];
                    m_List[child] = m_List[parent];
                    m_List[parent] = temp;

                    child = parent;
                    parent = (child - 1) / 2;
                }
            }

            public T Pop()
            {
                if (m_List.Count == 0)
                    return default(T);

                T top = m_List[0];
                T temp;

                m_List[0] = m_List[m_List.Count - 1];
                m_List.RemoveAt(m_List.Count - 1);

                int parent = 0;
                int lchild;
                int rchild;

                bool ltl, ltr, cltc;

                do
                {
                    lchild = parent * 2 + 1;
                    rchild = parent * 2 + 2;
                    ltl = ltr = cltc = false;

                    if (m_List.Count > lchild)
                        ltl = (m_List[parent].CompareTo(m_List[lchild]) > 0);

                    if (m_List.Count > rchild)
                        ltr = (m_List[parent].CompareTo(m_List[rchild]) > 0);

                    if (ltl && ltr)
                        cltc = (m_List[lchild].CompareTo(m_List[rchild]) > 0);

                    if (ltl && !ltr)
                    {
                        temp = m_List[parent];
                        m_List[parent] = m_List[lchild];
                        m_List[lchild] = temp;

                        parent = lchild;
                    }
                    else if (!ltl && ltr)
                    {
                        temp = m_List[parent];
                        m_List[parent] = m_List[rchild];
                        m_List[rchild] = temp;

                        parent = rchild;
                    }
                    else if (ltl && ltr && cltc)
                    {
                        temp = m_List[parent];
                        m_List[parent] = m_List[rchild];
                        m_List[rchild] = temp;

                        parent = rchild;
                    }
                    else if (ltl && ltr)
                    {
                        temp = m_List[parent];
                        m_List[parent] = m_List[lchild];
                        m_List[lchild] = temp;

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
            m_Path = null;
        }

        public AttendantGuide(Serial serial)
            : base(serial)
        {
        }

        public List<Vertex> Path => m_Path;
        public override void OnDoubleClick(Mobile from)
        {
            if (from.Alive && IsOwner(from))
            {
                Dictionary<int, Vertex> m_Shops = GuideHelper.FindShops(Region != null ? Region.Name : null, Location);

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
            if (from.Alive && IsOwner(from))
                list.Add(new AttendantUseEntry(this, 6249));

            base.AddCustomContextEntries(from, list);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (ControlMaster != null && m_Path != null && m_Path.Count > 0)
            {
                Vertex v = m_Path[m_Path.Count - 1];
                Mobile m = ControlMaster;

                if (m.NetState != null)
                {
                    if (Math.Abs(v.DistanceTo(m.Location) - v.DistanceTo(Location)) > 10)
                        Frozen = true;
                    else
                        Frozen = false;

                    if (CurrentWayPoint == null)
                    {
                        CurrentWayPoint = new WayPoint();
                        CurrentWayPoint.MoveToWorld(v.Location, Map);
                    }

                    int dist = v.DistanceTo(Location);

                    if (dist < (v.Teleporter ? 1 : 3) || dist > 100)
                    {
                        m_Path.RemoveAt(m_Path.Count - 1);

                        if (m_Path.Count > 0)
                        {
                            if (CurrentWayPoint == null)
                                CurrentWayPoint = new WayPoint();

                            CurrentWayPoint.MoveToWorld(m_Path[m_Path.Count - 1].Location, Map);
                        }
                        else
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(3), CommandFollow, m);
                            Say(1076051); // We have reached our destination
                            CommandStop(m);
                        }
                    }
                }
            }
        }

        public override void CommandFollow(Mobile by)
        {
            StopGuiding();

            base.CommandFollow(by);
        }

        public override void CommandStop(Mobile by)
        {
            StopGuiding();

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
            CurrentSpeed = PassiveSpeed;

            if (CurrentWayPoint != null)
                CurrentWayPoint.Delete();

            if (m_Path != null)
                m_Path.Clear();

            Controlled = true;
            m_Path = null;
        }

        public void StartGuiding(List<Vertex> path)
        {
            m_Path = path;

            if (ControlMaster != null && m_Path != null && m_Path.Count > 0)
            {
                if (m_Path.Count > 1)
                    m_Path.RemoveAt(m_Path.Count - 1);

                if (CurrentWayPoint == null)
                    CurrentWayPoint = new WayPoint();

                CurrentWayPoint.MoveToWorld(m_Path[m_Path.Count - 1].Location, Map);

                AIObject.Action = ActionType.Wander;
                CurrentSpeed = ActiveSpeed;
                Controlled = false;
                Say(1076114); // Please follow me.
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
                m_Guide = guide;
                m_Shops = shops;

                AddPage(0);

                AddBackground(0, 0, 273, 84 + ShopsPerPage * ShopHeight, 0x13BE);
                AddImageTiled(10, 10, 253, 20, 0xA40);
                AddImageTiled(10, 40, 253, 4 + ShopsPerPage * ShopHeight, 0xA40);
                AddImageTiled(10, 54 + ShopsPerPage * ShopHeight, 253, 20, 0xA40);
                AddAlphaRegion(10, 10, 253, 64 + ShopsPerPage * ShopHeight);
                AddButton(10, 54 + ShopsPerPage * ShopHeight, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 54 + ShopsPerPage * ShopHeight, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
                AddHtmlLocalized(14, 12, 273, 20, 1076029, 0x7FFF, false, false); // What sort of shop do you seek?

                int i = 0;
                int page = 0;
                int iPage = 0;

                foreach (KeyValuePair<int, Vertex> kvp in shops)
                {
                    if (i % ShopsPerPage == 0)
                    {
                        if (page > 0)
                        {
                            AddButton(188, 54 + ShopsPerPage * ShopHeight, 0xFA5, 0xFA7, 0, GumpButtonType.Page, page + 1);
                            AddHtmlLocalized(228, 56 + ShopsPerPage * ShopHeight, 60, 20, 1043353, 0x7FFF, false, false); // Next
                        }

                        AddPage(page + 1);

                        if (page > 0)
                        {
                            AddButton(113, 54 + ShopsPerPage * ShopHeight, 0xFAE, 0xFB0, 0, GumpButtonType.Page, page);
                            AddHtmlLocalized(153, 56 + ShopsPerPage * ShopHeight, 60, 20, 1011393, 0x7FFF, false, false); // Back
                        }

                        iPage = 0;
                        page++;
                    }

                    AddButton(19, 49 + iPage * ShopHeight, 0x845, 0x846, 100 + kvp.Key, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(44, 47 + iPage * ShopHeight, 213, 20, GuideHelper.FindShopName(kvp.Key), 0x7FFF, false, false);

                    i++;
                    iPage++;
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                int shop = info.ButtonID - 100;

                if (m_Guide == null || m_Guide.Deleted || m_Guide.Region == null || info.ButtonID == 0)
                    return;

                Vertex source = GuideHelper.ClosestVetrex(m_Guide.Region.Name, m_Guide.Location);

                if (m_Shops.ContainsKey(shop))
                {
                    Vertex destination = m_Shops[shop];
                    List<Vertex> path = GuideHelper.Dijkstra(m_Guide.Region.Name, source, destination);
                    m_Guide.StartGuiding(path);
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
            SetStr(50, 60);
            SetDex(20, 30);
            SetInt(100, 110);

            Name = NameList.RandomName("male");
            Female = false;
            Race = Race.Human;
            Hue = Race.RandomSkinHue();

            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            AddItem(new SamuraiTabi());
            AddItem(new Kasa());
            AddItem(new MaleKimono(Utility.RandomGreenHue()));
            AddItem(new ShepherdsCrook());
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
            SetStr(50, 60);
            SetDex(20, 30);
            SetInt(100, 110);

            Name = NameList.RandomName("female");
            Female = true;
            Race = Race.Human;
            Hue = Race.RandomSkinHue();

            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            AddItem(new Shoes(Utility.RandomBlueHue()));
            AddItem(new Shirt(0x8FD));
            AddItem(new FeatheredHat(Utility.RandomBlueHue()));
            AddItem(new Kilt(Utility.RandomBlueHue()));

            Item item = new Spellbook
            {
                Hue = Utility.RandomBlueHue()
            };
            AddItem(item);
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
