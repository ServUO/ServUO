using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a human corpse")]
    public class Protector : BaseCreature
    {
        [Constructable]
        public Protector()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Body = 401;
            this.Female = true;
            this.Hue = Race.Human.RandomSkinHue();
            this.HairItemID = Race.Human.RandomHair(this);
            this.HairHue = Race.Human.RandomHairHue();

            this.Name = "a Protector";
            this.Title = "the mystic llamaherder";

            this.SetStr(700, 800);
            this.SetDex(100, 150);
            this.SetInt(50, 75);

            this.SetHits(350, 450);

            this.SetDamage(6, 12);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 35, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Wrestling, 70.0, 100.0);
            this.SetSkill(SkillName.Tactics, 80.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 50.0, 70.0);
            this.SetSkill(SkillName.Anatomy, 70.0, 100.0);

            this.Fame = 10000;
            this.Karma = -10000;

            Item boots = new ThighBoots();
            boots.Movable = false;
            boots.Hue = Utility.Random(2);

            Item shroud = new Item(0x204E);
            shroud.Layer = Layer.OuterTorso;
            shroud.Movable = false;
            shroud.Hue = Utility.Random(2);

            this.AddItem(boots);
            this.AddItem(shroud);
        }

        public Protector(Serial serial)
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
        public override bool PropertyTitle
        {
            get
            {
                return false;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override void GenerateLoot(bool spawning)
        {
            if (spawning)
                return; // No loot/backpack on spawn

            base.GenerateLoot(true);
            base.GenerateLoot(false);
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
        }

        /*
        // TODO: uncomment once added
        public override void OnDeath( Container c )
        {
        base.OnDeath( c );

        if ( Utility.RandomDouble() < 0.4 )
        c.DropItem( new ProtectorsEssence() );
        }
        */
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