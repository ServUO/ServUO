using System;
using Server.Items;
using Server.Network;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    [CorpseName("a sheep corpse")]
    public class Sheep : BaseCreature, ICarvable
    {
        private DateTime m_NextWoolTime;
        [Constructable]
        public Sheep()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a sheep";
            this.Body = 0xCF;
            this.BaseSoundID = 0xD6;

            this.SetStr(19);
            this.SetDex(25);
            this.SetInt(5);

            this.SetHits(12);
            this.SetMana(0);

            this.SetDamage(1, 2);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);

            this.SetSkill(SkillName.MagicResist, 5.0);
            this.SetSkill(SkillName.Tactics, 6.0);
            this.SetSkill(SkillName.Wrestling, 5.0);

            this.Fame = 300;
            this.Karma = 0;

            this.VirtualArmor = 6;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 11.1;
        }

        public Sheep(Serial serial)
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
                return 3;
            }
        }
        public override MeatType MeatType
        {
            get
            {
                return MeatType.LambLeg;
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
                return (this.Body == 0xCF ? 3 : 0);
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

            if (from is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)from;
                foreach(BaseQuest quest in player.Quests)
                {
                    if(quest is ShearingKnowledgeQuest)
                    {
                        if(!quest.Completed && 
                            (from.Map == Map.Trammel || from.Map == Map.Felucca))
                        {
                            from.AddToBackpack(new BritannianWool(1));
                        }
                        break;
                    }
                }
            }

            this.NextWoolTime = DateTime.UtcNow + TimeSpan.FromHours(3.0); // TODO: Proper time delay
        }

        public override void OnThink()
        {
            base.OnThink();
            this.Body = (DateTime.UtcNow >= this.m_NextWoolTime) ? 0xCF : 0xDF;
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