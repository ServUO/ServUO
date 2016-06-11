#region Header
// **********
// ServUO - EventSink.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

using Server.Accounting;
using Server.Commands;
using Server.Guilds;
using Server.Network;
#endregion

namespace Server
{
	public delegate void CharacterCreatedEventHandler(CharacterCreatedEventArgs e);

	public delegate void OpenDoorMacroEventHandler(OpenDoorMacroEventArgs e);

	public delegate void SpeechEventHandler(SpeechEventArgs e);

	public delegate void LoginEventHandler(LoginEventArgs e);

	public delegate void ServerListEventHandler(ServerListEventArgs e);

	public delegate void MovementEventHandler(MovementEventArgs e);

	public delegate void HungerChangedEventHandler(HungerChangedEventArgs e);

	public delegate void CrashedEventHandler(CrashedEventArgs e);

	public delegate void ShutdownEventHandler(ShutdownEventArgs e);

	public delegate void HelpRequestEventHandler(HelpRequestEventArgs e);

	public delegate void DisarmRequestEventHandler(DisarmRequestEventArgs e);

	public delegate void StunRequestEventHandler(StunRequestEventArgs e);

	public delegate void OpenSpellbookRequestEventHandler(OpenSpellbookRequestEventArgs e);

	public delegate void CastSpellRequestEventHandler(CastSpellRequestEventArgs e);

	public delegate void BandageTargetRequestEventHandler(BandageTargetRequestEventArgs e);

	public delegate void AnimateRequestEventHandler(AnimateRequestEventArgs e);

	public delegate void LogoutEventHandler(LogoutEventArgs e);

	public delegate void SocketConnectEventHandler(SocketConnectEventArgs e);

	public delegate void ConnectedEventHandler(ConnectedEventArgs e);

	public delegate void DisconnectedEventHandler(DisconnectedEventArgs e);

	public delegate void RenameRequestEventHandler(RenameRequestEventArgs e);

	public delegate void PlayerDeathEventHandler(PlayerDeathEventArgs e);

	public delegate void VirtueGumpRequestEventHandler(VirtueGumpRequestEventArgs e);

	public delegate void VirtueItemRequestEventHandler(VirtueItemRequestEventArgs e);

	public delegate void VirtueMacroRequestEventHandler(VirtueMacroRequestEventArgs e);

	public delegate void ChatRequestEventHandler(ChatRequestEventArgs e);

	public delegate void AccountLoginEventHandler(AccountLoginEventArgs e);

	public delegate void PaperdollRequestEventHandler(PaperdollRequestEventArgs e);

	public delegate void ProfileRequestEventHandler(ProfileRequestEventArgs e);

	public delegate void ChangeProfileRequestEventHandler(ChangeProfileRequestEventArgs e);

	public delegate void AggressiveActionEventHandler(AggressiveActionEventArgs e);

	public delegate void GameLoginEventHandler(GameLoginEventArgs e);

	public delegate void DeleteRequestEventHandler(DeleteRequestEventArgs e);

	public delegate void WorldLoadEventHandler();

	public delegate void WorldSaveEventHandler(WorldSaveEventArgs e);

    public delegate void BeforeWorldSaveEventHandler(BeforeWorldSaveEventArgs e);

    public delegate void AfterWorldSaveEventHandler(AfterWorldSaveEventArgs e);

    public delegate void SetAbilityEventHandler(SetAbilityEventArgs e);

	public delegate void FastWalkEventHandler(FastWalkEventArgs e);

	public delegate void ServerStartedEventHandler();

	public delegate void CreateGuildHandler(CreateGuildEventArgs e);

	public delegate void GuildGumpRequestHandler(GuildGumpRequestArgs e);

	public delegate void QuestGumpRequestHandler(QuestGumpRequestArgs e);

	public delegate void ClientVersionReceivedHandler(ClientVersionReceivedArgs e);

	public delegate void OnKilledByEventHandler(OnKilledByEventArgs e);

	public delegate void OnItemUseEventHandler(OnItemUseEventArgs e);

	public delegate void OnEnterRegionEventHandler(OnEnterRegionEventArgs e);

	public delegate void OnConsumeEventHandler(OnConsumeEventArgs e);

	public delegate void OnPropertyChangedEventHandler(OnPropertyChangedEventArgs e);

	public delegate void BODUsedEventHandler(BODUsedEventArgs e);

	public delegate void BODOfferEventHandler(BODOfferEventArgs e);

	public delegate void ResourceHarvestAttemptEventHandler(ResourceHarvestAttemptEventArgs e);

	public delegate void ResourceHarvestSuccessEventHandler(ResourceHarvestSuccessEventArgs e);

	public delegate void CraftSuccessEventHandler(CraftSuccessEventArgs e);

	public delegate void ItemCreatedEventHandler(ItemCreatedEventArgs e);

	public delegate void ItemDeletedEventHandler(ItemDeletedEventArgs e);

	public delegate void MobileCreatedEventHandler(MobileCreatedEventArgs e);

	public delegate void MobileDeletedEventHandler(MobileDeletedEventArgs e);

	public class ClientVersionReceivedArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly ClientVersion m_Version;

