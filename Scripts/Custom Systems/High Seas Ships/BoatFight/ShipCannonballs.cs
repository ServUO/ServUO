using System;

namespace Server.Items
{
    public abstract class ShipCannonball : BaseShipProjectile
    {
        public ShipCannonball()
            : this(1)
        {
        }

        public ShipCannonball(int amount)
            : base(amount, 0xE74)
        {
        }

        public ShipCannonball(Serial serial)
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

    public class LightShipCannonball : ShipCannonball
    {
        [Constructable]
        public LightShipCannonball()
            : this(1)
        {
        }

        [Constructable]
        public LightShipCannonball(int amount)
            : base(amount)
        {
            Range = 17;
            Area = 0;
            AccuracyBonus = 0;
            PhysicalDamage = 1600;
            FireDamage = 0;
            FiringSpeed = 35;
            Name = "Light Ship Cannonball";
        }

        public LightShipCannonball(Serial serial)
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
        /*
        public override Item Dupe(int amount)
        {
        LightCannonball s = new LightCannonball(amount);
        return this.Dupe(s, amount);
        }
        * */
    }

    public class IronShipCannonball : ShipCannonball
    {
        [Constructable]
        public IronShipCannonball()
            : this(1)
        {
        }

        [Constructable]
        public IronShipCannonball(int amount)
            : base(amount)
        {
            Range = 15;
            Area = 0;
            AccuracyBonus = 0;
            PhysicalDamage = 4500;
            FireDamage = 0;
            FiringSpeed = 25;
            Name = "Iron Ship Cannonball";
        }

        public IronShipCannonball(Serial serial)
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
        /*
        public override Item Dupe(int amount)
        {
        IronCannonball s = new IronCannonball(amount);
        return this.Dupe(s, amount);
        }
        * */
    }

    public class ExplodingShipCannonball : ShipCannonball
    {
        [Constructable]
        public ExplodingShipCannonball()
            : this(1)
        {
        }

        [Constructable]
        public ExplodingShipCannonball(int amount)
            : base(amount)
        {
            Range = 11;
            Area = 1;
            AccuracyBonus = -10;
            PhysicalDamage = 300;
            FireDamage = 1250;
            FiringSpeed = 20;
            Hue = 46;
            Name = "Exploding Ship Cannonball";
        }

        public ExplodingShipCannonball(Serial serial)
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
        /*
        public override Item Dupe(int amount)
        {
        ExplodingCannonball s = new ExplodingCannonball(amount);
        return this.Dupe(s, amount);
        }
        * */
    }

    public class FieryShipCannonball : ShipCannonball
    {
        [Constructable]
        public FieryShipCannonball()
            : this(1)
        {
        }

        [Constructable]
        public FieryShipCannonball(int amount)
            : base(amount)
        {
            Range = 8;
            Area = 2;
            AccuracyBonus = -20;
            PhysicalDamage = 0;
            FireDamage = 2500;
            FiringSpeed = 10;
            Hue = 33;
            Name = "Fiery Ship Cannonball";
        }

        public FieryShipCannonball(Serial serial)
            : base(serial)
        {
        }

        // use a fireball animation when fired
        public override int AnimationID
        {
            get
            {
                return 0x36D4;
            }
        }
        public override int AnimationHue
        {
            get
            {
                return 0;
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
        /*
        public override Item Dupe(int amount)
        {
        FieryCannonball s = new FieryCannonball(amount);
        return this.Dupe(s, amount);
        }
        * */
    }

    public class ShipGrapeShot : ShipCannonball
    {
        [Constructable]
        public ShipGrapeShot()
            : this(1)
        {
        }

        [Constructable]
        public ShipGrapeShot(int amount)
            : base(amount)
        {
            Range = 17;
            Area = 1;
            AccuracyBonus = 0;
            PhysicalDamage = 1800;
            FireDamage = 0;
            FiringSpeed = 35;
            Name = "Ship Grape Shot";
        }

        public ShipGrapeShot(Serial serial)
            : base(serial)
        {
        }

        // only does damage to mobiles
        public override double StructureDamageMultiplier
        {
            get
            {
                return 0.0;
            }
        }//  damage multiplier for structures
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