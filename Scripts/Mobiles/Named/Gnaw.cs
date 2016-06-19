using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Gnaw corpse")]
    public class Gnaw : DireWolf
    {
        [Constructable]
        public Gnaw()
        {

            this.Name = "Gnaw";
            this.Hue = 0x130;

            this.SetStr(151, 172);
            this.SetDex(124, 145);
            this.SetInt(60, 86);

            this.SetHits(817, 857);
            this.SetStam(124, 145);
            this.SetMana(52, 86);

            this.SetDamage(16, 22);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 64, 69);
            this.SetResistance(ResistanceType.Fire, 53, 56);
            this.SetResistance(ResistanceType.Cold, 22, 27);
            this.SetResistance(ResistanceType.Poison, 27, 30);
            this.SetResistance(ResistanceType.Energy, 21, 34);

            this.SetSkill(SkillName.Wrestling, 106.4, 116.5);
            this.SetSkill(SkillName.Tactics, 84.1, 103.2);
            this.SetSkill(SkillName.MagicResist, 96.8, 110.7);

            this.Fame = 17500;
            this.Karma = -17500;

            Tamable = false;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }
		public override bool CanBeParagon { get { return false; } }
        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.3 )
                c.DropItem( new GnawsFang() );
        }
        
        public Gnaw(Serial serial)
            : base(serial)
        {
        }

        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override int Hides
        {
            get
            {
                return 28;
            }
        }
        public override int Meat
        {
            get
            {
                return 4;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
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