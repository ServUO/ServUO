using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a kepetch corpse")]
    public class KepetchAmbusher : BaseCreature, ICarvable
    {
        public override bool CanStealth { get { return true; } } //Stays Hidden until Combatant in range.
        public bool GatheredFur { get; set; }

        [Constructable]
        public KepetchAmbusher()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a kepetch ambusher";
            Body = 726;
            Hidden = true;

            SetStr(440, 446);
            SetDex(229, 254);
            SetInt(46, 46);

            SetHits(533, 544);

            SetDamage(7, 17);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 73, 95);
            SetResistance(ResistanceType.Fire, 57, 70);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 70, 95);

            SetSkill(SkillName.Anatomy, 104.3, 114.1);
            SetSkill(SkillName.MagicResist, 94.6, 97.4);
            SetSkill(SkillName.Tactics, 110.4, 123.5);
            SetSkill(SkillName.Wrestling, 107.3, 113.9);
            SetSkill(SkillName.Stealth, 125.0);
            SetSkill(SkillName.Hiding, 125.0);

            Fame = 2500;
            Karma = -2500;

            PackItem(new RawRibs(5));
            //	VirtualArmor = 16;
        }

        public bool Carve(Mobile from, Item item)
        {
            if (!GatheredFur)
            {
                var fur = new KepetchFur(30);

                if (from.Backpack == null || !from.Backpack.TryDropItem(from, fur, false))
                {
                    from.SendLocalizedMessage(1112359); // You would not be able to place the gathered kepetch fur in your backpack!
                    fur.Delete();
                }
                else
                {
                    from.SendLocalizedMessage(1112360); // You place the gathered kepetch fur into your backpack.
                    GatheredFur = true;
                    return true;
                }
            }
            else
                from.SendLocalizedMessage(1112358); // The Kepetch nimbly escapes your attempts to shear its mane.

            return false;
        }

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            base.OnCarve(from, corpse, with);

            if (!GatheredFur)
            {
                from.SendLocalizedMessage(1112765); // You shear it, and the fur is now on the corpse.
                corpse.AddCarvedItem(new KepetchFur(15), from);
                GatheredFur = true;
            }
        }

        public KepetchAmbusher(Serial serial)
            : base(serial)
        {
        }

        //Can Flush them out of Hiding
        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            RevealingAction();
            base.OnDamage(amount, from, willKill);
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            RevealingAction();
            base.OnDamagedBySpell(from);
        }

        public override int Meat
        {
            get { return 7; }
        }

        public override int Hides
        {
            get { return 12; }
        }

        public override HideType HideType
        {
            get { return HideType.Horned; }
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; }
        }

        public override int DragonBlood { get { return 8; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
        }

        public override int GetIdleSound()
        {
            return 1545;
        }

        public override int GetAngerSound()
        {
            return 1542;
        }

        public override int GetHurtSound()
        {
            return 1544;
        }

        public override int GetDeathSound()
        {
            return 1543;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.1)
            {
                c.DropItem(new KepetchWax());
            }
        }

        public override void OnThink()
        {

            if (!this.Alive || this.Deleted)
            {
                return;
            }

            if (!this.Hidden)
            {
                double chance = 0.05;

                if (this.Hits < 20)
                {
                    chance = 0.1;
                }

                if (this.Poisoned)
                {
                    chance = 0.01;
                }

                if (Utility.RandomDouble() < chance)
                {
                    HideSelf();
                }
                base.OnThink();
            }
        }

        private void HideSelf()
        {
            if (Core.TickCount >= this.NextSkillTime)
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);

                this.PlaySound(0x22F);
                this.Hidden = true;

                this.UseSkill(SkillName.Stealth);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2);
            writer.Write(GatheredFur);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();

            if (version == 1)
                reader.ReadDeltaTime();
            else
                GatheredFur = reader.ReadBool();
        }
    }
}