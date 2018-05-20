using System;
using Server.Engines.Craft;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    [Flipable(0x14F0, 0x14EF)]
    public abstract class BaseAddonDeed : Item, ICraftable
    {
        private CraftResource m_Resource;

        public BaseAddonDeed()
            : base(0x14F0)
        {
            Weight = 1.0;

            if (!Core.AOS)
                LootType = LootType.Newbied;
        }

        public BaseAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public abstract BaseAddon Addon { get; }
        public virtual bool UseCraftResource { get { return true; } }

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
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            // Version 1
            writer.Write((int)m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }

            if (Weight == 0.0)
                Weight = 1.0;
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

                    Server.Spells.SpellHelper.GetSurfaceTop(ref p);

                    BaseHouse house = null;
                    BaseGalleon boat = null;

                    AddonFitResult res = addon.CouldFit(p, map, from, ref house, ref boat);

                    if (res == AddonFitResult.Valid)
                    {
                        addon.Resource = m_Deed.Resource;

                        if (addon.RetainDeedHue)
                            addon.Hue = m_Deed.Hue;

                        addon.MoveToWorld(new Point3D(p), map);

                        if (house != null)
                            house.Addons[addon] = from;
                        else if (boat != null)
                            boat.AddAddon(addon);

                        m_Deed.DeleteDeed();
                    }
                    else if (res == AddonFitResult.Blocked)
                        from.SendLocalizedMessage(500269); // You cannot build that there.
                    else if (res == AddonFitResult.NotInHouse)
                        from.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                    else if (res == AddonFitResult.DoorTooClose)
                        from.SendLocalizedMessage(500271); // You cannot build near the door.
                    else if (res == AddonFitResult.NoWall)
                        from.SendLocalizedMessage(500268); // This object needs to be mounted on something.
					
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
        }
    }
}