using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Master Mikael corpse")]
    public class MasterMikael : BoneMagi
    {
        [Constructable]
        public MasterMikael()
        {
            this.Name = "Master Mikael";
            this.Hue = 0x8FD;

            this.SetStr(93, 122);
            this.SetDex(91, 100);
            this.SetInt(252, 271);

            this.SetHits(789, 1014);

            this.SetDamage(11, 19);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 59);
            this.SetResistance(ResistanceType.Fire, 40, 46);
            this.SetResistance(ResistanceType.Cold, 72, 80);
            this.SetResistance(ResistanceType.Poison, 44, 49);
            this.SetResistance(ResistanceType.Energy, 50, 57);

            this.SetSkill(SkillName.Wrestling, 80.1, 87.2);
            this.SetSkill(SkillName.Tactics, 79.0, 90.9);
            this.SetSkill(SkillName.MagicResist, 90.3, 106.9);
            this.SetSkill(SkillName.Magery, 103.8, 108.0);
            this.SetSkill(SkillName.EvalInt, 96.1, 105.3);
            this.SetSkill(SkillName.Necromancy, 103.8, 108.0);
            this.SetSkill(SkillName.SpiritSpeak, 96.1, 105.3);

            this.Fame = 18000;
            this.Karma = -18000;

            this.PackReg(3);
            this.PackNecroReg(1, 10);

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public MasterMikael(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.15 )
            c.DropItem( new DisintegratingThesisNotes() );

            if ( Utility.RandomDouble() < 0.1 )
            c.DropItem( new ParrotItem() );
        }

        /*public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }*/
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
            this.AddLoot(LootPack.MedScrolls, 2);
            this.AddLoot(LootPack.HighScrolls, 2);
        }

        // TODO: Special move?
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