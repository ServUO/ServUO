using System;
using Server.Accounting;
using Server.Commands;
using Server.Network;
using Server.Targeting;

namespace Server
{
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
        public int CpuModel
        {
            get
            {
                return this.m_CpuModel;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int CpuClockSpeed
        {
            get
            {
                return this.m_CpuClockSpeed;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int CpuQuantity
        {
            get
            {
                return this.m_CpuQuantity;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int OSMajor
        {
            get
            {
                return this.m_OSMajor;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int OSMinor
        {
            get
            {
                return this.m_OSMinor;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int OSRevision
        {
            get
            {
                return this.m_OSRevision;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int InstanceID
        {
            get
            {
                return this.m_InstanceID;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ScreenWidth
        {
            get
            {
                return this.m_ScreenWidth;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ScreenHeight
        {
            get
            {
                return this.m_ScreenHeight;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ScreenDepth
        {
            get
            {
                return this.m_ScreenDepth;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PhysicalMemory
        {
            get
            {
                return this.m_PhysicalMemory;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int CpuManufacturer
        {
            get
            {
                return this.m_CpuManufacturer;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int CpuFamily
        {
            get
            {
                return this.m_CpuFamily;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int VCVendorID
        {
            get
            {
                return this.m_VCVendorID;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int VCDeviceID
        {
            get
            {
                return this.m_VCDeviceID;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int VCMemory
        {
            get
            {
                return this.m_VCMemory;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int DXMajor
        {
            get
            {
                return this.m_DXMajor;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int DXMinor
        {
            get
            {
                return this.m_DXMinor;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string VCDescription
        {
            get
            {
                return this.m_VCDescription;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Language
        {
            get
            {
                return this.m_Language;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Distribution
        {
            get
            {
                return this.m_Distribution;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ClientsRunning
        {
            get
            {
                return this.m_ClientsRunning;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ClientsInstalled
        {
            get
            {
                return this.m_ClientsInstalled;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PartialInstalled
        {
            get
            {
                return this.m_PartialInstalled;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Unknown
        {
            get
            {
                return this.m_Unknown;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeReceived
        {
            get
            {
                return this.m_TimeReceived;
            }
        }
        public static void Initialize()
        {
            PacketHandlers.Register(0xD9, 0x10C, false, new OnPacketReceive(OnReceive));

            CommandSystem.Register("HWInfo", AccessLevel.GameMaster, new CommandEventHandler(HWInfo_OnCommand));
        }

        [Usage("HWInfo")]
        [Description("Displays information about a targeted player's hardware.")]
        public static void HWInfo_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(HWInfo_OnTarget));
            e.Mobile.SendMessage("Target a player to view their hardware information.");
        }

        public static void HWInfo_OnTarget(Mobile from, object obj)
        {
            if (obj is Mobile && ((Mobile)obj).Player)
            {
                Mobile m = (Mobile)obj;
                Account acct = m.Account as Account;

                if (acct != null)
                {
                    HardwareInfo hwInfo = acct.HardwareInfo;

                    if (hwInfo != null)
                        CommandLogging.WriteLine(from, "{0} {1} viewing hardware info of {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(m));

                    if (hwInfo != null)
                        from.SendGump(new Gumps.PropertiesGump(from, hwInfo));
                    else
                        from.SendMessage("No hardware information for that account was found.");
                }
                else
                {
                    from.SendMessage("No account has been attached to that player.");
                }
            }
            else
            {
                from.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(HWInfo_OnTarget));
                from.SendMessage("That is not a player. Try again.");
            }
        }

        public static void OnReceive(NetState state, PacketReader pvSrc)
        {
            pvSrc.ReadByte(); // 1: <4.0.1a, 2>=4.0.1a

            HardwareInfo info = new HardwareInfo();

            info.m_InstanceID = pvSrc.ReadInt32();
            info.m_OSMajor = pvSrc.ReadInt32();
            info.m_OSMinor = pvSrc.ReadInt32();
            info.m_OSRevision = pvSrc.ReadInt32();
            info.m_CpuManufacturer = pvSrc.ReadByte();
            info.m_CpuFamily = pvSrc.ReadInt32();
            info.m_CpuModel = pvSrc.ReadInt32();
            info.m_CpuClockSpeed = pvSrc.ReadInt32();
            info.m_CpuQuantity = pvSrc.ReadByte();
            info.m_PhysicalMemory = pvSrc.ReadInt32();
            info.m_ScreenWidth = pvSrc.ReadInt32();
            info.m_ScreenHeight = pvSrc.ReadInt32();
            info.m_ScreenDepth = pvSrc.ReadInt32();
            info.m_DXMajor = pvSrc.ReadInt16();
            info.m_DXMinor = pvSrc.ReadInt16();
            info.m_VCDescription = pvSrc.ReadUnicodeStringLESafe(64);
            info.m_VCVendorID = pvSrc.ReadInt32();
            info.m_VCDeviceID = pvSrc.ReadInt32();
            info.m_VCMemory = pvSrc.ReadInt32();
            info.m_Distribution = pvSrc.ReadByte();
            info.m_ClientsRunning = pvSrc.ReadByte();
            info.m_ClientsInstalled = pvSrc.ReadByte();
            info.m_PartialInstalled = pvSrc.ReadByte();
            info.m_Language = pvSrc.ReadUnicodeStringLESafe(4);
            info.m_Unknown = pvSrc.ReadStringSafe(64);

            info.m_TimeReceived = DateTime.UtcNow;

            Account acct = state.Account as Account;

            if (acct != null)
                acct.HardwareInfo = info;
        }
    }
}