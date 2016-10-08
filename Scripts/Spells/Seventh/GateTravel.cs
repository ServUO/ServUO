using System;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Multis;

namespace Server.Spells.Seventh
{
    public class GateTravelSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Gate Travel", "Vas Rel Por",
            263,
            9032,
            Reagent.BlackPearl,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh);
        private readonly RunebookEntry m_Entry;
        public GateTravelSpell(Mobile caster, Item scroll)
            : this(caster, scroll, null)
        {
        }

        public GateTravelSpell(Mobile caster, Item scroll, RunebookEntry entry)
            : base(caster, scroll, m_Info)
        {
            this.m_Entry = entry;
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Seventh;
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
                this.Caster.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }

            return SpellHelper.CheckTravel(this.Caster, TravelCheckType.GateFrom);
        }

        public void Effect(Point3D loc, Map map, bool checkMulti, bool isboatkey = false)
        {
            if (Factions.Sigil.ExistsOn(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (map == null || (!Core.AOS && this.Caster.Map != map))
            {
                this.Caster.SendLocalizedMessage(1005570); // You can not gate to another facet.
            }
            else if (!SpellHelper.CheckTravel(this.Caster, TravelCheckType.GateFrom))
            {
            }
            else if (!SpellHelper.CheckTravel(this.Caster, map, loc, TravelCheckType.GateTo))
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
                this.Caster.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
            }
            else if (!map.CanSpawnMobile(loc.X, loc.Y, loc.Z) && !isboatkey)
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if ((checkMulti && SpellHelper.CheckMulti(loc, map)) && !isboatkey)
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (Core.SE && (this.GateExistsAt(map, loc) || this.GateExistsAt(this.Caster.Map, this.Caster.Location))) // SE restricted stacking gates
            {
                this.Caster.SendLocalizedMessage(1071242); // There is already a gate there.
            }
            else if (Server.Engines.CityLoyalty.CityTradeSystem.HasTrade(Caster))
            {
                Caster.SendLocalizedMessage(1151733); // You cannot do that while carrying a Trade Order.
            }
            else if (this.CheckSequence())
            {
                this.Caster.SendLocalizedMessage(501024); // You open a magical gate to another location

                Effects.PlaySound(this.Caster.Location, this.Caster.Map, 0x20E);

                InternalItem firstGate = new InternalItem(loc, map);
                firstGate.MoveToWorld(this.Caster.Location, this.Caster.Map);

                Effects.PlaySound(loc, map, 0x20E);

                InternalItem secondGate = new InternalItem(this.Caster.Location, this.Caster.Map);
                secondGate.MoveToWorld(loc, map);

                firstGate.LinkedGate = secondGate;
                secondGate.LinkedGate = firstGate;

                firstGate.BoatGate = BaseBoat.FindBoatAt(firstGate, firstGate.Map) != null;
                secondGate.BoatGate = BaseBoat.FindBoatAt(secondGate, secondGate.Map) != null;
            }

            this.FinishSequence();
        }

        private bool GateExistsAt(Map map, Point3D loc)
        {
            bool _gateFound = false;

            IPooledEnumerable eable = map.GetItemsInRange(loc, 0);
            foreach (Item item in eable)
            {
                if (item is Moongate || item is PublicMoongate)
                {
                    _gateFound = true;
                    break;
                }
            }
            eable.Free();

            return _gateFound;
        }

        [DispellableField]
        private class InternalItem : Moongate
        {
            private Moongate m_LinkedGate;
            private bool m_BoatGate;

            [CommandProperty(AccessLevel.GameMaster)]
            public Moongate LinkedGate { get { return m_LinkedGate; } set { m_LinkedGate = value; } }

            [CommandProperty(AccessLevel.GameMaster)]
            public bool BoatGate { get { return m_BoatGate; } set { m_BoatGate = value; } }

            public InternalItem(Point3D target, Map map)
                : base(target, map)
            {
                this.Map = map;

                if (this.ShowFeluccaWarning && map == Map.Felucca)
                    this.ItemID = 0xDDA;

                this.Dispellable = true;

                InternalTimer t = new InternalTimer(this);
                t.Start();
            }

            public override void UseGate(Mobile m)
            {
                if (m_LinkedGate == null || !(m_LinkedGate is GateTravelSpell.InternalItem) || !((GateTravelSpell.InternalItem)m_LinkedGate).BoatGate || !m_LinkedGate.Deleted)
                    base.UseGate(m);
                else
                    m.SendMessage("The other gate no longer exists.");
            }

            public override void OnLocationChange(Point3D old)
            {
                if (!m_BoatGate)
                    base.OnLocationChange(old);

                else if (m_LinkedGate != null)
                    m_LinkedGate.Target = this.Location;
            }

            public override void OnMapChange()
            {
                if (!m_BoatGate)
                    base.OnMapChange();

                else if (m_LinkedGate != null)
                    m_LinkedGate.TargetMap = this.Map;
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public override bool ShowFeluccaWarning
            {
                get
                {
                    return Core.AOS;
                }
            }
            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                this.Delete();
            }

            private class InternalTimer : Timer
            {
                private readonly Item m_Item;
                public InternalTimer(Item item)
                    : base(TimeSpan.FromSeconds(30.0))
                {
                    this.Priority = TimerPriority.OneSecond;
                    this.m_Item = item;
                }

                protected override void OnTick()
                {
                    this.m_Item.Delete();
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly GateTravelSpell m_Owner;
            public InternalTarget(GateTravelSpell owner)
                : base(12, false, TargetFlags.None)
            {
                this.m_Owner = owner;

                owner.Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501029); // Select Marked item.
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is RecallRune)
                {
                    RecallRune rune = (RecallRune)o;

                    if (rune.Marked)
                        this.m_Owner.Effect(rune.Target, rune.TargetMap, true);
                    else
                        from.SendLocalizedMessage(501803); // That rune is not yet marked.
                }
                else if (o is Runebook)
                {
                    RunebookEntry e = ((Runebook)o).Default;

                    if (e != null)
                        this.m_Owner.Effect(e.Location, e.Map, true);
                    else
                        from.SendLocalizedMessage(502354); // Target is not marked.
                }

                #region High Seas
                else if (o is ShipRune && ((ShipRune)o).Galleon != null)
                {
                    BaseGalleon galleon = ((ShipRune)o).Galleon;

                    if (!galleon.Deleted && galleon.Map != null && galleon.Map != Map.Internal && galleon.HasAccess(from))
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
                    from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 501030, from.Name, "")); // I can not gate travel from that object.
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