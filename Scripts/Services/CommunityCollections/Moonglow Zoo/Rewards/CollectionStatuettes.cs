using System;
using Server.Network;

namespace Server.Items
{
    public class CollectionStatuette : BaseStatuette
    { 
        public CollectionStatuette(int itemID)
            : base(itemID)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;			
        }

        public CollectionStatuette(Serial serial)
            : base(serial)
        {
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (this.TurnedOn && this.IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, this.Location, 2) && !Utility.InRange(oldLocation, this.Location, 2))
            {
                this.PublicOverheadMessage(MessageType.Regular, 0x3B2, Utility.RandomMinMax(1073207, 1073216));
                Effects.PlaySound(this.Location, this.Map, Utility.RandomMinMax(0x5A, 0xE9));
            }
				
            base.OnMovement(m, oldLocation);
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

    public class SilverSteedZooStatuette : CollectionStatuette
    { 
        [Constructable]
        public SilverSteedZooStatuette()
            : base(0x259D)
        { 
        }

        public SilverSteedZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073219;
            }
        }// Interactive Silver Steed Contribution Statue
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

    public class QuagmireZooStatuette : CollectionStatuette
    { 
        [Constructable]
        public QuagmireZooStatuette()
            : base(0x2614)
        { 
        }

        public QuagmireZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074848;
            }
        }// Interactive Quagmire Contribution Statue from the Britannia Royal Zoo.
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

    public class BakeKitsuneZooStatuette : CollectionStatuette
    { 
        [Constructable]
        public BakeKitsuneZooStatuette()
            : base(0x2763)
        { 
        }

        public BakeKitsuneZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074849;
            }
        }// Interactive Bake Kitsune Contribution Statue from the Britannia Royal Zoo.
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

    public class DireWolfZooStatuette : CollectionStatuette
    { 
        [Constructable]
        public DireWolfZooStatuette()
            : base(0x25D0)
        { 
        }

        public DireWolfZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073196;
            }
        }// Interactive Dire Wolf Contribution Statue from the Britannia Royal Zoo.
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

    public class CraneZooStatuette : CollectionStatuette
    { 
        [Constructable]
        public CraneZooStatuette()
            : base(0x2764)
        { 
        }

        public CraneZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073197;
            }
        }// An Interactive Crane Contribution Statue from the Britannia Royal Zoo.
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

    public class PolarBearZooStatuette : CollectionStatuette
    { 
        [Constructable]
        public PolarBearZooStatuette()
            : base(0x20E1)
        { 
        }

        public PolarBearZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074851;
            }
        }// Interactive Polar Bear Contribution Statue from the Britannia Royal Zoo.
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

    public class ChangelingZooStatuette : CollectionStatuette
    { 
        [Constructable]
        public ChangelingZooStatuette()
            : base(0x2D8A)
        { 
        }

        public ChangelingZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074850;
            }
        }// Interactive Changeling Contribution Statue from the Britannia Royal Zoo.
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

    public class ReptalonZooStatuette : CollectionStatuette
    { 
        [Constructable]
        public ReptalonZooStatuette()
            : base(0x2D95)
        { 
        }

        public ReptalonZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074852;
            }
        }// Interactive Reptalon Contribution Statue from the Britannia Royal Zoo.
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

    public class SpecialAchievementZooStatuette : CollectionStatuette
    { 
        [Constructable]
        public SpecialAchievementZooStatuette()
            : base(0x2FF6)
        { 
            this.Weight = 10.0;
        }

        public SpecialAchievementZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073226;
            }
        }// Britannia Royal Zoo Special Achievement Award
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