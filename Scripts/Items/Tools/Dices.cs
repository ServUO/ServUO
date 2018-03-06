using System;
using Server.Network;
using Server.Gumps;
using Server.Multis;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
    public class Dices : Item, ITelekinesisable, ISecurable
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public Dices()
            : base(0xFA7)
        {
            Weight = 1.0;

            Level = SecureLevel.Friends;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public Dices(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
                return;

            Roll(from);
        }

        public void OnTelekinesis(Mobile from)
        {
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
            Effects.PlaySound(Location, Map, 0x1F5);

            Roll(from);
        }

        public void Roll(Mobile from)
        {
            int one = Utility.Random(1, 6);
            int two = Utility.Random(1, 6);

            this.SendLocalizedMessage(MessageType.Emote, 1042713, AffixType.Prepend, from.Name + " ", ""); // The first die rolls to a stop and shows:
            this.SendLocalizedMessage(MessageType.Regular, 1042714, AffixType.Append, " " + one.ToString(), ""); // The first die rolls to a stop and shows:
            this.SendLocalizedMessage(MessageType.Regular, 1042715, AffixType.Append, " " + two.ToString(), ""); // The second die stops and shows:
            this.SendLocalizedMessage(MessageType.Regular, 1042716, AffixType.Append, " " + (one + two).ToString(), ""); // Total for this roll:
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write((int)Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version > 0)
            {
                Level = (SecureLevel)reader.ReadInt();
            }
            else
            {
                Level = SecureLevel.Friends;
            }
        }
    }
}