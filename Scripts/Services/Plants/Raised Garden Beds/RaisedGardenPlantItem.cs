using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Network;
using Server.Gumps;

namespace Server.Engines.Plants
{
    public class RaisedGardenPlantItem : PlantItem
    {
        public override bool RequiresUpkeep { get { return false; } }
        public override int BowlOfDirtID { get { return 2323; } }
        public override int GreenBowlID
        {
            get
            {
                if (PlantStatus <= PlantStatus.Stage3)
                    return 0xC7E;
                else
                    return 0xC62;
            }
        }

        public override int ContainerLocalization { get { return 1150436; } } // mound
        public override int OnPlantLocalization { get { return 1150442; } } // You plant the seed in the mound of dirt.
        public override int CantUseLocalization { get { return 1150511; } } // That is not your gardening plot.

        public override int LabelNumber
        {
            get
            {
                int label = base.LabelNumber;

                if (label == 1029913)
                    label = 1022321;   // dirt patch

                return label;
            }
        }

        private GardenAddonComponent m_Component;

        [CommandProperty(AccessLevel.GameMaster)]
        public GardenAddonComponent Component 
        { 
            get 
            {
                if (m_Component != null)
                {
                    if (m_Component.X != this.X || m_Component.Y != this.Y || m_Component.Map != this.Map || m_Component.Deleted)
                        m_Component = null;
                }

                return m_Component;
            }
            set 
            { 
                m_Component = value;

                if (m_Component != null)
                {
                    if (m_Component.X != this.X || m_Component.Y != this.Y || m_Component.Map != this.Map || m_Component.Deleted)
                        m_Component = null;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool ValidGrowthLocation
        {
            get
            {
                return RootParent == null && this.Component != null && !this.Movable && !this.Deleted;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextGrowth
        {
            get
            {
                if (PlantSystem != null)
                    return PlantSystem.NextGrowth;
                return DateTime.MinValue;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlantGrowthIndicator GrowthIndicator
        {
            get
            {
                if (PlantSystem != null)
                    return PlantSystem.GrowthIndicator;
                return PlantGrowthIndicator.None;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForceGrow
        {
            get { return false; }
            set
            {
                if (value = true && PlantSystem != null)
                {
                    PlantSystem.NextGrowth = DateTime.UtcNow;
                    PlantSystem.DoGrowthCheck();
                }
            }
        }

        [Constructable]
        public RaisedGardenPlantItem() : this(false) 
        { 
        }

        [Constructable]
        public RaisedGardenPlantItem(bool fertileDirt) : base(2323, fertileDirt)
        {
            Movable = false;
        }

        public override void Delete()
        {
            if (m_Component != null && m_Component.Plant == this)
                m_Component.Plant = null;

            base.Delete();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (PlantStatus >= PlantStatus.DecorativePlant)
                return;

            Point3D loc = this.GetWorldLocation();

            if (!from.InLOS(loc) || !from.InRange(loc, 4))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1019045); // I can't reach that.
                return;
            }

            if (!IsUsableBy(from))
            {
                LabelTo(from, 1150327); // Only the house owner and co-owners can use the raised garden bed.

                return;
            }

            from.SendGump(new MainPlantGump(this));
        }

        public override bool IsUsableBy(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);
            return house != null && house.IsCoOwner(from) && IsAccessibleTo(from);
        }

        /*public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new RemovePlantEntry(this, IsUsableBy(from)));
        }

        private class RemovePlantEntry : ContextMenuEntry
        {
            private RaisedGardenPlantItem m_Patch;

            public RemovePlantEntry(RaisedGardenPlantItem patch, bool enabled) : base(1150324, 3)
            {
                m_Patch = patch;

                if (!enabled)
                    Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if (m_Patch.Deleted)
                    return;

                Mobile from = Owner.From;

                if (from.CheckAlive())
                {
                    from.SendLocalizedMessage(1150325); // You have removed the plant from the raised garden bed.

                    m_Patch.Movable = true;

                    if (from.Backpack != null)
                        from.Backpack.TryDropItem(from, m_Patch, false);
                }
            }
        }*/

        public RaisedGardenPlantItem( Serial serial ) : base( serial )
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(m_Component);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Component = reader.ReadItem() as GardenAddonComponent;
        }
    }
}