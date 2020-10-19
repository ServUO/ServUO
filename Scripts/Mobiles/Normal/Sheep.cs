using Server.Engines.Quests;
using Server.Items;
using Server.Network;
using System;

namespace Server.Mobiles
{
    [CorpseName("a sheep corpse")]
    public class Sheep : BaseCreature, ICarvable
    {
        private DateTime m_NextWoolTime;
        [Constructable]
        public Sheep()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a sheep";
            Body = 0xCF;
            BaseSoundID = 0xD6;

            SetStr(19);
            SetDex(25);
            SetInt(5);

            SetHits(12);
            SetMana(0);

            SetDamage(1, 2);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 5, 10);

            SetSkill(SkillName.MagicResist, 5.0);
            SetSkill(SkillName.Tactics, 6.0);
            SetSkill(SkillName.Wrestling, 5.0);

            Fame = 300;
            Karma = 0;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 11.1;
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
                return m_NextWoolTime;
            }
            set
            {
                m_NextWoolTime = value;
                Body = (DateTime.UtcNow >= m_NextWoolTime) ? 0xCF : 0xDF;
            }
        }
        public override int Meat => 3;
        public override MeatType MeatType => MeatType.LambLeg;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
        public override int Wool => (Body == 0xCF ? 3 : 0);
        public bool Carve(Mobile from, Item item)
        {
            if (DateTime.UtcNow < m_NextWoolTime)
            {
                // This sheep is not yet ready to be shorn.
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500449, from.NetState);
                return false;
            }

            from.SendLocalizedMessage(500452); // You place the gathered wool into your backpack.
            from.AddToBackpack(new Wool(Map == Map.Felucca ? 2 : 1));

            if (from is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)from;
                foreach (BaseQuest quest in player.Quests)
                {
                    if (quest is ShearingKnowledgeQuest)
                    {
                        if (!quest.Completed &&
                            (from.Map == Map.Trammel || from.Map == Map.Felucca))
                        {
                            from.AddToBackpack(new BritannianWool(1));
                        }
                        break;
                    }
                }
            }

            NextWoolTime = DateTime.UtcNow + TimeSpan.FromHours(2.0); // TODO: Proper time delay

            return true;
        }

        public override void OnThink()
        {
            base.OnThink();
            Body = (DateTime.UtcNow >= m_NextWoolTime) ? 0xCF : 0xDF;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);

            writer.WriteDeltaTime(m_NextWoolTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

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
