using System;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.Quests;
using Server.Commands;

namespace Server.Items
{
    public enum DecorType
    {
        Tunic,
        Pant,
        Book
    };

    public class MasterThinkerContoller : Item
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenMasterThinker", AccessLevel.Developer, new CommandEventHandler(GenMasterThinker_Command));
        }

        [Usage("GenMasterThinker")]
        private static void GenMasterThinker_Command(CommandEventArgs e)
        {
            if (Check())
            {
                e.Mobile.SendMessage("Sorcerers Plate is already present.");
            }
            else
            {
                e.Mobile.SendMessage("Creating Sorcerers Plate...");

                MasterThinkerContoller controller = new MasterThinkerContoller();
                controller.MoveToWorld(new Point3D(1652, 1547, 45), Map.Trammel);

                e.Mobile.SendMessage("Generation completed!");
            }
        }

        private static bool Check()
        {
            foreach (Item item in World.Items.Values)
                if (item is MasterThinkerContoller && !item.Deleted)
                    return true;

            return false;
        }

        public class MasterThinkerArray
        {
            public Mobile Mobile { get; set; }

            public bool Book { get; set; }

            public bool Pant { get; set; }

            public bool Tunic { get; set; }
        }

        private MasterThinkerDecor m_Book, m_Pant, m_Tunic;

        public static List<MasterThinkerArray> m_Array;

        public List<MasterThinkerArray> Array { get { return m_Array; } }


        public MasterThinkerContoller()
            : base(0x1F13)
        {
            Name = "Master Thinker Controller - Do not remove !!";
            Visible = false;
            Movable = false;

            m_Array = new List<MasterThinkerArray>();

            m_Book = new MasterThinkerDecor(0x42BF, 0, DecorType.Book, this);
            m_Book.MoveToWorld(new Point3D(1651, 1549, 49), Map.Trammel);

            m_Pant = new MasterThinkerDecor(0x1539, 2017, DecorType.Pant, this);
            m_Pant.MoveToWorld(new Point3D(1651, 1545, 47), Map.Trammel);

            m_Tunic = new MasterThinkerDecor(0x1FA2, 398, DecorType.Tunic, this);
            m_Tunic.MoveToWorld(new Point3D(1653, 1549, 47), Map.Trammel);
        }

        public MasterThinkerContoller(Serial serial)
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

            m_Array = new List<MasterThinkerArray>();
        }
    }

    public class MasterThinkerDecor : Item
    {
        private MasterThinkerContoller m_Controller;
        private DecorType m_Type;

        [CommandProperty(AccessLevel.GameMaster)]
        public DecorType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.InvalidateProperties();
            }
        }

        [Constructable]
        public MasterThinkerDecor(int id, int hue, DecorType type, MasterThinkerContoller controller)
            : base()
        {
            this.ItemID = id;
            this.m_Controller = controller;
            this.m_Type = type;
            this.Hue = hue;
            this.Movable = false;
        }

        public MasterThinkerDecor(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Controller != null)
            {
                if (this.m_Controller.Array.Where(s => s.Mobile == from).Count() == 0)
                {
                    this.m_Controller.Array.Add(new MasterThinkerContoller.MasterThinkerArray { Mobile = from, Book = false, Pant = false, Tunic = false });
                }

                if (this.m_Type == DecorType.Book)
                {
                    this.m_Controller.Array.Find(s => s.Mobile == from).Book = true;
                    from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154222); // *You thumb through the pages of the book, it seems to describe the anatomy of a variety of frost creatures*            
                }
                else if (this.m_Type == DecorType.Pant)
                {
                    this.m_Controller.Array.Find(s => s.Mobile == from).Pant = true;
                    from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154221); // *You carefully examine the garment and take note of it's superior quality. You surmise it would be useful in keeping you warm in a cold environment*
                }
                else if (this.m_Type == DecorType.Tunic)
                {
                    this.m_Controller.Array.Find(s => s.Mobile == from).Tunic = true;
                    from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154221); // *You carefully examine the garment and take note of it's superior quality. You surmise it would be useful in keeping you warm in a cold environment*
                }
                
                if (ClickCheck(from) == 1)
                {
                    PlayerMobile pm = from as PlayerMobile;

                    if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulsonComplete)
                    {
                        pm.ExploringTheDeepQuest = ExploringTheDeepQuestChain.CusteauPerronHouse;
                        this.m_Controller.Array.RemoveAll(s => s.Mobile == from);
                    }
                }
            }
        }

        public int ClickCheck(Mobile from)
        {
            return this.m_Controller.Array.Where(s => s.Mobile == from && s.Pant == true && s.Book == true && s.Tunic == true).Count();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_Controller);
            writer.Write((int)m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            
            this.m_Controller = reader.ReadItem() as MasterThinkerContoller;
            this.m_Type = (DecorType)reader.ReadInt();
        }
    }
}
