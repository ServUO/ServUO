using Server.Items;
using Server.Mobiles;
using Server.Services.TownCryer;
using System;

namespace Server.Engines.Quests
{
    public class PaladinsOfTrinsic : BaseQuest
    {
        public override QuestChain ChainID => QuestChain.PaladinsOfTrinsic;
        public override Type NextQuest => typeof(PaladinsOfTrinsic2);

        /* The Paladins of Trinsic */
        public override object Title => 1158093;

        /*It seems the Paladins of Trinsic are working hard to see the threats of Shame are kept inside Shame, 
         * perhaps it would be a good idea to visit their headquarters in Northeast Trinsic.*/
        public override object Description => 1158114;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Find the Lord Commander of the Paladins of Trinsic. */
        public override object Uncomplete => 1158117;

        /*You have proven yourself honorable and the Lord Commander has invited you to join the elite order of the Paladin of Trinsic!
         * Congratulations, Paladin!*/
        public override object Complete => 1158317;

        public override int CompleteMessage => 1156585;  // You've completed a quest!

        public override int AcceptSound => 0x2E8;
        public override bool DoneOnce => true;

        public PaladinsOfTrinsic()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1158120)); // A unique opportunity to join the Paladins of Trinsic.
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription => Quest.Uncomplete;

            public InternalObjective()
                : base(1)
            {
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
        }
    }

    public class PaladinsOfTrinsic2 : BaseQuest
    {
        public override QuestChain ChainID => QuestChain.PaladinsOfTrinsic;

        /* The Paladins of Trinsic */
        public override object Title => 1158093;

        /*Another who wishes to walk the path of the Paladins of Trinsic? Well you should know the path is not an easy one to walk,
         * and only those with the courage to pursue truth are admitted to the order. If you prove you are honorable, then you shall
         * join our ranks and gain the prestigious title of Paladin of Trinsic. We are bound by Honor, and thus we stand against 
         * Shame! To prove yourself you must venture deep within the dungeon Shame and slay the vile within. Succeed in this task 
         * and you will prove your worth, fail and you will bring to your name what you hope to defeat - Shame.*/
        public override object Description => 1158096;

        /* The way of the Paladin is not for everyone, I understand your decision but hope you reconsider... */
        public override object Refuse => 1158102;

        /* Go to the dungeon Shame and slay the creatures within, only then will you have the Honor of calling thyself a Paladin of 
         * Trinsic. */
        public override object Uncomplete => 1158105;

        /*You have proven yourself honorable and the Lord Commander has invited you to join the elite order of the Paladin of Trinsic!
         * Congratulations, Paladin!*/
        public override object Complete => 1158317;

        public override int AcceptSound => 0x2E8;
        public override bool DoneOnce => true;
        public override int CompleteMessage => 1158108;

        public bool SentMessage { get; set; }

        public PaladinsOfTrinsic2()
        {
            AddObjective(new SlayObjective(typeof(QuartzElemental), "quartz elemental", 1, "Shame"));
            AddObjective(new SlayObjective(typeof(FlameElemental), "flame elemental", 1, "Shame"));
            AddObjective(new SlayObjective(typeof(WindElemental), "wind elemental", 1, "Shame"));
            AddObjective(new SlayObjective(typeof(UnboundEnergyVortex), "unbound energy vortex", 1, "Shame"));

            AddReward(new BaseReward(typeof(PaladinOfTrinsicRewardTitleDeed), 1158099)); // Becoming a Paladin of Trinsic
        }

        public void CompleteQuest()
        {
            TownCryerSystem.CompleteQuest(Owner, new PaladinsOfTrinsic());

            OnCompleted();
            GiveRewards();

            QuestHelper.Delay(Owner, typeof(PaladinsOfTrinsic), RestartDelay);
        }
    }

    public class Morz : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(PaladinsOfTrinsic) };

        public static Morz TramInstance { get; set; }
        public static Morz FelInstance { get; set; }

        public static void Initialize()
        {
            if (TramInstance == null)
            {
                TramInstance = new Morz();
                TramInstance.MoveToWorld(new Point3D(2018, 2745, 30), Map.Trammel);
                TramInstance.Direction = Direction.South;
            }

            if (FelInstance == null)
            {
                FelInstance = new Morz();
                FelInstance.MoveToWorld(new Point3D(2018, 2745, 30), Map.Felucca);
                FelInstance.Direction = Direction.South;
            }
        }

        public Morz()
            : base("Morz", "the Lord Commander")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;

            Body = 0x190;
            Hue = Race.RandomSkinHue();
            HairItemID = 0;
            FacialHairItemID = 0x204D;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            SetWearable(new PlateChest(), 0x8A5);
            SetWearable(new PlateLegs(), 0x8A5);
            SetWearable(new PlateArms(), 0x8A5);
            SetWearable(new PlateGloves(), 0x8A5);
            SetWearable(new BodySash(), 1158);
            SetWearable(new Cloak(), 1158);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && m.InRange(Location, 5))
            {
                PaladinsOfTrinsic quest = QuestHelper.GetQuest((PlayerMobile)m, typeof(PaladinsOfTrinsic)) as PaladinsOfTrinsic;

                if (quest != null)
                {
                    quest.GiveRewards();
                }
                else
                {
                    PaladinsOfTrinsic2 quest2 = QuestHelper.GetQuest((PlayerMobile)m, typeof(PaladinsOfTrinsic2)) as PaladinsOfTrinsic2;

                    if (quest2 != null)
                    {
                        if (quest2.Completed)
                        {
                            quest2.CompleteQuest();
                        }
                        else
                        {
                            m.SendGump(new MondainQuestGump(quest2, MondainQuestGump.Section.InProgress, false));
                            quest2.InProgress();
                        }
                    }
                }
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is PlayerMobile && InRange(m.Location, 5) && !InRange(oldLocation, 5))
            {
                PaladinsOfTrinsic2 quest = QuestHelper.GetQuest<PaladinsOfTrinsic2>((PlayerMobile)m);

                if (quest != null && !quest.SentMessage && quest.Completed)
                {
                    m.SendLocalizedMessage(1158111); // You have proven yourself Honorable, the Lord Commander looks overjoyed as you approach him triumphantly! Speak to him to claim your reward!
                    quest.SentMessage = true;
                }
            }
        }

        public Morz(Serial serial)
            : base(serial)
        {
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

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }
            else if (Map == Map.Felucca)
            {
                FelInstance = this;
            }
        }
    }
}
