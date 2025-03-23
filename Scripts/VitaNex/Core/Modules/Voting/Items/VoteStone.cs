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
using System.Drawing;

using Server;
using Server.Misc;
using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.Voting
{
	public class VotingStone : Item
	{
		public override bool DisplayLootType => false;
		public override bool DisplayWeight => false;

		private int _SiteUID;

		[CommandProperty(Voting.Access)]
		public int SiteUID
		{
			get => _SiteUID;
			set
			{
				_SiteUID = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(Voting.Access)]
		public KnownColor UsageColor { get; set; }

		[Constructable]
		public VotingStone()
			: this(4963)
		{ }

		[Constructable]
		public VotingStone(int siteUID)
			: base(4963)
		{
			SiteUID = siteUID;
			UsageColor = KnownColor.SkyBlue;

			Name = "Voting Stone";
			LootType = LootType.Blessed;
			Weight = 0;
		}

		public VotingStone(Serial serial)
			: base(serial)
		{ }

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			var site = Voting.FindSite(SiteUID);

			if (site != null && !site.Deleted && site.Valid)
			{
				list.Add("Use: Cast a vote for {0} at '{1}'".WrapUOHtmlColor(UsageColor), ServerList.ServerName, site.Name);
			}
			else
			{
				list.Add("[No Vote Site Available]".WrapUOHtmlColor(Color.OrangeRed));
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			var voter = from as PlayerMobile;

			if (voter == null || voter.Deleted)
			{
				return;
			}

			var site = Voting.FindSite(SiteUID);

			if (site != null)
			{
				site.Vote(voter);
			}
			else if (voter.AccessLevel >= Voting.Access)
			{
				SuperGump.Send(new VoteAdminGump(voter));
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteFlag(UsageColor);
					writer.Write(_SiteUID);
				}
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
				{
					UsageColor = reader.ReadFlag<KnownColor>();
					_SiteUID = reader.ReadInt();
				}
				break;
			}
		}
	}
}