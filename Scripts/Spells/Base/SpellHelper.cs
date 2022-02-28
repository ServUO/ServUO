using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;
using Server.Services.Virtues;
using Server.Spells.Fifth;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Seventh;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells
{
    public enum TravelCheckType
    {
        RecallFrom,
        RecallTo,
        GateFrom,
        GateTo,
        Mark,
        TeleportFrom,
        TeleportTo
    }

    public class SpellHelper
    {
        #region Spell Focus and SDI Calculations
        private static readonly SkillName[] _Schools =
        {
            SkillName.Magery,
            SkillName.Mysticism,
            SkillName.Necromancy,
            SkillName.AnimalTaming,
            SkillName.Musicianship,
            SkillName.Spellweaving,
            SkillName.Chivalry,
            SkillName.Bushido,
            SkillName.Ninjitsu,
            SkillName.Parry,
            SkillName.EvalInt,
            SkillName.SpiritSpeak,
            SkillName.Focus,
            SkillName.Imbuing
        };

        private static bool IsExempt(SkillName main, SkillName secondary)
        {
            switch (main)
            {
                default: return false;
                case SkillName.Magery: return secondary == SkillName.EvalInt;
                case SkillName.Necromancy: return secondary == SkillName.SpiritSpeak;
                case SkillName.Mysticism: return secondary == SkillName.Focus || secondary == SkillName.Imbuing;
            }
        }

        public static bool HasSpellFocus(Mobile m, SkillName focus)
        {
            bool foundExempt = false;

            foreach (SkillName skill in _Schools)
            {
                if (skill != focus && m.Skills[skill].Value >= 30.0)
                {
                    if (!foundExempt && IsExempt(focus, skill))
                    {
                        foundExempt = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static int PvPSpellDamageCap(Mobile m, SkillName castskill)
        {
            if (HasSpellFocus(m, castskill))
            {
                return 25;
            }
            else
            {
                return 20;
            }
        }

        public static int GetSpellDamageBonus(Mobile caster, IDamageable damageable, SkillName skill, bool playerVsPlayer)
        {
            Mobile target = damageable as Mobile;

            int sdiBonus = AosAttributes.GetValue(caster, AosAttribute.SpellDamage);

            if (target != null)
            {
                if (RunedSashOfWarding.IsUnderEffects(target, WardingEffect.SpellDamage))
                    sdiBonus -= 10;

                sdiBonus -= Block.GetSpellReduction(target);
            }

            // PvP spell damage increase cap of 15% from an item’s magic property, 30% if spell school focused.
            if (playerVsPlayer)
            {
                sdiBonus = Math.Min(sdiBonus, PvPSpellDamageCap(caster, skill));
            }

            return sdiBonus;
        }
        #endregion 

        private static readonly TimeSpan AosDamageDelay = TimeSpan.FromSeconds(1.0);
        private static readonly TimeSpan OldDamageDelay = TimeSpan.FromSeconds(0.5);

        public static TimeSpan GetDamageDelayForSpell(Spell sp)
        {
            if (!sp.DelayedDamage)
                return TimeSpan.Zero;

            return (AosDamageDelay);
        }

        public static bool CheckMulti(Point3D p, Map map)
        {
            return CheckMulti(p, map, true, 0);
        }

        public static bool CheckMulti(Point3D p, Map map, bool houses)
        {
            return CheckMulti(p, map, houses, 0);
        }

        public static bool CheckMulti(Point3D p, Map map, bool houses, int housingrange)
        {
            if (map == null || map == Map.Internal)
                return false;

            Sector sector = map.GetSector(p.X, p.Y);

            for (int i = 0; i < sector.Multis.Count; ++i)
            {
                BaseMulti multi = sector.Multis[i];

                if (multi is BaseHouse)
                {
                    BaseHouse bh = (BaseHouse)multi;

                    if ((houses && bh.IsInside(p, 16)) || (housingrange > 0 && bh.InRange(p, housingrange)))
                        return true;
                }
                else if (multi.Contains(p))
                {
                    return true;
                }
            }

            return false;
        }

        public static void Turn(Mobile from, object to)
        {
            IPoint3D target = to as IPoint3D;
            int d = -1;

            if (target == null)
                return;

            if (target is Item)
            {
                Item item = (Item)target;

                if (item.RootParent != from)
                    d = (int)from.GetDirectionTo(item.GetWorldLocation());
            }
            else if (from != target)
            {
                d = (int)from.GetDirectionTo(target);
            }

            if (d > -1)
            {
                from.Direction = (Direction)d;
                from.ProcessDelta();
            }
        }

        public static bool CheckCombat(Mobile m, bool restrict = true)
        {
            if (!restrict)
                return false;

            return Aggression.CheckHasAggression(m, true);
        }

        public static bool AdjustField(ref Point3D p, Map map, int height, bool mobsBlock)
        {
            if (map == null)
                return false;

            for (int offset = 0; offset < 25; ++offset)
            {
                Point3D loc = new Point3D(p.X, p.Y, p.Z - offset);

                if (map.CanFit(loc, height, true, mobsBlock))
                {
                    p = loc;
                    return true;
                }

                loc = new Point3D(p.X, p.Y, p.Z + offset);

                if (map.CanFit(loc, height, true, mobsBlock))
                {
                    p = loc;
                    return true;
                }
            }

            return false;
        }

        public static bool CheckField(Point3D p, Map map)
        {
            if (map == null)
                return false;

            IPooledEnumerable eable = map.GetItemsInRange(p, 0);

            foreach (Item item in eable)
            {
                Type t = item.GetType();

                if (t.IsDefined(typeof(DispellableFieldAttribute), false) || t.IsDefined(typeof(DispellableFieldAttribute), true))
                {
                    eable.Free();
                    return false;
                }
            }

            eable.Free();
            return true;
        }

        public static bool CheckWater(Point3D p, Map map)
        {
            LandTile landTile = map.Tiles.GetLandTile(p.X, p.Y);

            if (landTile.Z == p.Z && ((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311)))
            {
                return false;
            }

            StaticTile[] tiles = map.Tiles.GetStaticTiles(p.X, p.Y, true);

            foreach (StaticTile tile in tiles)
            {
                if (tile.Z == p.Z && tile.ID >= 0x1796 && tile.ID <= 0x17B2)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CanRevealCaster(Mobile m)
        {
            if (m is BaseCreature)
            {
                BaseCreature c = (BaseCreature)m;

                if (!c.Controlled)
                    return true;
            }

            return false;
        }

        public static void GetSurfaceTop(ref IPoint3D p)
        {
            if (p is Item)
            {
                p = ((Item)p).GetSurfaceTop();
            }
            else if (p is StaticTarget)
            {
                StaticTarget t = (StaticTarget)p;
                int z = t.Z;

                if ((t.Flags & TileFlag.Surface) == 0)
                    z -= TileData.ItemTable[t.ItemID & TileData.MaxItemValue].CalcHeight;

                p = new Point3D(t.X, t.Y, z);
            }
        }

        protected static void RemoveStatOffsetCallback(object state)
        {
            if (!(state is Mobile))
                return;
            // This call has the side-effect of updating all stats
            ((Mobile)state).CheckStatTimers();
        }

        public static bool AddStatOffset(Mobile m, StatType type, int offset, TimeSpan duration)
        {
            if (offset > 0)
                return AddStatBonus(m, m, type, offset, duration);
            else if (offset < 0)
                return AddStatCurse(m, m, type, -offset, duration);

            return true;
        }

        public static bool AddStatBonus(Mobile caster, Mobile target, bool blockSkill, StatType type)
        {
            return AddStatBonus(caster, target, type, GetOffset(caster, target, type, false, blockSkill), GetDuration(caster, target));
        }

        public static bool AddStatBonus(Mobile caster, Mobile target, StatType type, int bonus, TimeSpan duration)
        {
            int offset = bonus;
            string name = string.Format("[Magic] {0} Buff", type);

            StatMod mod = target.GetStatMod(name);

            if (mod != null)
            {
                offset = Math.Max(mod.Offset, offset);
            }

            target.AddStatMod(new StatMod(type, name, offset, duration));
            Timer.DelayCall(duration, RemoveStatOffsetCallback, target);

            return true;
        }

        public static int GetBuffOffset(Mobile m, StatType type)
        {
            string name = string.Format("[Magic] {0} Buff", type);

            StatMod mod = m.GetStatMod(name);

            return mod != null ? mod.Offset : 0;
        }

        public static bool AddStatCurse(Mobile caster, Mobile target, StatType type)
        {
            return AddStatCurse(caster, target, type, true);
        }

        public static bool AddStatCurse(Mobile caster, Mobile target, StatType type, bool blockSkill)
        {
            return AddStatCurse(caster, target, type, GetOffset(caster, target, type, true, blockSkill), TimeSpan.Zero);
        }

        public static bool AddStatCurse(Mobile caster, Mobile target, StatType type, bool blockSkill, int offset)
        {
            return AddStatCurse(caster, target, type, offset, TimeSpan.Zero);
        }

        public static bool AddStatCurse(Mobile caster, Mobile target, StatType type, int curse, TimeSpan duration)
        {
            int offset = curse;
            string name = string.Format("[Magic] {0} Curse", type);

            StatMod mod = target.GetStatMod(name);

            if (mod != null)
                offset = Math.Max(mod.Offset, offset);

            offset *= -1;

            target.AddStatMod(new StatMod(type, name, offset, duration));
            return true;
        }

        public static TimeSpan GetDuration(Mobile caster, Mobile target)
        {
            int span = (((6 * caster.Skills.EvalInt.Fixed) / 50) + 1);

            if (caster.Spell is CurseSpell && SkillMasteries.ResilienceSpell.UnderEffects(target))
                span /= 2;

            return TimeSpan.FromSeconds(span);
        }

        public static int GetCurseOffset(Mobile m, StatType type)
        {
            string name = string.Format("[Magic] {0} Curse", type);

            StatMod mod = m.GetStatMod(name);

            return mod != null ? mod.Offset : 0;
        }

        public static double GetOffsetScalar(Mobile caster, Mobile target, bool curse)
        {
            double percent;

            if (curse)
            {
                double resistFixed = target.Skills.MagicResist.Fixed - (EvilOmenSpell.GetResistMalus(target) * 10);
                percent = 8 + (caster.Skills.EvalInt.Fixed / 100) - (resistFixed / 100);
            }
            else
                percent = 1 + (caster.Skills.EvalInt.Fixed / 100);

            percent *= 0.01;

            if (percent < 0)
                percent = 0;

            return percent;
        }

        public static int GetOffset(Mobile caster, Mobile target, StatType type, bool curse, bool blockSkill)
        {
            if (!blockSkill)
            {
                if (curse)
                    target.CheckSkill(SkillName.MagicResist, 0.0, 120.0);
            }

            double percent = GetOffsetScalar(caster, target, curse);

            switch (type)
            {
                case StatType.Str:
                    return (int)Math.Ceiling(target.RawStr * percent);
                case StatType.Dex:
                    return (int)Math.Ceiling(target.RawDex * percent);
                case StatType.Int:
                    return (int)Math.Ceiling(target.RawInt * percent);
            }

            return 1 + (int)(caster.Skills[SkillName.Magery].Value * 0.1);
        }

        public static Guild GetGuildFor(Mobile m)
        {
            Guild g = m.Guild as Guild;

            if (g == null && m is BaseCreature)
            {
                BaseCreature c = (BaseCreature)m;
                m = c.ControlMaster;

                if (m != null)
                    g = m.Guild as Guild;

                if (g == null)
                {
                    m = c.SummonMaster;

                    if (m != null)
                        g = m.Guild as Guild;
                }
            }

            return g;
        }

        public static bool ValidIndirectTarget(Mobile from, Mobile to)
        {
            if (from == to)
            {
                return true;
            }

            if (to.Hidden && to.AccessLevel > from.AccessLevel || Engines.ArenaSystem.PVPArenaSystem.IsFriendly(from, to))
            {
                return false;
            }

            CheckResponsible(ref from);
            CheckResponsible(ref to);

            var noto = Notoriety.Compute(from, to);

            if (noto == Notoriety.Enemy)
            {
                return true;
            }

            if (IsGuildAllyOrParty(from, to))
            {
                return false;
            }

            var toCreature = to as BaseCreature;

            if (toCreature != null && (toCreature.Controlled || toCreature.Summoned))
            {
                var master = toCreature.GetMaster();

                if (master == from || IsParty(master, from))
                {
                    return false;
                }
            }

            var fromCreature = from as BaseCreature;

            if (fromCreature != null)
            {
                if (fromCreature.Controlled || fromCreature.Summoned)
                {
                    var master = fromCreature.GetMaster();

                    if (master == to)
                    {
                        return false;
                    }

                    if (IsParty(master, to))
                    {
                        return false;
                    }
                }
                else
                {
                    if (to.Player ||
                        (toCreature != null && (toCreature.Controlled || toCreature.Summoned) && toCreature.GetMaster() is PlayerMobile))
                    {
                        return true;
                    }
                }
            }

            // Non-enemy monsters will no longer flag area spells on each other
            if (fromCreature != null && toCreature != null)
            {
                if (fromCreature.GetMaster() is BaseCreature)
                {
                    fromCreature = fromCreature.GetMaster() as BaseCreature;
                }

                if (toCreature.GetMaster() is BaseCreature)
                {
                    toCreature = toCreature.GetMaster() as BaseCreature;
                }

                if (toCreature.IsEnemy(fromCreature))   //Natural Enemies
                {
                    return true;
                }

                //All involved are monsters- no damage. If falls through this statement, normal noto rules apply
                if (!toCreature.Controlled && !toCreature.Summoned && !fromCreature.Controlled && !fromCreature.Summoned) //All involved are monsters- no damage
                {
                    return false;
                }
            }

            if (toCreature != null && !toCreature.Controlled && toCreature.InitialInnocent)
            {
                return true;
            }

            return (noto != Notoriety.Innocent || from.Murderer);
        }

        public static bool CheckResponsible(ref Mobile m)
        {
            var bc = m as BaseCreature;

            if (bc != null && bc.GetMaster() != null)
            {
                m = bc.GetMaster();
                return true;
            }

            return false;
        }

        public static bool IsGuildAllyOrParty(Mobile from, Mobile to)
        {
            return IsGuild(from, to) || IsParty(from, to);
        }

        public static bool IsGuild(Mobile from, Mobile to)
        {
            Guild fromGuild = GetGuildFor(from);
            Guild toGuild = GetGuildFor(to);

            return fromGuild != null && toGuild != null && (fromGuild == toGuild || fromGuild.IsAlly(toGuild));
        }

        public static bool IsParty(Mobile from, Mobile to)
        {
            Party p = Party.Get(from);

            return p != null && p.Contains(to);
        }

        public static IEnumerable<IDamageable> AcquireIndirectTargets(Mobile caster, IPoint3D p, Map map, int range)
        {
            return AcquireIndirectTargets(caster, p, map, range, true);
        }

        public static IEnumerable<IDamageable> AcquireIndirectTargets(Mobile caster, IPoint3D p, Map map, int range, bool losCheck)
        {
            if (map == null)
            {
                yield break;
            }

            IPooledEnumerable eable = map.GetObjectsInRange(new Point3D(p), range);

            foreach (IDamageable id in eable.OfType<IDamageable>())
            {
                if (id == caster)
                {
                    continue;
                }

                if (!id.Alive || (losCheck && !caster.InLOS(id)) || !caster.CanBeHarmful(id, false))
                {
                    continue;
                }

                if (id is Mobile && !ValidIndirectTarget(caster, (Mobile)id))
                {
                    continue;
                }

                yield return id;
            }

            eable.Free();
        }

        private static readonly int[] m_Offsets = new int[]
        {
            -1, -1,
            -1, 0,
            -1, 1,
            0, -1,
            0, 1,
            1, -1,
            1, 0,
            1, 1
        };

        public static void Summon(BaseCreature creature, Mobile caster, int sound, TimeSpan duration, bool scaleDuration, bool scaleStats, bool summoned = true, SkillName useSkill = SkillName.Magery)
        {
            Map map = caster.Map;

            if (map == null)
                return;

            double scale = 1.0 + ((caster.Skills[useSkill].Value - 100.0) / 200.0);

            if (scaleDuration)
                duration = TimeSpan.FromSeconds(duration.TotalSeconds * scale);

            if (scaleStats)
            {
                creature.RawStr = (int)(creature.RawStr * scale);
                creature.Hits = creature.HitsMax;

                creature.RawDex = (int)(creature.RawDex * scale);
                creature.Stam = creature.StamMax;

                creature.RawInt = (int)(creature.RawInt * scale);
                creature.Mana = creature.ManaMax;
            }

            Point3D p = new Point3D(caster);

            if (FindValidSpawnLocation(map, ref p, true))
            {
                BaseCreature.Summon(creature, summoned, caster, p, sound, duration);
                return;
            }

            creature.Delete();
            caster.SendLocalizedMessage(501942); // That location is blocked.
        }

        public static bool FindValidSpawnLocation(Map map, ref Point3D p, bool surroundingsOnly)
        {
            if (map == null)	//sanity
                return false;

            if (!surroundingsOnly)
            {
                if (map.CanSpawnMobile(p))	//p's fine.
                {
                    p = new Point3D(p);
                    return true;
                }

                int z = map.GetAverageZ(p.X, p.Y);

                if (map.CanSpawnMobile(p.X, p.Y, z))
                {
                    p = new Point3D(p.X, p.Y, z);
                    return true;
                }
            }

            int offset = Utility.Random(8) * 2;

            for (int i = 0; i < m_Offsets.Length; i += 2)
            {
                int x = p.X + m_Offsets[(offset + i) % m_Offsets.Length];
                int y = p.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

                if (map.CanSpawnMobile(x, y, p.Z))
                {
                    p = new Point3D(x, y, p.Z);
                    return true;
                }
                else
                {
                    int z = map.GetAverageZ(x, y);

                    if (map.CanSpawnMobile(x, y, z))
                    {
                        p = new Point3D(x, y, z);
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool RestrictRedTravel => Config.Get("General.RestrictRedsToFel", false);

        private delegate bool TravelValidator(Map map, Point3D loc);

        private static readonly TravelValidator[] m_Validators = new TravelValidator[]
        {
            IsFeluccaT2A,
            IsKhaldun,
            IsIlshenar,
            IsTrammelWind,
            IsFeluccaWind,
            IsFeluccaDungeon,
            IsTrammelSolenHive,
            IsFeluccaSolenHive,
            IsDoomGauntlet,
            IsDoomFerry,
            IsSafeZone,
            IsChampionSpawn,
            IsTokunoDungeon,
            IsLampRoom,
            IsGuardianRoom,
            IsHeartwood,
            IsMLDungeon,
            IsSADungeon,
            IsTombOfKings,
            IsMazeOfDeath,
            IsSAEntrance,
            IsEodon,
        };

        private static readonly bool[,] m_Rules = new bool[,]
        {
					/*T2A(Fel),	Khaldun,	Ilshenar,	Wind(Tram),	Wind(Fel),	Dungeons(Fel),	Solen(Tram),	Solen(Fel),	Gauntlet(Malas),	Gauntlet(Ferry),	SafeZone,	ChampionSpawn,	Dungeons(Tokuno[Malas]),	LampRoom(Doom),	GuardianRoom(Doom),	Heartwood,	MLDungeons, SA Dungeons		Tomb of Kings	Maze of Death	SA Entrance,    Eodon*/
/* Recall From */	{ false, false,      true,       true,       false,      false,          true,           false,      false,              false,              true,       false,          true,                       false,          false,              false,      false,      true,           true,           false,          false,          true} ,
/* Recall To */		{ false,    false,      false,      false,      false,      false,          false,          false,      false,              false,              false,      false,          false,                      false,          false,              false,      false,      false,          false,          false,          false,          false },
/* Gate From */		{ false,    false,      false,      false,      false,      false,          false,          false,      false,              false,              false,      false,          false,                      false,          false,              false,      false,      false,          false,          false,          false,          false },
/* Gate To */		{ false,    false,      false,      false,      false,      false,          false,          false,      false,              false,              false,      false,          false,                      false,          false,              false,      false,      false,          false,          false,          false,          false },
/* Mark In */		{ false,    false,      false,      false,      false,      false,          false,          false,      false,              false,              false,      false,          false,                      false,          false,              false,      false,      false,          false,          false,          false,          false },
/* Tele From */		{ true,     true,       true,       true,       true,       true,           true,           true,       true,               true,               true,       true,           true,                       true,           true,               false,      true,       true,           false,          false,          false,          true },
/* Tele To */		{ true,     true,       true,       true,       true,       true,           true,           true,       true,               false,              false,      true,           true,                       true,           true,               false,      false,      true,           false,          false,          false,          true },
        };

        public static void SendInvalidMessage(Mobile caster, TravelCheckType type)
        {
            if (type == TravelCheckType.RecallTo || type == TravelCheckType.GateTo)
                caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            else if (type == TravelCheckType.TeleportTo)
                caster.SendLocalizedMessage(501035); // You cannot teleport from here to the destination.
            else
                caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
        }

        public static bool CheckTravel(Mobile caster, TravelCheckType type)
        {
            return CheckTravel(caster, caster.Map, caster.Location, type);
        }

        public static bool CheckTravel(Map map, Point3D loc, TravelCheckType type)
        {
            return CheckTravel(null, map, loc, type);
        }

        private static Mobile m_TravelCaster;
        private static TravelCheckType m_TravelType;

        public static bool CheckTravel(Mobile caster, Map map, Point3D loc, TravelCheckType type)
        {
            if (IsInvalid(map, loc)) // null, internal, out of bounds
            {
                if (caster != null)
                    SendInvalidMessage(caster, type);

                return false;
            }

            if (caster != null)
            {
                if (caster.IsPlayer())
                {
                    // Jail region
                    if (caster.Region.IsPartOf<Jail>())
                    {
                        caster.SendLocalizedMessage(1114345); // You'll need a better jailbreak plan than that!
                        return false;
                    }
                    else if (caster.Region is GreenAcres)
                    {
                        caster.SendLocalizedMessage(502360); // You cannot teleport into that area.
                        return false;
                    }
                }

                // Always allow monsters to teleport
                if (caster is BaseCreature && (type == TravelCheckType.TeleportTo || type == TravelCheckType.TeleportFrom))
                {
                    BaseCreature bc = (BaseCreature)caster;

                    if (!bc.Controlled && !bc.Summoned)
                        return true;
                }

                if (Siege.SiegeShard && !Siege.CheckTravel(caster, loc, map, type))
                {
                    return false;
                }
            }

            m_TravelCaster = caster;
            m_TravelType = type;

            int v = (int)type;
            bool isValid = true;

            if (caster != null)
            {
                BaseRegion destination = Region.Find(loc, map) as BaseRegion;
                BaseRegion current = Region.Find(caster.Location, caster.Map) as BaseRegion;

                if (destination != null && !destination.CheckTravel(caster, loc, type))
                    isValid = false;

                if (isValid && current != null && !current.CheckTravel(caster, loc, type))
                    isValid = false;

                if (caster.Region != null)
                {
                    if (caster.Region.IsPartOf("Blighted Grove") && loc.Z < -10)
                        isValid = false;
                }

                if ((int)type <= 4 && (IsNewDungeon(caster.Map, caster.Location) || IsNewDungeon(map, loc)))
                    isValid = false;

                if (BaseBoat.IsDriving(caster))
                    return false;

            }

            for (int i = 0; isValid && i < m_Validators.Length; ++i)
                isValid = (m_Rules[v, i] || !m_Validators[i](map, loc));

            if (!isValid && caster != null)
                SendInvalidMessage(caster, type);

            return isValid;
        }

        public static bool CheckCanTravel(Mobile m)
        {
            if (m.Criminal)
            {
                m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            else if (CheckCombat(m))
            {
                m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }
            else if (WeightOverloading.IsOverloaded(m))
            {
                m.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                return false;
            }

            return true;
        }

        public static bool IsWindLoc(Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (x >= 5120 && y >= 0 && x < 5376 && y < 256);
        }

        public static bool IsFeluccaWind(Map map, Point3D loc)
        {
            return (map == Map.Felucca && IsWindLoc(loc));
        }

        public static bool IsTrammelWind(Map map, Point3D loc)
        {
            return (map == Map.Trammel && IsWindLoc(loc));
        }

        public static bool IsIlshenar(Map map, Point3D loc)
        {
            return (map == Map.Ilshenar);
        }

        public static bool IsTrammelSolenHive(Map map, Point3D loc)
        {
            return map == Map.Trammel && Region.Find(loc, map).Name == "Solen Hives";
        }

        public static bool IsFeluccaSolenHive(Map map, Point3D loc)
        {
            return map == Map.Felucca && Region.Find(loc, map).Name == "Solen Hives";
        }

        public static bool IsFeluccaT2A(Map map, Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (map == Map.Felucca && x >= 5120 && y >= 2304 && x < 6144 && y < 4096);
        }

        public static bool IsAnyT2A(Map map, Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return ((map == Map.Trammel || map == Map.Felucca) && x >= 5120 && y >= 2304 && x < 6144 && y < 4096);
        }

        public static bool IsFeluccaDungeon(Map map, Point3D loc)
        {
            Region region = Region.Find(loc, map);
            return (region.IsPartOf<DungeonRegion>() && region.Map == Map.Felucca);
        }

        public static bool IsKhaldun(Map map, Point3D loc)
        {
            return (Region.Find(loc, map).Name == "Khaldun");
        }

        public static bool IsSafeZone(Map map, Point3D loc)
        {
            return false;
        }

        public static bool IsChampionSpawn(Map map, Point3D loc)
        {
            return (Region.Find(loc, map).IsPartOf<Engines.CannedEvil.ChampionSpawnRegion>());
        }

        public static bool IsDoomFerry(Map map, Point3D loc)
        {
            if (map != Map.Malas)
                return false;

            int x = loc.X, y = loc.Y;

            if (x >= 426 && y >= 314 && x <= 430 && y <= 331)
                return true;

            if (x >= 406 && y >= 247 && x <= 410 && y <= 264)
                return true;

            return false;
        }

        public static bool IsTokunoDungeon(Map map, Point3D loc)
        {
            //The tokuno dungeons are really inside malas
            if (map != Map.Malas)
                return false;

            int x = loc.X, y = loc.Y, z = loc.Z;

            bool r1 = (x >= 0 && y >= 0 && x <= 128 && y <= 128);
            bool r2 = (x >= 45 && y >= 320 && x < 195 && y < 710);

            return (r1 || r2);
        }

        public static bool IsDoomGauntlet(Map map, Point3D loc)
        {
            if (map != Map.Malas)
                return false;

            int x = loc.X - 256, y = loc.Y - 304;

            return (x >= 0 && y >= 0 && x < 256 && y < 256);
        }

        public static bool IsLampRoom(Map map, Point3D loc)
        {
            if (map != Map.Malas)
                return false;

            int x = loc.X, y = loc.Y;

            return (x >= 465 && y >= 92 && x < 474 && y < 102);
        }

        public static bool IsGuardianRoom(Map map, Point3D loc)
        {
            if (map != Map.Malas)
                return false;

            int x = loc.X, y = loc.Y;

            return (x >= 356 && y >= 5 && x < 375 && y < 25);
        }

        public static bool IsHeartwood(Map map, Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (map == Map.Trammel || map == Map.Felucca) && (x >= 6911 && y >= 254 && x < 7167 && y < 511);
        }

        public static bool IsMLDungeon(Map map, Point3D loc)
        {
            return MondainsLegacy.IsMLRegion(Region.Find(loc, map));
        }

        public static bool IsTombOfKings(Map map, Point3D loc)
        {
            return (Region.Find(loc, map).IsPartOf(typeof(TombOfKingsRegion)));
        }

        public static bool IsMazeOfDeath(Map map, Point3D loc)
        {
            return (Region.Find(loc, map).IsPartOf(typeof(MazeOfDeathRegion)));
        }

        public static bool IsSAEntrance(Map map, Point3D loc)
        {
            return map == Map.TerMur && loc.X >= 1122 && loc.Y >= 1067 && loc.X <= 1144 && loc.Y <= 1086;
        }

        public static bool IsSADungeon(Map map, Point3D loc)
        {
            if (map != Map.TerMur)
                return false;

            Region region = Region.Find(loc, map);
            return (region.IsPartOf(typeof(DungeonRegion)) && !region.IsPartOf(typeof(TombOfKingsRegion)));
        }

        public static bool IsEodon(Map map, Point3D loc)
        {
            if (map == Map.Felucca && loc.X >= 6975 && loc.X <= 7042 && loc.Y >= 2048 && loc.Y <= 2115)
                return true;

            return map == Map.TerMur && loc.X >= 0 && loc.X <= 1087 && loc.Y >= 1344 && loc.Y <= 2495;
        }

        public static bool IsNewDungeon(Map map, Point3D loc)
        {
            if (map == Map.Trammel)
            {
                Region r = Region.Find(loc, map);

                // Revamped Dungeons with specific rules
                if (r.Name == "Void Pool" || r.Name == "Wrong")
                    return true;
            }

            return false;
        }

        public static bool IsInvalid(Map map, Point3D loc)
        {
            if (map == null || map == Map.Internal)
                return true;

            int x = loc.X, y = loc.Y;

            return (x < 0 || y < 0 || x >= map.Width || y >= map.Height);
        }

        //towns
        public static bool IsTown(IPoint3D loc, Mobile caster)
        {
            if (loc is Item)
                loc = ((Item)loc).GetWorldLocation();

            return IsTown(new Point3D(loc), caster);
        }

        public static bool IsTown(Point3D loc, Mobile caster)
        {
            Map map = caster.Map;

            if (map == null)
                return false;

            GuardedRegion reg = (GuardedRegion)Region.Find(loc, map).GetRegion(typeof(GuardedRegion));

            return (reg != null && !reg.IsDisabled());
        }

        public static bool CheckTown(IPoint3D loc, Mobile caster)
        {
            if (loc is Item)
                loc = ((Item)loc).GetWorldLocation();

            return CheckTown(new Point3D(loc), caster);
        }

        public static bool CheckTown(Point3D loc, Mobile caster)
        {
            if (IsTown(loc, caster))
            {
                caster.SendLocalizedMessage(500946); // You cannot cast this in town!
                return false;
            }

            return true;
        }

        //magic reflection
        public static bool CheckReflect(Spell spell, Mobile caster, ref Mobile target)
        {
            IDamageable c = caster as IDamageable;
            IDamageable t = target as IDamageable;

            bool reflect = CheckReflect(spell, ref c, ref t);

            if (c is Mobile)
                caster = (Mobile)c;

            if (t is Mobile)
                target = (Mobile)t;

            return reflect;
        }

        public static bool CheckReflect(Spell spell, IDamageable caster, ref Mobile target)
        {
            IDamageable t = target as IDamageable;

            bool reflect = CheckReflect(spell, ref caster, ref t);

            if (t is Mobile)
                caster = (Mobile)t;

            return reflect;
        }

        public static bool CheckReflect(Spell spell, Mobile caster, ref IDamageable target)
        {
            IDamageable c = caster as IDamageable;

            bool reflect = CheckReflect(spell, ref c, ref target);

            if (c is Mobile)
                caster = (Mobile)c;

            return reflect;
        }

        public static bool CheckReflect(Spell spell, ref Mobile caster, ref IDamageable target)
        {
            IDamageable c = caster as IDamageable;

            bool reflect = CheckReflect(spell, ref c, ref target);

            if (c is Mobile)
                caster = (Mobile)c;

            return reflect;
        }

        public static bool CheckReflect(Spell spell, ref Mobile caster, ref Mobile target)
        {
            return CheckReflect(spell, caster, ref target);
        }

        public static bool CheckReflect(Spell spell, ref IDamageable source, ref IDamageable defender)
        {
            bool reflect = false;

            if (spell.SpellDamageType == DamageType.Spell)
            {
                if (defender is DamageableItem && ((DamageableItem)defender).CheckReflect(spell, source))
                {
                    IDamageable temp = source;
                    source = defender;
                    defender = temp;
                    return true;
                }

                var caster = source as Mobile;
                var target = defender as Mobile;

                if (caster != null && target != null)
                {
                    Clone clone = MirrorImage.GetDeflect(target, (Mobile)defender);

                    if (clone != null)
                    {
                        defender = clone;
                        return false;
                    }

                    var context = MagicReflectSpell.GetContext(target);

                    if (context != null)
                    {
                        reflect = MagicReflectSpell.CheckReflectDamage(target, spell);

                        if (reflect)
                        {
                            IDamageable temp = source;
                            source = defender;
                            defender = temp;
                        }
                    }

                    if (!reflect)
                    {
                        var bc = defender as BaseCreature;

                        if (bc != null)
                        {
                            ((BaseCreature)target).CheckReflect(caster, ref reflect);

                            if (reflect)
                            {
                                target.FixedEffect(0x37B9, 10, 5);

                                IDamageable temp = source;
                                source = defender;
                                defender = temp;
                            }
                        }
                    }
                }
            }

            return reflect;
        }

        public static void Damage(Spell spell, Mobile target, double damage)
        {
            TimeSpan ts = GetDamageDelayForSpell(spell);

            Damage(spell, ts, target, spell.Caster, damage);
        }

        public static void Damage(TimeSpan delay, Mobile target, double damage)
        {
            Damage(delay, target, null, damage);
        }

        public static void Damage(TimeSpan delay, Mobile target, Mobile from, double damage)
        {
            Damage(null, delay, target, from, damage);
        }

        public static void Damage(Spell spell, TimeSpan delay, Mobile target, Mobile from, double damage)
        {
            int iDamage = (int)damage;

            if (delay == TimeSpan.Zero)
            {
                if (from is BaseCreature)
                    ((BaseCreature)from).AlterSpellDamageTo(target, ref iDamage);

                if (target is BaseCreature)
                    ((BaseCreature)target).AlterSpellDamageFrom(from, ref iDamage);

                target.Damage(iDamage, from);
            }
            else
            {
                new SpellDamageTimer(spell, target, from, iDamage, delay).Start();
            }

            if (target is BaseCreature && from != null && delay == TimeSpan.Zero)
            {
                BaseCreature c = (BaseCreature)target;

                c.OnHarmfulSpell(from);
                c.OnDamagedBySpell(from);
            }
        }

        public static void Damage(Spell spell, IDamageable damageable, double damage, int phys, int fire, int cold, int pois, int nrgy)
        {
            TimeSpan ts = GetDamageDelayForSpell(spell);

            Damage(spell, ts, damageable, spell.Caster, damage, phys, fire, cold, pois, nrgy, DFAlgorithm.Standard);
        }

        public static void Damage(Spell spell, IDamageable damageable, double damage, int phys, int fire, int cold, int pois, int nrgy, DFAlgorithm dfa)
        {
            TimeSpan ts = GetDamageDelayForSpell(spell);

            Damage(spell, ts, damageable, spell.Caster, damage, phys, fire, cold, pois, nrgy, dfa);
        }

        public static void Damage(Spell spell, IDamageable damageable, double damage, int phys, int fire, int cold, int pois, int nrgy, int chaos, int direct)
        {
            TimeSpan ts = GetDamageDelayForSpell(spell);

            Damage(spell, ts, damageable, spell.Caster, damage, phys, fire, cold, pois, nrgy, DFAlgorithm.Standard, chaos, direct);
        }

        public static void Damage(TimeSpan delay, IDamageable damageable, double damage, int phys, int fire, int cold, int pois, int nrgy)
        {
            Damage(delay, damageable, null, damage, phys, fire, cold, pois, nrgy);
        }

        public static void Damage(TimeSpan delay, IDamageable damageable, double damage, int phys, int fire, int cold, int pois, int nrgy, int chaos, int direct)
        {
            Damage(null, delay, damageable, null, damage, phys, fire, cold, pois, nrgy, DFAlgorithm.Standard, chaos, direct);
        }

        public static void Damage(TimeSpan delay, IDamageable damageable, Mobile from, double damage, int phys, int fire, int cold, int pois, int nrgy)
        {
            Damage(delay, damageable, from, damage, phys, fire, cold, pois, nrgy, DFAlgorithm.Standard);
        }

        public static void Damage(TimeSpan delay, IDamageable damageable, Mobile from, double damage, int phys, int fire, int cold, int pois, int nrgy, DFAlgorithm dfa)
        {
            Damage(null, delay, damageable, from, damage, phys, fire, cold, pois, nrgy, dfa);
        }

        public static void Damage(Spell spell, TimeSpan delay, IDamageable damageable, Mobile from, double damage, int phys, int fire, int cold, int pois, int nrgy, DFAlgorithm dfa, int chaos = 0, int direct = 0)
        {
            Mobile target = damageable as Mobile;
            int iDamage = (int)damage;

            if (delay == TimeSpan.Zero)
            {
                if (from is BaseCreature && target != null)
                    ((BaseCreature)from).AlterSpellDamageTo(target, ref iDamage);

                if (target is BaseCreature)
                    ((BaseCreature)target).AlterSpellDamageFrom(from, ref iDamage);

                DamageType dtype = spell != null ? spell.SpellDamageType : DamageType.Spell;

                if (target != null)
                {
                    target.DFA = dfa;
                }

                int damageGiven = AOS.Damage(damageable, from, iDamage, phys, fire, cold, pois, nrgy, chaos, direct, dtype);

                if (target != null)
                    Mysticism.SpellPlagueSpell.OnMobileDamaged(target);

                if (target != null && target.DFA != DFAlgorithm.Standard)
                {
                    target.DFA = DFAlgorithm.Standard;
                }

                NegativeAttributes.OnCombatAction(from);

                if (from != target)
                    NegativeAttributes.OnCombatAction(target);
            }
            else
            {
                new SpellDamageTimerAOS(spell, damageable, from, iDamage, phys, fire, cold, pois, nrgy, chaos, direct, delay, dfa).Start();
            }

            if (target is BaseCreature && from != null && delay == TimeSpan.Zero)
            {
                BaseCreature c = (BaseCreature)target;

                c.OnHarmfulSpell(from);
                c.OnDamagedBySpell(from);
            }
        }

        public static void Heal(int amount, Mobile target, Mobile from)
        {
            Heal(amount, target, from, true);
        }

        public static void Heal(int amount, Mobile target, Mobile from, bool message)
        {
            Spellweaving.ArcaneEmpowermentSpell.AddHealBonus(from, ref amount);

            if (amount > 0 && target != from && from is PlayerMobile && target is PlayerMobile)
            {
                if (SearingWounds.IsUnderEffects(target))
                {
                    amount /= 2;
                    target.SendLocalizedMessage(1151178); // The cauterized wound resists some of your healing.
                }

                int realAmount = Math.Min(amount, target.HitsMax - target.Hits);

                if (realAmount > 0 && target != from)
                    SpiritualityVirtue.OnHeal(from, realAmount);
            }

            target.Heal(amount, from, message);
        }

        private class SpellDamageTimer : Timer
        {
            private readonly Mobile m_Target;

            private readonly Mobile m_From;

            private int m_Damage;
            private readonly Spell m_Spell;

            public SpellDamageTimer(Spell s, Mobile target, Mobile from, int damage, TimeSpan delay)
                : base(delay)
            {
                m_Target = target;
                m_From = from;
                m_Damage = damage;
                m_Spell = s;

                if (m_Spell != null && m_Spell.DelayedDamage && !m_Spell.DelayedDamageStacking)
                    m_Spell.StartDelayedDamageContext(target, this);

                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (m_From is BaseCreature)
                    ((BaseCreature)m_From).AlterSpellDamageTo(m_Target, ref m_Damage);

                if (m_Target is BaseCreature)
                    ((BaseCreature)m_Target).AlterSpellDamageFrom(m_From, ref m_Damage);

                m_Target.Damage(m_Damage);
                if (m_Spell != null)
                    m_Spell.RemoveDelayedDamageContext(m_Target);
            }
        }

        public class SpellDamageTimerAOS : Timer
        {
            private readonly IDamageable m_Target;
            private readonly Mobile m_From;
            private int m_Damage;
            private readonly int m_Phys;
            private readonly int m_Fire;
            private readonly int m_Cold;
            private readonly int m_Pois;
            private readonly int m_Nrgy;
            private readonly int m_Chaos;
            private readonly int m_Direct;
            private readonly DFAlgorithm m_DFA;
            private readonly Spell m_Spell;

            public Spell Spell => m_Spell;

            public SpellDamageTimerAOS(Spell s, IDamageable target, Mobile from, int damage, int phys, int fire, int cold, int pois, int nrgy, int chaos, int direct, TimeSpan delay, DFAlgorithm dfa)
                : base(delay)
            {
                m_Target = target;
                m_From = from;
                m_Damage = damage;
                m_Phys = phys;
                m_Fire = fire;
                m_Cold = cold;
                m_Pois = pois;
                m_Nrgy = nrgy;
                m_Chaos = chaos;
                m_Direct = direct;
                m_DFA = dfa;
                m_Spell = s;

                if (m_Spell != null && m_Spell.DelayedDamage && !m_Spell.DelayedDamageStacking)
                    m_Spell.StartDelayedDamageContext(target, this);

                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                Mobile target = m_Target as Mobile;

                if (m_From is BaseCreature && target != null)
                    ((BaseCreature)m_From).AlterSpellDamageTo(target, ref m_Damage);

                if (m_Target is BaseCreature && m_From != null)
                    ((BaseCreature)m_Target).AlterSpellDamageFrom(m_From, ref m_Damage);

                DamageType dtype = m_Spell != null ? m_Spell.SpellDamageType : DamageType.Spell;

                if (target != null)
                {
                    target.DFA = m_DFA;
                }

                int damageGiven = AOS.Damage(m_Target, m_From, m_Damage, m_Phys, m_Fire, m_Cold, m_Pois, m_Nrgy, m_Chaos, m_Direct, dtype);

                if (target != null && target.DFA != DFAlgorithm.Standard)
                {
                    target.DFA = DFAlgorithm.Standard;
                }

                if (m_Target is BaseCreature && m_From != null)
                {
                    BaseCreature c = (BaseCreature)m_Target;

                    c.OnHarmfulSpell(m_From);
                    c.OnDamagedBySpell(m_From);
                }

                if (target != null)
                    Mysticism.SpellPlagueSpell.OnMobileDamaged(target);

                if (m_Spell != null)
                    m_Spell.RemoveDelayedDamageContext(m_Target);

                NegativeAttributes.OnCombatAction(m_From);

                if (m_From != target)
                    NegativeAttributes.OnCombatAction(target);
            }
        }
    }

    public class TransformationSpellHelper
    {
        private static readonly string _TimerID = "TransformSpell";

        #region Context Stuff
        private static readonly Dictionary<Mobile, TransformContext> m_Table = new Dictionary<Mobile, TransformContext>();

        public static void AddContext(Mobile m, TransformContext context)
        {
            m_Table[m] = context;
        }

        public static void RemoveContext(Mobile m, bool resetGraphics)
        {
            TransformContext context = GetContext(m);

            if (context != null)
                RemoveContext(m, context, resetGraphics);
        }

        public static void RemoveContext(Mobile m, TransformContext context, bool resetGraphics)
        {
            if (m_Table.ContainsKey(m))
            {
                m_Table.Remove(m);

                List<ResistanceMod> mods = context.Mods;

                for (int i = 0; i < mods.Count; ++i)
                    m.RemoveResistanceMod(mods[i]);

                if (resetGraphics)
                {
                    m.HueMod = -1;
                    m.BodyMod = 0;
                }

                TimerRegistry.RemoveFromRegistry(_TimerID, context.Spell);
                context.Spell.RemoveEffect(m);
            }
        }

        public static TransformContext GetContext(Mobile m)
        {
            TransformContext context = null;

            m_Table.TryGetValue(m, out context);

            return context;
        }

        public static bool UnderTransformation(Mobile m)
        {
            return (GetContext(m) != null);
        }

        public static bool UnderTransformation(Mobile m, Type type)
        {
            TransformContext context = GetContext(m);

            return (context != null && context.Type == type);
        }

        public static void CheckCastSkill(Mobile m, TransformContext context)
        {
            Spell spell = context.Spell as Spell;

            if (spell != null)
            {
                double min, max;

                spell.GetCastSkills(out min, out max);

                if (m.Skills[spell.CastSkill].Value < min)
                {
                    RemoveContext(m, context, true);
                }
            }
        }

        #endregion

        public static bool CheckCast(Mobile caster, Spell spell)
        {
            if (!caster.CanBeginAction(typeof(PolymorphSpell)))
            {
                caster.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
                return false;
            }
            else if (AnimalForm.UnderTransformation(caster))
            {
                caster.SendLocalizedMessage(1061091); // You cannot cast that spell in this form.
                return false;
            }
            else if (caster.Flying && !(spell is VampiricEmbraceSpell))
            {
                caster.SendLocalizedMessage(1112567); // You are flying.
                return false;
            }

            return true;
        }

        public static bool OnCast(Mobile caster, Spell spell)
        {
            ITransformationSpell transformSpell = spell as ITransformationSpell;

            if (transformSpell == null)
                return false;

            if (!caster.CanBeginAction(typeof(PolymorphSpell)))
            {
                caster.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
            }
            else if (DisguiseTimers.IsDisguised(caster))
            {
                caster.SendLocalizedMessage(1061631); // You can't do that while disguised.
                return false;
            }
            else if (AnimalForm.UnderTransformation(caster))
            {
                caster.SendLocalizedMessage(1061091); // You cannot cast that spell in this form.
            }
            else if (!caster.CanBeginAction(typeof(IncognitoSpell)) || (caster.IsBodyMod && GetContext(caster) == null))
            {
                spell.DoFizzle();
            }
            else if (spell.CheckSequence())
            {
                TransformContext context = GetContext(caster);
                Type ourType = spell.GetType();

                bool wasTransformed = (context != null);
                bool ourTransform = (wasTransformed && context.Type == ourType);

                if (wasTransformed)
                {
                    RemoveContext(caster, context, ourTransform);

                    if (ourTransform)
                    {
                        caster.PlaySound(0xFA);
                        caster.FixedParticles(0x3728, 1, 13, 5042, EffectLayer.Waist);
                    }
                }

                if (!ourTransform)
                {
                    List<ResistanceMod> mods = new List<ResistanceMod>();

                    if (transformSpell.PhysResistOffset != 0)
                        mods.Add(new ResistanceMod(ResistanceType.Physical, transformSpell.PhysResistOffset));

                    if (transformSpell.FireResistOffset != 0)
                        mods.Add(new ResistanceMod(ResistanceType.Fire, transformSpell.FireResistOffset));

                    if (transformSpell.ColdResistOffset != 0)
                        mods.Add(new ResistanceMod(ResistanceType.Cold, transformSpell.ColdResistOffset));

                    if (transformSpell.PoisResistOffset != 0)
                        mods.Add(new ResistanceMod(ResistanceType.Poison, transformSpell.PoisResistOffset));

                    if (transformSpell.NrgyResistOffset != 0)
                        mods.Add(new ResistanceMod(ResistanceType.Energy, transformSpell.NrgyResistOffset));

                    if (!((Body)transformSpell.Body).IsHuman)
                    {
                        IMount mt = caster.Mount;

                        if (mt != null)
                            mt.Rider = null;
                    }

                    caster.BodyMod = transformSpell.Body;
                    caster.HueMod = transformSpell.Hue;

                    for (int i = 0; i < mods.Count; ++i)
                        caster.AddResistanceMod(mods[i]);

                    transformSpell.DoEffect(caster);

                    TimerRegistry.Register(_TimerID, transformSpell, TimeSpan.FromSeconds(transformSpell.TickRate), TimeSpan.FromSeconds(transformSpell.TickRate), false, transSpell => OnTick(transSpell));

                    AddContext(caster, new TransformContext(mods, ourType, transformSpell));
                    return true;
                }
            }

            return false;
        }

        public static void OnTick(ITransformationSpell spell)
        {
            if (spell is Spell s)
            {
                var m = s.Caster;

                if (m.Deleted || !m.Alive || m.Body != spell.Body || (m.Hue != spell.Hue && !BestialSetHelper.IsBerserk(m)))
                {
                    RemoveContext(m, true);
                }
                else
                {
                    spell.OnTick(m);
                }
            }
        }
    }

    public interface ITransformationSpell
    {
        int Body { get; }
        int Hue { get; }

        int PhysResistOffset { get; }
        int FireResistOffset { get; }
        int ColdResistOffset { get; }
        int PoisResistOffset { get; }
        int NrgyResistOffset { get; }

        double TickRate { get; }
        void OnTick(Mobile m);

        void DoEffect(Mobile m);

        void RemoveEffect(Mobile m);
    }

    public class TransformContext
    {
        private readonly List<ResistanceMod> m_Mods;
        private readonly Type m_Type;
        private readonly ITransformationSpell m_Spell;

        public List<ResistanceMod> Mods => m_Mods;
        public Type Type => m_Type;
        public ITransformationSpell Spell => m_Spell;

        public TransformContext(List<ResistanceMod> mods, Type type, ITransformationSpell spell)
        {
            m_Mods = mods;
            m_Type = type;
            m_Spell = spell;
        }
    }
}
