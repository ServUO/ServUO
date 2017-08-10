using System;
using Server.Guilds;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Misc
{
    public class Keywords
    {
        public static void Initialize()
        {
            // Register our speech handler
            EventSink.Speech += EventSink_Speech;
        }

        public static void EventSink_Speech(SpeechEventArgs args)
        {
            Mobile from = args.Mobile;
            int[] keywords = args.Keywords;

            for (int i = 0; i < keywords.Length; ++i)
            {
                switch (keywords[i])
                {
                    case 0x002A: // *i resign from my guild*
                        {
                            if (from.Guild != null)
                                ((Guild)from.Guild).RemoveMember(from);

                            break;
                        }
                    case 0x0032: // *i must consider my sins*
                        {
                            if (!Core.SE)
                            {
                                from.SendMessage("Short Term Murders : {0}", from.ShortTermMurders);
                                from.SendMessage("Long Term Murders : {0}", from.Kills);
                            }
                            else
                            {
                                from.SendMessage(0x3B2, "Short Term Murders: {0} Long Term Murders: {1}", from.ShortTermMurders, from.Kills);
                            }
                            break;
                        }
                    case 0x0035: // i renounce my young player status*
                        {
                            if (from is PlayerMobile && ((PlayerMobile)from).Young && !from.HasGump(typeof(RenounceYoungGump)))
                            {
                                from.SendGump(new RenounceYoungGump());
                            }

                            break;
                        }
                }
            }
        }
    }
}