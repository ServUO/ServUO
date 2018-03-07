using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a coil corpse")]
    public class Coil : SilverSerpent
    {
        [Constructable]
        public Coil()
            : base()
        {
            this.ActiveSpeed = 0.1;
            this.PassiveSpeed = 0.2;
		
            this.Name = "Coil";

            this.Hue = 0x3F;
            this.BaseSoundID = 0xDB;

            this.SetStr(205, 343);
            this.SetDex(202, 283);
            this.SetInt(88, 142);

            this.SetHits(628, 1291);

            this.SetDamage(19, 28);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 56, 62);
            this.SetResistance(ResistanceType.Fire, 25, 29);
            this.SetResistance(ResistanceType.Cold, 25, 30);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 27, 30);

            this.SetSkill(SkillName.Wrestling, 124.5, 134.5);
            this.SetSkill(SkillName.Tactics, 130.2, 142.0);
            this.SetSkill(SkillName.MagicResist, 102.3, 113.0);
            this.SetSkill(SkillName.Anatomy, 120.8, 138.1);
            this.SetSkill(SkillName.Poisoning, 110.1, 133.4);

            this.Fame = 17500;
            this.Karma = -17500;

            this.PackGem(2);
            this.PackItem(new Bone());

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Coil(Serial serial)
            : base(serial)
        {
        }

        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
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
                return 48;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosUltraRich, 3);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.MortalStrike;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);		
			
            c.DropItem(new CoilsFang());
			
            if (Utility.RandomDouble() < 0.025)
            {
                switch( Utility.Random(5) )
                {
                    case 0:
                        c.DropItem(new AssassinChest());
                        break;
                    case 1:
                        c.DropItem(new DeathGloves());
                        break;
                    case 2:
                        c.DropItem(new LeafweaveLegs());
                        break;
                    case 3:
                        c.DropItem(new HunterLegs());
                        break;
                    case 4:
                        c.DropItem(new MyrmidonLegs());
                        break;
                }
            }
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