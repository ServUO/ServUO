using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class PaintCan : Item
    {
        private int _uses;		
        private int _dyedHue;
        private string Brand = "Paint Can";
        public override int LabelNumber { get { return 1016211; } }

        public virtual CustomHuePicker CustomHuePicker { get { return null; } }

        public virtual bool AllowRepaint { get { return false; } }
        public virtual bool AllowHouse { get { return false; } }
        public virtual bool UsesCharges { get { return true; } }

        public virtual bool AllowWood { get { return false; } } //Wood walls and doors
        public virtual bool AllowStone { get { return false; } } //Stone walls and doors
        public virtual bool AllowMarble { get { return false; } } //Marble walls and doors
        public virtual bool AllowPlaster { get { return false; } } //Plaster and clay walls and doors
        public virtual bool AllowSandstone { get { return false; } } //Sandstone walls and doors
        public virtual bool AllowOther { get { return false; } } //Hide, Paper, Bamboo or Rattan walls and doors

        public override bool DisplayWeight { get { return false; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)_uses);
            writer.Write((bool)Redyable);
            writer.Write((int)_dyedHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        _uses = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        Redyable = reader.ReadBool();
                        _dyedHue = reader.ReadInt();

                        break;
                    }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(Brand);
            list.Add(Name);
            if (UsesCharges)
            {
                if (_uses > 0) list.Add("Uses left: {0}", _uses);
                else list.Add("** Empty **");
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Uses { get { return _uses; } set { _uses = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Redyable { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DyedHue { get { return _dyedHue; } set { if (Redyable) { _dyedHue = value; Hue = value; } } }

        [Constructable]
        public PaintCan()
            : this(1)
        {
        }

        [Constructable]
        public PaintCan(int uses)
            : base(0xFAB)
        {
            _uses = uses;
            Weight = 6.0;
            Redyable = true;
        }

        public PaintCan(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage("Target what you want to paint.");
                from.Target = new InternalTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        private class InternalTarget : Target
        {
            private PaintCan _can;

            public InternalTarget(PaintCan can)
                : base(3, false, TargetFlags.None)
            {
                _can = can;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                bool GM = from.AccessLevel >= AccessLevel.GameMaster;

                //IPoint3D p3d;
                int id = 0;
                if (_can.UsesCharges && _can.Uses < 1)
                {
                    from.SendMessage("Your paint can is empty.");
                    return;
                }
				//if (from == BaseSmoothMulti.Owner)
				//{
						if (targeted is BaseSmoothMulti)
						{
							BaseSmoothMulti target = targeted as BaseSmoothMulti;
							id = target.ItemID;
							target.Hue = _can.DyedHue;
							from.PlaySound(0x23E);
							if (_can.UsesCharges)
								_can.Uses--;
							//from.SendMessage("TEMP: Successfully painted over BaseSmoothMulti.");

						}
						else
							if (targeted is MainMast)
							{
								MainMast target = targeted as MainMast;
								id = target.ItemID;
								target.Hue = _can.DyedHue;
								target.Ship.Hue = _can.DyedHue;
								from.PlaySound(0x23E);
								if (_can.UsesCharges)
									_can.Uses--;
							}
							else
								from.SendMessage("Unable to paint that using this type of paint.");
						
                 //}
                 //else
                 //   from.SendMessage("You must be standing in, and targeting a boat you own.");
                
            }
        }
    }
}