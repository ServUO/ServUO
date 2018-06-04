using System;

namespace Server.Items
{
    public class AbyssReaver : Cyclone
	{
		public override bool IsArtifact { get { return true; } }

        [Constructable]
        public AbyssReaver()
            : base(0x901)
        {
            Weight = 6.0;
            Layer = Layer.OneHanded;
            Hue = 1195;

            SkillBonuses.SetValues(0, SkillName.Throwing, 10.0);
            //Attributes.AttackChance = 15;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 35;

            Slayer = SlayerName.DaemonDismissal;
        }

        public AbyssReaver(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber { get { return 1112694; } } // Abyss Reaver
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = InheritsItem ? 1 : reader.ReadInt();

            if (version == 0)
            {
                Attributes.AttackChance = 0;
                Attributes.WeaponSpeed = 0;
            }
        }
    }
}