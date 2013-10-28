using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a kepetch corpse")]
    public class Kepetch : BaseCreature, ICarvable
    {
        private DateTime m_NextWoolTime;
        [Constructable]
        public Kepetch()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a kepetch";
            this.Body = 726;

            this.SetStr(337, 354);
            this.SetDex(184, 194);
            this.SetInt(32, 37);

            this.SetHits(308, 366);

            this.SetDamage(7, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 40, 45);
            this.SetResistance(ResistanceType.Cold, 45, 55);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 65, 75);

            this.SetSkill(SkillName.Anatomy, 119.7, 124.1);
            this.SetSkill(SkillName.MagicResist, 89.9, 97.4);
            this.SetSkill(SkillName.Tactics, 117.4, 123.5);
            this.SetSkill(SkillName.Wrestling, 107.7, 113.9);
        }

        public Kepetch(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextWoolTime
        {
            get
            {
                return this.m_NextWoolTime;
            }
            set
            {
                this.m_NextWoolTime = value;
                this.Body = (DateTime.UtcNow >= this.m_NextWoolTime) ? 0xCF : 0xDF;
            }
        }
        public override int Meat
        {
            get
            {
                return 5;
            }
        }
        public override int Hides
        {
            get
            {
                return 14;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }
        public override int Wool
        {
            get
            {
                return (this.Body == 726 ? 3 : 0);
            }
        }
        public void Carve(Mobile from, Item item)
        {
            if (DateTime.UtcNow < this.m_NextWoolTime)
            {
                // This sheep is not yet ready to be shorn.
                this.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500449, from.NetState);
                return;
            }

            from.SendLocalizedMessage(500452); // You place the gathered wool into your backpack.
            from.AddToBackpack(new Wool(this.Map == Map.Felucca ? 2 : 1));

            this.NextWoolTime = DateTime.UtcNow + TimeSpan.FromHours(3.0); // TODO: Proper time delay
        }

        public override void OnThink()
        {
            base.OnThink();
            this.Body = (DateTime.UtcNow >= this.m_NextWoolTime) ? 726 : 727;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average, 2);
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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);

            writer.WriteDeltaTime(this.m_NextWoolTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.NextWoolTime = reader.ReadDeltaTime();
                        break;
                    }
            }
        }
    }
}