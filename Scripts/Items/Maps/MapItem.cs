using System;
using System.Collections.Generic;
using Server.Engines.Craft;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x14EB, 0x14EC)]
    public class MapItem : Item, ICraftable
    {
        private Rectangle2D m_Bounds;

        private int m_Width, m_Height;

        private bool m_Protected;
        private bool m_Editable;

        private readonly List<Point2D> m_Pins = new List<Point2D>();

        private const int MaxUserPins = 50;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Protected
        {
            get
            {
                return this.m_Protected;
            }
            set
            {
                this.m_Protected = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D Bounds
        {
            get
            {
                return this.m_Bounds;
            }
            set
            {
                this.m_Bounds = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Width
        {
            get
            {
                return this.m_Width;
            }
            set
            {
                this.m_Width = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Height
        {
            get
            {
                return this.m_Height;
            }
            set
            {
                this.m_Height = value;
            }
        }

        public List<Point2D> Pins
        {
            get
            {
                return this.m_Pins;
            }
        }

        [Constructable]
        public MapItem()
            : base(0x14EC)
        {
            this.Weight = 1.0;

            this.m_Width = 200;
            this.m_Height = 200;
        }

        public virtual void CraftInit(Mobile from)
        {
        }

        public void SetDisplay(int x1, int y1, int x2, int y2, int w, int h)
        {
            this.Width = w;
            this.Height = h;

            if (x1 < 0)
                x1 = 0;

            if (y1 < 0)
                y1 = 0;

            if (x2 >= 5120)
                x2 = 5119;

            if (y2 >= 4096)
                y2 = 4095;

            this.Bounds = new Rectangle2D(x1, y1, x2 - x1, y2 - y1);
        }

        public MapItem(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 2))
                this.DisplayTo(from);
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        }

        public virtual void DisplayTo(Mobile from)
        {
            from.Send(new MapDetails(this));
            from.Send(new MapDisplay(this));

            for (int i = 0; i < this.m_Pins.Count; ++i)
                from.Send(new MapAddPin(this, this.m_Pins[i]));

            from.Send(new MapSetEditable(this, this.ValidateEdit(from)));
        }

        public virtual void OnAddPin(Mobile from, int x, int y)
        {
            if (!this.ValidateEdit(from))
                return;
            else if (this.m_Pins.Count >= MaxUserPins)
                return;

            this.Validate(ref x, ref y);
            this.AddPin(x, y);
        }

        public virtual void OnRemovePin(Mobile from, int number)
        {
            if (!this.ValidateEdit(from))
                return;

            this.RemovePin(number);
        }

        public virtual void OnChangePin(Mobile from, int number, int x, int y)
        {
            if (!this.ValidateEdit(from))
                return;

            this.Validate(ref x, ref y);
            this.ChangePin(number, x, y);
        }

        public virtual void OnInsertPin(Mobile from, int number, int x, int y)
        {
            if (!this.ValidateEdit(from))
                return;
            else if (this.m_Pins.Count >= MaxUserPins)
                return;

            this.Validate(ref x, ref y);
            this.InsertPin(number, x, y);
        }

        public virtual void OnClearPins(Mobile from)
        {
            if (!this.ValidateEdit(from))
                return;

            this.ClearPins();
        }

        public virtual void OnToggleEditable(Mobile from)
        {
            if (this.Validate(from))
                this.m_Editable = !this.m_Editable;

            from.Send(new MapSetEditable(this, this.Validate(from) && this.m_Editable));
        }

        public virtual void Validate(ref int x, ref int y)
        {
            if (x < 0)
                x = 0;
            else if (x >= this.m_Width)
                x = this.m_Width - 1;

            if (y < 0)
                y = 0;
            else if (y >= this.m_Height)
                y = this.m_Height - 1;
        }

        public virtual bool ValidateEdit(Mobile from)
        {
            return this.m_Editable && this.Validate(from);
        }

        public virtual bool Validate(Mobile from)
        {
            if (!from.CanSee(this) || from.Map != this.Map || !from.Alive || this.InSecureTrade)
                return false;
            else if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;
            else if (!this.Movable || this.m_Protected || !from.InRange(this.GetWorldLocation(), 2))
                return false;

            object root = this.RootParent;

            if (root is Mobile && root != from)
                return false;

            return true;
        }

        public void ConvertToWorld(int x, int y, out int worldX, out int worldY)
        {
            worldX = ((this.m_Bounds.Width * x) / this.Width) + this.m_Bounds.X;
            worldY = ((this.m_Bounds.Height * y) / this.Height) + this.m_Bounds.Y;
        }

        public void ConvertToMap(int x, int y, out int mapX, out int mapY)
        {
            mapX = ((x - this.m_Bounds.X) * this.Width) / this.m_Bounds.Width;
            mapY = ((y - this.m_Bounds.Y) * this.Width) / this.m_Bounds.Height;
        }

        public virtual void AddWorldPin(int x, int y)
        {
            int mapX, mapY;
            this.ConvertToMap(x, y, out mapX, out mapY);

            this.AddPin(mapX, mapY);
        }

        public virtual void AddPin(int x, int y)
        {
            this.m_Pins.Add(new Point2D(x, y));
        }

        public virtual void RemovePin(int index)
        {
            if (index > 0 && index < this.m_Pins.Count)
                this.m_Pins.RemoveAt(index);
        }

        public virtual void InsertPin(int index, int x, int y)
        {
            if (index < 0 || index >= this.m_Pins.Count)
                this.m_Pins.Add(new Point2D(x, y));
            else
                this.m_Pins.Insert(index, new Point2D(x, y));
        }

        public virtual void ChangePin(int index, int x, int y)
        {
            if (index >= 0 && index < this.m_Pins.Count)
                this.m_Pins[index] = new Point2D(x, y);
        }

        public virtual void ClearPins()
        {
            this.m_Pins.Clear();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(this.m_Bounds);

            writer.Write(this.m_Width);
            writer.Write(this.m_Height);

            writer.Write(this.m_Protected);
			
            writer.Write(this.m_Pins.Count);
            for (int i = 0; i < this.m_Pins.Count; ++i)
                writer.Write(this.m_Pins[i]);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Bounds = reader.ReadRect2D();

                        this.m_Width = reader.ReadInt();
                        this.m_Height = reader.ReadInt();

                        this.m_Protected = reader.ReadBool();

                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                            this.m_Pins.Add(reader.ReadPoint2D());

                        break;
                    }
            }
        }

        public static void Initialize()
        {
            PacketHandlers.Register(0x56, 11, true, new OnPacketReceive(OnMapCommand));
        }

        private static void OnMapCommand(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            MapItem map = World.FindItem(pvSrc.ReadInt32()) as MapItem;

            if (map == null)
                return;

            int command = pvSrc.ReadByte();
            int number = pvSrc.ReadByte();

            int x = pvSrc.ReadInt16();
            int y = pvSrc.ReadInt16();

            switch ( command )
            {
                case 1:
                    map.OnAddPin(from, x, y);
                    break;
                case 2:
                    map.OnInsertPin(from, number, x, y);
                    break;
                case 3:
                    map.OnChangePin(from, number, x, y);
                    break;
                case 4:
                    map.OnRemovePin(from, number);
                    break;
                case 5:
                    map.OnClearPins(from);
                    break;
                case 6:
                    map.OnToggleEditable(from);
                    break;
            }
        }

        private sealed class MapDetails : Packet
        {
            public MapDetails(MapItem map)
                : base(0x90, 19)
            {
                this.m_Stream.Write((int)map.Serial);
                this.m_Stream.Write((short)0x139D);
                this.m_Stream.Write((short)map.Bounds.Start.X);
                this.m_Stream.Write((short)map.Bounds.Start.Y);
                this.m_Stream.Write((short)map.Bounds.End.X);
                this.m_Stream.Write((short)map.Bounds.End.Y);
                this.m_Stream.Write((short)map.Width);
                this.m_Stream.Write((short)map.Height);
            }
        }

        private abstract class MapCommand : Packet
        {
            public MapCommand(MapItem map, int command, int number, int x, int y)
                : base(0x56, 11)
            {
                this.m_Stream.Write((int)map.Serial);
                this.m_Stream.Write((byte)command);
                this.m_Stream.Write((byte)number);
                this.m_Stream.Write((short)x);
                this.m_Stream.Write((short)y); 
            }
        }

        private sealed class MapDisplay : MapCommand
        {
            public MapDisplay(MapItem map)
                : base(map, 5, 0, 0, 0)
            {
            }
        }

        private sealed class MapAddPin : MapCommand
        {
            public MapAddPin(MapItem map, Point2D point)
                : base(map, 1, 0, point.X, point.Y)
            {
            }
        }

        private sealed class MapSetEditable : MapCommand
        {
            public MapSetEditable(MapItem map, bool editable)
                : base(map, 7, editable ? 1 : 0, 0, 0)
            {
            }
        }
        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            this.CraftInit(from);
            return 1;
        }
        #endregion
    }
}