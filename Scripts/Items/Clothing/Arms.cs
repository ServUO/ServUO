using System;

namespace Server.Items
{
    public class GargishClothArms : BaseClothing
    {
        [Constructable]
        public GargishClothArms()
            : this(0)
        {
        }

        [Constructable]
        public GargishClothArms(int hue)
            : base(0x0404, Layer.Arms, hue)
        {
            this.Weight = 2.0;
        }

        public GargishClothArms(Serial serial)
            : base(serial)
        {
        }

        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }
        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                if (((Mobile)parent).Female)
                    this.ItemID = 0x0403;
                else
                    this.ItemID = 0x0404;
            }
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
        }
    }

    public class FemaleGargishClothArms : BaseClothing
    {
        [Constructable]
        public FemaleGargishClothArms()
            : this(0)
        {
        }

        [Constructable]
        public FemaleGargishClothArms(int hue)
            : base(0x0403, Layer.Arms, hue)
        {
            this.Weight = 2.0;
        }

        public FemaleGargishClothArms(Serial serial)
            : base(serial)
        {
        }

        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
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
        }
    }

    public class MaleGargishClothArms : BaseClothing
    {
        [Constructable]
        public MaleGargishClothArms()
            : this(0)
        {
        }

        [Constructable]
        public MaleGargishClothArms(int hue)
            : base(0x0404, Layer.Arms, hue)
        {
            this.Weight = 2.0;
        }

        public MaleGargishClothArms(Serial serial)
            : base(serial)
        {
        }

        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
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
        }
    }
}