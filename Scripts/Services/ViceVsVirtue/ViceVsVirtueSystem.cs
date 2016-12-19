using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;
using Server.Network;
using Server.Guilds;
using System.Linq;
using Server.Engines.Points;
using Server.Factions;
using Server.Engines.CityLoyalty;
using Server.Regions;

namespace Server.Engines.VvV
{
    public enum VvVType
    {
        Virtue, 
        Vice
    }

    public enum VvVCity
    {
        Britain,
        Jhelom,
        Minoc,
        Moonglow,
        Ocllo, 
        SkaraBrae,
        Trinsic,
        Yew
    }

    public class ViceVsVirtueSystem : PointsSystem
    {
        public static int VirtueHue = 2124; //TODO: Get
        public static int ViceHue = 2118; //TODO: Get

        public static bool Enabled = true;
        public static ViceVsVirtueSystem Instance { get; set; }

        public override TextDefinition Name { get { return new TextDefinition("Vice Vs Virtue"); } }
        public override PointsType Loyalty { get { return PointsType.ViceVsVirtue; } }
        public override bool AutoAdd { get { return false; } }
        public override double MaxPoints { get { return 10000; } }

        public bool HasGenerated { get; set; }

        public Dictionary<Guild, VvVGuildStats> GuildStats { get; set; }
        public static Dictionary<Mobile, Mobile> FlaggedTo { get; set; }

        public VvVBattle Battle { get; private set; }

        public override string ToString()
        {
            return "...";
        }

        public ViceVsVirtueSystem()
        {
            Instance = this;
            Battle = new VvVBattle(this);

            GuildStats = new Dictionary<Guild, VvVGuildStats>();
        }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
        }

        public override PointsEntry GetSystemEntry(PlayerMobile pm)
        {
            return new VvVPlayerEntry(pm);
        }

        public void HandlePlayerDeath(PlayerMobile victim)
        {
            VvVPlayerEntry ventry = GetPlayerEntry<VvVPlayerEntry>(victim);

            if (ventry != null && ventry.Active)
            {
                List<DamageEntry> list = victim.DamageEntries.OrderBy(d => -d.DamageGiven).ToList();
                bool statloss = false;

                for(int i = 0; i < list.Count; i++)
                {
                    Mobile dam = list[i].Damager;

                    if (dam is BaseCreature && ((BaseCreature)dam).GetMaster() is PlayerMobile)
                        dam = ((BaseCreature)dam).GetMaster();

                    bool isEnemy = IsEnemy(victim, dam);

                    if (isEnemy && dam != null && Battle.IsInActiveBattle(dam, victim))
                    {
                        VvVPlayerEntry kentry = GetPlayerEntry<VvVPlayerEntry>(dam);

                        if (kentry != null)
                        {
                            if (i == 0)
                                Battle.Update(ventry, kentry, UpdateType.Kill);
                            else
                                Battle.Update(ventry, kentry, UpdateType.Assist);
                        }
                    }

                    if (!statloss && isEnemy)
                        statloss = true;
                }

                if (statloss)
                    Faction.ApplySkillLoss(victim);

                list.Clear();
                list.TrimExcess();
            }
        }

        public void TryAddGuild(Guild g)
        {
            if (g == null)
                return;

            foreach (PlayerMobile pm in g.Members.OfType<PlayerMobile>().Where(player => !player.Young))
            {
                VvVPlayerEntry entry = GetEntry(pm, true) as VvVPlayerEntry;

                entry.Guild = g;
                entry.Active = true;

                if (pm.NetState != null)
                {
                    pm.SendLocalizedMessage(1155564); // You have joined Vice vs Virtue!
                    pm.SendLocalizedMessage(1063156, g.Name); // The guild information for ~1_val~ has been updated.

                    CheckBattleStatus(pm);
                }
                else
                    entry.PendingJoinMessage = true;
            }
        }

        public bool IsResigning(PlayerMobile pm, Guild g)
        {
            VvVPlayerEntry entry = GetPlayerEntry<VvVPlayerEntry>(pm);

            return entry != null && entry.Guild == g && entry.Resigning;
        }

        public void ResignGuild(Guild g)
        {
            if (g == null)
                return;

            foreach (PlayerMobile pm in g.Members.OfType<PlayerMobile>())
            {
                OnRemovedFromGuild(pm);
            }
        }

