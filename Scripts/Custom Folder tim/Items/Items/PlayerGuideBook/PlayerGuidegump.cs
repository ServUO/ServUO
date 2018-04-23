//////////////////////////////////
//				                //
//				                //
//				                //
//    Created by Lord Talon	    //
//     www.uohelmsdeep.com	    //
//				                //
//	    Redone by Shade	     	//
//				                //
//////////////////////////////////	

/* DESCRIPTION: A player guide gump that contains all player commands, shardinfo. Aswell as 
 *              some other useful information
 */

using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Gumps
{
    public class PlayerGuidegump : Gump
    {
        public PlayerGuidegump()
            : base(0, 0)
        {

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(1);
            this.AddBackground(23, 50, 450, 166, 9250);

            this.AddLabel(195, 65, 0, @"Player Guide");//......Gump Title
            this.AddLabel(85, 90, 0, @"Commands");//......Button1 Title
            this.AddLabel(85, 120, 0, @"Server Info");//......Button2 Title
            this.AddLabel(85, 150, 0, @"Shard Info");//......Button3 Title
            this.AddLabel(225, 90, 0, @"Getting Started");// button 4
            this.AddLabel(225, 120, 0, @"Skill Information");//button 5

            this.AddButton(40, 90, 4005, 4005, (int)Buttons.Button1, GumpButtonType.Page, 2);//.......Go to page 2
            this.AddButton(40, 120, 4005, 4005, (int)Buttons.Button2, GumpButtonType.Page, 3);//......Go to page 3
            this.AddButton(40, 150, 4005, 4005, (int)Buttons.Button3, GumpButtonType.Page, 4);//......Go to page 4
            this.AddButton(180, 90, 4005, 4005, (int)Buttons.Button4, GumpButtonType.Page, 5);// page 5 - getting started
            this.AddButton(180, 120, 4005, 4005, (int)Buttons.Button5, GumpButtonType.Page, 6);// page 6 - skill info

            this.AddPage(6);
            this.AddBackground(23, 50, 525, 303, 9250);

            this.AddLabel(195, 65, 0, @"Skill Information");//......Gump Title
            this.AddButton(35, 320, 4014, 4014, (int)Buttons.BackButton1, GumpButtonType.Page, 1);//......Back to page 1
            this.AddImage(455, 71, 9000);
            this.AddHtml(42, 85, 407, 224, @"The following is basic information on the skills available

Alchemy: The skill of grinding reagents into a liquid to create potions
Alchemy requires a mortar and pestle and reagents

Blacksmithy: The skill of crafting weapons and armors with ingots and other materials
Blacksmithy requires a smith hammer and ingots as well as a forge and anvil

Bowcraft: The skill of crafting bows and ammunition
Bowcrafting requires fletching tools, wood and various other materials

Carpentry: The skill of crafting furniture and other items
Carpentry requires a tool such as a saw and many logs or other materials

Cooking: The skill of cooking food and other consumable items
Cooking requires various resources, a cooking tool and a heat source

Fishing: The skill of fishing up fish and other items from the sea
Fishing requires a fishing pole and fishing from a boat can yield many things

Healing: The skill of healing or ressurecting a target
Healing requires bandages and receives bonuses from the Anatomy skill

Herding: The skill of convincing animals to go where you direct them
Herding requires a shepherds crook to move an animal

Lockpicking: The skill of picking locks
Lockpicking requires lockpicks which can open treasure chests and other containers

Lumberjacking: The skill of harvesting wood from trees
Lumberjacking requires an axe of any kind and trees with harvestable resources

Magery: The skill of casting spells with reagents
Magery requires a spellbook and reagents or magic scrolls to cast 64 types of spells

Meditation: The skill of focusing your mind to replenish mana
Meditation is a passive and active skill

Mining: The skill to harvest ores and other resources
Mining requires a pickaxe or shovel and only works in mines, swamps and sand

Musicianship: The skill to play various instruments
Musicianship adds bonuses to all music type skills

Removing Traps: The skill of removing a trap from a container
Removing traps can be done by simply selecting the skill

Resisting Spells: The skill of resisting magic cast on you
Resisting spells can lower or negate damage taken from harmful spells

Snooping: The skill of peaking into another players backpack
Snooping allows you to view a players items and works with stealing

Stealing: The skill of taking what isn't yours
Stealing allows you to obtain items from other players illegally

Stealth: The skill of moving unnoticed
Stealth works with hiding to allow you to hide in the shadows

Tailoring: The skill of creating clothing and other items
Tailoring requires a sewing kit and cloth or other resources

Tinkering: The skill of making various items
Tinkering requires tinkering tools and various resources

Veterinary: The ability to heal pets
Veterinary requires bandages and receives a bonus from anatomy

Archery: The skill of fighting with ranged weapons
Fencing: The skill of fighting with piercing weapons
Mace Fighting: The skill of fighting with blunt weapons
Swordsmanship: The skil of fighting with slashing weapons
Tactics: The skill of finding a targets weak spot
Wrestling: The skill of fighting bare handed

Animal Taming: The skill of domesticating animals
Taming allows you to become the owner of a pet

Begging: The skill of making people pity you
Begging requires a poor sap with a fat wallet

Camping: The skill of securing an area
Camping allows you to make a small area safe for a short time

Cartography: The skill of making and reading maps
Cartography requires maps or scrolls and a mapmakers pen

Detect Hidden: The skill of finding those hidden in the shadows
Detect Hidden will reveal sneaky people who are hiding

Discordance: The skill of disoriating a creature
Discordance requires an instrument and musicianship

Hiding: The skill of hiding in the shadows
Hiding works with stealth

Inscription: The skill of crafting spells and magic items
Inscription requires a scribes pen, scrolls and other resources

Peacemaking: The skill of calming monsters
Peacemaking requires an instrument and musicianship

Poisoning: The skill of applying poisons
Poisoning requires poison potions

Provocation: The skill of making monsters aggresive
Provocation requires an instrument and musicianship

Spirit Speak: The skill of talking to the dead
Spirit speak requires some poor victim to be murdered

Tracking: The skill of tracking down creatures
Tracking allows tracking of players and monsters

Anatomy: The skill of anazlyzing a living being
Anatomy works with combat and healing skills

Animal Lore: The skill of animal whispering
Animal lore works with taming and veterinary

Evaluating Intelligence: The skill of judging a creatures IQ
Evaluating Intelligence works with magic spells and skills

Forensic Evaluation: The ability to poke dead things
Forensics allows you to see how something died

Item Identification: The skill to identify items

Taste Identification: The ability to lick things
Taste Identification can detect poison on food", (bool)true, (bool)true);

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            this.AddPage(5);
            this.AddBackground(23, 50, 525, 303, 9250);

            this.AddLabel(195, 65, 0, @"Getting Started");//......Gump Title
            this.AddButton(35, 320, 4014, 4014, (int)Buttons.BackButton1, GumpButtonType.Page, 1);//......Back to page 1
            this.AddImage(455, 71, 9000);
            this.AddHtml(42, 85, 407, 224, @"The following is general information to help new players get started in the world of Imagine Nation

After selecting you starting skills it is advised that you first travel to the Britain Mage Shop. There you will find a short quest to obtain a full spellbook. Simply speak with the npc standing out front, next to a stack of spellbooks, to begin the quest. After accepting you will be directed to head upstairs and participate in some target practice.
When you enter this quest all items you had on you will be instantly put into your bank and upon exiting they can be retrieved from the bank box right next to the quest area.
Once you have obtained your spell book, you may want to train a bit before heading out into the world. 

The best place for a new player to train is at the Mercenary Camp. You can find the Mercenary Camp entrance a bit south west of Britain Bank, which is where you started.
On top of a small stone building is a mage who will open a portal there on request. Simply step through to be sent to the Mercenary Camp.
Once there you will find sparring partners and training dummies in the main camping area. There are many NPCs in this area as well that have various quests to help you get started as well.
If you feel you are ready, you can proceed past the gate to a camp of monsters to begin earning money and finding yourself some items. It is advised to train a bit first though.
Just to the right of where you came in is a small stone building where you can enter a more relaxed starting dungeon. In here you will find a couple very simple quests and some very basic monsters.

While slaying various monsters can be quite a bit of fun with varying challenges, some players prefer to invest their time to become a merchant.
If you would rather craft armor than slay beasts, you may want to travel to the town of Minoc where you can mine for ores to craft with. 
There are various mines throughout the world but the mine right next to Minoc is under the protection of the town guards.

If you much rather become a carpenter or fletcher, Yew is the town for you.
Here you will find endless trees to harvest for logs that you will need to begin crafting. The surrounding forest is mostly under the protection of the town guards.
Straying too far may lead you to the Yew Graveyard or Yew Crypts, which are not places to be traveling as a merchant.

If you wish to be a fisherman, the best town to start out in is Vesper. The entire town is built on the water so there are countless spots to fish. 
Once you start catching fish you can either sell them or cook them. Selling them may be the best bet so you can afford to buy a boat.
Buying a boat allows you to travel the seas in search of treasures beyond fish and boots. Of course you may end up pulling in a monster or two as well.

If at any time you need help or have questions you can page the staff by selecting the Help button on your paperdoll.", (bool)true, (bool)true);//

            /////////////////////////////////////////////////////////////////////////////////////////////////
            this.AddPage(2);
            this.AddBackground(23, 50, 525, 303, 9250);

            this.AddLabel(195, 65, 0, @"Player Commands");//......Gump Title
            this.AddButton(35, 320, 4014, 4014, (int)Buttons.BackButton1, GumpButtonType.Page, 1);//......Back to page 1
            this.AddImage(455, 71, 9000);
            this.AddHtml(42, 85, 407, 224, @"General Commands
.BF - Removes packet sending of objects not in LOS to reduce lag
.DL On/Off - Toggling this on will show your location to others
.G - Sends a message to your guild members
.HelpInfo - Pulls up a gump that displays all commands
.On -  Displays players on that are currently visible
.OpenDoor - Opens the closest door to you if in range
.SP - If DL is enabled you can see the location of players that also have it enabled
.Vote - Opens your browser to vote for the server
.Where - Shows your current world location

Event Commands
.DMScore - Displays the current score of the Death Match event
.LeaveDM - Removes you from the current Death Match
.Qanswer - Used to answer questions during a quiz
.Qhelp - Displays the list of quiz commands and how they work

Questing Commands
.Quest - Displays available options of your current quest
.QuestLog - Displays quest history
.QuestPoints - Displays your current quest points
.QuestRanking - Displays the top players in the quest ranking", (bool)true, (bool)true);//......Add your command list here
            //this.AddButton(215, 320, 4020, 4020, (int)Buttons.WebsiteButton1, GumpButtonType.Reply, 0);//......To command list on website
            //this.AddLabel(250, 320, 0, @"Check website for a more detailed list");//......Website button label


            this.AddPage(3);
            this.AddBackground(23, 50, 450, 228, 9250);

            this.AddLabel(195, 65, 0, @"Shard Information");//......Gump Title
            this.AddLabel(50, 95, 0, @"Recommended Client:");
            this.AddLabel(180, 95, 0, @"6.0.4.0");//
            this.AddLabel(50, 120, 0, @"Skillcap:");
            this.AddLabel(110, 120, 0, @"4800");//
            this.AddLabel(50, 145, 0, @"Statcap:");
            this.AddLabel(110, 145, 0, @"300");//
            this.AddLabel(50, 170, 0, @"Individual Statcap:");
            this.AddLabel(165, 170, 0, @"120");//
            this.AddLabel(50, 195, 0, @"Accounts per IP:");
            this.AddLabel(160, 195, 0, @"1");//
            this.AddLabel(50, 220, 0, @"Characters per Account:");
            this.AddLabel(205, 220, 0, @"5");
            this.AddButton(35, 245, 4014, 4014, (int)Buttons.BackButton3, GumpButtonType.Page, 1);//......Back to page 1
            this.AddImage(395, 200, 5608);

            this.AddPage(4);
            this.AddBackground(23, 50, 450, 228, 9250);

            this.AddLabel(195, 65, 0, @"General Information");//......Gump Title
            this.AddLabel(85, 95, 0, @"Rules");//......Button title 1
            this.AddLabel(85, 120, 0, @"Forums");//......Button title 2
            this.AddLabel(85, 145, 0, @"Donate");//......Button title 3
            this.AddLabel(85, 170, 0, @"Updates");//......Button title 4
            this.AddLabel(85, 195, 0, @"How to Patch");//button 6
            this.AddButton(50, 95, 4020, 4020, (int)Buttons.WebsiteButton2, GumpButtonType.Reply, 0);//......To shard rules on website
            this.AddButton(50, 120, 4020, 4020, (int)Buttons.WebsiteButton3, GumpButtonType.Reply, 0);//......To shard features on website
            this.AddButton(50, 145, 4020, 4020, (int)Buttons.WebsiteButton4, GumpButtonType.Reply, 0);//......To Donation store
            this.AddButton(50, 170, 4020, 4020, (int)Buttons.WebsiteButton5, GumpButtonType.Reply, 0);//......To Shard updates on website
            this.AddButton(50, 195, 4020, 4020, (int)Buttons.WebsiteButton6, GumpButtonType.Reply, 0);// Patching link
            this.AddButton(35, 245, 4014, 4014, (int)Buttons.BackButton4, GumpButtonType.Page, 1);//......Back to page 1

        }
                
        public enum Buttons
        {
            Button1,
            Button2,
            Button3,
            Button4,
            Button5,
            BackButton1,
            BackButton3,
            BackButton4,
            WebsiteButton1,
            WebsiteButton2,
            WebsiteButton3,
            WebsiteButton4,
            WebsiteButton5,
            WebsiteButton6,
        }


        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile m = sender.Mobile;
            {
                switch (info.ButtonID)
                {
                    case (int)Buttons.WebsiteButton1:
                        sender.LaunchBrowser("http://in-uo.net/info/2011-11-06-11-05-03/regular--commands");//......Command List Url
                        break;

                    case (int)Buttons.WebsiteButton2:
                        sender.LaunchBrowser("http://in-uo.net/info/server-rules");//......Shard Rules Url
                        break;

                    case (int)Buttons.WebsiteButton3:
                        sender.LaunchBrowser("http://in-uo.net/forums");//......Forum Url
                        break;

                    case (int)Buttons.WebsiteButton4:
                        sender.LaunchBrowser("http://in-uo.net/donate");//......Shard Donation Store Url
                        break;

                    case (int)Buttons.WebsiteButton5:
                        sender.LaunchBrowser("http://in-uo.net/changelogs/2011-07-28-18-05-48");//......Shard Updates Url
                        break;

                    case (int)Buttons.WebsiteButton6:
                        sender.LaunchBrowser("http://in-uo.net/join");//how to patch
                        break;
                }
            }
        }
    }
}

