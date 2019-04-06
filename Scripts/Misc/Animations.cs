using System;

namespace Server.Misc
{
    public class Animations
    {
        public static void Initialize()
        {
            EventSink.AnimateRequest += new AnimateRequestEventHandler(EventSink_AnimateRequest);
        }

        private static void EventSink_AnimateRequest(AnimateRequestEventArgs e)
        {
            Mobile from = e.Mobile;

            int action;
            bool useNew = Core.SA;

            switch (e.Action)
            {
                case "bow":
                    action = useNew ? 0 : 32;
                    break;
                case "salute":
                    action = useNew ? 1 : 33;
                    break;
                default:
                    return;
            }

            if (from.Alive && !from.Mounted && (from.Body.IsHuman || from.Body.IsGargoyle))
            {
                if (useNew)
                {
                    from.Animate(AnimationType.Emote, action);
                }
                else
                {
                    from.Animate(action, 5, 1, true, false, 0);
                }
            }
        }
    }
}