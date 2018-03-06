using Server;
using System;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.ArenaSystem
{
    public class ArenaStone : Item
    {
        public override bool ForceShowProperties { get { return true; } }
        public override int LabelNumber { get { return 1115878; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public PVPArena Arena { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowArenaEffects
        {
            get { return false; }
            set { if (value) DoArenaEffects(); if (!value) HideArenaEffects(); }
        }

        [Constructable]
        public ArenaStone(PVPArena arena)
            : base(0xEDD)
        {
            Arena = arena;

            Movable = false;
            Hue = 1194;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile && from.InRange(Location, 10))
            {
                PlayerMobile pm = from as PlayerMobile;

                if (pm.Young)
                {
                    from.SendLocalizedMessage(1116002); // Young players and Trial Account users may not participate in duels.
                }
                else if (Arena != null && PVPArenaSystem.Enabled)
                {
                    if (Arena.CurrentDuel != null && Arena.CurrentDuel.IsParticipant(pm))
                    {
                        if (Arena.CurrentDuel.InPreFight)
                        {
                            BaseGump.SendGump(new OfferDuelGump(pm, Arena.CurrentDuel, Arena, false, true));
                        }
                        else
                        {
                            from.SendLocalizedMessage(1116387); // Please wait until the session which you participated is finished completely.
                        }
                    }
                    else
                    {
                        var duel = Arena.GetPendingDuel(from);

                        if (duel == null)
                        {
                            var booked = PVPArenaSystem.Instance.GetBookedDuel(pm);

                            if (booked != null)
                            {
                                BaseGump.SendGump(new OfferDuelGump(pm, booked, booked.Arena, true));
                            }
                            else
                            {
                                BaseGump.SendGump(new ArenaStoneGump(pm, Arena));
                            }
                        }
                        else
                        {
                            BaseGump.SendGump(new PendingDuelGump(pm, duel, Arena));
                        }
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(502138); // That is too far away for you to use.
            }
        }

        private List<Item> _Items;

        public void DoArenaEffects()
        {
            _Items = new List<Item>();

            foreach (var rec in Arena.Definition.EffectAreas)
            {
                for (int x = rec.X; x < rec.X + rec.Width; x++)
                {
                    for (int y = rec.Y; y < rec.Y + rec.Height; y++)
                    {
                        var st = new Static(0x3709);
                        st.MoveToWorld(new Point3D(x, y, Arena.Definition.Map.GetAverageZ(x, y)), Map);
                        _Items.Add(st);
                    }
                }
            }
        }

        public void HideArenaEffects()
        {
            if (_Items == null)
                return;

            _Items.ForEach(s =>
                {
                    s.Delete();
                });

            ColUtility.Free(_Items);
            _Items = null;
        }

        public ArenaStone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}