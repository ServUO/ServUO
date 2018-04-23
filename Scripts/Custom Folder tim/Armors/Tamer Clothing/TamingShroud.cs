// Created by GreyWolf79
// Created for crafting shards

using System; 
using Server; 

namespace Server.Items
{ 
	public class TamingShroud : HoodedShroudOfShadows
	{

        // For skill mods above cap without changing everything to above cap - GreyWolf.
        public SkillMod m_SkillMod0;

		[Constructable]
        public TamingShroud()
		{
			Name = "a Taming Shroud";
			Hue = 1161;

			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 2;
            SkillBonuses.SetValues(0, SkillName.AnimalTaming, 10.0);
            DefineMods();

		}

        public void DefineMods()
        {

            m_SkillMod0 = new DefaultSkillMod(SkillName.AnimalTaming, true, 10);

        }

        public void SetMods(Mobile wearer)
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
                m.RemoveStatMod("TamingShroud");

                if (m_SkillMod0 != null)
                    m_SkillMod0.Remove();


            }
        }

        public override void OnSingleClick(Mobile from)
        {
            this.LabelTo(from, Name);
        }

		public TamingShroud( Serial serial ) : base( serial )
		{

            DefineMods();

            if (Parent != null && this.Parent is Mobile)
                SetMods((Mobile)Parent);

		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
} 