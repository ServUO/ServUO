using Server.Items;
using System;

namespace Server.Mobiles
{
    public class BaneDragon : BaseMount
    {
        public static readonly int MaxPower = 10;

        [CommandProperty(AccessLevel.GameMaster)]
        public int PowerLevel { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PowerDecay
        {
            get { return _PowerDecay; }
            set
            {
                _PowerDecay = value;

                if (_PowerDecay >= 10)
                {
                    _PowerDecay = 0;
                    PowerLevel = Math.Max(1, PowerLevel - 1);
                }
            }
        }

        private DateTime _NextSpecial;
        private int _PowerDecay;

        [Constructable]
        public BaneDragon()
            : base("Bane Dragon", 0x31A, 0x3EBD, AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            BaseSoundID = 0x16A;
            Hue = 1175;

            SetStr(500, 555);
            SetDex(85, 125);
            SetInt(100, 165);

            SetHits(550, 650);

            SetDamage(20, 26);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 25);
            SetDamageType(ResistanceType.Poison, 25);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 30, 45);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 20, 40);

            SetSkill(SkillName.Anatomy, 10.0);
            SetSkill(SkillName.MagicResist, 85.0);
            SetSkill(SkillName.Tactics, 110.0);
            SetSkill(SkillName.Wrestling, 90.0);
            SetSkill(SkillName.Magery, 45.0);
            SetSkill(SkillName.EvalInt, 35.0);
            SetSkill(SkillName.Meditation, 35.0);

            Fame = 18000;
            Karma = -18000;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 107.1;

            PowerLevel = 5;
            _NextSpecial = DateTime.UtcNow;
        }

		public override bool SubdueBeforeTame => true;
        public override bool StatLossAfterTame => false;
        public override Poison HitPoison => Poison.Lethal;
        public override bool AlwaysMurderer => !Controlled;
        public override FoodType FavoriteFood => FoodType.BlackrockStew;

        public override bool CheckFeed(Mobile from, Item dropped)
        {
            if (dropped is BowlOfBlackrockStew)
            {
                if (PowerLevel >= MaxPower)
                {
                    from.SendLocalizedMessage(1115755); // The creature looks at you strangely and shakes its head no.
                }
                else
                {
                    PowerLevel++;

                    if (PowerLevel >= MaxPower)
                    {
                        from.SendLocalizedMessage(1115753); // Your bane dragon is returned to maximum power by this stew.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1115754); // Your bane dragon seems a bit peckish today and is not at full power.
                    }

                    return base.CheckFeed(from, dropped);
                }
            }

            return false;
        }

        public override void OnHarmfulSpell(Mobile from)
        {
            if (_NextSpecial < DateTime.UtcNow)
            {
                DoSpecial(from);

                _NextSpecial = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(15, 30) * (11.0 - PowerLevel));
            }
        }

        public void DoSpecial(Mobile from)
        {
            if (Controlled)
            {
                PowerDecay++;
            }

            MovingParticles(from, 0x36D4, 7, 0, false, true, 1163, 0, 9502, 4019, 0x160, 0);
            PlaySound(0x15E);

            Timer.DelayCall(TimeSpan.FromSeconds(1), m =>
                {
                    AOS.Damage(m, this, Utility.RandomMinMax(8 * PowerLevel, 10 * PowerLevel), 0, 50, 0, 50, 0);
                    m.ApplyPoison(this, GetHitPoison());
                }, from);
        }

        public BaneDragon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write(PowerLevel);
            writer.Write(PowerDecay);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    PowerLevel = reader.ReadInt();
                    PowerDecay = reader.ReadInt();
                    break;
                case 0: break;
            }
        }
    }
}
