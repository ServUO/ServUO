using System;

namespace Server.Items
{
    public class BaseFish : Item
    {
        private static readonly TimeSpan DeathDelay = TimeSpan.FromMinutes(5);
        private Timer m_Timer;

        public override int LabelNumber
        {
            get
            {
                if (ItemID >= 0xA35F)
                {
                    return 1117256 + ItemID;
                }

                return base.LabelNumber;
            }
        }

        [Constructable]
        public BaseFish(int itemID)
            : base(itemID)
        {
            StartTimer();
        }

        public BaseFish(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Dead => (ItemID == 0x3B0C);

        public virtual void StartTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = Timer.DelayCall(DeathDelay, Kill);

            InvalidateProperties();
        }

        public virtual void StopTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            InvalidateProperties();
        }

        public override void OnDelete()
        {
            StopTimer();
        }

        public virtual void Kill()
        {
            ItemID = 0x3B0C;
            StopTimer();

            InvalidateProperties();
        }

        public int GetDescription()
        {
            // TODO: This will never return "very unusual dead aquarium creature" due to the way it is killed
            if (ItemID > 0x3B0F)
                return Dead ? 1074424 : 1074422; // A very unusual [dead/live] aquarium creature
            else if (Hue != 0)
                return Dead ? 1074425 : 1074423; // A [dead/live] aquarium creature of unusual color

            return Dead ? 1073623 : 1073622; // A [dead/live] aquarium creature
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(GetDescription());

            if (!Dead && m_Timer != null)
                list.Add(1074507); // Gasping for air
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (!(Parent is Aquarium) && !(Parent is FishBowl))
                StartTimer();
        }
    }
}
