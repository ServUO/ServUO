using System;

namespace Server.Items
{
    public class SimpleMap : MapItem
    {
        private int m_PinIndex;
        [Constructable]
        public SimpleMap()
        {
            this.SetDisplay(0, 0, 5119, 4095, 400, 400);
        }

        public SimpleMap(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Spawner)]
        public int CurrentPin
        {
            // get/set the index (one-based) of the pin that will be referred to by PinLocation
            get
            {
                return this.m_PinIndex;
            }
            set
            {
                this.m_PinIndex = value;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int NPins
        {
            get 
            { 
                if (this.Pins != null)
                {
                    return this.Pins.Count; 
                }
                else
                {
                    return 0;
                }
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Point2D PinLocation
        {
            set 
            { 
                // change the coordinates of the current pin
                if (this.Pins != null && this.CurrentPin > 0 && this.CurrentPin <= this.Pins.Count)
                {
                    int mapx, mapy;
                    this.ConvertToMap(value.X, value.Y, out mapx, out mapy);
                    this.Pins[this.CurrentPin - 1] = new Point2D(mapx, mapy);
                }
            }
            get
            {
                // get the coordinates of the current pin
                if (this.Pins != null && this.CurrentPin > 0 && this.CurrentPin <= this.Pins.Count)
                {
                    int mapx, mapy;
                    this.ConvertToWorld(((Point2D)this.Pins[this.CurrentPin - 1]).X, ((Point2D)this.Pins[this.CurrentPin - 1]).Y, out mapx, out mapy);
                    return new Point2D(mapx, mapy);
                }
                else
                {
                    return Point2D.Zero;
                }
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Point2D NewPin
        {
            set 
            { 
                // add a new pin at the specified world coordinate
                this.AddWorldPin(value.X, value.Y);
                this.CurrentPin = this.NPins;
            }
            get
            {
                // return the last pin added to the Pins arraylist
                if (this.Pins != null && this.NPins > 0)
                {
                    int mapx, mapy;
                    this.ConvertToWorld(((Point2D)this.Pins[this.NPins - 1]).X, ((Point2D)this.Pins[this.NPins - 1]).Y, out mapx, out mapy);
                    return new Point2D(mapx, mapy);
                }
                else
                {
                    return Point2D.Zero;
                }
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public bool ClearAllPins
        {
            get
            {
                return false;
            }
            set
            {
                if (value == true)
                    this.ClearPins();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int PinRemove
        {
            set
            {
                this.RemovePin(value);
            }
            get
            {
                return 0;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Mobile ShowTo
        {
            set 
            { 
                if (value != null)
                {
                    //DisplayTo(value);
                    this.OnDoubleClick(value);
                }
            }
            get
            {
                return null;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1025355;
            }
        }// map
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}