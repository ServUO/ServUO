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
                return (this.m_Mobile.Skills[SkillName.Musicianship].Base > 10.0);
            }
        }

        public virtual bool m_CanUseBushido
        {
            get
            {
                return (this.m_Mobile.Skills[SkillName.Bushido].Base > 10.0);
            }
        }

        public virtual bool m_CanUseChivalry
        {
            get
            {
                return (this.m_Mobile.Skills[SkillName.Chivalry].Base > 10.0);
            }
        }

        public virtual bool m_CanUseMagery
        {
            get
            {
                return (this.m_Mobile.Skills[SkillName.Magery].Base > 10.0);
            }
        }

        public virtual bool m_CanUseNecromancy
        {
            get
            {
                return (this.m_Mobile.Skills[SkillName.Necromancy].Base > 10.0);
            }
        }

        public virtual bool m_CanUseNinjitsu
        {
            get
            {
                return (this.m_Mobile.Skills[SkillName.Ninjitsu].Base > 10.0);
            }
        }

        public virtual bool m_CanUseSpellweaving
        {
            get
            {
                return (this.m_Mobile.Skills[SkillName.Spellweaving].Base >= 10.0);
            }
        }

        public virtual bool m_CanUseMystic
        {
            get
            {
                return (this.m_Mobile.Skills[SkillName.Mysticism].Base >= 10.0);
            }
        }

        public virtual bool m_SwapWeapons
        {
            get
            {
                return this.m_CanUseBushido || this.m_CanUseNinjitsu;
            }
        }

        public virtual bool m_Melees
        {
            get
            {
                if (this.m_ForceMelee)
                    return true;
                else if (this.m_Mobile.Weapon is BaseRanged)
                    return false;
                if (this.m_CanUseChivalry || this.m_CanUseBushido || this.m_CanUseNinjitsu)
                    return true;
                else if (this.m_CanUseMagery || this.m_CanUseNecromancy)
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
            if (this.m_Mobile.Deleted)
                return false;

            if (DateTime.UtcNow > this.m_NextFieldCheck)
            {
                this.CheckForFieldSpells();
                this.m_NextFieldCheck = DateTime.UtcNow + TimeSpan.FromSeconds(3);
            }

            if (DateTime.UtcNow > this.m_NextCheckArmed)
            {
                this.CheckArmed(this.m_SwapWeapons);
                this.m_NextCheckArmed = DateTime.UtcNow + TimeSpan.FromSeconds(12);
            }

            if (this.ProcessTarget())
                return true;
            else
                return base.Think();
        }

        public override bool DoActionWander()
        {
            if (this.m_Mobile.Debug)
                this.m_Mobile.DebugSay("I have no combatant");

            if (!this.m_Mobile.Hidden && !this.m_Mobile.Poisoned && this.m_CanUseNinjitsu)
                this.m_Mobile.UseSkill(SkillName.Hiding);

            if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I have detected {0}, attacking", this.m_Mobile.FocusMob.Name);

                this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                this.Action = ActionType.Combat;
            }
            else if (this.m_Mobile.Mana < this.m_Mobile.ManaMax && this.m_Mobile.Skills[SkillName.Meditation].Value > 60.0)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I am going to meditate");

                this.m_Mobile.UseSkill(SkillName.Meditation);
            }
            else
            {
                base.DoActionWander();
                this.TryToHeal();
            }

            return true;
        }

        #endregion

        public override bool DoActionCombat()
        {
            Mobile combatant = this.m_Mobile.Combatant;

            if (DateTime.UtcNow > this.m_NextPetCommand)
            {
                BaseCreature bc = null;

                foreach (Mobile m in this.m_Mobile.GetMobilesInRange(10))
                {
                    if (m == null)
                        continue;
                    else if (m is BaseCreature)
                    {
                        bc = m as BaseCreature;

                        if (bc.ControlMaster == this.m_Mobile || bc.SummonMaster == this.m_Mobile)
                        {
                            bc.ControlTarget = combatant;
                            bc.ControlOrder = OrderType.Attack;
                        }
                    }
                }

                this.m_NextPetCommand = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            }

            if (combatant == null || combatant.Deleted || combatant.Map != this.m_Mobile.Map || !combatant.Alive || combatant.IsDeadBondedPet)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("My combatant is gone, so my guard is up");

                this.Action = ActionType.Guard;
                return true;
            }

            if (!this.m_Mobile.InRange(combatant, this.m_Mobile.RangePerception))
            {
                // They are somewhat far away, can we find something else?
                if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
                {
                    this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                    this.m_Mobile.FocusMob = null;
                }
                else if (!this.m_Mobile.InRange(combatant, this.m_Mobile.RangePerception * 3))
                {
                    this.m_Mobile.Combatant = null;
                }

                combatant = this.m_Mobile.Combatant;

                if (combatant == null)
                {
                    if (this.m_Mobile.Debug)
                        this.m_Mobile.DebugSay("My combatant has fled, so I am on guard");

                    this.Action = ActionType.Guard;
                    return true;
                }
            }

            if (this.MoveTo(combatant, true, this.m_Mobile.RangeFight))
            {
                this.m_Mobile.Direction = this.m_Mobile.GetDirectionTo(combatant);
            }
            else if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("My move is blocked, so I am going to attack {0}", this.m_Mobile.FocusMob.Name);

                this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                this.Action = ActionType.Combat;

                return true;
            }
            else if (this.m_Mobile.GetDistanceToSqrt(combatant) > this.m_Mobile.RangePerception + 1)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I cannot find {0}, so my guard is up", combatant.Name);

                this.Action = ActionType.Guard;

                return true;
            }
            else
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I should be closer to {0}", combatant.Name);
            }

            if (!this.m_Mobile.Controlled && !this.m_Mobile.Summoned && !this.m_Mobile.IsParagon)
            {
                if (this.m_Mobile.Hits < this.m_Mobile.HitsMax * 20 / 100)
                {
                    // We are low on health, should we flee?
                    bool flee = false;

                    if (this.m_Mobile.Hits < combatant.Hits)
                    {
                        // We are more hurt than them
                        int diff = combatant.Hits - this.m_Mobile.Hits;

                        flee = (Utility.Random(0, 100) < (10 + diff)); // (10 + diff)% chance to flee
                    }
                    else
                    {
                        flee = Utility.Random(0, 100) < 10; // 10% chance to flee
                    }

                    if (flee)
                    {
                        if (this.m_Mobile.Debug)
                            this.m_Mobile.DebugSay("I am going to flee from {0}", combatant.Name);

                        this.Action = ActionType.Flee;
                    }
                }
            }

            if (this.TryToHeal())
                return true;
            else if (this.m_Mobile.Spell == null && DateTime.UtcNow > this.m_NextCastTime && this.m_Mobile.InRange(combatant, 12))
            {
                this.m_NextCastTime = DateTime.UtcNow + TimeSpan.FromSeconds(4);

                List<int> skill = new List<int>();

                if (this.m_CanUseBushido)
                    skill.Add(1);
                if (this.m_CanUseChivalry)
                    skill.Add(2);
                if (this.m_CanUseMagery)
                    skill.Add(3);
                if (this.m_CanUseNecromancy)
                    skill.Add(4);
                if (this.m_CanUseNinjitsu)
                    skill.Add(5);
                if (this.m_CanUseSpellweaving)
                    skill.Add(6);
                if (this.m_CanUseMystic)
                    skill.Add(7);
                if (this.m_CanUseBard)
                    skill.Add(8);

                if (skill.Count == 0)
                    return true;

                int whichone = skill[0];

                if (skill.Count > 1)
                    whichone = skill[Utility.Random(skill.Count)];

                switch( whichone )
                {
                    case 1:
                        this.BushidoPower();
                        break;
                    case 2:
                        this.ChivalryPower();
                        break;
                    case 3:
                        this.MageryPower();
                        break;
                    case 4:
                        this.NecromancerPower();
                        break;
                    case 5:
                        this.NinjitsuPower();
                        break;
                    case 6:
                        this.SpellweavingPower();
                        break;
                    case 7:
                        this.MysticPower();
                        break;
                // case 8: BardPower(); break;
                }
            }

            return true;
        }

        public override bool DoActionGuard()
        {
            if (!this.m_Mobile.Hidden && !this.m_Mobile.Poisoned && this.m_CanUseNinjitsu)
                this.m_Mobile.UseSkill(SkillName.Hiding);

            if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I have detected {0}, attacking", this.m_Mobile.FocusMob.Name);

                this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                this.Action = ActionType.Combat;
            }
            else
            {
                base.DoActionGuard();
            }

            return true;
        }

        public override bool DoActionFlee()
        {
            if (this.m_Mobile.Hits > (this.m_Mobile.HitsMax / 2))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I am stronger now, so I will continue fighting");

                this.Action = ActionType.Combat;
            }
            else if (this.m_CanUseNinjitsu && this.m_Mobile.Combatant != null && this.m_SmokeBombs > 0 && this.m_Mobile.Mana >= 10 && this.m_Mobile.Hidden == false)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I am using a smoke bomb. ");

                --this.m_SmokeBombs;

                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I have {0} smoke bombs left.", this.m_SmokeBombs.ToString());

                this.m_Mobile.Mana -= 10;
                this.m_Mobile.Hidden = true;
                Server.SkillHandlers.Stealth.OnUse(this.m_Mobile);
                this.m_Mobile.FixedParticles(0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot);
                this.m_Mobile.PlaySound(0x22F);
            }

            this.m_Mobile.FocusMob = this.m_Mobile.Combatant;
            base.DoActionFlee();

            return true;
        }

        public override bool MoveTo(Mobile m, bool run, int range)
        {
            if (this.m_Mobile.Hidden && !this.m_Mobile.Poisoned && this.m_CanUseNinjitsu)
            {
                Server.SkillHandlers.Stealth.OnUse(this.m_Mobile);

                if (base.MoveTo(m, false, range) == false)
                {
                    if (this.m_Mobile.Hidden && this.m_Mobile.AllowedStealthSteps >= 1 && this.m_CanUseNinjitsu)
                    {
                        Spell spell = new Shadowjump(this.m_Mobile, null);
                        spell.Cast();
                    }

                    return false;
                }
                else
                    return true;
            }
            else if (!this.m_Melees && m != null) 
            {
                if (this.m_Mobile.InRange(m, 2) && this.CheckMove())
                {
                    Direction d = Direction.North;

                    switch( this.m_Mobile.GetDirectionTo(m) )
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

                    return this.DoMove(d, run);
                    // base.DoActionFlee();
                }
                else if (this.m_Mobile.InRange(m, 4))
                    return true;
            }

            return base.MoveTo(m, run, range);
        }

        public override bool WalkMobileRange(Mobile m, int iSteps, bool bRun, int iWantDistMin, int iWantDistMax)
        {
            if (this.m_Mobile.Hidden && !this.m_Mobile.Poisoned && this.m_CanUseNinjitsu)
            {
                Server.SkillHandlers.Stealth.OnUse(this.m_Mobile);

                return base.WalkMobileRange(m, iSteps, false, iWantDistMin, iWantDistMax);
            }
            else
                return base.WalkMobileRange(m, iSteps, bRun, iWantDistMin, iWantDistMax);
        }

        public override void WalkRandom(int iChanceToNotMove, int iChanceToDir, int iSteps)
        {
            if (this.m_Mobile.Hidden && this.m_CanUseNinjitsu)
                Server.SkillHandlers.Stealth.OnUse(this.m_Mobile);
			
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
            Target targ = this.m_Mobile.Target;

            if (targ == null)
                return false;

            Mobile toTarget;

            toTarget = this.m_Mobile.Combatant;

            //if ( toTarget != null )
            //RunTo( toTarget );

            if (targ is DispelSpell.InternalTarget && !(this.m_Mobile.AutoDispel))
            {
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in this.m_Mobile.GetMobilesInRange(12))
                {
                    if (m is BaseCreature)
                    {
                        if (((BaseCreature)m).IsDispellable && CanTarget(this.m_Mobile, m))
                            targets.Add(m);
                    }
                }

                if (targets.Count >= 0)
                {
                    int whichone = Utility.RandomMinMax(0, targets.Count);

                    if (targets[whichone] != null)
                        targ.Invoke(this.m_Mobile, targets[whichone]);
                }
            }
            else if (targ is TeleportSpell.InternalTarget || targ is Shadowjump.InternalTarget)
            {
                if (targ is Shadowjump.InternalTarget && !this.m_Mobile.Hidden)
                    return false;

                Map map = this.m_Mobile.Map;

                if (map == null)
                {
                    targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
                    return true;
                }

                int px, py;
                bool teleportAway = (this.m_Mobile.Hits < (this.m_Mobile.Hits / 10));

                if (teleportAway)
                {
                    int rx = this.m_Mobile.X - toTarget.X;
                    int ry = this.m_Mobile.Y - toTarget.Y;
                    double d = this.m_Mobile.GetDistanceToSqrt(toTarget);

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

                    if ((targ.Range == -1 || this.m_Mobile.InRange(p, targ.Range)) && this.m_Mobile.InLOS(lt) && map.CanSpawnMobile(px + x, py + y, lt.Z) && !SpellHelper.CheckMulti(p, map))
                    {
                        targ.Invoke(this.m_Mobile, lt);
                        return true;
                    }
                }

                int teleRange = targ.Range;

                if (teleRange < 0)
                    teleRange = 12;

                for (int i = 0; i < 10; ++i)
                {
                    Point3D randomPoint = new Point3D(this.m_Mobile.X - teleRange + Utility.Random(teleRange * 2 + 1), this.m_Mobile.Y - teleRange + Utility.Random(teleRange * 2 + 1), 0);

                    LandTarget lt = new LandTarget(randomPoint, map);

                    if (this.m_Mobile.InLOS(lt) && map.CanSpawnMobile(lt.X, lt.Y, lt.Z) && !SpellHelper.CheckMulti(randomPoint, map))
                    {
                        targ.Invoke(this.m_Mobile, new LandTarget(randomPoint, map));
                        return true;
                    }
                }
            }
            else if (targ is AnimateDeadSpell.InternalTarget)
            {
                Type type = null;

                List<Item> itemtargets = new List<Item>();

                foreach (Item itemstofind in this.m_Mobile.GetItemsInRange(5))
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
                        targ.Invoke(this.m_Mobile, items);
                        break;
                    }
                }

                if (targ != null)
                    targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
            }
            else if ((targ.Flags & TargetFlags.Harmful) != 0 && toTarget != null)
            {
                if ((targ.Range == -1 || this.m_Mobile.InRange(toTarget, targ.Range)) && this.m_Mobile.CanSee(toTarget) && this.m_Mobile.InLOS(toTarget))
                {
                    targ.Invoke(this.m_Mobile, toTarget);
                }
            }
            else if ((targ.Flags & TargetFlags.Beneficial) != 0)
            {
                targ.Invoke(this.m_Mobile, this.m_Mobile);
            }
            else
            {
                targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
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

            foreach (Mobile m in from.GetMobilesInRange(12))
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