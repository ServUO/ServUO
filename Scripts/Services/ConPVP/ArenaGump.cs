using System;
using System.Collections.Generic;
using System.Text;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.ConPVP
{
    public class ArenasMoongate : Item
    {
        [Constructable]
        public ArenasMoongate()
            : base(0x1FD4)
        {
            this.Movable = false;
            this.Light = LightType.Circle300;
        }

        public ArenasMoongate(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "arena moongate";
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            this.Light = LightType.Circle300;
        }

        public bool UseGate(Mobile from)
        {
            if (DuelContext.CheckCombat(from))
            {
                from.SendMessage(0x22, "You have recently been in combat with another player and cannot use this moongate.");
                return false;
            }
            else if (from.Spell != null)
            {
                from.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
                return false;
            }
            else
            {
                from.CloseGump(typeof(ArenaGump));
                from.SendGump(new ArenaGump(from, this));

                if (!from.Hidden || from.IsPlayer())
                    Effects.PlaySound(from.Location, from.Map, 0x20E);

                return true;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
                this.UseGate(from);
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that
        }

        public override bool OnMoveOver(Mobile m)
        {
            return (!m.Player || this.UseGate(m));
        }
    }

    public class ArenaGump : Gump
    {
        private readonly Mobile m_From;
        private readonly ArenasMoongate m_Gate;
        private readonly List<Arena> m_Arenas;
        private int m_ColumnX = 12;
        public ArenaGump(Mobile from, ArenasMoongate gate)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Gate = gate;
            this.m_Arenas = Arena.Arenas;

            this.AddPage(0);

            int height = 12 + 20 + (this.m_Arenas.Count * 31) + 24 + 12;

            this.AddBackground(0, 0, 499 + 40, height, 0x2436);

            List<Arena> list = this.m_Arenas;

            for (int i = 1; i < list.Count; i += 2)
                this.AddImageTiled(12, 32 + (i * 31), 475 + 40, 30, 0x2430);

            this.AddAlphaRegion(10, 10, 479 + 40, height - 20);

            this.AddColumnHeader(35, null);
            this.AddColumnHeader(115, "Arena");
            this.AddColumnHeader(325, "Participants");
            this.AddColumnHeader(40, "Obs");

            this.AddButton(499 + 40 - 12 - 63 - 4 - 63, height - 12 - 24, 247, 248, 1, GumpButtonType.Reply, 0);
            this.AddButton(499 + 40 - 12 - 63, height - 12 - 24, 241, 242, 2, GumpButtonType.Reply, 0);

            for (int i = 0; i < list.Count; ++i)
            {
                Arena ar = list[i];

                string name = ar.Name;

                if (name == null)
                    name = "(no name)";

                int x = 12;
                int y = 32 + (i * 31);

                int color = (ar.Players.Count > 0 ? 0xCCFFCC : 0xCCCCCC);

                this.AddRadio(x + 3, y + 1, 9727, 9730, false, i);
                x += 35;

                this.AddBorderedText(x + 5, y + 5, 115 - 5, name, color, 0);
                x += 115;

                StringBuilder sb = new StringBuilder();

                if (ar.Players.Count > 0)
                {
                    Ladder ladder = Ladder.Instance;

                    if (ladder == null)
                        continue;

                    LadderEntry p1 = null, p2 = null, p3 = null, p4 = null;

                    for (int j = 0; j < ar.Players.Count; ++j)
                    {
                        Mobile mob = (Mobile)ar.Players[j];
                        LadderEntry c = ladder.Find(mob);

                        if (p1 == null || c.Index < p1.Index)
                        {
                            p4 = p3;
                            p3 = p2;
                            p2 = p1;
                            p1 = c;
                        }
                        else if (p2 == null || c.Index < p2.Index)
                        {
                            p4 = p3;
                            p3 = p2;
                            p2 = c;
                        }
                        else if (p3 == null || c.Index < p3.Index)
                        {
                            p4 = p3;
                            p3 = c;
                        }
                        else if (p4 == null || c.Index < p4.Index)
                        {
                            p4 = c;
                        }
                    }

                    this.Append(sb, p1);
                    this.Append(sb, p2);
                    this.Append(sb, p3);
                    this.Append(sb, p4);

                    if (ar.Players.Count > 4)
                        sb.Append(", ...");
                }
                else
                {
                    sb.Append("Empty");
                }

                this.AddBorderedText(x + 5, y + 5, 325 - 5, sb.ToString(), color, 0);
                x += 325;

                this.AddBorderedText(x, y + 5, 40, this.Center(ar.Spectators.ToString()), color, 0);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID != 1)
                return;

            int[] switches = info.Switches;

            if (switches.Length == 0)
                return;

            int opt = switches[0];

            if (opt < 0 || opt >= this.m_Arenas.Count)
                return;

            Arena arena = this.m_Arenas[opt];

            if (!this.m_From.InRange(this.m_Gate.GetWorldLocation(), 1) || this.m_From.Map != this.m_Gate.Map)
            {
                this.m_From.SendLocalizedMessage(1019002); // You are too far away to use the gate.
            }
            else if (DuelContext.CheckCombat(this.m_From))
            {
                this.m_From.SendMessage(0x22, "You have recently been in combat with another player and cannot use this moongate.");
            }
            else if (this.m_From.Spell != null)
            {
                this.m_From.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
            }
            else if (this.m_From.Map == arena.Facet && arena.Zone.Contains(this.m_From))
            {
                this.m_From.SendLocalizedMessage(1019003); // You are already there.
            }
            else
            {
                BaseCreature.TeleportPets(this.m_From, arena.GateIn, arena.Facet);

                this.m_From.Combatant = null;
                this.m_From.Warmode = false;
                this.m_From.Hidden = true;

                this.m_From.MoveToWorld(arena.GateIn, arena.Facet);

                Effects.PlaySound(arena.GateIn, arena.Facet, 0x1FE);
            }
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        private void Append(StringBuilder sb, LadderEntry le)
        {
            if (le == null)
                return;

            if (sb.Length > 0)
                sb.Append(", ");

            sb.Append(le.Mobile.Name);
        }

        private void AddBorderedText(int x, int y, int width, string text, int color, int borderColor)
        {
            /*AddColoredText( x - 1, y, width, text, borderColor );
            AddColoredText( x + 1, y, width, text, borderColor );
            AddColoredText( x, y - 1, width, text, borderColor );
            AddColoredText( x, y + 1, width, text, borderColor );*/
            /*AddColoredText( x - 1, y - 1, width, text, borderColor );
            AddColoredText( x + 1, y + 1, width, text, borderColor );*/
            this.AddColoredText(x, y, width, text, color);
        }

        private void AddColoredText(int x, int y, int width, string text, int color)
        {
            if (color == 0)
                this.AddHtml(x, y, width, 20, text, false, false);
            else
                this.AddHtml(x, y, width, 20, this.Color(text, color), false, false);
        }

        private void AddColumnHeader(int width, string name)
        {
            this.AddBackground(this.m_ColumnX, 12, width, 20, 0x242C);
            this.AddImageTiled(this.m_ColumnX + 2, 14, width - 4, 16, 0x2430);

            if (name != null)
                this.AddBorderedText(this.m_ColumnX, 13, width, this.Center(name), 0xFFFFFF, 0);

            this.m_ColumnX += width;
        }
    }
}