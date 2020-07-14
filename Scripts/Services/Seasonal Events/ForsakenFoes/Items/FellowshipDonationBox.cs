using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Items
{
    public class FellowshipDonationBox : BaseContainer
    {
        public override int LabelNumber => 1159042;  // Fellowship Donation Box

        public override int DefaultGumpID => 0x10C;

        public static string FilePath = Path.Combine("Saves/Misc", "FellowshipDonationBox.bin");
        private static readonly Dictionary<Mobile, int> Donations = new Dictionary<Mobile, int>();

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static FellowshipDonationBox InstanceTram { get; set; }
        public static FellowshipDonationBox InstanceFel { get; set; }

        [Constructable]
        public FellowshipDonationBox()
            : base(0x2DE9)
        {
            Hue = 1191;
            Movable = false;
        }

        public FellowshipDonationBox(Serial serial)
            : base(serial)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            return Check(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            return Check(from, item);
        }

        public bool Check(Mobile from, Item item)
        {
            if (from == null || from.Deleted)
            {
                return false;
            }

            if (item != null && !(item is MaritimeCargo))
            {
                from.SendLocalizedMessage(1159030); // The Fellowship only requires trade cargo donations at this time.
                return false;
            }

            int amount = ((MaritimeCargo)item).GetAwardAmount() * 1000;

            if (Donations.ContainsKey(from))
            {
                Donations[from] += amount;
            }
            else
            {
                Donations.Add(from, amount);
            }

            from.SendLocalizedMessage(1159032, string.Format("{0}", Donations[from].ToString())); // The Fellowship thanks you for your donation. You have donated ~1_val~ worth of goods!

            if (Donations[from] >= 450000000)
            {
                from.SendLocalizedMessage(1152339, string.Format("{0}", Donations[from].ToString())); // A reward of ~1_ITEM~ has been placed in your backpack.
                from.AddToBackpack(new FellowshipCoin());
            }

            item.Delete();

            return true;
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

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    writer.Write(Donations.Count);

                    Donations.ToList().ForEach(s =>
                    {
                        writer.Write(s.Key);
                        writer.Write(s.Value);
                    });
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();
                    int count = reader.ReadInt();

                    for (int i = count; i > 0; i--)
                    {
                        Mobile m = reader.ReadMobile();
                        int value = reader.ReadInt();

                        if (m != null)
                        {
                            Donations.Add(m, value);
                        }
                    }
                });
        }
    }
}
