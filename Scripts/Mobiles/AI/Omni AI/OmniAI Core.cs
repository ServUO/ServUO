// Created by Peoharen
using System;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Items;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Targeting;

namespace Server.Mobiles
{
    public partial class OmniAI : BaseAI
    {
        private DateTime m_NextCastTime;
        private DateTime m_NextHealTime;
        private DateTime m_NextFieldCheck;
        private DateTime m_NextCheckArmed;
        private DateTime m_NextPetCommand;

        #region canuse
        public virtual bool m_IsSmart
        {
            get
            {
                return false;
            }
        }

        public virtual bool m_CanUseBard
        {
            get
            {
                return (m_Mobile.Skills[SkillName.Musicianship].Base > 10.0);
            }
        }

        public virtual bool m_CanUseBushido
        {
            get
            {
                return (m_Mobile.Skills[SkillName.Bushido].Base > 10.0);
            }
        }

        public virtual bool m_CanUseChivalry
        {
            get
            {
                return (m_Mobile.Skills[SkillName.Chivalry].Base > 10.0);
            }
        }

        public virtual bool m_CanUseMagery
        {
            get
            {
                return (m_Mobile.Skills[SkillName.Magery].Base > 10.0);
            }
        }

        public virtual bool m_CanUseNecromancy
        {
            get
            {
                return (m_Mobile.Skills[SkillName.Necromancy].Base > 10.0);
            }
        }

        public virtual bool m_CanUseNinjitsu
        {
            get
            {
                return (m_Mobile.Skills[SkillName.Ninjitsu].Base > 10.0);
            }
        }

        public virtual bool m_CanUseSpellweaving
        {
            get
            {
                return (m_Mobile.Skills[SkillName.Spellweaving].Base >= 10.0);
            }
        }

        public virtual bool m_CanUseMystic
        {
            get
            {
                return (m_Mobile.Skills[SkillName.Mysticism].Base >= 10.0);
            }
        }

        public virtual bool m_SwapWeapons
        {
            get
            {
                return m_CanUseBushido || m_CanUseNinjitsu;
            }
        }

        public virtual bool m_Melees
        {
            get
            {
                if (m_ForceMelee)
                    return true;
                else if (m_Mobile.Weapon is BaseRanged)
                    return false;
                if (m_CanUseChivalry || m_CanUseBushido || m_CanUseNinjitsu)
                    return true;
                else if (m_CanUseMagery || m_CanUseNecromancy)
                    return false;
                else
                    return true;
            }
        }
        public virtual bool m_ForceMelee
        {
            get
            {
                return false;
            }
        }
        #endregion

        public OmniAI(BaseCreature m)
            : base(m)
        {
        }

        #region getoutofmyway
        public override bool Think()
        {
            if (m_Mobile.Deleted)
                return false;

            if (DateTime.UtcNow > m_NextFieldCheck)
            {
                CheckForFieldSpells();
                m_NextFieldCheck = DateTime.UtcNow + TimeSpan.FromSeconds(3);
            }

            if (DateTime.UtcNow > m_NextCheckArmed)
            {
                CheckArmed(m_SwapWeapons);
                m_NextCheckArmed = DateTime.UtcNow + TimeSpan.FromSeconds(12);
            }

            if (ProcessTarget())
                return true;
            else
                return base.Think();
        }

        public override bool DoActionWander()
        {
            m_Mobile.DebugSay("I have no combatant");

            if (!m_Mobile.Hidden && !m_Mobile.Poisoned && m_CanUseNinjitsu)
                m_Mobile.UseSkill(SkillName.Hiding);

            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                m_Mobile.DebugSay("I have detected {0}, attacking", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
            }
            else if (m_Mobile.Mana < m_Mobile.ManaMax && m_Mobile.Skills[SkillName.Meditation].Value > 60.0)
            {
                m_Mobile.DebugSay("I am going to meditate");

                m_Mobile.UseSkill(SkillName.Meditation);
            }
            else
            {
                base.DoActionWander();
                TryToHeal();
            }

            return true;
        }

        #endregion

