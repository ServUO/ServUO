using Server.Items;
using Server.Multis;
using Server.Network;
using System;

namespace Server.Engines.Plants
{
    [TypeAlias("Server.Engines.Plants.RaisedGardenPlantItem")]
    public class GardenBedPlantItem : PlantItem
    {
        public override bool RequiresUpkeep => false;
        public override int BowlOfDirtID => 2323;
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

        public override int ContainerLocalization => 1150436;  // mound
        public override int OnPlantLocalization => 1150442;  // You plant the seed in the mound of dirt.
        public override int CantUseLocalization => 1150511;  // That is not your gardening plot.

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
                    if (m_Component.X != X || m_Component.Y != Y || m_Component.Map != Map || m_Component.Deleted)
                        m_Component = null;
                }

                return m_Component;
            }
            set
            {
                m_Component = value;

                if (m_Component != null)
                {
                    if (m_Component.X != X || m_Component.Y != Y || m_Component.Map != Map || m_Component.Deleted)
                        m_Component = null;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool ValidGrowthLocation => RootParent == null && Component != null && !Movable && !Deleted;

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
                if (value && PlantSystem != null)
                {
                    PlantSystem.NextGrowth = DateTime.UtcNow;
                    PlantSystem.DoGrowthCheck();
                }
            }
        }

        [Constructable]
        public GardenBedPlantItem()
            : this(false)
        {
        }

        [Constructable]
        public GardenBedPlantItem(bool fertileDirt)
            : base(2323, fertileDirt)
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

            Point3D loc = GetWorldLocation();

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

        public GardenBedPlantItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Component);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            m_Component = reader.ReadItem() as GardenAddonComponent;
        }
    }
}
