using System;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SpectralArmour : BaseCreature 
    { 
        [Constructable] 
        public SpectralArmour()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        { 
            this.Body = 637; 
            this.Hue = 0x8026; 
            this.Name = "spectral armour"; 

            Buckler buckler = new Buckler();
            ChainCoif coif = new ChainCoif();
            PlateGloves gloves = new PlateGloves();

            buckler.Hue = 0x835;
            buckler.Movable = false;
            coif.Hue = 0x835;
            gloves.Hue = 0x835;

            this.AddItem(buckler);
            this.AddItem(coif);
            this.AddItem(gloves);

            this.SetStr(101, 110); 
            this.SetDex(101, 110); 
            this.SetInt(101, 110);

            this.SetHits(178, 201);
            this.SetStam(191, 200);

            this.SetDamage(10, 22);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Cold, 25);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.Wrestling, 75.1, 100.0); 
            this.SetSkill(SkillName.Tactics, 90.1, 100.0); 
            this.SetSkill(SkillName.MagicResist, 90.1, 100); 

            this.VirtualArmor = 40; 
            this.Fame = 7000; 
            this.Karma = -7000;             
        }

        public SpectralArmour(Serial serial)
            : base(serial)
        { 
        }

        public override bool DeleteCorpseOnDeath
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Regular;
            }
        }
        public override int GetIdleSound()
        {
            return 0x200;
        }

        public override int GetAngerSound()
        {
            return 0x56;
        }

        public override bool OnBeforeDeath()
        {
            if (!base.OnBeforeDeath())
                return false;

            Gold gold = new Gold(Utility.RandomMinMax(240, 375));
            gold.MoveToWorld(this.Location, this.Map);

            Effects.SendLocationEffect(this.Location, this.Map, 0x376A, 10, 1);
            return true;
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