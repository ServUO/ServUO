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

using Server;
using Server.Targeting;
#endregion

namespace VitaNex.Targets
{
	/// <summary>
	///     Provides methods for selecting specific Items of the given Type
	/// </summary>
	/// <typeparam name="TItem">Type of the Item to be selected</typeparam>
	public class ItemSelectTarget<TItem> : GenericSelectTarget<TItem>
		where TItem : Item
	{
		/// <summary>
		///     Create an instance of ItemSelectTarget
		/// </summary>
		public ItemSelectTarget()
			: base(null, null)
		{ }

		/// <summary>
		///     Create an instance of ItemSelectTarget with additional options
		/// </summary>
		public ItemSelectTarget(int range, bool allowGround, TargetFlags flags)
			: base(null, null, range, allowGround, flags)
		{ }

		/// <summary>
		///     Create an instance of ItemSelectTarget with handlers
		/// </summary>
		public ItemSelectTarget(Action<Mobile, TItem> success, Action<Mobile> fail)
			: base(success, fail)
		{ }

		/// <summary>
		///     Create an instance of ItemSelectTarget with handlers and additional options
		/// </summary>
		public ItemSelectTarget(
			Action<Mobile, TItem> success,
			Action<Mobile> fail,
			int range,
			bool allowGround,
			TargetFlags flags)
			: base(success, fail, range, allowGround, flags)
		{ }
	}

	/// <summary>
	///     Provides methods for selecting specific Items of the given Type
	/// </summary>
	public class ItemSelectTarget : ItemSelectTarget<Item>
	{
		/// <summary>
		///     Create an instance of ItemSelectTarget
		/// </summary>
		public ItemSelectTarget()
			: base(null, null)
		{ }

		/// <summary>
		///     Create an instance of ItemSelectTarget with additional options
		/// </summary>
		public ItemSelectTarget(int range, bool allowGround, TargetFlags flags)
			: base(null, null, range, allowGround, flags)
		{ }

		/// <summary>
		///     Create an instance of ItemSelectTarget with handlers
		/// </summary>
		public ItemSelectTarget(Action<Mobile, Item> success, Action<Mobile> fail)
			: base(success, fail)
		{ }

		/// <summary>
		///     Create an instance of ItemSelectTarget with handlers and additional options
		/// </summary>
		public ItemSelectTarget(
			Action<Mobile, Item> success,
			Action<Mobile> fail,
			int range,
			bool allowGround,
			TargetFlags flags)
			: base(success, fail, range, allowGround, flags)
		{ }
	}
}