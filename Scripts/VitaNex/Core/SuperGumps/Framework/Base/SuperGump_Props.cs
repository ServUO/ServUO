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
using System.Linq;

using Server;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract partial class SuperGump
	{
		public void AddProperties(IEntity entity)
		{
			if (entity is Item)
			{
				AddProperties((Item)entity);
			}
			else if (entity is Mobile)
			{
				AddProperties((Mobile)entity);
			}
			else if (entity is Spoof)
			{
				AddProperties((Spoof)entity);
			}
			else
			{
				AddProperties(entity.Serial);
			}
		}

		public virtual void AddProperties(Item item)
		{
			if (item == null || item.Deleted)
			{
				return;
			}

			if (item.Parent != User && User.IsOnline() && GetEntries<GumpOPL>().All(o => o.Serial != item.Serial.Value))
			{
				var opl = item.GetOPL(User);
				var inf = new OPLInfo(opl);

				User.Send(opl);
				User.Send(inf);
			}

			AddProperties(item.Serial);
		}

		public virtual void AddProperties(Mobile mob)
		{
			if (mob == null || mob.Deleted)
			{
				return;
			}

			if (mob != User && User.IsOnline() && GetEntries<GumpOPL>().All(o => o.Serial != mob.Serial.Value))
			{
				var opl = mob.GetOPL(User);
				var inf = new OPLInfo(opl);

				User.Send(opl);
				User.Send(inf);
			}

			AddProperties(mob.Serial);
		}

		private void AddProperties(Spoof spoof)
		{
			if (spoof == null || spoof.Deleted)
			{
				return;
			}

			if (User.IsOnline() && GetEntries<GumpOPL>().All(o => o.Serial != spoof.Serial.Value))
			{
				User.Send(spoof.PropertyList);
				User.Send(spoof.OPLPacket);
			}

			AddProperties(spoof.Serial);
		}

		public virtual void AddProperties(Serial serial)
		{
			Add(new GumpOPL(serial));
		}
	}
}
