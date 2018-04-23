// Created by Nept
using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("corpse of the Maniacal Tailor")]
    public class DaneElec : BaseCreature
    {
        [Constructable]
        public DaneElec()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {

            Name = "Dane Elec";
	    Title = "The Maniacal Tailor";
            Body = 400;
            Hue = 43;

            SetStr(5350, 5400);
            SetDex(5150, 5200);
            SetInt(5150, 5200);

            SetHits(100000, 150000);

            SetDamage(25, 45);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 0, 1);
            SetResistance(ResistanceType.Fire, 0, 1);
            SetResistance(ResistanceType.Poison, 0, 1);
            SetResistance(ResistanceType.Energy, 0, 1);

            SetSkill(SkillName.EvalInt, 85.0, 100.0);
            SetSkill(SkillName.Tactics, 75.1, 100.0);
            SetSkill(SkillName.MagicResist, 75.0, 97.5);
            SetSkill(SkillName.Wrestling, 100.2, 105.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.Focus, 120.0);
            SetSkill(SkillName.Swords, 110.0, 120.0);

            Fame = 2500;
            Karma = -2500;

            VirtualArmor = 35;

			PackGold( 2000, 3000 );

			ManiacTailorChest Chest = new ManiacTailorChest();
			Chest.Movable = false;
			AddItem(Chest);

			ManiacTailorGorget Neck = new ManiacTailorGorget();
			Neck.Movable = false;
			AddItem(Neck);
			
			ManiacTailorArms Arms = new ManiacTailorArms();
			Arms.Movable = false;
			AddItem(Arms);
			
			ManiacTailorLegs Legs = new ManiacTailorLegs();
			Legs.Movable = false;
			AddItem(Legs);
			
			ManiacTailorGloves Gloves = new ManiacTailorGloves();
			Gloves.Movable = false;
			AddItem(Gloves);
			
			ManiacTailorHelm Helm = new ManiacTailorHelm();
			Helm.Movable = false;
			AddItem(Helm);

			ManiacTailorKnife Weapon = new ManiacTailorKnife();
			Weapon.Movable = false;
			AddItem(Weapon);

	  }

        public override void GenerateLoot()
        {

            switch (Utility.Random(75))
            {
                case 0: PackItem(new ManiacTailorChest()); break;
                case 1: PackItem(new ManiacTailorLegs()); break;
                case 2: PackItem(new ManiacTailorArms()); break;
                case 3: PackItem(new ManiacTailorGloves()); break;
                case 4: PackItem(new ManiacTailorHelm()); break;
		case 5: PackItem(new ManiacTailorKnife()); break;
		case 6: PackItem(new ManiacTailorGorget()); break;
                    {

                    }
            }
        }

        public DaneElec(Serial serial)
            : base(serial)
        {
        }
        public override bool AlwaysMurderer { get { return true; } }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
