using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Mobiles
{
    public class AttendantHerald : PersonalAttendant
    { 
        private static readonly HeraldEntry[] m_Announcements = new HeraldEntry[]
        {
            new HeraldEntry(1076044, "[OWNER TITLE]", "[OWNER NAME]"), // Attention, attention! All hail the arrival of the ~1_TITLE~ ~2_NAME~!
            new HeraldEntry(1076045, "[OWNER TITLE]", "[OWNER NAME]"), // Make way ye unwashed hordes! Clear the road! For ~1_TITLE~ ~2_NAME~ has business more important than yours!
            new HeraldEntry(1076046, "[OWNER TITLE]", "[OWNER NAME]"), // ~1_TITLE~ ~2_NAME~ approaches! Be ye prepared to kneel before their indomitable presence! And remember, tribute is to be only in gold!
            new HeraldEntry(1076047, "[OWNER TITLE]", "[OWNER NAME]"), // Throw down your capes and kerchiefs! Let the petals be strewn! For the ~1_TITLE~ ~2_NAME~ approacheth!
            new HeraldEntry(1076048, "[OWNER TITLE]", "[OWNER NAME]"), // ~1_TITLE~ ~2_NAME~ has arrived! Let the drinks flow! Let the festivities commence! And you there, with the pig, get that beast on a skewer!
            new HeraldEntry(1076049, "[OWNER SEX P]", "[OWNER OPPOSITE SEX P]", "[OWNER TITLE]", "[OWNER NAME]")// Let the ~1_SAME_SEX~ cower and the ~2_OPPOSITE_SEX~ swoon, for now approaches the ~3_TITLE~ ~4_NAME~.
        };
        private static readonly HeraldEntry[] m_Greetings = new HeraldEntry[]
        {
            new HeraldEntry(1076038, "[OWNER NAME]"), // Welcome to the home of ~1_OWNER_NAME~. Please keep your shoes off the furniture.
            new HeraldEntry(1076039, "[VISITOR TITLE]", "[VISITOR NAME]", "[OWNER SEX]"), // Announcing the arrival of the ~1_VISITOR_TITLE~ ~2_VISITOR_NAME~. The ~3_OWNER_SEX~ of the house bids you welcome.
            new HeraldEntry(1076040, "[OWNER SEX]","[VISITOR TITLE]", "[VISITOR NAME]"), // My ~1_OWNER_SEX~, you have a visitor! ~2_VISITOR_TITLE~ ~3_VISITOR_NAME~ is awaiting your presence.
            new HeraldEntry(1076041, "[OWNER TITLE]", "[OWNER NAME]"), // Welcome the the humble abode of ~1_OWNER_TITLE~ ~2_OWNER_NAME~, please remove your shoes, and have a seat where you may find one.
            new HeraldEntry(1076042, "[VISITOR TITLE]", "[VISITOR NAME]"), // Ahh, greetings to ~1_VISITOR_TITLE~ ~2_VISITOR_NAME~. Your coming here was foreseen, and I alone know of your purpose.
            new HeraldEntry(1076043), // *squints* Not you again! Fine, fine... whatever... come on in, I suppose. *sighs*
            new HeraldEntry(1076074, "[OWNER TITLE]", "[OWNER NAME]"), // Welcome to this humble marketplace. If you need any assistance ~1_OWNER_TITLE~ ~2_OWNER_NAME~ will be glad to help you.
            new HeraldEntry(1076075, "[OWNER TITLE]", "[OWNER NAME]"), // Come Friend! Enter ~1_OWNER_TITLE~ ~2_OWNER_NAME~'s wondrous shop of many things! If you can't find it here, I suggest you go somewhere else!
            new HeraldEntry(1076076, "[VISITOR NAME]")// *Looks ~1_VISITOR_NAME~ over with narrowed eyes, scowling, and points to the sign on the wall* "Reagents for spell casting only, Please do not eat!"
        };
        private HeraldEntry m_Announcement;
        private HeraldEntry m_Greeting;
        private DateTime m_NextYell;
        private BaseHouse m_House;
        private Point3D m_Location;
        public AttendantHerald()
            : base("the Herald")
        {
            this.m_Announcement = m_Announcements[0];
            this.m_Greeting = m_Greetings[0];

            this.m_NextYell = DateTime.UtcNow;
            this.m_House = null;
            this.m_Location = Point3D.Zero;
        }

        public AttendantHerald(Serial serial)
            : base(serial)
        {
        }

        public virtual TimeSpan YellDelay
        {
            get
            {
                return TimeSpan.FromSeconds(15);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public HeraldEntry Announcement
        {
            get
            {
                return this.m_Announcement;
            }
            set
            {
                this.m_Announcement = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public HeraldEntry Greeting
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
        public override void OnDoubleClick(Mobile from)
        {
            if (from.Alive && this.IsOwner(from))
            {
                from.CloseGump(typeof(OptionsGump));
                from.SendGump(new OptionsGump(this));
            }
            else
                base.OnDoubleClick(from);
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && this.IsOwner(from))
            {
                list.Add(new AttendantUseEntry(this, 6248));
                list.Add(new HeraldSetAnnouncementTextEntry(this));
                list.Add(new HeraldSetGreetingTextEntry(this));
                list.Add(new AttendantDismissEntry(this));
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m != null && m.Player && !m.Hidden && m != this)
            {
                if (this.ControlOrder == OrderType.Follow && this.m_NextYell < DateTime.UtcNow && m != this.ControlMaster && this.m_Announcement != null)
                {
                    this.m_Announcement.Say(this, m);
                    this.m_NextYell = DateTime.UtcNow + this.YellDelay + TimeSpan.FromSeconds(Utility.RandomMinMax(-2, 2));
                }
                else if (this.ControlOrder == OrderType.Stay && this.m_Greeting != null)
                {
                    if (this.m_Location != this.Location)
                    {
                        this.m_House = BaseHouse.FindHouseAt(this);
                        this.m_Location = this.Location;
                    }

                    if (this.m_House != null && !this.m_House.IsInside(oldLocation, 16) && this.m_House.IsInside(m))
                        this.m_Greeting.Say(this, m);
                }
            }
        }

        public override bool InGreetingMode(Mobile owner)
        {
            if (this.m_Location != this.Location)
            {
                this.m_House = BaseHouse.FindHouseAt(this);
                this.m_Location = this.Location;
            }

            return (this.m_House != null && this.m_House.IsOwner(owner) && this.ControlOrder == OrderType.Stay);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(this.m_Announcement != null);
			
            if (this.m_Announcement != null)
                this.m_Announcement.Serialize(writer);
			
            writer.Write(this.m_Greeting != null);
			
            if (this.m_Greeting != null)
                this.m_Greeting.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (reader.ReadBool())
            {
                this.m_Announcement = new HeraldEntry();
                this.m_Announcement.Deserialize(reader);
            }
				
            if (reader.ReadBool())
            {
                this.m_Greeting = new HeraldEntry();
                this.m_Greeting.Deserialize(reader);
            }

            this.m_Location = Point3D.Zero;
        }

        public virtual void SetAnnouncementText(Mobile by)
        {
            by.SendGump(new SetTextGump(this, m_Announcements, true));
        }

        public virtual void SetGreetingText(Mobile by)
        {
            by.SendGump(new SetTextGump(this, m_Greetings, false));
        }

        [PropertyObject]
        public class HeraldEntry
        {
            private TextDefinition m_Message;
            private string[] m_Arguments;
            public HeraldEntry()
                : this(null, null)
            {
            }

            public HeraldEntry(TextDefinition message)
                : this(message, null)
            {
            }

            public HeraldEntry(TextDefinition message, params string[] args)
            {
                this.m_Message = message;
                this.m_Arguments = args;
            }

            [CommandProperty(AccessLevel.GameMaster)]
            public TextDefinition Message
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
            [CommandProperty(AccessLevel.GameMaster)]
            public string Arguments
            { 
                get
                { 
                    string text = String.Empty;

                    foreach (string s in this.m_Arguments)
                        text += '|' + s;

                    return text + '|'; 
                }
                set
                {
                    if (value != null)
                        this.m_Arguments = value.Split('|');
                    else
                        this.m_Arguments = null;
                }
            }
            public override string ToString()
            {
                if (this.m_Message != null)
                    return this.m_Message.ToString();

                return base.ToString();
            }

            public void Say(AttendantHerald herald, Mobile visitor)
            {
                if (this.m_Message.Number > 0)
                {
                    herald.Say(this.m_Message.Number, this.ConstructNumber(herald, visitor));
                }
                else if (this.m_Message.String != null)
                {
                    herald.Say(this.ConstructString(herald, visitor));
                }
            }

            public GumpEntry Construct(AttendantHerald herald, int x, int y, int width, int height, int color)
            {
                if (this.m_Message.Number > 0)
                {
                    string args = this.ConstructNumber(herald, null);

                    return new GumpHtmlLocalized(x, y, width, height, this.m_Message.Number, args, color, false, false);
                }
                else if (this.m_Message.String != null)
                {
                    string message = String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, this.ConstructString(herald, null));

                    return new GumpHtml(x, y, width, height, message, false, false);
                }

                return null;
            }

            public string ConstructNumber(AttendantHerald herald, Mobile visitor)
            {
                string args = String.Empty;

                if (this.m_Arguments != null && this.m_Arguments.Length > 0)
                {
                    args = this.Construct(herald, visitor, this.m_Arguments[0]);

                    for (int i = 1; i < this.m_Arguments.Length; i++)
                        args = String.Format("{0}\t{1}", args, this.Construct(herald, visitor, this.m_Arguments[i]));
                }

                return args;
            }

            public string ConstructString(AttendantHerald herald, Mobile visitor)
            {
                string message = this.m_Message.String;

                if (this.m_Arguments != null && this.m_Arguments.Length > 0)
                {
                    string[] args = new string[this.m_Arguments.Length];

                    for (int i = 0; i < args.Length; i++)
                        args[i] = this.Construct(herald, visitor, this.m_Arguments[i]);

                    message = String.Format(message, args);
                }

                return message;
            }

            public string Construct(AttendantHerald herald, Mobile visitor, string argument)
            {
                if (herald == null || herald.Deleted || herald.ControlMaster == null)
                    return String.Empty;

                Mobile m = herald.ControlMaster;

                switch ( argument )
                {
                    case "[OWNER TITLE]":
                        return "Mighty"; 
                    case "[OWNER NAME]":
                        return m.Name;
                    case "[OWNER SEX]":
                        return m.Female ? "lady" : "lord";
                    case "[OWNER OPPOSITE SEX]":
                        return m.Female ? "lord" : "lady";
                    case "[OWNER SEX P]":
                        return m.Female ? "ladies" : "lords";
                    case "[OWNER OPPOSITE SEX P]":
                        return m.Female ? "lords" : "ladies";
                    case "[VISITOR TITLE]":
                        return visitor != null ? "Mighty" : argument; 
                    case "[VISITOR NAME]":
                        return visitor != null ? visitor.Name : argument;
                }

                return String.Empty;
            }

            public void Serialize(GenericWriter writer)
            {
                writer.WriteEncodedInt(0); // version
				
                if (this.m_Message.Number > 0)
                {
                    writer.Write((byte)0x1);
                    writer.Write(this.m_Message.Number);
                }
                else if (this.m_Message.String != null)
                {
                    writer.Write((byte)0x2);
                    writer.Write(this.m_Message.String);
                }
                else
                    writer.Write((byte)0x0);

                if (this.m_Arguments != null)
                {
                    writer.WriteEncodedInt(this.m_Arguments.Length);

                    foreach (string s in this.m_Arguments)
                        writer.Write(s);
                }
                else
                    writer.WriteEncodedInt(0);
            }

            public void Deserialize(GenericReader reader)
            {
                int version = reader.ReadEncodedInt();
					
                byte type = reader.ReadByte();
				
                switch ( type )
                {
                    case 0x1:
                        this.m_Message = reader.ReadInt();
                        break;
                    case 0x2:
                        this.m_Message = reader.ReadString();
                        break;
                }

                this.m_Arguments = new string[reader.ReadEncodedInt()];

                for (int i = 0; i < this.m_Arguments.Length; i++)
                    this.m_Arguments[i] = reader.ReadString();
            }
        }

        private class OptionsGump : Gump
        {
            private readonly AttendantHerald m_Herald;
            public OptionsGump(AttendantHerald herald)
                : base(200, 200)
            {
                this.m_Herald = herald;

                this.AddBackground(0, 0, 273, 324, 0x13BE);
                this.AddImageTiled(10, 10, 253, 20, 0xA40);
                this.AddImageTiled(10, 40, 253, 244, 0xA40);
                this.AddImageTiled(10, 294, 253, 20, 0xA40);
                this.AddAlphaRegion(10, 10, 253, 304);
                this.AddButton(10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 296, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
                this.AddHtmlLocalized(14, 12, 273, 20, 1075996, 0x7FFF, false, false); // Herald

                this.AddButton(15, 45, 0x845, 0x846, 3, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 43, 450, 20, 3006247, 0x7FFF, false, false); // Set Announcement Text

                this.AddButton(15, 65, 0x845, 0x846, 4, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 63, 450, 20, 3006246, 0x7FFF, false, false); // Set Greeting Text

                if (herald.ControlOrder == OrderType.Stay)
                {
                    this.AddHtmlLocalized(45, 83, 450, 20, 1076138, 0x7D32, false, false); // Stay here and greet guests

                    this.AddButton(15, 105, 0x845, 0x846, 6, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(45, 103, 450, 20, 1076139, 0x7FFF, false, false); // Follow me
                }
                else
                {
                    this.AddButton(15, 85, 0x845, 0x846, 5, GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(45, 83, 450, 20, 1076138, 0x7FFF, false, false); // Stay here and greet guests

                    this.AddHtmlLocalized(45, 103, 450, 20, 1076139, 0x7D32, false, false); // Follow me
                    this.AddTooltip(1076141); // You can only issue this command when your herald is in greeting mode.
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Herald == null || this.m_Herald.Deleted)
                    return;

                Mobile m = sender.Mobile;

                switch ( info.ButtonID )
                {
                    case 3:
                        this.m_Herald.SetAnnouncementText(m);
                        break;
                    case 4:
                        this.m_Herald.SetGreetingText(m);
                        break;
                    case 5: 
                        {
                            if (this.m_Herald.ControlOrder == OrderType.Follow)
                            {
                                BaseHouse house = BaseHouse.FindHouseAt(this.m_Herald);

                                if (house != null && house.IsOwner(m))
                                {
                                    this.m_Herald.ControlOrder = OrderType.Stay;
                                    this.m_Herald.ControlTarget = null;
                                }
                                else
                                    m.SendLocalizedMessage(1076140); // You must be in a house you control to put your herald into greeting mode.
                            }
						
                            break;
                        }
                    case 6:
                        {
                            if (this.m_Herald.ControlOrder == OrderType.Stay)
                            {
                                this.m_Herald.ControlOrder = OrderType.Follow;
                                this.m_Herald.ControlTarget = m;
                            }

                            break;
                        }
                }
            }
        }

        private class SetTextGump : Gump
        {
            private readonly AttendantHerald m_Herald;
            private readonly HeraldEntry[] m_Entries;
            private readonly bool m_Announcement;
            public SetTextGump(AttendantHerald herald, HeraldEntry[] entries, bool announcement)
                : base(60, 36)
            {
                this.m_Herald = herald;
                this.m_Entries = entries;
                this.m_Announcement = announcement;

                this.AddPage(0);

                this.AddBackground(0, 0, 520, 324, 0x13BE);
                this.AddImageTiled(10, 10, 500, 20, 0xA40);
                this.AddImageTiled(10, 40, 500, 244, 0xA40);
                this.AddImageTiled(10, 294, 500, 20, 0xA40);
                this.AddAlphaRegion(10, 10, 500, 304);
                this.AddButton(10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 296, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
                this.AddHtmlLocalized(14, 12, 520, 20, 3006246 + (announcement ? 1 : 0), 0x7FFF, false, false); // Set Announcement/Greeting Text

                for (int i = 0; i < entries.Length; i++)
                {
                    if (i % 5 == 0)
                    {
                        int page = i / 5 + 1;

                        if (page > 1)
                        {
                            this.AddButton(435, 294, 0xFA5, 0xFA7, 0, GumpButtonType.Page, page);
                            this.AddHtmlLocalized(475, 296, 60, 20, 1043353, 0x7FFF, false, false); // Next
                        }

                        this.AddPage(page);

                        if (page > 1)
                        {
                            this.AddButton(360, 294, 0xFAE, 0xFB0, 0, GumpButtonType.Page, page - 1);
                            this.AddHtmlLocalized(400, 296, 60, 20, 1011393, 0x7FFF, false, false); // Back
                        }
                    }
					
                    this.AddButton(19, 49 + (i % 5) * 48, 0x845, 0x846, 100 + i, GumpButtonType.Reply, 0);
                    this.Add(entries[i].Construct(herald, 44, 47 + (i % 5) * 48, 460, 40, 0x7FFF));
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Herald == null || this.m_Herald.Deleted)
                    return;
				
                int index = info.ButtonID - 100; 

                if (index >= 0 && index < this.m_Entries.Length)
                {
                    HeraldEntry entry = this.m_Entries[index];

                    if (this.m_Announcement)
                    {
                        this.m_Herald.Announcement = entry;
                        sender.Mobile.SendLocalizedMessage(1076686); // Your herald's announcement has been changed.
                    }
                    else
                    {
                        this.m_Herald.Greeting = entry;
                        sender.Mobile.SendLocalizedMessage(1076687); // Your herald's greeting has been changed.
                    }
                }
            }
        }
    }

    public class AttendantMaleHerald : AttendantHerald
    { 
        [Constructable]
        public AttendantMaleHerald()
            : base()
        {
        }

        public AttendantMaleHerald(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.SetStr(50, 60);
            this.SetDex(20, 30);
            this.SetInt(100, 110);

            this.Name = NameList.RandomName("male");
            this.Female = false;
            this.Race = Race.Human;
            this.Hue = this.Race.RandomSkinHue();

            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            this.AddItem(new FurBoots());
            this.AddItem(new LongPants(0x901));
            this.AddItem(new TricorneHat());
            this.AddItem(new FormalShirt(Utility.RandomBlueHue()));
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

    public class AttendantFemaleHerald : AttendantHerald
    { 
        [Constructable]
        public AttendantFemaleHerald()
            : base()
        {
        }

        public AttendantFemaleHerald(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.SetStr(50, 60);
            this.SetDex(20, 30);
            this.SetInt(100, 110);

            this.Name = NameList.RandomName("female");
            this.Female = true;
            this.Race = Race.Human;
            this.Hue = this.Race.RandomSkinHue();

            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            Lantern lantern = new Lantern();
            lantern.Ignite();

            this.AddItem(lantern);
            this.AddItem(new Shoes(Utility.RandomNeutralHue()));
            this.AddItem(new Bonnet(Utility.RandomPinkHue()));
            this.AddItem(new PlainDress(Utility.RandomPinkHue()));
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
}

namespace Server.ContextMenus
{
    public class HeraldSetAnnouncementTextEntry : ContextMenuEntry
    {
        private readonly AttendantHerald m_Attendant;
        public HeraldSetAnnouncementTextEntry(AttendantHerald attendant)
            : base(6247)
        {
            this.m_Attendant = attendant;
        }

        public override void OnClick()
        {
            if (this.m_Attendant == null || this.m_Attendant.Deleted)
                return;

            this.m_Attendant.SetAnnouncementText(this.Owner.From);
        }
    }

    public class HeraldSetGreetingTextEntry : ContextMenuEntry
    {
        private readonly AttendantHerald m_Attendant;
        public HeraldSetGreetingTextEntry(AttendantHerald attendant)
            : base(6246)
        {
            this.m_Attendant = attendant;
        }

        public override void OnClick()
        {
            if (this.m_Attendant == null || this.m_Attendant.Deleted)
                return;

            this.m_Attendant.SetGreetingText(this.Owner.From);
        }
    }
}