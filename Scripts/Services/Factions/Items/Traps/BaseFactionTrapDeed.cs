using System;
using Server.Engines.Craft;
using Server.Items;

namespace Server.Factions
{
    public abstract class BaseFactionTrapDeed : Item, ICraftable
    {
        public abstract Type TrapType { get; }

        private Faction m_Faction;

        [CommandProperty(AccessLevel.GameMaster)]
        public Faction Faction
        {
            get
            {
                return this.m_Faction;
            }
            set
            {
                this.m_Faction = value;

                if (this.m_Faction != null)
                    this.Hue = this.m_Faction.Definition.HuePrimary;
            }
        }

        public BaseFactionTrapDeed(int itemID)
            : base(itemID)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
        }

        public BaseFactionTrapDeed(bool createdFromDeed)
            : this(0x14F0)
        {
        }

        public BaseFactionTrapDeed(Serial serial)
            : base(serial)
        {
        }

        public virtual BaseFactionTrap Construct(Mobile from)
        {
            try
            {
                return Activator.CreateInstance(this.TrapType, new object[] { this.m_Faction, from }) as BaseFactionTrap;
            }
            catch
            {
                return null;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            Faction faction = Faction.Find(from);

            if (faction == null)
                from.SendLocalizedMessage(1010353, "", 0x23); // Only faction members may place faction traps
            else if (faction != this.m_Faction)
                from.SendLocalizedMessage(1010354, "", 0x23); // You may only place faction traps created by your faction
            else if (faction.Traps.Count >= faction.MaximumTraps)
                from.SendLocalizedMessage(1010358, "", 0x23); // Your faction already has the maximum number of traps placed
            else 
            {
                BaseFactionTrap trap = this.Construct(from);

                if (trap == null)
                    return;

                int message = trap.IsValidLocation(from.Location, from.Map);

                if (message > 0)
                {
                    from.SendLocalizedMessage(message, "", 0x23);
                    trap.Delete();
                }
                else
                {
                    from.SendLocalizedMessage(1010360); // You arm the trap and carefully hide it from view
                    trap.MoveToWorld(from.Location, from.Map);
                    faction.Traps.Add(trap);
                    this.Delete();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            Faction.WriteReference(writer, this.m_Faction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Faction = Faction.ReadReference(reader);
        }

        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            this.ItemID = 0x14F0;
            this.Faction = Faction.Find(from);

            return 1;
        }
        #endregion
    }
}