using System;

namespace Server.Items
{ 
    public class LibraryFriendLantern : Lantern
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LibraryFriendLantern()
            : base()
        {
        }

        public LibraryFriendLantern(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073339;
            }
        }// Friends of the Library Reading Lantern
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LibraryFriendReadingChair : BigElvenChair
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LibraryFriendReadingChair()
            : base()
        {
        }

        public LibraryFriendReadingChair(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073340;
            }
        }// Friends of the Library Reading Chair
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}