        public override bool DoActionCombat()
        {
            Mobile c = m_Mobile.Combatant as Mobile;

            if (DateTime.UtcNow > m_NextPetCommand)
            {
                BaseCreature bc = null;
                IPooledEnumerable eable = m_Mobile.GetMobilesInRange(10);

                foreach (Mobile m in eable)
                {
                    if (m == null)
                        continue;
                    else if (m is BaseCreature)
                    {
                        bc = m as BaseCreature;

                        if (bc.ControlMaster == m_Mobile || bc.SummonMaster == m_Mobile)
                        {
                            bc.ControlTarget = c;
                            bc.ControlOrder = OrderType.Attack;
                        }
                    }
                }
                eable.Free();

                m_NextPetCommand = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            }

            if (c == null || c.Deleted || c.Map != m_Mobile.Map || !c.Alive || c.IsDeadBondedPet)
            {
                m_Mobile.DebugSay("My combatant is gone, so my guard is up");

                Action = ActionType.Guard;
                return true;
            }

            if (!m_Mobile.InRange(c, m_Mobile.RangePerception))
            {
                // They are somewhat far away, can we find something else?
                if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
                {
                    m_Mobile.Combatant = m_Mobile.FocusMob;
                    m_Mobile.FocusMob = null;
                }
                else if (!m_Mobile.InRange(c, m_Mobile.RangePerception * 3))
                {
                    m_Mobile.Combatant = null;
                }

                c = m_Mobile.Combatant as Mobile;

                if (c == null)
                {
                    m_Mobile.DebugSay("My combatant has fled, so I am on guard");

                    Action = ActionType.Guard;
                    return true;
                }
            }

            if (MoveTo(c, true, m_Mobile.RangeFight))
            {
                m_Mobile.Direction = m_Mobile.GetDirectionTo(c);
            }
            else if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                m_Mobile.DebugSay("My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;

                return true;
            }
            else if (m_Mobile.GetDistanceToSqrt(c) > m_Mobile.RangePerception + 1)
            {
                m_Mobile.DebugSay("I cannot find {0}, so my guard is up", c.Name);

                Action = ActionType.Guard;

                return true;
            }
            else
            {
                m_Mobile.DebugSay("I should be closer to {0}", c.Name);
            }

            if (!m_Mobile.Controlled && !m_Mobile.Summoned && m_Mobile.CanFlee)
            {
                if (m_Mobile.Hits < m_Mobile.HitsMax * 20 / 100)
                {
                    // We are low on health, should we flee?
                    if (Utility.Random(100) <= Math.Max(10, 10 + c.Hits - m_Mobile.Hits))
                    {
                        m_Mobile.DebugSay("I am going to flee from {0}", c.Name);
                        Action = ActionType.Flee;
                        return true;
                    }
                }
            }

            if (TryToHeal())
                return true;
            else if (m_Mobile.Spell == null && DateTime.UtcNow > m_NextCastTime && m_Mobile.InRange(c, 12))
            {
                m_NextCastTime = DateTime.UtcNow + TimeSpan.FromSeconds(4);

                List<int> skill = new List<int>();

                if (m_CanUseBushido)
                    skill.Add(1);
                if (m_CanUseChivalry)
                    skill.Add(2);
                if (m_CanUseMagery)
                    skill.Add(3);
                if (m_CanUseNecromancy)
                    skill.Add(4);
                if (m_CanUseNinjitsu)
                    skill.Add(5);
                if (m_CanUseSpellweaving)
                    skill.Add(6);
                if (m_CanUseMystic)
                    skill.Add(7);
                if (m_CanUseBard)
                    skill.Add(8);

                if (skill.Count == 0)
                    return true;

                int whichone = skill[0];

                if (skill.Count > 1)
                    whichone = skill[Utility.Random(skill.Count)];

                switch( whichone )
                {
                    case 1:
                        BushidoPower();
                        break;
                    case 2:
                        ChivalryPower();
                        break;
                    case 3:
                        MageryPower();
                        break;
                    case 4:
                        NecromancerPower();
                        break;
                    case 5:
                        NinjitsuPower();
                        break;
                    case 6:
                        SpellweavingPower();
                        break;
                    case 7:
                        MysticPower();
                        break;
                // case 8: BardPower(); break;
                }
            }

            return true;
        }

        public override bool DoActionGuard()
        {
            if (!m_Mobile.Hidden && !m_Mobile.Poisoned && m_CanUseNinjitsu)
                m_Mobile.UseSkill(SkillName.Hiding);

            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                m_Mobile.DebugSay("I have detected {0}, attacking", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
            }
            else
            {
                base.DoActionGuard();
            }

            return true;
        }

