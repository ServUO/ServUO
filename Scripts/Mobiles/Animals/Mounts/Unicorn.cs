using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a unicorn corpse")]
    public class Unicorn : BaseMount
    {
        [Constructable]
        public Unicorn()
            : this("a unicorn")
        {
        }

        [Constructable]
        public Unicorn(string name)
            : base(name, 0x7A, 0x3EB4, AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            this.BaseSoundID = 0x4BC;

            this.SetStr(296, 325);
            this.SetDex(96, 115);
            this.SetInt(186, 225);

            this.SetHits(191, 210);

            this.SetDamage(16, 22);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 25, 40);
            this.SetResistance(ResistanceType.Cold, 25, 40);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 25, 40);

            this.SetSkill(SkillName.EvalInt, 80.1, 90.0);
            this.SetSkill(SkillName.Magery, 60.2, 80.0);
            this.SetSkill(SkillName.Meditation, 50.1, 60.0);
            this.SetSkill(SkillName.MagicResist, 75.3, 90.0);
            this.SetSkill(SkillName.Tactics, 20.1, 22.5);
            this.SetSkill(SkillName.Wrestling, 80.5, 92.5);

            this.Fame = 9000;
            this.Karma = 9000;

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 95.1;
        }

        public Unicorn(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowMaleRider
        {
            get
            {
                return false;
            }
        }
        public override bool AllowMaleTamer
        {
            get
            {
                return false;
            }
        }
        public override bool InitialInnocent
        {
            get
            {
                return true;
            }
        }
        public override TimeSpan MountAbilityDelay
        {
            get
            {
                return TimeSpan.FromHours(1.0);
            }
        }
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int Meat
        {
            get
            {
                return 3;
            }
        }
        public override int Hides
        {
            get
            {
                return 10;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Horned;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }
        public override void OnDisallowedRider(Mobile m)
        {
            m.SendLocalizedMessage(1042318); // The unicorn refuses to allow you to ride it.
        }

        public override bool DoMountAbility(int damage, Mobile attacker)
        {
            if (this.Rider == null || attacker == null)	//sanity
                return false;

            if (this.Rider.Poisoned && ((this.Rider.Hits - damage) < 40))
            {
                Poison p = this.Rider.Poison;

                if (p != null)
                {
                    int chanceToCure = 10000 + (int)(this.Skills[SkillName.Magery].Value * 75) - ((p.RealLevel + 1) * (Core.AOS ? (p.RealLevel < 4 ? 3300 : 3100) : 1750));
                    chanceToCure /= 100;

                    if (chanceToCure > Utility.Random(100))
                    {
                        if (this.Rider.CurePoison(this))	//TODO: Confirm if mount is the one flagged for curing it or the rider is
                        {
                            this.Rider.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, true, "Your mount senses you are in danger and aids you with magic.");
                            this.Rider.FixedParticles(0x373A, 10, 15, 5012, EffectLayer.Waist);
                            this.Rider.PlaySound(0x1E0);	// Cure spell effect.
                            this.Rider.PlaySound(0xA9);		// Unicorn's whinny.

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.LowScrolls);
            this.AddLoot(LootPack.Potions);
        }

		public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.3)
                c.DropItem(new UnicornRibs());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}