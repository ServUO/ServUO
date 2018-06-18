using System;
using Server.Gumps;

namespace Server.Items
{
    public class GreyGoblinStatuette : MonsterStatuette
    {
        [Constructable]
        public GreyGoblinStatuette()
            : base(MonsterStatuetteType.GreyGoblin)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (TurnedOn)
                {
                    TurnedOn = false;

                    from.BodyMod = 0;
                }
                else
                {
                    if (from.Mounted)
                    {
                        from.SendLocalizedMessage(1010097); // You cannot use this while mounted.
                    }
                    else if (from.Flying)
                    {
                        from.SendLocalizedMessage(1113414); // You can't use this while flying!
                    }
                    else if (from.IsBodyMod)
                    {
                        from.SendLocalizedMessage(1111896); // You may only change forms while in your original body.
                    }
                    else
                    {
                        TurnedOn = true;

                        from.BodyMod = 334;
                        from.HueMod = 0;

                        from.FixedParticles(0x3728, 1, 13, 5042, EffectLayer.Waist);
                    }
                }
            }
        }

        public GreyGoblinStatuette(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
