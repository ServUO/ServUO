using System;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Engines.Quests;

namespace Server.Items
{
    public class SorcerersPlateController : Item
    {
        public static readonly string EntityName = "sorcerersplate";

        public static void Initialize()
        {
            CommandSystem.Register("GenSorcerersPlate", AccessLevel.Developer, new CommandEventHandler(GenSorcerersPlate_Command));
            CommandSystem.Register("DelSorcerersPlate", AccessLevel.Developer, new CommandEventHandler(DelSorcerersPlate_Command));
        }

        [Usage("GenSorcerersPlate")]
        private static void GenSorcerersPlate_Command(CommandEventArgs e)
        {
            DeletePlates();
            e.Mobile.SendMessage("Creating Sorcerers Plate...");

            SorcerersPlateController controller = new SorcerersPlateController();
            WeakEntityCollection.Add(EntityName, controller);

            e.Mobile.SendMessage("Sorcerers Plate Generation Completed!");
        }

        [Usage("DelSorcerersPlate")]
        private static void DelSorcerersPlate_Command(CommandEventArgs e)
        {
            DeletePlates();
        }

        private static void DeletePlates()
        {
            WeakEntityCollection.Delete(EntityName);
        }

        private SorcerersPlate m_PerfectBlackPearl, m_BurstingBrimstone, m_BrightDaemonBlood, m_MightyMandrake, m_BurlyBone;

        public SorcerersPlateController()
            : base(0x1F13)
        {
            Name = "Sorcerers Plate Controller - Do not remove !!";
            Visible = false;
            Movable = false;

            MoveToWorld(new Point3D(100, 49, -22), Map.Ilshenar);

            CreateSorcerersPlates();

            Item decor;

            decor = new BrightDaemonBloodDecor();
            decor.MoveToWorld(new Point3D(149, 26, -28), Map.Ilshenar);

            decor = new BurlyBoneDecor();
            decor.MoveToWorld(new Point3D(159, 7, -23), Map.Ilshenar);

            decor = new BurstingBrimstoneDecor();
            decor.MoveToWorld(new Point3D(125, 9, -28), Map.Ilshenar);

            decor = new MightyMandrakeDecor();
            decor.MoveToWorld(new Point3D(98, 36, -18), Map.Ilshenar);

            decor = new PerfectBlackPearlDecor();
            decor.MoveToWorld(new Point3D(161, 63, -21), Map.Ilshenar);

            decor = new SorcerersRewardChest();
            decor.MoveToWorld(new Point3D(100, 41, -22), Map.Ilshenar);
        }

        public SorcerersPlateController(Serial serial)
            : base(serial)
        {
        }

        public void CreateSorcerersPlates()
        {
            m_BrightDaemonBlood = new SorcerersPlate(this, SorcerersPlate.RegsType.BrightDaemonBlood);
            m_BrightDaemonBlood.MoveToWorld(new Point3D(101, 45, -22), Map.Ilshenar);

            m_BurlyBone = new SorcerersPlate(this, SorcerersPlate.RegsType.BurlyBone);
            m_BurlyBone.MoveToWorld(new Point3D(101, 43, -22), Map.Ilshenar);

            m_BurstingBrimstone = new SorcerersPlate(this, SorcerersPlate.RegsType.BurstingBrimstone);
            m_BurstingBrimstone.MoveToWorld(new Point3D(101, 46, -22), Map.Ilshenar);

            m_MightyMandrake = new SorcerersPlate(this, SorcerersPlate.RegsType.MightyMandrake);
            m_MightyMandrake.MoveToWorld(new Point3D(101, 44, -22), Map.Ilshenar);

            m_PerfectBlackPearl = new SorcerersPlate(this, SorcerersPlate.RegsType.PerfectBlackPearl);
            m_PerfectBlackPearl.MoveToWorld(new Point3D(101, 47, -22), Map.Ilshenar);
        }

        public void Validate()
        {
            if (this.Validate(this.m_PerfectBlackPearl) && this.Validate(this.m_BurstingBrimstone) && this.Validate(this.m_BrightDaemonBlood) && this.Validate(this.m_MightyMandrake) && this.Validate(this.m_BurlyBone))
            {
                Mobile creature = TheMasterInstructor.Spawn(new Point3D(105, 38, -28), Map.Ilshenar, this);

                if (creature == null)
                    return;

                this.Clear(this.m_PerfectBlackPearl);
                this.Clear(this.m_BurstingBrimstone);
                this.Clear(this.m_BrightDaemonBlood);
                this.Clear(this.m_MightyMandrake);
                this.Clear(this.m_BurlyBone);
            }
        }

