using System;
using System.IO;
using System.Collections;
using Server;

namespace Knives.Chat3
{
    public enum MsgType { Normal, Invite, Staff, System }

    public class Message
    {
        public static bool CanMessage(Mobile from, Mobile to)
        {
            if (from == to)
                return false;

            if (from.AccessLevel > to.AccessLevel)
                return true;

            Data df = Data.GetData(from);
            Data dt = Data.GetData(to);

            if (df.Banned || dt.Banned)
                return false;
            if (df.FriendsOnly && !df.Friends.Contains(to))
                return false;
            if (dt.FriendsOnly && !dt.Friends.Contains(from))
                return false;
            if (df.Ignores.Contains(to))
                return false;
            if (dt.Ignores.Contains(from))
                return false;
            if (dt.Messages.Count >= Data.MaxMsgs && !dt.WhenFull)
                return false;

            return true;
        }

        public static bool StaffTimeout(Message msg)
        {
            return (msg.From.AccessLevel > AccessLevel.Player && msg.Received + TimeSpan.FromHours(5) < DateTime.Now);
        }

        private Mobile c_From;
        private string c_Msg, c_Subject;
        private MsgType c_Type;
        private DateTime c_Received;
        private bool c_Read;

        public Mobile From { get { return c_From; } }
        public string Msg { get { return c_Msg; } }
        public string Subject { get { return c_Subject; } }
        public MsgType Type { get { return c_Type; } }
        public DateTime Received { get { return c_Received; } }

        public bool Read
        {
            get { return c_Read; }
            set
            {
                c_Read = value;
            }
        }

        public Message()
        {
            c_Msg = "";
            c_Subject = "";
        }

        public Message(Mobile from, string sub, string msg, MsgType type)
        {
            c_From = from;
            c_Msg = msg;
            c_Subject = sub;
            c_Type = type;

            c_Received = DateTime.Now;
        }

        public void Save(GenericWriter writer)
        {
            writer.Write(0); // Version

            writer.Write(c_From);
            writer.Write(c_Msg);
            writer.Write(c_Subject);
            writer.Write((int)c_Type);
            writer.Write(c_Received);
            writer.Write(c_Read);
        }

        public void Load(GenericReader reader)
        {
            int version = reader.ReadInt();

            c_From = reader.ReadMobile();
            c_Msg = reader.ReadString();
            c_Subject = reader.ReadString();
            c_Type = (MsgType)reader.ReadInt();
            c_Received = reader.ReadDateTime();
            c_Read = reader.ReadBool();
        }
    }
}