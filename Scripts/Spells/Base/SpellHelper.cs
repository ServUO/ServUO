using System;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;
using Server.Spells.Fifth;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Seventh;
using Server.Spells.Fourth;
using Server.Targeting;
using Server.Spells.SkillMasteries;

namespace Server
{
    public class DefensiveSpell
    {
        public static void Nullify(Mobile from)
        {
            if (!from.CanBeginAction(typeof(DefensiveSpell)))
                new InternalTimer(from).Start();
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;

            public InternalTimer(Mobile m)
                : base(TimeSpan.FromMinutes(1.0))
            {
                this.m_Mobile = m;

                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                this.m_Mobile.EndAction(typeof(DefensiveSpell));
            }
        }
    }
}

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

        #region Spell Mastery Focus
        public static bool HasSpellMastery(Mobile m)
        {
            //Publish 71 PVP Spell damage increase cap changes. If you have GM of one school of magic and no others, you are "focused" in that school of magic and have 30% sdi cap instead of 15%.
            List<SkillName> schools = new List<SkillName>()
            {
                SkillName.Magery,
                SkillName.AnimalTaming,
                SkillName.Musicianship,
                SkillName.Mysticism,
                SkillName.Spellweaving,
                SkillName.Chivalry,
                SkillName.Necromancy,
                SkillName.Bushido,
                SkillName.Ninjitsu
            };

            bool spellMastery = false;

            foreach (SkillName skill in schools)
            {
                if (m.Skills[skill].Base >= 30.0 && spellMastery)
                    return false;
                if (m.Skills[skill].Base >= 100.0)
                    spellMastery = true;
            }
            return spellMastery;
        }
