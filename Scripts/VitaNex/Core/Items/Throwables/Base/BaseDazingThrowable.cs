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
using Server.Mobiles;
#endregion

namespace VitaNex.Items
{
	public abstract class BaseDazingThrowable<TMobile> : BaseThrowableAtMobile<TMobile>
		where TMobile : Mobile
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual TimeSpan MountRecovery { get; set; }

		public BaseDazingThrowable(int itemID, int amount = 1)
			: base(itemID, amount)
		{
			MountRecovery = TimeSpan.FromSeconds(3.0);
		}

		public BaseDazingThrowable(Serial serial)
			: base(serial)
		{ }

		public override bool CanThrowAt(Mobile from, TMobile target, bool message)
		{
			if (!base.CanThrowAt(from, target, message))
			{
				return false;
			}

			if (!target.Mounted)
			{
				if (message)
				{
					from.SendMessage(37, "Your target is not mounted.");
				}

				return false;
			}

			return true;
		}

		protected override void OnThrownAt(Mobile from, TMobile target)
		{
			base.OnThrownAt(from, target);

			var mounted = target.Mounted;

			if (target is PlayerMobile)
			{
				var pm = target as PlayerMobile;

				pm.SetMountBlock(BlockMountType.Dazed, MountRecovery, mounted);
			}

			if (!mounted)
			{
				return;
			}

			BaseMount.Dismount(target);
			OnTargetDismounted(from, target, true);
		}

		protected virtual void OnTargetDismounted(Mobile from, TMobile target, bool message)
		{
			if (from == null || from.Deleted || target == null || target.Deleted)
			{
				return;
			}

			if (message)
			{
				target.SendMessage(37, "{0} has brought you down with the {1}!", from.RawName, Name);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(MountRecovery);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
					MountRecovery = reader.ReadTimeSpan();
					break;
			}
		}
	}
}