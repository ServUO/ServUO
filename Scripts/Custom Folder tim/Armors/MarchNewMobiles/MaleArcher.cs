using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class MaleArcher : BaseCreature
	{   
        [Constructable]
        public MaleArcher()
            : base(AIType.AI_Archer, FightMode.Aggressor, 14, 1, 0.8, 1.6)
        {
            SpeechHue = Utility.RandomDyedHue();
            Title = "A Cove Archer";
            Hue = Utility.RandomSkinHue();

            
            {

                Body = 0x190;
                Name = NameList.RandomName("male");

                AddItem(new StuddedChest());
                AddItem(new StuddedArms());
                AddItem(new StuddedGloves());
                AddItem(new StuddedGorget());
                AddItem(new StuddedLegs());
                AddItem(new Boots());
                

                {


                    SetStr(90, 100);
                    SetDex(100, 140);
                    SetInt(60, 80);

                    SetDamage(10, 23);

                                               SetDamageType( ResistanceType.Physical, 5 );
                                               SetDamageType( ResistanceType.Cold, 5 );
                                               SetDamageType( ResistanceType.Fire, 5 );
                                               SetDamageType( ResistanceType.Energy, 5 );
                                               SetDamageType( ResistanceType.Poison, 80 );

                                               SetResistance( ResistanceType.Physical, 25 );
                                               SetResistance( ResistanceType.Cold, 25 );
                                               SetResistance( ResistanceType.Fire, 25 );
                                               SetResistance( ResistanceType.Energy, 25 );
                                               SetResistance( ResistanceType.Poison, 100 );

                    Skills[SkillName.Anatomy].Base = 120.0;
                    Skills[SkillName.Tactics].Base = 120.0;
                    Skills[SkillName.Archery].Base = 120.0;
                    Skills[SkillName.MagicResist].Base = 100.0;
                    Skills[SkillName.DetectHidden].Base = 100.0;

                    Fame = 1000;
                    Karma = 1000;
                    VirtualArmor = 40;

                    {



                        Bow bow = new Bow();

                        bow.Movable = true;
                        bow.Crafter = this;
                        AddItem(bow);

                        Container pack = new Backpack();

                        pack.Movable = false;

                        Arrow arrows = new Arrow(250);
                        PackItem(new Arrow(Utility.Random(100, 320)));
                      
                        
                        pack.DropItem(new Gold(10, 250));
                        
                        {



                            Utility.AssignRandomHair(this);
                        }
                    }
                }
            }
        }
		
		




		public MaleArcher( Serial serial ) : base( serial )
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