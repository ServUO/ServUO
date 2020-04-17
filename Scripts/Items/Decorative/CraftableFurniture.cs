using Server.Engines.Craft;
using System;

namespace Server.Items
{
    public class CraftableFurniture : Item, IResource, IQuality
    {
        public virtual bool ShowCrafterName => true;

        private Mobile m_Crafter;
        private CraftResource m_Resource;
        private ItemQuality m_Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality
        {
            get
            {
                return m_Quality;
            }
            set
            {
                m_Quality = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get
            {
                return m_Resource;
            }
            set
            {
                if (m_Resource != value)
                {
                    m_Resource = value;
                    Hue = CraftResources.GetHue(m_Resource);

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return m_Crafter;
            }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }

        public virtual bool PlayerConstructed => true;

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

            if (ShowCrafterName && m_Crafter != null)
                list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (m_Quality == ItemQuality.Exceptional)
                list.Add(1060636); // exceptional
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            CraftResourceInfo info = CraftResources.IsStandard(m_Resource) ? null : CraftResources.GetInfo(m_Resource);

            if (info != null && info.Number > 0)
                list.Add(info.Number);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            /* The jump to verison 1000 was due to needing to insert a class in the
			   inheritence chain for some items. We need to be certain that the new
			   version of CraftableFurniture that handles this data will not
			   conflict with the version numbers of the child classes.
			 */
            writer.Write(1000); // version

            writer.Write(m_Crafter);
            writer.Write((int)m_Resource);
            writer.Write((int)m_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.PeekInt();

            switch (version)
            {
                case 1000:
                    reader.ReadInt();
                    m_Crafter = reader.ReadMobile();
                    m_Resource = (CraftResource)reader.ReadInt();
                    m_Quality = (ItemQuality)reader.ReadInt();
                    break;
                case 0:
                    // Only these two items had this base class prior to the version change
                    if (this is ElvenPodium ||
                        this is GiantReplicaAcorn)
                    {
                        reader.ReadInt();
                        m_Crafter = reader.ReadMobile();
                        m_Resource = (CraftResource)reader.ReadInt();
                        m_Quality = (ItemQuality)reader.ReadInt();
                    }
                    // If we peeked a zero here any other way we should not consume data
                    else
                    {
                        m_Crafter = null;
                        m_Resource = CraftResource.None;
                        m_Quality = ItemQuality.Normal;
                    }
                    break;
                default:
                    throw new ArgumentException("Unhandled version number for CraftableFurniture");
            }
        }

        #region ICraftable
        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (makersMark)
                Crafter = from;

            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            Resource = CraftResources.GetFromType(resourceType);

            return quality;
        }
        #endregion
    }
}
