using Server.Mobiles;

namespace Server.Engines.Exodus
{
    public class VerLorRegController : Item
    {
        private static bool m_Active;

        [CommandProperty(AccessLevel.GameMaster)]
        public static bool Active
        {
            get => m_Active;
            set { if (value) Start(); else Stop(); }
        }

        private static ClockworkExodus m_Mobile;

        [CommandProperty(AccessLevel.Administrator)]
        public static ClockworkExodus Mobile { get => m_Mobile; set => m_Mobile = value; }

        [CommandProperty(AccessLevel.Administrator)]
        public static VerLorRegController IlshenarInstance { get; set; }

        public VerLorRegController() : base(7107)
        {
            Name = "Ver Lor Reg Controller";
            Visible = false;
            Movable = false;

            Start();
        }

        public VerLorRegController(Serial serial)
            : base(serial)
        {
        }

        public static void Initialize()
        {
            if (IlshenarInstance == null)
            {
                IlshenarInstance = new VerLorRegController();
                IlshenarInstance.MoveToWorld(new Point3D(849, 648, -40), Map.Ilshenar);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Active);
            writer.WriteMobile(m_Mobile);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            m_Active = reader.ReadBool();
            m_Mobile = (ClockworkExodus)reader.ReadMobile();

            if (Map == Map.Ilshenar)
                IlshenarInstance = this;
        }

        public static void Start()
        {
            if (m_Active)
                return;

            m_Active = true;

            if (m_Mobile == null)
            {
                ClockworkExodus m = new ClockworkExodus
                {
                    Home = new Point3D(854, 642, -40),
                    RangeHome = 4
                };
                m.MoveToWorld(new Point3D(854, 642, -40), Map.Ilshenar);
                m_Mobile = m;
            }
        }

        public static void Stop()
        {
            if (!m_Active)
                return;

            m_Active = false;
            m_Mobile.Delete();
            m_Mobile = null;
        }
    }
}
