using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Spells;
using Server.Spells.Eighth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Fifth;
using Server.Spells.Fourth;
using Server.Spells.Third;
using Server.Spells.Mysticism;
using Server.Spells.Spellweaving;
using Server.Spells.Necromancy;
using Server.Targeting;

namespace Server.Engines.ArenaSystem
{
    public class ArenaRegion : BaseRegion
    {
        public PVPArena Arena { get; set; }

        public ArenaRegion(PVPArena arena)
            : base(String.Format("Duel Arena {0}", arena.Definition.Name),
                    arena.Definition.Map, 
                    Region.DefaultPriority, 
                    arena.Definition.RegionBounds)
        {
            Arena = arena;
        }

        public override bool OnDoubleClick(Mobile m, object o)
        {
            if (Arena.CurrentDuel != null)
            {
                var duel = Arena.CurrentDuel;

                if (!duel.RidingFlyingAllowed)
                {
                    if (o is EtherealMount || o is BaseMount)
                    {
                        m.SendLocalizedMessage(1115997); // The rules prohibit riding a mount or flying.
                        return false;
                    }
                }

                if (o is BasePotion && duel.PotionRules != PotionRules.All)
                {
                    if (duel.PotionRules == PotionRules.None || o is BaseHealPotion)
                    {
                        return false;
                    }
                }
            }

            return base.OnDoubleClick(m, o);
        }

        public override bool OnBeginSpellCast(Mobile m, ISpell spell)
        {
            if (Arena.CurrentDuel != null)
            {
                var duel = Arena.CurrentDuel;

                if (!duel.SummonSpellsAllowed && (spell is AirElementalSpell || spell is EarthElementalSpell || spell is EnergyVortexSpell
                    || spell is FireElementalSpell || spell is SummonDaemonSpell || spell is WaterElementalSpell || spell is BladeSpiritsSpell
                    || spell is VengefulSpiritSpell || spell is RisingColossusSpell || spell is AnimatedWeaponSpell || spell is SummonFiendSpell
                    || spell is SummonFeySpell))
                {
                    m.SendLocalizedMessage(1149603); // The rules prohibit the use of summoning spells!
                    return false;
                }

                if (!duel.RidingFlyingAllowed && spell is FlySpell)
                {
                    m.SendLocalizedMessage(1115997); // The rules prohibit riding a mount or flying.
                    return false;
                }

                // TODO: Do these fail at cast, or target?
                /*if(!duel.FieldSpellsAllowed && (spell is FireFieldSpell || spell is ParalyzeFieldSpell || spell is PoisonFieldSpell || spell is EnergyFieldSpell
                    || spell is WallOfStoneSpell))
                {

                    return false;
                }*/
            }

            return base.OnBeginSpellCast(m, spell);
        }

        public override bool OnTarget(Mobile m, Target t, object o)
        {
            ArenaDuel duel = Arena.CurrentDuel;

            if (t is TeleportSpell.InternalTarget && Region.Find(m.Location, m.Map) != this)
            {
                m.SendLocalizedMessage(501035); // You cannot teleport from here to the destination.
                return false;
            }

            if (duel != null && !duel.FieldSpellsAllowed && (t is FireFieldSpell.InternalTarget || t is ParalyzeFieldSpell.InternalTarget || 
                t is PoisonFieldSpell.InternalTarget || t is EnergyFieldSpell.InternalTarget || t is WallOfStoneSpell.InternalTarget))
            {
                //TODO: Message? Effects?
                return false;
            }

            return base.OnTarget(m, t, o);
        }

        public override bool CheckTravel(Mobile traveller, Point3D p, TravelCheckType type)
        {
            return type > TravelCheckType.Mark;
        }

        public override bool AllowSpawn()
        {
            return false;
        }

        public override void OnDeath(Mobile m)
        {
            if (Arena != null && Arena.CurrentDuel != null && Arena.CurrentDuel.HasBegun && !Arena.CurrentDuel.InPreFight)
            {
                Arena.CurrentDuel.HandleDeath(m);
            }

            base.OnDeath(m);
        }

        public override bool AllowHarmful(Mobile from, IDamageable target)
        {
            if (Arena != null && Arena.CurrentDuel != null && Arena.CurrentDuel.Complete)
            {
                return false;
            }

            return true;
        }
    }
}