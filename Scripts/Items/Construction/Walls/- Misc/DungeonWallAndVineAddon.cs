using System;
using Server.Mobiles;

namespace Server.Items
{
    public class DungeonWallAndVineAddon : BaseAddon
    {
        [ Constructable ]
        public DungeonWallAndVineAddon()
        {
            this.AddComponent(new MagicVinesComponent(), 1, 0, 0);
            this.AddComponent(new DungeonWallComponent(), 0, 0, 0);
        }

        public DungeonWallAndVineAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DungeonWallComponent : AddonComponent
    {
        [Constructable]
        public DungeonWallComponent()
            : base(0x0242)
        {
        }

        public DungeonWallComponent(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (from.X > this.X)
            {
                from.SendLocalizedMessage(1111659); // You try to examine the strange wall but the vines get in your way.
            }
            else
            {
                this.Z += -22;
                Timer.DelayCall(TimeSpan.FromSeconds(15.0), delegate()
                {
                    this.Z += 22;
                });
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (this.Parent == null && Utility.InRange(this.Location, m.Location, 3) && !Utility.InRange(this.Location, oldLocation, 3) && m is PlayerMobile)
            {
                if (m.X > this.X)
                    m.SendLocalizedMessage(1111665); // You notice something odd about the vines covering the wall.
            }
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