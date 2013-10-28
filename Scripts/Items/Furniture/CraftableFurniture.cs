using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public enum ItemQuality
    {
        Low,
        Normal,
        Exceptional,
    }

    public class CraftableFurniture : Item, ICraftable
    {
        public virtual bool ShowCraferName
        {
            get
            {
                return true;
            }
        }

        private Mobile m_Crafter;
        private CraftResource m_Resource;
        private ItemQuality m_Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality
        {
            get
            {
                return this.m_Quality;
            }
            set
            {
                this.m_Quality = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get
            {
                return this.m_Resource;
            }
            set
            {
                if (this.m_Resource != value)
                {
                    this.m_Resource = value;
                    this.Hue = CraftResources.GetHue(this.m_Resource);
					
                    this.InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return this.m_Crafter;
            }
            set
            {
                this.m_Crafter = value;
                this.InvalidateProperties();
            }
        }
	
        public CraftableFurniture(int itemID)
            : base(itemID)
        {
        }

        public CraftableFurniture(Serial serial)
            : base(serial)
        {
        }
		
        public override void AddWeightProperty(ObjectPropertyList list)
        {
            base.AddWeightProperty(list);

            if (this.ShowCraferName && this.m_Crafter != null)
                list.Add(1050043, this.m_Crafter.Name); // crafted by ~1_NAME~

            if (this.m_Quality == ItemQuality.Exceptional)
                list.Add(1060636); // exceptional
        }
		
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            CraftResourceInfo info = CraftResources.IsStandard(this.m_Resource) ? null : CraftResources.GetInfo(this.m_Resource);

            if (info != null && info.Number > 0)
                list.Add(info.Number);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((Mobile)this.m_Crafter);
            writer.Write((int)this.m_Resource);
            writer.Write((int)this.m_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Crafter = reader.ReadMobile();
            this.m_Resource = (CraftResource)reader.ReadInt();
            this.m_Quality = (ItemQuality)reader.ReadInt();
        }

        #region ICraftable
        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            this.Quality = (ItemQuality)quality;

            if (makersMark)
                this.Crafter = from;

            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            this.Resource = CraftResources.GetFromType(resourceType);

            CraftContext context = craftSystem.GetContext(from);

            if (context != null && context.DoNotColor)
                this.Hue = 0;

            return quality;
        }
        #endregion
    }
}