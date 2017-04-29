using System;
using Server.Items;

namespace Server.Mobiles
{
    public class ForgottenServant : BaseCreature
    {
        [Constructable]
        public ForgottenServant()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.SpeechHue = Utility.RandomDyedHue();
            this.Title = "Forgotten Servant";
            this.Hue = 768;

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
                this.AddItem(new Skirt(Utility.RandomNeutralHue()));
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
                this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }

            this.SetStr(147, 215);
            this.SetDex(91, 115);
            this.SetInt(61, 85);

            this.SetHits(95, 123);

            this.SetDamage(4, 14);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 70.1, 85.0);
            this.SetSkill(SkillName.Swords, 60.1, 85.0);
            this.SetSkill(SkillName.Tactics, 75.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 85.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.AddItem(new Boots(Utility.RandomNeutralHue()));
            this.AddItem(new FancyShirt());
            this.AddItem(new Bandana());

            switch ( Utility.Random(7))
            {
                case 0:
                    this.AddItem(new Longsword());
                    break;
                case 1:
                    this.AddItem(new Cutlass());
                    break;
                case 2:
                    this.AddItem(new Broadsword());
                    break;
                case 3:
                    this.AddItem(new Axe());
                    break;
                case 4:
                    this.AddItem(new Club());
                    break;
                case 5:
                    this.AddItem(new Dagger());
                    break;
                case 6:
                    this.AddItem(new Spear());
                    break;
            }

            Utility.AssignRandomHair(this);
        }

        public ForgottenServant(Serial serial)
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
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
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