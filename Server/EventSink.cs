#region References
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

using Server.Accounting;
using Server.Commands;
using Server.ContextMenus;
using Server.Guilds;
using Server.Items;
using Server.Network;
#endregion

namespace Server
{
	public delegate void OnItemObtainedEventHandler(OnItemObtainedEventArgs e);

	public delegate void CheckEquipItemEventHandler(CheckEquipItemEventArgs e);

	public delegate void ContextMenuEventHandler(ContextMenuEventArgs e);

	public delegate void WorldBroadcastEventHandler(WorldBroadcastEventArgs e);

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

	public delegate void CreatureDeathEventHandler(CreatureDeathEventArgs e);

	public delegate void VirtueGumpRequestEventHandler(VirtueGumpRequestEventArgs e);

	public delegate void VirtueItemRequestEventHandler(VirtueItemRequestEventArgs e);

	public delegate void VirtueMacroRequestEventHandler(VirtueMacroRequestEventArgs e);

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

    public delegate void ClientTypeReceivedHandler(ClientTypeReceivedArgs e);

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

    public delegate void SkillGainEventHandler(SkillGainEventArgs e);

    public delegate void SkillCheckEventHandler(SkillCheckEventArgs e);

    public delegate void SkillCapChangeEventHandler(SkillCapChangeEventArgs e);

    public delegate void StatCapChangeEventHandler(StatCapChangeEventArgs e);

    public delegate void QuestCompleteEventHandler(QuestCompleteEventArgs e);

    public delegate void ItemCreatedEventHandler(ItemCreatedEventArgs e);

	public delegate void ItemDeletedEventHandler(ItemDeletedEventArgs e);

	public delegate void MobileCreatedEventHandler(MobileCreatedEventArgs e);

	public delegate void MobileDeletedEventHandler(MobileDeletedEventArgs e);

    public delegate void TargetedSpellEventHandler(TargetedSpellEventArgs e);

    public delegate void TargetedSkillEventHandler(TargetedSkillEventArgs e);

    public delegate void TargetedItemUseEventHandler(TargetedItemUseEventArgs e);

    public delegate void EquipMacroEventHandler(EquipMacroEventArgs e);

    public delegate void UnequipMacroEventHandler(UnequipMacroEventArgs e);

    public delegate void TargetByResourceMacroEventHandler(TargetByResourceMacroEventArgs e);

    public delegate void JoinGuildEventHandler(JoinGuildEventArgs e);

    public delegate void TameCreatureEventHandler(TameCreatureEventArgs e);

    public delegate void ValidVendorPurchaseEventHandler(ValidVendorPurchaseEventArgs e);

    public delegate void ValidVendorSellEventHandler(ValidVendorSellEventArgs e);

    public delegate void CorpseLootEventHandler(CorpseLootEventArgs e);

    public delegate void RepairItemEventHandler(RepairItemEventArgs e);

    public delegate void AlterItemEventHandler(AlterItemEventArgs e);

    public delegate void PlacePlayerVendorEventHandler(PlacePlayerVendorEventArgs e);

    public delegate void FameChangeEventHandler(FameChangeEventArgs e);

    public delegate void KarmaChangeEventHandler(KarmaChangeEventArgs e);

    public delegate void VirtueLevelChangeEventHandler(VirtueLevelChangeEventArgs e);

    public delegate void PlayerMurderedEventHandler(PlayerMurderedEventArgs e);

    public delegate void AccountGoldChangeEventHandler(AccountGoldChangeEventArgs e);

    public delegate void ContainerDroppedToEventHandler(ContainerDroppedToEventArgs e);

    public delegate void TeleportMovementEventHandler(TeleportMovementEventArgs e);

    public delegate void MultiDesignQueryHandler(MultiDesignQueryEventArgs e);

    public class OnItemObtainedEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly Item m_Item;

