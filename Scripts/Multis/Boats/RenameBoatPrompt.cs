using Server.Prompts;

namespace Server.Multis
{
    public class RenameBoatPrompt : Prompt
    {
        public override int MessageCliloc => 502580;
        private readonly BaseBoat m_Boat;

        public RenameBoatPrompt(BaseBoat boat)
        {
            m_Boat = boat;
        }

        public override void OnResponse(Mobile from, string text)
        {
            m_Boat.EndRename(from, text);
        }
    }
}