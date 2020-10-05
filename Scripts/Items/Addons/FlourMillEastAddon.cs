using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public enum FlourMillStage
    {
        Empty,
        Filled,
        Working
    }

    public interface IFlourMill
    {
        int MaxFlour { get; }
        int CurFlour { get; set; }
    }

    public class FlourMillEastAddon : BaseAddon, IFlourMill
    {
        private static readonly int[][] m_StageTable = new int[][]
        {
            new int[] { 0x1920, 0x1921, 0x1925 },
            new int[] { 0x1922, 0x1923, 0x1926 },
            new int[] { 0x1924, 0x1924, 0x1928 }
        };
        private int m_Flour;
        private Timer m_Timer;
        [Constructable]
        public FlourMillEastAddon()
        {
            AddComponent(new AddonComponent(0x1920), -1, 0, 0);
            AddComponent(new AddonComponent(0x1922), 0, 0, 0);
            AddComponent(new AddonComponent(0x1924), 1, 0, 0);
        }

        public FlourMillEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new FlourMillEastDeed();
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxFlour => 2;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CurFlour
        {
            get
            {
                return m_Flour;
            }
            set
            {
                m_Flour = Math.Max(0, Math.Min(value, MaxFlour));
                UpdateStage();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasFlour => (m_Flour > 0);
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsFull => (m_Flour >= MaxFlour);
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsWorking => (m_Timer != null);
        public void StartWorking(Mobile from)
        {
            if (IsWorking)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerStateCallback(FinishWorking_Callback), from);
            UpdateStage();
        }

        public void UpdateStage()
        {
            if (IsWorking)
                UpdateStage(FlourMillStage.Working);
            else if (HasFlour)
                UpdateStage(FlourMillStage.Filled);
            else
                UpdateStage(FlourMillStage.Empty);
        }

        public void UpdateStage(FlourMillStage stage)
        {
            List<AddonComponent> components = Components;

            for (int i = 0; i < components.Count; ++i)
            {
                AddonComponent component = components[i] as AddonComponent;

                if (component == null)
                    continue;

                int[] itemTable = FindItemTable(component.ItemID);

                if (itemTable != null)
                    component.ItemID = itemTable[(int)stage];
            }
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 4) || !from.InLOS(this))
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else if (!IsFull)
                from.SendLocalizedMessage(500997); // You need more wheat to make a sack of flour.
            else
                StartWorking(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(m_Flour);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_Flour = reader.ReadInt();
                        break;
                    }
            }

            UpdateStage();
        }

        private void FinishWorking_Callback(object state)
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            Mobile from = state as Mobile;

            if (from != null && !from.Deleted && !Deleted && IsFull)
            {
                SackFlour flour = new SackFlour
                {
                    ItemID = (Utility.RandomBool() ? 4153 : 4165)
                };

                if (from.PlaceInBackpack(flour))
                {
                    m_Flour = 0;
                }
                else
                {
                    flour.Delete();
                    from.SendLocalizedMessage(500998); // There is not enough room in your backpack!  You stop grinding.
                }
            }

            UpdateStage();
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

    public class FlourMillEastDeed : BaseAddonDeed
    {
        [Constructable]
        public FlourMillEastDeed()
        {
        }

        public FlourMillEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new FlourMillEastAddon();
        public override int LabelNumber => 1044347;// flour mill (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
