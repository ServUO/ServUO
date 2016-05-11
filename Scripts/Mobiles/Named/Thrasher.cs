using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a thrasher corpse")]
    public class Thrasher : BaseCreature
    {
        [Constructable]
        public Thrasher()
            : base(AIType.AI_Melee, FightMode.Evil, 10, 1, 0.1, 0.2)
        {
            this.Name = "Thrasher";
            this.Body = 0xCE;
            this.Hue = 0x497;
            this.BaseSoundID = 0x294;

            this.SetStr(93, 327);
            this.SetDex(7, 201);
            this.SetInt(15, 67);

            this.SetHits(260, 984);

            this.SetDamage(15, 25);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 53, 55);
            this.SetResistance(ResistanceType.Fire, 25, 29);
            this.SetResistance(ResistanceType.Poison, 25, 28);

            this.SetSkill(SkillName.Wrestling, 101.2, 118.3);
            this.SetSkill(SkillName.Tactics, 99.1, 117.3);
            this.SetSkill(SkillName.MagicResist, 102.4, 118.6);

            this.Fame = 22400;
            this.Karma = -22400;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Thrasher(Serial serial)
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
            this.AddLoot(LootPack.AosFilthyRich, 4);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.ArmorIgnore;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);		
							
            c.DropItem(new ThrashersTail());
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