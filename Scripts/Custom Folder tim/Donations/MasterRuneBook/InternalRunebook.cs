using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Multis;
using Server.Engines.Craft;
using Server.ContextMenus;

namespace Server.Items
{
    public class InternalRunebook : Runebook
    {
        public InternalRunebook()
            : this(0)
        {
        }

        public InternalRunebook(int maxCharges)
            : base(maxCharges)
        {
        }

        public InternalRunebook(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }

        public void AddRune(Mobile from, RecallRune dropped)
        {
            if (Entries.Count < 16)
            {
                RecallRune rune = dropped;

                if (rune.Marked && rune.TargetMap != null)
                {
                    Entries.Add(new RunebookEntry(rune.Target, rune.TargetMap, rune.Description, rune.House));

                    dropped.Delete();

                    from.Send(new PlaySound(0x42, GetWorldLocation()));

                    string desc = rune.Description;

                    if (desc == null || (desc = desc.Trim()).Length == 0)
                        desc = "(nondescript)";

                    from.SendMessage(desc);
                }
                else
                {
                    from.SendLocalizedMessage(502409); // This rune does not have a marked location.
                }
            }
            else
            {
                from.SendLocalizedMessage(502401); // This runebook is full.
            }
        }

        public override void OnTravel()
        {
            NextUse = DateTime.Now;
        }
    }
}