        public override bool DoActionFlee()
        {
            Mobile c = m_Mobile.Combatant as Mobile;

            if (m_Mobile.Hits > (m_Mobile.HitsMax / 2))
            {
                // If I have a target, go back and fight them
                if (c != null && m_Mobile.GetDistanceToSqrt(c) <= m_Mobile.RangePerception * 2)
                {
                    m_Mobile.DebugSay("I am stronger now, reengaging {0}", c.Name);
                    Action = ActionType.Combat;
                }
                else
                {
                    m_Mobile.DebugSay("I am stronger now, my guard is up");
                    Action = ActionType.Guard;
                }
            }
            else
            {
                if (m_CanUseNinjitsu && m_Mobile.Combatant != null && m_SmokeBombs > 0 && m_Mobile.Mana >= 10 && m_Mobile.Hidden == false)
                {
                    m_Mobile.DebugSay("I am using a smoke bomb. ");

                    --m_SmokeBombs;

                    m_Mobile.DebugSay("I have {0} smoke bombs left.", m_SmokeBombs.ToString());

                    m_Mobile.Mana -= 10;
                    m_Mobile.Hidden = true;
                    Server.SkillHandlers.Stealth.OnUse(m_Mobile);
                    m_Mobile.FixedParticles(0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot);
                    m_Mobile.PlaySound(0x22F);
                }

                base.DoActionFlee();
            }

            return true;
        }

        public override bool MoveTo(IPoint3D p, bool run, int range)
        {
            if (m_Mobile.Hidden && !m_Mobile.Poisoned && m_CanUseNinjitsu)
            {
                Server.SkillHandlers.Stealth.OnUse(m_Mobile);

                if (base.MoveTo(p, false, range) == false)
                {
                    if (m_Mobile.Hidden && m_Mobile.AllowedStealthSteps >= 1 && m_CanUseNinjitsu)
                    {
                        Spell spell = new Shadowjump(m_Mobile, null);
                        spell.Cast();
                    }

                    return false;
                }
                else
                    return true;
            }
            else if (!m_Melees && p != null)
            {
                if (m_Mobile.InRange(p, 2) && CheckMove())
                {
                    Direction d = Direction.North;

                    switch( m_Mobile.GetDirectionTo(p) )
                    {
                        case Direction.North:
                            d = Direction.South;
                            break;
                        case Direction.South:
                            d = Direction.North;
                            break;
                        case Direction.East:
                            d = Direction.West;
                            break;
                        case Direction.West:
                            d = Direction.East;
                            break;
                        case Direction.Up:
                            d = Direction.Down;
                            break;
                        case Direction.Down:
                            d = Direction.Up;
                            break;
                        case Direction.Right:
                            d = Direction.Left;
                            break;
                        case Direction.Left:
                            d = Direction.Right;
                            break;
                    }

                    return DoMove(d, run);
                    // base.DoActionFlee();
                }
                else if (m_Mobile.InRange(p, 4))
                    return true;
            }

            return base.MoveTo(p, run, range);
        }

        public override bool WalkMobileRange(IPoint3D p, int iSteps, bool bRun, int iWantDistMin, int iWantDistMax)
        {
            if (m_Mobile.Hidden && !m_Mobile.Poisoned && m_CanUseNinjitsu)
            {
                Server.SkillHandlers.Stealth.OnUse(m_Mobile);

                return base.WalkMobileRange(p, iSteps, false, iWantDistMin, iWantDistMax);
            }
            else
                return base.WalkMobileRange(p, iSteps, bRun, iWantDistMin, iWantDistMax);
        }

        public override void WalkRandom(int iChanceToNotMove, int iChanceToDir, int iSteps)
        {
            if (m_Mobile.Hidden && m_CanUseNinjitsu)
                Server.SkillHandlers.Stealth.OnUse(m_Mobile);

            base.WalkRandom(iChanceToNotMove, iChanceToDir, iSteps);
        }

        private static int[] m_RandomLocations = new int[]
        {
            -1, -1, -1, 0, -1, 1,
            0, -1, 0, 1, 1, -1,
            1, 0, 1, 1, -2, -2,
            -2, -1, -2, 0, -2, 1,
            -2, 2, -1, -2, -1, 2,
            0, -2, 0, 2, 1, -2,
            1, 2, 2, -2, 2, -1,
            2, 0, 2, 1, 2, 2
        };

