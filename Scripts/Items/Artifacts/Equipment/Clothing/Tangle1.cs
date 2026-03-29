using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTailoring), typeof(GargishTangle1))]
    public class Tangle1 : HalfApron
	{
        public override int LabelNumber { get { return 1114784; } } // Tangle
		public override bool IsArtifact { get { return true; } }

        [Constructable]
        public Tangle1()
            : base()
        {
            Hue = 506;
            Attributes.BonusInt = 10;
            Attributes.DefendChance = 5;
            Attributes.RegenMana = 2;

            switch (Utility.Random(5))
            {
                case 0: SkillBonuses.SetValues(0, SkillName.AnimalLore, 20.0); break;
                case 1: SkillBonuses.SetValues(0, SkillName.Mysticism, 20.0); break;
                case 2: SkillBonuses.SetValues(0, SkillName.Magery, 20.0); break;
                case 3: SkillBonuses.SetValues(0, SkillName.Spellweaving, 20.0); break;
                case 4: SkillBonuses.SetValues(0, SkillName.Necromancy, 20.0); break;
            }
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

    public class GargishTangle1 : GargoyleHalfApron
    {
        public override int LabelNumber { get { return 1114784; } } // Tangle
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishTangle1()
            : base()
        {
            Hue = 506;
            Attributes.BonusInt = 10;
            Attributes.DefendChance = 5;
            Attributes.RegenMana = 2;

            switch (Utility.Random(5))
            {
                case 0: SkillBonuses.SetValues(0, SkillName.AnimalLore, 20.0); break;
                case 1: SkillBonuses.SetValues(0, SkillName.Mysticism, 20.0); break;
                case 2: SkillBonuses.SetValues(0, SkillName.Magery, 20.0); break;
                case 3: SkillBonuses.SetValues(0, SkillName.Spellweaving, 20.0); break;
                case 4: SkillBonuses.SetValues(0, SkillName.Necromancy, 20.0); break;
            }
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