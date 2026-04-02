using System;

namespace Server.Items
{
    public class PendantOfTheMagi : GoldNecklace
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public PendantOfTheMagi()
        {
            Hue = 0x48D;
            Attributes.BonusInt = 10;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 30;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 30;
            Attributes.Luck = 150;
            SkillBonuses.SetValues(0, SkillName.EvalInt, 10.0);
            Resistances.Physical = 15;
            Resistances.Fire = 15;
            Resistances.Cold = 15;
            Resistances.Poison = 15;
            Resistances.Energy = 15;
        }

        public PendantOfTheMagi(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072937;
            }
        }// Pendant of the Magi
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}