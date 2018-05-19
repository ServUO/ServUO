using System;
using Server.Engines.Craft;
using Server.Multis;
using Server.Targeting;

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

        public BaseAddonContainerDeed()
            : base(0x14F0)
        {
            this.Weight = 1.0;

            if (!Core.AOS)
                this.LootType = LootType.Newbied;
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
            writer.Write((int)this.m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    this.m_Resource = (CraftResource)reader.ReadInt();
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!CraftResources.IsStandard(this.m_Resource))
                list.Add(CraftResources.GetLocalizationNumber(this.m_Resource));
        }

        #region ICraftable
        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            this.Resource = CraftResources.GetFromType(resourceType);

            return quality;
        }

        #endregion

        private class InternalTarget : Target
        {
            private readonly BaseAddonContainerDeed m_Deed;

            public InternalTarget(BaseAddonContainerDeed deed)
                : base(-1, true, TargetFlags.None)
            {
                this.m_Deed = deed;

                this.CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                Map map = from.Map;

                if (p == null || map == null || this.m_Deed.Deleted)
                    return;

                if (this.m_Deed.IsChildOf(from.Backpack))
                {
                    BaseAddonContainer addon = this.m_Deed.Addon;
                    addon.Resource = this.m_Deed.Resource;

                    Server.Spells.SpellHelper.GetSurfaceTop(ref p);

                    BaseHouse house = null;

                    AddonFitResult res = addon.CouldFit(p, map, from, ref house);

                    if (res == AddonFitResult.Valid)
                        addon.MoveToWorld(new Point3D(p), map);
                    else if (res == AddonFitResult.Blocked)
                        from.SendLocalizedMessage(500269); // You cannot build that there.
                    else if (res == AddonFitResult.NotInHouse)
                        from.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                    else if (res == AddonFitResult.DoorsNotClosed)
                        from.SendMessage("You must close all house doors before placing this.");
                    else if (res == AddonFitResult.DoorTooClose)
                        from.SendLocalizedMessage(500271); // You cannot build near the door.
                    else if (res == AddonFitResult.NoWall)
                        from.SendLocalizedMessage(500268); // This object needs to be mounted on something.

                    if (res == AddonFitResult.Valid)
                    {
                        this.m_Deed.Delete();
                        house.Addons[addon] = from;                        

                        if (addon is GardenShedAddon)
                        {
                            GardenShedAddon ad = addon as GardenShedAddon;
                            house.Addons[ad.SecondContainer] = from;
                        }

                        house.AddSecure(from, addon);
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