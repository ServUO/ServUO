using Server;
using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.ArenaSystem
{
    public class ArenaGate : Moongate
    {
        public override bool ForceShowProperties { get { return true; } }
        public override int LabelNumber { get { return 1115879; } } // Arena Gate

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaDuel Duel { get; set; }

        [Constructable]
        public ArenaGate(ArenaDuel duel)
            : base(0xF6C)
        {
            Duel = duel;
            Movable = false;

            Hue = 1194;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 1))
                TryUse(from);
        }

        public override bool OnMoveOver(Mobile m)
        {
            TryUse(m);

            return true;
        }

        public void TryUse(Mobile m)
        {
            if (m is PlayerMobile && CheckValidation((PlayerMobile)m))
            {
                Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                {
                    Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
                    Duel.MoveToArena((PlayerMobile)m);
                    Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
                });
            }
        }

        private bool CheckValidation(PlayerMobile pm)
        {
            if (pm.ControlSlots > Duel.PetSlots)
            {
                PVPArenaSystem.SendMessage(pm, 1115974); // You currently exceed the maximum number of pet slots for this duel. Please stable your pet(s) with the arena manager before proceeding.
            }

            return Duel.IsParticipant(pm);
        }

        public ArenaGate(Serial serial)
            : base(serial)
        {
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