        public void OnRemovedFromGuild(Mobile m)
        {
            VvVPlayerEntry entry = GetPlayerEntry<VvVPlayerEntry>(m as PlayerMobile);

            if (entry != null)
            {
                if (m.AccessLevel == AccessLevel.Player)
                    entry.ResignExpiration = DateTime.UtcNow + TimeSpan.FromDays(3);
                else
                    entry.ResignExpiration = DateTime.UtcNow;
            }
        }

        public void CheckResignation(PlayerMobile pm)
        {
            VvVPlayerEntry entry = GetPlayerEntry<VvVPlayerEntry>(pm);

            if (entry != null && entry.Resigning && entry.ResignExpiration < DateTime.UtcNow)
            {
                pm.PrivateOverheadMessage(MessageType.Regular, 1154, 1155561, pm.NetState); // You are no longer in Vice vs Virtue!

                entry.Active = false;
                entry.ResignExpiration = DateTime.MinValue;
                pm.Delta(MobileDelta.Noto);
            }
        }

        public void CheckPendingJoin(PlayerMobile pm)
        {
            VvVPlayerEntry entry = GetPlayerEntry<VvVPlayerEntry>(pm);

            if (entry != null && entry.PendingJoinMessage)
            {
                pm.SendLocalizedMessage(1155564); // You have joined Vice vs Virtue!
                pm.SendLocalizedMessage(1063156, entry.Guild != null ? entry.Guild.Name : "your guild"); // The guild information for ~1_val~ has been updated.

                entry.PendingJoinMessage = false;
            }
        }

        public void CheckBattleStatus(PlayerMobile pm)
        {
            if (!IsVvV(pm))
                return;

            if (Battle.OnGoing)
            {
                SendVvVMessageTo(pm, 1154721, String.Format("#{0}", GetCityLocalization(Battle.City).ToString()));
                // A Battle between Vice and Virtue is active! To Arms! The City of ~1_CITY~ is besieged!

                if (Battle != null && Battle.IsInActiveBattle(pm))
                {
                    Battle.CheckGump(pm);
                    Battle.CheckArrow(pm);
                }
            }
            else
            {
                int count = EnemyGuildCount(pm);

                if (count < 1)
                {
                    SendVvVMessageTo(pm, 1154936); // More players are needed before a VvV battle can begin! 
                }
                else if (Battle.InCooldown)
                {
                    SendVvVMessageTo(pm, 1154722); // A VvV battle has just concluded. The next battle will begin in less than five minutes!
                }
                else
                {
                    Battle.Begin();
                }
            }
        }

        public int EnemyGuildCount(Mobile exclude = null)
        {
            List<Guild> guilds = new List<Guild>();

            foreach (NetState ns in NetState.Instances)
            {
                Mobile m = ns.Mobile;

                if (exclude != null && exclude == m)
                    continue;

                if (m != null)
                {
                    Guild g = m.Guild as Guild;
                    VvVPlayerEntry entry = GetPlayerEntry<VvVPlayerEntry>(m);

                    if (g != null && entry != null && entry.Guild == g && !guilds.Contains(g))
                    {
                        bool hasally = false;

                        foreach (Guild guild in guilds)
                        {
                            if (guild.IsAlly(g))
                            {
                                hasally = true;
                                break;
                            }
                        }

                        if(!hasally)
                            guilds.Add(g);
                    }
                }
            }

            int count = guilds.Count;
            guilds.Clear();
            guilds.TrimExcess();

            return count;
        }

        public void SendVvVMessage(int cliloc, string args = "")
        {
            foreach(NetState state in NetState.Instances.Where(st => st.Mobile != null && IsVvV(st.Mobile)))
            {
                Mobile m = state.Mobile;

                if(m != null)
                {
                    SendVvVMessageTo(m, cliloc, args);
                }
            }
        }

        public void SendVvVMessageTo(Mobile m, int cliloc, string args = "")
        {
            m.SendLocalizedMessage(cliloc, false, "[Guild][VvV] ", args, m is PlayerMobile ? ((PlayerMobile)m).GuildMessageHue : 0x34);
        }

