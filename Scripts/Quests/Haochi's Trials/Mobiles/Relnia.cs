using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    public class Relnia : BaseQuester
    {
        [Constructable]
        public Relnia()
            : base("the Gypsy")
        {
        }

        public Relnia(Serial serial)
            : base(serial)
        {
        }

        public override int TalkNumber
        {
            get
            {
                return -1;
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = 0x83FF;

            this.Female = true;
            this.Body = 0x191;
            this.Name = "Disheveled Relnia";
        }

        public override void InitOutfit()
        {
            this.HairItemID = 0x203C;
            this.HairHue = 0x654;

            this.AddItem(new ThighBoots(0x901));
            this.AddItem(new FancyShirt(0x5F3));
            this.AddItem(new SkullCap(0x6A7));
            this.AddItem(new Skirt(0x544));
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                QuestSystem qs = player.Quest;

                if (qs is HaochisTrialsQuest)
                {
                    QuestObjective obj = qs.FindObjective(typeof(FourthTrialCatsObjective));

                    if (obj != null && !obj.Completed)
                    {
                        Gold gold = dropped as Gold;

                        if (gold != null)
                        {
                            obj.Complete();
                            qs.AddObjective(new FourthTrialReturnObjective(false));

                            this.SayTo(from, 1063241); // I thank thee.  This gold will be a great help to me and mine!

                            gold.Consume(); // Intentional difference from OSI: don't take all the gold of poor newbies!
                            return gold.Deleted;
                        }
                    }
                }
            }

            return base.OnDragDrop(from, dropped);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}