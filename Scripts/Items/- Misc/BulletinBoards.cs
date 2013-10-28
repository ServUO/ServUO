using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Items
{
    public struct BulletinEquip
    {
        public int itemID;
        public int hue;
        public BulletinEquip(int itemID, int hue)
        {
            this.itemID = itemID;
            this.hue = hue;
        }
    }

    [Flipable(0x1E5E, 0x1E5F)]
    public class BulletinBoard : BaseBulletinBoard
    {
        [Constructable]
        public BulletinBoard()
            : base(0x1E5E)
        {
        }

        public BulletinBoard(Serial serial)
            : base(serial)
        {
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
        }
    }

    public abstract class BaseBulletinBoard : Item
    {
        // Threads will be removed six hours after the last post was made
        private static readonly TimeSpan ThreadDeletionTime = TimeSpan.FromHours(6.0);
        // A player may only create a thread once every two minutes
        private static readonly TimeSpan ThreadCreateTime = TimeSpan.FromMinutes(2.0);
        // A player may only reply once every thirty seconds
        private static readonly TimeSpan ThreadReplyTime = TimeSpan.FromSeconds(30.0);
        private string m_BoardName;
        public BaseBulletinBoard(int itemID)
            : base(itemID)
        {
            this.m_BoardName = "bulletin board";
            this.Movable = false;
        }

        public BaseBulletinBoard(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string BoardName
        {
            get
            {
                return this.m_BoardName;
            }
            set
            {
                this.m_BoardName = value;
            }
        }
        public static bool CheckTime(DateTime time, TimeSpan range)
        {
            return (time + range) < DateTime.UtcNow;
        }

        public static string FormatTS(TimeSpan ts)
        {
            int totalSeconds = (int)ts.TotalSeconds;
            int seconds = totalSeconds % 60;
            int minutes = totalSeconds / 60;

            if (minutes != 0 && seconds != 0)
                return String.Format("{0} minute{1} and {2} second{3}", minutes, minutes == 1 ? "" : "s", seconds, seconds == 1 ? "" : "s");
            else if (minutes != 0)
                return String.Format("{0} minute{1}", minutes, minutes == 1 ? "" : "s");
            else
                return String.Format("{0} second{1}", seconds, seconds == 1 ? "" : "s");
        }

        public static void Initialize()
        {
            PacketHandlers.Register(0x71, 0, true, new OnPacketReceive(BBClientRequest));
        }

        public static void BBClientRequest(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;

            int packetID = pvSrc.ReadByte();
            BaseBulletinBoard board = World.FindItem(pvSrc.ReadInt32()) as BaseBulletinBoard;

            if (board == null || !board.CheckRange(from))
                return;

            switch ( packetID )
            {
                case 3:
                    BBRequestContent(from, board, pvSrc);
                    break;
                case 4:
                    BBRequestHeader(from, board, pvSrc);
                    break;
                case 5:
                    BBPostMessage(from, board, pvSrc);
                    break;
                case 6:
                    BBRemoveMessage(from, board, pvSrc);
                    break;
            }
        }

        public static void BBRequestContent(Mobile from, BaseBulletinBoard board, PacketReader pvSrc)
        {
            BulletinMessage msg = World.FindItem(pvSrc.ReadInt32()) as BulletinMessage;

            if (msg == null || msg.Parent != board)
                return;

            from.Send(new BBMessageContent(board, msg));
        }

        public static void BBRequestHeader(Mobile from, BaseBulletinBoard board, PacketReader pvSrc)
        {
            BulletinMessage msg = World.FindItem(pvSrc.ReadInt32()) as BulletinMessage;

            if (msg == null || msg.Parent != board)
                return;

            from.Send(new BBMessageHeader(board, msg));
        }

        public static void BBPostMessage(Mobile from, BaseBulletinBoard board, PacketReader pvSrc)
        {
            BulletinMessage thread = World.FindItem(pvSrc.ReadInt32()) as BulletinMessage;

            if (thread != null && thread.Parent != board)
                thread = null;

            int breakout = 0;

            while (thread != null && thread.Thread != null && breakout++ < 10)
                thread = thread.Thread;

            DateTime lastPostTime = DateTime.MinValue;

            if (board.GetLastPostTime(from, (thread == null), ref lastPostTime))
            {
                if (!CheckTime(lastPostTime, (thread == null ? ThreadCreateTime : ThreadReplyTime)))
                {
                    if (thread == null)
                        from.SendMessage("You must wait {0} before creating a new thread.", FormatTS(ThreadCreateTime));
                    else
                        from.SendMessage("You must wait {0} before replying to another thread.", FormatTS(ThreadReplyTime));

                    return;
                }
            }

            string subject = pvSrc.ReadUTF8StringSafe(pvSrc.ReadByte());

            if (subject.Length == 0)
                return;

            string[] lines = new string[pvSrc.ReadByte()];

            if (lines.Length == 0)
                return;

            for (int i = 0; i < lines.Length; ++i)
                lines[i] = pvSrc.ReadUTF8StringSafe(pvSrc.ReadByte());

            board.PostMessage(from, thread, subject, lines);
        }

        public static void BBRemoveMessage(Mobile from, BaseBulletinBoard board, PacketReader pvSrc)
        {
            BulletinMessage msg = World.FindItem(pvSrc.ReadInt32()) as BulletinMessage;

            if (msg == null || msg.Parent != board)
                return;

            if (from.AccessLevel < AccessLevel.GameMaster && msg.Poster != from)
                return;

            msg.Delete();
        }

        public virtual void Cleanup()
        {
            List<Item> items = this.Items;

            for (int i = items.Count - 1; i >= 0; --i)
            {
                if (i >= items.Count)
                    continue;

                BulletinMessage msg = items[i] as BulletinMessage;

                if (msg == null)
                    continue;

                if (msg.Thread == null && CheckTime(msg.LastPostTime, ThreadDeletionTime))
                {
                    msg.Delete();
                    this.RecurseDelete(msg); // A root-level thread has expired
                }
            }
        }

        public virtual bool GetLastPostTime(Mobile poster, bool onlyCheckRoot, ref DateTime lastPostTime)
        {
            List<Item> items = this.Items;
            bool wasSet = false;

            for (int i = 0; i < items.Count; ++i)
            {
                BulletinMessage msg = items[i] as BulletinMessage;

                if (msg == null || msg.Poster != poster)
                    continue;

                if (onlyCheckRoot && msg.Thread != null)
                    continue;

                if (msg.Time > lastPostTime)
                {
                    wasSet = true;
                    lastPostTime = msg.Time;
                }
            }

            return wasSet;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.CheckRange(from))
            {
                this.Cleanup();

                NetState state = from.NetState;

                state.Send(new BBDisplayBoard(this));
                if (state.ContainerGridLines)
                    state.Send(new ContainerContent6017(from, this));
                else
                    state.Send(new ContainerContent(from, this));
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public virtual bool CheckRange(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            return (from.Map == this.Map && from.InRange(this.GetWorldLocation(), 2));
        }

        public void PostMessage(Mobile from, BulletinMessage thread, string subject, string[] lines)
        {
            if (thread != null)
                thread.LastPostTime = DateTime.UtcNow;

            this.AddItem(new BulletinMessage(from, thread, subject, lines));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((string)this.m_BoardName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_BoardName = reader.ReadString();
                        break;
                    }
            }
        }

        private void RecurseDelete(BulletinMessage msg)
        {
            List<Item> found = new List<Item>();
            List<Item> items = this.Items;

            for (int i = items.Count - 1; i >= 0; --i)
            {
                if (i >= items.Count)
                    continue;

                BulletinMessage check = items[i] as BulletinMessage;

                if (check == null)
                    continue;

                if (check.Thread == msg)
                {
                    check.Delete();
                    found.Add(check);
                }
            }

            for (int i = 0; i < found.Count; ++i)
                this.RecurseDelete((BulletinMessage)found[i]);
        }
    }

    public class BulletinMessage : Item
    {
        private Mobile m_Poster;
        private string m_Subject;
        private DateTime m_Time, m_LastPostTime;
        private BulletinMessage m_Thread;
        private string m_PostedName;
        private int m_PostedBody;
        private int m_PostedHue;
        private BulletinEquip[] m_PostedEquip;
        private string[] m_Lines;
        public BulletinMessage(Mobile poster, BulletinMessage thread, string subject, string[] lines)
            : base(0xEB0)
        {
            this.Movable = false;

            this.m_Poster = poster;
            this.m_Subject = subject;
            this.m_Time = DateTime.UtcNow;
            this.m_LastPostTime = this.m_Time;
            this.m_Thread = thread;
            this.m_PostedName = this.m_Poster.Name;
            this.m_PostedBody = this.m_Poster.Body;
            this.m_PostedHue = this.m_Poster.Hue;
            this.m_Lines = lines;

            List<BulletinEquip> list = new List<BulletinEquip>();

            for (int i = 0; i < poster.Items.Count; ++i)
            {
                Item item = poster.Items[i];

                if (item.Layer >= Layer.OneHanded && item.Layer <= Layer.Mount)
                    list.Add(new BulletinEquip(item.ItemID, item.Hue));
            }

            this.m_PostedEquip = list.ToArray();
        }

        public BulletinMessage(Serial serial)
            : base(serial)
        {
        }

        public Mobile Poster
        {
            get
            {
                return this.m_Poster;
            }
        }
        public BulletinMessage Thread
        {
            get
            {
                return this.m_Thread;
            }
        }
        public string Subject
        {
            get
            {
                return this.m_Subject;
            }
        }
        public DateTime Time
        {
            get
            {
                return this.m_Time;
            }
        }
        public DateTime LastPostTime
        {
            get
            {
                return this.m_LastPostTime;
            }
            set
            {
                this.m_LastPostTime = value;
            }
        }
        public string PostedName
        {
            get
            {
                return this.m_PostedName;
            }
        }
        public int PostedBody
        {
            get
            {
                return this.m_PostedBody;
            }
        }
        public int PostedHue
        {
            get
            {
                return this.m_PostedHue;
            }
        }
        public BulletinEquip[] PostedEquip
        {
            get
            {
                return this.m_PostedEquip;
            }
        }
        public string[] Lines
        {
            get
            {
                return this.m_Lines;
            }
        }
        public string GetTimeAsString()
        {
            return this.m_Time.ToString("MMM dd, yyyy");
        }

        public override bool CheckTarget(Mobile from, Server.Targeting.Target targ, object targeted)
        {
            return false;
        }

        public override bool IsAccessibleTo(Mobile check)
        {
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((Mobile)this.m_Poster);
            writer.Write((string)this.m_Subject);
            writer.Write((DateTime)this.m_Time);
            writer.Write((DateTime)this.m_LastPostTime);
            writer.Write((bool)(this.m_Thread != null));
            writer.Write((Item)this.m_Thread);
            writer.Write((string)this.m_PostedName);
            writer.Write((int)this.m_PostedBody);
            writer.Write((int)this.m_PostedHue);

            writer.Write((int)this.m_PostedEquip.Length);

            for (int i = 0; i < this.m_PostedEquip.Length; ++i)
            {
                writer.Write((int)this.m_PostedEquip[i].itemID);
                writer.Write((int)this.m_PostedEquip[i].hue);
            }

            writer.Write((int)this.m_Lines.Length);

            for (int i = 0; i < this.m_Lines.Length; ++i)
                writer.Write((string)this.m_Lines[i]);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        this.m_Poster = reader.ReadMobile();
                        this.m_Subject = reader.ReadString();
                        this.m_Time = reader.ReadDateTime();
                        this.m_LastPostTime = reader.ReadDateTime();
                        bool hasThread = reader.ReadBool();
                        this.m_Thread = reader.ReadItem() as BulletinMessage;
                        this.m_PostedName = reader.ReadString();
                        this.m_PostedBody = reader.ReadInt();
                        this.m_PostedHue = reader.ReadInt();

                        this.m_PostedEquip = new BulletinEquip[reader.ReadInt()];

                        for (int i = 0; i < this.m_PostedEquip.Length; ++i)
                        {
                            this.m_PostedEquip[i].itemID = reader.ReadInt();
                            this.m_PostedEquip[i].hue = reader.ReadInt();
                        }

                        this.m_Lines = new string[reader.ReadInt()];

                        for (int i = 0; i < this.m_Lines.Length; ++i)
                            this.m_Lines[i] = reader.ReadString();

                        if (hasThread && this.m_Thread == null)
                            this.Delete();

                        if (version == 0)
                            ValidationQueue<BulletinMessage>.Add(this);

                        break;
                    }
            }
        }

        public void Validate()
        {
            if (!(this.Parent is BulletinBoard && ((BulletinBoard)this.Parent).Items.Contains(this)))
                this.Delete();
        }
    }

    public class BBDisplayBoard : Packet
    {
        public BBDisplayBoard(BaseBulletinBoard board)
            : base(0x71)
        {
            string name = board.BoardName;

            if (name == null)
                name = "";

            this.EnsureCapacity(38);

            byte[] buffer = Utility.UTF8.GetBytes(name);

            this.m_Stream.Write((byte)0x00); // PacketID
            this.m_Stream.Write((int)board.Serial); // Bulletin board serial

            // Bulletin board name
            if (buffer.Length >= 29)
            {
                this.m_Stream.Write(buffer, 0, 29);
                this.m_Stream.Write((byte)0);
            }
            else
            {
                this.m_Stream.Write(buffer, 0, buffer.Length);
                this.m_Stream.Fill(30 - buffer.Length);
            }
        }
    }

    public class BBMessageHeader : Packet
    {
        public BBMessageHeader(BaseBulletinBoard board, BulletinMessage msg)
            : base(0x71)
        {
            string poster = this.SafeString(msg.PostedName);
            string subject = this.SafeString(msg.Subject);
            string time = this.SafeString(msg.GetTimeAsString());

            this.EnsureCapacity(22 + poster.Length + subject.Length + time.Length);

            this.m_Stream.Write((byte)0x01); // PacketID
            this.m_Stream.Write((int)board.Serial); // Bulletin board serial
            this.m_Stream.Write((int)msg.Serial); // Message serial

            BulletinMessage thread = msg.Thread;

            if (thread == null)
                this.m_Stream.Write((int)0); // Thread serial--root
            else
                this.m_Stream.Write((int)thread.Serial); // Thread serial--parent

            this.WriteString(poster);
            this.WriteString(subject);
            this.WriteString(time);
        }

        public void WriteString(string v)
        {
            byte[] buffer = Utility.UTF8.GetBytes(v);
            int len = buffer.Length + 1;

            if (len > 255)
                len = 255;

            this.m_Stream.Write((byte)len);
            this.m_Stream.Write(buffer, 0, len - 1);
            this.m_Stream.Write((byte)0);
        }

        public string SafeString(string v)
        {
            if (v == null)
                return String.Empty;

            return v;
        }
    }

    public class BBMessageContent : Packet
    {
        public BBMessageContent(BaseBulletinBoard board, BulletinMessage msg)
            : base(0x71)
        {
            string poster = this.SafeString(msg.PostedName);
            string subject = this.SafeString(msg.Subject);
            string time = this.SafeString(msg.GetTimeAsString());

            this.EnsureCapacity(22 + poster.Length + subject.Length + time.Length);

            this.m_Stream.Write((byte)0x02); // PacketID
            this.m_Stream.Write((int)board.Serial); // Bulletin board serial
            this.m_Stream.Write((int)msg.Serial); // Message serial

            this.WriteString(poster);
            this.WriteString(subject);
            this.WriteString(time);

            this.m_Stream.Write((short)msg.PostedBody);
            this.m_Stream.Write((short)msg.PostedHue);

            int len = msg.PostedEquip.Length;

            if (len > 255)
                len = 255;

            this.m_Stream.Write((byte)len);

            for (int i = 0; i < len; ++i)
            {
                BulletinEquip eq = msg.PostedEquip[i];

                this.m_Stream.Write((short)eq.itemID);
                this.m_Stream.Write((short)eq.hue);
            }

            len = msg.Lines.Length;

            if (len > 255)
                len = 255;

            this.m_Stream.Write((byte)len);

            for (int i = 0; i < len; ++i)
                this.WriteString(msg.Lines[i], true);
        }

        public void WriteString(string v)
        {
            this.WriteString(v, false);
        }

        public void WriteString(string v, bool padding)
        {
            byte[] buffer = Utility.UTF8.GetBytes(v);
            int tail = padding ? 2 : 1;
            int len = buffer.Length + tail;

            if (len > 255)
                len = 255;

            this.m_Stream.Write((byte)len);
            this.m_Stream.Write(buffer, 0, len - tail);

            if (padding)
                this.m_Stream.Write((short)0); // padding compensates for a client bug
            else
                this.m_Stream.Write((byte)0);
        }

        public string SafeString(string v)
        {
            if (v == null)
                return String.Empty;

            return v;
        }
    }
}