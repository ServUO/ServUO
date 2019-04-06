using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a slimey corpse")]
    public class CorrosiveSlime : BaseCreature
    {
        [Constructable]
        public CorrosiveSlime()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a corrosive slime";
            Body = 51;
            BaseSoundID = 456;

            Hue = Utility.RandomSlimeHue();

            SetStr(22, 34);
            SetDex(16, 21);
            SetInt(16, 20);

            SetHits(15, 19);

            SetDamage(1, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 5, 10);
            SetResistance(ResistanceType.Poison, 15, 20);

            SetSkill(SkillName.Poisoning, 36.0, 49.1);
            SetSkill(SkillName.Anatomy, 0);
            SetSkill(SkillName.MagicResist, 15.9, 18.9);
            SetSkill(SkillName.Tactics, 24.6, 26.1);
            SetSkill(SkillName.Wrestling, 24.9, 26.1);

            Fame = 300;
            Karma = -300;

            VirtualArmor = 8;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 23.1;
        }
           
        public CorrosiveSlime(Serial serial)
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
        public override Poison HitPoison
        {
            get
            {
                return Poison.Regular;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Fish;
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
            AddLoot(LootPack.Gems);
        }
        public override void OnDeath(Container c)
        {
            if (!Controlled && Map != null && Map != Map.TerMur && Utility.Random(10) == 0)
            {
                Item item = null;

                switch (Utility.Random(3))
                {
                    case 0: item = new GelatanousSkull(); break;
                    case 1: item = new CoagulatedLegs(); break;
                    case 2: item = new PartiallyDigestedTorso(); break;
                }

				if (item != null)
					c.DropItem(item);
            }

            base.OnDeath(c);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0 && (AbilityProfile == null || AbilityProfile.MagicalAbility == MagicalAbility.None))
            {
                SetMagicalAbility(MagicalAbility.Poisoning);
            }
        }
    }
}
