#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;

using VitaNex;
#endregion

namespace Server.ContextMenus
{
	public class CustomContextEntry : ContextMenuEntry
	{
		public static Color555 DefaultColor = 0xFFFF;

		private static Action<IEntity, Mobile> WrapCallback(Action<Mobile> callback)
		{
			if (callback != null)
			{
				return (e, m) => callback(m);
			}

			return null;
		}

		public Action<IEntity, Mobile> Callback { get; set; }

		public CustomContextEntry(int clilocID, Action<Mobile> callback)
			: this(clilocID, WrapCallback(callback))
		{ }

		public CustomContextEntry(int clilocID, Action<Mobile> callback, Color555 color)
			: this(clilocID, WrapCallback(callback), color)
		{ }

		public CustomContextEntry(int clilocID, Action<Mobile> callback, bool enabled)
			: this(clilocID, WrapCallback(callback), enabled)
		{ }

		public CustomContextEntry(int clilocID, Action<Mobile> callback, bool enabled, Color555 color)
			: this(clilocID, WrapCallback(callback), enabled, color)
		{ }

		public CustomContextEntry(int clilocID, Action<IEntity, Mobile> callback)
			: this(clilocID, callback, true, DefaultColor)
		{ }

		public CustomContextEntry(int clilocID, Action<IEntity, Mobile> callback, Color555 color)
			: this(clilocID, callback, true, color)
		{ }

		public CustomContextEntry(int clilocID, Action<IEntity, Mobile> callback, bool enabled)
			: this(clilocID, callback, enabled, DefaultColor)
		{ }

		public CustomContextEntry(int clilocID, Action<IEntity, Mobile> callback, bool enabled, Color555 color)
			: base(clilocID)
		{
			Callback = callback;
			Enabled = enabled;
			Color = color;
		}

		public override sealed void OnClick()
		{
			if (Enabled && OnClick(Owner.Target, Owner.From))
			{
				OnCallback(Owner.Target, Owner.From);
			}
		}

		protected virtual void OnCallback(object owner, Mobile user)
		{
			if (owner is IEntity)
			{
				OnCallback((IEntity)owner, user);
			}
		}

		protected virtual void OnCallback(IEntity owner, Mobile user)
		{
			if (Callback != null)
			{
				Callback(owner, user);
			}
		}

		protected virtual bool OnClick(object owner, Mobile user)
		{
			return owner is IEntity && OnClick((IEntity)owner, user);
		}

		protected virtual bool OnClick(IEntity owner, Mobile user)
		{
			return true;
		}
	}
}