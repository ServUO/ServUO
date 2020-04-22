namespace Server.Misc
{
    public class Animations
    {
        public static void Initialize()
        {
            EventSink.AnimateRequest += EventSink_AnimateRequest;
        }

        private static void EventSink_AnimateRequest(AnimateRequestEventArgs e)
        {
            Mobile from = e.Mobile;

            int action;
            switch (e.Action)
            {
                case "bow":
                    action = 0;
                    break;
                case "salute":
                    action = 1;
                    break;
                default:
                    return;
            }

            if (from.Alive && !from.Mounted && (from.Body.IsHuman || from.Body.IsGargoyle))
            {
                from.Animate(AnimationType.Emote, action);
            }
        }
    }
}
