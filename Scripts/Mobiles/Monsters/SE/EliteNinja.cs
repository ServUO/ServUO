using System;
using Server.Items;

namespace Server.Mobiles
{
    public class EliteNinja : BaseCreature
    {
        [Constructable]
        public EliteNinja()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.SpeechHue = Utility.RandomDyedHue();
            this.Hue = Utility.RandomSkinHue();
            this.Name = "an elite ninja";

            this.Body = (this.Female = Utility.RandomBool()) ? 0x191 : 0x190;

            this.SetHits(251, 350);

            this.SetStr(126, 225);
            this.SetDex(81, 95);
            this.SetInt(151, 165);

            this.SetDamage(12, 20);

            this.SetDamageType(ResistanceType.Physical, 65);
            this.SetDamageType(ResistanceType.Fire, 15);
            this.SetDamageType(ResistanceType.Poison, 15);
            this.SetDamageType(ResistanceType.Energy, 5);

            this.SetResistance(ResistanceType.Physical, 35, 65);
            this.SetResistance(ResistanceType.Fire, 40, 60);
            this.SetResistance(ResistanceType.Cold, 25, 45);
            this.SetResistance(ResistanceType.Poison, 40, 60);
            this.SetResistance(ResistanceType.Energy, 35, 55);

            this.SetSkill(SkillName.Anatomy, 105.0, 120.0);
            this.SetSkill(SkillName.MagicResist, 80.0, 100.0);
            this.SetSkill(SkillName.Tactics, 115.0, 130.0);
            this.SetSkill(SkillName.Wrestling, 95.0, 120.0);
            this.SetSkill(SkillName.Fencing, 95.0, 120.0);
            this.SetSkill(SkillName.Macing, 95.0, 120.0);
            this.SetSkill(SkillName.Swords, 95.0, 120.0);
            this.SetSkill(SkillName.Ninjitsu, 95.0, 120.0);

            this.Fame = 8500;
            this.Karma = -8500;

            /* TODO:	
            Uses Smokebombs
            Hides
            Stealths
            Can use Ninjitsu Abilities
            Can change weapons during a fight
            */
					
            this.AddItem(new NinjaTabi());
            this.AddItem(new LeatherNinjaJacket());
            this.AddItem(new LeatherNinjaHood());
            this.AddItem(new LeatherNinjaPants());
            this.AddItem(new LeatherNinjaMitts());
			
            if (Utility.RandomDouble() < 0.33)
                this.AddItem(new SmokeBomb());

            switch ( Utility.Random(8))
            {
                case 0:
                    this.AddItem(new Tessen());
                    break;
                case 1:
                    this.AddItem(new Wakizashi());
                    break;
                case 2:
                    this.AddItem(new Nunchaku());
                    break;
                case 3:
                    this.AddItem(new Daisho());
                    break;
                case 4:
                    this.AddItem(new Sai());
                    break;
                case 5:
                    this.AddItem(new Tekagi());
                    break;
                case 6:
                    this.AddItem(new Kama());
                    break;
                case 7:
                    this.AddItem(new Katana());
                    break;
            }

            Utility.AssignRandomHair(this);
        }

        public EliteNinja(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return true;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            c.DropItem(new BookOfNinjitsu());
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Gems, 2);
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