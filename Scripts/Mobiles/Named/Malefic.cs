using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Malefic corpse")]
    public class Malefic : DreadSpider
    {
        [Constructable]
        public Malefic()
        {
            this.Name = "Malefic";
            this.Hue = 0x455;

            this.SetStr(210, 284);
            this.SetDex(153, 197);
            this.SetInt(349, 390);

            this.SetHits(600, 747);
            this.SetStam(153, 197);
            this.SetMana(349, 390);

            this.SetDamage(15, 22);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Poison, 80);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 49);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 41, 48);

            this.SetSkill(SkillName.Wrestling, 96.9, 112.4);
            this.SetSkill(SkillName.Tactics, 91.3, 105.4);
            this.SetSkill(SkillName.MagicResist, 79.8, 95.1);
            this.SetSkill(SkillName.Magery, 103.0, 118.6);
            this.SetSkill(SkillName.EvalInt, 105.7, 119.6);
            this.SetSkill(SkillName.Meditation, 0);

            this.Fame = 21000;
            this.Karma = -21000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }

            /*
            // TODO: uncomment once added
            if ( Utility.RandomDouble() < 0.1 )
            PackItem( new ParrotItem() );
            */
        }

        public Malefic(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 3);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Dismount;
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