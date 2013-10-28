using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a pestilent bandage corpse")]
    public class PestilentBandage : BaseCreature
    {
        // Neither Stratics nor UOGuide have much description 
        // beyond being a "Grey Mummy". BodyValue, Sound and 
        // Hue are all guessed until they can be verified.
        // Loot and Fame/Karma are also guesses at this point.
        //
        // They also apparently have a Poison Attack, which I've stolen from Yamandons.
        [Constructable]
        public PestilentBandage()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)// NEED TO CHECK
        {
            this.Name = "a pestilent bandage";
            this.Body = 154;
            this.Hue = 0x515; 
            this.BaseSoundID = 471; 

            this.SetStr(691, 740);
            this.SetDex(141, 180);
            this.SetInt(51, 80);

            this.SetHits(415, 445);

            this.SetDamage(13, 23);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.Poisoning, 0.0, 10.0);
            this.SetSkill(SkillName.Anatomy, 0);
            this.SetSkill(SkillName.MagicResist, 75.0, 80.0);
            this.SetSkill(SkillName.Tactics, 80.0, 85.0);
            this.SetSkill(SkillName.Wrestling, 70.0, 75.0);

            this.Fame = 20000;
            this.Karma = -20000;

            // VirtualArmor = 28; // Don't know what it should be

            this.PackItem(new Bandage(5));  // How many?
        }

        public PestilentBandage(Serial serial)
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
        public override bool CanHeal
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);  // Need to verify
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