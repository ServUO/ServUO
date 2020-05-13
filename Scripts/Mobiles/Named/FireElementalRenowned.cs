using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("Fire Elemental [Renowned] corpse")]
    public class FireElementalRenowned : BaseRenowned
    {
        [Constructable]
        public FireElementalRenowned()
            : base(AIType.AI_Mage)
        {
            Name = "Fire Elemental";
            Title = "[Renowned]";
            Body = 15;
            BaseSoundID = 838;

            Hue = 1161;

            SetStr(450, 500);
            SetDex(200, 250);
            SetInt(300, 350);

            SetHits(1200, 1600);

            SetDamage(7, 9);

            SetDamageType(ResistanceType.Physical, 25);
            SetDamageType(ResistanceType.Fire, 75);

            SetResistance(ResistanceType.Physical, 45, 60);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 5, 10);
            SetResistance(ResistanceType.Poison, 30, 50);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.EvalInt, 100.1, 110.0);
            SetSkill(SkillName.Magery, 105.1, 110.0);
            SetSkill(SkillName.MagicResist, 110.2, 120.0);
            SetSkill(SkillName.Tactics, 100.1, 105.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 4500;
            Karma = -4500;

            AddItem(new LightSource());
        }

        public FireElementalRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList => new Type[] { typeof(JadeWarAxe) };
        public override Type[] SharedSAList => new Type[] { typeof(TokenOfHolyFavor), typeof(SwordOfShatteredHopes), typeof(LegacyOfDespair) };
        public override double DispelDifficulty => 117.5;
        public override double DispelFocus => 45.0;
        public override bool BleedImmune => true;
        public override int TreasureMapLevel => 2;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich);
            AddLoot(LootPack.LootItem<SulfurousAsh>(3, true));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
