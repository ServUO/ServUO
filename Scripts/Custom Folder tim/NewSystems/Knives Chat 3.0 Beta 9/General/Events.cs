using System;
using Server;

namespace Knives.Chat3
{
    public delegate void ChatEventHandler(ChatEventArgs e);
    public delegate void FilterViolationEventHandler(FilterViolationEventArgs e);
    public delegate void ErrorEventHandler(ErrorEventArgs e);
    public delegate void GumpCreatedEventHandler(GumpCreatedEventArgs e);

    public class ChatEventArgs : EventArgs
    {
        private Mobile c_Mobile;
        public Mobile Mobile { get { return c_Mobile; } }

        private Channel c_Channel;
        public Channel Channel { get { return c_Channel; } }

        private string c_Speech;
        public string Speech { get { return c_Speech; } }

        public ChatEventArgs(Mobile m, Channel c, string txt)
        {
            c_Mobile = m;
            c_Channel = c;
            c_Speech = txt;
        }
    }

    public class FilterViolationEventArgs : EventArgs
    {
        private Mobile c_Mobile;
        public Mobile Mobile { get { return c_Mobile; } }

        public FilterViolationEventArgs(Mobile m)
        {
            c_Mobile = m;
        }
    }

    public class ErrorEventArgs : EventArgs
    {
        private string c_Text;
        public string Text { get { return c_Text; } }

        public ErrorEventArgs(string txt)
        {
            c_Text = txt;
        }
    }

    public class GumpCreatedEventArgs : EventArgs
    {
        private Mobile c_Mobile;
        public Mobile Mobile { get { return c_Mobile; } }

        private GumpPlus c_Gump;
        public GumpPlus Gump { get { return c_Gump; } }

        public GumpCreatedEventArgs(Mobile m, GumpPlus g)
        {
            c_Mobile = m;
            c_Gump = g;
        }
    }

    public class Events
    {
        public static event ChatEventHandler Chat;
        public static event FilterViolationEventHandler FilterViolation;
        public static event ErrorEventHandler Error;
        public static event GumpCreatedEventHandler GumpCreated;

        public static void InvokeChat(ChatEventArgs args)
        {
            if (Chat != null)
                Chat(args);
        }

        public static void InvokeFilterViolation(FilterViolationEventArgs args)
        {
            if (FilterViolation != null)
                FilterViolation(args);
        }

        public static void InvokeError(ErrorEventArgs args)
        {
            if (Error != null)
                Error(args);
        }

        public static void InvokeGumpCreated(GumpCreatedEventArgs args)
        {
            if (GumpCreated != null)
                GumpCreated(args);
        }
    }

}