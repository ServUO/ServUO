using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.Quests.Hag
{
    public class HagApprenticeCorpse : Corpse
    {
        [Constructable]
        public HagApprenticeCorpse()
            : base(GetOwner(), GetEquipment())
        {
            Direction = Direction.South;

            foreach (Item item in EquipItems)
            {
                DropItem(item);
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

        public override void Open(Mobile from, bool checkSelfLoot)
        {
            if (!from.InRange(GetWorldLocation(), 2))
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
                            Delete();
                        }
                        else
                        {
                            SendLocalizedMessageTo(from, 1055047); // You examine the corpse, but it doesn't fit the description of the particular apprentice the Hag tasked you with finding.
                        }

                        return;
                    }
                }
            }

            SendLocalizedMessageTo(from, 1055048); // You examine the corpse, but find nothing of interest.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private static Mobile GetOwner()
        {
            Mobile apprentice = new Mobile
            {
                Hue = Utility.RandomSkinHue(),
                Female = false,
                Body = 0x190
            };

            apprentice.Delete();

            return apprentice;
        }

        private static List<Item> GetEquipment()
        {
            return new List<Item>();
        }
    }
}
