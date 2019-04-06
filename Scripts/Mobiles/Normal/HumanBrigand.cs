/*using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a human corpse")]
    public class HumanBrigand : BaseCreature
    {
        [Constructable]
        public HumanBrigand()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Race = Race.Human;

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 401;
                this.Name = NameList.RandomName("female");
            }
            else
            {
                this.Body = 400;
                this.Name = NameList.RandomName("male");
            }

            this.Title = "the brigand";
            this.Hue = this.Race.RandomSkinHue();

            this.SetStr(86, 100);
            this.SetDex(81, 95);
            this.SetInt(61, 75);

            this.SetDamage(10, 23);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 10, 15);
            this.SetResistance(ResistanceType.Fire, 10, 15);
            this.SetResistance(ResistanceType.Poison, 10, 15);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.MagicResist, 25.0, 47.5);
            this.SetSkill(SkillName.Tactics, 65.0, 87.5);
            this.SetSkill(SkillName.Wrestling, 15.0, 37.5);

            this.Fame = 1000;
            this.Karma = -1000;

            // outfit
            this.AddItem(new Shirt(Utility.RandomNeutralHue()));

            switch (Utility.Random(4))
            {
                case 0:
                    this.AddItem(new Sandals());
                    break;
                case 1:
                    this.AddItem(new Shoes());
                    break;
                case 2:
                    this.AddItem(new Boots());
                    break;
                case 3:
                    this.AddItem(new ThighBoots());
                    break;
            }

            if (this.Female)
            {
                if (Utility.RandomBool())
                    this.AddItem(new Skirt(Utility.RandomNeutralHue()));
                else
                    this.AddItem(new Kilt(Utility.RandomNeutralHue()));
            }
            else
                this.AddItem(new ShortPants(Utility.RandomNeutralHue()));

            // hair, facial hair			
            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
            this.FacialHairItemID = this.Race.RandomFacialHair(this.Female);

            // weapon, shield
            Item weapon = Loot.RandomWeapon();

            this.AddItem(weapon);

            if (weapon.Layer == Layer.OneHanded && Utility.RandomBool())
                this.AddItem(Loot.RandomShield());

            this.PackGold(50, 150);
        }

        public HumanBrigand(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.75)
                c.DropItem(new SeveredHumanEars());
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
}*/