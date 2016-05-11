using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Pyre corpse")]
    public class Pyre : Phoenix
    {
        [Constructable]
        public Pyre()
        {
            this.Name = "Pyre";
            this.Hue = 0x489;

            this.FightMode = FightMode.Closest;

            this.SetStr(605, 611);
            this.SetDex(391, 519);
            this.SetInt(669, 818);

            this.SetHits(1783, 1939);

            this.SetDamage(30);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Fire, 50);

            this.SetResistance(ResistanceType.Physical, 65);
            this.SetResistance(ResistanceType.Fire, 72, 75);
            this.SetResistance(ResistanceType.Poison, 36, 41);
            this.SetResistance(ResistanceType.Energy, 50, 51);

            this.SetSkill(SkillName.Wrestling, 121.9, 130.6);
            this.SetSkill(SkillName.Tactics, 114.4, 117.4);
            this.SetSkill(SkillName.MagicResist, 147.7, 153.0);
            this.SetSkill(SkillName.Poisoning, 122.8, 124.0);
            this.SetSkill(SkillName.Magery, 121.8, 127.8);
            this.SetSkill(SkillName.EvalInt, 103.6, 117.0);
            this.SetSkill(SkillName.Meditation, 100.0, 110.0);

            this.Fame = 21000;
            this.Karma = -21000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Pyre(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

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
        public override bool HasAura
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
            if (Utility.RandomBool())
                return WeaponAbility.ParalyzingBlow;
            else
                return WeaponAbility.BleedAttack;
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