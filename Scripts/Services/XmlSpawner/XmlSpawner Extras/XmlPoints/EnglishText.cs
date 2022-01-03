using System;
using Server;
using System.Text;

namespace Server.Engines.XmlSpawner2
{

    public class EnglishPointsText
    {

        private static void Add(int index, string text)
        {
            XmlPoints.AddText(XmlPoints.LanguageType.ENGLISH, index, text);
        }

    	public static void Initialize()
    	{
            // add in all of the text phrases that will be used
            // indices that begin with 100 refer to messages displayed in the game window, so no real length limit
            // indices that begin with 200 refer to gump labels and so should be translated to text of similar length for proper appearance
            
            // basechallengegame.cs
            Add(100000, "Entry fee of {0} gold has been returned to you.");
            Add(100001, "Entry fee of {0} gold has been returned to your bank account.");
            Add(100002, "Let the games begin!");
            Add(100003, "{0} cancelled");

            // challengeregion.cs
            Add(100104, "Harmful acts are not allowed before or after a Challenge Game.");
            Add(100105, "You are not allowed to open that here.");
            Add(100106, "You have entered the Challenge Game region '{0}'");
            Add(100107, "You have left the Challenge Game region '{0}'");
            
            // xmlpoints.cs
            Add(100207, "They are too inexperienced to be challenged");
            Add(100208, "{0} mins remaining until current challenge is cancelled.");
            Add(100209, "Canceling current challenge.  Please wait {0} minutes");
            Add(100210, "{0} is canceling the current challenge. {1} minutes remain");
            Add(100211, "{0} is already being challenged.");
            Add(100212, "You cannot challenge yourself.");
            Add(100213, "No XmlPoints support.");
            Add(100214, "Challenge with {0} has been cancelled");
            Add(100215, "You receive {0} points for killing {1}");
            Add(100216, "{0} has defeated {1} in combat.");
            Add(100217, "You lost {0} point(s) for being killed by {1}");
            // xmlpoints identify
            Add(100218, "Current Points = {0}");
            Add(100219, "Rank = {0}");
            Add(100220, "No ranking");
            Add(100221, "Available Credits = {0}");
            Add(100222, "Total Kills = {0}\nTotal Deaths = {1}\nRecent Kill List");
            Add(100223, "{0} killed at {1}");
            // xmlpoints gump
            Add(200224, "Points Standing for {0}");
            Add(200225, "Currently challenging {0}");
            Add(200226, "See kills" );
            Add(200227, "Broadcast kills");
            Add(200228, "Top players");
            Add(200229, "Challenge");
            Add(200230, "LMS");
            Add(200231, "Deathmatch");
            Add(200232, "KotH");
            Add(200233, "DeathBall");
            Add(200234, "Team LMS");
            Add(200235, "Team DMatch");
            Add(200236, "Team DBall");
            Add(200237, "Team KotH");
            Add(200238, "CTF");
            // topplayers gump
            Add(200239, "Top Player Rankings");
            Add(200240, "Filter by Guild");
            Add(200241, "Filter by Name");
            Add(200242, "Name");
            Add(200243, "Guild");
            Add(200244, "Points");
            Add(200245, "Kills");
            Add(200246, "Deaths");
            Add(200247, "Rank");
            Add(200248, "Change");
            Add(200249, "Time at Rank" );
            Add(200250, "{0} days " );
            Add(200251, "{0} hours " );
            Add(200252, "{0} mins" );
            Add(200253, "just changed" );
            // challenge gump
            Add(200254, "You are challenging" );
            Add(200255, "{0}. Continue?" );
            Add(200256, "You will NOT gain points!" );
            Add(100257, "You have issued a challenge to {0}." );
            Add(100258, "You decided against challenging {0}." );
            Add(200259, "You have been challenged by" );
            Add(200260, "{0}. Accept?" );
            Add(100261, "{0} has already been challenged." );
            Add(100262, "You are already being challenged." );
            Add(100263, "{0} accepted your challenge!" );
            Add(100264, "You have accepted the challenge from {0}!" );
            Add(100265, "Your challenge to {0} was declined." );
            Add(100266, "You declined the challenge by {0}." );
			Add(100267, "You cannot issue a challenge here.");

            // gauntlet scripts
            Add(100300, "Prize from {0}" );
            Add(100301, "You have received a bank check for {0}" );
            Add(100302, "Last Man Standing" );
            Add(100303, "Unable to set up a {0} Challenge: Another Challenge Game is already in progress in this Challenge Game region.");
            Add(100304, "Unable to set up a {0} Challenge: Must be in a Challenge Game region.");
            Add(100305, "Unable to set up a {0} Challenge.");
            Add(100306, "Setting up a {0} Challenge.");
            Add(100307, "{0} Challenge being prepared in '{1}' by {2}");
            Add(100308, "{0} has been disqualified");
            Add(100309, "You are out of bounds!  You have {0} seconds to return");
            Add(100310, "You have {0} seconds become unhidden");
            Add(100311, "You have won {0}");
            Add(100312, "The winner is {0}");
            Add(100313, "The match is a draw");
            Add(100314, "{0} has been killed");
            Add(100315, "Challenge by {0}");
            Add(100400, "Deathmatch");
            Add(100401, "{0} was penalized.");
            Add(100410, "King of the Hill");
            Add(100411, "Deathball");
            Add(100412, "{0} has dropped the ball!");
            Add(100413, "Team LMS");
            Add(100414, "Team {0} is the winner!");
            Add(100415, "Team Deathmatch");
            Add(100416, "Team Deathball");
            Add(100417, "Team KotH");
            Add(100418, "CTF");
            Add(100419, "Team {0} flag has been returned to base");
            Add(100420, "Team {0} has the Team {1} flag");
            Add(100421, "Team {0} has scored");
            
            // game gump scripts
            Add(200500, "Last Man Standing Challenge");
            Add(200501, "Organized by: {0}");
            Add(200502, "Entry Fee: {0}");
            Add(200503, "Arena Size: {0}");
            Add(200504, "Total Purse: {0}");
            Add(200505, "Loc: {0} {1}");
            Add(200506, "Players: {0}");
            Add(200507, "Active: {0}");
            Add(200508, "Page: {0}/{1}");
            Add(200509, "Waiting");
            Add(200510, "Accepted");
            Add(200511, "Insufficient funds");
            Add(200520, "Forfeit");
            Add(200521, "Hidden");
            Add(200522, "Out of Bounds");
            Add(200523, "Offline");
            Add(200524, "Winner");
            Add(200525, "Active");
            Add(200526, "Dead");
            Add(200527, "Disqualified");
            Add(200528, "Add");
            Add(200529, "Start");
            Add(200530, "Set Entry Fee: ");
            Add(200531, "Set Arena Size: ");
            Add(200532, "Refresh");
            Add(200533, "Game is in progress!");
            Add(200534, "{0} is the winner!");
            Add(100535, "Challenge is full!");
            Add(100536, "{0} does not qualify. No points support.");
            Add(100537, "{0} is already in a Challenge.");
            Add(100538, "{0} cannot afford the Entry fee.");
            Add(100539, "{0} has not accepted yet.");
            Add(100540, "Insufficient number of players.");
            Add(100541, "Could not withdraw the Entry fee of {0} gold from your bank.");
            Add(100542, "The Entry fee of {0} gold has been withdrawn from your bank.");
            Add(100543, "You dropped out of {0}");
            Add(100544, "{0} has dropped out.");
            Add(100545, "You have been kicked from {0}");
            Add(100546, "{0} does not qualify. No points support.");
            Add(100547, "{0} is already in a Challenge.");
            Add(100548, "{0} has already been added to the game.");
            Add(100549, "You have added {0} to the challenge.");
            Add(100550, "You have been invited to participate in {0}.");
            Add(200560, "Deathmatch Challenge");
            Add(200561, "Target Score: {0}");
            Add(200562, "Target Score: None");
            Add(200563, "Match Length: {0}");
            Add(200564, "Match Length: Unlimited");
            Add(200565, "Time left {0}");
            Add(200566, "Score: ");
            Add(200567, "Length mins: ");
            Add(100568, "No valid end condition for match.");
            Add(200570, "Deathball Challenge");
            Add(200572, "Entry Fee: ");
            Add(200573, "Arena Size: ");
            Add(200580, "King of the Hill Challenge");
            Add(200590, "Team Deathmatch Challenge");
            Add(200591, "Team");
            Add(200592, "Set Teams");
            Add(200593, "Team {0} is the winner!");
            Add(100594, "{0} has not been assigned a team.");
            Add(200595, "Deathmatch Team Status");
            Add(200596, "Members");
            Add(200597, "Active");
            Add(200598, "Score");
            Add(200600, "Team KotH Challenge");
            Add(200601, "KotH Team Status");
            Add(200610, "Team LMS Challenge");
            Add(200611, "LMS Team Status");
            Add(200620, "Capture the Flag Challenge");
            Add(100621, "Team {0} base not defined.");
            Add(200622, "Base");
            Add(200623, "CTF Team Status");
            Add(200630, "Team Deathball Challenge");
            Add(200631, "Deathball Team Status");
            
            Add(200640, "Faction");
            Add(200649, "Location: {0}");
            Add(200650, "{0} is occupied");

            Add(200660, "Cancel");
            Add(200661, "Duel here");
            Add(100670, "{0} is in combat.");

    	}
    }
}
