#region References
using Server.Engines.ArenaSystem;
using Server.Engines.PartySystem;
using Server.Engines.VvV;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.SkillHandlers;
using Server.Spells.Chivalry;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server.Misc
{
    public class NotorietyHandlers
    {
        public static void Initialize()
        {
            Notoriety.Hues[Notoriety.Innocent] = 0x59;
            Notoriety.Hues[Notoriety.Ally] = 0x3F;
            Notoriety.Hues[Notoriety.CanBeAttacked] = 0x3B2;
            Notoriety.Hues[Notoriety.Criminal] = 0x3B2;
            Notoriety.Hues[Notoriety.Enemy] = 0x90;
            Notoriety.Hues[Notoriety.Murderer] = 0x22;
            Notoriety.Hues[Notoriety.Invulnerable] = 0x35;

            Notoriety.Handler = MobileNotoriety;

            Mobile.AllowBeneficialHandler = Mobile_AllowBeneficial;
            Mobile.AllowHarmfulHandler = Mobile_AllowHarmful;
        }

        private enum GuildStatus
        {
            None,
            Peaceful,
            Warring
        }

        private static GuildStatus GetGuildStatus(Mobile m)
        {
            if (m.Guild == null)
                return GuildStatus.None;

            if (((Guild)m.Guild).Enemies.Count == 0)
                return GuildStatus.Peaceful;

            return GuildStatus.Warring;
        }

        private static bool CheckBeneficialStatus(GuildStatus from, GuildStatus target)
        {
            if (from == GuildStatus.Warring || target == GuildStatus.Warring)
                return false;

            return true;
        }

        public static bool Mobile_AllowBeneficial(Mobile from, Mobile target)
        {
            if (from == null || target == null || from.IsStaff() || target.IsStaff())
                return true;

            Map map = from.Map;

            if (map != null && (map.Rules & MapRules.BeneficialRestrictions) == 0)
                return true; // In felucca, anything goes

            if (!from.Player)
                return true; // NPCs have no restrictions

            if (target is BaseCreature && !((BaseCreature)target).Controlled)
                return false; // Players cannot heal uncontrolled mobiles

            if (from is PlayerMobile && ((PlayerMobile)from).Young && target is BaseCreature &&
                ((BaseCreature)target).Controlled)
                return true;

            if (from is PlayerMobile && ((PlayerMobile)from).Young &&
                (!(target is PlayerMobile) || !((PlayerMobile)target).Young))
                return false; // Young players cannot perform beneficial actions towards older players

            Guild fromGuild = from.Guild as Guild;
            Guild targetGuild = target.Guild as Guild;

            if (fromGuild != null && targetGuild != null)
            {
                if (targetGuild == fromGuild || fromGuild.IsAlly(targetGuild))
                    return true; // Guild members can be beneficial
            }

            return CheckBeneficialStatus(GetGuildStatus(from), GetGuildStatus(target));
        }

        public static bool Mobile_AllowHarmful(Mobile from, IDamageable damageable)
        {
            Mobile target = damageable as Mobile;

            if (from == null || target == null || from.IsStaff() || target.IsStaff())
                return true;

            Map map = from.Map;

            if (map != null && (map.Rules & MapRules.HarmfulRestrictions) == 0)
                return true; // In felucca, anything goes

            BaseCreature bc = from as BaseCreature;

            if (!from.Player && !(bc != null && bc.GetMaster() != null && bc.GetMaster().IsPlayer()))
            {
                if (!CheckAggressor(from.Aggressors, target) && !CheckAggressed(from.Aggressed, target) && target is PlayerMobile && ((PlayerMobile)target).CheckYoungProtection(from))
                {
                    return false;
                }

                return true; // Uncontrolled NPCs are only restricted by the young system
            }

            // Summons should follow the same rules as their masters
            if (from is BaseCreature && ((BaseCreature)from).Summoned && ((BaseCreature)from).SummonMaster != null)
            {
                from = ((BaseCreature)from).SummonMaster;
            }

            if (target is BaseCreature && ((BaseCreature)target).Summoned && ((BaseCreature)target).SummonMaster != null)
            {
                target = ((BaseCreature)target).SummonMaster;
            }

            Guild fromGuild = GetGuildFor(from.Guild as Guild, from);
            Guild targetGuild = GetGuildFor(target.Guild as Guild, target);

            if (fromGuild != null && targetGuild != null)
            {
                if (fromGuild == targetGuild || fromGuild.IsAlly(targetGuild) || fromGuild.IsEnemy(targetGuild))
                    return true; // Guild allies or enemies can be harmful
            }

            if (ViceVsVirtueSystem.Enabled && ViceVsVirtueSystem.EnhancedRules && ViceVsVirtueSystem.IsEnemy(from, damageable))
                return true;

            if (target is BaseCreature)
            {
                if (((BaseCreature)target).Controlled)
                    return false; // Cannot harm other controlled mobiles

                if (((BaseCreature)target).Summoned && from != ((BaseCreature)target).SummonMaster)
                    return false; // Cannot harm other controlled mobiles
            }

            if (target.Player)
                return false; // Cannot harm other players

            if (!(target is BaseCreature && ((BaseCreature)target).InitialInnocent))
            {
                if (Notoriety.Compute(from, target) == Notoriety.Innocent)
                    return false; // Cannot harm innocent mobiles
            }

            return true;
        }

        public static Guild GetGuildFor(Guild def, Mobile m)
        {
            Guild g = def;

            BaseCreature c = m as BaseCreature;

            if (c != null && c.Controlled && c.ControlMaster != null && !c.ForceNotoriety)
            {
                c.DisplayGuildTitle = false;

                if (c.Map != null && c.Map != Map.Internal)
                {
                    g = (Guild)(c.Guild = c.ControlMaster.Guild);
                }
                else
                {
                    if (c.Map == null || c.Map == Map.Internal || c.ControlMaster.Guild == null)
                        g = (Guild)(c.Guild = null);
                }
            }

            return g;
        }

        public static int CorpseNotoriety(Mobile source, Corpse target)
        {
            if (target.AccessLevel > AccessLevel.VIP)
                return Notoriety.CanBeAttacked;

            Body body = target.Amount;

            BaseCreature cretOwner = target.Owner as BaseCreature;

            if (cretOwner != null)
            {
                Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
                Guild targetGuild = GetGuildFor(target.Guild, target.Owner);

                if (sourceGuild != null && targetGuild != null)
                {
                    if (sourceGuild == targetGuild)
                        return Notoriety.Ally;

                    if (sourceGuild.IsAlly(targetGuild))
                        return Notoriety.Ally;

                    if (sourceGuild.IsEnemy(targetGuild))
                        return Notoriety.Enemy;
                }

                if (ViceVsVirtueSystem.Enabled && ViceVsVirtueSystem.IsEnemy(source, cretOwner) && (ViceVsVirtueSystem.EnhancedRules || source.Map == ViceVsVirtueSystem.Facet))
                    return Notoriety.Enemy;

                if (CheckHouseFlag(source, cretOwner, target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

                int actual = Notoriety.CanBeAttacked;

                if (target.Murderer)
                    actual = Notoriety.Murderer;
                else if (body.IsMonster && IsSummoned(cretOwner))
                    actual = Notoriety.Murderer;
                else if (cretOwner.AlwaysMurderer || cretOwner.IsAnimatedDead)
                    actual = Notoriety.Murderer;

                if (DateTime.UtcNow >= (target.TimeOfDeath + Corpse.MonsterLootRightSacrifice))
                    return actual;

                Party sourceParty = Party.Get(source);

                foreach (Mobile m in target.Aggressors)
                {
                    if (m == source || (sourceParty != null && Party.Get(m) == sourceParty) || (sourceGuild != null && m.Guild == sourceGuild))
                        return actual;
                }

                return Notoriety.Innocent;
            }
            else
            {
                if (target.Murderer)
                    return Notoriety.Murderer;

                if (target.Criminal && target.Map != null && ((target.Map.Rules & MapRules.HarmfulRestrictions) == 0))
                    return Notoriety.Criminal;

                Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
                Guild targetGuild = GetGuildFor(target.Guild, target.Owner);

                if (sourceGuild != null && targetGuild != null)
                {
                    if (sourceGuild == targetGuild || sourceGuild.IsAlly(targetGuild))
                        return Notoriety.Ally;

                    if (sourceGuild.IsEnemy(targetGuild))
                        return Notoriety.Enemy;
                }

                if (CheckHouseFlag(source, target.Owner, target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

                if (!(target.Owner is PlayerMobile) && !IsPet(target.Owner as BaseCreature))
                    return Notoriety.CanBeAttacked;

                List<Mobile> list = target.Aggressors;

                foreach (Mobile m in list)
                {
                    if (m == source)
                        return Notoriety.CanBeAttacked;
                }

                return Notoriety.Innocent;
            }
        }

        public static int MobileNotoriety(Mobile source, IDamageable damageable)
        {
            Mobile target = damageable as Mobile;

            if (target == null)
                return Notoriety.CanBeAttacked;

            if (target.Blessed)
                return Notoriety.Invulnerable;

            if (target is BaseVendor && ((BaseVendor)target).IsInvulnerable)
                return Notoriety.Invulnerable;

            if (target is PlayerVendor || target is TownCrier)
                return Notoriety.Invulnerable;

            EnemyOfOneContext context = EnemyOfOneSpell.GetContext(source);

            if (context != null && context.IsEnemy(target))
                return Notoriety.Enemy;

            if (PVPArenaSystem.IsEnemy(source, target))
                return Notoriety.Enemy;

            if (PVPArenaSystem.IsFriendly(source, target))
                return Notoriety.Ally;

            if (target.IsStaff())
                return Notoriety.CanBeAttacked;

            var bc = target as BaseCreature;

            if (source.Player && bc != null)
            {
                Mobile master = bc.GetMaster();

                if (master != null && master.IsStaff())
                    return Notoriety.CanBeAttacked;

                master = bc.ControlMaster;

                if (master != null && !bc.ForceNotoriety)
                {
                    if (source == master && CheckAggressor(target.Aggressors, source))
                        return Notoriety.CanBeAttacked;

                    if (CheckAggressor(source.Aggressors, bc))
                        return Notoriety.CanBeAttacked;

                    return MobileNotoriety(source, master);
                }
            }

            if (target.Murderer)
                return Notoriety.Murderer;

            if (target.Body.IsMonster && IsSummoned(bc))
            {
                if (!(target is BaseFamiliar) && !(target is ArcaneFey) && !(target is Golem))
                    return Notoriety.Murderer;
            }

            if (bc != null && (bc.AlwaysMurderer || bc.IsAnimatedDead))
            {
                return Notoriety.Murderer;
            }

            if (target.Criminal)
                return Notoriety.Criminal;

            Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
            Guild targetGuild = GetGuildFor(target.Guild as Guild, target);

            if (sourceGuild != null && targetGuild != null)
            {
                if (sourceGuild == targetGuild)
                    return Notoriety.Ally;

                if (sourceGuild.IsAlly(targetGuild))
                    return Notoriety.Ally;

                if (sourceGuild.IsEnemy(targetGuild))
                    return Notoriety.Enemy;
            }

            if (ViceVsVirtueSystem.Enabled && ViceVsVirtueSystem.IsEnemy(source, target) && (ViceVsVirtueSystem.EnhancedRules || source.Map == ViceVsVirtueSystem.Facet))
                return Notoriety.Enemy;

            if (Stealing.ClassicMode && target is PlayerMobile && ((PlayerMobile)target).PermaFlags.Contains(source))
                return Notoriety.CanBeAttacked;

            if (bc != null && bc.AlwaysAttackable)
            {
                return Notoriety.CanBeAttacked;
            }

            if (CheckHouseFlag(source, target, target.Location, target.Map))
                return Notoriety.CanBeAttacked;

            //If Target is NOT A baseCreature, OR it's a BC and the BC is initial innocent...
            if (!(bc != null && ((BaseCreature)target).InitialInnocent))
            {
                if (!target.Body.IsHuman && !target.Body.IsGhost && !IsPet(target as BaseCreature) && !(target is PlayerMobile))
                    return Notoriety.CanBeAttacked;
            }

            if (CheckAggressor(source.Aggressors, target))
                return Notoriety.CanBeAttacked;

            if (source is PlayerMobile && CheckPetAggressor((PlayerMobile)source, target))
                return Notoriety.CanBeAttacked;

            if (CheckAggressed(source.Aggressed, target))
                return Notoriety.CanBeAttacked;

            if (source is PlayerMobile && CheckPetAggressed((PlayerMobile)source, target))
                return Notoriety.CanBeAttacked;

            if (bc != null)
            {
                if (bc.AlwaysInnocent)
                {
                    return Notoriety.Innocent;
                }
                else if (bc.Controlled && bc.ControlOrder == OrderType.Guard && bc.ControlTarget == source)
                {
                    return Notoriety.CanBeAttacked;
                }
            }

            if (source is BaseCreature)
            {
                Mobile master = ((BaseCreature)source).GetMaster();

                if (master != null)
                {
                    if (CheckAggressor(master.Aggressors, target))
                    {
                        return Notoriety.CanBeAttacked;
                    }

                    if (MobileNotoriety(master, target) == Notoriety.CanBeAttacked)
                    {
                        return Notoriety.CanBeAttacked;
                    }

                    if (target is BaseCreature)
                    {
                        return Notoriety.CanBeAttacked;
                    }
                }
            }

            return Notoriety.Innocent;
        }

        public static bool CheckHouseFlag(Mobile from, Mobile m, Point3D p, Map map)
        {
            BaseHouse house = BaseHouse.FindHouseAt(p, map, 16);

            if (house == null || house.Public || !house.IsFriend(from))
                return false;

            if (m != null && house.IsFriend(m))
                return false;

            BaseCreature c = m as BaseCreature;

            if (c != null && !c.Deleted && c.Controlled && c.ControlMaster != null)
                return !house.IsFriend(c.ControlMaster);

            return true;
        }

        public static bool IsPet(BaseCreature c)
        {
            return (c != null && c.Controlled);
        }

        public static bool IsSummoned(BaseCreature c)
        {
            return (c != null && /*c.Controlled &&*/ c.Summoned);
        }

        public static bool CheckAggressor(List<AggressorInfo> list, Mobile target)
        {
            return list != null && list.Any(info => info.Attacker == target);
        }

        public static bool CheckAggressed(List<AggressorInfo> list, Mobile target)
        {
            return list != null && list.Any(info => info.CriminalAggression && info.Defender == target);
        }

        public static bool CheckPetAggressor(PlayerMobile source, Mobile target)
        {
            return source.AllFollowers.Any(follower => CheckAggressor(follower.Aggressors, target));
        }

        public static bool CheckPetAggressed(PlayerMobile source, Mobile target)
        {
            return source.AllFollowers.Any(follower => CheckAggressed(follower.Aggressed, target));
        }
    }
}
