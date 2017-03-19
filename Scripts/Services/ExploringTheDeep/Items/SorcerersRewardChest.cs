using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public class SorcerersRewardChest : LockableContainer
    {
        public override int LabelNumber { get { return 1023712; } } // strong box
        private short m_MaxSpawnTime = 10;
        private short m_MinSpawnTime = 5;
        private ResetTimer m_ResetTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public short MaxSpawnTime
        {
            get { return this.m_MaxSpawnTime; }
            set { this.m_MaxSpawnTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public short MinSpawnTime
        {
            get { return this.m_MinSpawnTime; }
            set { this.m_MinSpawnTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool Locked
        {
            get { return base.Locked; }
            set
            {
                if (base.Locked != value)
                {
                    base.Locked = value;

                    if (!value)
                        this.StartResetTimer();
                }
            }
        }
        public override bool IsDecoContainer { get { return false; } }

        [Constructable]
        public SorcerersRewardChest() : base(0x9AA)
        {
            this.Locked = true;
            this.Movable = false;
            this.Hue = 1912;
            TrapType = TrapType.MagicTrap;
            TrapPower = 4 * Utility.Random(1, 25);
            this.GenerateTreasure();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.Locked)
            {
                from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154226); // *It's an unassuming strong box. You examine the lock more closely and determine there is no way to pick it. You'll need to find a key.*
                return;
            }

            base.OnDoubleClick(from);
        }

        public SorcerersRewardChest(Serial serial) : base(serial)
        {
        }

        public override void OnTelekinesis(Mobile from)
        {
            from.SendLocalizedMessage(501857); // This spell won't work on that!
            return;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(this.m_MinSpawnTime);
            writer.Write(this.m_MaxSpawnTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_MinSpawnTime = reader.ReadShort();
            this.m_MaxSpawnTime = reader.ReadShort();

            if (!this.Locked)
                this.StartResetTimer();
        }

        public void ClearContents()
        {
            for (int i = this.Items.Count - 1; i >= 0; --i)
            {
                if (i < this.Items.Count)
                    this.Items[i].Delete();
            }
        }

        public void Reset()
        {
            if (this.m_ResetTimer != null)
            {
                if (this.m_ResetTimer.Running)
                    this.m_ResetTimer.Stop();
            }

            this.Locked = true;
            this.ClearContents();
            this.GenerateTreasure();
        }

        protected virtual void GenerateTreasure()
        {
            DropItem(new Gold(200, 400));
            DropItem(new BlankScroll(Utility.Random(1, 4)));

            for (int i = Utility.Random(1, 4); i > 1; i--)
            {
                Item ReagentLoot = Loot.RandomReagent();
                ReagentLoot.Amount = Utility.Random(6, 12);
                DropItem(ReagentLoot);
            }

            for (int i = Utility.Random(1, 4); i > 1; i--)
                DropItem(Loot.RandomPotion());

            if (0.75 > Utility.RandomDouble()) //75% chance = 3/4
                for (int i = Utility.RandomMinMax(8, 16); i > 0; i--)
                    DropItem(Loot.RandomScroll(0, 47, SpellbookType.Regular));

            if (0.75 > Utility.RandomDouble()) //75% chance = 3/4
                for (int i = Utility.RandomMinMax(6, 12) + 1; i > 0; i--)
                    DropItem(Loot.RandomGem());

            for (int i = Utility.Random(1, 4); i > 1; i--)
                DropItem(Loot.RandomWand());


            for (int i = Utility.Random(3) + 1; i > 0; i--) // random 1 to 3
            {
                switch (Utility.Random(3))
                {
                    case 0: DropItem(new Bolt(10)); break;
                    case 1: DropItem(new Bandage(TrapPower / 5)); break;
                    case 2: DropItem(new HealPotion(TrapPower / 5)); break;
                }
            }
        }

        private void StartResetTimer()
        {
            if (this.m_ResetTimer == null)
                this.m_ResetTimer = new ResetTimer(this);
            else
                this.m_ResetTimer.Delay = TimeSpan.FromMinutes(Utility.Random(this.m_MinSpawnTime, this.m_MaxSpawnTime));

            this.m_ResetTimer.Start();
        }

        private class ResetTimer : Timer
        {
            private readonly SorcerersRewardChest m_Chest;
            public ResetTimer(SorcerersRewardChest chest)
                : base(TimeSpan.FromMinutes(Utility.Random(chest.MinSpawnTime, chest.MaxSpawnTime)))
            {
                this.m_Chest = chest;
                this.Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                this.m_Chest.Reset();
            }
        }
    }

    public class StrongboxKey : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154227; } } // Strongbox Key
        

        [Constructable]
        public StrongboxKey() : base(0x410A)
        {
            Stackable = false;
            Weight = 0.01;
            Hue = 2721;
            LootType = LootType.Blessed;
        }

        public override int Lifespan { get { return 3600; } }
        public override bool UseSeconds { get { return false; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public StrongboxKey(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile))
                return;

            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1054107); // This item must be in your backpack.
                return;
            }

            from.Target = new ChestTarget(from, this);
        }

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

    public class ChestTarget : Target
    {
        private static Mobile m_From;
        private static StrongboxKey m_Key;

        public ChestTarget(Mobile from, StrongboxKey key) : base(2, false, TargetFlags.None)
        {
            m_From = from;
            m_Key = key;
        }

        protected override void OnTarget(Mobile from, object o)
        {
            if (o is SorcerersRewardChest)
            {
                SorcerersRewardChest chest = (SorcerersRewardChest)o;

                Container box = (Container)chest;

                box.DropItem(new SalvagerSuitPlans());

                m_From.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154228); // *You insert the key into the mechanism and turn it. To your delight the lock opens with a click and you remove the contents*

                chest.Locked = false;

                m_Key.Delete();
            }
        }
    }
}
