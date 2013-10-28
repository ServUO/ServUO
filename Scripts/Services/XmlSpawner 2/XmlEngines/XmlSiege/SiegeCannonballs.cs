using System;

namespace Server.Items
{
    public abstract class SiegeCannonball : BaseSiegeProjectile
    {
        public SiegeCannonball()
            : this(1)
        {
        }

        public SiegeCannonball(int amount)
            : base(amount, 0xE74)
        {
        }

        public SiegeCannonball(Serial serial)
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

    public class LightCannonball : SiegeCannonball
    {
        [Constructable]
        public LightCannonball()
            : this(1)
        {
        }

        [Constructable]
        public LightCannonball(int amount)
            : base(amount)
        {
            this.Range = 17;
            this.Area = 0;
            this.AccuracyBonus = 0;
            this.PhysicalDamage = 80;
            this.FireDamage = 0;
            this.FiringSpeed = 35;
            this.Name = "Light Cannonball";
        }

        public LightCannonball(Serial serial)
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

    public class IronCannonball : SiegeCannonball
    {
        [Constructable]
        public IronCannonball()
            : this(1)
        {
        }

        [Constructable]
        public IronCannonball(int amount)
            : base(amount)
        {
            this.Range = 15;
            this.Area = 0;
            this.AccuracyBonus = 0;
            this.PhysicalDamage = 100;
            this.FireDamage = 0;
            this.FiringSpeed = 25;
            this.Name = "Iron Cannonball";
        }

        public IronCannonball(Serial serial)
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

    public class ExplodingCannonball : SiegeCannonball
    {
        [Constructable]
        public ExplodingCannonball()
            : this(1)
        {
        }

        [Constructable]
        public ExplodingCannonball(int amount)
            : base(amount)
        {
            this.Range = 11;
            this.Area = 1;
            this.AccuracyBonus = -10;
            this.PhysicalDamage = 10;
            this.FireDamage = 40;
            this.FiringSpeed = 20;
            this.Hue = 46;
            this.Name = "Exploding Cannonball";
        }

        public ExplodingCannonball(Serial serial)
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

    public class FieryCannonball : SiegeCannonball
    {
        [Constructable]
        public FieryCannonball()
            : this(1)
        {
        }

        [Constructable]
        public FieryCannonball(int amount)
            : base(amount)
        {
            this.Range = 8;
            this.Area = 2;
            this.AccuracyBonus = -20;
            this.PhysicalDamage = 0;
            this.FireDamage = 30;
            this.FiringSpeed = 10;
            this.Hue = 33;
            this.Name = "Fiery Cannonball";
        }

        public FieryCannonball(Serial serial)
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

    public class GrapeShot : SiegeCannonball
    {
        [Constructable]
        public GrapeShot()
            : this(1)
        {
        }

        [Constructable]
        public GrapeShot(int amount)
            : base(amount)
        {
            this.Range = 17;
            this.Area = 1;
            this.AccuracyBonus = 0;
            this.PhysicalDamage = 20;
            this.FireDamage = 0;
            this.FiringSpeed = 35;
            this.Name = "Grape Shot";
        }

        public GrapeShot(Serial serial)
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