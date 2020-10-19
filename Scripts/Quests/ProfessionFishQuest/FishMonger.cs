using Server.Engines.Quests;
using Server.Items;
using Server.Multis;
using System;

namespace Server.Mobiles
{
    public class FishMonger : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(ProfessionalFisherQuest) };

        [Constructable]
        public FishMonger()
        {
            FishQuestHelper.AddMonger(this);
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Name = NameList.RandomName("male");
            Title = "the fish monger";

            Hue = Race.RandomSkinHue();
            Race.RandomHair(this);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new ShortPants());
            SetWearable(new Shirt());
            SetWearable(new Sandals());
        }

        public override void OnTalk(PlayerMobile player)
        {
            int distance = 100;
            BaseBoat boat = FishQuestHelper.GetBoat(player);

            if (boat == null)
                SayTo(player, 1116514); //Bring yer ship around, I might have some work for ye!);
            else
            {
                bool inRange = InRange(boat.Location, distance) && boat.Map == Map;

                if (!FishQuestHelper.HasFishQuest(player, this, inRange))
                {
                    FishMonger monger = FishQuestHelper.GetRandomMonger(player, this);

                    if (monger == null)
                        SayTo(player, "It seems my fellow fish mongers are on vacation.  Try again later, or perhaps another Facet.");
                    else
                    {
                        ProfessionalFisherQuest quest = new ProfessionalFisherQuest(player, monger, this, boat);

                        quest.Quester = this;
                        quest.Owner = player;
                        player.CloseGump(typeof(MondainQuestGump));
                        player.SendGump(new MondainQuestGump(quest));

                        if (boat.IsClassicBoat)
                            SayTo(player, "Such a weak vessle can only catch a weak line.");
                    }
                }
            }
        }

        public FishMonger(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            FishQuestHelper.AddMonger(this);
        }
    }
}
