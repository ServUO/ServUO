using System;
using System.Collections;
using Server.Mobiles;
using Server.Targeting;
using Server.Targets;
using Server.Network;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public interface IDamageableItem : IDamageable
    {
        bool CanDamage { get; }
        bool CheckHit(Mobile attacker);
        void OnHarmfulSpell(Mobile attacker);
    }

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

		[CommandProperty(AccessLevel.GameMaster)]
		public ItemLevel Level
		{
			get
			{
				return m_ItemLevel;
			}
			set
			{
				m_ItemLevel = value;

				double bonus = (double)(((int)m_ItemLevel * 100.0) * ((int)m_ItemLevel * 5));

				HitsMax = ((int)(100 + bonus));
				Hits = ((int)(100 + bonus));
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int IDStart
		{
			get
			{
				return m_StartID;
			}
			set
			{
				if (value < 0)
					m_StartID = 0;
				else if (value > int.MaxValue)
					m_StartID = int.MaxValue;
				else
					m_StartID = value;

				if (m_Hits >= (m_HitsMax * IDChange))
				{
					if (ItemID != m_StartID)
						ItemID = m_StartID;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int IDHalfHits
		{
			get
			{
				return m_HalfHitsID;
			}
			set
			{
				if (value < 0)
					m_HalfHitsID = 0;
				else if (value > int.MaxValue)
					m_HalfHitsID = int.MaxValue;
				else
					m_HalfHitsID = value;

				if (m_Hits < (m_HitsMax * IDChange))
				{
					if (ItemID != m_HalfHitsID)
						ItemID = m_HalfHitsID;
				}
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
				return m_Hits;
			}
			set
			{
                if (value > HitsMax)
                {
                    value = HitsMax;
                }

                if (m_Hits != value)
                {
                    int oldValue = m_Hits;
                    m_Hits = value;
                    UpdateDelta();
                    OnHitsChange(oldValue);
                }

                int id = ItemID;

                if (m_Hits >= (m_HitsMax * IDChange) && id != m_StartID)
                {
                    ItemID = m_StartID;
                    OnIDChange(id);
                }
                else if (m_Hits <= (m_HitsMax * IDChange) && id == m_StartID)
                {
                    ItemID = m_HalfHitsID;
                    OnIDChange(id);
                }
                
                if (m_Hits < 0)
                {
                    Destroy();
                    return;
                }
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int HitsMax
		{
			get
			{
				return m_HitsMax;
			}
			set
			{
				if (value > int.MaxValue)
					m_HitsMax = int.MaxValue;
				else
					m_HitsMax = value;

				if (Hits > m_HitsMax)
					Hits = m_HitsMax;
			}
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Destroyed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ResistBasePhys { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ResistBaseFire { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ResistBaseCold { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ResistBasePoison { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ResistBaseEnergy { get; set; }

        public Dictionary<Mobile, int> DamageStore { get; set; }

        public virtual int HitEffect { get { return -1; } }
        public virtual int DestroySound { get { return 0x3B3; } }
        public virtual double IDChange { get { return 0.5; } }
        public virtual bool DeleteOnDestroy { get { return true; } }
        public virtual bool Alive { get { return !Destroyed; } }
        public virtual bool CanDamage { get { return true; } }

        public override int PhysicalResistance { get { return ResistBasePhys; } }
        public override int FireResistance { get { return ResistBaseFire; } }
        public override int ColdResistance { get { return ResistBaseCold; } }
        public override int PoisonResistance { get { return ResistBasePoison; } }
        public override int EnergyResistance { get { return ResistBaseEnergy; } }

        public override bool ForceShowProperties { get { return false; } }

		[Constructable]
		public DamageableItem(int StartID, int HalfID, int destroyID = -1)
			: base(StartID)
		{
			Hue = 0;
			Movable = false;

			Level = ItemLevel.NotSet;

			IDStart = StartID;
			IDHalfHits = HalfID;
            IDDestroyed = destroyID;
		}

        public override void OnDoubleClick(Mobile m)
        {
            if (m.Warmode)
                m.Attack(this);
        }

        public virtual bool CheckHit(Mobile attacker)
        {
            return true; // Always hits
        }

        public virtual void OnHarmfulSpell(Mobile attacker)
        {
        }

        public virtual void OnStatsQuery(Mobile from)
        {
            if (from.Map == Map && Utility.InUpdateRange(this, from) && from.CanSee(this))
            {
                from.Send(new MobileStatusCompact(false, this));
            }
        }

        public virtual void UpdateDelta()
        {
            var eable = Map.GetClientsInRange(Location);
            Mobile beholder = null;

            Packet status = Packet.Acquire(new MobileHitsN(this));

            foreach (NetState ns in eable)
            {
                beholder = ns.Mobile;

                if (beholder != null && beholder.CanSee(this))
                {
                    ns.Send(status);
                }
            }

            Packet.Release(status);
            eable.Free();
        }

        public virtual void OnHitsChange(int oldhits)
        {       
        }

		public virtual bool OnBeforeDestroyed()
		{
			return true;
		}

		public virtual void OnAfterDestroyed()
		{
		}

		public virtual void Damage(int amount, Mobile from)
		{
            if (!CanDamage && from.Combatant == this)
            {
                from.Combatant = null;
                return;
            }

			Hits -= amount;

            if (amount > 0)
                RegisterDamage(from, amount);

            if (HitEffect > 0)
                Effects.SendLocationEffect(Location, Map, HitEffect, 10, 5);

            NetState theirState = (from == null ? null : from.NetState);

            if (theirState == null && from != null)
            {
                Mobile master = from.GetDamageMaster(null);

                if (master != null)
                {
                    theirState = master.NetState;
                }
            }

            if (amount > 0 && theirState != null)
            {
                theirState.Send(Packet.Acquire(new DamagePacket(this, amount)));
            }

			OnDamage(amount, from, Hits < 0);
		}

        public void RegisterDamage(Mobile m, int damage)
        {
            if(m == null)
                return;

            if (DamageStore == null)
                DamageStore = new Dictionary<Mobile, int>();

            if (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)
                m = ((BaseCreature)m).GetMaster();

            if (!DamageStore.ContainsKey(m))
                DamageStore[m] = 0;

            DamageStore[m] += damage;
        }

        public List<Mobile> GetLootingRights()
        {
            if (DamageStore == null)
                return null;

            return DamageStore.Keys.Where(m => DamageStore[m] > 0 && DamageStore[m] >= HitsMax / 16).ToList();
        }

        public virtual void OnDamage(int amount, Mobile from, bool willkill)
        {
        }

		public bool Destroy()
		{
			if (this == null || Deleted || Destroyed)
				return false;

            Effects.PlaySound(Location, Map, DestroySound);

            if (OnBeforeDestroyed())
            {
                if (DeleteOnDestroy)
                {
                    Delete();
                }
                else if (m_DestroyedID >= 0)
                {
                    ItemID = m_DestroyedID;
                }

                Destroyed = true;
                OnAfterDestroyed();

                return true;
            }

			return false;
		}

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (DamageStore != null)
                DamageStore.Clear();
        }

        public virtual void OnIDChange(int oldID)
        {
        }

		public DamageableItem(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version

			writer.Write((int)m_StartID);
			writer.Write((int)m_HalfHitsID);
			writer.Write((int)m_DestroyedID);
			writer.Write((int)m_ItemLevel);
			writer.Write((int)m_Hits);
			writer.Write((int)m_HitsMax);
            writer.Write(Destroyed);

            writer.Write(ResistBasePhys);
            writer.Write(ResistBaseFire);
            writer.Write(ResistBaseCold);
            writer.Write(ResistBasePoison);
            writer.Write(ResistBaseEnergy);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			m_StartID = (int)reader.ReadInt();
			m_HalfHitsID = (int)reader.ReadInt();
			m_DestroyedID = (int)reader.ReadInt();
			m_ItemLevel = (ItemLevel)reader.ReadInt();
			m_Hits = (int)reader.ReadInt();
			m_HitsMax = (int)reader.ReadInt();
            Destroyed = reader.ReadBool();

            ResistBasePhys = reader.ReadInt();
            ResistBaseFire = reader.ReadInt();
            ResistBaseCold = reader.ReadInt();
            ResistBasePoison = reader.ReadInt();
            ResistBaseEnergy = reader.ReadInt();
		}
	}

    public class TestDamageableItem : DamageableItem
    {
        [Constructable]
        public TestDamageableItem(int itemid)
            : base(itemid, itemid)
        {
            Name = "Test Damageable Item";
        }

        public TestDamageableItem(Serial serial)
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
}