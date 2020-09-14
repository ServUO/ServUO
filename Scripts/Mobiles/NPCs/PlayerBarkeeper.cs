#region References
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Prompts;
using System;
using System.Collections.Generic;
#endregion

namespace Server.Mobiles
{
    public class ChangeRumorMessagePrompt : Prompt
    {
        private readonly PlayerBarkeeper m_Barkeeper;
        private readonly int m_RumorIndex;

        public ChangeRumorMessagePrompt(PlayerBarkeeper barkeeper, int rumorIndex)
        {
            m_Barkeeper = barkeeper;
            m_RumorIndex = rumorIndex;
        }

        public override void OnCancel(Mobile from)
        {
            OnResponse(from, "");
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (text.Length > 130)
            {
                text = text.Substring(0, 130);
            }

            m_Barkeeper.EndChangeRumor(from, m_RumorIndex, text);
        }
    }

    public class ChangeRumorKeywordPrompt : Prompt
    {
        private readonly PlayerBarkeeper m_Barkeeper;
        private readonly int m_RumorIndex;

        public ChangeRumorKeywordPrompt(PlayerBarkeeper barkeeper, int rumorIndex)
        {
            m_Barkeeper = barkeeper;
            m_RumorIndex = rumorIndex;
        }

        public override void OnCancel(Mobile from)
        {
            OnResponse(from, "");
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (text.Length > 130)
            {
                text = text.Substring(0, 130);
            }

            m_Barkeeper.EndChangeKeyword(from, m_RumorIndex, text);
        }
    }

    public class ChangeTipMessagePrompt : Prompt
    {
        private readonly PlayerBarkeeper m_Barkeeper;

        public ChangeTipMessagePrompt(PlayerBarkeeper barkeeper)
        {
            m_Barkeeper = barkeeper;
        }

        public override void OnCancel(Mobile from)
        {
            OnResponse(from, "");
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (text.Length > 130)
            {
                text = text.Substring(0, 130);
            }

            m_Barkeeper.EndChangeTip(from, text);
        }
    }

    public class BarkeeperRumor
    {
        private string m_Message;
        private string m_Keyword;

        public BarkeeperRumor(string message, string keyword)
        {
            m_Message = message;
            m_Keyword = keyword;
        }

        public string Message { get { return m_Message; } set { m_Message = value; } }
        public string Keyword { get { return m_Keyword; } set { m_Keyword = value; } }

        public static BarkeeperRumor Deserialize(GenericReader reader)
        {
            if (!reader.ReadBool())
            {
                return null;
            }

            return new BarkeeperRumor(reader.ReadString(), reader.ReadString());
        }

        public static void Serialize(GenericWriter writer, BarkeeperRumor rumor)
        {
            if (rumor == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(rumor.m_Message);
                writer.Write(rumor.m_Keyword);
            }
        }
    }

    public class ManageBarkeeperEntry : ContextMenuEntry
    {
        private readonly Mobile m_From;
        private readonly PlayerBarkeeper m_Barkeeper;

        public ManageBarkeeperEntry(Mobile from, PlayerBarkeeper barkeeper)
            : base(6151, 12)
        {
            m_From = from;
            m_Barkeeper = barkeeper;
        }

        public override void OnClick()
        {
            m_Barkeeper.BeginManagement(m_From);
        }
    }

    public class PlayerBarkeeper : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        private Mobile m_Owner;
        private BaseHouse m_House;
        private string m_TipMessage;
        private BarkeeperRumor[] m_Rumors;
        private Timer m_NewsTimer;

        public PlayerBarkeeper(Mobile owner, BaseHouse house)
            : base("the barkeeper")
        {
            m_Owner = owner;
            House = house;
            m_Rumors = new BarkeeperRumor[3];

            LoadSBInfo();
        }

