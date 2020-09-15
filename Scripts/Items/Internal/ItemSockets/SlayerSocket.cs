using System;

namespace Server.Items
{
    public class SlayerSocket : ItemSocket
    {
        public SlayerName Slayer { get; set; }

        public SlayerSocket()
        {
        }

        public SlayerSocket(SlayerName slayer, TimeSpan duration)
            : base(duration)
        {
            Slayer = slayer;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            list.Add(1155617); // Silvered
        }

        public override void OnRemoved()
        {
            if (Owner != null && !Owner.Deleted && Owner.RootParent is Mobile)
            {
                ((Mobile)Owner.RootParent).SendLocalizedMessage(1155618, string.IsNullOrEmpty(Owner.Name) ? string.Format("#{0}", Owner.LabelNumber) : Owner.Name); // Your ~1_name~'s Tincture of Silver has worn off.
            }
        }

        public static SlayerName GetSlayer(Item item)
        {
            if (item == null)
            {
                return SlayerName.None;
            }

            SlayerSocket socket = item.GetSocket<SlayerSocket>();

            if (socket != null)
            {
                return socket.Slayer;
            }

            return SlayerName.None;
        }

        public override void OnAfterDuped(ItemSocket oldSocket)
        {
            if (oldSocket is SlayerSocket)
            {
                Slayer = ((SlayerSocket)oldSocket).Slayer;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Slayer);
        }

        public override void Deserialize(Item owner, GenericReader reader)
        {
            base.Deserialize(owner, reader);
            reader.ReadInt(); // version

            Slayer = (SlayerName)reader.ReadInt();
        }
    }
}