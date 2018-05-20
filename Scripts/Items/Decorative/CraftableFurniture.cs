using System;
using Server.Engines.Craft;
using Server.Mobiles;

namespace Server.Items
{
    public enum ItemQuality
    {
        Low,
        Normal,
        Exceptional,
    }

    public class CraftableFurniture : Item, IResource
    {
        public virtual bool ShowCrafterName
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

        public virtual bool PlayerConstructed { get { return true; } }

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

            if (this.ShowCrafterName && this.m_Crafter != null)
				list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

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
		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (m_Crafter != null)
			{
				LabelTo(from, 1050043, m_Crafter.TitleName); // crafted by ~1_NAME~
			}
		}
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

			/* The jump to verison 1000 was due to needing to insert a class in the
			   inheritence chain for some items. We need to be certain that the new
			   version of CraftableFurniture that handles this data will not
			   conflict with the version numbers of the child classes.
			 */
            writer.Write((int)1000); // version

            writer.Write((Mobile)this.m_Crafter);
            writer.Write((int)this.m_Resource);
            writer.Write((int)this.m_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

			int version = reader.PeekInt();

			switch (version)
			{
				case 1000:
					reader.ReadInt();
					this.m_Crafter = reader.ReadMobile();
					this.m_Resource = (CraftResource)reader.ReadInt();
					this.m_Quality = (ItemQuality)reader.ReadInt();
					break;
				case 0:
					// Only these two items had this base class prior to the version change
					if(this is ElvenPodium ||
						this is GiantReplicaAcorn)
					{
						reader.ReadInt();
						this.m_Crafter = reader.ReadMobile();
						this.m_Resource = (CraftResource)reader.ReadInt();
						this.m_Quality = (ItemQuality)reader.ReadInt();
					}
					// If we peeked a zero here any other way we should not consume data
					else
					{
						this.m_Crafter = null;
						this.m_Resource = CraftResource.None;
						this.m_Quality = ItemQuality.Normal;
					}
					break;
				default:
					throw new ArgumentException("Unhandled version number for CraftableFurniture");
			}
        }

        #region ICraftable
        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            this.Quality = (ItemQuality)quality;

            if (makersMark)
                this.Crafter = from;

            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            this.Resource = CraftResources.GetFromType(resourceType);

            return quality;
        }
        #endregion
    }
}