#endregion 

        private static readonly TimeSpan AosDamageDelay = TimeSpan.FromSeconds(1.0);
        private static readonly TimeSpan OldDamageDelay = TimeSpan.FromSeconds(0.5);

        public static TimeSpan GetDamageDelayForSpell(Spell sp)
        {
            if (!sp.DelayedDamage)
                return TimeSpan.Zero;

            return (Core.AOS ? AosDamageDelay : OldDamageDelay);
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

            if (target == null)
                return;

            if (target is Item)
            {
                Item item = (Item)target;

                if (item.RootParent != from)
                    from.Direction = from.GetDirectionTo(item.GetWorldLocation());
            }
            else if (from != target)
            {
                from.Direction = from.GetDirectionTo(target);
            }
        }

        private static readonly TimeSpan CombatHeatDelay = TimeSpan.FromSeconds(30.0);
        private static readonly bool RestrictTravelCombat = true;

        public static bool CheckCombat(Mobile m)
        {
            if (!RestrictTravelCombat)
                return false;

            for (int i = 0; i < m.Aggressed.Count; ++i)
            {
                AggressorInfo info = m.Aggressed[i];

                if (info.Defender.Player && (DateTime.UtcNow - info.LastCombatTime) < CombatHeatDelay)
                    return true;
            }

            if (Core.Expansion == Expansion.AOS)
            {
                for (int i = 0; i < m.Aggressors.Count; ++i)
                {
                    AggressorInfo info = m.Aggressors[i];

                    if (info.Attacker.Player && (DateTime.UtcNow - info.LastCombatTime) < CombatHeatDelay)
                        return true;
                }
            }

            return false;
        }

        public static bool AdjustField(ref Point3D p, Map map, int height, bool mobsBlock)
        {
            if (map == null)
                return false;

            for (int offset = 0; offset < 10; ++offset)
            {
                Point3D loc = new Point3D(p.X, p.Y, p.Z - offset);

                if (map.CanFit(loc, height, true, mobsBlock))
                {
                    p = loc;
                    return true;
                }
            }

            return false;
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

        public static bool AddStatBonus(Mobile caster, Mobile target, StatType type)
        {
            return AddStatBonus(caster, target, type, GetOffset(caster, target, type, false), GetDuration(caster, target));
        }

        public static bool AddStatBonus(Mobile caster, Mobile target, StatType type, int bonus, TimeSpan duration)
        {
            int offset = bonus;
            string name = String.Format("[Magic] {0} Buff", type);

            StatMod mod = target.GetStatMod(name);
			if (mod != null)
				offset = Math.Max(mod.Offset, offset);

            target.AddStatMod(new StatMod(type, name, offset, duration));
			Timer.DelayCall(duration, RemoveStatOffsetCallback, target);

            return true;
        }

        public static bool AddStatCurse(Mobile caster, Mobile target, StatType type)
        {
            return AddStatCurse(caster, target, type, GetOffset(caster, target, type, true), GetDuration(caster, target));
        }

        public static bool AddStatCurse(Mobile caster, Mobile target, StatType type, int curse, TimeSpan duration)
        {
            int offset = curse;
            string name = String.Format("[Magic] {0} Curse", type);

            StatMod mod = target.GetStatMod(name);
			if (mod != null)
				offset = Math.Max(mod.Offset, offset);
			offset *= -1;

            target.AddStatMod(new StatMod(type, name, offset, duration));
			Timer.DelayCall(duration, RemoveStatOffsetCallback, target);
			return true;
        }

        public static TimeSpan GetDuration(Mobile caster, Mobile target)
        {
            if (Core.AOS)
            {
                int span = (((6 * caster.Skills.EvalInt.Fixed) / 50) + 1);

                if (caster.Spell is CurseSpell && SkillMasterySpell.GetSpellForParty(target, typeof(ResilienceSpell)) != null)
                    span /= 2;

                return TimeSpan.FromSeconds(span);
            }

            return TimeSpan.FromSeconds(caster.Skills[SkillName.Magery].Value * 1.2);
        }

        private static bool m_DisableSkillCheck;

        public static bool DisableSkillCheck
        {
            get
            {
                return m_DisableSkillCheck;
            }
            set
            {
                m_DisableSkillCheck = value;
            }
        }

        public static double GetOffsetScalar(Mobile caster, Mobile target, bool curse)
        {
            double percent;

            if (curse)
                percent = 8 + (caster.Skills.EvalInt.Fixed / 100) - (target.Skills.MagicResist.Fixed / 100);
            else
                percent = 1 + (caster.Skills.EvalInt.Fixed / 100);

            percent *= 0.01;

            if (percent < 0)
                percent = 0;

            return percent;
        }

        public static int GetOffset(Mobile caster, Mobile target, StatType type, bool curse)
        {
            if (Core.AOS)
            {
                if (!m_DisableSkillCheck)
                {
                    caster.CheckSkill(SkillName.EvalInt, 0.0, 120.0);

                    if (curse)
                        target.CheckSkill(SkillName.MagicResist, 0.0, 120.0);
                }

                double percent = GetOffsetScalar(caster, target, curse);

                switch( type )
                {
                    case StatType.Str:
                        return (int)(target.RawStr * percent);
                    case StatType.Dex:
                        return (int)(target.RawDex * percent);
                    case StatType.Int:
                        return (int)(target.RawInt * percent);
                }
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
                return true;

            if (to.Hidden && to.AccessLevel > from.AccessLevel)
                return false;

            #region Dueling
            PlayerMobile pmFrom = from as PlayerMobile;
            PlayerMobile pmTarg = to as PlayerMobile;

            if (pmFrom == null && from is BaseCreature)
            {
                BaseCreature bcFrom = (BaseCreature)from;

                if (bcFrom.Summoned)
                    pmFrom = bcFrom.SummonMaster as PlayerMobile;
            }

            if (pmTarg == null && to is BaseCreature)
            {
                BaseCreature bcTarg = (BaseCreature)to;

                if (bcTarg.Summoned)
                    pmTarg = bcTarg.SummonMaster as PlayerMobile;
            }

            if (pmFrom != null && pmTarg != null)
            {
                if (pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.Started && pmFrom.DuelPlayer != null && pmTarg.DuelPlayer != null)
                    return (pmFrom.DuelPlayer.Participant != pmTarg.DuelPlayer.Participant);
            }
            #endregion

            Guild fromGuild = GetGuildFor(from);
            Guild toGuild = GetGuildFor(to);

            if (fromGuild != null && toGuild != null && (fromGuild == toGuild || fromGuild.IsAlly(toGuild)))
                return false;

            Party p = Party.Get(from);

            if (p != null && p.Contains(to))
                return false;

            if (to is BaseCreature)
            {
                BaseCreature c = (BaseCreature)to;

                if (c.Controlled || c.Summoned)
                {
                    if (c.ControlMaster == from || c.SummonMaster == from)
                        return false;

                    if (p != null && (p.Contains(c.ControlMaster) || p.Contains(c.SummonMaster)))
                        return false;
                }
            }

            if (from is BaseCreature)
            {
                BaseCreature c = (BaseCreature)from;

                if (c.Controlled || c.Summoned)
                {
                    if (c.ControlMaster == to || c.SummonMaster == to)
                        return false;

                    p = Party.Get(to);

                    if (p != null && (p.Contains(c.ControlMaster) || p.Contains(c.SummonMaster)))
                        return false;
                }
            }

            // Non-enemy monsters will no longer flag area spells on each other
            if (from is BaseCreature && to is BaseCreature)
            {
                BaseCreature fromBC = (BaseCreature)from;
                BaseCreature toBC = (BaseCreature)to;

                if (fromBC.GetMaster() is BaseCreature)
                    fromBC = fromBC.GetMaster() as BaseCreature;

                if (toBC.GetMaster() is BaseCreature)
                    toBC = toBC.GetMaster() as BaseCreature;

                if (toBC.IsEnemy(fromBC))   //Natural Enemies
                    return true;

                // All involved are monsters- no damage. If falls through this statement, normal noto rules apply
                if (!toBC.Controlled && !toBC.Summoned && !fromBC.Controlled && !fromBC.Summoned) //All involved are monsters- no damage
                    return false;
            }

            if (to is BaseCreature && !((BaseCreature)to).Controlled && ((BaseCreature)to).InitialInnocent)
                return true;

            int noto = Notoriety.Compute(from, to);

            return (noto != Notoriety.Innocent || from.Kills >= 5);
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

            if (SpellHelper.FindValidSpawnLocation(map, ref p, true))
            {
                BaseCreature.Summon(creature, summoned, caster, p, sound, duration);
                return;
            }

            /*
            int offset = Utility.Random( 8 ) * 2;

            for( int i = 0; i < m_Offsets.Length; i += 2 )
            {
            int x = caster.X + m_Offsets[(offset + i) % m_Offsets.Length];
            int y = caster.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

            if( map.CanSpawnMobile( x, y, caster.Z ) )
            {
            BaseCreature.Summon( creature, caster, new Point3D( x, y, caster.Z ), sound, duration );
            return;
            }
            else
            {
            int z = map.GetAverageZ( x, y );

            if( map.CanSpawnMobile( x, y, z ) )
            {
            BaseCreature.Summon( creature, caster, new Point3D( x, y, z ), sound, duration );
            return;
            }
            }
            }
            * */

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

        private delegate bool TravelValidator(Map map, Point3D loc);

        private static readonly TravelValidator[] m_Validators = new TravelValidator[]
        {
            new TravelValidator(IsFeluccaT2A),
            new TravelValidator(IsKhaldun),
            new TravelValidator(IsIlshenar),
            new TravelValidator(IsTrammelWind),
            new TravelValidator(IsFeluccaWind),
            new TravelValidator(IsFeluccaDungeon),
            new TravelValidator(IsTrammelSolenHive),
            new TravelValidator(IsFeluccaSolenHive),
            new TravelValidator(IsCrystalCave),
            new TravelValidator(IsDoomGauntlet),
            new TravelValidator(IsDoomFerry),
            new TravelValidator(IsSafeZone),
            new TravelValidator(IsFactionStronghold),
            new TravelValidator(IsChampionSpawn),
            new TravelValidator(IsTokunoDungeon),
            new TravelValidator(IsLampRoom),
            new TravelValidator(IsGuardianRoom),
            new TravelValidator(IsHeartwood),
            new TravelValidator(IsMLDungeon)
        };

        private static readonly bool[,] m_Rules = new bool[,]
        {
					/*T2A(Fel),	Khaldun,	Ilshenar,	Wind(Tram),	Wind(Fel),	Dungeons(Fel),	Solen(Tram),	Solen(Fel),	CrystalCave(Malas),	Gauntlet(Malas),	Gauntlet(Ferry),	SafeZone,	Stronghold,	ChampionSpawn,	Dungeons(Tokuno[Malas]),	LampRoom(Doom),	GuardianRoom(Doom),	Heartwood,	MLDungeons */
/* Recall From */	{ false,	false,		true,		true,		false,		false,			true,			false,		false,				false,				false,				true,		true,		false,			true,						false,			false,				false,		false },
/* Recall To */		{ false,	false,		false,		false,		false,		false,			false,			false,		false,				false,				false,				false,		false,		false,			false,						false,			false,				false,		false },
/* Gate From */		{ false,	false,		false,		false,		false,		false,			false,			false,		false,				false,				false,				false,		false,		false,			false,						false,			false,				false,		false },
/* Gate To */		{ false,	false,		false,		false,		false,		false,			false,			false,		false,				false,				false,				false,		false,		false,			false,						false,			false,				false,		false },
/* Mark In */		{ false,	false,		false,		false,		false,		false,			false,			false,		false,				false,				false,				false,		false,		false,			false,						false,			false,				false,		false },
/* Tele From */		{ true,		true,		true,		true,		true,		true,			true,			true,		false,				true,				true,				true,		false,		true,			true,						true,			true,				false,		true },
/* Tele To */		{ true,		true,		true,		true,		true,		true,			true,			true,		false,				true,				false,				false,		false, 		true,			true,						true,			true,				false,		false },
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

            if (caster != null && caster.IsPlayer())
            {
				// Jail region
				if (caster.Region.IsPartOf(typeof(Regions.Jail)))
				{
					caster.SendLocalizedMessage(1114345); // You'll need a better jailbreak plan than that!
					return false;
				}
				else if(caster.Region is Regions.GreenAcres)
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

            m_TravelCaster = caster;
            m_TravelType = type;

            int v = (int)type;
            bool isValid = true;

            BaseRegion destination = Region.Find(loc, map) as BaseRegion;
            BaseRegion current = Region.Find(caster.Location, map) as BaseRegion;

            if (destination != null && !destination.CheckTravel(caster, loc, type))
                isValid = false;

            if (isValid && current != null && !current.CheckTravel(caster, loc, type))
                isValid = false;

            #region Mondain's Legacy
            if (m_TravelCaster != null && m_TravelCaster.Region != null)
            {
                if (m_TravelCaster.Region.IsPartOf("Blighted Grove") && loc.Z < -10)
                    isValid = false;
            }
            #endregion

            #region High Seas
            if (BaseBoat.IsDriving(caster))
                return false;
            #endregion

            for (int i = 0; isValid && i < m_Validators.Length; ++i)
                isValid = (m_Rules[v, i] || !m_Validators[i](map, loc));

            if (!isValid && caster != null)
                SendInvalidMessage(caster, type);

            return isValid;
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

        public static bool IsSolenHiveLoc(Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (x >= 5640 && y >= 1776 && x < 5935 && y < 2039);
        }

        public static bool IsTrammelSolenHive(Map map, Point3D loc)
        {
            return (map == Map.Trammel && IsSolenHiveLoc(loc));
        }

        public static bool IsFeluccaSolenHive(Map map, Point3D loc)
        {
            return (map == Map.Felucca && IsSolenHiveLoc(loc));
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
            return (region.IsPartOf(typeof(DungeonRegion)) && region.Map == Map.Felucca);
        }

        public static bool IsKhaldun(Map map, Point3D loc)
        {
            return (Region.Find(loc, map).Name == "Khaldun");
        }

        public static bool IsCrystalCave(Map map, Point3D loc)
        {
            if (map != Map.Malas || loc.Z >= -80)
                return false;

            int x = loc.X, y = loc.Y;

            return (x >= 1182 && y >= 437 && x < 1211 && y < 470) ||
                   (x >= 1156 && y >= 470 && x < 1211 && y < 503) ||
                   (x >= 1176 && y >= 503 && x < 1208 && y < 509) ||
                   (x >= 1188 && y >= 509 && x < 1201 && y < 513);
        }

        public static bool IsSafeZone(Map map, Point3D loc)
        {
            #region Duels
            if (Region.Find(loc, map).IsPartOf(typeof(Engines.ConPVP.SafeZone)))
            {
                if (m_TravelType == TravelCheckType.TeleportTo || m_TravelType == TravelCheckType.TeleportFrom)
                {
                    PlayerMobile pm = m_TravelCaster as PlayerMobile;

                    if (pm != null && pm.DuelPlayer != null && !pm.DuelPlayer.Eliminated)
                        return true;
                }

                return true;
            }
            #endregion

            return false;
        }

        public static bool IsFactionStronghold(Map map, Point3D loc)
        {
            /*// Teleporting is allowed, but only for faction members
            if ( !Core.AOS && m_TravelCaster != null && (m_TravelType == TravelCheckType.TeleportTo || m_TravelType == TravelCheckType.TeleportFrom) )
            {
            if ( Factions.Faction.Find( m_TravelCaster, true, true ) != null )
            return false;
            }*/
            return (Region.Find(loc, map).IsPartOf(typeof(Factions.StrongholdRegion)));
        }

        public static bool IsChampionSpawn(Map map, Point3D loc)
        {
            return (Region.Find(loc, map).IsPartOf(typeof(Engines.CannedEvil.ChampionSpawnRegion)));
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

            #region Dueling
            Engines.ConPVP.SafeZone sz = (Engines.ConPVP.SafeZone)Region.Find(loc, map).GetRegion(typeof(Engines.ConPVP.SafeZone));

            if (sz != null)
            {
                PlayerMobile pm = (PlayerMobile)caster;

                if (pm == null || pm.DuelContext == null || !pm.DuelContext.Started || pm.DuelPlayer == null || pm.DuelPlayer.Eliminated)
                    return true;
            }
            #endregion

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
        public static void CheckReflect(int circle, Mobile caster, ref Mobile target)
        {
            CheckReflect(circle, ref caster, ref target);
        }

        public static void CheckReflect(int circle, ref Mobile caster, ref Mobile target)
        {
            if (target.MagicDamageAbsorb > 0)
            {
                ++circle;

                target.MagicDamageAbsorb -= circle;

                // This order isn't very intuitive, but you have to nullify reflect before target gets switched

                bool reflect = (target.MagicDamageAbsorb >= 0);

                if (target is BaseCreature)
                    ((BaseCreature)target).CheckReflect(caster, ref reflect);

                if (target.MagicDamageAbsorb <= 0)
                {
                    target.MagicDamageAbsorb = 0;
                    DefensiveSpell.Nullify(target);
                }

                if (reflect)
                {
                    target.FixedEffect(0x37B9, 10, 5);

                    Mobile temp = caster;
                    caster = target;
                    target = temp;
                }
            }
            else if (target is BaseCreature)
            {
                bool reflect = false;

                ((BaseCreature)target).CheckReflect(caster, ref reflect);

                if (reflect)
                {
                    target.FixedEffect(0x37B9, 10, 5);

                    Mobile temp = caster;
                    caster = target;
                    target = temp;
                }
            }
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

                WeightOverloading.DFA = dfa;

                int damageGiven = AOS.Damage(damageable, from, iDamage, phys, fire, cold, pois, nrgy, chaos, direct);

                if (from != null && target != null) // sanity check
                {
                    DoLeech(damageGiven, from, target);
                }

                WeightOverloading.DFA = DFAlgorithm.Standard;
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

        public static void DoLeech(int damageGiven, Mobile from, Mobile target)
        {
            TransformContext context = TransformationSpellHelper.GetContext(from);

            if (context != null) /* cleanup */
            {
                if (context.Type == typeof(WraithFormSpell))
                {
                    int wraithLeech = Math.Min(target.Mana, (5 + (int)((15 * from.Skills.SpiritSpeak.Value) / 100))); // Wraith form gives 5-20% mana leech
                    int manaLeech = AOS.Scale(damageGiven, wraithLeech);
                    if (manaLeech != 0)
                    {
                        from.Mana += manaLeech;
                        from.PlaySound(0x44D);

                        target.Mana -= manaLeech;
                    }
                }
                else if (context.Type == typeof(VampiricEmbraceSpell))
                {
                    #region High Seas
                    if (target is BaseCreature && ((BaseCreature)target).TaintedLifeAura)
                    {
                        AOS.Damage(from, target, AOS.Scale(damageGiven, 20), false, 0, 0, 0, 0, 0, 0, 100, false, false, false);
                        from.SendLocalizedMessage(1116778); //The tainted life force energy damages you as your body tries to absorb it.
                    }
                    #endregion
                    else
                    {
                        from.Hits += AOS.Scale(damageGiven, 20);
                        from.PlaySound(0x44D);
                    }
                }
            }
        }

        public static void Heal(int amount, Mobile target, Mobile from)
        {
            Spellweaving.ArcaneEmpowermentSpell.AddHealBonus(from, ref amount);
            Heal(amount, target, from, true);
        }

        public static void Heal(int amount, Mobile target, Mobile from, bool message)
        {
            //TODO: All Healing *spells* go through ArcaneEmpowerment
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
                this.m_Target = target;
                this.m_From = from;
                this.m_Damage = damage;
                this.m_Spell = s;

                if (this.m_Spell != null && this.m_Spell.DelayedDamage && !this.m_Spell.DelayedDamageStacking)
                    this.m_Spell.StartDelayedDamageContext(target, this);

                this.Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (this.m_From is BaseCreature)
                    ((BaseCreature)this.m_From).AlterSpellDamageTo(this.m_Target, ref this.m_Damage);

                if (this.m_Target is BaseCreature)
                    ((BaseCreature)this.m_Target).AlterSpellDamageFrom(this.m_From, ref this.m_Damage);

                this.m_Target.Damage(this.m_Damage);
                if (this.m_Spell != null)
                    this.m_Spell.RemoveDelayedDamageContext(this.m_Target);
            }
        }

        private class SpellDamageTimerAOS : Timer
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

            public SpellDamageTimerAOS(Spell s, IDamageable target, Mobile from, int damage, int phys, int fire, int cold, int pois, int nrgy, int chaos, int direct, TimeSpan delay, DFAlgorithm dfa)
                : base(delay)
            {
                this.m_Target = target;
                this.m_From = from;
                this.m_Damage = damage;
                this.m_Phys = phys;
                this.m_Fire = fire;
                this.m_Cold = cold;
                this.m_Pois = pois;
                this.m_Nrgy = nrgy;
                this.m_Chaos = chaos;
                this.m_Direct = direct;
                this.m_DFA = dfa;
                this.m_Spell = s;
                if (this.m_Spell != null && this.m_Spell.DelayedDamage && !this.m_Spell.DelayedDamageStacking)
                    this.m_Spell.StartDelayedDamageContext(target, this);

                this.Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                Mobile target = m_Target as Mobile;

                if (this.m_From is BaseCreature && target != null)
                    ((BaseCreature)this.m_From).AlterSpellDamageTo(target, ref this.m_Damage);

                if (this.m_Target is BaseCreature && this.m_From != null)
                    ((BaseCreature)this.m_Target).AlterSpellDamageFrom(this.m_From, ref this.m_Damage);

                WeightOverloading.DFA = this.m_DFA;

                int damageGiven = AOS.Damage(this.m_Target, this.m_From, this.m_Damage, this.m_Phys, this.m_Fire, this.m_Cold, this.m_Pois, this.m_Nrgy, this.m_Chaos, this.m_Direct);

                if (this.m_From != null && target != null) // sanity check
                {
                    DoLeech(damageGiven, this.m_From, target);
                }

                WeightOverloading.DFA = DFAlgorithm.Standard;

                if (this.m_Target is BaseCreature && this.m_From != null)
                {
                    BaseCreature c = (BaseCreature)this.m_Target;

                    c.OnHarmfulSpell(this.m_From);
                    c.OnDamagedBySpell(this.m_From);
                }

                if (this.m_Spell != null)
                    this.m_Spell.RemoveDelayedDamageContext(this.m_Target);
            }
        }
    }

    public class TransformationSpellHelper
    {
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

                context.Timer.Stop();
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

        #endregion

        public static bool CheckCast(Mobile caster, Spell spell)
        {
            if (Factions.Sigil.ExistsOn(caster))
            {
                caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }
            else if (!caster.CanBeginAction(typeof(PolymorphSpell)))
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

            if (Factions.Sigil.ExistsOn(caster))
            {
                caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (!caster.CanBeginAction(typeof(PolymorphSpell)))
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
                        Mobiles.IMount mt = caster.Mount;

                        if (mt != null)
                            mt.Rider = null;
                    }

                    caster.BodyMod = transformSpell.Body;
                    caster.HueMod = transformSpell.Hue;

                    for (int i = 0; i < mods.Count; ++i)
                        caster.AddResistanceMod(mods[i]);

                    transformSpell.DoEffect(caster);

                    Timer timer = new TransformTimer(caster, transformSpell);
                    timer.Start();

                    AddContext(caster, new TransformContext(timer, mods, ourType, transformSpell));
                    return true;
                }
            }

            return false;
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
        private readonly Timer m_Timer;
        private readonly List<ResistanceMod> m_Mods;
        private readonly Type m_Type;
        private readonly ITransformationSpell m_Spell;

        public Timer Timer
        {
            get
            {
                return this.m_Timer;
            }
        }
        public List<ResistanceMod> Mods
        {
            get
            {
                return this.m_Mods;
            }
        }
        public Type Type
        {
            get
            {
                return this.m_Type;
            }
        }
        public ITransformationSpell Spell
        {
            get
            {
                return this.m_Spell;
            }
        }

        public TransformContext(Timer timer, List<ResistanceMod> mods, Type type, ITransformationSpell spell)
        {
            this.m_Timer = timer;
            this.m_Mods = mods;
            this.m_Type = type;
            this.m_Spell = spell;
        }
    }

    public class TransformTimer : Timer
    {
        private readonly Mobile m_Mobile;
        private readonly ITransformationSpell m_Spell;

        public TransformTimer(Mobile from, ITransformationSpell spell)
            : base(TimeSpan.FromSeconds(spell.TickRate), TimeSpan.FromSeconds(spell.TickRate))
        {
            this.m_Mobile = from;
            this.m_Spell = spell;

            this.Priority = TimerPriority.TwoFiftyMS;
        }

        protected override void OnTick()
        {
            if (this.m_Mobile.Deleted || !this.m_Mobile.Alive || this.m_Mobile.Body != this.m_Spell.Body || (this.m_Mobile.Hue != this.m_Spell.Hue && BestialSetHelper.IsBerserk(m_Mobile)))
            {
                TransformationSpellHelper.RemoveContext(this.m_Mobile, true);
                this.Stop();
            }
            else
            {
                this.m_Spell.OnTick(this.m_Mobile);
            }
        }
    }
}