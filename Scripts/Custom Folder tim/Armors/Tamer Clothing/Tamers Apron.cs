// Created by GreyWolf79
// Idea by Misty Dain
// Created on 06/13/2008

using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class TamersApron : HalfApron
  {
public override int ArtifactRarity{ get{ return 10; } }

		public override int InitMinHits{ get{ return 155; } }
		public override int InitMaxHits{ get{ return 255; } }

		public override int BaseColdResistance{ get{ return 10; } } 
		public override int BaseEnergyResistance{ get{ return 10; } } 
		public override int BasePhysicalResistance{ get{ return 17; } } 
		public override int BasePoisonResistance{ get{ return 15; } } 
		public override int BaseFireResistance{ get{ return 10; } }

        private SkillMod m_SkillMod0;
        private SkillMod m_SkillMod1;
      
      [Constructable]
		public TamersApron()
		{
			Weight = 1;
            Name = "Tamers Apron";
            Hue = Utility.RandomList( 2433, 2573, 1175, 2305, 2433, 2471, 2474, 2282, 2504, 2520, 2618, 2634, 1161, 2877, 2003, 2435, 2235, 2612, 2878, 1981, 2091 );
            Attributes.BonusInt = 5;
            Attributes.Luck = 50;
            Attributes.ReflectPhysical = 15;
            Attributes.RegenMana = 2;

            DefineMods();            

		}

      private void DefineMods()
      {
          m_SkillMod0 = new DefaultSkillMod(SkillName.AnimalTaming, true, 10);
          m_SkillMod1 = new DefaultSkillMod(SkillName.AnimalLore, true, 10);

      }

      private void SetMods(Mobile wearer)
      {
          wearer.AddSkillMod(m_SkillMod0);
          wearer.AddSkillMod(m_SkillMod1);

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
              m.RemoveStatMod("SashoftheTamer");

              if (m_SkillMod0 != null)
                  m_SkillMod0.Remove();

              if (m_SkillMod1 != null)
                  m_SkillMod1.Remove();


          }
      }

      public override void OnSingleClick(Mobile from)
      {
          this.LabelTo(from, Name);
      }

		public TamersApron( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
