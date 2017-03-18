using System;
using Server.Items;

namespace Server.ContextMenus
{
	public class SimpleContextMenuEntry : ContextMenuEntry
	{
        public Mobile From { get; private set; }
        public Action<Mobile> Callback { get; set; }

		public SimpleContextMenuEntry( Mobile from, int localization, Action<Mobile> callback = null, int range = -1, bool enabled = true ) : base( localization, range )
		{
			From = from;
            Callback = callback;

            Enabled = enabled;
		}

		public override void OnClick()
		{
            if (Callback != null)
                Callback(From);
		}
	}
}