using Server.Engines.Craft;
using Server.Multis;
using Server.Targeting;
using System;

namespace Server.Items
{
    [Flipable(0x14F0, 0x14EF)]
    public abstract class BaseAddonDeed : Item, ICraftable
    {
        private CraftResource m_Resource;
        private bool m_ReDeed;

        public BaseAddonDeed()
            : base(0x14F0)
        {
            Weight = 1.0;
        }

        public BaseAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public abstract BaseAddon Addon { get; }

        public virtual bool UseCraftResource => true;

        public virtual bool ExcludeDeedHue => false;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get
            {
                return m_Resource;
            }
            set
            {
                if (UseCraftResource && m_Resource != value)
                {
                    m_Resource = value;
                    Hue = CraftResources.GetHue(m_Resource);

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsReDeed
        {
            get { return m_ReDeed; }
            set
            {
                m_ReDeed = value;

                if (UseCraftResource)
                {
                    if (m_ReDeed && ItemID == 0x14F0)
                    {
                        ItemID = 0x14EF;
                    }
                    else if (!m_ReDeed && ItemID == 0x14EF)
                    {
                        ItemID = 0x14F0;
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            // Version 2
            writer.Write(m_ReDeed);

            // Version 1
            writer.Write((int)m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        m_ReDeed = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }

            if (version == 1 && UseCraftResource && Hue == 0 && m_Resource != CraftResource.None)
            {
                Hue = CraftResources.GetHue(m_Resource);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public virtual void DeleteDeed()
        {
            Delete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!CraftResources.IsStandard(m_Resource))
                list.Add(CraftResources.GetLocalizationNumber(m_Resource));
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            Resource = CraftResources.GetFromType(resourceType);

            CraftContext context = craftSystem.GetContext(from);

            if (context != null && context.DoNotColor)
                Hue = 0;
            else if (Hue == 0)
                Hue = resHue;

            return quality;
        }

        private class InternalTarget : Target
        {
            private readonly BaseAddonDeed m_Deed;
            public InternalTarget(BaseAddonDeed deed)
                : base(-1, true, TargetFlags.None)
            {
                m_Deed = deed;
                CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                Map map = from.Map;

                if (p == null || map == null || m_Deed.Deleted)
                    return;

                if (m_Deed.IsChildOf(from.Backpack))
                {
                    BaseAddon addon = m_Deed.Addon;

                    Spells.SpellHelper.GetSurfaceTop(ref p);

                    BaseHouse house = null;
                    BaseGalleon galleon = CheckGalleonPlacement(from, addon, new Point3D(p), map);

                    AddonFitResult res = galleon != null ? AddonFitResult.Valid : addon.CouldFit(p, map, from, ref house);

                    if (res == AddonFitResult.Valid)
                    {
                        addon.Resource = m_Deed.Resource;

                        if (!m_Deed.ExcludeDeedHue)
                        {
                            if (addon.RetainDeedHue || (m_Deed.Hue != 0 && CraftResources.GetHue(m_Deed.Resource) != m_Deed.Hue))
                            {
                                addon.Hue = m_Deed.Hue;
                            }
                        }

                        addon.MoveToWorld(new Point3D(p), map);

                        if (house != null)
                        {
                            house.Addons[addon] = from;
                        }

                        if (galleon != null)
                        {
                            galleon.AddAddon(addon);
                        }

                        m_Deed.DeleteDeed();
                    }
                    else if (res == AddonFitResult.Blocked)
                    {
                        from.SendLocalizedMessage(500269); // You cannot build that there.
                    }
                    else if (res == AddonFitResult.NotInHouse)
                    {
                        from.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                    }
                    else if (res == AddonFitResult.OwnerNotInHouse)
                    {
                        from.SendLocalizedMessage(1153770); // The deed is not in the same house as you.
                    }
                    else if (res == AddonFitResult.DoorTooClose)
                    {
                        from.SendLocalizedMessage(500271); // You cannot build near the door.
                    }
                    else if (res == AddonFitResult.NoWall)
                    {
                        from.SendLocalizedMessage(500268); // This object needs to be mounted on something.
                    }
                    else if (res == AddonFitResult.FoundationStairs)
                    {
                        from.SendLocalizedMessage(1071262); // You can't place the multi-tile addon at the entrance!
                    }
                    else if (res == AddonFitResult.InternalStairs)
                    {
                        from.SendLocalizedMessage(1152735); // The targeted location has at least one impassable tile adjacent to the structure.
                        from.SendLocalizedMessage(500277); // Construction aborted. Please try again.
                    }

                    if (res != AddonFitResult.Valid)
                    {
                        addon.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
            }

            public BaseGalleon CheckGalleonPlacement(Mobile from, BaseAddon addon, Point3D p, Map map)
            {
                if (addon.Components.Count > 1)
                {
                    return null;
                }

                BaseGalleon galleon = BaseGalleon.FindGalleonAt(p, map);

                if (galleon != null && galleon.CanAddAddon(p))
                {
                    return galleon;
                }

                return null;
            }
        }
    }
}
