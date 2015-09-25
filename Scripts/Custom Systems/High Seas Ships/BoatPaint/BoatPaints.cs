using System;
using Server;

namespace Server.Items
{
    public class GMBoatPaint : BoatPaint
    {
        public override bool UsesCharges { get { return false; } }

        public override bool AllowWood { get { return true; } } //Wood walls and doors
        public override bool AllowStone { get { return true; } } //Stone walls and doors
        public override bool AllowMarble { get { return true; } } //Marble walls and doors
        public override bool AllowPlaster { get { return true; } } //Plaster and clay walls and doors
        public override bool AllowSandstone { get { return true; } } //Sandstone walls and doors
        public override bool AllowOther { get { return true; } } //Hide, Paper, Bamboo or Rattan walls and doors

        public override bool AllowRepaint { get { return true; } }

        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.SpecialDyeTub; } }

        [Constructable]
        public GMBoatPaint()
        {
            Name = "GM Boat Paint";
        }

        public GMBoatPaint(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel < AccessLevel.GameMaster)
                return;

            base.OnDoubleClick(from);
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
	
    public class GreenBoatPaint : BoatPaint
    {
        public override bool UsesCharges { get { return true; } }

        public override bool AllowWood { get { return false; } } //Wood walls and doors
        public override bool AllowStone { get { return false; } } //Stone walls and doors
        public override bool AllowMarble { get { return false; } } //Marble walls and doors
        public override bool AllowPlaster { get { return false; } } //Plaster and clay walls and doors
        public override bool AllowSandstone { get { return false; } } //Sandstone walls and doors
        public override bool AllowOther { get { return false; } } //Hide, Paper, Bamboo or Rattan walls and doors

        public override bool AllowRepaint { get { return true; } }

        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.SpecialDyeTub; } }

		public override int DyedHue { get { return 1420; } }
		
        [Constructable]
        public GreenBoatPaint()
        {
            Name = "Boat Paint";
			Hue = 1420;
        }

        public GreenBoatPaint(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {

            base.OnDoubleClick(from);
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
	
    public class BlueBoatPaint : BoatPaint
    {
        public override bool UsesCharges { get { return true; } }

        public override bool AllowWood { get { return false; } } //Wood walls and doors
        public override bool AllowStone { get { return false; } } //Stone walls and doors
        public override bool AllowMarble { get { return false; } } //Marble walls and doors
        public override bool AllowPlaster { get { return false; } } //Plaster and clay walls and doors
        public override bool AllowSandstone { get { return false; } } //Sandstone walls and doors
        public override bool AllowOther { get { return false; } } //Hide, Paper, Bamboo or Rattan walls and doors

        public override bool AllowRepaint { get { return true; } }

        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.SpecialDyeTub; } }

		public override int DyedHue { get { return 1303; } }
		
        [Constructable]
        public BlueBoatPaint()
        {
            Name = "Boat Paint";
			Hue = 1303;
        }

        public BlueBoatPaint(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {

            base.OnDoubleClick(from);
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
	
    public class PurpleBoatPaint : BoatPaint
    {
        public override bool UsesCharges { get { return true; } }

        public override bool AllowWood { get { return false; } } //Wood walls and doors
        public override bool AllowStone { get { return false; } } //Stone walls and doors
        public override bool AllowMarble { get { return false; } } //Marble walls and doors
        public override bool AllowPlaster { get { return false; } } //Plaster and clay walls and doors
        public override bool AllowSandstone { get { return false; } } //Sandstone walls and doors
        public override bool AllowOther { get { return false; } } //Hide, Paper, Bamboo or Rattan walls and doors

        public override bool AllowRepaint { get { return true; } }

        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.SpecialDyeTub; } }

		public override int DyedHue { get { return 1230; } }
		
        [Constructable]
        public PurpleBoatPaint()
        {
            Name = "Boat Paint";
			Hue = 1230;
        }

        public PurpleBoatPaint(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {

            base.OnDoubleClick(from);
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

    public class BrownBoatPaint : BoatPaint
    {
        public override bool UsesCharges { get { return true; } }

        public override bool AllowWood { get { return false; } } //Wood walls and doors
        public override bool AllowStone { get { return false; } } //Stone walls and doors
        public override bool AllowMarble { get { return false; } } //Marble walls and doors
        public override bool AllowPlaster { get { return false; } } //Plaster and clay walls and doors
        public override bool AllowSandstone { get { return false; } } //Sandstone walls and doors
        public override bool AllowOther { get { return false; } } //Hide, Paper, Bamboo or Rattan walls and doors

        public override bool AllowRepaint { get { return true; } }

        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.SpecialDyeTub; } }

		public override int DyedHue { get { return 1501; } }
		
        [Constructable]
        public BrownBoatPaint()
        {
            Name = "Boat Paint";
			Hue = 1501;
        }

        public BrownBoatPaint(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {

            base.OnDoubleClick(from);
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
	
    public class MaronBoatPaint : BoatPaint
    {
        public override bool UsesCharges { get { return true; } }

        public override bool AllowWood { get { return false; } } //Wood walls and doors
        public override bool AllowStone { get { return false; } } //Stone walls and doors
        public override bool AllowMarble { get { return false; } } //Marble walls and doors
        public override bool AllowPlaster { get { return false; } } //Plaster and clay walls and doors
        public override bool AllowSandstone { get { return false; } } //Sandstone walls and doors
        public override bool AllowOther { get { return false; } } //Hide, Paper, Bamboo or Rattan walls and doors

        public override bool AllowRepaint { get { return true; } }

        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.SpecialDyeTub; } }

		public override int DyedHue { get { return 2013; } }
		
        [Constructable]
        public MaronBoatPaint()
        {
            Name = "Boat Paint";
			Hue = 2013;
        }

        public MaronBoatPaint(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {

            base.OnDoubleClick(from);
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

    public class RoseBoatPaint : BoatPaint
    {
        public override bool UsesCharges { get { return true; } }

        public override bool AllowWood { get { return false; } } //Wood walls and doors
        public override bool AllowStone { get { return false; } } //Stone walls and doors
        public override bool AllowMarble { get { return false; } } //Marble walls and doors
        public override bool AllowPlaster { get { return false; } } //Plaster and clay walls and doors
        public override bool AllowSandstone { get { return false; } } //Sandstone walls and doors
        public override bool AllowOther { get { return false; } } //Hide, Paper, Bamboo or Rattan walls and doors

        public override bool AllowRepaint { get { return true; } }

        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.SpecialDyeTub; } }

		public override int DyedHue { get { return 1619; } }
		
        [Constructable]
        public RoseBoatPaint()
        {
            Name = "Boat Paint";
			Hue = 1619;
        }

        public RoseBoatPaint(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {

            base.OnDoubleClick(from);
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
	
    public class RedBoatPaint : BoatPaint
    {
        public override bool UsesCharges { get { return true; } }

        public override bool AllowWood { get { return false; } } //Wood walls and doors
        public override bool AllowStone { get { return false; } } //Stone walls and doors
        public override bool AllowMarble { get { return false; } } //Marble walls and doors
        public override bool AllowPlaster { get { return false; } } //Plaster and clay walls and doors
        public override bool AllowSandstone { get { return false; } } //Sandstone walls and doors
        public override bool AllowOther { get { return false; } } //Hide, Paper, Bamboo or Rattan walls and doors

        public override bool AllowRepaint { get { return true; } }

        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.SpecialDyeTub; } }

		public override int DyedHue { get { return 1640; } }
		
        [Constructable]
        public RedBoatPaint()
        {
            Name = "Boat Paint";
			Hue = 1640;
        }

        public RedBoatPaint(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {

            base.OnDoubleClick(from);
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

    public class OliveBoatPaint : BoatPaint
    {
        public override bool UsesCharges { get { return true; } }

        public override bool AllowWood { get { return false; } } //Wood walls and doors
        public override bool AllowStone { get { return false; } } //Stone walls and doors
        public override bool AllowMarble { get { return false; } } //Marble walls and doors
        public override bool AllowPlaster { get { return false; } } //Plaster and clay walls and doors
        public override bool AllowSandstone { get { return false; } } //Sandstone walls and doors
        public override bool AllowOther { get { return false; } } //Hide, Paper, Bamboo or Rattan walls and doors

        public override bool AllowRepaint { get { return true; } }

        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.SpecialDyeTub; } }

		public override int DyedHue { get { return 2001; } }
		
        [Constructable]
        public OliveBoatPaint()
        {
            Name = "Boat Paint";
			Hue = 2001;
        }

        public OliveBoatPaint(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {

            base.OnDoubleClick(from);
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