using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Items;

namespace Server.Network.Misc
{
    // The entire reason this class exists is to sort the bounties. "Items" only has a get accessor.
    public class BountyPackets
    {
        public sealed class BBContent : Packet
        {
            public BBContent(Mobile beholder, Item beheld) : base(0x3C)
            {
                List<Item> items = new List<Item>(beheld.Items);
                items = items.Where(x => x is BountyMessage).OrderByDescending(x => ((BountyMessage)x).BountyAmount).ToList();
                int count = items.Count;

                this.EnsureCapacity(5 + (count * 19));

                long pos = m_Stream.Position;

                int written = 0;

                m_Stream.Write((ushort)0);

                for (int i = 0; i < count; ++i)
                {
                    Item child = items[i];

                    if (!child.Deleted && beholder.CanSee(child))
                    {
                        Point3D loc = child.Location;

                        m_Stream.Write((int)child.Serial);
                        m_Stream.Write((ushort)child.ItemID);
                        m_Stream.Write((byte)0); // signed, itemID offset
                        m_Stream.Write((ushort)child.Amount);
                        m_Stream.Write((short)loc.X);
                        m_Stream.Write((short)loc.Y);
                        m_Stream.Write((int)beheld.Serial);
                        m_Stream.Write((ushort)(child.Hue));

                        ++written;
                    }
                }

                m_Stream.Seek(pos, SeekOrigin.Begin);
                m_Stream.Write((ushort)written);
            }
        }

        public sealed class BBContent6017 : Packet
        {
            public BBContent6017(Mobile beholder, Item beheld) : base(0x3C)
            {
                List<Item> items = new List<Item>(beheld.Items);
                items = items.Where(x => x is BountyMessage).OrderByDescending(x => ((BountyMessage)x).BountyAmount).ToList();
                int count = items.Count;

                this.EnsureCapacity(5 + (count * 20));

                long pos = m_Stream.Position;

                int written = 0;

                m_Stream.Write((ushort)0);

                for (int i = 0; i < count; ++i)
                {
                    Item child = items[i];

                    if (!child.Deleted && beholder.CanSee(child))
                    {
                        Point3D loc = child.Location;

                        m_Stream.Write((int)child.Serial);
                        m_Stream.Write((ushort)child.ItemID);
                        m_Stream.Write((byte)0); // signed, itemID offset
                        m_Stream.Write((ushort)child.Amount);
                        m_Stream.Write((short)loc.X);
                        m_Stream.Write((short)loc.Y);
                        m_Stream.Write((byte)0); // Grid Location?
                        m_Stream.Write((int)beheld.Serial);
                        m_Stream.Write((ushort)(child.Hue));

                        ++written;
                    }
                }

                m_Stream.Seek(pos, SeekOrigin.Begin);
                m_Stream.Write((ushort)written);
            }
        }
    }
}