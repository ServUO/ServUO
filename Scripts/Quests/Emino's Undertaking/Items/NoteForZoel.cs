using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
    public class NoteForZoel : QuestItem
    {
        [Constructable]
        public NoteForZoel()
            : base(0x14EF)
        {
            this.Weight = 1.0;
            this.Hue = 0x6B9;
        }

        public NoteForZoel(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063186;
            }
        }// A Note for Zoel
        public override bool CanDrop(PlayerMobile player)
        {
            EminosUndertakingQuest qs = player.Quest as EminosUndertakingQuest;

            if (qs == null)
                return true;

            //return !qs.IsObjectiveInProgress( typeof( GiveZoelNoteObjective ) );
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