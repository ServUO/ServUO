using System;
using System.Collections;
using Server.Mobiles;
using Server.Targeting;
using Server.Targets;

namespace Server.Items
{
	public class DamageableItem : Item
	{
		public enum ItemLevel
		{
			NotSet,
			VeryEasy,
			Easy,
			Average,
			Hard,
			VeryHard,
			Insane
		}

		private int m_Hits;
		private int m_HitsMax;
		private int m_StartID;
		private int m_DestroyedID;
		private int m_HalfHitsID;
		private ItemLevel m_ItemLevel;
		private IDamageableItem m_Child;

		[CommandProperty(AccessLevel.GameMaster)]
		public IDamageableItem Link
		{
			get
			{
				return this.m_Child;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public ItemLevel Level
		{
			get
			{
				return this.m_ItemLevel;
			}
			set
			{
				this.m_ItemLevel = value;

				double bonus = (double)(((int)this.m_ItemLevel * 100.0) * ((int)this.m_ItemLevel * 5));

				this.HitsMax = ((int)(100 + bonus));
				this.Hits = ((int)(100 + bonus));

				this.InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int IDStart
		{
			get
			{
				return this.m_StartID;
			}
			set
			{
				if (value < 0)
					this.m_StartID = 0;
				else if (value > int.MaxValue)
					this.m_StartID = int.MaxValue;
				else
					this.m_StartID = value;

				if (this.m_Hits >= (this.m_HitsMax * 0.5))
				{
					if (this.ItemID != this.m_StartID)
						this.ItemID = this.m_StartID;
				}

				this.InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int IDHalfHits
		{
			get
			{
				return this.m_HalfHitsID;
			}
			set
			{
				if (value < 0)
					this.m_HalfHitsID = 0;
				else if (value > int.MaxValue)
					this.m_HalfHitsID = int.MaxValue;
				else
					this.m_HalfHitsID = value;

				if (this.m_Hits < (this.m_HitsMax * 0.5))
				{
					if (this.ItemID != this.m_HalfHitsID)
						this.ItemID = this.m_HalfHitsID;
				}

				this.InvalidateProperties();
			}
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int Hits
		{
			get
			{
				return this.m_Hits;
			}
			set
			{
				if (value > this.m_HitsMax)
					this.m_Hits = this.m_HitsMax;
				else
					this.m_Hits = value;

				if (this.m_Child != null && (this.m_Hits > this.m_Child.Hits || this.m_Hits < this.m_Child.Hits))
					this.UpdateHitsToEntity();

				this.InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int HitsMax
		{
			get
			{
				return this.m_HitsMax;
			}
			set
			{
				if (value > int.MaxValue)
					this.m_HitsMax = int.MaxValue;
				else
					this.m_HitsMax = value;

				if (this.Hits > this.m_HitsMax)
					this.Hits = this.m_HitsMax;

				if (this.m_Child != null && (this.m_HitsMax > this.m_Child.HitsMax || this.m_HitsMax < this.m_Child.HitsMax))
					this.UpdateMaxHitsToEntity();

				this.InvalidateProperties();
			}
		}

		[Constructable]
		public DamageableItem(int StartID, int HalfID)
			: base(StartID)
		{
			this.Name = "Damageable Item";
			this.Hue = 0;
			this.Movable = true;

			this.Level = ItemLevel.NotSet;

			this.IDStart = StartID;
			this.IDHalfHits = HalfID;
		}

		public virtual void OnDamage(int amount, Mobile from, bool willKill)
		{
			return;
		}

		public virtual bool OnBeforeDestroyed()
		{
			return true;
		}

		public virtual void OnDestroyed(WoodenBox lootbox)
		{
			return;
		}

		public void UpdateMaxHitsToEntity()
		{
			this.m_Child.SetHits(this.HitsMax);
		}

		public void UpdateHitsToEntity()
		{
			this.m_Child.Hits = this.Hits;
		}

		public void Damage(int amount, Mobile from, bool willKill)
		{
			if (willKill)
			{
				this.Destroy();
				return;
			}

			this.Hits -= amount;

			if (this.Hits >= (this.HitsMax * 0.5))
			{
				this.ItemID = this.IDStart;
			}
			else if (this.Hits < (this.HitsMax * 0.5))
			{
				this.ItemID = this.IDHalfHits;
			}
			else if (this.Hits <= 0)
			{
				this.Destroy();
				return;
			}

			this.OnDamage(amount, from, willKill);
		}

		public bool Destroy()
		{
			if (this == null || this.Deleted)
				return false;

			if (this.OnBeforeDestroyed())
			{
				if (this.m_Child != null && !this.m_Child.Deleted && !this.m_Child.Alive)
				{
					if (this.m_Child != null)
						this.m_Child.Delete();

					this.Delete();
					return true;
				}
				else
				{
					this.Delete();
					return true;
				}
			}

			return false;
		}

		//Provides the Parent Item (this) with a new Entity Link
		private void ProvideEntity()
		{
			if (this.m_Child != null)
			{
				this.m_Child.Delete();
			}

			IDamageableItem Idam = new IDamageableItem(this);

			if (Idam != null && !Idam.Deleted && this.Map != null)
			{
				this.m_Child = Idam;
				this.m_Child.Update();
			}
		}

		//If the child Link is not at our location, bring it back!
		public override void OnLocationChange(Point3D oldLocation)
		{
			if (this.Location != oldLocation)
			{
				if (this.m_Child != null && !this.m_Child.Deleted)
				{
					if (this.m_Child.Location == oldLocation)
						this.m_Child.Update();
				}
			}

			base.OnLocationChange(oldLocation);
		}

		//This Wraps the target and resets the targeted ITEM to it's child BASECREATURE
		//Unfortunately, there is no accessible Array for a player's followers,
		//so we must use the GetMobilesInRage(int range) method for our reference checks!
		public override bool CheckTarget(Mobile from, Target targ, object targeted)
		{
			#region CheckEntity
			//Check to see if we have an Entity Link (Child BaseCreature)
			//If not, create one!
			//(Without Combatant Change, since this is for pets)
			PlayerMobile pm = from as PlayerMobile;

			if (pm != null)
			{
				if (this.m_Child != null && !this.m_Child.Deleted)
				{
					this.m_Child.Update();
				}
				else
				{
					this.ProvideEntity();

					if (this.m_Child != null && !this.m_Child.Deleted)
					{
						this.m_Child.Update();
					}
				}
			}
			#endregion
		
			if (targ is AIControlMobileTarget && targeted == this)
			{
				//Wrap the target
				AIControlMobileTarget t = targ as AIControlMobileTarget;
				//Get the OrderType
				OrderType order = t.Order;

				//Search for our controlled pets within screen range
				foreach (Mobile m in from.GetMobilesInRange(16))
				{
					if (!(m is BaseCreature))
						continue;

					BaseCreature bc = m as BaseCreature;

					if (from != null && !from.Deleted && from.Alive)
					{
						if (bc == null || bc.Deleted || !bc.Alive || !bc.Controlled || bc.ControlMaster != from)
							continue;

						//Reset the pet's ControlTarget and OrderType.
						bc.ControlTarget = this.m_Child;
						bc.ControlOrder = t.Order;
					}
				}
			}

			return base.CheckTarget(from, targ, targeted);
		}

		public override void OnDoubleClick(Mobile from)
		{
			#region CheckEntity
			//Check to see if we have an Entity Link (Child BaseCreature)
			//If not, create one!
			//(With Combatant change to simulate Attacking)
			PlayerMobile pm = from as PlayerMobile;

			if (pm != null)
			{
				if (this.m_Child != null && !this.m_Child.Deleted)
				{
					this.m_Child.Update();
					pm.Warmode = true;
					pm.Combatant = this.m_Child;
				}
				else
				{
					this.ProvideEntity();

					if (this.m_Child != null && !this.m_Child.Deleted)
					{
						this.m_Child.Update();
						pm.Warmode = true;
						pm.Combatant = this.m_Child;
					}
				}
			}
			#endregion

			base.OnDoubleClick(from);
		}

		public override bool OnDragLift(Mobile from)
		{
			return (from.IsStaff());
		}

		public override void Delete()
		{
			base.Delete();

			if (this.m_Child != null && !this.m_Child.Deleted)
			{
				this.m_Child.Delete();
				return;
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			ArrayList strings = new ArrayList();

			strings.Add("-Strength-");
			strings.Add(this.m_Hits + "/" + this.m_HitsMax);

			string toAdd = "";
			int amount = strings.Count;
			int current = 1;

			foreach (string str in strings)
			{
				toAdd += str;

				if (current != amount)
					toAdd += "\n";

				++current;
			}

			if (toAdd != "")
				list.Add(1070722, toAdd);
		}

		public DamageableItem(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version

			writer.Write((Mobile)this.m_Child);
			writer.Write((int)this.m_StartID);
			writer.Write((int)this.m_HalfHitsID);
			writer.Write((int)this.m_DestroyedID);
			writer.Write((int)this.m_ItemLevel);
			writer.Write((int)this.m_Hits);
			writer.Write((int)this.m_HitsMax);
			writer.Write((bool)this.Movable);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			this.m_Child = (IDamageableItem)reader.ReadMobile();
			this.m_StartID = (int)reader.ReadInt();
			this.m_HalfHitsID = (int)reader.ReadInt();
			this.m_DestroyedID = (int)reader.ReadInt();
			this.m_ItemLevel = (ItemLevel)reader.ReadInt();
			this.m_Hits = (int)reader.ReadInt();
			this.m_HitsMax = (int)reader.ReadInt();
			this.Movable = (bool)reader.ReadBool();
		}
	}

	public class IDamageableItem : BaseCreature
	{
		private DamageableItem m_Parent;

		[CommandProperty(AccessLevel.GameMaster)]
		public DamageableItem Link
		{
			get
			{
				return this.m_Parent;
			}
		}

		[Constructable]
		public IDamageableItem(DamageableItem parent)
			: base(AIType.AI_Melee, FightMode.None, 1, 1, 0.2, 0.4)
		{
			if (parent != null && !parent.Deleted)
				this.m_Parent = parent;
				
			//Nullify the name, so it doeesn't pop up when we come into range
			this.Name = null;

			this.Body = 803; //Mustache is barely visible!
			this.BodyValue = 803; //Mustache is barely visible!
			this.Hue = 0;
			this.BaseSoundID = 0; //QUIET!!!
			this.Fame = 0;
			this.Karma = 0;
			this.ControlSlots = 0;
			this.Tamable = false;

			this.Frozen = true;
			this.Paralyzed = true;
			this.CantWalk = true;

			this.DamageMin = 0;
			this.DamageMax = 0;

			this.SetStr(this.m_Parent.HitsMax);
			this.SetHits(this.m_Parent.HitsMax);
			this.Hits = this.m_Parent.Hits;

			for (int skill = 0; skill < this.Skills.Length; skill++)
			{
				this.Skills[(SkillName)skill].Cap = 1.0 + ((int)this.m_Parent.Level * 0.05);
				this.Skills[(SkillName)skill].Base = 1.0 + ((int)this.m_Parent.Level * 0.05);
			}

			this.Update();
		}

		public override Poison PoisonImmune
		{
			get
			{
				return Poison.Lethal;
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			ArrayList strings = new ArrayList();

			strings.Add("-Strength-");
			strings.Add(this.m_Parent.Hits + "/" + this.m_Parent.HitsMax);

			string toAdd = "";
			int amount = strings.Count;
			int current = 1;

			foreach (string str in strings)
			{
				toAdd += str;

				if (current != amount)
					toAdd += "\n";

				++current;
			}

			if (toAdd != "")
				list.Add(1070722, toAdd);
		}

		public void Update()
		{
			if (this == null || this.Deleted)
				return;

			if (this.m_Parent != null && !this.m_Parent.Deleted)
			{
				this.Home = this.m_Parent.Location;
				this.Location = this.m_Parent.Location;
				this.Map = this.m_Parent.Map;

				return;
			}

			if (this.m_Parent == null || this.m_Parent.Deleted)
			{
				this.Delete();
				return;
			}
		}

		public override void Delete()
		{
			base.Delete();
		}

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			base.OnDamage(amount, from, willKill);

			if (this.m_Parent != null && !this.m_Parent.Deleted)
			{
				this.m_Parent.Damage(amount, from, willKill);
			}
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			if (this.m_Parent != null && !this.m_Parent.Deleted)
				this.m_Parent.Destroy();
		}

		public IDamageableItem(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);

			writer.Write((Item)this.m_Parent);
			writer.Write((bool)this.Frozen);
			writer.Write((bool)this.Paralyzed);
			writer.Write((bool)this.CantWalk);
			writer.Write((int)this.DamageMin);
			writer.Write((int)this.DamageMax);
			writer.Write((int)this.BodyValue);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			this.m_Parent = (DamageableItem)reader.ReadItem();
			this.Frozen = (bool)reader.ReadBool();
			this.Paralyzed = (bool)reader.ReadBool();
			this.CantWalk = (bool)reader.ReadBool();
			this.DamageMin = (int)reader.ReadInt();
			this.DamageMax = (int)reader.ReadInt();
			this.BodyValue = (int)reader.ReadInt();
		}
	}
}