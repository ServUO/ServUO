using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class CreepyWeeds : Item
    {
        [Constructable]
        public CreepyWeeds()
            : base(0x0CB8)
        {
            this.Name = "Creepy Weeds";
            this.Weight = 1;		
        }

        public CreepyWeeds(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.CheckUse(from))
                return;
            Map map = this.Map;
            Point3D loc = this.Location;

            if (from.InRange(loc, 1) || from.InLOS(this))
            {
                (this).Delete();

                Snake snake = new Snake();
                Mongbat mongbat = new Mongbat();
                SilverSerpent silverserpent = new SilverSerpent();
                Raptor raptor = new Raptor();
                Ballem ballem = new Ballem();
                FNPitchfork fnpitchfork = new FNPitchfork();

                switch (Utility.Random(6))
                {
                    case 0:
                        snake.MoveToWorld(loc, map);
                        break;
                    case 1:
                        mongbat.MoveToWorld(loc, map);
                        break;
                    case 2:
                        silverserpent.MoveToWorld(loc, map);
                        break;
                    case 3:
                        raptor.MoveToWorld(loc, map);
                        break;
                    case 4:
                        ballem.MoveToWorld(loc, map);
                        break;
                    case 5:
                        if (Utility.RandomDouble() < 0.20)
                        {
                            fnpitchfork.MoveToWorld(loc, map);
                            from.SendMessage("You find Farmer Nash's pitchfork under one of the brambles of weeds. You pick up the pitchfork and put it in your backpack.");
                            break;
                        }
                        else
                        {
                            silverserpent.MoveToWorld(loc, map);
                            break;
                        }
                }
            }
        }

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

        protected virtual bool CheckUse(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (this.Deleted || !this.IsAccessibleTo(from))
            {
                return false;
            }
            else if (from.Map != this.Map || !from.InRange(this.GetWorldLocation(), 1))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}