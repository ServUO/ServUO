using System;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;

namespace Server.Items
{
    public class EnchantedApple : BaseMagicalFood
    {
        [Constructable]
        public EnchantedApple()
            : base(0x2FD8)
        {
            Weight = 1.0;
            Hue = 0x488;
	        Stackable = true;
        }

        public EnchantedApple(Serial serial)
            : base(serial)
        {
        }

        public override MagicalFood FoodID
        {
            get
            {
                return MagicalFood.EnchantedApple;
            }
        }
        public override TimeSpan Cooldown
        {
            get
            {
                return TimeSpan.FromSeconds(30);
            }
        }
        public override int EatMessage
        {
            get
            {
                return 1074846;
            }
        }// A tasty bite of the enchanted apple lifts all curses from your soul.
        public override bool Eat(Mobile from)
        {
            if (base.Eat(from))
            {
                from.PlaySound(0xF6);
                from.PlaySound(0x1F7);
                from.FixedParticles(0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head);

                IEntity mfrom = new Entity(Serial.Zero, new Point3D(from.X, from.Y, from.Z - 10), from.Map);
                IEntity mto = new Entity(Serial.Zero, new Point3D(from.X, from.Y, from.Z + 50), from.Map);
                Effects.SendMovingParticles(mfrom, mto, 0x2255, 1, 0, false, false, 13, 3, 9501, 1, 0, EffectLayer.Head, 0x100);

                from.RemoveStatMod("[Magic] Str Curse");
				from.RemoveStatMod("[Magic] Dex Curse");
				from.RemoveStatMod("[Magic] Int Curse");

                from.Asleep = false;

                EvilOmenSpell.TryEndEffect(from);
                StrangleSpell.RemoveCurse(from);
                CorpseSkinSpell.RemoveCurse(from);
                CurseSpell.RemoveEffect(from);
				MortalStrike.EndWound(from);
	            BloodOathSpell.RemoveCurse(from);
				MindRotSpell.ClearMindRotScalar(from);

                BuffInfo.RemoveBuff(from, BuffIcon.Clumsy);
                BuffInfo.RemoveBuff(from, BuffIcon.FeebleMind);
                BuffInfo.RemoveBuff(from, BuffIcon.Weaken);
                BuffInfo.RemoveBuff(from, BuffIcon.MassCurse);
				BuffInfo.RemoveBuff(from, BuffIcon.Curse);
				BuffInfo.RemoveBuff(from, BuffIcon.MortalStrike);
				BuffInfo.RemoveBuff(from, BuffIcon.Mindrot);
				BuffInfo.RemoveBuff(from, BuffIcon.CorpseSkin);
				
                return true;
            }
			
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}