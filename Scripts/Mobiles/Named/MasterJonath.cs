using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Master Jonath corpse")]
    public class MasterJonath : BoneMagi
    {
        [Constructable]
        public MasterJonath()
        {
            this.Name = "Master Jonath";
            this.Hue = 0x455;

            this.SetStr(109, 131);
            this.SetDex(98, 110);
            this.SetInt(232, 259);

            this.SetHits(766, 920);

            this.SetDamage(10, 15);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 60);
            this.SetResistance(ResistanceType.Fire, 43, 49);
            this.SetResistance(ResistanceType.Cold, 45, 80);
            this.SetResistance(ResistanceType.Poison, 41, 45);
            this.SetResistance(ResistanceType.Energy, 54, 55);

            this.SetSkill(SkillName.Wrestling, 80.5, 88.6);
            this.SetSkill(SkillName.Tactics, 88.5, 95.1);
            this.SetSkill(SkillName.MagicResist, 102.7, 102.9);
            this.SetSkill(SkillName.Magery, 100.0, 106.6);
            this.SetSkill(SkillName.EvalInt, 99.6, 106.9);
            this.SetSkill(SkillName.Necromancy, 100.0, 106.6);
            this.SetSkill(SkillName.SpiritSpeak, 99.6, 106.9);

            this.Fame = 18000;
            this.Karma = -18000;

            this.PackReg(7);
            this.PackReg(7);
            this.PackReg(8);

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public MasterJonath(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.05 )
            c.DropItem( new ParrotItem() );

            if ( Utility.RandomDouble() < 0.15 )
            c.DropItem( new DisintegratingThesisNotes() );

            if ( Paragon.ChestChance > Utility.RandomDouble() )
            c.DropItem( new ParagonChest( Name, TreasureMapLevel ) );

        }

        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 3);
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