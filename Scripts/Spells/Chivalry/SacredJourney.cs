using System;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Chivalry
{
    public class SacredJourneySpell : PaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Sacred Journey", "Sanctum Viatas",
            -1,
            9002);
        private readonly RunebookEntry m_Entry;
        private readonly Runebook m_Book;
        public SacredJourneySpell(Mobile caster, Item scroll)
            : this(caster, scroll, null, null)
        {
        }

        public SacredJourneySpell(Mobile caster, Item scroll, RunebookEntry entry, Runebook book)
            : base(caster, scroll, m_Info)
        {
            this.m_Entry = entry;
            this.m_Book = book;
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 15.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 10;
            }
        }
        public override int RequiredTithing
        {
            get
            {
                return 15;
            }
        }
        public override int MantraNumber
        {
            get
            {
                return 1060727;
            }
        }// Sanctum Viatas
        public override bool BlocksMovement
        {
            get
            {
                return false;
            }
        }
        public override void OnCast()
        {
            if (this.m_Entry == null)
                this.Caster.Target = new InternalTarget(this);
            else
                this.Effect(this.m_Entry.Location, this.m_Entry.Map, true, m_Entry.Galleon != null);
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (Factions.Sigil.ExistsOn(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }
            else if (this.Caster.Criminal)
            {
                this.Caster.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            else if (SpellHelper.CheckCombat(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061282); // You cannot use the Sacred Journey ability to flee from combat.
                return false;
            }
            else if (Server.Misc.WeightOverloading.IsOverloaded(this.Caster))
            {
                this.Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                return false;
            }

            return SpellHelper.CheckTravel(this.Caster, TravelCheckType.RecallFrom);
        }

        public void Effect(Point3D loc, Map map, bool checkMulti, bool isboatkey = false)
        {
            if (Factions.Sigil.ExistsOn(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (map == null || (!Core.AOS && this.Caster.Map != map))
            {
                this.Caster.SendLocalizedMessage(1005569); // You can not recall to another facet.
            }
            else if (!SpellHelper.CheckTravel(this.Caster, TravelCheckType.RecallFrom))
            {
            }
            else if (!SpellHelper.CheckTravel(this.Caster, map, loc, TravelCheckType.RecallTo))
            {
            }
            else if (map == Map.Felucca && this.Caster is PlayerMobile && ((PlayerMobile)this.Caster).Young)
            {
                this.Caster.SendLocalizedMessage(1049543); // You decide against traveling to Felucca while you are still young.
            }
            else if (this.Caster.Kills >= 5 && map != Map.Felucca)
            {
                this.Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }
            else if (this.Caster.Criminal)
            {
                this.Caster.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
            }
            else if (SpellHelper.CheckCombat(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061282); // You cannot use the Sacred Journey ability to flee from combat.
            }
            else if (Server.Misc.WeightOverloading.IsOverloaded(this.Caster))
            {
                this.Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
            }
            else if (!map.CanSpawnMobile(loc.X, loc.Y, loc.Z) && !isboatkey)
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if ((checkMulti && SpellHelper.CheckMulti(loc, map)) && !isboatkey)
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (this.m_Book != null && this.m_Book.CurCharges <= 0)
            {
                this.Caster.SendLocalizedMessage(502412); // There are no charges left on that item.
            }
            else if (Server.Engines.CityLoyalty.CityTradeSystem.HasTrade(Caster))
            {
                Caster.SendLocalizedMessage(1151733); // You cannot do that while carrying a Trade Order.
            }
            else if (this.CheckSequence())
            {
                BaseCreature.TeleportPets(this.Caster, loc, map, true);

                if (this.m_Book != null)
                    --this.m_Book.CurCharges;

                Effects.SendLocationParticles(EffectItem.Create(this.Caster.Location, this.Caster.Map, EffectItem.DefaultDuration), 0, 0, 0, 5033);

                this.Caster.PlaySound(0x1FC);
                this.Caster.MoveToWorld(loc, map);
                this.Caster.PlaySound(0x1FC);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly SacredJourneySpell m_Owner;
            public InternalTarget(SacredJourneySpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is RecallRune)
                {
                    RecallRune rune = (RecallRune)o;

                    if (rune.Marked)
                        this.m_Owner.Effect(rune.Target, rune.TargetMap, true);
                    else
                        from.SendLocalizedMessage(501805); // That rune is not yet marked.
                }
                else if (o is Runebook)
                {
                    RunebookEntry e = ((Runebook)o).Default;

                    if (e != null)
                        this.m_Owner.Effect(e.Location, e.Map, true);
                    else
                        from.SendLocalizedMessage(502354); // Target is not marked.
                }
                else if (o is Key && ((Key)o).KeyValue != 0 && ((Key)o).Link is BaseBoat)
                {
                    BaseBoat boat = ((Key)o).Link as BaseBoat;

                    if (!boat.Deleted && boat.CheckKey(((Key)o).KeyValue))
                        this.m_Owner.Effect(boat.GetMarkedLocation(), boat.Map, false);
                    else
                        from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "")); // I can not recall from that object.
                }
                else if (o is HouseRaffleDeed && ((HouseRaffleDeed)o).ValidLocation())
                {
                    HouseRaffleDeed deed = (HouseRaffleDeed)o;

                    this.m_Owner.Effect(deed.PlotLocation, deed.PlotFacet, true);
                }

                #region High Seas
                else if (o is ShipRune && ((ShipRune)o).Galleon != null)
                {
                    BaseGalleon galleon = ((ShipRune)o).Galleon;

                    if (!galleon.Deleted && galleon.Map != null && galleon.HasAccess(from))
                        m_Owner.Effect(galleon.GetMarkedLocation(), galleon.Map, false, true);
                    else
                        from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "")); // I can not recall from that object.
                }
                #endregion

                #region New Magincia
                else if (o is Server.Engines.NewMagincia.WritOfLease)
                {
                    Server.Engines.NewMagincia.WritOfLease lease = (Server.Engines.NewMagincia.WritOfLease)o;

                    if (lease.RecallLoc != Point3D.Zero && lease.Facet != null && lease.Facet != Map.Internal)
                        m_Owner.Effect(lease.RecallLoc, lease.Facet, false);
                    else
                        from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "")); // I can not recall from that object.
                }
                #endregion

                else
                {
                    from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "")); // I can not recall from that object.
                }
            }

            protected override void OnNonlocalTarget(Mobile from, object o)
            {
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}