using System;
using Server.Mobiles;

namespace Server.Items
{
    public class StygianDragonAltar : PeerlessAltar
    { 
        private int m_ID;
        [Constructable]
        public StygianDragonAltar()
            : base(0x2206)
        {
            this.Visible = false;
				
            this.BossLocation = new Point3D(327, 159, 20);
            this.TeleportDest = new Point3D(346, 158, 0);
            this.ExitDest = new Point3D(529, 921, -1);
			
            this.m_ID = 0;
        }

        public StygianDragonAltar(Serial serial)
            : base(serial)
        {
        }

        public override int KeyCount
        {
            get
            {
                return 3;
            }
        }
        public override MasterKey MasterKey
        {
            get
            {
                return new StygianDragonKey();
            }
        }
        public override Type[] Keys
        {
            get
            {
                return new Type[]
                {
                    typeof(DraconicOrbKey), typeof(DraconicOrbKeyBlue), typeof(DraconicOrbKeyRed),
                    typeof(DraconicOrbKeyOrange)//, typeof( ScatteredCrystals ), typeof( ShatteredCrystals )
                };
            }
        }
        public override BasePeerless Boss
        {
            get
            {
                return new StygianDragon();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
            writer.Write((int)this.m_ID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            this.m_ID = reader.ReadInt();
        }

        public int GetID()
        {
            int id = this.m_ID;
            this.m_ID += 1;
            return id;
        }

        public bool TryDrop(Mobile from, Item item, int id)
        {
            if (id >= 0 && id < this.Keys.Length && item != null)
            {
                if (item.GetType() == this.Keys[id])
                    return this.OnDragDrop(from, item);
            }
			
            return false;
        }
    }

    public class StygianDragonBrazier : Container
    {
        private StygianDragonAltar m_Altar;
        private int m_ID;
        public StygianDragonBrazier(StygianDragonAltar altar, int hue)
            : base(0x207B)
        {
            this.Hue = 2416;
            this.Movable = false;
		
            this.m_Altar = altar;
			
            if (this.m_Altar != null)
                this.m_ID = this.m_Altar.GetID();
        }

        public StygianDragonBrazier(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public StygianDragonAltar Altar
        {
            get
            {
                return this.m_Altar;
            }
            set
            {
                this.m_Altar = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ID
        {
            get
            {
                return this.m_ID;
            }
            set
            {
                this.m_ID = value;
            }
        }
        public override bool OnDragDrop(Mobile from, Item dropped)
        { 
            if (this.m_Altar == null)
                return false;
											
            if (this.m_Altar.Activated)
            { 
                from.SendLocalizedMessage(1075213); // The master of this realm has already been summoned and is engaged in combat.  Your opportunity will come after he has squashed the current batch of intruders!
                return false;
            }
			
            if (!this.m_Altar.TryDrop(from, dropped, this.m_ID))
            {
                from.SendLocalizedMessage(1072682); // This is not the proper key.
                return false;
            }
            else
                return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
            writer.Write((int)this.m_ID);
            writer.Write((Item)this.m_Altar);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            this.m_ID = reader.ReadInt();
            this.m_Altar = reader.ReadItem() as StygianDragonAltar;
        }
    }
}