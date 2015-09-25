using System.Collections.Generic;
using Server.Multis;

namespace Server.Items
{
    public abstract class RowBoatItem : BaseShipItem, IRefreshItemID
    {
        private int _baseItemID;

        protected int BaseItemID { get { return _baseItemID; } }
		
		public RowBoat LinkedRowBoat { get; set; }
        
        protected RowBoatItem(RowBoat rowBoat, int northItemID)
            : this(rowBoat, northItemID, Point3D.Zero)
        {
        }

        protected RowBoatItem(RowBoat rowBoat, int northItemID, Point3D initOffset)
            : base(rowBoat, northItemID, initOffset)
        {
            _baseItemID = northItemID;
			LinkedRowBoat = rowBoat;
        }

        public RowBoatItem(Serial serial)
            : base(serial)
        {
        }

        public virtual void RefreshItemID(int itemIDModifier)
        {
            Ship.SetItemIDOnSmooth(this, itemIDModifier + _baseItemID);
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
			
            writer.Write((int)_baseItemID);
			writer.Write(LinkedRowBoat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
            _baseItemID = reader.ReadInt();
			LinkedRowBoat = reader.ReadItem() as RowBoat;
        }
        #endregion
    }
}
