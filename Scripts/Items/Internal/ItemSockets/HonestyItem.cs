using System;

namespace Server.Items
{
    public class HonestyItemSocket : ItemSocket
    {
        public const int ExpireTime = 3;

        public string HonestyRegion { get; set; }
        public Mobile HonestyOwner { get; set; }
        public Timer HonestyTimer { get; set; }
        public DateTime HonestyPickup { get; set; }
        public bool HonestyTimerTicking { get; set; }

        public override TimeSpan TickDuration { get { return TimeSpan.FromSeconds(5); } }

        public HonestyItemSocket()
        {
        }

        public void StartHonestyTimer()
        {
            HonestyTimerTicking = true;

            BeginTimer();
        }

        protected override void OnTick()
        {
            if ((HonestyPickup + TimeSpan.FromHours(ExpireTime)) < DateTime.UtcNow)
            {
                Remove();
            }
            else
            {
                Owner.InvalidateProperties();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            if (HonestyPickup != DateTime.MinValue)
            {
                int minutes = (int)(HonestyPickup + TimeSpan.FromHours(3) - DateTime.UtcNow).TotalMinutes;
                list.Add(1151914, minutes.ToString()); // Minutes remaining for credit: ~1_val~
            }

            list.Add(1151520); // lost item (Return to gain Honesty)
        }

        public override void OnRemoved()
        {
            if (Owner != null && !Owner.Deleted)
            {
                Owner.HonestyItem = false;

                if (Owner.RootParent is Mobile)
                {
                    ((Mobile)Owner.RootParent).SendLocalizedMessage(1151519); // You claim the item as your own.  Finders keepers, losers weepers!
                }
            }
        }

        public override void OnAfterDuped(ItemSocket oldSocket)
        {
            if (oldSocket is HonestyItemSocket)
            {
                var honesty = oldSocket as HonestyItemSocket;

                HonestyRegion = honesty.HonestyRegion;
                HonestyOwner = honesty.HonestyOwner;
                HonestyPickup = honesty.HonestyPickup;
                HonestyTimerTicking = honesty.HonestyTimerTicking;

                if (HonestyTimerTicking)
                {
                    BeginTimer();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(HonestyRegion);
            writer.Write(HonestyOwner);
            writer.Write(HonestyPickup);
            writer.Write(HonestyTimerTicking);
        }

        public override void Deserialize(Item owner, GenericReader reader)
        {
            base.Deserialize(owner, reader);
            reader.ReadInt(); // version

            HonestyRegion = reader.ReadString();
            HonestyOwner = reader.ReadMobile();
            HonestyPickup = reader.ReadDateTime();
            HonestyTimerTicking = reader.ReadBool();

            Owner.HonestyItem = true;

            if (HonestyTimerTicking)
            {
                BeginTimer();
            }
        }
    }
}