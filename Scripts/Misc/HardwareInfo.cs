#region Header
// **********
// ServUO - HardwareInfo.cs
// **********
#endregion

#region References
using System;

using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
#endregion

namespace Server
{
	[PropertyObject]
	public class HardwareInfo
	{
		private int m_InstanceID;
		private int m_OSMajor, m_OSMinor, m_OSRevision;
		private int m_CpuManufacturer, m_CpuFamily, m_CpuModel, m_CpuClockSpeed, m_CpuQuantity;
		private int m_PhysicalMemory;
		private int m_ScreenWidth, m_ScreenHeight, m_ScreenDepth;
		private int m_DXMajor, m_DXMinor;
		private int m_VCVendorID, m_VCDeviceID, m_VCMemory;
		private int m_Distribution, m_ClientsRunning, m_ClientsInstalled, m_PartialInstalled;
		private string m_VCDescription;
		private string m_Language;
		private string m_Unknown;
		private DateTime m_TimeReceived;

		[CommandProperty(AccessLevel.GameMaster)]
		public int CpuModel { get { return m_CpuModel; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int CpuClockSpeed { get { return m_CpuClockSpeed; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int CpuQuantity { get { return m_CpuQuantity; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int OSMajor { get { return m_OSMajor; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int OSMinor { get { return m_OSMinor; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int OSRevision { get { return m_OSRevision; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int InstanceID { get { return m_InstanceID; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ScreenWidth { get { return m_ScreenWidth; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ScreenHeight { get { return m_ScreenHeight; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ScreenDepth { get { return m_ScreenDepth; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int PhysicalMemory { get { return m_PhysicalMemory; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int CpuManufacturer { get { return m_CpuManufacturer; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int CpuFamily { get { return m_CpuFamily; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int VCVendorID { get { return m_VCVendorID; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int VCDeviceID { get { return m_VCDeviceID; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int VCMemory { get { return m_VCMemory; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DXMajor { get { return m_DXMajor; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DXMinor { get { return m_DXMinor; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string VCDescription { get { return m_VCDescription; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string Language { get { return m_Language; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Distribution { get { return m_Distribution; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ClientsRunning { get { return m_ClientsRunning; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ClientsInstalled { get { return m_ClientsInstalled; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int PartialInstalled { get { return m_PartialInstalled; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string Unknown { get { return m_Unknown; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime TimeReceived { get { return m_TimeReceived; } }

		public static void Initialize()
		{
			PacketHandlers.Register(0xD9, 0x10C, false, OnReceive);

			CommandSystem.Register("HWInfo", AccessLevel.GameMaster, HWInfo_OnCommand);
		}

		[Usage("HWInfo")]
		[Description("Displays information about a targeted player's hardware.")]
		public static void HWInfo_OnCommand(CommandEventArgs e)
		{
			e.Mobile.BeginTarget(-1, false, TargetFlags.None, HWInfo_OnTarget);
			e.Mobile.SendMessage("Target a player to view their hardware information.");
		}

		public static void HWInfo_OnTarget(Mobile from, object obj)
		{
			if (obj is Mobile && ((Mobile)obj).Player)
			{
				var m = (Mobile)obj;
				var acct = m.Account as Account;

				if (acct != null)
				{
					var hwInfo = acct.HardwareInfo;

					if (hwInfo != null)
					{
						CommandLogging.WriteLine(
							from,
							"{0} {1} viewing hardware info of {2}",
							from.AccessLevel,
							CommandLogging.Format(from),
							CommandLogging.Format(m));
					}

					if (hwInfo != null)
					{
						from.SendGump(new PropertiesGump(from, hwInfo));
					}
					else
					{
						from.SendMessage("No hardware information for that account was found.");
					}
				}
				else
				{
					from.SendMessage("No account has been attached to that player.");
				}
			}
			else
			{
				from.BeginTarget(-1, false, TargetFlags.None, HWInfo_OnTarget);
				from.SendMessage("That is not a player. Try again.");
			}
		}

		public static void OnReceive(NetState state, PacketReader pvSrc)
		{
			pvSrc.ReadByte(); // 1: <4.0.1a, 2>=4.0.1a

			var info = new HardwareInfo
			{
				m_InstanceID = pvSrc.ReadInt32(),
				m_OSMajor = pvSrc.ReadInt32(),
				m_OSMinor = pvSrc.ReadInt32(),
				m_OSRevision = pvSrc.ReadInt32(),
				m_CpuManufacturer = pvSrc.ReadByte(),
				m_CpuFamily = pvSrc.ReadInt32(),
				m_CpuModel = pvSrc.ReadInt32(),
				m_CpuClockSpeed = pvSrc.ReadInt32(),
				m_CpuQuantity = pvSrc.ReadByte(),
				m_PhysicalMemory = pvSrc.ReadInt32(),
				m_ScreenWidth = pvSrc.ReadInt32(),
				m_ScreenHeight = pvSrc.ReadInt32(),
				m_ScreenDepth = pvSrc.ReadInt32(),
				m_DXMajor = pvSrc.ReadInt16(),
				m_DXMinor = pvSrc.ReadInt16(),
				m_VCDescription = pvSrc.ReadUnicodeStringLESafe(64),
				m_VCVendorID = pvSrc.ReadInt32(),
				m_VCDeviceID = pvSrc.ReadInt32(),
				m_VCMemory = pvSrc.ReadInt32(),
				m_Distribution = pvSrc.ReadByte(),
				m_ClientsRunning = pvSrc.ReadByte(),
				m_ClientsInstalled = pvSrc.ReadByte(),
				m_PartialInstalled = pvSrc.ReadByte(),
				m_Language = pvSrc.ReadUnicodeStringLESafe(4),
				m_Unknown = pvSrc.ReadStringSafe(64),
				m_TimeReceived = DateTime.UtcNow
			};

			var acct = state.Account as Account;

			if (acct != null)
			{
				acct.HardwareInfo = info;
			}
		}
	}
}