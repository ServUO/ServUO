using System;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
    public class SpellStone : SpellScroll
    {
        private static readonly Dictionary<int, string> m_SpellNames = new Dictionary<int, string>();
        private Mobile m_Caster;
        [Constructable]
        public SpellStone(Mobile caster, int spellid)
            : base(spellid, 0x4079, 1)
        {
            this.m_Caster = caster;
        }

        public SpellStone(Serial serial)
            : base(serial)
        {
        }

        public static void Configure()
        {
            m_SpellNames.Add(677, "Nether Bolt");
            m_SpellNames.Add(678, "Healing Stone");
            m_SpellNames.Add(679, "Purge Magic");
            m_SpellNames.Add(680, "Enchant");
            m_SpellNames.Add(681, "Sleep");
            m_SpellNames.Add(682, "Eagle Strike");
            m_SpellNames.Add(683, "Animated Weapon");
            m_SpellNames.Add(684, "Stone Form");
            m_SpellNames.Add(685, "Spell Tigger");
            m_SpellNames.Add(686, "Mass Sleep");
            m_SpellNames.Add(687, "Cleansing Winds");
            m_SpellNames.Add(688, "Bombard");
            m_SpellNames.Add(689, "Spell Plague");
            m_SpellNames.Add(690, "Hail Storm");
            m_SpellNames.Add(691, "Nether Cyclone");
            m_SpellNames.Add(692, "Colossus");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from != this.m_Caster)
            {
                from.SendLocalizedMessage(500294); // You cannot use that.
                this.Delete();
            }
            else if (this.SpellID >= 677 && this.SpellID <= 692)
                base.OnDoubleClick(from);
            else
            {
                from.SendMessage("There was no Spell Stored in that stone.");
                this.Delete();
            }
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            this.Delete();
            return false;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            return false;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_SpellNames.ContainsKey(this.SpellID))
                list.Add(1080166, m_SpellNames[this.SpellID]); // Use: ~1_spellName~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(this.m_Caster);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_Caster = reader.ReadMobile();
        }
    }
}