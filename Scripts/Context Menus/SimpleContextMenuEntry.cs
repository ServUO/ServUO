using System;

namespace Server.ContextMenus
{
    public class SimpleContextMenuEntry : ContextMenuEntry
    {
        public Mobile From { get; private set; }
        public Action<Mobile> Callback { get; set; }

        public SimpleContextMenuEntry(Mobile from, int localization, Action<Mobile> callback = null, int range = -1, bool enabled = true) : base(localization, range)
        {
            From = from;
            Callback = callback;

            Enabled = enabled;
        }

        public override void OnClick()
        {
            Callback?.Invoke(From);
        }
    }

    public class SimpleContextMenuEntry<T> : ContextMenuEntry
    {
        private readonly bool _NonLocalUse;

        public Mobile From { get; private set; }
        public T State { get; private set; }
        public Action<Mobile, T> Callback { get; set; }

        public override bool NonLocalUse => _NonLocalUse;

        public SimpleContextMenuEntry(Mobile from, int localization, Action<Mobile, T> callback, T state, int range = -1, bool enabled = true, bool nonlocalUse = false) : base(localization, range)
        {
            From = from;
            State = state;
            Callback = callback;

            _NonLocalUse = nonlocalUse;

            Enabled = enabled;
        }

        public override void OnClick()
        {
            if (Callback != null)
            {
                Callback(From, State);
            }
        }
    }
}