        public void OnBattleEnd()
        {
            foreach (KeyValuePair<Guild, VvVGuildBattleStats> kvp in Battle.GuildStats)
            {
                Guild g = kvp.Key;

                if (!GuildStats.ContainsKey(g))
                    GuildStats[g] = new VvVGuildStats(g);

                int score = (int)kvp.Value.Points;

                GuildStats[g].Kills += kvp.Value.Kills;
                GuildStats[g].ReturnedSigils += kvp.Value.ReturnedSigils;
                GuildStats[g].Score += score;

                List<Mobile> list = g.Members.Where(mob => mob.NetState != null && mob.Region.IsPartOf(Battle.Region)).ToList();

                foreach (Mobile m in list)
                {
                    VvVPlayerEntry entry = GetPlayerEntry<VvVPlayerEntry>(m, true);

                    if (entry != null)
                    {
                        entry.Score += score;
                    }
                }
            }
        }

        private List<Item> VvVItems = new List<Item>();

        public void AddVvVItem(Item item)
        {
            if (item is IVvVItem)
            {
                ((IVvVItem)item).IsVvVItem = true;
                VvVItems.Add(item);
            }
        }

        public static void Initialize()
        {
            if (!Enabled)
                return;

            EventSink.Login += OnLogin;
            EventSink.PlayerDeath += OnPlayerDeath;

            Server.Commands.CommandSystem.Register("BattleProps", AccessLevel.GameMaster, e =>
                {
                    if(Instance.Battle != null)
                        e.Mobile.SendGump(new PropertiesGump(e.Mobile, Instance.Battle));
                });

            Server.Commands.CommandSystem.Register("ForceStartBattle", AccessLevel.GameMaster, e =>
            {
                if (Instance.Battle != null && !Instance.Battle.OnGoing)
                    Instance.Battle.Begin();
            });

            if (!Instance.HasGenerated)
            {
                CreateSilverTraders();
            }
        }

