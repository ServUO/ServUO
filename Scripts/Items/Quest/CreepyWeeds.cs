using System;
using Server;
using Server.Network;
using Server.Regions;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
    public class CreepyWeeds : Item
    {
        [Constructable]
        public CreepyWeeds()
            : base(0x0CB8)
        {
            Name = "Creepy Weeds";
            Weight = 1;
            Movable = false;

            Timer.DelayCall(TimeSpan.FromMinutes(10.0), delegate()
            {
                Delete();
            });
        }

        public CreepyWeeds(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!CheckUse(from))
                return;

            Map map = Map;
            Point3D loc = Location;

            if (map != null && map != Map.Internal && from.InRange(loc, 1) && from.InLOS(this))
            {
                Delete();

                switch (Utility.Random(18))
                {
                    case 0:
                        new Snake().MoveToWorld(loc, map);
                        break;
                    case 1:
                        new Mongbat().MoveToWorld(loc, map);
                        break;
                    case 2:
                        new SilverSerpent().MoveToWorld(loc, map);
                        break;
                    case 3:
                        new Raptor().MoveToWorld(loc, map);
                        break;
                    case 4:
                        new Ballem().MoveToWorld(loc, map);
                        break;
                    case 5: case 10: case 15:
						if (Utility.RandomDouble() < 0.20)
                        {
                            new FNPitchfork().MoveToWorld(loc, map);
                            from.SendMessage("You find Farmer Nash's pitchfork under one of the brambles of weeds. You pick up the pitchfork and put it in your backpack."); 
                        }
						break;
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

            if (Deleted || !IsAccessibleTo(from))
            {
                return false;
            }
            else if (from.Map != Map || !from.InRange(GetWorldLocation(), 1))
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