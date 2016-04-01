#region Header
// **********
// ServUO - VendorMallManager.cs
// **********
#endregion

#region References
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class VendorMallManager : BaseVendor
	{
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		public override NpcGuild NpcGuild { get { return NpcGuild.MerchantsGuild; } }

		[Constructable]
		public VendorMallManager()
			: base("The Mall Manager")
		{
			CantWalk = true;
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add(new SBVendorMallManager());
		}

		public static int GetBalance(Mobile m)
		{
			return Banker.GetBalance(m);
		}

		public static int GetBalance(Mobile m, out Item[] gold, out Item[] checks)
		{
			return Banker.GetBalance(m, out gold, out checks);
		}

		public static bool Withdraw(Mobile from, int amount)
		{
			return Banker.Withdraw(from, amount);
		}

		public static bool Deposit(Mobile from, int amount)
		{
			return Banker.Deposit(from, amount);
		}

		public static int DepositUpTo(Mobile from, int amount)
		{
			return Banker.DepositUpTo(from, amount);
		}

		public static void Deposit(Container cont, int amount)
		{
			Banker.Deposit(cont, amount);
		}

		public VendorMallManager(Serial serial)
			: base(serial)
		{ }

		public override bool HandlesOnSpeech(Mobile from)
		{
			if (from.InRange(Location, 12))
			{
				return true;
			}

			return base.HandlesOnSpeech(from);
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			Banker.HandleSpeech(this, e);

			base.OnSpeech(e);
		}

		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (from.Alive)
			{
				list.Add(new OpenBankEntry(from, this));
			}

			base.AddCustomContextEntries(from, list);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}