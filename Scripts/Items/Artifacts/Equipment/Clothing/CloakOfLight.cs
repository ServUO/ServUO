using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTailoring), typeof(WingArmorOfLight))]
    public class CloakOfLight : Cloak
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public CloakOfLight()
        {
			Attributes.NightSight = 1;
            Attributes.RegenHits = 2;
            Attributes.AttackChance = 5;
        }

        public CloakOfLight(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber { get{return 1155608;} }// Cloak of Light
       
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WingArmorOfLight : GargishClothWingArmor
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public WingArmorOfLight()
        {
            Attributes.NightSight = 1;
            Attributes.RegenHits = 2;
            Attributes.AttackChance = 5;
        }

        public WingArmorOfLight(Serial serial)
            : base(serial)
        {
        }

		public override int LabelNumber { get{return 1155683;} }// Wing Armor of Light
  
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}