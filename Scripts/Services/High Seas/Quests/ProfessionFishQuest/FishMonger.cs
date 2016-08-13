using Server;
using System;
using Server.Engines.Quests;
using Server.Multis;
using Server.Items;

namespace Server.Mobiles
{
    public class FishMonger : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { typeof(ProfessionalFisherQuest) }; } }

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

            AddItem(new ShortPants());
            AddItem(new Shirt());
            AddItem(new Sandals());
        }

        public override void OnTalk(PlayerMobile player)
        {
            int distance = 100;
            BaseBoat boat = FishQuestHelper.GetBoat(player);

            if (boat == null)
                SayTo(player, 1116514); //Bring yer ship around, I might have some work for ye!);
            else
            {
                bool inRange = InRange(boat.Location, distance) && boat.Map == this.Map;

                if (!FishQuestHelper.HasFishQuest(player, this, inRange))
                {
                    FishMonger monger = FishQuestHelper.GetRandomMonger(player, this);

                    if (monger == null)
                        SayTo(player, "It seems my fellow fish mongers are on vacation.  Try again later, or perhaps another Facet.");
                    else
                    {

                        ProfessionalFisherQuest quest = new ProfessionalFisherQuest(player, monger, this, boat);

                        if (quest != null)
                        {
                            quest.Quester = this;
                            quest.Owner = player;
                            player.CloseGump(typeof(MondainQuestGump));
                            player.SendGump(new MondainQuestGump(quest));

                            if (boat.IsClassicBoat)
                                this.SayTo(player, "Such a weak vessle can only catch a weak line.");
                        }
                    }
                }
            }
        }

        public FishMonger(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            FishQuestHelper.AddMonger(this);
        }
    }
}