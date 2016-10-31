using System;
using System.Collections;
using Server.Mobiles;
using Server.Targeting;
using Server.Targets;

namespace Server.Items
{
	public class DamageableItem : Item, IDamageableItem
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
		private DamagePlaceholder m_Child;

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Link
		{
			get
			{
				return this.m_Child;
			}
            set 
            { }
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

				if (this.m_Hits >= (this.m_HitsMax * IDChange))
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

				if (this.m_Hits < (this.m_HitsMax * IDChange))
				{
					if (this.ItemID != this.m_HalfHitsID)
						this.ItemID = this.m_HalfHitsID;
				}

				this.InvalidateProperties();
			}
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public int IDDestroyed
        {
            get
            {
                return m_DestroyedID;
            }
            set
            {
                if (value < 0 || value > int.MaxValue)
                    m_DestroyedID = -1;
                else
                    m_DestroyedID = value;
            }
        }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int Hits
		{
			get
			{
                if (m_Child != null && m_Hits != m_Child.Hits)
                    m_Hits = m_Child.Hits;

				return this.m_Hits;
			}
			set
			{
				if (value > this.m_HitsMax)
					this.m_Hits = this.m_HitsMax;
				else
					this.m_Hits = value;

				if (this.m_Child != null && this.m_Hits != this.m_Child.Hits)
					this.UpdateHitsToEntity();

                int id = ItemID;

                if (m_Hits >= (m_HitsMax * IDChange) && id != m_StartID)
                {
                    ItemID = m_StartID;
                    OnIDChange(id);
                }
                else if (m_Hits <= (m_HitsMax * IDChange) && id == m_StartID)
                {
                    this.ItemID = m_HalfHitsID;
                    OnIDChange(id);
                }
                else if (m_Hits <= 0)
                {
                    this.Destroy();
                    return;
                }

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

				if (this.m_Child != null && this.m_HitsMax != this.m_Child.HitsMax)
					this.UpdateMaxHitsToEntity();

				this.InvalidateProperties();
			}
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public string PlaceholderName { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Destroyed { get; set; }

        public virtual int HitEffect { get { return 14265; } }
        public virtual int DestroySound { get { return 0x3B3; } }
        public virtual double IDChange { get { return 0.5; } }
        public virtual bool DeleteOnDestroy { get { return true; } }

        public virtual DamagePlaceholder Placeholder { get { return new DamagePlaceholder(this); } }

        public override int PhysicalResistance { get { return 50; } }
        public override int FireResistance { get { return 99; } }
        public override int ColdResistance { get { return 99; } }
        public override int PoisonResistance { get { return 99; } }
        public override int EnergyResistance { get { return 99; } }

        public override bool ForceShowProperties { get { return true; } }

		[Constructable]
		public DamageableItem(int StartID, int HalfID, int destroyID = -1)
			: base(StartID)
		{
			this.Hue = 0;
			this.Movable = false;

			this.Level = ItemLevel.NotSet;

			this.IDStart = StartID;
			this.IDHalfHits = HalfID;
            this.IDDestroyed = destroyID;
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

		public virtual void Damage(int amount, Mobile from, bool willKill)
		{
			if (willKill)
			{
				this.Destroy();
				return;
			}

			this.Hits -= amount;

            if (HitEffect > 0)
                Effects.SendLocationEffect(this.Location, this.Map, HitEffect, 10, 5);

			this.OnDamage(amount, from, willKill);
		}

		public bool Destroy()
		{
			if (this == null || this.Deleted || Destroyed)
				return false;

            Effects.PlaySound(this.Location, this.Map, DestroySound);

            if (this.OnBeforeDestroyed())
            {
                if (this.m_Child != null && !this.m_Child.Deleted && !this.m_Child.Alive)
                    this.m_Child.Delete();

                if (DeleteOnDestroy)
                {
                    this.Delete();
                }
                else if (m_DestroyedID >= 0)
                {
                    ItemID = m_DestroyedID;
                }

                Destroyed = true;
                return true;
            }

			return false;
		}

        public virtual void OnIDChange(int oldID)
        {
        }

        public virtual void OnPlaceholderCreated(BaseCreature bc)
        {
        }

        public void CheckEntity()
        {
            if (m_Child != null && !m_Child.Deleted)
            {
                m_Child.Update();
            }
            else if (!Destroyed)
            {
                this.ProvideEntity();

                if (m_Child != null && !m_Child.Deleted)
                {
                    this.m_Child.Update();
                }
            }
        }

		protected void ProvideEntity()
		{
            if (Destroyed)
                return;

			if (this.m_Child != null)
			{
				this.m_Child.Delete();
			}

            DamagePlaceholder Idam = Placeholder;

			if (Idam != null && !Idam.Deleted && this.Map != null)
			{
				this.m_Child = Idam;
				this.m_Child.Update();
			}
		}

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

        public override void OnMapChange()
        {
            base.OnMapChange();

            if(m_Child != null)
                m_Child.Update();
        }

		public override void OnDoubleClick(Mobile from)
		{
            CheckEntity();

            if (from.InRange(Location, 12) && from.InLOS(this) && from.CanBeHarmful(m_Child, true, false))
            {
                from.Combatant = m_Child;
            }
            else
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
            writer.Write(Destroyed);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			this.m_Child = (DamagePlaceholder)reader.ReadMobile();
			this.m_StartID = (int)reader.ReadInt();
			this.m_HalfHitsID = (int)reader.ReadInt();
			this.m_DestroyedID = (int)reader.ReadInt();
			this.m_ItemLevel = (ItemLevel)reader.ReadInt();
			this.m_Hits = (int)reader.ReadInt();
			this.m_HitsMax = (int)reader.ReadInt();
			this.Movable = (bool)reader.ReadBool();
            Destroyed = reader.ReadBool();
		}
	}

	public class DamagePlaceholder : BaseCreature
	{
		private DamageableItem m_Parent;

		[CommandProperty(AccessLevel.GameMaster)]
		public DamageableItem Link
		{
			get
			{
				return this.m_Parent;
			}
            set
            {
            }
		}

        public override bool CanRegenHits { get { return false; } }
        public override bool DeleteCorpseOnDeath { get { return true; } }
        public override bool BleedImmune { get { return true; } }
        public override bool BardImmune { get { return false; } }

		public DamagePlaceholder(DamageableItem parent)
			: base(AIType.AI_Melee, FightMode.None, 1, 1, 0.2, 0.4)
		{
			if (parent != null && !parent.Deleted)
				this.m_Parent = parent;

            if (!String.IsNullOrEmpty(parent.PlaceholderName))
                Name = parent.PlaceholderName;
            else
			    Name = parent.Name;

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

            this.SetResistance(ResistanceType.Physical, parent.PhysicalResistance);
            this.SetResistance(ResistanceType.Fire, parent.FireResistance);
            this.SetResistance(ResistanceType.Cold, parent.ColdResistance);
            this.SetResistance(ResistanceType.Poison, parent.PoisonResistance);
            this.SetResistance(ResistanceType.Energy, parent.EnergyResistance);

			for (int skill = 0; skill < this.Skills.Length; skill++)
			{
				this.Skills[(SkillName)skill].Cap = 1.0 + ((int)this.m_Parent.Level * 0.05);
				this.Skills[(SkillName)skill].Base = 1.0 + ((int)this.m_Parent.Level * 0.05);
			}

            parent.OnPlaceholderCreated(this);

			this.Update();
		}

		public override Poison PoisonImmune
		{
			get
			{
				return Poison.Lethal;
			}
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
            {
                this.m_Parent.Destroy();
            }
		}

		public DamagePlaceholder(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);

			writer.Write((Item)this.m_Parent);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			this.m_Parent = (DamageableItem)reader.ReadItem();

            Frozen = true;
            Paralyzed = true;
            CantWalk = true;
		}
	}
}