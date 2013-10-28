using System;

namespace Server.Items
{
    public class BaseFish : Item
    {
        private static readonly TimeSpan DeathDelay = TimeSpan.FromMinutes(5);
        private Timer m_Timer;
        [Constructable]
        public BaseFish(int itemID)
            : base(itemID)
        {
            this.StartTimer();
        }

        public BaseFish(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Dead
        {
            get
            {
                return (this.ItemID == 0x3B0C);
            }
        }
        public virtual void StartTimer()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = Timer.DelayCall(DeathDelay, new TimerCallback(Kill));

            this.InvalidateProperties();
        }

        public virtual void StopTimer()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            this.InvalidateProperties();
        }

        public override void OnDelete()
        {
            this.StopTimer();
        }

        public virtual void Kill()
        {
            this.ItemID = 0x3B0C;
            this.StopTimer();

            this.InvalidateProperties();
        }

        public int GetDescription()
        {
            // TODO: This will never return "very unusual dead aquarium creature" due to the way it is killed
            if (this.ItemID > 0x3B0F)
                return this.Dead ? 1074424 : 1074422; // A very unusual [dead/live] aquarium creature
            else if (this.Hue != 0)
                return this.Dead ? 1074425 : 1074423; // A [dead/live] aquarium creature of unusual color

            return this.Dead ? 1073623 : 1073622; // A [dead/live] aquarium creature
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(this.GetDescription());

            if (!this.Dead && this.m_Timer != null)
                list.Add(1074507); // Gasping for air
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

            if (!(this.Parent is Aquarium) && !(this.Parent is FishBowl))
                this.StartTimer();
        }
    }
}