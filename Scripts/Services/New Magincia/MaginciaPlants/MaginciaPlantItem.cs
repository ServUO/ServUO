using Server.Network;
using System;

namespace Server.Engines.Plants
{
    public class MaginciaPlantItem : PlantItem
    {
        public override bool MaginciaPlant => true;
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

        public override int ContainerLocalization => 1150436;  // mound of dirt
        public override int OnPlantLocalization => 1150442;  // You plant the seed in the mound of dirt.
        public override int CantUseLocalization => 501648;  // You cannot use this unless you are the owner.

        public override int LabelNumber
        {
            get
            {
                int label = base.LabelNumber;

                if (label == 1029913)
                    label = 1022321;    // patch of dirt

                return label;
            }
        }

        private DateTime m_Planted;
        private DateTime m_Contract;
        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Planted { get { return m_Planted; } set { m_Planted = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ContractTime { get { return m_Contract; } set { m_Contract = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ContractEndTime => ContractTime + TimeSpan.FromDays(14);

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsContract => ContractEndTime > DateTime.UtcNow;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SetToDecorative { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool ValidGrowthLocation => RootParent == null && !Movable;

        [Constructable]
        public MaginciaPlantItem()
            : this(false)
        {
        }

        [Constructable]
        public MaginciaPlantItem(bool fertile)
            : base(2323, fertile)
        {
            Movable = false;

            Planted = DateTime.UtcNow;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (PlantStatus >= PlantStatus.DecorativePlant)
                return;

            Point3D loc = GetWorldLocation();

            if (!from.InLOS(loc) || !from.InRange(loc, 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1019045); // I can't reach that.
                return;
            }

            if (!IsUsableBy(from))
            {
                LabelTo(from, CantUseLocalization);

                return;
            }

            from.SendGump(new MainPlantGump(this));
        }

        public override bool IsUsableBy(Mobile from)
        {
            return RootParent == null && !Movable && Owner == from && IsAccessibleTo(from);
        }

        public override void Die()
        {
            base.Die();

            Timer.DelayCall(TimeSpan.FromMinutes(Utility.RandomMinMax(2, 5)), Delete);
        }

        public override void Delete()
        {
            if (Owner != null && PlantStatus < PlantStatus.DecorativePlant)
                MaginciaPlantSystem.OnPlantDelete(Owner, Map);

            base.Delete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Owner != null)
            {
                list.Add(1150474, string.Format("{0}\t{1}", "#1011345", Owner.Name)); // Planted in ~1_val~ by: ~2_val~
                list.Add(1150478, m_Planted.ToShortDateString());

                if (IsContract)
                {
                    DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(ContractEndTime, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                    list.Add(1155763, easternTime.ToString("MM-dd-yyyy HH:mm 'ET'")); // Gardening Contract Expires: ~1_TIME~
                }

                if (PlantStatus == PlantStatus.DecorativePlant)
                    list.Add(1150490, SetToDecorative.ToShortDateString()); // Date harvested: ~1_val~
            }
        }

        public void StartTimer()
        {
            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(2), Delete);
        }

        public override bool PlantSeed(Mobile from, Seed seed)
        {
            if (!CheckLocation(from, seed) || !base.PlantSeed(from, seed))
                return false;

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            return true;
        }

        private bool CheckLocation(Mobile from, Seed seed)
        {
            if (!BlocksMovement(seed))
                return true;

            IPooledEnumerable eable = Map.GetItemsInRange(Location, 1);

            foreach (Item item in eable)
            {
                if (item != this && item is MaginciaPlantItem)
                {
                    if (((MaginciaPlantItem)item).BlocksMovement())
                    {
                        eable.Free();
                        from.SendLocalizedMessage(1150434); // Plants that block movement cannot be planted next to other plants that block movement.
                        return false;
                    }
                }
            }

            eable.Free();
            return true;
        }

        public bool BlocksMovement()
        {
            if (PlantStatus == PlantStatus.BowlOfDirt || PlantStatus == PlantStatus.DeadTwigs)
                return false;

            PlantTypeInfo info = PlantTypeInfo.GetInfo(PlantType);
            ItemData data = TileData.ItemTable[info.ItemID & TileData.MaxItemValue];

            TileFlag flags = data.Flags;

            return (flags & TileFlag.Impassable) > 0;
        }

        public static bool BlocksMovement(Seed seed)
        {
            PlantTypeInfo info = PlantTypeInfo.GetInfo(seed.PlantType);
            ItemData data = TileData.ItemTable[info.ItemID & TileData.MaxItemValue];

            TileFlag flags = data.Flags;

            return (flags & TileFlag.Impassable) > 0;
        }

        public MaginciaPlantItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write(ContractTime);
            writer.Write(Owner);
            writer.Write(m_Planted);
            writer.Write(SetToDecorative);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        ContractTime = reader.ReadDateTime();
                        Owner = reader.ReadMobile();
                        m_Planted = reader.ReadDateTime();
                        SetToDecorative = reader.ReadDateTime();
                        break;
                    }
                case 0:
                    {
                        Owner = reader.ReadMobile();
                        m_Planted = reader.ReadDateTime();
                        SetToDecorative = reader.ReadDateTime();
                        break;
                    }
            }

            if (PlantStatus == PlantStatus.BowlOfDirt)
                Delete();
        }
    }
}
