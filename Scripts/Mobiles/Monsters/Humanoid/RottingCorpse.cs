using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a rotting corpse")]
    public class RottingCorpse : BaseCreature
    {
        [Constructable]
        public RottingCorpse()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a rotting corpse";
            this.Body = 155;
            this.BaseSoundID = 471;

            this.SetStr(301, 350);
            this.SetDex(75);
            this.SetInt(151, 200);

            this.SetHits(1200);
            this.SetStam(150);
            this.SetMana(0);

            this.SetDamage(8, 10);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Cold, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 50, 70);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.Poisoning, 120.0);
            this.SetSkill(SkillName.MagicResist, 250.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 6000;
            this.Karma = -6000;

            this.VirtualArmor = 40;
        }

        public RottingCorpse(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune
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
                return Poison.Lethal;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
        }
        public override void OnDeath(Container c)
        {

            base.OnDeath(c);
            Region reg = Region.Find(c.GetWorldLocation(), c.Map);
            if (0.25 > Utility.RandomDouble() && reg.Name == "The Lands of the Lich")
            {
                if (Utility.RandomDouble() < 0.6)
                    c.DropItem(new EssenceDirection());
            }
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