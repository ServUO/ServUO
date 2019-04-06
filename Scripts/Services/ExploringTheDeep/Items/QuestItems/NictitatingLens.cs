using System;
using Server.Network;

namespace Server.Items
{
    public class NictitatingLens : ElvenGlasses
    {
        public override int LabelNumber { get { return 1154234; } } // Nictitating Lens

        [Constructable]
        public NictitatingLens()
            : base()
        {
            this.Hue = 1916;
            this.Weight = 2.0;
            this.LootType = LootType.Blessed;
        }
		
		public override void OnDoubleClick(Mobile from)
        {
			base.OnDoubleClick(from);			
			
			from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154235); // *Finely crafted lenses for use in allowing the wearer to maintain visual acuity while navigating an aquatic environment*
        }
		
		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }
		

        public override int BasePhysicalResistance { get { return 2; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 2; } }
        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 60; } }

        public NictitatingLens(Serial serial)
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
	
	public class GargishNictitatingLens : GargishGlasses
    {
        public override int LabelNumber { get { return 1154234; } } // Nictitating Lens

        [Constructable]
        public GargishNictitatingLens()
            : base()
        {
            this.Hue = 1916;
            this.Weight = 2.0;
            this.LootType = LootType.Blessed;
        }
		
		public override void OnDoubleClick(Mobile from)
        {
			base.OnDoubleClick(from);			
			
			from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154235); // *Finely crafted lenses for use in allowing the wearer to maintain visual acuity while navigating an aquatic environment*
        }

        public override int BasePhysicalResistance { get { return 2; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 2; } }
        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 60; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public GargishNictitatingLens(Serial serial)
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
