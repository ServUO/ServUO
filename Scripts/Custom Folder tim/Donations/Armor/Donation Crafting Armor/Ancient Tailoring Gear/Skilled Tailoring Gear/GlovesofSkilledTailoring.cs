// Created by GreyWolf
// Created On: 10/4/2007
// Modified Nov. 3, 2007

using System;
using Server;

namespace Server.Items
{
    public class GlovesofSkilledTailoring : LeatherGloves
    {
        public override int BasePhysicalResistance{ get{ return 5; } }
        public override int BaseColdResistance{ get{ return 5; } }
        public override int BaseFireResistance{ get{ return 5; } }
        public override int BaseEnergyResistance{ get{ return 5; } }
        public override int BasePoisonResistance{ get{ return 5; } }
        public override int InitMinHits{ get{ return 50; } }
        public override int InitMaxHits{ get{ return 100; } }

        // For skill mods above cap without changing everything to above cap - GreyWolf.
        private SkillMod m_SkillMod0;

        [Constructable]
        public GlovesofSkilledTailoring()
        {
            Name = "Gloves of Skilled Tailoring +3";
            Hue = 62;
            LootType = LootType.Blessed;
            Attributes.NightSight = 1;
            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.RegenStam = 5;

            DefineMods();

        }

        private void DefineMods()
        {

            m_SkillMod0 = new DefaultSkillMod(SkillName.Tailoring, true, 3);

        }

        private void SetMods(Mobile wearer)
        {
            wearer.AddSkillMod(m_SkillMod0);


        }

        public override bool OnEquip(Mobile from)
        {
            SetMods(from);
            return true;
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile m = (Mobile)parent;
                m.RemoveStatMod("GlovesofSkilledTailoring");

                if (m_SkillMod0 != null)
                    m_SkillMod0.Remove();


            }
        }

        public override void OnSingleClick(Mobile from)
        {
            this.LabelTo(from, Name);
        }

        public GlovesofSkilledTailoring(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 );
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    } // End Class
} // End Namespace
