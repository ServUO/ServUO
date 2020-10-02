using System;

namespace Server.Items
{
    public abstract class BaseDungeonChest : LockableContainer
    {
        public static readonly string m_DeleteTimerID = "DungeonChest";

        public override int DefaultGumpID => 0x42;
        public override int DefaultDropSound => 0x42;
        public override Rectangle2D Bounds => new Rectangle2D(20, 105, 150, 180);
        public override bool IsDecoContainer => false;

        public BaseDungeonChest(int itemID) : base(itemID)
        {
            Locked = true;
            Movable = false;

            Key key = (Key)FindItemByType(typeof(Key));

            if (key != null)
                key.Delete();

            RefinementComponent.Roll(this, 1, 0.08);
        }

        public BaseDungeonChest(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);

            writer.Write(TimerRegistry.HasTimer(m_DeleteTimerID, this));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 1 && reader.ReadBool())
            {
                StartDeleteTimer();
            }
        }

        public override void OnTelekinesis(Mobile from)
        {
            if (CheckLocked(from))
            {
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
                Effects.PlaySound(Location, Map, 0x1F5);
                return;
            }

            base.OnTelekinesis(from);
            Name = "a treasure chest";
            StartDeleteTimer();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (CheckLocked(from))
                return;

            base.OnDoubleClick(from);
            Name = "a treasure chest";
            StartDeleteTimer();
        }

        protected void AddLoot(Item item)
        {
            if (item == null)
                return;

            if (RandomItemGenerator.Enabled)
            {
                int min, max;
                TreasureMapChest.GetRandomItemStat(out min, out max);

                RunicReforging.GenerateRandomItem(item, 0, min, max);
            }

            DropItem(item);
        }

        private void StartDeleteTimer()
        {
            TimerRegistry.Register(m_DeleteTimerID, this, TimeSpan.FromMinutes(Utility.RandomMinMax(2, 5)), chest => chest.Delete());
        }
    }
}