        private bool ProcessTarget()
        {
            Target targ = m_Mobile.Target;

            if (targ == null)
                return false;

            Mobile toTarget;

            toTarget = m_Mobile.Combatant as Mobile;

            //if ( toTarget != null )
            //RunTo( toTarget );

            if (targ is DispelSpell.InternalTarget && !(m_Mobile.AutoDispel))
            {
                List<Mobile> targets = new List<Mobile>();
                IPooledEnumerable eable = m_Mobile.GetMobilesInRange(12);

                foreach (Mobile m in eable)
                {
                    if (m is BaseCreature)
                    {
                        if (((BaseCreature)m).IsDispellable && CanTarget(m_Mobile, m))
                            targets.Add(m);
                    }
                }

                eable.Free();

                if (targets.Count >= 0)
                {
                    int whichone = Utility.RandomMinMax(0, targets.Count);

                    if (targets[whichone] != null)
                        targ.Invoke(m_Mobile, targets[whichone]);
                }
            }
            else if (targ is TeleportSpell.InternalTarget || targ is Shadowjump.InternalTarget)
            {
                if (targ is Shadowjump.InternalTarget && !m_Mobile.Hidden)
                    return false;

                Map map = m_Mobile.Map;

                if (map == null)
                {
                    targ.Cancel(m_Mobile, TargetCancelType.Canceled);
                    return true;
                }

                int px, py;
                bool teleportAway = (m_Mobile.Hits < (m_Mobile.Hits / 10));

                if (teleportAway)
                {
                    int rx = m_Mobile.X - toTarget.X;
                    int ry = m_Mobile.Y - toTarget.Y;
                    double d = m_Mobile.GetDistanceToSqrt(toTarget);

                    px = toTarget.X + (int)(rx * (10 / d));
                    py = toTarget.Y + (int)(ry * (10 / d));
                }
                else
                {
                    px = toTarget.X;
                    py = toTarget.Y;
                }

                for (int i = 0; i < m_RandomLocations.Length; i += 2)
                {
                    int x = m_RandomLocations[i], y = m_RandomLocations[i + 1];

                    Point3D p = new Point3D(px + x, py + y, 0);

                    LandTarget lt = new LandTarget(p, map);

                    if ((targ.Range == -1 || m_Mobile.InRange(p, targ.Range)) && m_Mobile.InLOS(lt) && map.CanSpawnMobile(px + x, py + y, lt.Z) && !SpellHelper.CheckMulti(p, map))
                    {
                        targ.Invoke(m_Mobile, lt);
                        return true;
                    }
                }

                int teleRange = targ.Range;

                if (teleRange < 0)
                    teleRange = 12;

                for (int i = 0; i < 10; ++i)
                {
                    Point3D randomPoint = new Point3D(m_Mobile.X - teleRange + Utility.Random(teleRange * 2 + 1), m_Mobile.Y - teleRange + Utility.Random(teleRange * 2 + 1), 0);

                    LandTarget lt = new LandTarget(randomPoint, map);

                    if (m_Mobile.InLOS(lt) && map.CanSpawnMobile(lt.X, lt.Y, lt.Z) && !SpellHelper.CheckMulti(randomPoint, map))
                    {
                        targ.Invoke(m_Mobile, new LandTarget(randomPoint, map));
                        return true;
                    }
                }
            }
            else if (targ is AnimateDeadSpell.InternalTarget)
            {
                Type type = null;

                List<Item> itemtargets = new List<Item>();

                foreach (Item itemstofind in m_Mobile.GetItemsInRange(5))
                {
                    if (itemstofind is Corpse)
                    {
                        itemtargets.Add(itemstofind);
                    }
                }

                for (int i = 0; i < itemtargets.Count; ++i)
                {
                    Corpse items = (Corpse)itemtargets[i];

                    if (items.Owner != null)
                        type = items.Owner.GetType();

                    if (items.ItemID != 0x2006 || items.Channeled || type == typeof(PlayerMobile) || type == null || (items.Owner != null && items.Owner.Fame < 100) || ((items.Owner != null) && (items.Owner is BaseCreature) && (((BaseCreature)items.Owner).Summoned || ((BaseCreature)items.Owner).IsBonded)))
                        continue;
                    else
                    {
                        targ.Invoke(m_Mobile, items);
                        break;
                    }
                }

                if (targ != null)
                    targ.Cancel(m_Mobile, TargetCancelType.Canceled);
            }
            else if ((targ.Flags & TargetFlags.Harmful) != 0 && toTarget != null)
            {
                if ((targ.Range == -1 || m_Mobile.InRange(toTarget, targ.Range)) && m_Mobile.CanSee(toTarget) && m_Mobile.InLOS(toTarget))
                {
                    targ.Invoke(m_Mobile, toTarget);
                }
            }
            else if ((targ.Flags & TargetFlags.Beneficial) != 0)
            {
                targ.Invoke(m_Mobile, m_Mobile);
            }
            else
            {
                targ.Cancel(m_Mobile, TargetCancelType.Canceled);
            }

            return true;
        }

