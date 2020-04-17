using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class BoundToTheLandQuest : BaseQuest
    {
        public BoundToTheLandQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(InsaneDryad), "insane dryads", 12));
            AddObjective(new SlayObjective(typeof(Saliva), "saliva", 1));

            AddReward(new BaseReward(typeof(DryadsBlessing), 1074345));
        }

        public override QuestChain ChainID => QuestChain.BlightedGrove;
        /* Bound to the Land */
        public override object Title => 1074433;
        /* Oh help! Help me!  I don't want to end up like my sisters, enslaved by the cruel Melisande.  I cannot flee, I am bound 
        here by the evil of the harpy Saliva.  Won't you please help me and redeem my sisters at the same time?  I will grant you 
        my blessing in return. */
        public override object Description => 1074434;
        /* Please, I'm so frightened.  I don't want to become twisted and corrupt.  Please free me! */
        public override object Refuse => 1074435;
        /* Until my sisters have been treated with mercy and released from their twisted existence and until the vile Saliva is 
        slain, I cannot feel safe or bestow my blessing. */
        public override object Uncomplete => 1074436;
        /* Oh!  You've returned.  I cannot thank you enough for saving me.  I only hope Melisande doesn't return them to life once 
        more.  Bless you, fair adventurer. Bless you.  If you wish to face Melisande in battle, place the token of my blessing in 
        the basket.  May you be triumphant and redeem us all through your efforts. */
        public override object Complete => 1074437;
        public override bool CanOffer()
        {
            return MondainsLegacy.BlightedGrove;
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

    public class FrightenedDryad : MondainQuester
    {
        [Constructable]
        public FrightenedDryad()
            : base("The Frightened Dryad")
        {
        }

        public FrightenedDryad(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(BoundToTheLandQuest)
                };

        public override void InitBody()
        {
            InitStats(100, 100, 25);
            Female = true;
            Body = 266;
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