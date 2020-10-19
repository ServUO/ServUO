using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a boura corpse")]
    public class LowlandBoura : BaseCreature, ICarvable
    {
        private bool GatheredFur { get; set; }

        [Constructable]
        public LowlandBoura() : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a lowland boura";
            Body = 715;

            SetStr(337, 411);
            SetDex(82, 93);
            SetInt(23, 25);

            SetHits(438, 553);

            SetDamage(18, 23);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 35, 40);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Anatomy, 81.2, 84.4);
            SetSkill(SkillName.MagicResist, 70.7, 75.0);
            SetSkill(SkillName.Tactics, 83.4, 86.7);
            SetSkill(SkillName.Wrestling, 95.1, 97.3);

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 19.1;

            Fame = 5000;
            Karma = -3500;

            SetSpecialAbility(SpecialAbility.ColossalBlow);
        }

        public LowlandBoura(Serial serial) : base(serial)
        {
        }

        public override int Meat => 10;
        public override int Hides => 20;
        public override int DragonBlood => 8;
        public override HideType HideType => HideType.Horned;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies;
        public override int Fur => GatheredFur ? 0 : 30;
        public override FurType FurType => FurType.Green;

        public bool Carve(Mobile from, Item item)
        {
            if (!GatheredFur)
            {
                Fur fur = new Fur(FurType, Fur);

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
            {
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1112354, from.NetState); // The boura glares at you and will not let you shear its fur.
            }

            return false;
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

            if (Controlled)
                return;

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
            int version = reader.ReadInt();

            if (version == 1)
                reader.ReadDeltaTime();
            else
            {
                GatheredFur = reader.ReadBool();
            }
        }
    }
}
