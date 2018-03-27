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
        public override double HealChance { get { return 1.0; } }

        [Constructable]
        public PestilentBandage()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)// NEED TO CHECK
        {
            Name = "a pestilent bandage";
            Body = 154;
            Hue = 0x515; 
            BaseSoundID = 471; 

            SetStr(691, 740);
            SetDex(141, 180);
            SetInt(51, 80);

            SetHits(415, 445);

            SetDamage(13, 23);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.Poisoning, 0.0, 10.0);
            SetSkill(SkillName.Anatomy, 0);
            SetSkill(SkillName.MagicResist, 75.0, 80.0);
            SetSkill(SkillName.Tactics, 80.0, 85.0);
            SetSkill(SkillName.Wrestling, 70.0, 75.0);

            Fame = 20000;
            Karma = -20000;

            // VirtualArmor = 28; // Don't know what it should be

            PackItem(new Bandage(5));  // How many?

            SetAreaEffect(AreaEffect.PoisonBreath);
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
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);  // Need to verify
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