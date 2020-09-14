using Server.Engines.Craft;
using Server.Multis;
using Server.Targeting;
using System;

namespace Server.Items
{
    [Flipable(0x14F0, 0x14EF)]
    public abstract class BaseAddonContainerDeed : Item, ICraftable
    {
        public abstract BaseAddonContainer Addon { get; }

        private CraftResource m_Resource;

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

        public BaseAddonContainerDeed()
            : base(0x14F0)
        {
            Weight = 1.0;
        }

        public BaseAddonContainerDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            // version 1
            writer.Write((int)m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Resource = (CraftResource)reader.ReadInt();
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!CraftResources.IsStandard(m_Resource))
                list.Add(CraftResources.GetLocalizationNumber(m_Resource));
        }

        #region ICraftable
        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            Resource = CraftResources.GetFromType(resourceType);

            CraftContext context = craftSystem.GetContext(from);

            if (context != null && context.DoNotColor)
                Hue = 0;

            return quality;
        }

        #endregion

        private class InternalTarget : Target
        {
            private readonly BaseAddonContainerDeed m_Deed;

            public InternalTarget(BaseAddonContainerDeed deed)
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
                    BaseAddonContainer addon = m_Deed.Addon;
                    addon.Resource = m_Deed.Resource;

                    Spells.SpellHelper.GetSurfaceTop(ref p);

                    BaseHouse house = null;

                    AddonFitResult res = addon.CouldFit(p, map, from, ref house);

                    if (res == AddonFitResult.Valid)
                        addon.MoveToWorld(new Point3D(p), map);
                    else if (res == AddonFitResult.Blocked)
                        from.SendLocalizedMessage(500269); // You cannot build that there.
                    else if (res == AddonFitResult.NotInHouse)
                        from.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                    else if (res == AddonFitResult.DoorTooClose)
                        from.SendLocalizedMessage(500271); // You cannot build near the door.
                    else if (res == AddonFitResult.NoWall)
                        from.SendLocalizedMessage(500268); // This object needs to be mounted on something.

                    if (res == AddonFitResult.Valid)
                    {
                        m_Deed.Delete();
                        house.Addons[addon] = from;

                        if (addon is GardenShedAddon)
                        {
                            GardenShedAddon ad = addon as GardenShedAddon;
                            house.Addons[ad.SecondContainer] = from;
                        }

                        if (addon.Security)
                        {
                            house.AddSecure(from, addon);
                        }
                    }
                    else
                    {
                        addon.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
            }
        }
    }    
}
