using System;
using Server.Mobiles;

namespace Server.Factions
{
    [CorpseName("a war horse corpse")]
    public class FactionWarHorse : BaseMount
    {
        public const int SilverPrice = 500;
        public const int GoldPrice = 3000;
        private Faction m_Faction;
        [Constructable]
        public FactionWarHorse()
            : this(null)
        {
        }

        public FactionWarHorse(Faction faction)
            : base("a war horse", 0xE2, 0x3EA0, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.BaseSoundID = 0xA8;

            this.SetStr(400);
            this.SetDex(125);
            this.SetInt(51, 55);

            this.SetHits(240);
            this.SetMana(0);

            this.SetDamage(5, 8);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 25.1, 30.0);
            this.SetSkill(SkillName.Tactics, 29.3, 44.0);
            this.SetSkill(SkillName.Wrestling, 29.3, 44.0);

            this.Fame = 300;
            this.Karma = 300;

            this.Tamable = true;
            this.ControlSlots = 1;

            this.Faction = faction;
        }

        public FactionWarHorse(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public Faction Faction
        {
            get
            {
                return this.m_Faction;
            }
            set
            {
                this.m_Faction = value;

                this.Body = (this.m_Faction == null ? 0xE2 : this.m_Faction.Definition.WarHorseBody);
                this.ItemID = (this.m_Faction == null ? 0x3EA0 : this.m_Faction.Definition.WarHorseItem);
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            PlayerState pl = PlayerState.Find(from);

            if (pl == null)
                from.SendLocalizedMessage(1010366); // You cannot mount a faction war horse!
            else if (pl.Faction != this.Faction)
                from.SendLocalizedMessage(1010367); // You cannot ride an opposing faction's war horse!
            else if (pl.Rank.Rank < 2)
                from.SendLocalizedMessage(1010368); // You must achieve a faction rank of at least two before riding a war horse!
            else
                base.OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            Faction.WriteReference(writer, this.m_Faction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.Faction = Faction.ReadReference(reader);
                        break;
                    }
            }
        }
    }
}