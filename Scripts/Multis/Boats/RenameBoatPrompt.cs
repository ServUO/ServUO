using System;
using Server.Prompts;

namespace Server.Multis
{
    public class RenameBoatPrompt : Prompt
    {
        private readonly BaseBoat m_Boat;
        public RenameBoatPrompt(BaseBoat boat)
        {
            this.m_Boat = boat;
        }

        public override void OnResponse(Mobile from, string text)
        {
            this.m_Boat.EndRename(from, text);
        }
    }
}