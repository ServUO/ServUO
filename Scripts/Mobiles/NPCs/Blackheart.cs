using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests.Hag
{
    public class Blackheart : BaseQuester
    {
        [Constructable]
        public Blackheart()
            : base("the Drunken Pirate")
        {
        }

        public Blackheart(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Hue = 0x83EF;

            Female = false;
            Body = 0x190;
            Name = "Captain Blackheart";
        }

        public override void InitOutfit()
        {
            AddItem(new FancyShirt());
            AddItem(new LongPants(0x66D));
            AddItem(new ThighBoots());
            AddItem(new TricorneHat(0x1));
            AddItem(new BodySash(0x66D));

            LeatherGloves gloves = new LeatherGloves
            {
                Hue = 0x66D
            };
            AddItem(gloves);

            FacialHairItemID = 0x203E; // Long Beard
            FacialHairHue = 0x455;

            Item sword = new Cutlass
            {
                Movable = false
            };
            AddItem(sword);
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            Direction = GetDirectionTo(player);
            Animate(33, 20, 1, true, false, 0);

            QuestSystem qs = player.Quest;

            if (qs is WitchApprenticeQuest)
            {
                FindIngredientObjective obj = qs.FindObjective(typeof(FindIngredientObjective)) as FindIngredientObjective;

                if (obj != null && !obj.Completed && obj.Ingredient == Ingredient.Whiskey)
                {
                    PlaySound(Utility.RandomBool() ? 0x42E : 0x43F);

                    Item hat = player.FindItemOnLayer(Layer.Helm);
                    bool tricorne = hat is TricorneHat;

                    if (tricorne && player.BAC >= 20)
                    {
                        obj.Complete();

                        if (obj.BlackheartMet)
                            qs.AddConversation(new BlackheartPirateConversation(false));
                        else
                            qs.AddConversation(new BlackheartPirateConversation(true));
                    }
                    else if (!obj.BlackheartMet)
                    {
                        obj.Complete();

                        qs.AddConversation(new BlackheartFirstConversation());
                    }
                    else
                    {
                        qs.AddConversation(new BlackheartNoPirateConversation(tricorne, player.BAC > 0));
                    }

                    return;
                }
            }

            PlaySound(0x42C);
            SayTo(player, 1055041); // The drunken pirate shakes his fist at you and goes back to drinking.
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

            Heave();
        }

        private void Heave()
        {
            PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, 500849); // *hic*

            Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(60, 180)), Heave);
        }
    }
}
