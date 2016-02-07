using System;
using Server;
using Server.Targeting;
using Server.Network;
using System.Collections;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
	public abstract class SiegeLog : BaseSiegeProjectile
	{
		public override double MobDamageMultiplier { get { return 0.1; } } // default damage multiplier for creatures

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
			Range = 4;
			Area = 0;
			AccuracyBonus = 0;
			PhysicalDamage = 80;
			FireDamage = 0;
			FiringSpeed = 35;
			Name = "Light Siege Log";
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
			Range = 4;
			Area = 0;
			AccuracyBonus = 0;
			PhysicalDamage = 100;
			FireDamage = 0;
			FiringSpeed = 25;
			Name = "Heavy Siege Log";
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
			Range = 4;
			Area = 0;
			AccuracyBonus = 0;
			PhysicalDamage = 120;
			FireDamage = 0;
			FiringSpeed = 15;
			Name = "Iron Siege Log";
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
