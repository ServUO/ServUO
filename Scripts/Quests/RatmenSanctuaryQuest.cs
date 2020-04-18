using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class SpringCleaningQuest : BaseQuest
    {

        /* Spring Cleaning */
        public override object Title => 1072373;

        /*Mangy, lice encrusted furballs!  Those filthy ratmen have been sneaking into camp again,
          the signs are obvious.  They've fouled the water stores again and stolen what they  
          couldn't eat.  Are you up to cleaning up the rat warrens?   */
        public override object Description => 1072674;

        /* I understand.  The stench in those tunnels is unbearable. */
        public override object Refuse => 1072684;

        /* The ratmen have holes all over the place that lead to their warrens.  Sometimes they're 
           loitering around on the surface too.  Either way, they're not hard to find. */
        public override object Uncomplete => 1072685;

        /* Excellent! That’s the old fighting spirit. */
        public override object Complete => 1075384;

        public SpringCleaningQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Ratman), "Ratmen", 15, "Sanctuary"));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
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

    public class CowardsQuest : BaseQuest
    {

        /* Cowards! */
        public override object Title => 1072689;

        /* When you were exterminating those untidy vermin, you must have noticed the archers amongst them? 
           Those cowardly rats have been peppering us with arrows and then running away before we can retaliate.
           Can I count on you to handle this for me? */
        public override object Description => 1072697;

        /* I understand.  The stench in those tunnels is unbearable. */
        public override object Refuse => 1072684;

        /* The ratmen have holes all over the place that lead to their warrens.  Sometimes they're 
           loitering around on the surface too.  Either way, they're not hard to find. */
        public override object Uncomplete => 1072685;

        /* Hah! I knew you were up to the challenge. */
        public override object Complete => 1075409;

        public CowardsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(RatmanArcher), "Ratmen Archers", 10, "Sanctuary"));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
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

    public class TokenOfLoveQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        public TokenOfLoveQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(StolenRing), "A ring engraved: 'Beloved Ciala'", 1, 0x1F09));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Token of Love */
        public override object Title => 1072690;
        /* Argh!  I knew those ratmen were up to no good!  They have stolen the ring I was making for my beloved Ciala.
         The sparkle must have caught their greedy eyes.  Find the ring please!  I beg you.  */
        public override object Description => 1072698;
        /* Please, I beg of you, reconsider! */
        public override object Refuse => 1072699;
        /* The stolen ring is easy enough to recognize.  It has an inscription on it declaring my love for Ciala.
           Please find this token of love for me! */
        public override object Uncomplete => 1072700;
        /* Ah!  You found my property.  Thank you for your honesty in returning it to me.*/
        public override object Complete => 1074582;
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