        #region Targeting
        public static Mobile FindRandomTarget(Mobile from)
        {
            return FindRandomTarget(from, true);
        }

        public static Mobile FindRandomTarget(Mobile from, bool allowcombatant)
        {
            List<Mobile> list = new List<Mobile>();
            IPooledEnumerable eable = from.GetMobilesInRange(12);

            foreach (Mobile m in eable)
            {
                if (m != null && m != from)
                    if (CanTarget(from, m) && from.InLOS(m))
                    {
                        if (allowcombatant && m == from.Combatant)
                            continue;
                        else
                            list.Add(m);
                    }
            }
            eable.Free();

            if (list.Count == 0)
                return null;
            if (list.Count == 1)
                return list[0];

            return list[Utility.Random(list.Count)];
        }

        public static bool CanTarget(Mobile from, Mobile to)
        {
            return CanTarget(from, to, true, false, false);
        }

        public static bool CanTarget(Mobile from, Mobile to, bool harm)
        {
            return CanTarget(from, to, harm, false, false);
        }

        public static bool CanTarget(Mobile from, Mobile to, bool harm, bool checkguildparty, bool allownull)
        {
            if (to == null)
                return false;
            else if (from == null)
                return allownull;
            else if (from == to && !harm)
                return true;
            else if ((harm && to.Blessed) || (to.AccessLevel != AccessLevel.Player && to.Hidden))
                return false;

            if (checkguildparty)
            {
                //Guilds
                Guild fromguild = GetGuild(from);
                Guild toguild = GetGuild(to);

                if (fromguild != null && toguild != null)
                    if (fromguild == toguild || fromguild.IsAlly(toguild))
                        return !harm;

                //Parties
                Party p = GetParty(from);

                if (p != null && p.Contains(to))
                    return !harm;
            }

            //Default
            if (harm)
                return (IsGoodGuy(from) && !(IsGoodGuy(to))) | (!(IsGoodGuy(from)) && IsGoodGuy(to));
            else
                return (IsGoodGuy(from) && IsGoodGuy(to)) | (!(IsGoodGuy(from)) && !(IsGoodGuy(to)));
        }

        public static bool IsGoodGuy(Mobile m)
        {
            if (m.Criminal)
                return false;

            if (m.Player && m.Kills < 5)
                return true;

            if (m is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)m;

                if (bc.Controlled || bc.Summoned)
                {
                    if (bc.ControlMaster != null)
                        return IsGoodGuy(bc.ControlMaster);
                    else if (bc.SummonMaster != null)
                        return IsGoodGuy(bc.SummonMaster);
                }
            }

            return false;
        }

        public static Guild GetGuild(Mobile m)
        {
            Guild guild = m.Guild as Guild;

            if (guild == null && m is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)m;
                m = bc.ControlMaster;

                if (m != null)
                    guild = m.Guild as Guild;

                m = bc.SummonMaster;

                if (m != null && guild == null)
                    guild = m.Guild as Guild;
            }

            return guild;
        }

        public static Party GetParty(Mobile m)
        {
            Party party = Party.Get(m);

            if (party == null && m is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)m;
                m = bc.ControlMaster;

                if (m != null)
                    party = Party.Get(m);

                m = bc.SummonMaster;

                if (m != null && party == null)
                    party = Party.Get(m);
            }

            return party;
        }
        #endregion
    }
}