		public NetState State { get { return m_State; } }
		public ClientVersion Version { get { return m_Version; } }

		public ClientVersionReceivedArgs(NetState state, ClientVersion cv)
		{
			m_State = state;
			m_Version = cv;
		}
	}

	public class CreateGuildEventArgs : EventArgs
	{
		public int Id { get; set; }

		public BaseGuild Guild { get; set; }

		public CreateGuildEventArgs(int id)
		{
			Id = id;
		}
	}

	public class GuildGumpRequestArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public GuildGumpRequestArgs(Mobile mobile)
		{
			m_Mobile = mobile;
		}
	}

	public class QuestGumpRequestArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public QuestGumpRequestArgs(Mobile mobile)
		{
			m_Mobile = mobile;
		}
	}

	public class SetAbilityEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly int m_Index;

		public Mobile Mobile { get { return m_Mobile; } }
		public int Index { get { return m_Index; } }

		public SetAbilityEventArgs(Mobile mobile, int index)
		{
			m_Mobile = mobile;
			m_Index = index;
		}
	}

	public class DeleteRequestEventArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly int m_Index;

		public NetState State { get { return m_State; } }
		public int Index { get { return m_Index; } }

		public DeleteRequestEventArgs(NetState state, int index)
		{
			m_State = state;
			m_Index = index;
		}
	}

	public class GameLoginEventArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly string m_Username;
		private readonly string m_Password;

		public NetState State { get { return m_State; } }
		public string Username { get { return m_Username; } }
		public string Password { get { return m_Password; } }
		public bool Accepted { get; set; }
		public CityInfo[] CityInfo { get; set; }

		public GameLoginEventArgs(NetState state, string un, string pw)
		{
			m_State = state;
			m_Username = un;
			m_Password = pw;
		}
	}

	public class AggressiveActionEventArgs : EventArgs
	{
		private Mobile m_Aggressed;
		private Mobile m_Aggressor;
		private bool m_Criminal;

		public Mobile Aggressed { get { return m_Aggressed; } }
		public Mobile Aggressor { get { return m_Aggressor; } }
		public bool Criminal { get { return m_Criminal; } }

		private static readonly Queue<AggressiveActionEventArgs> m_Pool = new Queue<AggressiveActionEventArgs>();

		public static AggressiveActionEventArgs Create(Mobile aggressed, Mobile aggressor, bool criminal)
		{
			AggressiveActionEventArgs args;

			if (m_Pool.Count > 0)
			{
				args = m_Pool.Dequeue();

				args.m_Aggressed = aggressed;
				args.m_Aggressor = aggressor;
				args.m_Criminal = criminal;
			}
			else
			{
				args = new AggressiveActionEventArgs(aggressed, aggressor, criminal);
			}

			return args;
		}

		private AggressiveActionEventArgs(Mobile aggressed, Mobile aggressor, bool criminal)
		{
			m_Aggressed = aggressed;
			m_Aggressor = aggressor;
			m_Criminal = criminal;
		}

		public void Free()
		{
			m_Pool.Enqueue(this);
		}
	}

	public class ProfileRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Beholder;
		private readonly Mobile m_Beheld;

		public Mobile Beholder { get { return m_Beholder; } }
		public Mobile Beheld { get { return m_Beheld; } }

		public ProfileRequestEventArgs(Mobile beholder, Mobile beheld)
		{
			m_Beholder = beholder;
			m_Beheld = beheld;
		}
	}

	public class ChangeProfileRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Beholder;
		private readonly Mobile m_Beheld;
		private readonly string m_Text;

		public Mobile Beholder { get { return m_Beholder; } }
		public Mobile Beheld { get { return m_Beheld; } }
		public string Text { get { return m_Text; } }

		public ChangeProfileRequestEventArgs(Mobile beholder, Mobile beheld, string text)
		{
			m_Beholder = beholder;
			m_Beheld = beheld;
			m_Text = text;
		}
	}

	public class PaperdollRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Beholder;
		private readonly Mobile m_Beheld;

		public Mobile Beholder { get { return m_Beholder; } }
		public Mobile Beheld { get { return m_Beheld; } }

		public PaperdollRequestEventArgs(Mobile beholder, Mobile beheld)
		{
			m_Beholder = beholder;
			m_Beheld = beheld;
		}
	}

	public class AccountLoginEventArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly string m_Username;
		private readonly string m_Password;

		public NetState State { get { return m_State; } }
		public string Username { get { return m_Username; } }
		public string Password { get { return m_Password; } }
		public bool Accepted { get; set; }
		public ALRReason RejectReason { get; set; }

		public AccountLoginEventArgs(NetState state, string username, string password)
		{
			m_State = state;
			m_Username = username;
			m_Password = password;
			Accepted = true;
		}
	}

	public class VirtueItemRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Beholder;
		private readonly Mobile m_Beheld;
		private readonly int m_GumpID;

		public Mobile Beholder { get { return m_Beholder; } }
		public Mobile Beheld { get { return m_Beheld; } }
		public int GumpID { get { return m_GumpID; } }

		public VirtueItemRequestEventArgs(Mobile beholder, Mobile beheld, int gumpID)
		{
			m_Beholder = beholder;
			m_Beheld = beheld;
			m_GumpID = gumpID;
		}
	}

	public class VirtueGumpRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Beholder;
		private readonly Mobile m_Beheld;

		public Mobile Beholder { get { return m_Beholder; } }
		public Mobile Beheld { get { return m_Beheld; } }

		public VirtueGumpRequestEventArgs(Mobile beholder, Mobile beheld)
		{
			m_Beholder = beholder;
			m_Beheld = beheld;
		}
	}

	public class VirtueMacroRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly int m_VirtueID;

		public Mobile Mobile { get { return m_Mobile; } }
		public int VirtueID { get { return m_VirtueID; } }

		public VirtueMacroRequestEventArgs(Mobile mobile, int virtueID)
		{
			m_Mobile = mobile;
			m_VirtueID = virtueID;
		}
	}

	public class ChatRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public ChatRequestEventArgs(Mobile mobile)
		{
			m_Mobile = mobile;
		}
	}

	public class PlayerDeathEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public PlayerDeathEventArgs(Mobile mobile)
		{
			m_Mobile = mobile;
		}
	}

	public class RenameRequestEventArgs : EventArgs
	{
		private readonly Mobile m_From;
		private readonly Mobile m_Target;
		private readonly string m_Name;

		public Mobile From { get { return m_From; } }
		public Mobile Target { get { return m_Target; } }
		public string Name { get { return m_Name; } }

		public RenameRequestEventArgs(Mobile from, Mobile target, string name)
		{
			m_From = from;
			m_Target = target;
			m_Name = name;
		}
	}

	public class LogoutEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public LogoutEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class SocketConnectEventArgs : EventArgs
	{
		private readonly Socket m_Socket;

		public Socket Socket { get { return m_Socket; } }
		public bool AllowConnection { get; set; }

		public SocketConnectEventArgs(Socket s)
		{
			m_Socket = s;
			AllowConnection = true;
		}
	}

	public class ConnectedEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public ConnectedEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class DisconnectedEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public DisconnectedEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class AnimateRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly string m_Action;

		public Mobile Mobile { get { return m_Mobile; } }
		public string Action { get { return m_Action; } }

		public AnimateRequestEventArgs(Mobile m, string action)
		{
			m_Mobile = m;
			m_Action = action;
		}
	}

	public class CastSpellRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly Item m_Spellbook;
		private readonly int m_SpellID;

		public Mobile Mobile { get { return m_Mobile; } }
		public Item Spellbook { get { return m_Spellbook; } }
		public int SpellID { get { return m_SpellID; } }

		public CastSpellRequestEventArgs(Mobile m, int spellID, Item book)
		{
			m_Mobile = m;
			m_Spellbook = book;
			m_SpellID = spellID;
		}
	}

	public class BandageTargetRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly Item m_Bandage;
		private readonly Mobile m_Target;

		public Mobile Mobile { get { return m_Mobile; } }
		public Item Bandage { get { return m_Bandage; } }
		public Mobile Target { get { return m_Target; } }

		public BandageTargetRequestEventArgs(Mobile m, Item bandage, Mobile target)
		{
			m_Mobile = m;
			m_Bandage = bandage;
			m_Target = target;
		}
	}

	public class OpenSpellbookRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly int m_Type;

		public Mobile Mobile { get { return m_Mobile; } }
		public int Type { get { return m_Type; } }

		public OpenSpellbookRequestEventArgs(Mobile m, int type)
		{
			m_Mobile = m;
			m_Type = type;
		}
	}

	public class StunRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public StunRequestEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class DisarmRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public DisarmRequestEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class HelpRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public HelpRequestEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class ShutdownEventArgs : EventArgs
	{ }

	public class CrashedEventArgs : EventArgs
	{
		private readonly Exception m_Exception;

		public Exception Exception { get { return m_Exception; } }
		public bool Close { get; set; }

		public CrashedEventArgs(Exception e)
		{
			m_Exception = e;
		}
	}

	public class HungerChangedEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly int m_OldValue;

		public Mobile Mobile { get { return m_Mobile; } }
		public int OldValue { get { return m_OldValue; } }

		public HungerChangedEventArgs(Mobile mobile, int oldValue)
		{
			m_Mobile = mobile;
			m_OldValue = oldValue;
		}
	}

	public class MovementEventArgs : EventArgs
	{
		private Mobile m_Mobile;
		private Direction m_Direction;
		private bool m_Blocked;

		public Mobile Mobile { get { return m_Mobile; } }
		public Direction Direction { get { return m_Direction; } }
		public bool Blocked { get { return m_Blocked; } set { m_Blocked = value; } }

		private static readonly Queue<MovementEventArgs> m_Pool = new Queue<MovementEventArgs>();

		public static MovementEventArgs Create(Mobile mobile, Direction dir)
		{
			MovementEventArgs args;

			if (m_Pool.Count > 0)
			{
				args = m_Pool.Dequeue();

				args.m_Mobile = mobile;
				args.m_Direction = dir;
				args.m_Blocked = false;
			}
			else
			{
				args = new MovementEventArgs(mobile, dir);
			}

			return args;
		}

		public MovementEventArgs(Mobile mobile, Direction dir)
		{
			m_Mobile = mobile;
			m_Direction = dir;
		}

		public void Free()
		{
			m_Pool.Enqueue(this);
		}
	}

	public class ServerListEventArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly IAccount m_Account;
		private readonly List<ServerInfo> m_Servers;

		public NetState State { get { return m_State; } }
		public IAccount Account { get { return m_Account; } }
		public bool Rejected { get; set; }
		public List<ServerInfo> Servers { get { return m_Servers; } }

		public void AddServer(string name, IPEndPoint address)
		{
			AddServer(name, 0, TimeZone.CurrentTimeZone, address);
		}

		public void AddServer(string name, int fullPercent, TimeZone tz, IPEndPoint address)
		{
			m_Servers.Add(new ServerInfo(name, fullPercent, tz, address));
		}

		public ServerListEventArgs(NetState state, IAccount account)
		{
			m_State = state;
			m_Account = account;
			m_Servers = new List<ServerInfo>();
		}
	}

	public struct SkillNameValue
	{
		private readonly SkillName m_Name;
		private readonly int m_Value;

		public SkillName Name { get { return m_Name; } }
		public int Value { get { return m_Value; } }

		public SkillNameValue(SkillName name, int value)
		{
			m_Name = name;
			m_Value = value;
		}
	}

	public class CharacterCreatedEventArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly IAccount m_Account;
		private readonly CityInfo m_City;
		private readonly SkillNameValue[] m_Skills;
		private readonly int m_ShirtHue;
		private readonly int m_PantsHue;
		private readonly int m_HairID;
		private readonly int m_HairHue;
		private readonly int m_BeardID;
		private readonly int m_BeardHue;
		private readonly string m_Name;
		private readonly bool m_Female;
		private readonly int m_Hue;
		private readonly int m_Str;
		private readonly int m_Dex;
		private readonly int m_Int;

		private readonly Race m_Race;

		public NetState State { get { return m_State; } }
		public IAccount Account { get { return m_Account; } }
		public Mobile Mobile { get; set; }
		public string Name { get { return m_Name; } }
		public bool Female { get { return m_Female; } }
		public int Hue { get { return m_Hue; } }
		public int Str { get { return m_Str; } }
		public int Dex { get { return m_Dex; } }
		public int Int { get { return m_Int; } }
		public CityInfo City { get { return m_City; } }
		public SkillNameValue[] Skills { get { return m_Skills; } }
		public int ShirtHue { get { return m_ShirtHue; } }
		public int PantsHue { get { return m_PantsHue; } }
		public int HairID { get { return m_HairID; } }
		public int HairHue { get { return m_HairHue; } }
		public int BeardID { get { return m_BeardID; } }
		public int BeardHue { get { return m_BeardHue; } }
		public int Profession { get; set; }
		public Race Race { get { return m_Race; } }

		public CharacterCreatedEventArgs(
			NetState state,
			IAccount a,
			string name,
			bool female,
			int hue,
			int str,
			int dex,
			int intel,
			CityInfo city,
			SkillNameValue[] skills,
			int shirtHue,
			int pantsHue,
			int hairID,
			int hairHue,
			int beardID,
			int beardHue,
			int profession,
			Race race)
		{
			m_State = state;
			m_Account = a;
			m_Name = name;
			m_Female = female;
			m_Hue = hue;
			m_Str = str;
			m_Dex = dex;
			m_Int = intel;
			m_City = city;
			m_Skills = skills;
			m_ShirtHue = shirtHue;
			m_PantsHue = pantsHue;
			m_HairID = hairID;
			m_HairHue = hairHue;
			m_BeardID = beardID;
			m_BeardHue = beardHue;
			Profession = profession;
			m_Race = race;
		}
	}

	public class OpenDoorMacroEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public OpenDoorMacroEventArgs(Mobile mobile)
		{
			m_Mobile = mobile;
		}
	}

	public class SpeechEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly MessageType m_Type;
		private readonly int m_Hue;
		private readonly int[] m_Keywords;

		public Mobile Mobile { get { return m_Mobile; } }
		public string Speech { get; set; }
		public MessageType Type { get { return m_Type; } }
		public int Hue { get { return m_Hue; } }
		public int[] Keywords { get { return m_Keywords; } }
		public bool Handled { get; set; }
		public bool Blocked { get; set; }

		public bool HasKeyword(int keyword)
		{
			for (int i = 0; i < m_Keywords.Length; ++i)
			{
				if (m_Keywords[i] == keyword)
				{
					return true;
				}
			}

			return false;
		}

		public SpeechEventArgs(Mobile mobile, string speech, MessageType type, int hue, int[] keywords)
		{
			m_Mobile = mobile;
			Speech = speech;
			m_Type = type;
			m_Hue = hue;
			m_Keywords = keywords;
		}
	}

	public class LoginEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public LoginEventArgs(Mobile mobile)
		{
			m_Mobile = mobile;
		}
	}

	public class WorldSaveEventArgs : EventArgs
	{
		private readonly bool m_Msg;

		public bool Message { get { return m_Msg; } }

		public WorldSaveEventArgs(bool msg)
		{
			m_Msg = msg;
		}
	}

    public class BeforeWorldSaveEventArgs : EventArgs
    {
        public BeforeWorldSaveEventArgs()
        {
        }
    }


    public class AfterWorldSaveEventArgs : EventArgs
    {
        public AfterWorldSaveEventArgs()
        {
        }
    }


    public class FastWalkEventArgs : EventArgs
	{
		private readonly NetState m_State;

		public FastWalkEventArgs(NetState state)
		{
			m_State = state;
			Blocked = false;
		}

		public NetState NetState { get { return m_State; } }
		public bool Blocked { get; set; }
	}

	public class OnKilledByEventArgs : EventArgs
	{
		private readonly Mobile m_Killed;
		private readonly Mobile m_KilledBy;

		public OnKilledByEventArgs(Mobile killed, Mobile killedBy)
		{
			m_Killed = killed;
			m_KilledBy = killedBy;
		}

		public Mobile Killed { get { return m_Killed; } }
		public Mobile KilledBy { get { return m_KilledBy; } }
	}

	public class OnItemUseEventArgs : EventArgs
	{
		private readonly Mobile m_From;
		private readonly Item m_Item;

		public OnItemUseEventArgs(Mobile from, Item item)
		{
			m_From = from;
			m_Item = item;
		}

		public Mobile From { get { return m_From; } }
		public Item Item { get { return m_Item; } }
	}

	public class OnEnterRegionEventArgs : EventArgs
	{
		private readonly Mobile m_From;
		private readonly Region m_Region;

		public OnEnterRegionEventArgs(Mobile from, Region region)
		{
			m_From = from;
			m_Region = region;
		}

		public Mobile From { get { return m_From; } }
		public Region Region { get { return m_Region; } }
	}

	public class OnConsumeEventArgs : EventArgs
	{
		private readonly Mobile m_Consumer;
		private readonly Item m_Consumed;
		private readonly int m_Quantity;

		public OnConsumeEventArgs(Mobile consumer, Item consumed)
			: this(consumer, consumed, 1)
		{ }

		public OnConsumeEventArgs(Mobile consumer, Item consumed, int quantity)
		{
			m_Consumer = consumer;
			m_Consumed = consumed;
			m_Quantity = quantity;
		}

		public Mobile Consumer { get { return m_Consumer; } }

		public Item Consumed { get { return m_Consumed; } }

		public int Quantity { get { return m_Quantity; } }
	}

	public class OnPropertyChangedEventArgs : EventArgs
	{
		public Mobile Mobile { get; private set; }
		public PropertyInfo Property { get; private set; }
		public object Instance { get; private set; }
		public object OldValue { get; private set; }
		public object NewValue { get; private set; }

		public OnPropertyChangedEventArgs(Mobile m, object instance, PropertyInfo prop, object oldValue, object newValue)
		{
			Mobile = m;
			Property = prop;
			Instance = instance;
			OldValue = oldValue;
			NewValue = newValue;
		}
	}

	public class BODUsedEventArgs : EventArgs
	{
		public Mobile User { get; private set; }
		public Item BODItem { get; private set; }

		public BODUsedEventArgs(Mobile m, Item i)
		{
			User = m;
			BODItem = i;
		}
	}

	public class BODOfferEventArgs : EventArgs
	{
		public Mobile Player { get; private set; }
		public Mobile Vendor { get; private set; }

		public BODOfferEventArgs(Mobile p, Mobile v)
		{
			Player = p;
			Vendor = v;
		}
	}

	public class ResourceHarvestAttemptEventArgs : EventArgs
	{
		public Mobile Harvester { get; private set; }
		public Item Tool { get; private set; }
		public object HarvestSystem { get; private set; }

		public ResourceHarvestAttemptEventArgs(Mobile m, Item i, object o)
		{
			Harvester = m;
			Tool = i;
			HarvestSystem = o;
		}
	}

	public class ResourceHarvestSuccessEventArgs : EventArgs
	{
		public Mobile Harvester { get; private set; }
		public Item Tool { get; private set; }
		public Item Resource { get; private set; }
		public object HarvestSystem { get; private set; }

		public ResourceHarvestSuccessEventArgs(Mobile m, Item i, Item r, object o)
		{
			Harvester = m;
			Tool = i;
			Resource = r;
			HarvestSystem = o;
		}
	}

	public class CraftSuccessEventArgs : EventArgs
	{
		public Mobile Crafter { get; private set; }
		public Item Tool { get; private set; }
		public Item CraftedItem { get; private set; }

		public CraftSuccessEventArgs(Mobile m, Item i, Item t)
		{
			Crafter = m;
			Tool = t;
			CraftedItem = i;
		}
	}

	public class ItemCreatedEventArgs : EventArgs
	{
		public Item Item { get; set; }

		public ItemCreatedEventArgs(Item item)
		{
			Item = item;
		}
	}

	public class ItemDeletedEventArgs : EventArgs
	{
		public Item Item { get; set; }

		public ItemDeletedEventArgs(Item item)
		{
			Item = item;
		}
	}

	public class MobileCreatedEventArgs : EventArgs
	{
		public Mobile Mobile { get; set; }

		public MobileCreatedEventArgs(Mobile mobile)
		{
			Mobile = mobile;
		}
	}

	public class MobileDeletedEventArgs : EventArgs
	{
		public Mobile Mobile { get; set; }

		public MobileDeletedEventArgs(Mobile mobile)
		{
			Mobile = mobile;
		}
	}

	public static class EventSink
	{
		public static event CharacterCreatedEventHandler CharacterCreated;
		public static event OpenDoorMacroEventHandler OpenDoorMacroUsed;
		public static event SpeechEventHandler Speech;
		public static event LoginEventHandler Login;
		public static event ServerListEventHandler ServerList;
		public static event MovementEventHandler Movement;
		public static event HungerChangedEventHandler HungerChanged;
		public static event CrashedEventHandler Crashed;
		public static event ShutdownEventHandler Shutdown;
		public static event HelpRequestEventHandler HelpRequest;
		public static event DisarmRequestEventHandler DisarmRequest;
		public static event StunRequestEventHandler StunRequest;
		public static event OpenSpellbookRequestEventHandler OpenSpellbookRequest;
		public static event CastSpellRequestEventHandler CastSpellRequest;
		public static event BandageTargetRequestEventHandler BandageTargetRequest;
		public static event AnimateRequestEventHandler AnimateRequest;
		public static event LogoutEventHandler Logout;
		public static event SocketConnectEventHandler SocketConnect;
		public static event ConnectedEventHandler Connected;
		public static event DisconnectedEventHandler Disconnected;
		public static event RenameRequestEventHandler RenameRequest;
		public static event PlayerDeathEventHandler PlayerDeath;
		public static event VirtueGumpRequestEventHandler VirtueGumpRequest;
		public static event VirtueItemRequestEventHandler VirtueItemRequest;
		public static event VirtueMacroRequestEventHandler VirtueMacroRequest;
		public static event ChatRequestEventHandler ChatRequest;
		public static event AccountLoginEventHandler AccountLogin;
		public static event PaperdollRequestEventHandler PaperdollRequest;
		public static event ProfileRequestEventHandler ProfileRequest;
		public static event ChangeProfileRequestEventHandler ChangeProfileRequest;
		public static event AggressiveActionEventHandler AggressiveAction;
		public static event CommandEventHandler Command;
		public static event GameLoginEventHandler GameLogin;
		public static event DeleteRequestEventHandler DeleteRequest;
		public static event WorldLoadEventHandler WorldLoad;
		public static event WorldSaveEventHandler WorldSave;
        public static event BeforeWorldSaveEventHandler BeforeWorldSave;
        public static event AfterWorldSaveEventHandler AfterWorldSave;
        public static event SetAbilityEventHandler SetAbility;
		public static event FastWalkEventHandler FastWalk;
		public static event CreateGuildHandler CreateGuild;
		public static event ServerStartedEventHandler ServerStarted;
		public static event GuildGumpRequestHandler GuildGumpRequest;
		public static event QuestGumpRequestHandler QuestGumpRequest;
		public static event ClientVersionReceivedHandler ClientVersionReceived;
		public static event OnKilledByEventHandler OnKilledBy;
		public static event OnItemUseEventHandler OnItemUse;
		public static event OnEnterRegionEventHandler OnEnterRegion;
		public static event OnConsumeEventHandler OnConsume;
		public static event OnPropertyChangedEventHandler OnPropertyChanged;
		public static event BODUsedEventHandler BODUsed;
		public static event BODOfferEventHandler BODOffered;
		public static event ResourceHarvestAttemptEventHandler ResourceHarvestAttempt;
		public static event ResourceHarvestSuccessEventHandler ResourceHarvestSuccess;
		public static event CraftSuccessEventHandler CraftSuccess;

		public static event ItemCreatedEventHandler ItemCreated;
		public static event ItemDeletedEventHandler ItemDeleted;
		public static event MobileCreatedEventHandler MobileCreated;
		public static event MobileDeletedEventHandler MobileDeleted;

		public static void InvokeClientVersionReceived(ClientVersionReceivedArgs e)
		{
			if (ClientVersionReceived != null)
			{
				ClientVersionReceived(e);
			}
		}

		public static void InvokeServerStarted()
		{
			if (ServerStarted != null)
			{
				ServerStarted();
			}
		}

		public static void InvokeCreateGuild(CreateGuildEventArgs e)
		{
			if (CreateGuild != null)
			{
				CreateGuild(e);
			}
		}

		public static void InvokeSetAbility(SetAbilityEventArgs e)
		{
			if (SetAbility != null)
			{
				SetAbility(e);
			}
		}

		public static void InvokeGuildGumpRequest(GuildGumpRequestArgs e)
		{
			if (GuildGumpRequest != null)
			{
				GuildGumpRequest(e);
			}
		}

		public static void InvokeQuestGumpRequest(QuestGumpRequestArgs e)
		{
			if (QuestGumpRequest != null)
			{
				QuestGumpRequest(e);
			}
		}

		public static void InvokeFastWalk(FastWalkEventArgs e)
		{
			if (FastWalk != null)
			{
				FastWalk(e);
			}
		}

		public static void InvokeDeleteRequest(DeleteRequestEventArgs e)
		{
			if (DeleteRequest != null)
			{
				DeleteRequest(e);
			}
		}

		public static void InvokeGameLogin(GameLoginEventArgs e)
		{
			if (GameLogin != null)
			{
				GameLogin(e);
			}
		}

		public static void InvokeCommand(CommandEventArgs e)
		{
			if (Command != null)
			{
				Command(e);
			}
		}

		public static void InvokeAggressiveAction(AggressiveActionEventArgs e)
		{
			if (AggressiveAction != null)
			{
				AggressiveAction(e);
			}
		}

		public static void InvokeProfileRequest(ProfileRequestEventArgs e)
		{
			if (ProfileRequest != null)
			{
				ProfileRequest(e);
			}
		}

		public static void InvokeChangeProfileRequest(ChangeProfileRequestEventArgs e)
		{
			if (ChangeProfileRequest != null)
			{
				ChangeProfileRequest(e);
			}
		}

		public static void InvokePaperdollRequest(PaperdollRequestEventArgs e)
		{
			if (PaperdollRequest != null)
			{
				PaperdollRequest(e);
			}
		}

		public static void InvokeAccountLogin(AccountLoginEventArgs e)
		{
			if (AccountLogin != null)
			{
				AccountLogin(e);
			}
		}

		public static void InvokeChatRequest(ChatRequestEventArgs e)
		{
			if (ChatRequest != null)
			{
				ChatRequest(e);
			}
		}

		public static void InvokeVirtueItemRequest(VirtueItemRequestEventArgs e)
		{
			if (VirtueItemRequest != null)
			{
				VirtueItemRequest(e);
			}
		}

		public static void InvokeVirtueGumpRequest(VirtueGumpRequestEventArgs e)
		{
			if (VirtueGumpRequest != null)
			{
				VirtueGumpRequest(e);
			}
		}

		public static void InvokeVirtueMacroRequest(VirtueMacroRequestEventArgs e)
		{
			if (VirtueMacroRequest != null)
			{
				VirtueMacroRequest(e);
			}
		}

		public static void InvokePlayerDeath(PlayerDeathEventArgs e)
		{
			if (PlayerDeath != null)
			{
				PlayerDeath(e);
			}
		}

		public static void InvokeRenameRequest(RenameRequestEventArgs e)
		{
			if (RenameRequest != null)
			{
				RenameRequest(e);
			}
		}

		public static void InvokeLogout(LogoutEventArgs e)
		{
			if (Logout != null)
			{
				Logout(e);
			}
		}

		public static void InvokeSocketConnect(SocketConnectEventArgs e)
		{
			if (SocketConnect != null)
			{
				SocketConnect(e);
			}
		}

		public static void InvokeConnected(ConnectedEventArgs e)
		{
			if (Connected != null)
			{
				Connected(e);
			}
		}

		public static void InvokeDisconnected(DisconnectedEventArgs e)
		{
			if (Disconnected != null)
			{
				Disconnected(e);
			}
		}

		public static void InvokeAnimateRequest(AnimateRequestEventArgs e)
		{
			if (AnimateRequest != null)
			{
				AnimateRequest(e);
			}
		}

		public static void InvokeCastSpellRequest(CastSpellRequestEventArgs e)
		{
			if (CastSpellRequest != null)
			{
				CastSpellRequest(e);
			}
		}

		public static void InvokeBandageTargetRequest(BandageTargetRequestEventArgs e)
		{
			if (BandageTargetRequest != null)
			{
				BandageTargetRequest(e);
			}
		}

		public static void InvokeOpenSpellbookRequest(OpenSpellbookRequestEventArgs e)
		{
			if (OpenSpellbookRequest != null)
			{
				OpenSpellbookRequest(e);
			}
		}

		public static void InvokeDisarmRequest(DisarmRequestEventArgs e)
		{
			if (DisarmRequest != null)
			{
				DisarmRequest(e);
			}
		}

		public static void InvokeStunRequest(StunRequestEventArgs e)
		{
			if (StunRequest != null)
			{
				StunRequest(e);
			}
		}

		public static void InvokeHelpRequest(HelpRequestEventArgs e)
		{
			if (HelpRequest != null)
			{
				HelpRequest(e);
			}
		}

		public static void InvokeShutdown(ShutdownEventArgs e)
		{
			if (Shutdown != null)
			{
				Shutdown(e);
			}
		}

		public static void InvokeCrashed(CrashedEventArgs e)
		{
			if (Crashed != null)
			{
				Crashed(e);
			}
		}

		public static void InvokeHungerChanged(HungerChangedEventArgs e)
		{
			if (HungerChanged != null)
			{
				HungerChanged(e);
			}
		}

		public static void InvokeMovement(MovementEventArgs e)
		{
			if (Movement != null)
			{
				Movement(e);
			}
		}

		public static void InvokeServerList(ServerListEventArgs e)
		{
			if (ServerList != null)
			{
				ServerList(e);
			}
		}

		public static void InvokeLogin(LoginEventArgs e)
		{
			if (Login != null)
			{
				Login(e);
			}
		}

		public static void InvokeSpeech(SpeechEventArgs e)
		{
			if (Speech != null)
			{
				Speech(e);
			}
		}

		public static void InvokeCharacterCreated(CharacterCreatedEventArgs e)
		{
			if (CharacterCreated != null)
			{
				CharacterCreated(e);
			}
		}

		public static void InvokeOpenDoorMacroUsed(OpenDoorMacroEventArgs e)
		{
			if (OpenDoorMacroUsed != null)
			{
				OpenDoorMacroUsed(e);
			}
		}

		public static void InvokeWorldLoad()
		{
			if (WorldLoad != null)
			{
				WorldLoad();
			}
		}

		public static void InvokeWorldSave(WorldSaveEventArgs e)
		{
			if (WorldSave != null)
			{
				WorldSave(e);
			}
		}

        public static void InvokeBeforeWorldSave(BeforeWorldSaveEventArgs e)
        {
            if (BeforeWorldSave != null)
            {
                BeforeWorldSave(e);
            }
        }

        public static void InvokeAfterWorldSave(AfterWorldSaveEventArgs e)
        {
            if (AfterWorldSave != null)
            {
                AfterWorldSave(e);
            }
        }

        public static void InvokeOnKilledBy(OnKilledByEventArgs e)
		{
			if (OnKilledBy != null)
			{
				OnKilledBy(e);
			}
		}

		public static void InvokeOnItemUse(OnItemUseEventArgs e)
		{
			if (OnItemUse != null)
			{
				OnItemUse(e);
			}
		}

		public static void InvokeOnEnterRegion(OnEnterRegionEventArgs e)
		{
			if (OnEnterRegion != null)
			{
				OnEnterRegion(e);
			}
		}

		public static void InvokeOnConsume(OnConsumeEventArgs e)
		{
			if (OnConsume != null)
			{
				OnConsume(e);
			}
		}

		public static void InvokeOnPropertyChanged(OnPropertyChangedEventArgs e)
		{
			if (OnPropertyChanged != null)
			{
				OnPropertyChanged(e);
			}
		}

		public static void InvokeBODUsed(BODUsedEventArgs e)
		{
			if(BODUsed != null)
			{
				BODUsed(e);
			}
		}

		public static void InvokeBODOffered(BODOfferEventArgs e)
		{
			if(BODOffered != null)
			{
				BODOffered(e);
			}
		}

		public static void InvokeResourceHarvestAttempt(ResourceHarvestAttemptEventArgs e)
		{
			if(ResourceHarvestAttempt != null)
			{
				ResourceHarvestAttempt(e);
			}
		}

		public static void InvokeResourceHarvestSuccess(ResourceHarvestSuccessEventArgs e)
		{
			if (ResourceHarvestSuccess != null)
			{
				ResourceHarvestSuccess(e);
			}
		}

		public static void InvokeCraftSuccess(CraftSuccessEventArgs e)
		{
			if (CraftSuccess != null)
			{
				CraftSuccess(e);
			}
		}

		public static void InvokeItemCreated(ItemCreatedEventArgs e)
		{
			if (ItemCreated != null)
			{
				ItemCreated(e);
			}
		}

		public static void InvokeItemDeleted(ItemDeletedEventArgs e)
		{
			if (ItemDeleted != null)
			{
				ItemDeleted(e);
			}
		}

		public static void InvokeMobileCreated(MobileCreatedEventArgs e)
		{
			if (MobileCreated != null)
			{
				MobileCreated(e);
			}
		}

		public static void InvokeMobileDeleted(MobileDeletedEventArgs e)
		{
			if (MobileDeleted != null)
			{
				MobileDeleted(e);
			}
		}

		public static void Reset()
		{
			CharacterCreated = null;
			OpenDoorMacroUsed = null;
			Speech = null;
			Login = null;
			ServerList = null;
			Movement = null;
			HungerChanged = null;
			Crashed = null;
			Shutdown = null;
			HelpRequest = null;
			DisarmRequest = null;
			StunRequest = null;
			OpenSpellbookRequest = null;
			CastSpellRequest = null;
			BandageTargetRequest = null;
			AnimateRequest = null;
			Logout = null;
			SocketConnect = null;
			Connected = null;
			Disconnected = null;
			RenameRequest = null;
			PlayerDeath = null;
			VirtueGumpRequest = null;
			VirtueItemRequest = null;
			VirtueMacroRequest = null;
			ChatRequest = null;
			AccountLogin = null;
			PaperdollRequest = null;
			ProfileRequest = null;
			ChangeProfileRequest = null;
			AggressiveAction = null;
			Command = null;
			GameLogin = null;
			DeleteRequest = null;
			WorldLoad = null;
			WorldSave = null;
			SetAbility = null;
			GuildGumpRequest = null;
			QuestGumpRequest = null;
			OnKilledBy = null;
			OnItemUse = null;
			OnEnterRegion = null;
			OnConsume = null;
			OnPropertyChanged = null;
			BODUsed = null;
			BODOffered = null;
			ResourceHarvestAttempt = null;
			ResourceHarvestSuccess = null;
			CraftSuccess = null;
			
			ItemCreated = null;
			ItemDeleted = null;
			MobileCreated = null;
			MobileDeleted = null;
		}
	}
}