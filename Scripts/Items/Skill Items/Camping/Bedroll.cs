using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    [FlipableAttribute(0xA57, 0xA58, 0xA59)]
    public class Bedroll : Item
    {
        [Constructable]
        public Bedroll()
            : base(0xA57)
        {
            this.Weight = 5.0;
        }

        public Bedroll(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.Parent != null || !this.VerifyMove(from))
                return;

            if (!from.InRange(this, 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            if (this.ItemID == 0xA57) // rolled
            {
                Direction dir = PlayerMobile.GetDirection4(from.Location, this.Location);

                if (dir == Direction.North || dir == Direction.South)
                    this.ItemID = 0xA55;
                else
                    this.ItemID = 0xA56;
            }
            else // unrolled
            {
                this.ItemID = 0xA57;

                if (!from.HasGump(typeof(LogoutGump)))
                {
                    CampfireEntry entry = Campfire.GetEntry(from);

                    if (entry != null && entry.Safe)
                        from.SendGump(new LogoutGump(entry, this));
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        private class LogoutGump : Gump
        {
            private readonly Timer m_CloseTimer;
            private readonly CampfireEntry m_Entry;
            private readonly Bedroll m_Bedroll;
            public LogoutGump(CampfireEntry entry, Bedroll bedroll)
                : base(100, 0)
            {
                this.m_Entry = entry;
                this.m_Bedroll = bedroll;

                this.m_CloseTimer = Timer.DelayCall(TimeSpan.FromSeconds(10.0), new TimerCallback(CloseGump));

                this.AddBackground(0, 0, 400, 350, 0xA28);

                this.AddHtmlLocalized(100, 20, 200, 35, 1011015, false, false); // <center>Logging out via camping</center>

                /* Using a bedroll in the safety of a camp will log you out of the game safely.
                * If this is what you wish to do choose CONTINUE and you will be logged out.
                * Otherwise, select the CANCEL button to avoid logging out at this time.
                * The camp will remain secure for 10 seconds at which time this window will close
                * and you not be logged out.
                */
                this.AddHtmlLocalized(50, 55, 300, 140, 1011016, true, true);

                this.AddButton(45, 298, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(80, 300, 110, 35, 1011011, false, false); // CONTINUE

                this.AddButton(200, 298, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(235, 300, 110, 35, 1011012, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                PlayerMobile pm = this.m_Entry.Player;

                this.m_CloseTimer.Stop();

                if (Campfire.GetEntry(pm) != this.m_Entry)
                    return;

                if (info.ButtonID == 1 && this.m_Entry.Safe && this.m_Bedroll.Parent == null && this.m_Bedroll.IsAccessibleTo(pm) &&
                    this.m_Bedroll.VerifyMove(pm) && this.m_Bedroll.Map == pm.Map && pm.InRange(this.m_Bedroll, 2))
                {
                    pm.PlaceInBackpack(this.m_Bedroll);

                    pm.BedrollLogout = true;
                    sender.Dispose();
                }

                Campfire.RemoveEntry(this.m_Entry);
            }

            private void CloseGump()
            {
                Campfire.RemoveEntry(this.m_Entry);
                this.m_Entry.Player.CloseGump(typeof(LogoutGump));
            }
        }
    }
}