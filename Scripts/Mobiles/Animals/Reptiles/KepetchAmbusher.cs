using System;
using Server.Items;
using Server.Network;
//
//

//

namespace Server.Mobiles
{
    [CorpseName("a kepetch corpse")]
    public class KepetchAmbusher : BaseCreature, ICarvable
    {
        private DateTime m_NextWoolTime;

        [Constructable]
        public KepetchAmbusher() : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a kepetch ambusher";
            Body = 726;

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

            PackItem(new DragonBlood(6));

            Fame = 2500;
            Karma = -2500;

            QLPoints = 25;

            //	VirtualArmor = 16;
        }

        public KepetchAmbusher(Serial serial) : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextWoolTime
        {
            get { return m_NextWoolTime; }
            set
            {
                m_NextWoolTime = value;
                Body = (DateTime.Now >= m_NextWoolTime) ? 0x2D6 : 0x2D7;
            }
        }

        public override int Meat
        {
            get { return 7; }
        }

        public override int Hides
        {
            get { return 14; }
        }

        // public override int DragonBlood { get { return 6; } }
        public override HideType HideType
        {
            get { return HideType.Horned; }
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; }
        }

        public override int Wool
        {
            get { return (Body == 0x2D6 ? 3 : 0); }
        }

        public void Carve(Mobile from, Item item)
        {
            if (DateTime.Now < m_NextWoolTime)
            {
                // The Kepetch nimbly escapes your attempts to shear its mane.
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1112358, from.NetState);
                return;
            }

            from.SendLocalizedMessage(1112360); // You place the gathered kepetch fur into your backpack.
            //from.AddToBackpack( new FurO( Map == Map.Felucca ? 2 : 15 ) );
            from.AddToBackpack(new Fur(Map == Map.Felucca ? 2 : 15));

            NextWoolTime = DateTime.Now + TimeSpan.FromHours(3.0); // TODO: Proper time delay
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.ShadowStrike;
        }

        public override void OnThink()
        {
            base.OnThink();
            Body = (DateTime.Now >= m_NextWoolTime) ? 0x2D6 : 0x2D7;
        }

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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); //0
            writer.WriteDeltaTime(m_NextWoolTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();

            switch (version)
            {
                case 1:
                {
                    NextWoolTime = reader.ReadDeltaTime();
                    break;
                }
            }
        }
    }
}