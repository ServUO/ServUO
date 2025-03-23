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
using Server;
#endregion

namespace VitaNex
{
	[PropertyObject]
	public abstract class PropertyObject : IHued
	{
		int IHued.HuedItemID => 2278;

		[CommandProperty(AccessLevel.Administrator)]
		public virtual bool InvokeClear
		{
			get => true;
			set
			{
				if (value)
				{
					Clear();
				}
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public virtual bool InvokeReset
		{
			get => true;
			set
			{
				if (value)
				{
					Reset();
				}
			}
		}

		public PropertyObject()
		{ }

		public PropertyObject(GenericReader reader)
			: this(reader, false)
		{ }

		public PropertyObject(GenericReader reader, bool deferred)
			: this()
		{
			if (deferred)
			{
				Timer.DelayCall(Deserialize, reader);
			}
			else
			{
				Deserialize(reader);
			}
		}

		public virtual void Clear()
		{ }

		public virtual void Reset()
		{ }

		public virtual void Serialize(GenericWriter writer)
		{
			writer.SetVersion(0);
		}

		public virtual void Deserialize(GenericReader reader)
		{
			reader.GetVersion();
		}

		public override string ToString()
		{
			return "...";
		}
	}
}