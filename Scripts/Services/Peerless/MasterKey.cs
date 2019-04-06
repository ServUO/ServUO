using System;
using System.Linq;
using Server;
using Server.Engines.PartySystem;
using Server.Gumps;

namespace Server.Items
{
    public class MasterKey : PeerlessKey
    {
        public override int LabelNumber { get { return 1074348; } } // master key

        [CommandProperty(AccessLevel.GameMaster)]
        public PeerlessAltar Altar { get; set; }

        [Constructable]
        public MasterKey(int itemID)
            : base(itemID)
        {
            LootType = LootType.Blessed;
        }

        public MasterKey(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                if (!CanOfferConfirmation(from) && Altar == null)
                    return;

                //Altar.OnMasterKeyUse(from);
                if (Altar.Peerless == null)
                {
                    from.CloseGump(typeof(ConfirmPartyGump));
                    from.SendGump(new ConfirmPartyGump(this));
                }
                else
                {
                    var p = Party.Get(from);

                    if (p != null)
                    {
                        foreach (var m in p.Members.Select(x => x.Mobile).Where(m => m.InRange(from.Location, 25)))
                        {
                            m.CloseGump(typeof(ConfirmEntranceGump));
                            m.SendGump(new ConfirmEntranceGump(Altar, m));
                        }
                    }
                    else
                    {
                        from.CloseGump(typeof(ConfirmEntranceGump));
                        from.SendGump(new ConfirmEntranceGump(Altar, from));
                    }
                }
            }
        }

        public override void OnAfterDelete()
        {
            if (Altar == null)
                return;

            Altar.MasterKeys.Remove(this);

            if (Altar.MasterKeys.Count == 0 && Altar.Fighters.Count == 0)
                Altar.FinishSequence();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((Item)Altar);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Altar = reader.ReadItem() as PeerlessAltar;
        }

        public virtual bool CanOfferConfirmation(Mobile from)
        {
            if (Altar != null && Altar.Fighters.Contains(from))
            {
                from.SendLocalizedMessage(1063296); // You may not use that teleporter at this time.
                return false;
            }

            return true;
        }
    }
}
