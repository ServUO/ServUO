using System;

namespace Server.Items
{
    [Flipable(40641, 40642)]
	[Furniture]
    public class MaleTopper : Item
    {
        [Constructable]
        public MaleTopper()
            : base(40641)
        {
            Name = "Male Topper";
			this.Weight = 1.0;
			LootType = LootType.Blessed;
        }

        public MaleTopper(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 6.0)
                this.Weight = 1.0;
        }
    }
	
	[Flipable(40643, 40644)]
	[Furniture]
    public class FemaleTopper : Item
    {
        [Constructable]
        public FemaleTopper()
            : base(40643)
        {
            Name = "Female Topper";
			this.Weight = 1.0;
			LootType = LootType.Blessed;
        }

        public FemaleTopper(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 6.0)
                this.Weight = 1.0;
        }
    }
}