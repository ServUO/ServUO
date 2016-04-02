using System;

namespace Server.Items
{
    public class BoneCouchComponent : AddonComponent
    {
        public BoneCouchComponent(int itemID)
            : base(itemID)
        {
        }

        public BoneCouchComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074477;
            }
        }// Bone couch
        public override bool OnMoveOver(Mobile m)
        {
            bool allow = base.OnMoveOver(m);

            if (allow && m.Alive && m.Player && (m.IsPlayer() || !m.Hidden))
                Effects.PlaySound(this.Location, this.Map, Utility.RandomMinMax(0x547, 0x54A));

            return allow;
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
    }

    [FlipableAddon(Direction.South, Direction.East)]
    public class BoneCouchAddon : BaseAddon
    {
        [Constructable]
        public BoneCouchAddon()
            : base()
        {
            this.Direction = Direction.South;

            this.AddComponent(new BoneCouchComponent(0x2A5A), 0, 0, 0);
            this.AddComponent(new BoneCouchComponent(0x2A5B), -1, 0, 0);
        }

        public BoneCouchAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new BoneCouchDeed();
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

        public virtual void Flip(Mobile from, Direction direction)
        {
            switch ( direction )
            {
                case Direction.East:
                    this.AddComponent(new BoneCouchComponent(0x2A80), 0, 0, 0);
                    this.AddComponent(new BoneCouchComponent(0x2A7F), 0, 1, 0);
                    break;
                case Direction.South:
                    this.AddComponent(new BoneCouchComponent(0x2A5A), 0, 0, 0);
                    this.AddComponent(new BoneCouchComponent(0x2A5B), -1, 0, 0);
                    break;
            }
        }
    }

    public class BoneCouchDeed : BaseAddonDeed
    {
        [Constructable]
        public BoneCouchDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public BoneCouchDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new BoneCouchAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074477;
            }
        }// Bone couch
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