        public void Clear(SorcerersPlate plate)
        {
            if (plate != null)
            {
                Effects.SendBoltEffect(plate);

                if (plate.Reg != null)
                    plate.Reg.Delete();

                plate.Delete();
            }
        }

        public bool Validate(SorcerersPlate plate)
        {
            return (plate != null && plate.Reg != null && !plate.Reg.Deleted);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(this.m_PerfectBlackPearl);
            writer.Write(this.m_BurstingBrimstone);
            writer.Write(this.m_BrightDaemonBlood);
            writer.Write(this.m_MightyMandrake);
            writer.Write(this.m_BurlyBone);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        this.m_PerfectBlackPearl = reader.ReadItem() as SorcerersPlate;
                        this.m_BurstingBrimstone = reader.ReadItem() as SorcerersPlate;
                        this.m_BrightDaemonBlood = reader.ReadItem() as SorcerersPlate;
                        this.m_MightyMandrake = reader.ReadItem() as SorcerersPlate;
                        this.m_BurlyBone = reader.ReadItem() as SorcerersPlate;

                        break;
                    }
            }
        }
    }

    public class SorcerersPlate : Item
    {
        private SorcerersPlateController m_Controller;
        private RegsType m_Type;
        private Item m_Reg;

        public enum RegsType
        {
            PerfectBlackPearl,
            BurstingBrimstone,
            BrightDaemonBlood,
            MightyMandrake,
            BurlyBone
        };

        [CommandProperty(AccessLevel.GameMaster)]
        public SorcerersPlateController Controller
        {
            get
            {
                return this.m_Controller;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public RegsType Type
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

        [CommandProperty(AccessLevel.GameMaster)]
        public Item Reg
        {
            get
            {
                return this.m_Reg;
            }
            set
            {
                this.m_Reg = value;
                if (this.m_Controller != null)
                    this.m_Controller.Validate();
            }
        }

        public override int LabelNumber { get { return 1011199; } } // Plate

        [Constructable]
        public SorcerersPlate(SorcerersPlateController controller, RegsType type)
            : base(0x9D7)
        {
            this.m_Type = type;
            this.Movable = false;
            this.Hue = 2406;
            this.m_Controller = controller;
        }

        public SorcerersPlate(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1154248, "#{0}", 1154249 + (int)m_Type); // The Plate is Inscribed: ~1_CLUE~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile))
                return;

            from.Target = new RegsTarget(from, this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_Type);
            writer.Write(this.m_Controller);
            writer.Write(this.m_Reg);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        this.m_Type = (RegsType)reader.ReadInt();
                        this.m_Controller = reader.ReadItem() as SorcerersPlateController;
                        this.m_Reg = reader.ReadItem();

                        if (this.m_Controller == null)
                            this.Delete();

                        break;
                    }
            }
        }

        public class RegsTarget : Target
        {
            private static Mobile m_from;
            private static SorcerersPlate m_Plate;

            public RegsTarget(Mobile from, SorcerersPlate plate) : base(2, false, TargetFlags.None)
            {
                from.SendLocalizedMessage(1154254); // What do you wish to put on the plate?
                m_from = from;
                m_Plate = plate;
            }

            private bool CheckRegs(Item item)
            {
                return item is PerfectBlackPearl || item is BurstingBrimstone || item is BrightDaemonBlood || item is MightyMandrake || item is BurlyBone;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Item)
                {
                    Item item = (Item)o;

                    if (!item.IsChildOf(from.Backpack))
                    {
                        from.SendLocalizedMessage(1054107); // This item must be in your backpack.
                        return;
                    }

                    if (!CheckRegs(item))
                    {
                        from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154255); // *You place the item on the plate but it simply melts away. You had better search for a more pure reagent*
                        return;
                    }

                    PlayerMobile pm = (PlayerMobile)from;

                    if (pm.ExploringTheDeepQuest != ExploringTheDeepQuestChain.Sorcerers)
                    {
                        from.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                        return;
                    }

                    if (item.GetType().Name == m_Plate.Type.ToString())
                    {
                        from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154256); // *You place the reagent on the plate and hear an ominous click in the background...*
                        item.MoveToWorld(m_Plate.GetWorldTop(), m_Plate.Map);
                        item.Movable = false;
                        m_Plate.Reg = item;
                    }
                    else
                    {
						from.SendLocalizedMessage(500309); // Nothing Happens.                        
                    }
                }
            }
        }
    }
}
