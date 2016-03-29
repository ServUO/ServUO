using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Items
{
    public class FlourMillSouthAddon : BaseAddon, IFlourMill
    {
        private static readonly int[][] m_StageTable = new int[][]
        {
            new int[] { 0x192C, 0x192D, 0x1931 },
            new int[] { 0x192E, 0x192F, 0x1932 },
            new int[] { 0x1930, 0x1930, 0x1934 }
        };
        private int m_Flour;
        private Timer m_Timer;
        [Constructable]
        public FlourMillSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x192C), 0, -1, 0);
            this.AddComponent(new AddonComponent(0x192E), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x1930), 0, 1, 0);
        }

        public FlourMillSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new FlourMillSouthDeed();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxFlour
        {
            get
            {
                return 2;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int CurFlour
        {
            get
            {
                return this.m_Flour;
            }
            set
            {
                this.m_Flour = Math.Max(0, Math.Min(value, this.MaxFlour));
                this.UpdateStage();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasFlour
        {
            get
            {
                return (this.m_Flour > 0);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsFull
        {
            get
            {
                return (this.m_Flour >= this.MaxFlour);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsWorking
        {
            get
            {
                return (this.m_Timer != null);
            }
        }
        public void StartWorking(Mobile from)
        {
            if (this.IsWorking)
                return;

            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerStateCallback(FinishWorking_Callback), from);
            this.UpdateStage();
        }

        public void UpdateStage()
        {
            if (this.IsWorking)
                this.UpdateStage(FlourMillStage.Working);
            else if (this.HasFlour)
                this.UpdateStage(FlourMillStage.Filled);
            else
                this.UpdateStage(FlourMillStage.Empty);
        }

        public void UpdateStage(FlourMillStage stage)
        {
            List<AddonComponent> components = this.Components;

            int[][] stageTable = m_StageTable;

            for (int i = 0; i < components.Count; ++i)
            {
                AddonComponent component = components[i] as AddonComponent;

                if (component == null)
                    continue;

                int[] itemTable = this.FindItemTable(component.ItemID);

                if (itemTable != null)
                    component.ItemID = itemTable[(int)stage];
            }
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 4) || !from.InLOS(this))
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else if (!this.IsFull)
                from.SendLocalizedMessage(500997); // You need more wheat to make a sack of flour.
            else
                this.StartWorking(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_Flour);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Flour = reader.ReadInt();
                        break;
                    }
            }

            this.UpdateStage();
        }

        private void FinishWorking_Callback(object state)
        {
            if (this.m_Timer != null)
            {
                this.m_Timer.Stop();
                this.m_Timer = null;
            }

            Mobile from = state as Mobile;

            if (from != null && !from.Deleted && !this.Deleted && this.IsFull)
            {
                SackFlour flour = new SackFlour();

                flour.ItemID = (Utility.RandomBool() ? 4153 : 4165);

                if (from.PlaceInBackpack(flour))
                {
                    this.m_Flour = 0;
                }
                else
                {
                    flour.Delete();
                    from.SendLocalizedMessage(500998); // There is not enough room in your backpack!  You stop grinding.
                }
            }

            this.UpdateStage();
        }

        private int[] FindItemTable(int itemID)
        {
            for (int i = 0; i < m_StageTable.Length; ++i)
            {
                int[] itemTable = m_StageTable[i];

                for (int j = 0; j < itemTable.Length; ++j)
                {
                    if (itemTable[j] == itemID)
                        return itemTable;
                }
            }

            return null;
        }
    }

    public class FlourMillSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public FlourMillSouthDeed()
        {
        }

        public FlourMillSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FlourMillSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044348;
            }
        }// flour mill (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}