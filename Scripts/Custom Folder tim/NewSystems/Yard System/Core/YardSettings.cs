using System;
using System.Collections.Generic;
using System.Text;

namespace Server.ACC.YS
{
    public class YardSettings
    {
        //Demensions of the yard can be set here.
        //Set all to 0 if you only want the player to place inside their house.

        // Spaces to left of house allowed for placement.
        public const int Left = 10;
        
        // Spaces to right of house allowed for placement.
        public const int Right = 10;

        // Spaces to front of house allowed for placement.
        public const int Front = 10;

        // Spaces to back of house allowed for placement.
        public const int Back = 10;

        //This variable is used to tell the system how many seconds after 
        //the World.Save the cleanup of any orphaned YardItems happens.
        //Set it so it runs after the save is complete, so if your saves 
        //take 10 seconds, set it to 15.
        public const int SecondsToCleanup = 30;

        //This variable is used to tell the system if it allows placement 
        //in other player's house.
        public const bool AllowOtherHouses = false;
    }
}
