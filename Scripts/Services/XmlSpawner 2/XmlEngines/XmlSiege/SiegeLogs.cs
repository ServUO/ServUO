using System;

namespace Server.Items
{
    public abstract class SiegeLog : BaseSiegeProjectile
    {
        public SiegeLog()
            : this(1)
        {
        }

        public SiegeLog(int amount)
            : base(amount, 0x1BDE)
        {
        }

        public SiegeLog(Serial serial)
            : base(serial)
        {
        }

        public override double MobDamageMultiplier
        {
            get
            {
                return 0.1;
            }
        }// default damage multiplier for creatures
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

    public class LightSiegeLog : SiegeLog
    {
        [Constructable]
        public LightSiegeLog()
            : this(1)
        {
        }

        [Constructable]
        public LightSiegeLog(int amount)
            : base(amount)
        {
            this.Range = 4;
            this.Area = 0;
            this.AccuracyBonus = 0;
            this.PhysicalDamage = 80;
            this.FireDamage = 0;
            this.FiringSpeed = 35;
            this.Name = "Light Siege Log";
        }

        public LightSiegeLog(Serial serial)
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
        LightSiegeLog s = new LightSiegeLog(amount);
        return this.Dupe(s, amount);
        }
        * */
    }

    public class HeavySiegeLog : SiegeLog
    {
        [Constructable]
        public HeavySiegeLog()
            : this(1)
        {
        }

        [Constructable]
        public HeavySiegeLog(int amount)
            : base(amount)
        {
            this.Range = 4;
            this.Area = 0;
            this.AccuracyBonus = 0;
            this.PhysicalDamage = 100;
            this.FireDamage = 0;
            this.FiringSpeed = 25;
            this.Name = "Heavy Siege Log";
        }

        public HeavySiegeLog(Serial serial)
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
        HeavySiegeLog s = new HeavySiegeLog(amount);
        return this.Dupe(s, amount);
        }
        * */
    }

    public class IronSiegeLog : SiegeLog
    {
        [Constructable]
        public IronSiegeLog()
            : this(1)
        {
        }

        [Constructable]
        public IronSiegeLog(int amount)
            : base(amount)
        {
            this.Range = 4;
            this.Area = 0;
            this.AccuracyBonus = 0;
            this.PhysicalDamage = 120;
            this.FireDamage = 0;
            this.FiringSpeed = 15;
            this.Name = "Iron Siege Log";
        }

        public IronSiegeLog(Serial serial)
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
}