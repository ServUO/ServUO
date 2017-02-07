using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Prompts;

namespace Server.Items
{
    public class PlayerBBSouth : BasePlayerBB
    {
        [Constructable]
        public PlayerBBSouth()
            : base(0x2311)
        {
            this.Weight = 15.0;
        }

        public PlayerBBSouth(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1062421;
            }
        }// bulletin board (south)
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

    public class PlayerBBEast : BasePlayerBB
    {
        [Constructable]
        public PlayerBBEast()
            : base(0x2312)
        {
            this.Weight = 15.0;
        }

        public PlayerBBEast(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1062420;
            }
        }// bulletin board (east)
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

    public abstract class BasePlayerBB : Item, ISecurable
    {
        private PlayerBBMessage m_Greeting;
        private List<PlayerBBMessage> m_Messages;
        private string m_Title;
        private SecureLevel m_Level;
        public BasePlayerBB(int itemID)
            : base(itemID)
        {
            this.m_Messages = new List<PlayerBBMessage>();
            this.m_Level = SecureLevel.Anyone;
        }

        public BasePlayerBB(Serial serial)
            : base(serial)
        {
        }

        public List<PlayerBBMessage> Messages
        {
            get
            {
                return this.m_Messages;
            }
        }
        public PlayerBBMessage Greeting
        {
            get
            {
                return this.m_Greeting;
            }
            set
            {
                this.m_Greeting = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Title
        {
            get
            {
                return this.m_Title;
            }
            set
            {
                this.m_Title = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }
        public static bool CheckAccess(BaseHouse house, Mobile from)
        {
            if (house.Public || !house.IsAosRules)
                return !house.IsBanned(from);

            return house.HasAccess(from);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);

            writer.Write((int)this.m_Level);

            writer.Write(this.m_Title);

            if (this.m_Greeting != null)
            {
                writer.Write(true);
                this.m_Greeting.Serialize(writer);
            }
            else
            {
                writer.Write(false);
            }

            writer.WriteEncodedInt(this.m_Messages.Count);

            for (int i = 0; i < this.m_Messages.Count; ++i)
                this.m_Messages[i].Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 1)
                            this.m_Level = SecureLevel.Anyone;

                        this.m_Title = reader.ReadString();

                        if (reader.ReadBool())
                            this.m_Greeting = new PlayerBBMessage(reader);

                        int count = reader.ReadEncodedInt();

                        this.m_Messages = new List<PlayerBBMessage>(count);

                        for (int i = 0; i < count; ++i)
                            this.m_Messages.Add(new PlayerBBMessage(reader));

                        break;
                    }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house == null || !house.IsLockedDown(this))
                from.SendLocalizedMessage(1062396); // This bulletin board must be locked down in a house to be usable.
            else if (!from.InRange(this.GetWorldLocation(), 2) || !from.InLOS(this))
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else if (CheckAccess(house, from))
                from.SendGump(new PlayerBBGump(from, house, this, 0));
        }

        public class PostPrompt : Prompt
        {
            private readonly int m_Page;
            private readonly BaseHouse m_House;
            private readonly BasePlayerBB m_Board;
            private readonly bool m_Greeting;
            public PostPrompt(int page, BaseHouse house, BasePlayerBB board, bool greeting)
            {
                this.m_Page = page;
                this.m_House = house;
                this.m_Board = board;
                this.m_Greeting = greeting;
            }

            public override void OnCancel(Mobile from)
            {
                this.OnResponse(from, "");
            }

            public override void OnResponse(Mobile from, string text)
            {
                int page = this.m_Page;
                BaseHouse house = this.m_House;
                BasePlayerBB board = this.m_Board;

                if (house == null || !house.IsLockedDown(board))
                {
                    from.SendLocalizedMessage(1062396); // This bulletin board must be locked down in a house to be usable.
                    return;
                }
                else if (!from.InRange(board.GetWorldLocation(), 2) || !from.InLOS(board))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                    return;
                }
                else if (!CheckAccess(house, from))
                {
                    from.SendLocalizedMessage(1062398); // You are not allowed to post to this bulletin board.
                    return;
                }
                else if (this.m_Greeting && !house.IsOwner(from))
                {
                    return;
                }

                text = text.Trim();

                if (text.Length > 255)
                    text = text.Substring(0, 255);

                if (text.Length > 0)
                {
                    PlayerBBMessage message = new PlayerBBMessage(DateTime.UtcNow, from, text);

                    if (this.m_Greeting)
                    {
                        board.Greeting = message;
                    }
                    else
                    {
                        board.Messages.Add(message);

                        if (board.Messages.Count > 50)
                        {
                            board.Messages.RemoveAt(0);

                            if (page > 0)
                                --page;
                        }
                    }
                }

