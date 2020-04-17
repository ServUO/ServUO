using Server.Items;

namespace Server.Engines.Quests
{
    public class UnusualGoods : BaseQuest
    {
        public UnusualGoods()
        {
            AddObjective(new ObtainObjective(typeof(PerfectEmerald), "Perfect Emerald", 2, 0x3194));

            AddObjective(new ObtainObjective(typeof(CrystallineBlackrock), "Crystalline Blackrock", 1, 0x5732));

            AddReward(new BaseReward(typeof(EssenceBox), "Essence Box"));

            AddReward(new BaseReward("Loyalty Rating"));
        }

        /*Unusual Goods*/

        public override object Title => 1113787;

        /*Psst. Do you want to buy something rare and valuable? Yes? Good. I have in my possession an imbuing ingredient that is highly sought after. 
        If you wish to make a trade, it will not come cheaply. 
        Provide me with two perfect emeralds and one piece of crystalline blackrock, and what is in this box shall be yours. */

        public override object Description => 1113788;

        /*It is your choice, but do not speak of this to anyone else.*/

        public override object Refuse => 1113789;

        /*In exchange for this bag, I want two perfect emeralds and one piece of crystalline blackrock. Nothing more, nothing less.*/

        public override object Uncomplete => 1113790;

        /*Let me see what you have brought me. Yes, this is of fine quality, and I accept it in trade. 
        Here is your box. Please do not spread word of our deal, as I do not want attention brought upon me. I am sure you understand.*/

        public override object Complete => 1113791;

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