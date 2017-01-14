using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a frozen ogre lord's corpse")]
    [TypeAlias("Server.Mobiles.ArticOgreLord")]
    public class ArcticOgreLord : BaseCreature
    {
        [Constructable]
        public ArcticOgreLord()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an arctic ogre lord";
            this.Body = 135;
            this.BaseSoundID = 427;

            this.SetStr(767, 945);
            this.SetDex(66, 75);
            this.SetInt(46, 70);

            this.SetHits(476, 552);

            this.SetDamage(20, 25);

            this.SetDamageType(ResistanceType.Physical, 30);
            this.SetDamageType(ResistanceType.Cold, 70);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.MagicResist, 125.1, 140.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 15000;
            this.Karma = -15000;

            this.VirtualArmor = 50;

            this.PackItem(new Club());
        }

        public ArcticOgreLord(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Regular;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 3;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
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