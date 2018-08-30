using Server.Commands;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Server.Gumps
{
    internal delegate void OnTarget(Mobile from, object targeted);
    internal delegate void OnCancel(Mobile from);

    public static class HouseGenInfo
    {
        private static List<InternalData> housesToGen = new List<InternalData>();
        internal static List<InternalData> HousesToGen { get { return housesToGen; } }
    }

    public abstract class HouseGenGump : Gump
    {
        public HouseGenGump() : base( 500, 250 )
        {
            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
            
            AddPage(0);
			AddBackground(0, 0, 300, 465, 2620);
			AddLabel(76, 10, 2102, @"House Script Generator");
        }

        public abstract int ButtonOffset { get; }

        public override int GetTypeID()
        {
            return typeof(HouseGenGump).FullName.GetHashCode();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
        }

        public void AddLabelButtonAt(int index, string lableText)
        {
            var y = index % 15;
            AddLabel(15, 35 + 25 * y, 2102, lableText);
            AddButton(257, 36 + 25 * y, 4005, 4007, index + ButtonOffset, GumpButtonType.Reply, 0);
        }

        public void AddListItemAt(int index, int count, string lableText)
        {
            var y = index % 15;
            AddLabel(15, 35 + 25 * y, 2102, lableText);
            AddButton(222, 36 + 25 * y, 4020, 4022, count + index + ButtonOffset, GumpButtonType.Reply, 0);
            AddButton(257, 36 + 25 * y, 4005, 4007, index + ButtonOffset, GumpButtonType.Reply, 0);
        }
        
        private int textEntries;

        public void AddTextEntryAt(int index, string defaultText)
        {
            var y = index % 15;
            AddBackground(15, 35 + 25 * y, 270, 25, 3000);
            AddTextEntry(18, 40 + 25 * y, 270, 20, 0, textEntries++, defaultText);
        }

        public void AddLabelEntryAt(int index, string lableText, string defaultText)
        {
            var y = index % 15;
            AddLabel(15, 35 + 25 * y, 2102, lableText);
            AddBackground(165, 35 + 25 * y, 120, 25, 3000);
            AddTextEntry(168, 40 + 25 * y, 120, 20, 0, textEntries++, defaultText);
        }

        public void AddSaveButton(int buttonID)
        {
            //Save
            AddButton(230, 430, 2444, 2443, buttonID, GumpButtonType.Reply, 0);
            AddLabel(250, 432, 0, @"Save");
        }
    }
    
    public class HouseGenList : HouseGenGump
    {
        private static string deedTemplate = @"";
        private static string houseTemplate = @"";
        private static string outputFolder = @"";

        public override int ButtonOffset { get { return 3; } }

        public static void Initialize()
        {
            CommandSystem.Register(@"HouseGen", AccessLevel.Administrator, new CommandEventHandler((e)=> {
                e.Mobile.SendGump(new HouseGenList());
            }));
            CommandSystem.Register(@"HG", AccessLevel.Administrator, new CommandEventHandler((e) => {
                e.Mobile.SendGump(new HouseGenList());
            }));
        }

        public HouseGenList() : base()
        {
            //add
            AddButton(15, 435, 2462, 2461, 1, GumpButtonType.Reply, 0);

            AddSaveButton(2);

            var count = HouseGenInfo.HousesToGen.Count;
            var pages = count / 15 + (count % 15 > 0 ? 1 : 0);

            for (int i = 0; i < count; i++)
            {
                if (i % 15 == 0)
                {
                    var page = i / 15 + 1;
                    AddPage(page);

                    if (pages > 1)
                    {
                        //previous
                        AddButton(69, 435, 2468, 2467, 1, GumpButtonType.Page, page - 1 == 0 ? pages : page - 1);
                        //next
                        AddButton(153, 435, 2471, 2470, 2, GumpButtonType.Page, page + 1 > pages ? 1 : page + 1);
                    }
                }

                AddListItemAt(i, HouseGenInfo.HousesToGen.Count, HouseGenInfo.HousesToGen[i].BuildingName);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);
            switch(info.ButtonID)
            {
                case 0:
                    break;
                case 1:
                    //Add
                    sender.Mobile.SendGump(new HouseGenItem(HouseGenInfo.HousesToGen.Count));
                    break;
                case 2:
                    sender.Mobile.SendMessage(32, "Saving " + HouseGenInfo.HousesToGen.Count + " House and Deed Scripts.");
                    OnSave();
                    sender.Mobile.SendMessage(32, "All House Entries have been saved.");
                    break;
                default:
                    //Item at index
                    var t = (info.ButtonID - ButtonOffset) / HouseGenInfo.HousesToGen.Count;
                    var index = (info.ButtonID - ButtonOffset) % HouseGenInfo.HousesToGen.Count;
                    if (t < 1)
                    {
                        if(index >= 0 && index < HouseGenInfo.HousesToGen.Count)
                            sender.Mobile.SendGump(new HouseGenItem(index));
                    }
                    else
                    {
                        if (index >= 0 && index < HouseGenInfo.HousesToGen.Count)
                        {
                            sender.Mobile.SendMessage(32, HouseGenInfo.HousesToGen[index].BuildingName + " deleted.");
                            HouseGenInfo.HousesToGen.RemoveAt(index);
                        }
                        sender.Mobile.SendGump(new HouseGenList());
                    }
                    //Item at index
                    
                    break;
            }
        }

        private void OnSave()
        {
            foreach (var id in HouseGenInfo.HousesToGen)
            {
                string line = null;
                using (StreamReader reader = new StreamReader(deedTemplate))
                {
                    var fname = id.BuildingName + "Deed.cs";
                    var o = Path.Combine(outputFolder, fname);
                    using (StreamWriter writer = new StreamWriter(o))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = FormatLine(line, id);
                            writer.WriteLine(line);
                        }
                    }
                }

                using (StreamReader reader = new StreamReader(houseTemplate))
                {
                    var fname = id.BuildingName + ".cs";
                    var o = Path.Combine(outputFolder, fname);
                    using (StreamWriter writer = new StreamWriter(o))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = FormatLine(line, id);
                            writer.WriteLine(line);
                        }
                    }
                }
            }
        }

        private static string FormatLine(string line, InternalData data)
        {
            var nospace = data.BuildingName.Replace(" ", "");
            nospace = Regex.Replace(nospace, "[^0-9A-Za-z]+", "");
            line = line.Replace("~HouseName~", nospace);
            line = line.Replace("~HouseNameSpace~", data.BuildingName);
            line = line.Replace("~MultiID~", data.Multi.ItemID.ToString());
            line = line.Replace("~PositionOffset~", data.PlacementOffset.X + ", " + data.PlacementOffset.Y + ", " + data.PlacementOffset.Z);
            line = line.Replace("~AreaArray~", AreaArray(data.Rects.ToArray()));
            line = line.Replace("~MaxLockDown~", data.MaxLockDowns.ToString());
            line = line.Replace("~MaxSecures~", data.MaxSecures.ToString());
            line = line.Replace("~DoorSetUp~", data.DoorString());
            line = line.Replace("~SignSetUp~", SignString(data.SignOffset));
            line = line.Replace("~Price~", data.Price.ToString());
            line = line.Replace("~BanPoint~", data.BanPoint.X + ", " + data.BanPoint.Y + ", " + data.BanPoint.Z);
            return line;
        }

        private static string AreaArray(Rectangle2D[] rects)
        {
            var n = "new Rectangle2D(rect), ";
            var area = "";

            foreach (var r in rects)
            {
                var o = "x, y, w, h"
                    .Replace("x", r.X.ToString())
                    .Replace("y", r.Y.ToString())
                    .Replace("w", r.Width.ToString())
                    .Replace("h", r.Height.ToString());
                area += n.Replace("rect", o);
            }
            area = area.Substring(0, area.Length - 2);
            return area;
        }

        private static string SignString(Point3D signOffset)
        {
            var s = "this.SetSign(x, y, z);";
            s = s.Replace("x", signOffset.X.ToString());
            s = s.Replace("y", signOffset.Y.ToString());
            s = s.Replace("z", signOffset.Z.ToString());
            return s;
        }
    }

    public class HouseGenItem : HouseGenGump
    {
        private enum Operations
        {
            Close,
            Save,
            Name,
            Multi,
            Placement,
            Sign,
            Ban,
            Areas,
            Doors,
            MaxLockdowns,
            MaxSecures,
            Price
        }
        private enum TextEntries
        {
            Name,
            MaxLockdows,
            MaxSecure,
            Price
        }

        public override int ButtonOffset { get { return 2; } }

        private int index;

        private InternalData data;

        private Operations currentOp;

        public static void Initialize()
        {
            CommandSystem.Register(@"HouseGenAdd", AccessLevel.Administrator, new CommandEventHandler((e) => {
                if (string.IsNullOrWhiteSpace(e.ArgString))
                {
                    e.Mobile.SendMessage("Invalid Arguments Supplied");
                    return;
                }

                e.Mobile.SendMessage("Select the BaseMulti to use.");
                e.Mobile.Target = new InternalTarget((from, t)=> {
                    var data = new InternalData();
                    var p = (IPoint2D)t;

                    var multi = FindMultiAt(p, from.Map);

                    if (multi == null)
                    {
                        from.SendMessage(@"No BaseMulti was found.");
                        return;
                    }

                    data.Multi = multi;
                    data.BuildingName = e.ArgString;

                    from.SendGump(new HouseGenItem(-1, data));
                }, (f)=> { });
            }));
        }

        /// <summary>
        /// Creates gump by index
        /// Loads Internal Data and Clones when avaliable, When not avaliable creates a new.
        /// </summary>
        /// <param name="index"></param>
        public HouseGenItem(int index) : this(index, index > -1 && index < HouseGenInfo.HousesToGen.Count ? InternalData.Clone(HouseGenInfo.HousesToGen[index]) : new InternalData()){}

        internal HouseGenItem(int index, InternalData data) : base()
        {
            this.index = index;
            this.data = data;

            AddSaveButton(1);

            int i = 0;
            AddTextEntryAt(i++, data.BuildingName ?? @"House Name");
            AddLabelButtonAt(i++, @"Multi: " + (data.Multi == null ? @"Set Target" : data.Multi.ItemID.ToString()));
            AddLabelButtonAt(i++, @"Placement Point: " + data.PlacementOffset.ToString());
            AddLabelButtonAt(i++, @"Sign Point: " + data.SignOffset.ToString());
            AddLabelButtonAt(i++, @"Ban Point: " + data.BanPoint.ToString());
            AddLabelButtonAt(i++, @"Areas: " + data.Rects.Count.ToString());
            AddLabelButtonAt(i++, @"Doors: " + data.doors.Count.ToString());
            AddLabelEntryAt (i++, @"Max Lock Downs: ", data.MaxLockDowns.ToString());
            AddLabelEntryAt (i++, @"Max Secures: ", data.MaxSecures.ToString());
            AddLabelEntryAt (i++, @"Price: ", data.Price.ToString());

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var Name = info.TextEntries[(int)TextEntries.Name].Text;
            var Locks = info.TextEntries[(int)TextEntries.MaxLockdows].Text;
            var Sec = info.TextEntries[(int)TextEntries.MaxSecure].Text;
            var Price = info.TextEntries[(int)TextEntries.Price].Text;
            
            currentOp = (Operations)info.ButtonID;

            if (currentOp != Operations.Close)
            {
                data.BuildingName = Name;

                try
                {
                    data.MaxLockDowns = Int32.Parse(Locks);
                }
                catch {
                    sender.Mobile.SendMessage(32, @"Max Lockdowns may only contain numbers, received: " + Locks);
                }
                try
                {
                    data.MaxSecures = Int32.Parse(Sec);
                }
                catch
                {
                    sender.Mobile.SendMessage(32, @"Max Secures may only contain numbers, received: " + Sec);
                }
                try
                {
                    data.Price = Int32.Parse(Price);
                }
                catch
                {
                    sender.Mobile.SendMessage(32, @"Price may only contain numbers, received: " + Price);
                }
            }

            switch (currentOp)
            {
                case Operations.Close:
                    //return to list
                    sender.Mobile.SendGump(new HouseGenList());
                    return;
                case Operations.Save:
                    if (index > -1 && index < HouseGenInfo.HousesToGen.Count)
                        HouseGenInfo.HousesToGen[index] = data;
                    else
                        HouseGenInfo.HousesToGen.Add(data);

                    sender.Mobile.SendMessage(@"Saved to list: " + data.BuildingName);
                    sender.Mobile.SendGump(new HouseGenList());
                    break;
                case Operations.Multi:
                    sender.Mobile.SendMessage(@"Please select the Multi");
                    sender.Mobile.Target = new InternalTarget(OnMultiTarget, OnTargetCancel);
                    return;
                case Operations.Placement:
                    sender.Mobile.SendMessage(@"Please select the Placement Point");
                    sender.Mobile.Target = new InternalTarget(OnOffsetTarget, OnTargetCancel);
                    break;
                case Operations.Sign:
                    sender.Mobile.SendMessage(@"Please select the Sign Point");
                    sender.Mobile.Target = new InternalTarget(OnOffsetTarget, OnTargetCancel);
                    break;
                case Operations.Ban:
                    sender.Mobile.SendMessage(@"Please select the Ban Point");
                    sender.Mobile.Target = new InternalTarget(OnOffsetTarget, OnTargetCancel);
                    break;
                case Operations.Areas:
                    sender.Mobile.SendGump(new HouseGenAreaList(index, data));
                    break;
                case Operations.Doors:
                    sender.Mobile.SendGump(new HouseGenDoorList(index, data));
                    break;
            }
        }

        private void OnOffsetTarget(Mobile from, object t)
        {
            if (data.Multi == null)
            {
                from.SendMessage(32, @"Multi must be targeted first.");
                from.SendGump(new HouseGenItem(index, data));
                return;
            }

            var p = new Point3D((IPoint3D)t);

            p.X -= data.Multi.X;
            p.Y -= data.Multi.Y;
            p.Z -= data.Multi.Z;

            switch (currentOp)
            {   case Operations.Placement:
                    data.PlacementOffset = p;
                    break;
                case Operations.Sign:
                    data.SignOffset = p;
                    break;
                case Operations.Ban:
                    data.BanPoint = p;
                    break;
                default:
                    from.SendMessage(32, @"Internal Error.");
                    break;
            }

            from.SendGump(new HouseGenItem(index, data));
        }

        private void OnMultiTarget(Mobile from, object t)
        {
            var p = (IPoint2D)t;

            var multi = FindMultiAt(p, from.Map);

            if (multi == null)
            {
                from.SendMessage(@"No BaseMulti was found. Please try again, Esc to cancel.");
                from.Target = new InternalTarget(OnMultiTarget, OnTargetCancel);
                return;
            }

            data.Multi = multi;
            from.SendGump(new HouseGenItem(index, data));
        }

        private static BaseMulti FindMultiAt(IPoint2D p, Map map)
        {
            if (p == null || map == null)
                return null;

            var s = map.GetSector(p);
            if (s != null && s.Multis != null && s.Multis.Count > 0)
                return s.Multis.Find(m => m.Contains(p.X, p.Y));

            return null;
        }

        private void OnTargetCancel(Mobile from)
        {
            from.SendMessage(32, @"User canceled target.");
            from.SendGump(new HouseGenItem(index, data));
        }
    }

    internal class HouseGenAreaList : HouseGenGump
    {
        public override int ButtonOffset { get { return 3; } }

        int index;
        private InternalData data;
        List<Rectangle2D> rects;

        public HouseGenAreaList(int index, InternalData data) : this(index, data, data.Rects.ToList()) {}

        private HouseGenAreaList(int index, InternalData data, List<Rectangle2D> rects) : base()
        {
            this.index = index;
            this.data = data;
            this.rects = rects;

            //add
            AddButton(15, 435, 2462, 2461, 1, GumpButtonType.Reply, 0);

            AddSaveButton(2);

            var count = rects.Count;
            var pages = count / 15 + (count % 15 > 0 ? 1 : 0);

            for (int i = 0; i < count; i++)
            {
                if (i % 15 == 0)
                {
                    var page = i / 15 + 1;
                    AddPage(page);

                    if (pages > 1)
                    {
                        //previous
                        AddButton(69, 435, 2468, 2467, 1, GumpButtonType.Page, page - 1 == 0 ? pages : page - 1);
                        //next
                        AddButton(153, 435, 2471, 2470, 2, GumpButtonType.Page, page + 1 > pages ? 1 : page + 1);
                    }
                }
                
                AddListItemAt(i, rects.Count, rects[i].ToString());
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);
            switch (info.ButtonID)
            {
                case 0:
                    sender.Mobile.SendGump(new HouseGenItem(this.index, data));
                    break;
                case 1:
                    //Add
                    sender.Mobile.SendMessage(32, @"Select the area to add to house.");
                    BoundingBoxPicker.Begin(sender.Mobile, OnAreaSelected, null);
                    break;
                case 2:
                    //Save
                    data.Rects = rects;
                    sender.Mobile.SendGump(new HouseGenItem(this.index, data));
                    break;
                default:
                    //Item at index
                    var index = info.ButtonID - ButtonOffset;
                    var t = (info.ButtonID - ButtonOffset) / rects.Count;
                    var i = (info.ButtonID - ButtonOffset) % rects.Count;
                    
                    if (t < 1)
                    {
                        var p = rects[i];
                        var n = new Point3D(data.Multi.X + p.X, data.Multi.Y + p.Y, data.Multi.Z);
                        ShowArea(i);
                        sender.Mobile.SendGump(new HouseGenAreaList(this.index, this.data, rects));
                    }
                    else
                    {
                        var p = rects[i];
                        rects.Remove(p);
                        sender.Mobile.SendGump(new HouseGenAreaList(this.index, this.data, rects));
                    }
                    break;
            }
        }

        private void ShowArea(int i)
        {
            var r = rects[i];
            for (int x = r.X + data.Multi.X; x < r.X + r.Width + data.Multi.X; x++)
            {
                for(int y = r.Y + data.Multi.Y; y < r.Y + r.Height + data.Multi.Y; y++)
                {
                    var a = new Item(0x1CD9);
                    a.MoveToWorld(new Point3D(x, y, 10), data.Multi.Map);
                }
            }
        }

        private void OnAreaSelected(Mobile from, Map map, Point3D start, Point3D end, object state)
        {
            if (data.Multi == null)
            {
                from.SendMessage(32, @"Multi must be targeted first.");
                from.SendGump(new HouseGenItem(index, data));
                return;
            }

            start.X -= data.Multi.X;
            start.Y -= data.Multi.Y;

            end.X -= data.Multi.X;
            end.Y -= data.Multi.Y;

            var rect = new Rectangle2D(start, end);

            if (!rects.Contains(rect))
                rects.Add(rect);
            else
                from.SendMessage(32, @"Duplicate rect found.");

            from.SendGump(new HouseGenAreaList(index, data, rects));
        }
    }

    internal class HouseGenDoorList : HouseGenGump
    {
        public override int ButtonOffset { get { return 3; } }

        private int index;
        private InternalData data;
        private Dictionary<Point3D, InternalData.InternalDoorInfo> doors;

        public HouseGenDoorList(int index, InternalData data) : this(index, data, new Dictionary<Point3D, InternalData.InternalDoorInfo>(data.doors)) {}

        private HouseGenDoorList(int index, InternalData data, Dictionary<Point3D, InternalData.InternalDoorInfo> doors) : base()
        {
            this.index = index;
            this.data = data;
            this.doors = doors;

            //add
            AddButton(15, 435, 2462, 2461, 1, GumpButtonType.Reply, 0);

            AddSaveButton(2);

            var count = doors.Count;
            var pages = count / 15 + (count % 15 > 0 ? 1 : 0);

            for (int i = 0; i < count; i++)
            {
                if (i % 15 == 0)
                {
                    var page = i / 15 + 1;
                    AddPage(page);

                    if (pages > 1)
                    {
                        //previous
                        AddButton(69, 435, 2468, 2467, 1, GumpButtonType.Page, page - 1 == 0 ? pages : page - 1);
                        //next
                        AddButton(153, 435, 2471, 2470, 2, GumpButtonType.Page, page + 1 > pages ? 1 : page + 1);
                    }
                }

                var loc = doors.Keys.ToList()[i];
                AddCheckListItemAt(i, doors.Count, loc.ToString() + @" " + doors[loc].id.ToString());
            }
        }
        
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);
            switch (info.ButtonID)
            {
                case 0:
                    sender.Mobile.SendGump(new HouseGenItem(this.index, data));
                    break;
                case 1:
                    //Add
                    sender.Mobile.Target = new InternalTarget(OnDoorTarget, OnTargetCanceled);
                    break;
                case 2:
                    //Save
                    data.doors = doors;
                    sender.Mobile.SendMessage(@"Doors have been saved to house.");
                    sender.Mobile.SendGump(new HouseGenItem(this.index, data));
                    break;
                default:
                    //Item at index
                    var t = (info.ButtonID - ButtonOffset) / doors.Count;
                    var i = (info.ButtonID - ButtonOffset) % doors.Count;

                    if (t < 1)
                    {
                        var p = doors.Keys.ToList()[i];
                        var n = new Point3D(data.Multi.X + p.X, data.Multi.Y + p.Y, data.Multi.Z + p.Z);
                        sender.Mobile.MoveToWorld(n, data.Multi.Map);
                        sender.Mobile.SendGump(new HouseGenDoorList(this.index, this.data, doors));
                    }
                    else
                    {
                        var p = doors.Keys.ToList()[i];
                        doors.Remove(p);
                        sender.Mobile.SendGump(new HouseGenDoorList(this.index, this.data, doors));
                    }
                    break;
            }
        }

        private void AddCheckListItemAt(int i, int count, string v)
        {
            var y = i % 15;
            AddListItemAt(i, count, v);
            var check = doors[doors.Keys.ToList()[i]].doubleDoor;
            AddCheck(192, 36 + 25 * y, 0x868, 0x86A, check, i);
        }
        
        private void OnDoorTarget(Mobile from, object targeted)
        {
            if (data.Multi == null)
            {
                from.SendMessage(32, @"Multi must be targeted first.");
                from.SendGump(new HouseGenItem(index, data));
                return;
            }

            var d = targeted as BaseDoor;
            if(d == null)
            {
                from.SendMessage(32, @"Target must be of type BaseDoor. Please try again, Esc to cancel.");
                from.Target = new InternalTarget(OnDoorTarget, OnTargetCanceled);
                return;
            }

            var p = new Point3D((IPoint3D)targeted);
            p.X -= data.Multi.X;
            p.Y -= data.Multi.Y;
            p.Z -= data.Multi.Z;

            var i = new InternalData.InternalDoorInfo() { doubleDoor = false, id = d.ClosedID };

            if (doors.ContainsKey(p))
            {
                from.SendMessage(32, @"Unable to add door. Entry already exists at location.");
                from.SendGump(new HouseGenDoorList(index, data, doors));
                return;
            }

            doors.Add(p, i);
            from.SendGump(new HouseGenDoorList(index, data, doors));
        }

        private void OnTargetCanceled(Mobile from)
        {
            from.SendMessage(32, @"User canceled target.");
            from.SendGump(new HouseGenDoorList(index, data, doors));
        }
    }
    
    internal partial class InternalData
    {
        public string BuildingName;

        private BaseMulti multi;
        public BaseMulti Multi
        {
            get { return multi; }
            set { if (value != null) { multi = value; Scan(); } }
        }

        public Point3D PlacementOffset;
        public Point3D SignOffset;
        public Point3D BanPoint;

        public List<Rectangle2D> Rects = new List<Rectangle2D>();

        public Dictionary<Point3D, InternalDoorInfo> doors = new Dictionary<Point3D, InternalDoorInfo>();

        public int MaxLockDowns;
        public int MaxSecures;
        public int Price;

        private void Scan()
        {
            //reset
            {
                doors.Clear();
                Rects.Clear();

                PlacementOffset = Point3D.Zero;
                SignOffset = Point3D.Zero;
                BanPoint = Point3D.Zero;
            }

            //area of multi +2 srounding tiles.
            var minpoint = new Point2D(multi.X + multi.Components.Min.X - 2, multi.Y + multi.Components.Min.Y - 2);
            var maxpoint = new Point2D(multi.X + multi.Components.Max.X + 2, multi.Y + multi.Components.Max.Y + 2);

            List<Item> items = multi.Map.GetItemsInBounds(new Rectangle2D(minpoint, maxpoint)).
                OfType<Item>().Where(o => !(o is BaseMulti)).ToList();

            foreach (Item i in items)
            {
                if (i is BaseDoor)
                {
                    var door = i as BaseDoor;
                    var p = new Point3D(i.Location.X - multi.X, i.Location.Y - multi.Y, i.Location.Z - multi.Z);
                    int idoffset = 0;

                    if (!doors.ContainsKey(p))
                    {
                        var checkLoc = p;
                        switch (door.ClosedID)
                        {
                            case 1653:
                            case 1685:
                            case 1701:
                            case 1749:
                            case 1765:
                                idoffset = 2;
                                checkLoc.X++;
                                break;
                            case 1655:
                            case 1687:
                            case 1703:
                            case 1751:
                            case 1767:
                                idoffset = -2;
                                checkLoc.X--;
                                break;
                            case 1663:
                            case 1695:
                            case 1711:
                            case 1759:
                            case 1775:
                                idoffset = -2;
                                checkLoc.Y++;
                                break;
                            case 1661:
                            case 1693:
                            case 1709:
                            case 1757:
                            case 1773:
                                idoffset = 2;
                                checkLoc.Y--;
                                break;
                        }

                        if (checkLoc != p && Math.Abs(idoffset) == 2 && doors.ContainsKey(checkLoc) && doors[checkLoc].id == door.ClosedID + idoffset)
                        {
                            doors[checkLoc].doubleDoor = true;
                            continue;
                        }

                        doors.Add(p, new InternalDoorInfo() { id = door.ClosedID });
                    }
                }
                else if (i.ItemID == 0x0FEA)
                    BanPoint = new Point3D(i.Location.X - multi.X, i.Location.Y - multi.Y, i.Location.Z - multi.Z);
                else if (i.ItemID == 0x0AEC)
                    PlacementOffset = new Point3D(i.Location.X - multi.X, i.Location.Y - multi.Y, i.Location.Z - multi.Z);
                else if (i.ItemID == 0x0B95 || i.ItemID == 0x0B96 || i.ItemID >= 0x0BA3 || i.ItemID <= 0x0C0E)
                    SignOffset = new Point3D(i.Location.X - multi.X, i.Location.Y - multi.Y, i.Location.Z - multi.Z);
            }
            var zmax = multi.Components.List.OrderBy(o => o.m_OffsetZ).Select(s => (int)s.m_OffsetZ).Last();
            var floors = zmax / 20;
            var vol = multi.Components.List.Where(w=>w.m_OffsetZ < floors * 20).Count();
            Price = vol * 600;
            MaxSecures = Price / 75;
            MaxLockDowns = MaxSecures / 2;

            Rects = multi.Rects();
        }

        public string DoorString()
        {
            var ds = "";
            foreach (var d in doors)
            {
                ds += (d.Value.doubleDoor ?
                            "this.AddDoorsEx(" + (int)d.Value.id + ", point, keyValue)" :
                            "this.AddDoorEx(" + (int)d.Value.id + ", point, keyValue);");

                ds = ds.Replace("point", d.Key.X.ToString() + ", " + d.Key.Y.ToString() + ", " + d.Key.Z.ToString());
                ds += ";\r\n";
            }
            return ds;
        }

        public class InternalDoorInfo
        {
            public int id;
            public bool doubleDoor;
        }

        public static InternalData Clone(InternalData other)
        {
            return (InternalData)other.MemberwiseClone();
        }
    }

    internal class InternalTarget : Target
    {
        OnTarget onTarget;
        OnCancel onCancel;
        public InternalTarget(OnTarget onTarget, OnCancel onCancel) : base(-1, true, TargetFlags.None)
        {
            this.onTarget = onTarget;
            this.onCancel = onCancel;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            onTarget(from, targeted);
        }

        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            onCancel(from);
        }
    }
}