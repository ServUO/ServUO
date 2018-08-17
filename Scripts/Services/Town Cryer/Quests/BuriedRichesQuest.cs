using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Services.TownCryer;

namespace Server.Engines.Quests
{
    public class BuriedRichesQuest : BaseQuest
    {
        /* Buried Riches */
        public override object Title { get { return 1158230; } }

        /*Treasure Hunting sure does sound like an interesting profession! Think of the riches to be found! You'd have everything 
         * you've ever dreamed of! The Town Cryer article is fairly vague, however you have heard whispers of a mapmaker in Vesper
         * at the Majestic Boat who may know a thing or two about decoding treasure maps.*/
        public override object Description { get { return 1158223; } }

        /* You decide against accepting the quest. */
        public override object Refuse { get { return 1158130; } }

        /* Visit the Skara Brae Ranger's Guild and participate in the Huntmaster's Challenge */
        public override object Uncomplete { get { return 1158231; } }

        /*The Cartographer seems busy at her desk pouring over stacks of rolled parchment. You decide to break the 
         * silence with courteous *Ahem**/
        public override object Complete { get { return 1158226; } }

        public override int CompleteMessage { get { return 1156585; } } // You've completed a quest!

        public override int AcceptSound { get { return 0x2E8; } }
        public override bool DoneOnce { get { return true; } }

