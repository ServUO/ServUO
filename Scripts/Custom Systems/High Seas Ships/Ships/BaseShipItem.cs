using Server.Multis;

namespace Server.Items
{
    public abstract class BaseShipItem : Item, IShareHue
    {	
        #region Properties
		[CommandProperty(AccessLevel.GameMaster)]
		public BaseShip Ship { get; set; }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool ShareHue { get { return true; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set { base.Hue = value; }
        }
        #endregion

        protected BaseShipItem(BaseShip ship, int itemID)
            : this(ship, itemID, Point3D.Zero)
        {
        }

        protected BaseShipItem(BaseShip ship, int itemID, Point3D initOffset)
            : base(itemID)
        {
            Movable = false;
			if (ship != null)
			{
				Ship = ship;
				Location = initOffset;				
				ship.Embark(this);
			}
        }

        public BaseShipItem(Serial serial)
            : base(serial)
        {
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
			
			writer.Write((Item)Ship);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
			
			Ship = reader.ReadItem() as BaseShip;
			
			if (Ship == null)
				Delete();
        }
        #endregion
    }

    public abstract class BaseShipContainer : Container, IShareHue
    {	
        #region Properties
		[CommandProperty(AccessLevel.GameMaster)]
		public BaseShip Ship { get; set; }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool ShareHue { get { return true; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set { base.Hue = value; }
        }
        #endregion

        protected BaseShipContainer(BaseShip ship, int itemID)
            : this(ship, itemID, Point3D.Zero)
        {
        }

        protected BaseShipContainer(BaseShip ship, int itemID, Point3D initOffset)
            : base(itemID)
        {
            Movable = false;
			if (ship != null)
			{
				Ship = ship;
				Location = initOffset;				
				ship.Embark(this);
			}
			GumpID = 0x4C;
        }

        public BaseShipContainer(Serial serial)
            : base(serial)
        {
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
			
			writer.Write((Item)Ship);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
			
			Ship = reader.ReadItem() as BaseShip;
			
			if (Ship == null)
				Delete();			
        }
        #endregion
    }
}
