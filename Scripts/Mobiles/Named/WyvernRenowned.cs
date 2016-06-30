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
            this.Name = "Wyvern";
            this.Title = "[Renowned]";
            this.Body = 62;
            this.Hue = 243;
            this.BaseSoundID = 362;

            this.SetStr(1370, 1422);
            this.SetDex(103, 151);
            this.SetInt(835, 1002);

            this.SetHits(2412, 2734);
            this.SetStam(103, 151);
            this.SetMana(835, 1002);

            this.SetDamage(29, 35);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Fire, 25);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 80, 90);
            this.SetResistance(ResistanceType.Cold, 70, 80);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 60, 70);

            this.SetSkill(SkillName.Magery, 107.7, 109.1);
            this.SetSkill(SkillName.Meditation, 63.9, 78.2);
            this.SetSkill(SkillName.EvalInt, 106.8, 111.1);
            this.SetSkill(SkillName.Wrestling, 108.6, 109.4);
            this.SetSkill(SkillName.MagicResist, 125.8, 127.6);
            this.SetSkill(SkillName.Tactics, 112.8, 123.7);

            this.Fame = 24000;
            this.Karma = -24000;

            this.VirtualArmor = 70;
            this.QLPoints = 50;

            this.PackItem(new EssenceDiligence());
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
            get { return new[] {typeof (AnimatedLegsoftheInsaneTinker), typeof (PillarOfStrength), typeof(StormCaller) }; }
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
            AddLoot(LootPack.UltraRich);
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