        public static void OnLogin(LoginEventArgs e)
        {
            if (!Enabled)
                return;

            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null && Instance != null)
            {
                Timer.DelayCall<PlayerMobile>(TimeSpan.FromSeconds(1), Instance.CheckResignation, pm);
                Timer.DelayCall<PlayerMobile>(TimeSpan.FromSeconds(1), Instance.CheckPendingJoin, pm);
                Timer.DelayCall<PlayerMobile>(TimeSpan.FromSeconds(2), Instance.CheckBattleStatus, pm);
            }
        }

        public static void OnPlayerDeath(PlayerDeathEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null && Instance != null)
            {
                Instance.HandlePlayerDeath(pm);
            }
        }

        public static bool IsVvV(Mobile m, bool checkpet = true)
        {
            if (m is BaseCreature && checkpet)
            {
                if (((BaseCreature)m).GetMaster() is PlayerMobile)
                    m = ((BaseCreature)m).GetMaster();
            }

            VvVPlayerEntry entry = Instance.GetPlayerEntry<VvVPlayerEntry>(m as PlayerMobile);

            return entry != null && entry.Active;
        }

        public static bool IsVvV(Mobile m, out VvVPlayerEntry entry, bool checkpet = true)
        {
            if (m is BaseCreature && checkpet)
            {
                if (((BaseCreature)m).GetMaster() is PlayerMobile)
                    m = ((BaseCreature)m).GetMaster();
            }

            entry = Instance.GetPlayerEntry<VvVPlayerEntry>(m as PlayerMobile);

            if (entry != null && !entry.Active)
                entry = null;

            return entry != null;
        }

        public static bool IsEnemy(IDamageable from, IDamageable to)
        {
            if (from is Mobile && to is Mobile)
                return IsEnemy((Mobile)to, (Mobile)from);

            //TODO: VvV items, such as traps, turrets, etc
            return false;
        }

        public static bool IsEnemy(Mobile from, Mobile to)
        {
            //TODO: Support for VvV city games regarding non-participants in the city, as well as ones who flagged
            if (from is BaseCreature && ((BaseCreature)from).GetMaster() is PlayerMobile)
                from = ((BaseCreature)from).GetMaster();

            if (to is BaseCreature && ((BaseCreature)to).GetMaster() is PlayerMobile)
                to = ((BaseCreature)to).GetMaster();

            VvVPlayerEntry fromentry = Instance.GetPlayerEntry<VvVPlayerEntry>(from);
            VvVPlayerEntry toentry = Instance.GetPlayerEntry<VvVPlayerEntry>(to);

            if (fromentry == null || toentry == null || !fromentry.Active || !toentry.Active)
            {
                if (fromentry != null && toentry == null && FlaggedTo != null && FlaggedTo.ContainsKey(from) && FlaggedTo[from] == to)
                    return true;

                return false;
            }

            Guild fromguild = fromentry.Guild;
            Guild toguild = toentry.Guild;

            return fromguild != null && toguild != null && fromguild != toguild && !fromguild.IsAlly(toguild);
        }

        public static bool IsBattleRegion(Region r)
        {
            if (r == null || Instance == null)
            {
                return false;
            }

            return Instance.Battle.OnGoing && r.IsPartOf(Instance.Battle.Region);
        }

        public static int GetCityLocalization(VvVCity city)
        {
            switch (city)
            {
                default: return 0;
                case VvVCity.Moonglow: return 1011344;
                case VvVCity.Britain: return 1011028;
                case VvVCity.Jhelom: return 1011343;
                case VvVCity.Yew: return 1011032;
                case VvVCity.Minoc: return 1011031;
                case VvVCity.Trinsic: return 1011029;
                case VvVCity.SkaraBrae: return 1011347;
                case VvVCity.Ocllo: return 1076027;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(HasGenerated);
            Battle.Serialize(writer);

            writer.Write(VvVItems.Count);
            foreach (Item item in VvVItems)
            {
                writer.Write(item);
            }

            writer.Write(GuildStats.Count);
            foreach (KeyValuePair<Guild, VvVGuildStats> kvp in GuildStats)
            {
                writer.Write(kvp.Key);
                kvp.Value.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            HasGenerated = reader.ReadBool();

            GuildStats = new Dictionary<Guild, VvVGuildStats>();
            Battle = new VvVBattle(reader, this);

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Item item = reader.ReadItem();

                if (item != null)
                    AddVvVItem(item);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Guild g = reader.ReadGuild() as Guild;
                VvVGuildStats stats = new VvVGuildStats(g, reader);

                if (g != null)
                    GuildStats[g] = stats;
            }
        }

        public static void CreateSilverTraders()
        {
            Map map = Map.Felucca;

            foreach (CityInfo info in CityInfo.Infos.Values)
            {
                IPooledEnumerable eable = map.GetMobilesInRange(info.TraderLoc, 3);
                bool found = false;

                foreach (Mobile m in eable)
                {
                    if (m is SilverTrader)
                    {
                        eable.Free();
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    SilverTrader trader = new SilverTrader();
                    trader.MoveToWorld(info.TraderLoc, map);
                }
            }
        }
    }

    public class VvVPlayerEntry : PointsEntry
    {
        private bool _Active;

        public int Score { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int ReturnedSigils { get; set; }
        public int DisarmedTraps { get; set; }
        public int StolenSigils { get; set; }

        public bool PendingJoinMessage { get; set; }
        public Guild Guild { get; set; }

        public bool Active
        {
            get 
            { 
                return _Active; 
            }
            set
            {
                if (!_Active && value)
                {
                    Points = 2000.0;
                }

                _Active = value;
            }
        }

        public DateTime ResignExpiration { get; set; }
        public bool Resigning { get { return ResignExpiration > DateTime.MinValue; } }

        public VvVPlayerEntry(PlayerMobile pm)
            : base(pm)
        {
            Active = true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Active);
            writer.Write(PendingJoinMessage);
            writer.Write(Guild);

            writer.Write(Score);
            writer.Write(Kills);
            writer.Write(Deaths);
            writer.Write(Assists);
            writer.Write(ReturnedSigils);
            writer.Write(DisarmedTraps);
            writer.Write(StolenSigils);
            writer.Write(ResignExpiration);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Active = reader.ReadBool();
            PendingJoinMessage = reader.ReadBool();
            Guild = reader.ReadGuild() as Guild;

            Score = reader.ReadInt();
            Kills = reader.ReadInt();
            Deaths = reader.ReadInt();
            Assists = reader.ReadInt();
            ReturnedSigils = reader.ReadInt();
            DisarmedTraps = reader.ReadInt();
            StolenSigils = reader.ReadInt();
            ResignExpiration = reader.ReadDateTime();
        }
    }
}