        public PlayerBarkeeper(Serial serial)
            : base(serial)
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }

        public BaseHouse House
        {
            get { return m_House; }
            set
            {
                if (m_House != null)
                {
                    m_House.PlayerBarkeepers.Remove(this);
                }

                if (value != null)
                {
                    value.PlayerBarkeepers.Add(this);
                }

                m_House = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string TipMessage { get { return m_TipMessage; } set { m_TipMessage = value; } }

        public override bool IsActiveBuyer => false;
        public override bool IsActiveSeller => (m_SBInfos.Count > 0);
        public override bool DisallowAllMoves => true;
        public override bool NoHouseRestrictions => true;
        public BarkeeperRumor[] Rumors => m_Rumors;
        public override VendorShoeType ShoeType => Utility.RandomBool() ? VendorShoeType.ThighBoots : VendorShoeType.Boots;
        protected override List<SBInfo> SBInfos => m_SBInfos;

        public override bool GetGender()
        {
            return false; // always starts as male
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(new HalfApron(Utility.RandomBrightHue()));

            Container pack = Backpack;

            if (pack != null)
            {
                pack.Delete();
            }
        }

        public override void InitBody()
        {
            base.InitBody();

            if (BodyValue == 0x340 || BodyValue == 0x402)
            {
                Hue = 0;
            }
            else
            {
                Hue = 0x83F4; // hue is not random
            }

            Container pack = Backpack;

            if (pack != null)
            {
                pack.Delete();
            }
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (InRange(from, 3))
            {
                return true;
            }

            return base.HandlesOnSpeech(from);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            House = null;
        }

        public override bool OnBeforeDeath()
        {
            if (!base.OnBeforeDeath())
            {
                return false;
            }

            Item shoes = FindItemOnLayer(Layer.Shoes);

            if (shoes is Sandals)
            {
                shoes.Hue = 0;
            }

            return true;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            if (!e.Handled && InRange(e.Mobile, 3))
            {
                if (m_NewsTimer == null && e.HasKeyword(0x30)) // *news*
                {
                    TownCrierEntry tce = GlobalTownCrierEntryList.Instance.GetRandomEntry();

                    if (tce == null)
                    {
                        PublicOverheadMessage(MessageType.Regular, 0x3B2, 1005643); // I have no news at this time.
                    }
                    else
                    {
                        m_NewsTimer = Timer.DelayCall(
                            TimeSpan.FromSeconds(1.0),
                            TimeSpan.FromSeconds(3.0),
                            new TimerStateCallback(ShoutNews_Callback),
                            new object[] { tce, 0 });

                        PublicOverheadMessage(MessageType.Regular, 0x3B2, 502978); // Some of the latest news!
                    }
                }

                for (int i = 0; i < m_Rumors.Length; ++i)
                {
                    BarkeeperRumor rumor = m_Rumors[i];

                    if (rumor == null)
                    {
                        continue;
                    }

                    string keyword = rumor.Keyword;

                    if (keyword == null || (keyword = keyword.Trim()).Length == 0)
                    {
                        continue;
                    }

                    if (Insensitive.Equals(keyword, e.Speech))
                    {
                        string message = rumor.Message;

                        if (message == null || (message = message.Trim()).Length == 0)
                        {
                            continue;
                        }

                        PublicOverheadMessage(MessageType.Regular, 0x3B2, false, message);
                    }
                }
            }
        }

        public override bool CheckGold(Mobile from, Item dropped)
        {
            if (dropped is Gold)
            {
                Gold g = (Gold)dropped;

                if (g.Amount > 50)
                {
                    PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "I cannot accept so large a tip!", from.NetState);
                }
                else
                {
                    string tip = m_TipMessage;

                    if (tip == null || (tip = tip.Trim()).Length == 0)
                    {
                        PrivateOverheadMessage(
                            MessageType.Regular,
                            0x3B2,
                            false,
                            "It would not be fair of me to take your money and not offer you information in return.",
                            from.NetState);
                    }
                    else
                    {
                        Direction = GetDirectionTo(from);
                        PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, tip, from.NetState);

                        g.Delete();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsOwner(Mobile from)
        {
            if (from == null || from.Deleted || Deleted)
            {
                return false;
            }

            if (from.AccessLevel > AccessLevel.GameMaster)
            {
                return true;
            }

            return (m_Owner == from);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (IsOwner(from) && from.InLOS(this))
            {
                list.Add(new ManageBarkeeperEntry(from, this));
            }
        }

        public void BeginManagement(Mobile from)
        {
            if (!IsOwner(from))
            {
                return;
            }

            from.SendGump(new BarkeeperGump(from, this));
        }

        public void Dismiss()
        {
            Delete();
        }

        public void BeginChangeRumor(Mobile from, int index)
        {
            if (index < 0 || index >= m_Rumors.Length)
            {
                return;
            }

            from.Prompt = new ChangeRumorMessagePrompt(this, index);
            PrivateOverheadMessage(
                MessageType.Regular, 0x3B2, false, "Say what news you would like me to tell our guests.", from.NetState);
        }

        public void EndChangeRumor(Mobile from, int index, string text)
        {
            if (index < 0 || index >= m_Rumors.Length)
            {
                return;
            }

            if (m_Rumors[index] == null)
            {
                m_Rumors[index] = new BarkeeperRumor(text, null);
            }
            else
            {
                m_Rumors[index].Message = text;
            }

            from.Prompt = new ChangeRumorKeywordPrompt(this, index);
            PrivateOverheadMessage(
                MessageType.Regular, 0x3B2, false, "What keyword should a guest say to me to get this news?", from.NetState);
        }

        public void EndChangeKeyword(Mobile from, int index, string text)
        {
            if (index < 0 || index >= m_Rumors.Length)
            {
                return;
            }

            if (m_Rumors[index] == null)
            {
                m_Rumors[index] = new BarkeeperRumor(null, text);
            }
            else
            {
                m_Rumors[index].Keyword = text;
            }

            PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "I'll pass on the message.", from.NetState);
        }

        public void RemoveRumor(Mobile from, int index)
        {
            if (index < 0 || index >= m_Rumors.Length)
            {
                return;
            }

            m_Rumors[index] = null;
        }

        public void BeginChangeTip(Mobile from)
        {
            from.Prompt = new ChangeTipMessagePrompt(this);
            PrivateOverheadMessage(
                MessageType.Regular,
                0x3B2,
                false,
                "Say what you want me to tell guests when they give me a good tip.",
                from.NetState);
        }

        public void EndChangeTip(Mobile from, string text)
        {
            m_TipMessage = text;
            PrivateOverheadMessage(
                MessageType.Regular, 0x3B2, false, "I'll say that to anyone who gives me a good tip.", from.NetState);
        }

        public void RemoveTip(Mobile from)
        {
            m_TipMessage = null;
        }

        public void BeginChangeTitle(Mobile from)
        {
            from.SendGump(new BarkeeperTitleGump(from, this));
        }

        public void EndChangeTitle(Mobile from, string title, bool vendor)
        {
            Title = title;

            LoadSBInfo();
        }

        public void CancelChangeTitle(Mobile from)
        {
            from.SendGump(new BarkeeperGump(from, this));
        }

        public void BeginChangeAppearance(Mobile from)
        {
            from.CloseGump(typeof(PlayerVendorCustomizeGump));
            from.SendGump(new PlayerVendorCustomizeGump(this, from));
        }

        public void ChangeGender(Mobile from)
        {
            Female = !Female;

            if (Female)
            {
                Body = 401;
                Name = NameList.RandomName("female");

                FacialHairItemID = 0;
            }
            else
            {
                Body = 400;
                Name = NameList.RandomName("male");
            }
        }

        public override void InitSBInfo()
        {
            if (Title == "the waiter" || Title == "the barkeeper" || Title == "the baker" || Title == "the innkeeper" ||
                Title == "the chef")
            {
                if (m_SBInfos.Count == 0)
                {
                    m_SBInfos.Add(new SBPlayerBarkeeper());
                }
            }
            else
            {
                m_SBInfos.Clear();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version;

            writer.Write(m_House);

            writer.Write(m_Owner);

            writer.WriteEncodedInt(m_Rumors.Length);

            for (int i = 0; i < m_Rumors.Length; ++i)
            {
                BarkeeperRumor.Serialize(writer, m_Rumors[i]);
            }

            writer.Write(m_TipMessage);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        House = (BaseHouse)reader.ReadItem();

                        goto case 0;
                    }
                case 0:
                    {
                        m_Owner = reader.ReadMobile();

                        m_Rumors = new BarkeeperRumor[reader.ReadEncodedInt()];

                        for (int i = 0; i < m_Rumors.Length; ++i)
                        {
                            m_Rumors[i] = BarkeeperRumor.Deserialize(reader);
                        }

                        m_TipMessage = reader.ReadString();

                        break;
                    }
            }

            if (version < 1)
            {
                Timer.DelayCall(TimeSpan.Zero, UpgradeFromVersion0);
            }
        }

        private void ShoutNews_Callback(object state)
        {
            object[] states = (object[])state;
            TownCrierEntry tce = (TownCrierEntry)states[0];
            int index = (int)states[1];

            if (index < 0 || index >= tce.Lines.Length)
            {
                if (m_NewsTimer != null)
                {
                    m_NewsTimer.Stop();
                }

                m_NewsTimer = null;
            }
            else
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, false, tce.Lines[index]);
                states[1] = index + 1;
            }
        }

        private void UpgradeFromVersion0()
        {
            House = BaseHouse.FindHouseAt(this);
        }
    }

    public class BarkeeperTitleGump : Gump
    {
        private static readonly Entry[] m_Entries = new[]
        {
            new Entry("Alchemist"), new Entry("Animal Tamer"), new Entry("Apothecary"), new Entry("Artist"),
            new Entry("Baker", true), new Entry("Bard"), new Entry("Barkeep", "the barkeeper", true), new Entry("Beggar"),
            new Entry("Blacksmith"), new Entry("Bounty Hunter"), new Entry("Brigand"), new Entry("Butler"),
            new Entry("Carpenter"), new Entry("Chef", true), new Entry("Commander"), new Entry("Curator"), new Entry("Drunkard"),
            new Entry("Farmer"), new Entry("Fisherman"), new Entry("Gambler"), new Entry("Gypsy"), new Entry("Herald"),
            new Entry("Herbalist"), new Entry("Hermit"), new Entry("Innkeeper", true), new Entry("Jailor"), new Entry("Jester"),
            new Entry("Librarian"), new Entry("Mage"), new Entry("Mercenary"), new Entry("Merchant"), new Entry("Messenger"),
            new Entry("Miner"), new Entry("Monk"), new Entry("Noble"), new Entry("Paladin"), new Entry("Peasant"),
            new Entry("Pirate"), new Entry("Prisoner"), new Entry("Prophet"), new Entry("Ranger"), new Entry("Sage"),
            new Entry("Sailor"), new Entry("Scholar"), new Entry("Scribe"), new Entry("Sentry"), new Entry("Servant"),
            new Entry("Shepherd"), new Entry("Soothsayer"), new Entry("Stoic"), new Entry("Storyteller"), new Entry("Tailor"),
            new Entry("Thief"), new Entry("Tinker"), new Entry("Town Crier"), new Entry("Treasure Hunter"),
            new Entry("Waiter", true), new Entry("Warrior"), new Entry("Watchman"), new Entry("No Title", null, false)
        };

        private readonly Mobile m_From;
        private readonly PlayerBarkeeper m_Barkeeper;

        public BarkeeperTitleGump(Mobile from, PlayerBarkeeper barkeeper)
            : base(0, 0)
        {
            m_From = from;
            m_Barkeeper = barkeeper;

            from.CloseGump(typeof(BarkeeperGump));
            from.CloseGump(typeof(BarkeeperTitleGump));

            Entry[] entries = m_Entries;

            RenderBackground();

            int pageCount = (entries.Length + 19) / 20;

            for (int i = 0; i < pageCount; ++i)
            {
                RenderPage(entries, i);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int buttonID = info.ButtonID;

            if (buttonID > 0)
            {
                --buttonID;

                if (buttonID > 0)
                {
                    --buttonID;

                    if (buttonID >= 0 && buttonID < m_Entries.Length)
                    {
                        m_Barkeeper.EndChangeTitle(m_From, m_Entries[buttonID].m_Title, m_Entries[buttonID].m_Vendor);
                    }
                }
                else
                {
                    m_Barkeeper.CancelChangeTitle(m_From);
                }
            }
        }

        private void RenderBackground()
        {
            AddPage(0);

            AddBackground(30, 40, 585, 410, 5054);

            AddImage(30, 40, 9251);
            AddImage(180, 40, 9251);
            AddImage(30, 40, 9253);
            AddImage(30, 130, 9253);
            AddImage(598, 40, 9255);
            AddImage(598, 130, 9255);
            AddImage(30, 433, 9257);
            AddImage(180, 433, 9257);
            AddImage(30, 40, 9250);
            AddImage(598, 40, 9252);
            AddImage(598, 433, 9258);
            AddImage(30, 433, 9256);

            AddItem(30, 40, 6816);
            AddItem(30, 125, 6817);
            AddItem(30, 233, 6817);
            AddItem(30, 341, 6817);
            AddItem(580, 40, 6814);
            AddItem(588, 125, 6815);
            AddItem(588, 233, 6815);
            AddItem(588, 341, 6815);

            AddImage(560, 20, 1417);
            AddItem(580, 44, 4033);

            AddBackground(183, 25, 280, 30, 5054);

            AddImage(180, 25, 10460);
            AddImage(434, 25, 10460);

            AddHtml(223, 32, 200, 40, "BARKEEP CUSTOMIZATION MENU", false, false);
            AddBackground(243, 433, 150, 30, 5054);

            AddImage(240, 433, 10460);
            AddImage(375, 433, 10460);

            AddImage(80, 398, 2151);
            AddItem(72, 406, 2543);

            AddHtml(110, 412, 180, 25, "sells food and drink", false, false);
        }

        private void RenderPage(Entry[] entries, int page)
        {
            AddPage(1 + page);

            AddHtml(430, 70, 180, 25, string.Format("Page {0} of {1}", page + 1, (entries.Length + 19) / 20), false, false);

            for (int count = 0, i = (page * 20); count < 20 && i < entries.Length; ++count, ++i)
            {
                Entry entry = entries[i];

                AddButton(80 + ((count / 10) * 260), 100 + ((count % 10) * 30), 4005, 4007, 2 + i, GumpButtonType.Reply, 0);
                AddHtml(
                    120 + ((count / 10) * 260),
                    100 + ((count % 10) * 30),
                    entry.m_Vendor ? 148 : 180,
                    25,
                    entry.m_Description,
                    true,
                    false);

                if (entry.m_Vendor)
                {
                    AddImage(270 + ((count / 10) * 260), 98 + ((count % 10) * 30), 2151);
                    AddItem(262 + ((count / 10) * 260), 106 + ((count % 10) * 30), 2543);
                }
            }

            AddButton(340, 400, 4005, 4007, 0, GumpButtonType.Page, 1 + ((page + 1) % ((entries.Length + 19) / 20)));
            AddHtml(380, 400, 180, 25, "More Job Titles", false, false);

            AddButton(338, 437, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddHtml(290, 440, 35, 40, "Back", false, false);
        }

        private class Entry
        {
            public readonly string m_Description;
            public readonly string m_Title;
            public readonly bool m_Vendor;

            public Entry(string desc)
                : this(desc, string.Format("the {0}", desc.ToLower()), false)
            { }

            public Entry(string desc, bool vendor)
                : this(desc, string.Format("the {0}", desc.ToLower()), vendor)
            { }

            public Entry(string desc, string title, bool vendor)
            {
                m_Description = desc;
                m_Title = title;
                m_Vendor = vendor;
            }
        }
    }

    public class BarkeeperGump : Gump
    {
        private readonly Mobile m_From;
        private readonly PlayerBarkeeper m_Barkeeper;

        public BarkeeperGump(Mobile from, PlayerBarkeeper barkeeper)
            : base(0, 0)
        {
            m_From = from;
            m_Barkeeper = barkeeper;

            from.CloseGump(typeof(BarkeeperGump));
            from.CloseGump(typeof(BarkeeperTitleGump));

            RenderBackground();
            RenderCategories();
            RenderMessageManagement();
            RenderDismissConfirmation();
            RenderMessageManagement_Message_AddOrChange();
            RenderMessageManagement_Message_Remove();
            RenderMessageManagement_Tip_AddOrChange();
            RenderMessageManagement_Tip_Remove();
            RenderAppearanceCategories();
        }

        public void RenderBackground()
        {
            AddPage(0);

            AddBackground(30, 40, 585, 410, 5054);

            AddImage(30, 40, 9251);
            AddImage(180, 40, 9251);
            AddImage(30, 40, 9253);
            AddImage(30, 130, 9253);
            AddImage(598, 40, 9255);
            AddImage(598, 130, 9255);
            AddImage(30, 433, 9257);
            AddImage(180, 433, 9257);
            AddImage(30, 40, 9250);
            AddImage(598, 40, 9252);
            AddImage(598, 433, 9258);
            AddImage(30, 433, 9256);

            AddItem(30, 40, 6816);
            AddItem(30, 125, 6817);
            AddItem(30, 233, 6817);
            AddItem(30, 341, 6817);
            AddItem(580, 40, 6814);
            AddItem(588, 125, 6815);
            AddItem(588, 233, 6815);
            AddItem(588, 341, 6815);

            AddBackground(183, 25, 280, 30, 5054);

            AddImage(180, 25, 10460);
            AddImage(434, 25, 10460);
            AddImage(560, 20, 1417);

            AddHtml(223, 32, 200, 40, "BARKEEP CUSTOMIZATION MENU", false, false);
            AddBackground(243, 433, 150, 30, 5054);

            AddImage(240, 433, 10460);
            AddImage(375, 433, 10460);
        }

        public void RenderCategories()
        {
            AddPage(1);

            AddButton(130, 120, 4005, 4007, 0, GumpButtonType.Page, 2);
            AddHtml(170, 120, 200, 40, "Message Control", false, false);

            AddButton(130, 200, 4005, 4007, 0, GumpButtonType.Page, 8);
            AddHtml(170, 200, 200, 40, "Customize your barkeep", false, false);

            AddButton(130, 280, 4005, 4007, 0, GumpButtonType.Page, 3);
            AddHtml(170, 280, 200, 40, "Dismiss your barkeep", false, false);

            AddButton(338, 437, 4014, 4016, 0, GumpButtonType.Reply, 0);
            AddHtml(290, 440, 35, 40, "Back", false, false);

            AddItem(574, 43, 5360);
        }

        public void RenderMessageManagement()
        {
            AddPage(2);

            AddButton(130, 120, 4005, 4007, 0, GumpButtonType.Page, 4);
            AddHtml(170, 120, 380, 20, "Add or change a message and keyword", false, false);

            AddButton(130, 200, 4005, 4007, 0, GumpButtonType.Page, 5);
            AddHtml(170, 200, 380, 20, "Remove a message and keyword from your barkeep", false, false);

            AddButton(130, 280, 4005, 4007, 0, GumpButtonType.Page, 6);
            AddHtml(170, 280, 380, 20, "Add or change your barkeeper's tip message", false, false);

            AddButton(130, 360, 4005, 4007, 0, GumpButtonType.Page, 7);
            AddHtml(170, 360, 380, 20, "Delete your barkeepers tip message", false, false);

            AddButton(338, 437, 4014, 4016, 0, GumpButtonType.Page, 1);
            AddHtml(290, 440, 35, 40, "Back", false, false);

            AddItem(580, 46, 4030);
        }

        public void RenderDismissConfirmation()
        {
            AddPage(3);

            AddHtml(170, 160, 380, 20, "Are you sure you want to dismiss your barkeeper?", false, false);

            AddButton(205, 280, 4005, 4007, GetButtonID(0, 0), GumpButtonType.Reply, 0);
            AddHtml(240, 280, 100, 20, @"Yes", false, false);

            AddButton(395, 280, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtml(430, 280, 100, 20, "No", false, false);

            AddButton(338, 437, 4014, 4016, 0, GumpButtonType.Page, 1);
            AddHtml(290, 440, 35, 40, "Back", false, false);

            AddItem(574, 43, 5360);
            AddItem(584, 34, 6579);
        }

        public void RenderMessageManagement_Message_AddOrChange()
        {
            AddPage(4);

            AddHtml(250, 60, 500, 25, "Add or change a message", false, false);

            BarkeeperRumor[] rumors = m_Barkeeper.Rumors;

            for (int i = 0; i < rumors.Length; ++i)
            {
                BarkeeperRumor rumor = rumors[i];

                AddHtml(100, 70 + (i * 120), 50, 20, "Message", false, false);
                AddHtml(100, 90 + (i * 120), 450, 40, rumor == null ? "No current message" : rumor.Message, true, false);
                AddHtml(100, 130 + (i * 120), 50, 20, "Keyword", false, false);
                AddHtml(100, 150 + (i * 120), 450, 40, rumor == null ? "None" : rumor.Keyword, true, false);

                AddButton(60, 90 + (i * 120), 4005, 4007, GetButtonID(1, i), GumpButtonType.Reply, 0);
            }

            AddButton(338, 437, 4014, 4016, 0, GumpButtonType.Page, 2);
            AddHtml(290, 440, 35, 40, "Back", false, false);

            AddItem(580, 46, 4030);
        }

        public void RenderMessageManagement_Message_Remove()
        {
            AddPage(5);

            AddHtml(190, 60, 500, 25, "Choose the message you would like to remove", false, false);

            BarkeeperRumor[] rumors = m_Barkeeper.Rumors;

            for (int i = 0; i < rumors.Length; ++i)
            {
                BarkeeperRumor rumor = rumors[i];

                AddHtml(100, 70 + (i * 120), 50, 20, "Message", false, false);
                AddHtml(100, 90 + (i * 120), 450, 40, rumor == null ? "No current message" : rumor.Message, true, false);
                AddHtml(100, 130 + (i * 120), 50, 20, "Keyword", false, false);
                AddHtml(100, 150 + (i * 120), 450, 40, rumor == null ? "None" : rumor.Keyword, true, false);

                AddButton(60, 90 + (i * 120), 4005, 4007, GetButtonID(2, i), GumpButtonType.Reply, 0);
            }

            AddButton(338, 437, 4014, 4016, 0, GumpButtonType.Page, 2);
            AddHtml(290, 440, 35, 40, "Back", false, false);

            AddItem(580, 46, 4030);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (!m_Barkeeper.IsOwner(m_From))
            {
                return;
            }

            int index = info.ButtonID - 1;

            if (index < 0)
            {
                return;
            }

            int type = index % 6;
            index /= 6;

            switch (type)
            {
                case 0: // Controls
                    {
                        switch (index)
                        {
                            case 0: // Dismiss
                                {
                                    m_Barkeeper.Dismiss();
                                    break;
                                }
                        }

                        break;
                    }
                case 1: // Change message
                    {
                        m_Barkeeper.BeginChangeRumor(m_From, index);
                        break;
                    }
                case 2: // Remove message
                    {
                        m_Barkeeper.RemoveRumor(m_From, index);
                        break;
                    }
                case 3: // Change tip
                    {
                        m_Barkeeper.BeginChangeTip(m_From);
                        break;
                    }
                case 4: // Remove tip
                    {
                        m_Barkeeper.RemoveTip(m_From);
                        break;
                    }
                case 5: // Appearance category selection
                    {
                        switch (index)
                        {
                            case 0:
                                m_Barkeeper.BeginChangeTitle(m_From);
                                break;
                            case 1:
                                m_Barkeeper.BeginChangeAppearance(m_From);
                                break;
                            case 2:
                                m_Barkeeper.ChangeGender(m_From);
                                break;
                        }

                        break;
                    }
            }
        }

        private int GetButtonID(int type, int index)
        {
            return 1 + (index * 6) + type;
        }

        private void RenderMessageManagement_Tip_AddOrChange()
        {
            AddPage(6);

            AddHtml(250, 95, 500, 20, "Change this tip message", false, false);
            AddHtml(100, 190, 50, 20, "Message", false, false);
            AddHtml(
                100, 210, 450, 40, m_Barkeeper.TipMessage == null ? "No current message" : m_Barkeeper.TipMessage, true, false);

            AddButton(60, 210, 4005, 4007, GetButtonID(3, 0), GumpButtonType.Reply, 0);

            AddButton(338, 437, 4014, 4016, 0, GumpButtonType.Page, 2);
            AddHtml(290, 440, 35, 40, "Back", false, false);

            AddItem(580, 46, 4030);
        }

        private void RenderMessageManagement_Tip_Remove()
        {
            AddPage(7);

            AddHtml(250, 95, 500, 20, "Remove this tip message", false, false);
            AddHtml(100, 190, 50, 20, "Message", false, false);
            AddHtml(
                100, 210, 450, 40, m_Barkeeper.TipMessage == null ? "No current message" : m_Barkeeper.TipMessage, true, false);

            AddButton(60, 210, 4005, 4007, GetButtonID(4, 0), GumpButtonType.Reply, 0);

            AddButton(338, 437, 4014, 4016, 0, GumpButtonType.Page, 2);
            AddHtml(290, 440, 35, 40, "Back", false, false);

            AddItem(580, 46, 4030);
        }

        private void RenderAppearanceCategories()
        {
            AddPage(8);

            AddButton(130, 120, 4005, 4007, GetButtonID(5, 0), GumpButtonType.Reply, 0);
            AddHtml(170, 120, 120, 20, "Title", false, false);

            if (m_Barkeeper.BodyValue != 0x340 && m_Barkeeper.BodyValue != 0x402)
            {
                AddButton(130, 200, 4005, 4007, GetButtonID(5, 1), GumpButtonType.Reply, 0);
                AddHtml(170, 200, 120, 20, "Appearance", false, false);

                AddButton(130, 280, 4005, 4007, GetButtonID(5, 2), GumpButtonType.Reply, 0);
                AddHtml(170, 280, 120, 20, "Male / Female", false, false);

                AddButton(338, 437, 4014, 4016, 0, GumpButtonType.Page, 1);
                AddHtml(290, 440, 35, 40, "Back", false, false);
            }

            AddItem(580, 44, 4033);
        }
    }
}