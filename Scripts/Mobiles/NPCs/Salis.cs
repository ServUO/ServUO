using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class DishBestServedColdQuest : BaseQuest
    {
        public DishBestServedColdQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(TerathanAvengerArms), "Terathan Avenger Arms", 1));
            AddObjective(new ObtainObjective(typeof(BowlOfRotwormStew), "Bowl of Rotworm Stew", 1));
            AddObjective(new ObtainObjective(typeof(SmallPieceofBlackrock), "Small Pieces of Blackrock", 5));

            AddReward(new BaseReward(1150043));
        }

        public override bool DoneOnce => true;

        /* A Dish Best Served Cold */
        public override object Title => 1072372;

        /* Obtain a bowl of rotworm stew, five small pieces of blackrock, and the arms of a Terathan Avenger and return to Salis for your reward.<br><br>
         * <center>-----</center><br><br>Greetings ally of the Ophidians-s-s.  I am working on a secret project to undermine the power of our hated enemies
         * the Bane Chos-s-sen.<br><br>We know that the Bane Chos-s-sen use their knowledge of the Bane Dragon food to manipulate those hungry for power over
         * Bane Dragons-s-s.  If I can discover the s-s-secret recipe for their mysterious blackrock stew, we can sell it freely and weaken their power bas-s-se.
         * <br><br>I s-s-seek assistance in this project.  If you will assist, I will give you a copy of the recipe when I discover it.<br><br>I need the following
         * ingredients-s-s, the arms of a Terathan Avenger, one bowl of rotworm stew, and five small pieces of blackrock.  Will you assis-s-st us-s-s? */
        public override object Description => 1150011;

        /* As-s-s you wish, ally.  Jus-s-st remember that whoever has this recipe, not only has power over the Bane Bragon but has power over thos-s-se who wish to ride them. */
        public override object Refuse => 1150012;

        /* Have you retrieved the ingredients-s-s?  Do not wait, or the Bane Chos-s-sen will des-s-stroy all.  Remember, I need five small blackrock pieces, one bowl
         * of rotworm s-s-stew, and the arms of a Terethan Avenger.*/
        public override object Uncomplete => 1150013;

        /* *** Salis takes the ingredients and slithers to the table to experiment.  After a few minutes of experimenting and tasting it slithers back to you.
         * ***<br><br>I have discovered the recipe to blackrock stew, the secret to the bane dragon's power.
         * As-s-s promised, I will share it with you.  Thank you for your assis-s-stance.*/
        public override object Complete => 1150014;

        public override bool CanOffer()
        {
            // TODO: Ophidian Points
            return true;
        }

        public override void GiveRewards()
        {
            base.GiveRewards();

            Owner.Backpack.DropItem(new RecipeScroll(604));
            Owner.SendLocalizedMessage(1074360, "#1150043"); // You receive a reward: ~1_REWARD~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class Salis : MondainQuester
    {
        public static Salis InstanceTram { get; set; }
        public static Salis InstanceFel { get; set; }

        public static void Initialize()
        {
            if (InstanceTram == null)
            {
                InstanceTram = new Salis();
                InstanceTram.MoveToWorld(new Point3D(5768, 2610, 46), Map.Trammel);
                InstanceTram.Home = InstanceTram.Location;
                InstanceTram.RangeHome = 5;
            }

            if (InstanceFel == null)
            {
                InstanceFel = new Salis();
                InstanceFel.MoveToWorld(new Point3D(5768, 2610, 46), Map.Felucca);
                InstanceFel.Home = InstanceFel.Location;
                InstanceFel.RangeHome = 5;
            }
        }

        public Salis()
            : base("Salis", "the Ophidian Cook")
        {
        }

        public Salis(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
        {
            typeof(DishBestServedColdQuest)
        };

        public override void InitBody()
        {
            InitStats(100, 100, 25);
            Body = 87;
        }

        public override void InitOutfit()
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

            reader.ReadInt();

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }
            else if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }
    }
}
