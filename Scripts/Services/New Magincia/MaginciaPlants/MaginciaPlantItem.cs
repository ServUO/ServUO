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
    public class MaginciaPlantItem : PlantItem
    {
        public override bool MaginciaPlant { get { return true; } }
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

        public override int ContainerLocalization { get { return 1150436; } } // mound of dirt
        public override int OnPlantLocalization { get { return 1150442; } } // You plant the seed in the mound of dirt.
        public override int CantUseLocalization { get { return 1150511; } } // That is not your gardening plot.

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

        private Mobile m_Owner;
        private DateTime m_Planted;
        private DateTime m_SetToDecorative;
        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Planted { get { return m_Planted; } set { m_Planted = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SetToDecorative { get { return m_SetToDecorative; } set { m_SetToDecorative = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool ValidGrowthLocation
        {
            get
            {
                return RootParent == null && !Movable;
            }
        }

        [Constructable]
        public MaginciaPlantItem() : this(false)
        {
        }

        [Constructable]
        public MaginciaPlantItem(bool fertile) : base(2323, fertile)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (PlantStatus >= PlantStatus.DecorativePlant)
                return;

            Point3D loc = this.GetWorldLocation();

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
            if (PlantStatus == PlantStatus.BowlOfDirt)
                return true;

            return RootParent == null && !Movable && m_Owner == from && IsAccessibleTo(from);
        }

        public override void Die()
        {
            base.Die();

            Timer.DelayCall(TimeSpan.FromMinutes(Utility.RandomMinMax(2, 5)), new TimerCallback(Delete));
        }

        public override void Delete()
        {
            if (m_Owner != null && PlantStatus < PlantStatus.DecorativePlant)
                MaginciaPlantSystem.OnPlantDelete(this.Owner, this.Map);

            base.Delete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Owner != null && PlantStatus > PlantStatus.BowlOfDirt)
            {
                list.Add(1150474, String.Format("{0}\t{1}", "New Magincia", m_Owner.Name)); // Planted in ~1_val~ by: ~2_val~
                list.Add(1150478, m_Planted.ToShortDateString());

                if(this.PlantStatus == PlantStatus.DecorativePlant)
                    list.Add(1150490, m_SetToDecorative.ToShortDateString()); // Date harvested: ~1_val~
            }
        }

        public void StartTimer()
        {
            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(10), new TimerCallback(Delete));
        }

        public override bool PlantSeed(Mobile from, Seed seed)
        {
            if (!MaginciaPlantSystem.CheckDelay(from) || !CheckLocation(from, seed) || !base.PlantSeed(from, seed))
                return false;

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            Owner = from;
            Planted = DateTime.UtcNow;

            MaginciaPlantSystem.OnPlantPlanted(from, from.Map);

            return true;
        }

        private bool CheckLocation(Mobile from, Seed seed)
        {
            if(!BlocksMovement(seed))
                return true;

            IPooledEnumerable eable = this.Map.GetItemsInRange(this.Location, 1);

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

        public MaginciaPlantItem( Serial serial ) : base( serial )
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(m_Owner);
            writer.Write(m_Planted);
            writer.Write(m_SetToDecorative);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Owner = reader.ReadMobile();
            m_Planted = reader.ReadDateTime();
            m_SetToDecorative = reader.ReadDateTime();

            if (m_Owner == null)
                Delete();
        }
    }
}