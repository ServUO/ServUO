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
        public static int VirtueHue = 2124;
        public static int ViceHue = 2118;

        public static bool Enabled = Config.Get("VvV.Enabled", true);
        public static int StartSilver = Config.Get("VvV.StartSilver", 2000);

        public static ViceVsVirtueSystem Instance { get; set; }

        public override TextDefinition Name { get { return new TextDefinition("Vice Vs Virtue"); } }
        public override PointsType Loyalty { get { return PointsType.ViceVsVirtue; } }
        public override bool AutoAdd { get { return false; } }
        public override double MaxPoints { get { return 10000; } }

        public bool HasGenerated { get; set; }

        public Dictionary<Guild, VvVGuildStats> GuildStats { get; set; }
        public static Dictionary<Mobile, DateTime> TempParticipants { get; set; }

        public List<VvVCity> ExemptCities { get; set; }

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
            ExemptCities = new List<VvVCity>();
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
                List<Mobile> handled = new List<Mobile>();
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

                        if (kentry != null && !handled.Contains(dam))
                        {
                            if (i == 0)
                                Battle.Update(ventry, kentry, UpdateType.Kill);
                            else
                                Battle.Update(ventry, kentry, UpdateType.Assist);

                            handled.Add(dam);
                        }
                    }

                    if (!statloss && isEnemy)
                        statloss = true;
                }

                if (statloss)
                    Faction.ApplySkillLoss(victim);

                ColUtility.Free(list);
                ColUtility.Free(handled);
            }
        }

        public void AddPlayer(PlayerMobile pm)
        {
            if (pm == null || pm.Guild == null)
                return;

            Guild g = pm.Guild as Guild;
            VvVPlayerEntry entry = GetEntry(pm, true) as VvVPlayerEntry;

            if (!entry.Active)
                entry.Active = true;

            pm.SendLocalizedMessage(1155564); // You have joined Vice vs Virtue!
            pm.SendLocalizedMessage(1063156, g.Name); // The guild information for ~1_val~ has been updated.

            pm.ProcessDelta();

            CheckBattleStatus(pm);
        }

        public bool IsResigning(PlayerMobile pm, Guild g)
        {
            VvVPlayerEntry entry = GetPlayerEntry<VvVPlayerEntry>(pm);

            return entry != null && entry.Guild == g && entry.Resigning;
        }

        public void OnResign(Mobile m, bool quitguild = false)
        {
            VvVPlayerEntry entry = GetPlayerEntry<VvVPlayerEntry>(m as PlayerMobile);

            if (entry != null && entry.Active && !entry.Resigning)
            {
                if (m.AccessLevel == AccessLevel.Player)
                    entry.ResignExpiration = DateTime.UtcNow + TimeSpan.FromDays(3);
                else
                    entry.ResignExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(1);

                if (quitguild)
                    m.SendLocalizedMessage(1155580); // You have quit a guild while participating in Vice vs Virtue.  You will be freely attackable by members of Vice vs Virtue until your resignation period has ended!
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
                pm.ValidateEquipment();
            }
        }

        public void CheckBattleStatus()
        {
            if (Battle.OnGoing)
                return;

            int count = EnemyGuildCount();

            if (count > 0)
            {
                Battle.Begin();
            }
        }

        public void CheckBattleStatus(PlayerMobile pm)
        {
            if (!IsVvV(pm) || !Enabled)
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
                int count = EnemyGuildCount();

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

        public int EnemyGuildCount()
        {
            List<Guild> guilds = new List<Guild>();

            foreach (NetState ns in NetState.Instances)
            {
                Mobile m = ns.Mobile;

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

            return count - 1;
        }

        public void SendVvVMessage(string message)
        {
            foreach (NetState state in NetState.Instances.Where(st => st.Mobile != null && IsVvV(st.Mobile)))
            {
                Mobile m = state.Mobile;

                if (m != null)
                {
                    m.SendMessage("[Guild][VvV] {0}", message);
                }
            }
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

        private List<Item> VvVItems = new List<Item>();

        public void AddVvVItem(Item item, bool initial = false)
        {
            if (ViceVsVirtueSystem.Enabled && item is IVvVItem)
            {
                ((IVvVItem)item).IsVvVItem = true;

                if (!VvVItems.Contains(item))
                {
                    VvVItems.Add(item);
                }

                if (initial)
                {
                    FactionEquipment.CheckProperties(item);
                }
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

            Server.Commands.CommandSystem.Register("ExemptCities", AccessLevel.Administrator, e =>
            {
                e.Mobile.SendGump(new ExemptCitiesGump());
            });

            Server.Commands.CommandSystem.Register("VvVKick", AccessLevel.GameMaster, e =>
            {
                e.Mobile.SendMessage("Target the person you'd like to remove from VvV.");
                e.Mobile.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, (from, targeted) =>
                    {
                        if (targeted is PlayerMobile)
                        {
                            var pm = targeted as PlayerMobile;
                            VvVPlayerEntry entry = Instance.GetPlayerEntry<VvVPlayerEntry>(pm);

                            if (entry != null && entry.Active)
                            {
                                pm.PrivateOverheadMessage(MessageType.Regular, 1154, 1155561, pm.NetState); // You are no longer in Vice vs Virtue!

                                entry.Active = false;
                                entry.ResignExpiration = DateTime.MinValue;
                                pm.Delta(MobileDelta.Noto);
                                pm.ValidateEquipment();

                                from.SendMessage("{0} has been removed from VvV.", pm.Name);
                                pm.SendMessage("You have been removed from VvV.");
                            }
                            else
                            {
                                from.SendMessage("{0} is not an active VvV member.", pm.Name);
                            }
                        }
                    });
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
                Timer.DelayCall<PlayerMobile>(TimeSpan.FromSeconds(2), Instance.CheckBattleStatus, pm);
            }
        }

        public static void OnPlayerDeath(PlayerDeathEventArgs e)
        {
            if (!Enabled)
                return;

            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null && Instance != null)
            {
                Instance.HandlePlayerDeath(pm);
            }
        }

        public static bool IsVvV(Mobile m, bool checkpet = true, bool guildedonly = false)
        {
            if (!Enabled)
                return false;

            if (m is BaseCreature && checkpet)
            {
                if (((BaseCreature)m).GetMaster() is PlayerMobile)
                    m = ((BaseCreature)m).GetMaster();
            }

            VvVPlayerEntry entry = Instance.GetPlayerEntry<VvVPlayerEntry>(m as PlayerMobile);

            if (entry == null)
                return false;

            return entry.Active && (!guildedonly || entry.Guild != null);
        }

        public static bool IsVvV(Mobile m, out VvVPlayerEntry entry, bool checkpet = true, bool guildedonly = false)
        {
            if (!Enabled)
            {
                entry = null;
                return false;
            }

            if (m is BaseCreature && checkpet)
            {
                if (((BaseCreature)m).GetMaster() is PlayerMobile)
                    m = ((BaseCreature)m).GetMaster();
            }

            entry = Instance.GetPlayerEntry<VvVPlayerEntry>(m as PlayerMobile);

            if (entry != null && !entry.Active)
                entry = null;

            return entry != null && entry.Active && (!guildedonly || entry.Guild != null);
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
            if (!Enabled || from == to)
                return false;

            if (from is BaseCreature && ((BaseCreature)from).GetMaster() is PlayerMobile)
                from = ((BaseCreature)from).GetMaster();

            if (to is BaseCreature && ((BaseCreature)to).GetMaster() is PlayerMobile)
                to = ((BaseCreature)to).GetMaster();

            if (from == to || !IsVvVCombatant(to) || !IsVvVCombatant(from))
                return false;

            VvVPlayerEntry fromentry = Instance.GetPlayerEntry<VvVPlayerEntry>(from);
            VvVPlayerEntry toentry = Instance.GetPlayerEntry<VvVPlayerEntry>(to);

            Guild fromGuild = from.Guild as Guild;
            Guild toGuild = to.Guild as Guild;

            if (fromGuild != null && toGuild != null && (fromGuild == toGuild || fromGuild.IsAlly(toGuild)))
            {
                return false;
            }

            return true;
        }

        public static void AddTempParticipant(Mobile m)
        {
            if (TempParticipants == null)
                TempParticipants = new Dictionary<Mobile, DateTime>();

            TempParticipants[m] = DateTime.UtcNow + TimeSpan.FromMinutes(30);
            m.Delta(MobileDelta.Noto);
        }

        public static bool IsVvVCombatant(Mobile mobile)
        {
            return IsVvV(mobile) || (TempParticipants != null && TempParticipants.ContainsKey(mobile));
        }

        public static void CheckHarmful(Mobile attacker, Mobile defender)
        {
            if (attacker == null || defender == null)
                return;

            if (!IsVvV(attacker) && IsVvV(defender) && !defender.Aggressed.Any(info => info.Defender == attacker))
            {
                Guild attackerguild = attacker.Guild as Guild;
                Guild defenderguild = defender.Guild as Guild;

                if ((attackerguild == null && defenderguild == null) || attackerguild != defenderguild)
                {
                    AddTempParticipant(attacker);
                }
            }
        }

        public static void CheckBeneficial(Mobile from, Mobile target)
        {
            if (from == null || target == null)
                return;

            if (!IsVvV(from) && IsVvV(target))
            {
                AddTempParticipant(from);
            }
        }

        public static void RemoveTempParticipant(Mobile m)
        {
            if (TempParticipants == null)
                return;

            if (TempParticipants.ContainsKey(m))
            {
                TempParticipants.Remove(m);
                m.Delta(MobileDelta.Noto);
            }
        }

        public static void CheckTempParticipants()
        {
            if (TempParticipants == null)
                return;

            List<Mobile> remove = new List<Mobile>();

            foreach (var kvp in TempParticipants)
            {
                if (kvp.Value < DateTime.UtcNow)
                {
                    remove.Add(kvp.Key);
                }
            }

            foreach (var m in remove)
            {
                RemoveTempParticipant(m);
            }

            ColUtility.Free(remove);
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
            writer.Write(3);

            writer.Write(Enabled);

            writer.Write(ExemptCities.Count);
            ExemptCities.ForEach(c => writer.Write((int)c));

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

            GuildStats = new Dictionary<Guild, VvVGuildStats>();
            ExemptCities = new List<VvVCity>();
            bool enabled = false;

            switch (version)
            {
                case 3:
                    enabled = reader.ReadBool();
                    goto case 2;
                case 2:
                case 1:
                    {
                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            ExemptCities.Add((VvVCity)reader.ReadInt());
                        }

                        goto case 0;
                    }
                case 0:
                    {
                        HasGenerated = reader.ReadBool();

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
                    break;
            }

            if (version == 1)
                Timer.DelayCall(FixVvVItems);

            if (Enabled && !enabled)
            {
                CreateSilverTraders();
                Server.Factions.Generator.RemoveFactions();
            }
            else if (!Enabled && enabled)
            {
                DeleteSilverTraders();
            }
        }

        public void FixVvVItems()
        {
            foreach (var item in VvVItems.Where(i => i is Spellbook))
            {
                var book = item as Spellbook;
                var attrs = RunicReforging.GetNegativeAttributes(item);

                if (attrs != null)
                {
                    attrs.Antique = 0;
                }

                book.MaxHitPoints = 0;
                book.HitPoints = 0;
            }
        }

        public static void CreateSilverTraders()
        {
            if (!Enabled)
                return;

            Map map = Map.Felucca;

            foreach (CityInfo info in CityInfo.Infos.Values)
            {
                IPooledEnumerable eable = map.GetMobilesInRange(info.TraderLoc, 3);
                bool found = false;

                foreach (Mobile m in eable)
                {
                    if (m is SilverTrader)
                    {
                        found = true;
                        break;
                    }
                }

                eable.Free();

                if (!found)
                {
                    SilverTrader trader = new SilverTrader();
                    trader.MoveToWorld(info.TraderLoc, map);
                }
            }
        }

        public static void DeleteSilverTraders()
        {
            var list = new List<Mobile>(World.Mobiles.Values.Where(m => m is SilverTrader));

            foreach (var mob in list)
            {
                mob.Delete();
            }

            ColUtility.Free(list);
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

        public Guild Guild
        {
            get
            {
                return Player != null ? Player.Guild as Guild : null;
            }
        }

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
                    Points = 0;
                }

                _Active = value;
            }
        }

        public DateTime ResignExpiration { get; set; }
        public bool Resigning { get { return ResignExpiration > DateTime.MinValue; } }

        public VvVPlayerEntry(PlayerMobile pm)
            : base(pm)
        {
            _Active = true;
            Points = ViceVsVirtueSystem.StartSilver;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);

            writer.Write(Active);

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

            if(version == 0)
                reader.ReadBool();

            if(version < 2)
                reader.ReadGuild();

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