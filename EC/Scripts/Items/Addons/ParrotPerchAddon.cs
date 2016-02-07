using System;
using Server.Mobiles;

namespace Server.Items
{
    public class ParrotPerchAddon : BaseAddon
    {
        private PetParrot m_Parrot;
        [Constructable]
        public ParrotPerchAddon()
            : this(null)
        { 
        }

        public ParrotPerchAddon(PetParrot parrot)
        {
            this.AddComponent(new AddonComponent(0x2FB6), 0, 0, 0);
			
            this.m_Parrot = parrot;
        }

        public ParrotPerchAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new ParrotPerchAddonDeed(this.m_Parrot);
            }
        }
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public PetParrot Parrot
        {
            get
            {
                return this.m_Parrot;
            }
            set
            {
                this.m_Parrot = value;
            }
        }
        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);
			
            if (this.m_Parrot != null)
                this.m_Parrot.Location = new Point3D(this.X, this.Y, this.Z + 12);
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
			
            if (this.m_Parrot != null)
                this.m_Parrot.Map = this.Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
			
            if (this.m_Parrot != null)
                this.m_Parrot.Internalize();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //  version
			
            writer.Write((Mobile)this.m_Parrot);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            this.m_Parrot = reader.ReadMobile() as PetParrot;
        }
    }

    public class ParrotPerchAddonDeed : BaseAddonDeed
    {
        private PetParrot m_Parrot;
        private bool m_Safety;
        [Constructable]
        public ParrotPerchAddonDeed()
            : this(null)
        {
        }

        public ParrotPerchAddonDeed(PetParrot parrot)
        {
            this.LootType = LootType.Blessed;
		
            this.m_Parrot = parrot;
        }

        public ParrotPerchAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072619;
            }
        }// A deed for a Parrot Perch		
        public override BaseAddon Addon
        {
            get
            {
                return new ParrotPerchAddon(this.m_Parrot);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public PetParrot Parrot
        {
            get
            {
                return this.m_Parrot;
            }
            set
            {
                this.m_Parrot = value;
                this.InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (this.m_Parrot != null)
            {
                if (this.m_Parrot.Name != null)
                    list.Add(1072624, this.m_Parrot.Name); // Includes a pet Parrot named ~1_NAME~
                else
                    list.Add(1072620); // Includes a pet Parrot
					
                int weeks = PetParrot.GetWeeks(this.m_Parrot.Birth);
				
                if (weeks == 1)
                    list.Add(1072626); // 1 week old
                else if (weeks > 1)
                    list.Add(1072627, weeks.ToString()); // ~1_AGE~ weeks old
            }
        }

        public override void DeleteDeed()
        {
            this.m_Safety = true;
			
            base.DeleteDeed();
        }

        public override void OnAfterDelete()
        { 
            base.OnAfterDelete();
			
            if (!this.m_Safety && this.m_Parrot != null)
                this.m_Parrot.Delete();
				
            this.m_Safety = false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); //  version
			
            writer.Write((Mobile)this.m_Parrot);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            switch ( version )
            {
                case 1:
                    this.m_Parrot = reader.ReadMobile() as PetParrot;
					
                    break;
                case 0: 
                    reader.ReadDateTime();
                    reader.ReadString();
                    reader.ReadInt();
					
                    break;
            }
        }
    }
}