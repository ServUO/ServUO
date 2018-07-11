using System;
using Server.Gumps;

namespace Server.Items
{
    public class GreenGoblinStatuette : MonsterStatuette
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile User { get; set; }

        [Constructable]
        public GreenGoblinStatuette()
            : base(MonsterStatuetteType.GreenGoblin)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (TurnedOn)
                {
                    TurnOff();
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
                        TurnOn(from);
                    }
                }
            }
        }

        public void TurnOn(Mobile from)
        {
            User = from;
            TurnedOn = true;

            from.BodyMod = 723;
            from.HueMod = 0;

            ItemID = 0xA098;

            from.FixedParticles(0x3728, 1, 13, 5042, EffectLayer.Waist);
        }

        public void TurnOff()
        {
            TurnedOn = false;

            if (User != null)
            {
                User.BodyMod = 0;
                User.HueMod = -1;
            }

            ItemID = 0xA097;

            User = null;
        }

        public override bool OnDragLift(Mobile from)
        {
            if (TurnedOn)
            {
                TurnOff();
            }

            return base.OnDragLift(from);
        }

        public override void OnDelete()
        {
            if (TurnedOn)
            {
                TurnOff();
            }

            base.OnDelete();
        }

        public override void OnItemRemoved(Item item)
        {
            if (TurnedOn)
            {
                TurnOff();
            }

            base.OnItemRemoved(item);
        }

        public GreenGoblinStatuette(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
            writer.Write(User);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch(version)
            {
                case 1:
                    User = reader.ReadMobile();
                    break;
            }

            if(User != null)
            {
                TurnOn(User);
            }
        }
    }
}
