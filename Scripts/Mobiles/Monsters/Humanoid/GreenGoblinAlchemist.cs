using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class GreenGoblinAlchemist : BaseCreature
    {
        //public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }
        [Constructable]
        public GreenGoblinAlchemist()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Green Goblin Alchemist";
            this.Body = 723;
            this.BaseSoundID = 0x45A;

            this.SetStr(410, 420);
            this.SetDex(75, 80);
            this.SetInt(15, 18);

            this.SetHits(350, 365);
            this.SetStam(75, 80);
            this.SetMana(15, 18);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 45);
            this.SetResistance(ResistanceType.Fire, 45, 47);
            this.SetResistance(ResistanceType.Cold, 35, 36);
            this.SetResistance(ResistanceType.Poison, 35, 37);
            this.SetResistance(ResistanceType.Energy, 15, 20);

            this.SetSkill(SkillName.MagicResist, 124.1, 124.2);
            this.SetSkill(SkillName.Tactics, 75.3, 80.1);
            this.SetSkill(SkillName.Anatomy, 0.0, 0.0);
            this.SetSkill(SkillName.Wrestling, 105.4, 107.4);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 28;

            // loot 60 gold, magic item, gem, bola ball, liquar,gob blood
            switch ( Utility.Random(20) )
            {
                case 0:
                    this.PackItem(new Scimitar());
                    break;
                case 1:
                    this.PackItem(new Katana());
                    break;
                case 2:
                    this.PackItem(new WarMace());
                    break;
                case 3:
                    this.PackItem(new WarHammer());
                    break;
                case 4:
                    this.PackItem(new Kryss());
                    break;
                case 5:
                    this.PackItem(new Pitchfork());
                    break;
            }

            this.PackItem(new ThighBoots());

            switch ( Utility.Random(3) )
            {
                case 0:
                    this.PackItem(new Ribs());
                    break;
                case 1:
                    this.PackItem(new Shaft());
                    break;
                case 2:
                    this.PackItem(new Candle());
                    break;
            }

            if (0.2 > Utility.RandomDouble())
                this.PackItem(new BolaBall());
        }

        //Item item = aggressor.FindItemOnLayer( Layer.Helm );

        //if ( item is OrcishKinMask )
        //{
        //	AOS.Damage( aggressor, 50, 0, 100, 0, 0, 0 );
        //	item.Delete();
        //	aggressor.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
        //	aggressor.PlaySound( 0x307 );
        //}
        //}
        public GreenGoblinAlchemist(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
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
                return 1;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.SavagesAndOrcs;
            }
        }
        //public override bool IsEnemy( Mobile m )
        //{
        //	if ( m.Player && m.FindItemOnLayer( Layer.Helm ) is OrcishKinMask )
        //		return false;

        //	return base.IsEnemy( m );
        //}

        //public override void AggressiveAction( Mobile aggressor, bool criminal )
        //{
        //base.AggressiveAction( aggressor, criminal );
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
        }

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