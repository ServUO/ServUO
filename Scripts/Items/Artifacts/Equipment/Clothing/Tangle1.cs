using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTailoring), typeof(GargishCrimsonCincture))]
    public class Tangle1 : HalfApron
	{
        public override int LabelNumber { get { return 1114784; } } // Tangle
		public override bool IsArtifact { get { return true; } }

        [Constructable]
        public Tangle1()
            : base()
        {
            this.Hue = 506;
			
            this.Attributes.BonusInt = 10;
            this.Attributes.DefendChance = 5;
            this.Attributes.RegenMana = 2;
        }

        public Tangle1(Serial serial)
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
        }
    }

    public class GargishTangle1 : GargoyleHalfApron, ITokunoDyable
    {
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

        public override int LabelNumber { get { return 1114784; } } // Tangle
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishTangle1()
            : base()
        {
            this.Hue = 506;

            this.Attributes.BonusInt = 10;
            this.Attributes.DefendChance = 5;
            this.Attributes.RegenMana = 2;
        }

        public GargishTangle1(Serial serial)
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
        }
    }
}