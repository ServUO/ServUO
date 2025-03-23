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
using Server.Mobiles;

using VitaNex.FX;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class CTFPodium : Item
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual CTFTeam Team { get; set; }

		public override bool HandlesOnMovement => true;

		public CTFPodium(CTFTeam team)
			: base(16144)
		{
			Team = team;

			Name = Team.Name;
			Hue = Team.Color;
			Movable = false;
		}

		public CTFPodium(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile from)
		{
			if (this.CheckDoubleClick(from, true, false, 2))
			{
				CheckCapture(from as PlayerMobile);
			}
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			base.OnMovement(m, oldLocation);

			CheckCapture(m as PlayerMobile);
		}

		public virtual void CheckCapture(PlayerMobile attacker)
		{
			if (Team == null || Team.Deleted || Team.Flag == null || Team.Flag.Deleted)
			{
				return;
			}

			if (attacker == null || attacker.Deleted || !attacker.InRange3D(this, 2, -10, 10) || !Team.IsMember(attacker))
			{
				return;
			}

			if (Team.Flag.Carrier != null && !Team.Flag.Carrier.Deleted)
			{
				return;
			}

			Team.Battle.ForEachTeam<CTFTeam>(
				t =>
				{
					if (t == Team || t.Flag == null || t.Flag.Deleted || t.Flag.Carrier != attacker)
					{
						return;
					}

					t.Flag.Capture(attacker);

					ExplodeFX.Random.CreateInstance(this, Map, 3, 0, null, e => e.Hue = t.Color).Send();
				});
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}