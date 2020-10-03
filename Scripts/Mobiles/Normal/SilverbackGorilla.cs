using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a silverback gorilla corpse")]
    public class SilverbackGorilla : BaseCreature
    {
        [Constructable]
        public SilverbackGorilla()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "a silverback gorilla";
            Body = 0x1D;
            BaseSoundID = 0x9E;

            SetStr(79, 106);
            SetDex(77, 91);
            SetInt(16, 29);

            SetDamage(5, 10);

            SetHits(446, 588);
            SetMana(0);

            SetResistance(ResistanceType.Physical, 20);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetSkill(SkillName.MagicResist, 30.1, 43.5);
            SetSkill(SkillName.Tactics, 30.1, 49.0);
            SetSkill(SkillName.Wrestling, 40, 50);

            Fame = 5000;
            Karma = -5000;
        }

        public override int Meat => 1;
        public override int Hides => 7;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies;

        private DateTime _NextBanana;
        private int _Thrown;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootGold(60, 70));
            AddLoot(LootPack.LootItemCallback(TryDropBannana, 25.0, Utility.RandomMinMax(1, 5), false, false));
        }

        public override void OnActionCombat()
        {
            Mobile combatant = Combatant as Mobile;

            if (DateTime.UtcNow < _NextBanana || combatant == null || combatant.Deleted || combatant.Map != Map || !InRange(combatant, 12) || !CanBeHarmful(combatant) || !InLOS(combatant))
                return;

            ThrowBanana(combatant);

            _Thrown++;

            if (0.75 >= Utility.RandomDouble() && (_Thrown % 2) == 1) // 75% chance to quickly throw another bomb
                _NextBanana = DateTime.UtcNow + TimeSpan.FromSeconds(3.0);
            else
                _NextBanana = DateTime.UtcNow + TimeSpan.FromSeconds(5.0 + (10.0 * Utility.RandomDouble())); // 5-15 seconds
        }

        public void ThrowBanana(Mobile m)
        {
            DoHarmful(m);

            MovingParticles(m, Utility.RandomList(0x171f, 0x1720, 0x1721, 0x1722), 10, 0, false, true, 0, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
            {
                m.PlaySound(0x11D);
                AOS.Damage(m, this, Utility.RandomMinMax(50, 85), 100, 0, 0, 0, 0);
            });
        }

        private Item TryDropBannana(IEntity e)
        {
            if (Region.Find(e.Location, e.Map).IsPartOf("GreatApeLair"))
                return new PerfectBanana();

            return null;
        }

        public SilverbackGorilla(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
