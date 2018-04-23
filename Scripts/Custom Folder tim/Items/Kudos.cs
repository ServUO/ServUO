//   ___|========================|___
//   \  |  Written by Felladrin  |  /   [Kudos - Exchangeable Game Time Reward] - Current version: 1.3 (September 10, 2013)
//    > |       July 2013        | <
//   /__|========================|__\   Description: An item given to players for each period of time they stay online.

using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.GameTimeReward
{
    public class KudosTimer : Timer
    {
        public static class Config
        {
            public static int MinutesOnline = 6;                            // Every X minutes we give kudos to all players online.
            public static bool DropOnBank = true;                           // Should we place the kudos on character's bankbox (true) or backpack (false)?
            public static AccessLevel MaxAccessLevel = AccessLevel.Player;  // Any character with this access and below receives kudos.
        }

        public static void Initialize()
        {
            new KudosTimer().Start();
        }

        public KudosTimer() : base(TimeSpan.Zero, TimeSpan.FromMinutes(Config.MinutesOnline))
        {
            Priority = TimerPriority.OneMinute;
        }

        protected override void OnTick()
        {
            foreach (NetState state in NetState.Instances)
            {
                Mobile m = state.Mobile;

                if (m != null && m is PlayerMobile && m.AccessLevel <= Config.MaxAccessLevel)
                {
                    if (Config.DropOnBank && m.BankBox != null)
                    {
                        Item kudos = m.BankBox.FindItemByType(typeof(Kudos));

                        if (kudos != null)
                        {
                            kudos.Amount++;
                        }
                        else
                        {
                            m.BankBox.DropItem(new Kudos());
                        }
                    }
                    else if (m.Backpack != null)
                    {
                        Item kudos = m.Backpack.FindItemByType(typeof(Kudos));

                        if (kudos != null)
                        {
                            kudos.Amount++;
                        }
                        else
                        {
                            m.Backpack.DropItem(new Kudos());
                        }
                    }
                }
            }
        }
    }

    public class Kudos : Item
    {
        [Constructable]
        public Kudos(int amount)
        {
            Name = "Kudos";
            ItemID = 0xF11;
            Hue = 0x47E;
            Stackable = true;
            Amount = amount;
            Weight = 0;
        }

        [Constructable]
        public Kudos() : this(1) { }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Exchangeable Game Time Reward");
        }

        public override bool DisplayWeight { get { return false; } }

        public Kudos(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
