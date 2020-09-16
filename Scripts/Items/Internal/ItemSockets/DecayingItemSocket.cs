using System;

namespace Server.Items
{
    public class DecayingItemSocket : ItemSocket
    {
        public override TimeSpan TickDuration => TimeSpan.FromSeconds(10);

        public int Remaining => Expires > DateTime.UtcNow ? (int)(Expires - DateTime.UtcNow).TotalSeconds : 0;

        public bool DisplaySeconds { get; set; }

        public DecayingItemSocket()
        {
        }

        public DecayingItemSocket(int lifespan, bool useSeconds)
            : base(lifespan == 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(lifespan))
        {
            DisplaySeconds = useSeconds;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            if (Expires > DateTime.UtcNow)
            {
                if (DisplaySeconds)
                {
                    list.Add(1072517, Remaining.ToString()); // Lifespan: ~1_val~ seconds
                }
                else
                {
                    TimeSpan t = Expires - DateTime.UtcNow;

                    int weeks = t.Days / 7;
                    int days = t.Days;
                    int hours = t.Hours;
                    int minutes = t.Minutes;

                    if (weeks > 1)
                        list.Add(1153092, (t.Days / 7).ToString()); // Lifespan: ~1_val~ weeks
                    else if (days > 1)
                        list.Add(1153091, t.Days.ToString()); // Lifespan: ~1_val~ days
                    else if (hours > 1)
                        list.Add(1153090, t.Hours.ToString()); // Lifespan: ~1_val~ hours
                    else if (minutes > 1)
                        list.Add(1153089, t.Minutes.ToString()); // Lifespan: ~1_val~ minutes
                    else
                        list.Add(1072517, t.Seconds.ToString()); // Lifespan: ~1_val~ seconds
                }
            }
        }

        protected override void OnTick()
        {
            if (Expires < DateTime.UtcNow && !Owner.Deleted)
            {
                var item = Owner as BaseDecayingItem;

                if (item != null)
                {
                    item.Decay();
                }
                else
                {
                    Owner.Delete();
                }
            }
            else
            {
                Owner.InvalidateProperties();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(DisplaySeconds);
        }

        public override void Deserialize(Item owner, GenericReader reader)
        {
            base.Deserialize(owner, reader);
            reader.ReadInt(); // version

            DisplaySeconds = reader.ReadBool();
        }
    }
}