        public BuriedRichesQuest()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1158224)); // A step closer to becoming a treasure hunter.
        }

        public void CompleteQuest()
        {
            OnCompleted();
            Objectives[0].CurProgress++;
            TownCryerSystem.CompleteQuest(Owner, 1158225, Complete, 0x614);
            GiveRewards();
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription { get { return 1158231; } } // Visit the Legendary Cartographer at the The Majestic Boat in Vesper.

            public InternalObjective()
                : base(1)
            {
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

    public class ToolsOfTheTradeQuest : BaseQuest
    {
        /* Tools of the Trade */
        public override object Title { get { return 1158232; } }

        /*A treasure seeker then? There is no telling the lengths someone will go to protect their most prized possessions. Over 
         * time maps to certain treasure troves have been found and their bounties recovered. The bounty within a treasure chest 
         * is directly related to the difficulty of deciphering the map and overcoming the protections of the chest itself. 
         * Cartographers use a variety of terms to describe the difficulty of a map that is drawn. This includes maps that are 
         * plainly, expertly, adeptly, cleverly, deviously, ingeniously, and diabolically drawn. Even the most basically trained 
         * cartographer can decode a plainly drawn map.  Beyond that, however, some training in cartography is required. Once
         * deciphered, the cartographer must find the location within the world and use a digging tool, such as a pickaxe or shovel 
         * to dig up the chest. Those skilled at mining will have a much easier time finding the chest, but it is not a requirement. 
         * The chest will be no doubt locked and trapped, so some skill with lockpicking and trap removal is suggested, although 
         * mages skilled enough may use magical means to unlock and untrap lower end treasure chests. Finally, the chest is likely 
         * to be guarded by a variety of creatures that will attempt to defend the treasure at all costs. Combat skills are imperative
         * to dispatch those creatures safely! Sounds like a challenge? Well it can be, but alas it is also incredibly rewarding and 
         * you will have a much easier time of it if you recruit other adventures into your budding treasure seeking business. Alas, 
         * here's a map I had lying around. You can visit the Adventurer's Supplies just on the mainland of Vesper to get some basic 
         * supplies.*/
        public override object Description { get { return 1158227; } }

        /* You decide against accepting the quest. */
        public override object Refuse { get { return 1158130; } }

        /* Visit the Adventurer's Supplies in Northern Vesper, just across the bridge, and speak to the Master Provisioner 
         * to get some basic treasure hunting equipment. */
        public override object Uncomplete { get { return 1158228; } }

        /*The Adventurer's Supplies is a large provisioner with many different types of adventuring equipment available for purchase.
         * You spot the shopkeeper the Cartographer described and approach with a friendly greeting!*/
        public override object Complete { get { return 1158233; } }

        public override int CompleteMessage { get { return 1156585; } } // You've completed a quest!

        public override int AcceptSound { get { return 0x2E8; } }
        public override bool DoneOnce { get { return true; } }

        public ToolsOfTheTradeQuest()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1158224)); // A step closer to becoming a treasure hunter.
        }

        public override void OnAccept()
        {
            base.OnAccept();

            if (QuestHelper.TryReceiveQuestItem(Owner, typeof(BuriedRichesTreasureMap), TimeSpan.FromDays(7)))
            {
                Owner.AddToBackpack(new BuriedRichesTreasureMap(1));
            }
        }

        public void CompleteQuest()
        {
            OnCompleted();
            Objectives[0].CurProgress++;
            TownCryerSystem.CompleteQuest(Owner, 1016275, Complete, 0x619);
            GiveRewards();
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription { get { return 1158228; } } // Visit the Adventurer's Supplies in Northern Vesper, just across the bridge, and speak to the Master Provisioner to get some basic treasure hunting equipment.

            public InternalObjective()
                : base(1)
            {
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

    public class TheTreasureChaseQuest : BaseQuest
    {
        /* The Treasure Chase */
        public override object Title { get { return 1158239; } }

        /*Finest provisions in all of Britannia! Right here! *The Provisioner looks you up and down* I've seen your kind before - 
         * I know that look! You're a treasure seeker! I take it you spoke to the Cartographer then? Of course you have, why else
         * would you be visiting Britannia's premiere outfitter of Treasure Hunting supplies! I trust you will be most successful
         * with treasure hunting so I'll kit you out with basic supplies free of charge - just remember me when you've become a 
         * famous treasure hunter!*/
        public override object Description { get { return 1158365; } }

        /* You decide against accepting the quest. */
        public override object Refuse { get { return 1158130; } }

        /* Use the book "Treasure Hunting: A Practical Approach" to help you decode the treasure maps you have been given and those 
         * that you find during your adventure and use the information contained within the text to aid you in your quest.*/
        public override object Uncomplete { get { return 1158238; } }

        /*The Adventurer's Supplies is a large provisioner with many different types of adventuring equipment available for purchase.
         * You spot the shopkeeper the Cartographer described and approach with a friendly greeting!*/
        public override object Complete { get { return 1158233; } }

        public override int CompleteMessage { get { return 1158247; } } //You have found the final zealot treasure! There are no doubt riches to be had within! Your experience has earned you a 
                                                                        // reward title that has been placed in your backpack.

        public override int AcceptSound { get { return 0x2E8; } }
        public override bool DoneOnce { get { return true; } }

        public TheTreasureChaseQuest()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(typeof(TreasureHunterRewardTitleDeed), 1158237)); // Treasure!
        }

        public override void OnAccept()
        {
            base.OnAccept();

            if (QuestHelper.TryReceiveQuestItem(Owner, typeof(TreasureHuntingBook), TimeSpan.FromDays(7)))
            {
                var chest = new WoodenChest();
                chest.DropItem(new TreasureHuntingBook());

                var heals = new GreaterHealPotion();
                heals.Amount = 10;
                chest.DropItem(heals);

                var scrolls = new TelekinisisScroll();
                scrolls.Amount = 20;
                chest.DropItem(scrolls);

                chest.DropItem(new Pickaxe());
                chest.DropItem(new TreasureSeekersLockpick());

                Owner.Backpack.DropItem(chest);
            }
        }

        public override bool RenderObjective(MondainQuestGump g, bool offer)
        {
            if (offer)
                g.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                g.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

            g.AddHtmlObject(160, 70, 200, 40, Title, BaseQuestGump.DarkGreen, false, false);
            g.AddHtmlLocalized(98, 140, 312, 16, 1049073, 0x2710, false, false); // Objective:

            g.AddHtmlLocalized(98, 156, 312, 16, 1072208, 0x2710, false, false); // All of the following	

            g.AddHtmlLocalized(98, 172, 312, 83, 1158234, BaseQuestGump.LightGreen, false, false);
            /* Find the location marked on the Treasure Map given to you by the Cartographer and use the supplies the Provisioner 
            * gave you to recover the treasure.*/

            g.AddHtmlLocalized(98, 255, 312, 40, 1158235, BaseQuestGump.LightGreen, false, false); 
            //Expand your experience as a Treasure Hunter to an Expertly Drawn Map.

            g.AddHtmlLocalized(98, 335, 312, 40, 1158236, BaseQuestGump.LightGreen, false, false); 
            // Complete your experience as a Treasure Hunter by discovering the final treasure hoard.

            return true;
        }

        public void CompleteQuest()
        {
            OnCompleted();
            Objectives[0].CurProgress++;
            TownCryerSystem.CompleteQuest(Owner, 1158239, 1158249, 0x655);
            /*Another rusted chest emerges from the broken ground at your feet! As you pry it open the brilliant 
             * shimmer of gold and jewels catches your eye. This map too is highly decorated with ancient runic 
             * text and marks another location for the hoard. You notice the magical creatures guarding the previous 
             * hoard were more challenging than the first, and you expect that trend to continue. With greater
             * difficulty comes greater reward! On the reverse of the map is a short hand-written note,<br><br><i>For
             * those who will come long after and discover this treasure, know you will never truly discover the 
             * extent of our wealth. If you posses this map you no doubt have some connection to our society, which
             * has survived generation after generation. Use this wealth for what we have used it for, to be virtuous
             * and good throughout Sosaria...</i><br><br>The note is very cryptic about the origins of these zealots 
             * and their beliefs, but from what you can gleam they are a long gone organization who's values were 
             * that of virtue and good. You are warmed by this altruistic purpose and decide to use your wealth to
             * promote their cause throughout the realm as you search for other treasures.*/
            GiveRewards();
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription { get { return 1158231; } } // Visit the Legendary Cartographer at the The Majestic Boat in Vesper.

            public InternalObjective()
                : base(1)
            {
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

    public class LegendaryCartographer : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { typeof(ToolsOfTheTradeQuest) }; } }

        public static LegendaryCartographer TramInstance { get; set; }
        public static LegendaryCartographer FelInstance { get; set; }

        public static void Initialize()
        {
            if (Core.TOL)
            {
                if (TramInstance == null)
                {
                    TramInstance = new LegendaryCartographer();
                    TramInstance.MoveToWorld(new Point3D(3005, 811, 0), Map.Trammel);
                    TramInstance.Direction = Direction.West;
                }

                if (FelInstance == null)
                {
                    FelInstance = new LegendaryCartographer();
                    FelInstance.MoveToWorld(new Point3D(3005, 811, 0), Map.Felucca);
                    FelInstance.Direction = Direction.West;
                }
            }
        }

        public LegendaryCartographer()
            : base(NameList.RandomName("female"), "the Legendary Cartographer")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            CantWalk = true;

            Body = 0x191;
            Hue = Race.RandomSkinHue();
            HairItemID = 0x2045;

            HairHue = 0x8A8;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            SetWearable(new Doublet());
            SetWearable(new Kilt(), 443);
            SetWearable(new ThighBoots(), 1837);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && !QuestHelper.CheckDoneOnce((PlayerMobile)m, typeof(BuriedRichesQuest), this, false))
            {
                m.SendLocalizedMessage(1080107); // I'm sorry, I have nothing for you at this time.
            }
            else
            {
                base.OnDoubleClick(m);
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is PlayerMobile && InRange(m.Location, 5) && !InRange(oldLocation, 5))
            {
                BuriedRichesQuest quest = QuestHelper.GetQuest<BuriedRichesQuest>((PlayerMobile)m);

                if (quest != null)
                {
                    quest.CompleteQuest();
                }
            }
        }

        public LegendaryCartographer(Serial serial)
            : base(serial)
        {
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

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }

            if (Map == Map.Felucca)
            {
                FelInstance = this;
            }

            if(!Core.TOL)
            {
                Delete();
            }
        }
    }

    public class MasterProvisioner : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { typeof(TheTreasureChaseQuest) }; } }

        public static MasterProvisioner TramInstance { get; set; }
        public static MasterProvisioner FelInstance { get; set; }

        public static void Initialize()
        {
            if (Core.TOL)
            {
                if (TramInstance == null)
                {
                    TramInstance = new MasterProvisioner();
                    TramInstance.MoveToWorld(new Point3D(2989, 636, 0), Map.Trammel);
                    TramInstance.Direction = Direction.West;
                }

                if (FelInstance == null)
                {
                    FelInstance = new MasterProvisioner();
                    FelInstance.MoveToWorld(new Point3D(2989, 636, 0), Map.Felucca);
                    FelInstance.Direction = Direction.West;
                }
            }
        }

        public MasterProvisioner()
            : base(NameList.RandomName("male"), "the Master Provisioner")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;

            Body = 0x190;
            Hue = Race.RandomSkinHue();
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            SetWearable(new FancyShirt());
            SetWearable(new JinBaori());
            SetWearable(new Kilt());
            SetWearable(new ThighBoots(), 1908);
            SetWearable(new GoldNecklace());
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && !QuestHelper.CheckDoneOnce((PlayerMobile)m, typeof(BuriedRichesQuest), this, false))
            {
                m.SendLocalizedMessage(1080107); // I'm sorry, I have nothing for you at this time.
            }
            else
            {
                base.OnDoubleClick(m);
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is PlayerMobile && InRange(m.Location, 5) && !InRange(oldLocation, 5))
            {
                ToolsOfTheTradeQuest quest = QuestHelper.GetQuest<ToolsOfTheTradeQuest>((PlayerMobile)m);

                if (quest != null)
                {
                    quest.CompleteQuest();
                }
            }
        }

        public MasterProvisioner(Serial serial)
            : base(serial)
        {
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

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }

            if (Map == Map.Felucca)
            {
                FelInstance = this;
            }

            if (!Core.TOL)
            {
                Delete();
            }
        }
    }
}