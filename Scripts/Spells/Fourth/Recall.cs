using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Spells.Necromancy;
using Server.Targeting;

namespace Server.Spells.Fourth
{
    public class RecallSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Recall", "Kal Ort Por",
            239,
            9031,
            Reagent.BlackPearl,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot);

        private readonly RunebookEntry m_Entry;
        private readonly Runebook m_Book;
        private readonly VendorSearchMap m_SearchMap;
        private readonly AuctionMap m_AuctionMap;

        public bool NoSkillRequirement => (m_Book != null || m_AuctionMap != null || m_SearchMap != null) || TransformationSpellHelper.UnderTransformation(Caster, typeof(WraithFormSpell));

        public RecallSpell(Mobile caster, Item scroll)
            : this(caster, scroll, null, null)
        {
        }

        public RecallSpell(Mobile caster, Item scroll, RunebookEntry entry, Runebook book)
            : base(caster, scroll, m_Info)
        {
            m_Entry = entry;
            m_Book = book;
        }

        public RecallSpell(Mobile caster, Item scroll, VendorSearchMap map)
            : base(caster, scroll, m_Info)
        {
            m_SearchMap = map;
        }

        public RecallSpell(Mobile caster, Item scroll, AuctionMap map)
            : base(caster, scroll, m_Info)
        {
            m_AuctionMap = map;
        }

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override void GetCastSkills(out double min, out double max)
        {
            if (NoSkillRequirement)	//recall using Runebook charge, wraith form or using vendor search map
                min = max = 0;
            else
                base.GetCastSkills(out min, out max);
        }

        public override void OnCast()
        {
            if (m_Entry == null && m_SearchMap == null && m_AuctionMap == null)
            {
                Caster.Target = new InternalTarget(this);
            }
            else
            {
                Point3D loc;
                Map map;

                if (m_Entry != null)
                {
                    if (m_Entry.Type != RecallRuneType.Ship)
                    {
                        loc = m_Entry.Location;
                        map = m_Entry.Map;
                    }
                    else
                    {
                        Effect(m_Entry.Galleon);
                        return;
                    }
                }
                else if (m_SearchMap != null)
                {
                    loc = m_SearchMap.GetLocation(Caster);
                    map = m_SearchMap.GetMap();
                }
                else
                {
                    loc = m_AuctionMap.GetLocation(Caster);
                    map = m_AuctionMap.GetMap();
                }

                Effect(loc, map, true, false);
            }
        }

        public override bool CheckCast()
        {
            if (Engines.VvV.VvVSigil.ExistsOn(Caster))
            {
                Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }
            else if (Engines.CityLoyalty.CityTradeSystem.HasTrade(Caster))
            {
                Caster.SendLocalizedMessage(1151733); // You cannot do that while carrying a Trade Order.
                return false;
            }
            else if (Caster.Criminal)
            {
                Caster.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            else if (SpellHelper.CheckCombat(Caster))
            {
                Caster.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }
            else if (Misc.WeightOverloading.IsOverloaded(Caster))
            {
                Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                return false;
            }

            return SpellHelper.CheckTravel(Caster, TravelCheckType.RecallFrom);
        }

        public void Effect(BaseGalleon galleon)
        {
            if (galleon == null)
            {
                Caster.SendLocalizedMessage(1116767); // The ship could not be located.
            }
            else if (galleon.Map == Map.Internal)
            {
                Caster.SendLocalizedMessage(1149569); // That ship is in dry dock.
            }
            else if (!galleon.HasAccess(Caster))
            {
                Caster.SendLocalizedMessage(1116617); // You do not have permission to board this ship.
            }
            else
            {
                Effect(galleon.GetMarkedLocation(), galleon.Map, false, true);
            }
        }

        public void Effect(Point3D loc, Map map, bool checkMulti, bool isboatkey = false)
        {
            if (Engines.VvV.VvVSigil.ExistsOn(Caster))
            {
                Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (map == null)
            {
                Caster.SendLocalizedMessage(1005569); // You can not recall to another facet.
            }
            else if (!SpellHelper.CheckTravel(Caster, TravelCheckType.RecallFrom))
            {
            }
            else if (!SpellHelper.CheckTravel(Caster, map, loc, TravelCheckType.RecallTo))
            {
            }
            else if (map == Map.Felucca && Caster is PlayerMobile && ((PlayerMobile)Caster).Young)
            {
                Caster.SendLocalizedMessage(1049543); // You decide against traveling to Felucca while you are still young.
            }
            else if (SpellHelper.RestrictRedTravel && Caster.Murderer && map.Rules != MapRules.FeluccaRules)
            {
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }
            else if (Caster.Criminal)
            {
                Caster.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
            }
            else if (SpellHelper.CheckCombat(Caster))
            {
                Caster.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
            }
            else if (Misc.WeightOverloading.IsOverloaded(Caster))
            {
                Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
            }
            else if (!map.CanSpawnMobile(loc.X, loc.Y, loc.Z) && !isboatkey)
            {
                Caster.SendLocalizedMessage(501025); // Something is blocking the location.
            }
            else if (checkMulti && SpellHelper.CheckMulti(loc, map) && !isboatkey)
            {
                Caster.SendLocalizedMessage(501025); // Something is blocking the location.
            }
            else if (m_Book != null && m_Book.CurCharges <= 0)
            {
                Caster.SendLocalizedMessage(502412); // There are no charges left on that item.
            }
            else if (Caster.Holding != null)
            {
                Caster.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
            }
            else if (Engines.CityLoyalty.CityTradeSystem.HasTrade(Caster))
            {
                Caster.SendLocalizedMessage(1151733); // You cannot do that while carrying a Trade Order.
            }
            else if (CheckSequence())
            {
                BaseCreature.TeleportPets(Caster, loc, map, true);

                if (m_Book != null)
                    --m_Book.CurCharges;

                if (m_SearchMap != null)
                    m_SearchMap.OnBeforeTravel(Caster);

                if (m_AuctionMap != null)
                    m_AuctionMap.OnBeforeTravel(Caster);

                Caster.PlaySound(0x1FC);
                Caster.MoveToWorld(loc, map);
                Caster.PlaySound(0x1FC);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly RecallSpell m_Owner;

            public InternalTarget(RecallSpell owner)
                : base(10, false, TargetFlags.None)
            {
                m_Owner = owner;

                owner.Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501029); // Select Marked item.
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is RecallRune)
                {
                    RecallRune rune = (RecallRune)o;

                    if (rune.Marked)
                    {
                        if (rune.Type == RecallRuneType.Ship)
                        {
                            m_Owner.Effect(rune.Galleon);
                        }
                        else
                        {
                            m_Owner.Effect(rune.Target, rune.TargetMap, true);
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(501805); // That rune is not yet marked.
                    }
                }
                else if (o is Runebook)
                {
                    RunebookEntry e = ((Runebook)o).Default;

                    if (e != null)
                    {
                        if (e.Type == RecallRuneType.Ship)
                        {
                            m_Owner.Effect(e.Galleon);
                        }
                        else
                        {
                            m_Owner.Effect(e.Location, e.Map, true);
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(502354); // Target is not marked.
                    }
                }
                else if (o is Key && ((Key)o).KeyValue != 0 && ((Key)o).Link is BaseBoat)
                {
                    BaseBoat boat = ((Key)o).Link as BaseBoat;

                    if (!boat.Deleted && boat.CheckKey(((Key)o).KeyValue))
                        m_Owner.Effect(boat.GetMarkedLocation(), boat.Map, false, true);
                    else
                        from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "")); // I can not recall from that object.
                }
                else if (o is Engines.NewMagincia.WritOfLease)
                {
                    Engines.NewMagincia.WritOfLease lease = (Engines.NewMagincia.WritOfLease)o;

                    if (lease.RecallLoc != Point3D.Zero && lease.Facet != null && lease.Facet != Map.Internal)
                        m_Owner.Effect(lease.RecallLoc, lease.Facet, false);
                    else
                        from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "")); // I can not recall from that object.
                }

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
                m_Owner.FinishSequence();
            }
        }
    }
}