		public OnItemObtainedEventArgs(Mobile from, Item item)
		{
			m_Mobile = from;
			m_Item = item;
		}

		public Mobile Mobile { get { return m_Mobile; } }
		public Item Item { get { return m_Item; } }
	}

	public class CheckEquipItemEventArgs : EventArgs
	{
		public Mobile Mobile { get; private set; }
		public Item Item { get; private set; }
		public bool Message { get; private set; }

		public bool Block { get; set; }

		public CheckEquipItemEventArgs(Mobile m, Item item, bool message)
		{
			Mobile = m;
			Item = item;
			Message = message;
		}
	}

	public class ContextMenuEventArgs : EventArgs
	{
		public Mobile Mobile { get; private set; }
		public IEntity Target { get; private set; }
		public List<ContextMenuEntry> Entries { get; private set; }

		public ContextMenuEventArgs(Mobile m, IEntity target, List<ContextMenuEntry> entries)
		{
			Mobile = m;
			Target = target;
			Entries = entries;
		}
	}

	public class WorldBroadcastEventArgs : EventArgs
	{
		public int Hue { get; set; }
		public bool Ascii { get; set; }
		public AccessLevel Access { get; set; }
		public string Text { get; set; }

		public WorldBroadcastEventArgs(int hue, bool ascii, AccessLevel access, string text)
		{
			Hue = hue;
			Ascii = ascii;
			Access = access;
			Text = text;
		}
	}

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

    public class ClientTypeReceivedArgs : EventArgs
    {
        private readonly NetState m_State;

        public NetState State { get { return m_State; } }

        public ClientTypeReceivedArgs(NetState state)
        {
            m_State = state;
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

	public class PlayerDeathEventArgs : EventArgs
	{
		public Mobile Mobile { get; private set; }
		public Mobile Killer { get; private set; }
		public Container Corpse { get; private set; }

		public PlayerDeathEventArgs(Mobile mobile)
			: this(mobile, mobile.LastKiller, mobile.Corpse)
		{ }

		public PlayerDeathEventArgs(Mobile mobile, Mobile killer, Container corpse)
		{
			Mobile = mobile;
			Killer = killer;
			Corpse = corpse;
		}
	}

	public class CreatureDeathEventArgs : EventArgs
	{
		public Mobile Creature { get; private set; }
		public Mobile Killer { get; private set; }
		public Container Corpse { get; private set; }

		public List<Item> ForcedLoot { get; private set; }

		public bool PreventDefault { get; set; }
		public bool PreventDelete { get; set; }
		public bool ClearCorpse { get; set; }

		public CreatureDeathEventArgs(Mobile creature)
			: this(creature, creature.LastKiller, creature.Corpse)
		{ }

		public CreatureDeathEventArgs(Mobile creature, Mobile killer, Container corpse)
		{
			Creature = creature;
			Killer = killer;
			Corpse = corpse;

			ForcedLoot = new List<Item>();
		}

		public void ClearLoot(bool free)
		{
			if (free)
			{
				ForcedLoot.Clear();
				ForcedLoot.TrimExcess();
			}
			else
			{
				ForcedLoot = new List<Item>();
			}
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
        private readonly int m_Face;
        private readonly int m_FaceHue;

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
        public int FaceID { get { return m_Face; } }
        public int FaceHue { get { return m_FaceHue; } }

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
            : this(state, a, name, female, hue, str, dex, intel, city, skills, shirtHue, pantsHue, hairID, hairHue, beardID, beardHue, profession, race, 0, 0)
        {
        }

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
			Race race,
            int faceID,
            int faceHue)
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
            m_Face = faceID;
            m_FaceHue = faceHue;
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
        private readonly Region m_OldRegion;
		private readonly Region m_NewRegion;

		public OnEnterRegionEventArgs(Mobile from, Region oldRegion, Region newRegion)
		{
			m_From = from;
            m_OldRegion = oldRegion;
			m_NewRegion = newRegion;
		}

		public Mobile From { get { return m_From; } }
		public Region OldRegion { get { return m_OldRegion; } }
        public Region NewRegion { get { return m_NewRegion; } }
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
        public Item BonusResource { get; private set; }
		public object HarvestSystem { get; private set; }

		public ResourceHarvestSuccessEventArgs(Mobile m, Item i, Item r, Item b, object o)
		{
			Harvester = m;
			Tool = i;
			Resource = r;
            BonusResource = b;
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

    public class SkillGainEventArgs : EventArgs
    {
        public int Gained { get; private set; }
        public Mobile From { get; private set; }
        public Skill Skill { get; private set; }

        public SkillGainEventArgs(Mobile from, Skill skill, int toGain)
        {
            From = from;
            Skill = skill;
            Gained = toGain;
        }
    }

    public class SkillCheckEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public Mobile From { get; set; }
        public Skill Skill { get; set; }

        public SkillCheckEventArgs(Mobile from, Skill skill, bool success)
        {
            From = from;
            Skill = skill;
            Success = success;
        }
    }

    public class SkillCapChangeEventArgs : EventArgs
    {
        public Mobile Mobile { get; private set; }
        public Skill Skill { get; private set; }
        public double OldCap { get; private set; }
        public double NewCap { get; private set; }

        public SkillCapChangeEventArgs(Mobile from, Skill skill, double oldCap, double newCap)
        {
            Mobile = from;
            Skill = skill;
            OldCap = oldCap;
            NewCap = newCap;
        }
    }

    public class StatCapChangeEventArgs : EventArgs
    {
        public Mobile Mobile { get; private set; }
        public int OldCap { get; private set; }
        public int NewCap { get; private set; }

        public StatCapChangeEventArgs(Mobile from, int oldCap, int newCap)
        {
            Mobile = from;
            OldCap = oldCap;
            NewCap = newCap;
        }
    }

    public class QuestCompleteEventArgs : EventArgs
    {
        public Type QuestType { get; private set; }
        public Mobile Mobile { get; private set; }

        public QuestCompleteEventArgs(Mobile from, Type type)
        {
            Mobile = from;
            QuestType = type;
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

    public class TargetedSpellEventArgs : EventArgs
    {
        private Mobile m_Mobile;
        private IEntity m_Target;
        private short m_SpellID;

        public Mobile Mobile { get { return m_Mobile; } }
        public IEntity Target { get { return m_Target; } }
        public short SpellID { get { return m_SpellID; } }

        public TargetedSpellEventArgs(Mobile m, IEntity target, short spellID)
        {
            m_Mobile = m;
            m_Target = target;
            m_SpellID = spellID;
        }
    }

    public class TargetedSkillEventArgs : EventArgs
    {
        private Mobile m_Mobile;
        private IEntity m_Target;
        private short m_SkillID;

        public Mobile Mobile { get { return m_Mobile; } }
        public IEntity Target { get { return m_Target; } }
        public short SkillID { get { return m_SkillID; } }

        public TargetedSkillEventArgs(Mobile m, IEntity target, short skillID)
        {
            m_Mobile = m;
            m_Target = target;
            m_SkillID = skillID;
        }
    }

    public class TargetedItemUseEventArgs : EventArgs
    {
        private Mobile m_Mobile;
        private IEntity m_Source;
        private IEntity m_Target;

        public Mobile Mobile { get { return m_Mobile; } }
        public IEntity Source { get { return m_Source; } }
        public IEntity Target { get { return m_Target; } }

        public TargetedItemUseEventArgs(Mobile mobile, IEntity src, IEntity target)
        {
            m_Mobile = mobile;
            m_Source = src;
            m_Target = target;
        }
    }

    public class TargetByResourceMacroEventArgs : EventArgs
    {
        private Mobile m_Mobile;
        private Item m_Tool;
        private int m_ResourceType;

        public Mobile Mobile { get { return m_Mobile; } }
        public Item Tool { get { return m_Tool; } }
        public int ResourceType { get { return m_ResourceType; } }

        public TargetByResourceMacroEventArgs(Mobile mobile, Item tool, int type)
        {
            m_Mobile = mobile;
            m_Tool = tool;
            m_ResourceType = type;
        }
    }

    public class EquipMacroEventArgs : EventArgs
    {
        private Mobile m_Mobile;
        private List<int> m_List;

        public Mobile Mobile { get { return m_Mobile; } }
        public List<int> List { get { return m_List; } }

        public EquipMacroEventArgs(Mobile mobile, List<int> list)
        {
            m_Mobile = mobile;
            m_List = list;
        }
    }

    public class UnequipMacroEventArgs : EventArgs
    {
        private Mobile m_Mobile;
        private List<int> m_List;

        public Mobile Mobile { get { return m_Mobile; } }
        public List<int> List { get { return m_List; } }

        public UnequipMacroEventArgs(Mobile mobile, List<int> list)
        {
            m_Mobile = mobile;
            m_List = list;
        }
    }

    public class JoinGuildEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public BaseGuild Guild { get; set; }

        public JoinGuildEventArgs(Mobile m, BaseGuild g)
        {
            Mobile = m;
            Guild = g;
        }
    }

    public class TameCreatureEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public Mobile Creature { get; set; }

        public TameCreatureEventArgs(Mobile m, Mobile creature)
        {
            Mobile = m;
            Creature = creature;
        }
    }

    public class ValidVendorPurchaseEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public Mobile Vendor { get; set; }
        public IEntity Bought { get; set; }
        public int AmountPerUnit { get; set; }

        public ValidVendorPurchaseEventArgs(Mobile m, Mobile vendor, IEntity bought, int costPer)
        {
            Mobile = m;
            Vendor = vendor;
            Bought = bought;
            AmountPerUnit = costPer;
        }
    }

    public class ValidVendorSellEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public Mobile Vendor { get; set; }
        public IEntity Sold { get; set; }
        public int AmountPerUnit { get; set; }

        public ValidVendorSellEventArgs(Mobile m, Mobile vendor, IEntity sold, int costPer)
        {
            Mobile = m;
            Vendor = vendor;
            Sold = sold;
            AmountPerUnit = costPer;
        }
    }

    public class CorpseLootEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public Container Corpse { get; set; }
        public Item Looted { get; set; }

        public CorpseLootEventArgs(Mobile m, Container c, Item looted)
        {
            Mobile = m;
            Corpse = c;
            Looted = looted;
        }
    }

    public class RepairItemEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public Item Tool { get; set; }
        public IEntity Repaired { get; set; }

        public RepairItemEventArgs(Mobile m, Item tool, IEntity repaired)
        {
            Mobile = m;
            Tool = tool;
            Repaired = repaired;
        }
    }

    public class AlterItemEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public Item Tool { get; set; }
        public Item OldItem { get; set; }
        public Item NewItem { get; set; }

        public AlterItemEventArgs(Mobile m, Item tool, Item oldItem, Item newItem)
        {
            Mobile = m;
            Tool = tool;
            OldItem = oldItem;
            NewItem = newItem;
        }
    }

    public class PlacePlayerVendorEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public Mobile Vendor { get; set; }

        public PlacePlayerVendorEventArgs(Mobile m, Mobile vendor)
        {
            Mobile = m;
            Vendor = vendor;
        }
    }

    public class FameChangeEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public int OldValue { get; set; }
        public int NewValue { get; set; }

        public FameChangeEventArgs(Mobile m, int oldValue, int newValue)
        {
            Mobile = m;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class KarmaChangeEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public int OldValue { get; set; }
        public int NewValue { get; set; }

        public KarmaChangeEventArgs(Mobile m, int oldValue, int newValue)
        {
            Mobile = m;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class VirtueLevelChangeEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public int OldLevel { get; set; }
        public int NewLevel { get; set; }
        public int Virtue { get; set; }

        public VirtueLevelChangeEventArgs(Mobile m, int oldLevel, int newLevel, int virtue)
        {
            Mobile = m;
            OldLevel = oldLevel;
            NewLevel = newLevel;
            Virtue = virtue;
        }
    }

    public class PlayerMurderedEventArgs : EventArgs
    {
        public Mobile Murderer { get; set; }
        public Mobile Victim { get; set; }

        public PlayerMurderedEventArgs(Mobile murderer, Mobile victim)
        {
            Murderer = murderer;
            Victim = victim;
        }
    }

    public class AccountGoldChangeEventArgs : EventArgs
    {
        public IAccount Account { get; set; }
        public double OldAmount { get; set; }
        public double NewAmount { get; set; }

        public AccountGoldChangeEventArgs(IAccount account, double oldAmount, double newAmount)
        {
            Account = account;
            OldAmount = oldAmount;
            NewAmount = newAmount;
        }
    }

    public class ContainerDroppedToEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public Container Container { get; set; }
        public Item Dropped { get; set; }

        public ContainerDroppedToEventArgs(Mobile m, Container container, Item dropped)
        {
            Mobile = m;
            Container = container;
            Dropped = dropped;
        }
    }

    public class TeleportMovementEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }
        public Point3D OldLocation { get; set; }
        public Point3D NewLocation { get; set; }

        public TeleportMovementEventArgs(Mobile m, Point3D oldLoc, Point3D newLoc)
        {
            Mobile = m;
            OldLocation = oldLoc;
            NewLocation = newLoc;
        }
    }

    public class MultiDesignQueryEventArgs : EventArgs
    {
        public NetState State { get; set; }
        public BaseMulti Multi { get; set; }

        public MultiDesignQueryEventArgs(NetState state, BaseMulti multi)
        {
            State = state;
            Multi = multi;
        }
    }

    public static class EventSink
	{
		public static event OnItemObtainedEventHandler OnItemObtained;
		public static event CheckEquipItemEventHandler CheckEquipItem;
		public static event ContextMenuEventHandler ContextMenu;
		public static event WorldBroadcastEventHandler WorldBroadcast;
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
		public static event CreatureDeathEventHandler CreatureDeath;
		public static event VirtueGumpRequestEventHandler VirtueGumpRequest;
		public static event VirtueItemRequestEventHandler VirtueItemRequest;
		public static event VirtueMacroRequestEventHandler VirtueMacroRequest;
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
        public static event ClientTypeReceivedHandler ClientTypeReceived;
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
        public static event SkillGainEventHandler SkillGain;
        public static event SkillCheckEventHandler SkillCheck;
        public static event SkillCapChangeEventHandler SkillCapChange;
        public static event StatCapChangeEventHandler StatCapChange;
        public static event QuestCompleteEventHandler QuestComplete;

        public static event ItemCreatedEventHandler ItemCreated;
		public static event ItemDeletedEventHandler ItemDeleted;
		public static event MobileCreatedEventHandler MobileCreated;
		public static event MobileDeletedEventHandler MobileDeleted;

        public static event TargetedSpellEventHandler TargetedSpell;
        public static event TargetedSkillEventHandler TargetedSkill;
        public static event TargetedItemUseEventHandler TargetedItemUse;
        public static event EquipMacroEventHandler EquipMacro;
        public static event UnequipMacroEventHandler UnequipMacro;
        public static event TargetByResourceMacroEventHandler TargetByResourceMacro;

        public static event JoinGuildEventHandler JoinGuild;
        public static event TameCreatureEventHandler TameCreature;
        public static event ValidVendorPurchaseEventHandler ValidVendorPurchase;
        public static event ValidVendorSellEventHandler ValidVendorSell;
        public static event CorpseLootEventHandler CorpseLoot;
        public static event RepairItemEventHandler RepairItem;
        public static event AlterItemEventHandler AlterItem;
        public static event PlacePlayerVendorEventHandler PlacePlayerVendor;
        public static event FameChangeEventHandler FameChange;
        public static event KarmaChangeEventHandler KarmaChange;
        public static event VirtueLevelChangeEventHandler VirtueLevelChange;
        public static event PlayerMurderedEventHandler PlayerMurdered;
        public static event AccountGoldChangeEventHandler AccountGoldChange;
        public static event ContainerDroppedToEventHandler ContainerDroppedTo;
        public static event TeleportMovementEventHandler TeleportMovement;
        public static event MultiDesignQueryHandler MultiDesign;

        public static void InvokeOnItemObtained(OnItemObtainedEventArgs e)
		{
			if (OnItemObtained != null)
			{
				OnItemObtained(e);
			}
		}

		public static void InvokeCheckEquipItem(CheckEquipItemEventArgs e)
		{
			if (CheckEquipItem != null)
			{
				CheckEquipItem(e);
			}
		}

		public static void InvokeContextMenu(ContextMenuEventArgs e)
		{
			if (ContextMenu != null)
			{
				ContextMenu(e);
			}
		}

		public static void InvokeWorldBroadcast(WorldBroadcastEventArgs e)
		{
			if (WorldBroadcast != null)
			{
				WorldBroadcast(e);
			}
		}

        public static void InvokeClientVersionReceived(ClientVersionReceivedArgs e)
		{
			if (ClientVersionReceived != null)
			{
				ClientVersionReceived(e);
			}
		}

        public static void InvokeClientTypeReceived(ClientTypeReceivedArgs e)
        {
            if (ClientTypeReceived != null)
            {
                ClientTypeReceived(e);
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

		public static void InvokeCreatureDeath(CreatureDeathEventArgs e)
		{
			if (CreatureDeath != null)
			{
				CreatureDeath(e);
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

        public static void InvokeSkillGain(SkillGainEventArgs e)
        {
            if(SkillGain != null)
            {
                SkillGain(e);
            }
        }

        public static void InvokeSkillCheck(SkillCheckEventArgs e)
        {
            if (SkillCheck != null)
            {
                SkillCheck(e);
            }
        }

        public static void InvokeSkillCapChange(SkillCapChangeEventArgs e)
        {
            if (SkillCapChange != null)
            {
                SkillCapChange(e);
            }
        }

        public static void InvokeStatCapChange(StatCapChangeEventArgs e)
        {
            if (StatCapChange != null)
            {
                StatCapChange(e);
            }
        }

        public static void InvokeQuestComplete(QuestCompleteEventArgs e)
        {
            if (QuestComplete != null)
            {
                QuestComplete(e);
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

        public static void InvokeTargetedSpell(TargetedSpellEventArgs e)
        {
            if (TargetedSpell != null)
            {
                TargetedSpell(e);
            }
        }

        public static void InvokeTargetedSkill(TargetedSkillEventArgs e)
        {
            if (TargetedSkill != null)
            {
                TargetedSkill(e);
            }
        }

        public static void InvokeTargetedItemUse(TargetedItemUseEventArgs e)
        {
            if (TargetedItemUse != null)
            {
                TargetedItemUse(e);
            }
        }

        public static void InvokeTargetByResourceMacro(TargetByResourceMacroEventArgs e)
        {
            if (TargetByResourceMacro != null)
            {
                TargetByResourceMacro(e);
            }
        }

        public static void InvokeEquipMacro(EquipMacroEventArgs e)
        {
            if (EquipMacro != null)
            {
                EquipMacro(e);
            }
        }

        public static void InvokeUnequipMacro(UnequipMacroEventArgs e)
        {
            if (UnequipMacro != null)
            {
                UnequipMacro(e);
            }
        }

        public static void InvokeJoinGuild(JoinGuildEventArgs e)
        {
            if (JoinGuild != null)
            {
                JoinGuild(e);
            }
        }

        public static void InvokeTameCreature(TameCreatureEventArgs e)
        {
            if (TameCreature != null)
            {
                TameCreature(e);
            }
        }

        public static void InvokeValidVendorPurchase(ValidVendorPurchaseEventArgs e)
        {
            if (ValidVendorPurchase != null)
            {
                ValidVendorPurchase(e);
            }
        }

        public static void InvokeValidVendorSell(ValidVendorSellEventArgs e)
        {
            if (ValidVendorSell != null)
            {
                ValidVendorSell(e);
            }
        }

        public static void InvokeCorpseLoot(CorpseLootEventArgs e)
        {
            if (CorpseLoot != null)
            {
                CorpseLoot(e);
            }
        }

        public static void InvokeRepairItem(RepairItemEventArgs e)
        {
            if (RepairItem != null)
            {
                RepairItem(e);
            }
        }

        public static void InvokeAlterItem(AlterItemEventArgs e)
        {
            if (AlterItem != null)
            {
                AlterItem(e);
            }
        }

        public static void InvokePlacePlayerVendor(PlacePlayerVendorEventArgs e)
        {
            if (PlacePlayerVendor != null)
            {
                PlacePlayerVendor(e);
            }
        }

        public static void InvokeFameChange(FameChangeEventArgs e)
        {
            if (FameChange != null)
            {
                FameChange(e);
            }
        }

        public static void InvokeKarmaChange(KarmaChangeEventArgs e)
        {
            if (KarmaChange != null)
            {
                KarmaChange(e);
            }
        }

        public static void InvokeVirtueLevelChange(VirtueLevelChangeEventArgs e)
        {
            if (VirtueLevelChange != null)
            {
                VirtueLevelChange(e);
            }
        }

        public static void InvokePlayerMurdered(PlayerMurderedEventArgs e)
        {
            if (PlayerMurdered != null)
            {
                PlayerMurdered(e);
            }
        }

        public static void InvokeAccountGoldChange(AccountGoldChangeEventArgs e)
        {
            if (AccountGoldChange != null)
            {
                AccountGoldChange(e);
            }
        }

        public static void InvokeContainerDroppedTo(ContainerDroppedToEventArgs e)
        {
            if (ContainerDroppedTo != null)
            {
                ContainerDroppedTo(e);
            }
        }

        public static void InvokeTeleportMovement(TeleportMovementEventArgs e)
        {
            if (TeleportMovement != null)
            {
                TeleportMovement(e);
            }
        }

        public static void InvokeMultiDesignQuery(MultiDesignQueryEventArgs e)
        {
            if (MultiDesign != null)
            {
                MultiDesign(e);
            }
        }

        public static void Reset()
		{
			OnItemObtained = null;
			CheckEquipItem = null;
			ContextMenu = null;
			WorldBroadcast = null;
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
			CreatureDeath = null;
			VirtueGumpRequest = null;
			VirtueItemRequest = null;
			VirtueMacroRequest = null;
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
			SkillGain = null;
            SkillCheck = null;

			ItemCreated = null;
			ItemDeleted = null;
			MobileCreated = null;
			MobileDeleted = null;

            TargetedSpell = null;
            TargetedSkill = null;
            TargetedItemUse = null;

            JoinGuild = null;
            TameCreature = null;
            ValidVendorPurchase = null;
            ValidVendorSell = null;
            CorpseLoot = null;
            RepairItem = null;
            AlterItem = null;
            PlacePlayerVendor = null;
            FameChange = null;
            KarmaChange = null;
            VirtueLevelChange = null;
            PlayerMurdered = null;
            AccountGoldChange = null;
            ContainerDroppedTo = null;
            TeleportMovement = null;

            MultiDesign = null;
        }
	}
}
