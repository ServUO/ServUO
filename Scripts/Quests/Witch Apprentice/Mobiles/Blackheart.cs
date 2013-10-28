using System;
using Server.Items;
using Server.Mobiles;

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
            this.InitStats(100, 100, 25);

            this.Hue = 0x83EF;

            this.Female = false;
            this.Body = 0x190;
            this.Name = "Captain Blackheart";
        }

        public override void InitOutfit()
        {
            this.AddItem(new FancyShirt());
            this.AddItem(new LongPants(0x66D));
            this.AddItem(new ThighBoots());
            this.AddItem(new TricorneHat(0x1));
            this.AddItem(new BodySash(0x66D));

            LeatherGloves gloves = new LeatherGloves();
            gloves.Hue = 0x66D;
            this.AddItem(gloves);

            this.FacialHairItemID = 0x203E; // Long Beard
            this.FacialHairHue = 0x455;

            Item sword = new Cutlass();
            sword.Movable = false;
            this.AddItem(sword);
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            this.Direction = this.GetDirectionTo(player);
            this.Animate(33, 20, 1, true, false, 0);

            QuestSystem qs = player.Quest;

            if (qs is WitchApprenticeQuest)
            {
                FindIngredientObjective obj = qs.FindObjective(typeof(FindIngredientObjective)) as FindIngredientObjective;

                if (obj != null && !obj.Completed && obj.Ingredient == Ingredient.Whiskey)
                {
                    this.PlaySound(Utility.RandomBool() ? 0x42E : 0x43F);

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

            this.PlaySound(0x42C);
            this.SayTo(player, 1055041); // The drunken pirate shakes his fist at you and goes back to drinking.
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

            this.Heave();
        }

        private void Heave()
        {
            this.PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, 500849); // *hic*

            Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(60, 180)), new TimerCallback(Heave));
        }
    }
}