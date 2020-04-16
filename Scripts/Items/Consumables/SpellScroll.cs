using Server.ContextMenus;
using Server.Mobiles;
using Server.Spells;
using System.Collections.Generic;

namespace Server.Items
{
    public class SpellScroll : Item, ICommodity
    {
        private int m_SpellID;
        public SpellScroll(Serial serial)
            : base(serial)
        {
        }

        [Constructable]
        public SpellScroll(int spellID, int itemID)
            : this(spellID, itemID, 1)
        {
        }

        [Constructable]
        public SpellScroll(int spellID, int itemID, int amount)
            : base(itemID)
        {
            this.Stackable = true;
            this.Weight = 1.0;
            this.Amount = amount;

            this.m_SpellID = spellID;
        }

        public int SpellID
        {
            get
            {
                return this.m_SpellID;
            }
        }
        TextDefinition ICommodity.Description
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_SpellID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        this.m_SpellID = reader.ReadInt();

                        break;
                    }
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive && this.Movable)
                list.Add(new ContextMenus.AddToSpellbookEntry());
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!Multis.DesignContext.Check(from))
                return; // They are customizing

            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            #region SA
            else if (from.Flying && from is PlayerMobile && BaseMount.OnFlightPath(from))
            {
                from.SendLocalizedMessage(1113749); // You may not use that while flying over such precarious terrain.
                return;
            }
            #endregion

            Spell spell = SpellRegistry.NewSpell(this.m_SpellID, from, this);

            if (spell != null)
                spell.Cast();
            else
                from.SendLocalizedMessage(502345); // This spell has been temporarily disabled.
        }
    }
}
