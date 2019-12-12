using System;
using Server;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
	public class EnchantedTimepiece : Item
	{
		public override int LabelNumber 
        { 
            get 
            {
                int hour, min;

                Clock.GetTime(this.Map, GetWorldLocation().X, GetWorldLocation().Y, out hour, out min);

                if (hour > 20 || hour < 4)
                    return 1156199; // Moon Dial

                return 1123890; // Sun Dial
                //return 1077186; 
            } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int ItemID
        {
            get
            {
                int id = base.ItemID;

                if (id != GetID())
                    return GetID();

                return id;
            }
            set
            {
                base.ItemID = value;
            }
        }

		[Constructable]
		public EnchantedTimepiece() : base(0x9BC6)
		{
            _TimePieces.Add(this);
		}

        public override void OnDoubleClick(Mobile from)
        {
            int genericNumber;
            string exactTime;

            Clock.GetTime(from, out genericNumber, out exactTime);

            SendLocalizedMessageTo(from, genericNumber);
            SendLocalizedMessageTo(from, 1042958, exactTime); // ~1_TIME~ to be exact
        }

        public void InvalidateID()
        {
            int hour, min;

            Clock.GetTime(this.Map, GetWorldLocation().X, GetWorldLocation().Y, out hour, out min);

            if (hour > 12)
                hour -= 12;

            int id = 0x9BBA + Math.Max(0, (hour - 1));

            if (id != ItemID)
                ItemID = id;
        }

        private int GetID()
        {
            int id = 0x9BC6;

            if (!Movable && this.Map != Map.Internal)
            {
                int hour, min;

                Clock.GetTime(this.Map, GetWorldLocation().X, GetWorldLocation().Y, out hour, out min);

                if (hour > 12)
                    hour -= 12;

                id = 0x9BBA + Math.Max(0, (hour - 1));
            }

            return id;
        }

        public override void Delete()
        {
            base.Delete();

            if (_TimePieces.Contains(this))
                _TimePieces.Remove(this);
        }

        private static List<EnchantedTimepiece> _TimePieces = new List<EnchantedTimepiece>();

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), () =>
                {
                    foreach (EnchantedTimepiece tp in _TimePieces.Where(t => t != null && !t.Movable && t.Map != Map.Internal))
                    {
                        tp.InvalidateID();
                    }
                });
        }

		public EnchantedTimepiece(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}
 
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            _TimePieces.Add(this);
        }
	}
}