using System;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests.Hag
{
    public class HagApprenticeCorpse : Corpse
    {
        [Constructable]
        public HagApprenticeCorpse()
            : base(GetOwner(), GetEquipment())
        {
            this.Direction = Direction.South;

            foreach (Item item in this.EquipItems)
            {
                this.DropItem(item);
            }
        }

        public HagApprenticeCorpse(Serial serial)
            : base(serial)
        {
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add("a charred corpse");
        }

        public override void OnSingleClick(Mobile from)
        {
            int hue = Notoriety.GetHue(NotorietyHandlers.CorpseNotoriety(from, this));

            from.Send(new AsciiMessage(this.Serial, this.ItemID, MessageType.Label, hue, 3, "", "a charred corpse"));
        }

        public override void Open(Mobile from, bool checkSelfLoot)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
                return;

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                QuestSystem qs = player.Quest;

                if (qs is WitchApprenticeQuest)
                {
                    FindApprenticeObjective obj = qs.FindObjective(typeof(FindApprenticeObjective)) as FindApprenticeObjective;

                    if (obj != null && !obj.Completed)
                    {
                        if (obj.Corpse == this)
                        {
                            obj.Complete();
                            this.Delete();
                        }
                        else
                        {
                            this.SendLocalizedMessageTo(from, 1055047); // You examine the corpse, but it doesn't fit the description of the particular apprentice the Hag tasked you with finding.
                        }

                        return;
                    }
                }
            }

            this.SendLocalizedMessageTo(from, 1055048); // You examine the corpse, but find nothing of interest.
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

        private static Mobile GetOwner()
        {
            Mobile apprentice = new Mobile();

            apprentice.Hue = Utility.RandomSkinHue();
            apprentice.Female = false;
            apprentice.Body = 0x190;

            apprentice.Delete();

            return apprentice;
        }

        private static List<Item> GetEquipment()
        {
            return new List<Item>();
        }
    }
}