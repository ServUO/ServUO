using Server.Engines.JollyRoger;
using Server.Gumps;
using Server.Network;
using System;

namespace Server.Items
{
    public class MysteriousFragment : Item
    {
        public override int LabelNumber => 1159025;  // mysterious fragment

        private Timer _Timer;

        private ShrineBattleController _Controller;

        [Constructable]
        public MysteriousFragment()
            : base(0x1F13)
        {
            Hue = JollyRogerData.FragmentRandomHue();
        }

        public override void OnDoubleClick(Mobile from)
        {
            Gump g = new Gump(100, 100);
            g.AddBackground(0, 0, 454, 400, 0x24A4);
            g.AddItem(75, 120, ItemID, Hue);
            g.AddHtmlLocalized(177, 50, 250, 18, 1114513, "#1159025", 0x3442, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
            g.AddHtmlLocalized(177, 77, 250, 36, 1114513, "#1159026", 0x3442, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
            g.AddHtmlLocalized(177, 122, 250, 228, 1159027, 0xC63, true, true); // The item appears to be the jagged fragment of a larger piece.  While you cannot quite discern the origins or purpose of such a piece, it is no doubt fascinating.  The color shimmers with a strange brilliance that you feel you have seen before, yet cannot quite place.  Whatever created this fragment did so with awesome force.


            from.SendGump(g);

            from.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1157722, "its origin", from.NetState); // *Your proficiency in ~1_SKILL~ reveals more about the item*
            from.SendSound(from.Female ? 0x30B : 0x41A);
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            var drop = base.DropToWorld(from, p);

            var region = (ShrineBattleRegion)Region.Find(new Point3D(p.X, p.Y, p.Z), from.Map).GetRegion(typeof(ShrineBattleRegion));

            if (region != null && region._Controller != null)
            {
                _Controller = region._Controller;

                if (!_Controller.Active && _Controller.FragmentCount < 8 &&
                    JollyRogerData.GetShrine(this) == _Controller.Shrine)
                {
                    if (_Timer != null)
                    {
                        _Timer.Stop();
                    }

                    _Controller.FragmentCount++;

                    from.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1159028, from.NetState); // *The fragment settles into the ground and surges with power as it begins to sink!*
                    Effects.SendPacket(Location, Map, new GraphicalEffect(EffectType.FixedXYZ, Serial.Zero, Serial.Zero, 0x3735, Location, Location, 1, 120, true, true));
                    from.PlaySound(0x5C);
                    _Timer = new FragmentTimer(this, from, _Controller.FragmentCount);
                    _Timer.Start();
                }
            }

            return drop;
        }

        public override bool OnDragLift(Mobile from)
        {
            if (_Controller != null)
            {
                _Controller.FragmentCount--;
                _Controller = null;
            }

            if (_Timer != null)
            {
                _Timer.Stop();
            }

            return true;
        }

        private class FragmentTimer : Timer
        {
            private readonly int _Count;
            private readonly MysteriousFragment _Item;
            private readonly Mobile _Mobile;

            public FragmentTimer(MysteriousFragment item, Mobile m, int count)
                : base(TimeSpan.FromSeconds(5.0))
            {
                _Item = item;
                _Mobile = m;
                _Count = count;
                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (_Item != null && _Item._Controller != null && _Mobile != null)
                {
                    Effects.SendPacket(_Item.Location, _Item.Map, new GraphicalEffect(EffectType.FixedXYZ, Serial.Zero, Serial.Zero, 0x377A, _Item.Location, _Item.Location, 1, 72, true, true));

                    _Mobile.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1159029, _Mobile.NetState); // *You feel a slight energy pulse through you...*
                    Effects.SendPacket(_Mobile.Location, _Mobile.Map, new GraphicalEffect(EffectType.FixedFrom, _Mobile.Serial, Serial.Zero, 0x377A, _Mobile.Location, _Mobile.Location, 1, 72, true, true));
                    _Mobile.PlaySound(0x202);

                    if (_Item._Controller.FragmentCount == 8 && _Count == 8)
                    {
                        _Item._Controller.FragmentCount = 0;
                        DelayCall(TimeSpan.FromSeconds(5), () => _Item._Controller.Active = true);
                    }

                    JollyRogerData.FragmentIncrease(_Mobile, _Item._Controller.Shrine);

                    _Item.Delete();
                }
            }
        }

        public MysteriousFragment(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
