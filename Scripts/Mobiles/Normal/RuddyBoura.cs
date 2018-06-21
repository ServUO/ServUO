using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a boura corpse")]
    public class RuddyBoura : BaseCreature, ICarvable
    {
        private bool GatheredFur { get; set; }

        [Constructable]
        public RuddyBoura() : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a ruddy boura";
            Body = 715;

            SetStr(396, 480);
            SetDex(68, 82);
            SetInt(16, 20);

            SetHits(435, 509);
            SetStam(68, 82);
            SetMana(16, 20);

            SetDamage(16, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 35, 40);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Anatomy, 86.6, 88.8);
            SetSkill(SkillName.MagicResist, 69.7, 87.7);
            SetSkill(SkillName.Tactics, 83.3, 88.8);
            SetSkill(SkillName.Wrestling, 86.6, 87.9);

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 19.1;

            Fame = 5000;
            Karma = -2500;

            VirtualArmor = 16;
        }

        public RuddyBoura(Serial serial) : base(serial)
        {
        }

        public override int Meat
        {
            get { return 10; }
        }

        public override int Hides
        {
            get { return 20; }
        }

        public override int DragonBlood{ get{ return 8; } }
        public override bool DoesColossalBlow { get { return true; } }

        public override HideType HideType
        {
            get { return HideType.Spined; }
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; }
        }

        public override int Wool
        {
            get { return (Body == 0x2CB ? 3 : 0); }
        }

        public bool Carve(Mobile from, Item item)
        {
            if (!GatheredFur)
            {
                var fur = new BouraFur(30);

                if (from.Backpack == null || !from.Backpack.TryDropItem(from, fur, false))
                {
                    from.SendLocalizedMessage(1112352); // You would not be able to place the gathered boura fur in your backpack!
                    fur.Delete();
                }
                else
                {
                    from.SendLocalizedMessage(1112353); // You place the gathered boura fur into your backpack.
                    GatheredFur = true;

                    return true;
                }
            }
            else
                from.SendLocalizedMessage(1112354); // The boura glares at you and will not let you shear its fur.

            return false;
        }

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            base.OnCarve(from, corpse, with);

            if (!GatheredFur)
            {
                from.SendLocalizedMessage(1112765); // You shear it, and the fur is now on the corpse.
                corpse.AddCarvedItem(new BouraFur(15), from);
                GatheredFur = true;
            }
        }

        public override int GetIdleSound()
        {
            return 1507;
        }

        public override int GetAngerSound()
        {
            return 1504;
        }

        public override int GetHurtSound()
        {
            return 1506;
        }

        public override int GetDeathSound()
        {
            return 1505;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (!Controlled)
            c.DropItem(new BouraSkin());
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
            {
                GatheredFur = reader.ReadBool();
            }
        }
    }
}
