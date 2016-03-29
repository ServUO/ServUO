using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Prompts;

namespace Server.Items
{
    public class BallotBox : AddonComponent
    {
        public static readonly int MaxTopicLines = 6;
        private string[] m_Topic;
        private List<Mobile> m_Yes;
        private List<Mobile> m_No;
        [Constructable]
        public BallotBox()
            : base(0x9A8)
        {
            this.m_Topic = new string[0];
            this.m_Yes = new List<Mobile>();
            this.m_No = new List<Mobile>();
        }

        public BallotBox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041006;
            }
        }// a ballot box
        public string[] Topic
        {
            get
            {
                return this.m_Topic;
            }
        }
        public List<Mobile> Yes
        {
            get
            {
                return this.m_Yes;
            }
        }
        public List<Mobile> No
        {
            get
            {
                return this.m_No;
            }
        }
        public void ClearTopic()
        {
            this.m_Topic = new string[0];

            this.ClearVotes();
        }

        public void AddLineToTopic(string line)
        {
            if (this.m_Topic.Length >= MaxTopicLines)
                return;

            string[] newTopic = new string[this.m_Topic.Length + 1];
            this.m_Topic.CopyTo(newTopic, 0);
            newTopic[this.m_Topic.Length] = line;

            this.m_Topic = newTopic;

            this.ClearVotes();
        }

        public void ClearVotes()
        {
            this.Yes.Clear();
            this.No.Clear();
        }

        public bool IsOwner(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);
            return (house != null && house.IsOwner(from));
        }

        public bool HasVoted(Mobile from)
        {
            return (this.Yes.Contains(from) || this.No.Contains(from));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            this.SendLocalizedMessageTo(from, 500369); // I'm a ballot box, not a container!
            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else
            {
                bool isOwner = this.IsOwner(from);
                from.SendGump(new InternalGump(this, isOwner));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteEncodedInt(this.m_Topic.Length);

            for (int i = 0; i < this.m_Topic.Length; i++)
                writer.Write((string)this.m_Topic[i]);

            writer.Write(this.m_Yes, true);
            writer.Write(this.m_No, true);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Topic = new string[reader.ReadEncodedInt()];

            for (int i = 0; i < this.m_Topic.Length; i++)
                this.m_Topic[i] = reader.ReadString();

            this.m_Yes = reader.ReadStrongMobileList();
            this.m_No = reader.ReadStrongMobileList();
        }

        private class InternalGump : Gump
        {
            private readonly BallotBox m_Box;
            public InternalGump(BallotBox box, bool isOwner)
                : base(110, 70)
            {
                this.m_Box = box;

                this.AddBackground(0, 0, 400, 350, 0xA28);

                if (isOwner)
                    this.AddHtmlLocalized(0, 15, 400, 35, 1011000, false, false); // <center>Ballot Box Owner's Menu</center>
                else
                    this.AddHtmlLocalized(0, 15, 400, 35, 1011001, false, false); // <center>Ballot Box -- Vote Here!</center>

                this.AddHtmlLocalized(0, 50, 400, 35, 1011002, false, false); // <center>Topic</center>

                int lineCount = box.Topic.Length;
                this.AddBackground(25, 90, 350, Math.Max(20 * lineCount, 20), 0x1400);

                for (int i = 0; i < lineCount; i++)
                {
                    string line = box.Topic[i];

                    if (!String.IsNullOrEmpty(line))
                        this.AddLabelCropped(30, 90 + i * 20, 340, 20, 0x3E3, line);
                }

                int yesCount = box.Yes.Count;
                int noCount = box.No.Count;
                int totalVotes = yesCount + noCount;

                this.AddHtmlLocalized(0, 215, 400, 35, 1011003, false, false); // <center>votes</center>

                if (!isOwner)
                    this.AddButton(20, 240, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(55, 242, 25, 35, 1011004, false, false); // aye:
                this.AddLabel(78, 242, 0x0, String.Format("[{0}]", yesCount));

                if (!isOwner)
                    this.AddButton(20, 275, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(55, 277, 25, 35, 1011005, false, false); // nay:
                this.AddLabel(78, 277, 0x0, String.Format("[{0}]", noCount));

                if (totalVotes > 0)
                {
                    this.AddImageTiled(130, 242, (yesCount * 225) / totalVotes, 10, 0xD6);
                    this.AddImageTiled(130, 277, (noCount * 225) / totalVotes, 10, 0xD6);
                }

                this.AddButton(45, 305, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(80, 308, 40, 35, 1011008, false, false); // done

                if (isOwner)
                {
                    this.AddButton(120, 305, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(155, 308, 100, 35, 1011006, false, false); // change topic

                    this.AddButton(240, 305, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(275, 308, 300, 100, 1011007, false, false); // reset votes
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Box.Deleted || info.ButtonID == 0)
                    return;

                Mobile from = sender.Mobile;

                if (from.Map != this.m_Box.Map || !from.InRange(this.m_Box.GetWorldLocation(), 2))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                    return;
                }

                bool isOwner = this.m_Box.IsOwner(from);

                switch ( info.ButtonID )
                {
                    case 1: // change topic
                        {
                            if (isOwner)
                            {
                                this.m_Box.ClearTopic();

                                from.SendLocalizedMessage(500370, "", 0x35); // Enter a line of text for your ballot, and hit ENTER. Hit ESC after the last line is entered.
                                from.Prompt = new TopicPrompt(this.m_Box);
                            }

                            break;
                        }
                    case 2: // reset votes
                        {
                            if (isOwner)
                            {
                                this.m_Box.ClearVotes();
                                from.SendLocalizedMessage(500371); // Votes zeroed out.
                            }

                            goto default;
                        }
                    case 3: // aye
                        {
                            if (!isOwner)
                            {
                                if (this.m_Box.HasVoted(from))
                                {
                                    from.SendLocalizedMessage(500374); // You have already voted on this ballot.
                                }
                                else
                                {
                                    this.m_Box.Yes.Add(from);
                                    from.SendLocalizedMessage(500373); // Your vote has been registered.
                                }
                            }

                            goto default;
                        }
                    case 4: // nay
                        {
                            if (!isOwner)
                            {
                                if (this.m_Box.HasVoted(from))
                                {
                                    from.SendLocalizedMessage(500374); // You have already voted on this ballot.
                                }
                                else
                                {
                                    this.m_Box.No.Add(from);
                                    from.SendLocalizedMessage(500373); // Your vote has been registered.
                                }
                            }

                            goto default;
                        }
                    default:
                        {
                            from.SendGump(new InternalGump(this.m_Box, isOwner));
                            break;
                        }
                }
            }
        }

        private class TopicPrompt : Prompt
        {
            private readonly BallotBox m_Box;
            public TopicPrompt(BallotBox box)
            {
                this.m_Box = box;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (this.m_Box.Deleted || !this.m_Box.IsOwner(from))
                    return;

                if (from.Map != this.m_Box.Map || !from.InRange(this.m_Box.GetWorldLocation(), 2))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                    return;
                }

                this.m_Box.AddLineToTopic(text.TrimEnd());

                if (this.m_Box.Topic.Length < MaxTopicLines)
                {
                    from.SendLocalizedMessage(500377, "", 0x35); // Next line or ESC to finish:
                    from.Prompt = new TopicPrompt(this.m_Box);
                }
                else
                {
                    from.SendLocalizedMessage(500376, "", 0x35); // Ballot entry complete.
                    from.SendGump(new InternalGump(this.m_Box, true));
                }
            }

            public override void OnCancel(Mobile from)
            {
                if (this.m_Box.Deleted || !this.m_Box.IsOwner(from))
                    return;

                if (from.Map != this.m_Box.Map || !from.InRange(this.m_Box.GetWorldLocation(), 2))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                    return;
                }

                from.SendLocalizedMessage(500376, "", 0x35); // Ballot entry complete.
                from.SendGump(new InternalGump(this.m_Box, true));
            }
        }
    }

    public class BallotBoxAddon : BaseAddon
    {
        public BallotBoxAddon()
        {
            this.AddComponent(new BallotBox(), 0, 0, 0);
        }

        public BallotBoxAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new BallotBoxDeed();
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
    }

    public class BallotBoxDeed : BaseAddonDeed
    {
        [Constructable]
        public BallotBoxDeed()
        {
        }

        public BallotBoxDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new BallotBoxAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044327;
            }
        }// ballot box
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
    }
}