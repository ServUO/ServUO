using Server.Mobiles;
using System;

namespace Server.Items
{
    public class SecretWall : Item
    {
        [Constructable]
        public SecretWall(int itemID)
            : base(itemID)
        {
            Active = true;
            Locked = true;
        }

        public SecretWall(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D PointDest { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map MapDest { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Locked { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active { get; set; }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 2))
            {
                if (!Locked && Active)
                {
                    BaseCreature.TeleportPets(from, PointDest, MapDest);
                    from.MoveToWorld(PointDest, MapDest);
                    from.SendLocalizedMessage(1072790); // The wall becomes transparent, and you push your way through it.
                }
                else
                    from.Say(502684); // This door appears to be locked.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(PointDest);
            writer.Write(MapDest);
            writer.Write(Locked);
            writer.Write(Active);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            PointDest = reader.ReadPoint3D();
            MapDest = reader.ReadMap();
            Locked = reader.ReadBool();
            Active = reader.ReadBool();
        }
    }

    public class SecretSwitch : Item
    {
        [Constructable]
        public SecretSwitch()
            : this(0x108F, null)
        {
        }

        [Constructable]
        public SecretSwitch(int itemID, SecretWall wall)
            : base(itemID)
        {
            Wall = wall;
        }

        public SecretSwitch(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecretWall Wall { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TurnedOn { get; set; }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 2) && Wall != null)
            {
                if (TurnedOn)
                    ItemID -= 1;
                else
                {
                    ItemID += 1;

                    Timer.DelayCall(TimeSpan.FromSeconds(10), Lock);
                }

                TurnedOn = !TurnedOn;
                Wall.Locked = !Wall.Locked;

                if (Utility.RandomBool())
                {
                    Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x36B0, 1, 14, 63, 7, 9915, 0);
                    Effects.PlaySound(from.Location, from.Map, 0x229);

                    AOS.Damage(from, Utility.Random(4, 5), 0, 0, 0, 100, 0);
                }

                from.SendLocalizedMessage(1072739); // You hear a click behind the wall.
                from.PlaySound(0x3E5);
            }
        }

        public virtual void Lock()
        {
            if (Wall != null)
            {
                if (TurnedOn)
                    ItemID -= 1;

                TurnedOn = false;
                Wall.Locked = true;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(Wall);
            writer.Write(TurnedOn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            Wall = reader.ReadItem() as SecretWall;
            TurnedOn = reader.ReadBool();
        }
    }
}
