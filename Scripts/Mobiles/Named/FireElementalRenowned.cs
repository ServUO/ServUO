using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Fire Elemental [Renowned] corpse")] 
    public class FireElementalRenowned : BaseRenowned
    {
        [Constructable]
        public FireElementalRenowned()
            : base(AIType.AI_Mage)
        {
            this.Name = "Fire Elemental";
            this.Title = "[Renowned]";
            this.Body = 15;
            this.BaseSoundID = 838;

            this.Hue = 1161;

            this.SetStr(450, 500);
            this.SetDex(200, 250);
            this.SetInt(300, 350);

            this.SetHits(1200, 1600);

            this.SetDamage(7, 9);

            this.SetDamageType(ResistanceType.Physical, 25);
            this.SetDamageType(ResistanceType.Fire, 75);

            this.SetResistance(ResistanceType.Physical, 45, 60);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Cold, 5, 10);
            this.SetResistance(ResistanceType.Poison, 30, 50);
            this.SetResistance(ResistanceType.Energy, 40, 60);

            this.SetSkill(SkillName.EvalInt, 100.1, 110.0);
            this.SetSkill(SkillName.Magery, 105.1, 110.0);
            this.SetSkill(SkillName.MagicResist, 110.2, 120.0);
            this.SetSkill(SkillName.Tactics, 100.1, 105.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 4500;
            this.Karma = -4500;

            this.VirtualArmor = 40;
            this.QLPoints = 50;

            this.PackItem(new EssencePrecision());

            this.ControlSlots = 4;

            this.PackItem(new SulfurousAsh(3));

            this.AddItem(new LightSource());
        }

        public FireElementalRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[] { typeof(JadeWarAxe) };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[] { typeof(TokenOfHolyFavor), typeof(SwordOfShatteredHopes) };
            }
        }
        public override double DispelDifficulty
        {
            get
            {
                return 117.5;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 45.0;
            }
        }
        public override bool BleedImmune
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
                return 2;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich);
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

            if (this.BaseSoundID == 274)
                this.BaseSoundID = 838;
        }
    }
}