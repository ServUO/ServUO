using System;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a ki-rin corpse")]
    public class Kirin : BaseMount
    {
        [Constructable]
        public Kirin()
            : this("a ki-rin")
        {
        }

        [Constructable]
        public Kirin(string name)
            : base(name, 132, 0x3EAD, AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            this.BaseSoundID = 0x3C5;

            this.SetStr(296, 325);
            this.SetDex(86, 105);
            this.SetInt(186, 225);

            this.SetHits(191, 210);

            this.SetDamage(16, 22);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Fire, 10);
            this.SetDamageType(ResistanceType.Cold, 10);
            this.SetDamageType(ResistanceType.Energy, 10);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 35, 45);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.EvalInt, 80.1, 90.0);
            this.SetSkill(SkillName.Magery, 60.4, 100.0);
            this.SetSkill(SkillName.Meditation, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 85.3, 100.0);
            this.SetSkill(SkillName.Tactics, 20.1, 22.5);
            this.SetSkill(SkillName.Wrestling, 80.5, 92.5);

            this.Fame = 9000;
            this.Karma = 9000;

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 95.1;
        }

        public Kirin(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowFemaleRider
        {
            get
            {
                return false;
            }
        }
        public override bool AllowFemaleTamer
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

        public override TribeType Tribe { get { return TribeType.Fey; } }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
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
            m.SendLocalizedMessage(1042319); // The Ki-Rin refuses your attempts to mount it.
        }

        public override bool DoMountAbility(int damage, Mobile attacker)
        {
            if (this.Rider == null || attacker == null)	//sanity
                return false;

            if ((this.Rider.Hits - damage) < 30 && this.Rider.Map == attacker.Map && this.Rider.InRange(attacker, 18))	//Range and map checked here instead of other base fuction because of abiliites that don't need to check this
            {
                attacker.BoltEffect(0);
                // 35~100 damage, unresistable, by the Ki-rin.
                attacker.Damage(Utility.RandomMinMax(35, 100), this, false);	//Don't inform mount about this damage, Still unsure wether or not it's flagged as the mount doing damage or the player.  If changed to player, without the extra bool it'd be an infinite loop

                this.Rider.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1042534);	// Your mount calls down the forces of nature on your opponent.
                this.Rider.FixedParticles(0, 0, 0, 0x13A7, EffectLayer.Waist);
                this.Rider.PlaySound(0xA9);	// Ki-rin's whinny.
                return true;
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
                c.DropItem(new KirinBrains());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                this.AI = AIType.AI_Mage;
        }
    }
}
