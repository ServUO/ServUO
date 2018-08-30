using System;
using Server;
using Server.Network;
using Knives.Chat3;

namespace Server.Engines.PartySystem
{
    public class Chat3Guild
    {
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AfterInit));
        }

        public static void AfterInit()
        {
            PacketHandlers.Register(0x03, 0, true, new OnPacketReceive(AsciiSpeechChat3));
            PacketHandlers.Register(0xAD, 0, true, new OnPacketReceive(UnicodeSpeechChat3));
        }

        public static void AsciiSpeechChat3(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;

            MessageType type = (MessageType)pvSrc.ReadByte();
            int hue = pvSrc.ReadInt16();
            pvSrc.ReadInt16(); // font
            string text = pvSrc.ReadStringSafe().Trim();

            if (text.Length <= 0 || text.Length > 128)
                return;

            if (!Enum.IsDefined(typeof(MessageType), type))
                type = MessageType.Regular;

            Channel c = Channel.GetByType(typeof(Guild));
            if (RUOVersion.GuildChat(type) && c != null)
                c.OnChat(from, text);
            else
                from.DoSpeech(text, c_EmptyInts, type, Utility.ClipDyedHue(hue));
        }

        private static KeywordList c_KeywordList = new KeywordList();
        private static int[] c_EmptyInts = new int[0];

        public static void UnicodeSpeechChat3(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;

            MessageType type = (MessageType)pvSrc.ReadByte();
            int hue = pvSrc.ReadInt16();
            pvSrc.ReadInt16(); // font
            string lang = pvSrc.ReadString(4);
            string text;

            bool isEncoded = (type & MessageType.Encoded) != 0;
            int[] keywords;

            if (isEncoded)
            {
                int value = pvSrc.ReadInt16();
                int count = (value & 0xFFF0) >> 4;
                int hold = value & 0xF;

                if (count < 0 || count > 50)
                    return;

                KeywordList keyList = c_KeywordList;

                for (int i = 0; i < count; ++i)
                {
                    int speechID;

                    if ((i & 1) == 0)
                    {
                        hold <<= 8;
                        hold |= pvSrc.ReadByte();
                        speechID = hold;
                        hold = 0;
                    }
                    else
                    {
                        value = pvSrc.ReadInt16();
                        speechID = (value & 0xFFF0) >> 4;
                        hold = value & 0xF;
                    }

                    if (!keyList.Contains(speechID))
                        keyList.Add(speechID);
                }

                text = pvSrc.ReadUTF8StringSafe();

                keywords = keyList.ToArray();
            }
            else
            {
                text = pvSrc.ReadUnicodeStringSafe();

                keywords = c_EmptyInts;
            }

            text = text.Trim();

            if (text.Length <= 0 || text.Length > 128)
                return;

            type &= ~MessageType.Encoded;

            if (!Enum.IsDefined(typeof(MessageType), type))
                type = MessageType.Regular;

            from.Language = lang;

            Channel c = Channel.GetByType(typeof(Guild));
            if (RUOVersion.GuildChat(type) && c != null)
            {
                if(c.CanChat(from, true))
                    c.OnChat(from, text);
            }
            else
                from.DoSpeech(text, keywords, type, Utility.ClipDyedHue(hue));
        }
    }
}