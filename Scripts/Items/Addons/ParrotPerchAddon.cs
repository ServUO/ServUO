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
            AddComponent(new AddonComponent(0x2FB6), 0, 0, 0);

            m_Parrot = parrot;
        }

        public ParrotPerchAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ParrotPerchAddonDeed(m_Parrot);
        public override bool RetainDeedHue => true;
        [CommandProperty(AccessLevel.GameMaster)]
        public PetParrot Parrot
        {
            get
            {
                return m_Parrot;
            }
            set
            {
                m_Parrot = value;
            }
        }
        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            if (m_Parrot != null)
                m_Parrot.Location = new Point3D(X, Y, Z + 12);
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (m_Parrot != null)
                m_Parrot.Map = Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Parrot != null)
                m_Parrot.Internalize();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); //  version

            writer.Write(m_Parrot);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Parrot = reader.ReadMobile() as PetParrot;
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
            LootType = LootType.Blessed;

            m_Parrot = parrot;
        }

        public ParrotPerchAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072619;// A deed for a Parrot Perch		
        public override BaseAddon Addon => new ParrotPerchAddon(m_Parrot);
        [CommandProperty(AccessLevel.GameMaster)]
        public PetParrot Parrot
        {
            get
            {
                return m_Parrot;
            }
            set
            {
                m_Parrot = value;
                InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Parrot != null)
            {
                if (m_Parrot.Name != null)
                    list.Add(1072624, m_Parrot.Name); // Includes a pet Parrot named ~1_NAME~
                else
                    list.Add(1072620); // Includes a pet Parrot

                int weeks = PetParrot.GetWeeks(m_Parrot.Birth);

                if (weeks == 1)
                    list.Add(1072626); // 1 week old
                else if (weeks > 1)
                    list.Add(1072627, weeks.ToString()); // ~1_AGE~ weeks old
            }
        }

        public override void DeleteDeed()
        {
            m_Safety = true;

            base.DeleteDeed();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (!m_Safety && m_Parrot != null)
                m_Parrot.Delete();

            m_Safety = false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); //  version

            writer.Write(m_Parrot);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Parrot = reader.ReadMobile() as PetParrot;

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