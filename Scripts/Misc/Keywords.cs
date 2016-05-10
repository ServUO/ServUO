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

            if (args.Speech == "lum lum lum")
            {
                if (from is PlayerMobile && ((PlayerMobile)from).HumilityHunt && (DateTime.UtcNow > ((PlayerMobile)from).HumilityHuntLastEnded + TimeSpan.FromSeconds(30)))
                {
                    Timer mTimer = new HumilityTimer((PlayerMobile)from);
                    mTimer.Start();
                    from.SendLocalizedMessage(1155795, "30"); //Your journey on the Path of the Humble will end in ~1_SECONDS~ seconds, at that time your resists will be returned to normal. 
                }
                else if (from is PlayerMobile && ((PlayerMobile)from).HumilityHunt == false &&
                         (DateTime.UtcNow > ((PlayerMobile)from).HumilityHuntLastEnded + TimeSpan.FromSeconds(60)))
                {
                    ((PlayerMobile)from).HumilityHunt = true;
                    from.SendLocalizedMessage(1155802, "-70"); //You have begun your journey on the Path of Humility.  Your resists have been debuffed by ~1_DEBUFF~.
                    from.SendGump(new HumilityGump());

                }
                else
                {
                    from.SendLocalizedMessage(1155801);
                    //You must wait before you can once again begin your journey on the Path of Humility.
                }
            }
        }
    }

    public class HumilityTimer : Timer
    {
        private PlayerMobile pm;

        public HumilityTimer(PlayerMobile m)
            : base(TimeSpan.FromSeconds(30))
        {
            pm = m;
            pm.HumilityHuntLastEnded = DateTime.UtcNow;
            Priority = TimerPriority.OneSecond;
        }

        protected override void OnTick()
        {
            pm.HumilityHunt = false;
            pm.SendLocalizedMessage(1155800); // You have ended your journey on the Path of Humility.
            Stop();
        }
    }
}