using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public class PrismOfLightAltar : PeerlessAltar
    {
        private int m_ID;
        public override int KeyCount { get { return 3; } }
        public override MasterKey MasterKey { get { return new PrismOfLightKey(); } }
        public List<Item> Pedestals = new List<Item>();

        public override Type[] Keys
        {
            get
            {
                return new Type[]
                {
                    typeof(JaggedCrystals), typeof(BrokenCrystals), typeof(PiecesOfCrystal),
                    typeof(CrushedCrystals), typeof(ScatteredCrystals), typeof(ShatteredCrystals)
                };
            }
        }

        public override BasePeerless Boss { get { return new ShimmeringEffusion(); } }

        [Constructable]
        public PrismOfLightAltar() : base(0x2206)
        {
            Visible = false;

            BossLocation = new Point3D(6520, 122, -20);
            TeleportDest = new Point3D(6520, 139, -20);
            ExitDest = new Point3D(3785, 1107, 20);

            m_ID = 0;
        }

        public override void ClearContainer()
        {
            base.ClearContainer();

            Pedestals.ForEach(x => x.Hue = ((PrismOfLightPillar)x).OrgHue);
        }

        public override Rectangle2D[] BossBounds
        {
            get { return m_Bounds; }
        }

        private Rectangle2D[] m_Bounds = new Rectangle2D[]
        {
            new Rectangle2D(6500, 111, 45, 35),
        };

        public PrismOfLightAltar(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write(Pedestals, true);

            writer.Write((int)m_ID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {                        
                        Pedestals = reader.ReadStrongItemList();
                        goto case 0;
                    }
                case 0:
                    {
                        m_ID = reader.ReadInt();                        
                        break;
                    }                
            }                     
        }

        public int GetID()
        {
            int id = m_ID;
            m_ID += 1;
            return id;
        }

        public bool TryDrop(Mobile from, Item item, PrismOfLightPillar c)
        {
            int id = c.ID;

            if (id >= 0 && id < Keys.Length && item != null)
            {
                if (item.GetType() == Keys[id])
                {
                    if (Items.Count < Keys.Length)
                    {
                        c.Hue = 36;
                    }

                    return OnDragDrop(from, item);
                }
            }

            return false;
        }
    }

    public class PrismOfLightPillar : Container
    {
        public override int LabelNumber { get { return 1024643; } } // pedestal
        private PrismOfLightAltar m_Altar;
        private int m_ID;
        private int m_OrgHue;

        [CommandProperty(AccessLevel.GameMaster)]
        public PrismOfLightAltar Altar
        {
            get { return m_Altar; }
            set
            {
                m_Altar = value;
                                
                if (!m_Altar.Pedestals.Contains(this))
                    m_Altar.Pedestals.Add(this);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int OrgHue
        {
            get { return m_OrgHue; }
            set
            {
                m_OrgHue = value;
                Hue = m_OrgHue;
                InvalidateProperties();
            }
        }

        public PrismOfLightPillar(PrismOfLightAltar altar, int hue) 
            : base(0x207D)
        {
            OrgHue = hue;
            Movable = false;

            m_Altar = altar;

            if (m_Altar != null)
            { 
                m_ID = m_Altar.GetID();
                m_Altar.Pedestals.Add(this);
            }
        }

        public PrismOfLightPillar(Serial serial) : base(serial)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (m_Altar == null)
                return false;

            if (m_Altar.Activated)
            {
                from.SendLocalizedMessage(1075213); // The master of this realm has already been summoned and is engaged in combat.  Your opportunity will come after he has squashed the current batch of intruders!
                return false;
            }

            if (!m_Altar.TryDrop(from, dropped, this))
            {
                from.SendLocalizedMessage(1072682); // This is not the proper key.
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write((int)m_OrgHue);

            writer.Write((int)m_ID);
            writer.Write((Item)m_Altar);            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_OrgHue = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {                        
                        m_ID = reader.ReadInt();
                        m_Altar = reader.ReadItem() as PrismOfLightAltar;                        

                        break;
                    }                
            }

            if (version < 1)
            {
                if (m_OrgHue == 0)
                    m_OrgHue = Hue;

                if (!m_Altar.Pedestals.Contains(this))
                    m_Altar.Pedestals.Add(this);
            }
        }
    }
}
