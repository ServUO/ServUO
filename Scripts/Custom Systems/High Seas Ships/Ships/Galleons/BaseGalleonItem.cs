using System.Collections.Generic;
using Server.Multis;

namespace Server.Items
{
    public interface IRefreshItemID
    {
        void RefreshItemID(int mod);
    }

    public abstract class BaseGalleonItem : BaseShipItem, IRefreshItemID
    {
        private int _baseItemID;

        protected int BaseItemID { get { return _baseItemID; } }
		
		public BaseGalleon Galleon { get; set; }
        
        protected BaseGalleonItem(BaseGalleon galleon, int northItemID)
            : this(galleon, northItemID, Point3D.Zero)
        {
        }

        protected BaseGalleonItem(BaseGalleon galleon, int northItemID, Point3D initOffset)
            : base(galleon, northItemID, initOffset)
        {
            _baseItemID = northItemID;
			Galleon = galleon;
        }

        public BaseGalleonItem(Serial serial)
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

            writer.Write((int)1);
			
            writer.Write((int)_baseItemID);
			writer.Write(Galleon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
			
            _baseItemID = reader.ReadInt();
			Galleon = reader.ReadItem() as BaseGalleon;
        }
        #endregion
    }

    public abstract class BaseGalleonContainer : BaseShipContainer, IRefreshItemID
    {
        private int _baseItemID;
		
		public BaseGalleon Galleon { get; set; }

        protected BaseGalleonContainer(BaseGalleon galleon, int northItemID, Point3D initOffset)
            : base(galleon, northItemID, initOffset)
        {
            _baseItemID = northItemID;
			Galleon = galleon;
        }

        public BaseGalleonContainer(Serial serial)
            : base(serial)
        { }

        public virtual void RefreshItemID(int itemIDModifier)
        {
            Ship.SetItemIDOnSmooth(this, itemIDModifier + _baseItemID);
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
			
            writer.Write((int)_baseItemID);
			writer.Write(Galleon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
			
            _baseItemID = reader.ReadInt();
			Galleon = reader.ReadItem() as BaseGalleon;
        }
        #endregion
    }

    public abstract class BaseGalleonMultiItem : BaseGalleonItem
    {
        private List<GalleonMultiComponent> _components;		

        protected BaseGalleonMultiItem(BaseGalleon galleon, int northItemId, Point3D initOffset)
            : base(galleon, northItemId, initOffset)
        {
            _components = new List<GalleonMultiComponent>();
        }

        public BaseGalleonMultiItem(Serial serial)
            : base(serial)
        {
        }

        public void AddComponent(GalleonMultiComponent comp)
        {
            _components.Add(comp);
        }

        public override void RefreshItemID(int itemIDModifier)
        {
            base.RefreshItemID(itemIDModifier);
            _components.ForEach(comp => comp.RefreshItemID(itemIDModifier));
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            _components.ForEach(comp => comp.Delete());
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((int)_components.Count);
            foreach (GalleonMultiComponent comp in _components)
                writer.Write((Item)comp);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();

            _components = new List<GalleonMultiComponent>();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
                _components.Add(reader.ReadItem() as GalleonMultiComponent);            
        }
        #endregion
    }

    public abstract class BaseGalleonMultiContainer : BaseGalleonContainer
    {
        private List<GalleonMultiComponent> _components;

        protected BaseGalleonMultiContainer(BaseGalleon galleon, int northItemId, Point3D initOffset)
            : base(galleon, northItemId, initOffset)
        {
            _components = new List<GalleonMultiComponent>();
        }

        public BaseGalleonMultiContainer(Serial serial)
            : base(serial)
        {
        }

        public void AddComponent(GalleonMultiComponent comp)
        {
            _components.Add(comp);
        }

        public override void RefreshItemID(int itemIDModifier)
        {
            base.RefreshItemID(itemIDModifier);
            foreach(GalleonMultiComponent component in _components)
                component.RefreshItemID(itemIDModifier);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            _components.ForEach(comp => comp.Delete());
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)_components.Count);
            _components.ForEach(comp => writer.Write((Item)comp));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt(); // version

            _components = new List<GalleonMultiComponent>();
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
                _components.Add(reader.ReadItem() as GalleonMultiComponent);
        }
        #endregion
    }

    public class GalleonMultiComponent : BaseGalleonItem
    {
        private Item _parentItem;

        public GalleonMultiComponent(int northItemId, BaseGalleonMultiItem parent, Point3D initOffSet)
            : base(parent.Galleon as BaseGalleon, northItemId)
        {
			Name = " ";
            _parentItem = parent;
            Location = new Point3D(parent.X + initOffSet.X, parent.Y + initOffSet.Y, parent.Z + initOffSet.Z);
        }

        public GalleonMultiComponent(int northItemId, BaseGalleonMultiContainer parent, Point3D initOffSet)
            : base(parent.Galleon as BaseGalleon, northItemId)
        {
			Name = " ";
            _parentItem = parent;
            Location = new Point3D(parent.X + initOffSet.X, parent.Y + initOffSet.Y, parent.Z + initOffSet.Z);
        }

        public GalleonMultiComponent(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (_parentItem != null)
                _parentItem.OnDoubleClick(from);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (_parentItem != null)
                _parentItem.Delete();
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((Item)_parentItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            _parentItem = reader.ReadItem();

            if (_parentItem == null)
                Delete();
        }
        #endregion
    }
}
