using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a krakens corpse")]
    public class Kraken : BaseCreature
    {
        

        [Constructable]
        public Kraken()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a kraken";
            this.Body = 77;
            this.BaseSoundID = 353;

            this.SetStr(756, 780);
            this.SetDex(226, 245);
            this.SetInt(26, 40);

            this.SetHits(454, 468);
            this.SetMana(0);

            this.SetDamage(19, 33);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Cold, 30);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 15.1, 20.0);
            this.SetSkill(SkillName.Tactics, 45.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 45.1, 60.0);

            this.Fame = 11000;
            this.Karma = -11000;

            this.VirtualArmor = 50;

            this.CanSwim = true;
            this.CantWalk = true;

            //Rope is supposed to be a rare drop.  ref UO Guide Kraken
            if (Utility.RandomDouble() < .05)
            {
                Rope rope = new Rope();
                rope.ItemID = 0x14F8;
                this.PackItem(rope);
            }

                       
        }

        public Kraken(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel
        {
            get
            {
                return 4;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
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