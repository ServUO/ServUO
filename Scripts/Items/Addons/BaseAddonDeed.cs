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
            this.Weight = 1.0;

            if (!Core.AOS)
                this.LootType = LootType.Newbied;
        }

        public BaseAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public abstract BaseAddon Addon { get; }
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
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            // Version 1
            writer.Write((int)this.m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        this.m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }

            if (this.Weight == 0.0)
                this.Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public virtual void DeleteDeed()
        {
            this.Delete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!CraftResources.IsStandard(this.m_Resource))
                list.Add(CraftResources.GetLocalizationNumber(this.m_Resource));
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            this.Resource = CraftResources.GetFromType(resourceType);

            CraftContext context = craftSystem.GetContext(from);

			if (context != null && context.DoNotColor)
				this.Hue = 0;
			else
				this.Hue = resHue;

            return quality;
        }

        private class InternalTarget : Target
        {
            private readonly BaseAddonDeed m_Deed;
            public InternalTarget(BaseAddonDeed deed)
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
                    BaseAddon addon = this.m_Deed.Addon;

                    Server.Spells.SpellHelper.GetSurfaceTop(ref p);

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
                        if (addon != null)
                        {
                            addon.Resource = this.m_Deed.Resource;

                            if (addon.RetainDeedHue)
                                addon.Hue = this.m_Deed.Hue;
                        }

                        this.m_Deed.DeleteDeed();

                        house.Addons.Add(addon);
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