                from.SendGump(new PlayerBBGump(from, house, board, page));
            }
        }

        public class SetTitlePrompt : Prompt
        {
            public override int MessageCliloc { get { return 1062402; } }
            private readonly int m_Page;
            private readonly BaseHouse m_House;
            private readonly BasePlayerBB m_Board;
            public SetTitlePrompt(int page, BaseHouse house, BasePlayerBB board)
            {
                this.m_Page = page;
                this.m_House = house;
                this.m_Board = board;
            }

            public override void OnCancel(Mobile from)
            {
                this.OnResponse(from, "");
            }

            public override void OnResponse(Mobile from, string text)
            {
                int page = this.m_Page;
                BaseHouse house = this.m_House;
                BasePlayerBB board = this.m_Board;

                if (house == null || !house.IsLockedDown(board))
                {
                    from.SendLocalizedMessage(1062396); // This bulletin board must be locked down in a house to be usable.
                    return;
                }
                else if (!from.InRange(board.GetWorldLocation(), 2) || !from.InLOS(board))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                    return;
                }
                else if (!CheckAccess(house, from))
                {
                    from.SendLocalizedMessage(1062398); // You are not allowed to post to this bulletin board.
                    return;
                }

                text = text.Trim();

                if (text.Length > 255)
                    text = text.Substring(0, 255);

                if (text.Length > 0)
                    board.Title = text;

                from.SendGump(new PlayerBBGump(from, house, board, page));
            }
        }
    }

    public class PlayerBBMessage
    {
        private DateTime m_Time;
        private Mobile m_Poster;
        private string m_Message;
        public PlayerBBMessage(DateTime time, Mobile poster, string message)
        {
            this.m_Time = time;
            this.m_Poster = poster;
            this.m_Message = message;
        }

        public PlayerBBMessage(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Time = reader.ReadDateTime();
                        this.m_Poster = reader.ReadMobile();
                        this.m_Message = reader.ReadString();
                        break;
                    }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Time
        {
            get
            {
                return this.m_Time;
            }
            set
            {
                this.m_Time = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Poster
        {
            get
            {
                return this.m_Poster;
            }
            set
            {
                this.m_Poster = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Message
        {
            get
            {
                return this.m_Message;
            }
            set
            {
                this.m_Message = value;
            }
        }
        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write(this.m_Time);
            writer.Write(this.m_Poster);
            writer.Write(this.m_Message);
        }
    }

    public class PlayerBBGump : Gump
    {
        private const int LabelColor = 0x7FFF;
        private const int LabelHue = 1153;
        private readonly int m_Page;
        private readonly Mobile m_From;
        private readonly BaseHouse m_House;
        private readonly BasePlayerBB m_Board;
        public PlayerBBGump(Mobile from, BaseHouse house, BasePlayerBB board, int page)
            : base(50, 10)
        {
            from.CloseGump(typeof(PlayerBBGump));

            this.m_Page = page;
            this.m_From = from;
            this.m_House = house;
            this.m_Board = board;

            this.AddPage(0);

            this.AddImage(30, 30, 5400);

            this.AddButton(393, 145, 2084, 2084, 4, GumpButtonType.Reply, 0); // Scroll up
            this.AddButton(390, 371, 2085, 2085, 5, GumpButtonType.Reply, 0); // Scroll down

            this.AddButton(32, 183, 5412, 5413, 1, GumpButtonType.Reply, 0); // Post message

            if (house.IsOwner(from))
            {
                this.AddButton(63, 90, 5601, 5605, 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(81, 89, 230, 20, 1062400, LabelColor, false, false); // Set title

                this.AddButton(63, 109, 5601, 5605, 3, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(81, 108, 230, 20, 1062401, LabelColor, false, false); // Post greeting
            }

            string title = board.Title;

            if (title != null)
                this.AddHtml(183, 68, 180, 23, title, false, false);

            this.AddHtmlLocalized(385, 89, 60, 20, 1062409, LabelColor, false, false); // Post

            this.AddLabel(440, 89, LabelHue, page.ToString());
            this.AddLabel(455, 89, LabelHue, "/");
            this.AddLabel(470, 89, LabelHue, board.Messages.Count.ToString());

            PlayerBBMessage message = board.Greeting;

            if (page >= 1 && page <= board.Messages.Count)
                message = (PlayerBBMessage)board.Messages[page - 1];

            this.AddImageTiled(150, 220, 240, 1, 2700); // Separator

            this.AddHtmlLocalized(150, 180, 100, 20, 1062405, 16715, false, false); // Posted On:
            this.AddHtmlLocalized(150, 200, 100, 20, 1062406, 16715, false, false); // Posted By:

            if (message != null)
            {
                this.AddHtml(255, 180, 150, 20, message.Time.ToString("yyyy-MM-dd HH:mm:ss"), false, false);

                Mobile poster = message.Poster;
                string name = (poster == null ? null : poster.Name);

                if (name == null || (name = name.Trim()).Length == 0)
                    name = "Someone";

                this.AddHtml(255, 200, 150, 20, name, false, false);

                string body = message.Message;

                if (body == null)
                    body = "";

                this.AddHtml(150, 240, 250, 100, body, false, false);

                if (message != board.Greeting && house.IsOwner(from))
                {
                    this.AddButton(130, 395, 1209, 1210, 6, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(150, 393, 150, 20, 1062410, LabelColor, false, false); // Banish Poster

                    this.AddButton(310, 395, 1209, 1210, 7, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(330, 393, 150, 20, 1062411, LabelColor, false, false); // Delete Message
                }

                if (from.AccessLevel >= AccessLevel.GameMaster)
                    this.AddButton(135, 242, 1209, 1210, 8, GumpButtonType.Reply, 0); // Post props
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int page = this.m_Page;
            Mobile from = this.m_From;
            BaseHouse house = this.m_House;
            BasePlayerBB board = this.m_Board;

            if (house == null || !house.IsLockedDown(board))
            {
                from.SendLocalizedMessage(1062396); // This bulletin board must be locked down in a house to be usable.
                return;
            }
            else if (!from.InRange(board.GetWorldLocation(), 2) || !from.InLOS(board))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }
            else if (!BasePlayerBB.CheckAccess(house, from))
            {
                from.SendLocalizedMessage(1062398); // You are not allowed to post to this bulletin board.
                return;
            }

            switch ( info.ButtonID )
            {
                case 1: // Post message
                    {
                        from.Prompt = new BasePlayerBB.PostPrompt(page, house, board, false);
                        from.SendLocalizedMessage(1062397); // Please enter your message:

                        break;
                    }
                case 2: // Set title
                    {
                        if (house.IsOwner(from))
                        {
                            from.Prompt = new BasePlayerBB.SetTitlePrompt(page, house, board);
                            from.SendLocalizedMessage(1062402); // Enter new title:
                        }

                        break;
                    }
                case 3: // Post greeting
                    {
                        if (house.IsOwner(from))
                        {
                            from.Prompt = new BasePlayerBB.PostPrompt(page, house, board, true);
                            from.SendLocalizedMessage(1062404); // Enter new greeting (this will always be the first post):
                        }

                        break;
                    }
                case 4: // Scroll up
                    {
                        if (page == 0)
                            page = board.Messages.Count;
                        else
                            page -= 1;

                        from.SendGump(new PlayerBBGump(from, house, board, page));

                        break;
                    }
                case 5: // Scroll down
                    {
                        page += 1;
                        page %= board.Messages.Count + 1;

                        from.SendGump(new PlayerBBGump(from, house, board, page));

                        break;
                    }
                case 6: // Banish poster
                    {
                        if (house.IsOwner(from))
                        {
                            if (page >= 1 && page <= board.Messages.Count)
                            {
                                PlayerBBMessage message = (PlayerBBMessage)board.Messages[page - 1];
                                Mobile poster = message.Poster;

                                if (poster == null)
                                {
                                    from.SendGump(new PlayerBBGump(from, house, board, page));
                                    return;
                                }

                                if (poster.IsStaff() && from.AccessLevel <= poster.AccessLevel)
                                {
                                    from.SendLocalizedMessage(501354); // Uh oh...a bigger boot may be required.
                                }
                                else if (house.IsFriend(poster))
                                {
                                    from.SendLocalizedMessage(1060750); // That person is a friend, co-owner, or owner of this house, and therefore cannot be banished!
                                }
                                else if (poster is PlayerVendor)
                                {
                                    from.SendLocalizedMessage(501351); // You cannot eject a vendor.
                                }
                                else if (house.Bans.Count >= BaseHouse.MaxBans)
                                {
                                    from.SendLocalizedMessage(501355); // The ban limit for this house has been reached!
                                }
                                else if (house.IsBanned(poster))
                                {
                                    from.SendLocalizedMessage(501356); // This person is already banned!
                                }
                                else if (poster is BaseCreature && ((BaseCreature)poster).NoHouseRestrictions)
                                {
                                    from.SendLocalizedMessage(1062040); // You cannot ban that.
                                }
                                else
                                {
                                    if (!house.Bans.Contains(poster))
                                        house.Bans.Add(poster);

                                    from.SendLocalizedMessage(1062417); // That person has been banned from this house.

                                    if (house.IsInside(poster) && !BasePlayerBB.CheckAccess(house, poster))
                                        poster.MoveToWorld(house.BanLocation, house.Map);
                                }
                            }

                            from.SendGump(new PlayerBBGump(from, house, board, page));
                        }

                        break;
                    }
                case 7: // Delete message
                    {
                        if (house.IsOwner(from))
                        {
                            if (page >= 1 && page <= board.Messages.Count)
                                board.Messages.RemoveAt(page - 1);

                            from.SendGump(new PlayerBBGump(from, house, board, 0));
                        }

                        break;
                    }
                case 8: // Post props
                    {
                        if (from.AccessLevel >= AccessLevel.GameMaster)
                        {
                            PlayerBBMessage message = board.Greeting;

                            if (page >= 1 && page <= board.Messages.Count)
                                message = (PlayerBBMessage)board.Messages[page - 1];

                            from.SendGump(new PlayerBBGump(from, house, board, page));
                            from.SendGump(new PropertiesGump(from, message));
                        }

                        break;
                    }
            }
        }
    }
}