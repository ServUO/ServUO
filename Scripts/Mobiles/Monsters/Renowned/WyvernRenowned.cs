using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Wyvern [Renowned] corpse")]
    public class WyvernRenowned : BaseRenowned
    {
        [Constructable]
        public WyvernRenowned()
            : base(AIType.AI_Mage)
        {
            Name = "Wyvern";
            Title = "[Renowned]";
            Body = 62;
            Hue = 243;
            BaseSoundID = 362;

            SetStr(1364, 1544);
            SetDex(144, 160);
            SetInt(861, 1081);

            SetHits(2782, 2848);

            SetDamage(29, 35);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);

            SetResistance(ResistanceType.Physical, 61, 66);
            SetResistance(ResistanceType.Fire, 67, 89);
            SetResistance(ResistanceType.Cold, 61, 77);
            SetResistance(ResistanceType.Poison, 56, 62);
            SetResistance(ResistanceType.Energy, 53, 63);

            SetSkill(SkillName.Magery, 90.1, 94.3);
            SetSkill(SkillName.Meditation, 64.1, 70.1);
            SetSkill(SkillName.EvalInt, 90.1, 94.3);
            //SetSkill( SkillName.Wrestling, 65.1, 80.0 );

            Fame = 24000;
            Karma = -24000;

            VirtualArmor = 70;

            PackItem(new EssenceDiligence());
        }

        public WyvernRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get { return new Type[] {}; }
        }

        public override Type[] SharedSAList
        {
            get { return new[] {typeof (AnimatedLegsoftheInsaneTinker), typeof (PillarOfStrength)}; }
        }

        public override bool ReacquireOnMovement
        {
            get { return true; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Deadly; }
        }

        public override Poison HitPoison
        {
            get { return Poison.Deadly; }
        }

        public override bool AutoDispel
        {
            get { return true; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override int TreasureMapLevel
        {
            get { return 5; }
        }

        public override int Meat
        {
            get { return 10; }
        }

        public override int Hides
        {
            get { return 20; }
        }

        public override HideType HideType
        {
            get { return HideType.Horned; }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.Gems, 5);
        }

        public override int GetAttackSound()
        {
            return 713;
        }

        public override int GetAngerSound()
        {
            return 718;
        }

        public override int GetDeathSound()
        {
            return 716;
        }

        public override int GetHurtSound()
        {
            return 721;
        }

        public override int GetIdleSound()
        {
            return 725;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
        }
    }
}