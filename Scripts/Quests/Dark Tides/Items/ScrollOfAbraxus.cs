using System;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.Quests.Necro
{
    public class ScrollOfAbraxus : QuestItem
    {
        [Constructable]
        public ScrollOfAbraxus()
            : base(0x227B)
        {
            this.Weight = 1.0;
        }

        public ScrollOfAbraxus(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1028827;
            }
        }// Scroll of Abraxus
        public override bool CanDrop(PlayerMobile player)
        {
            DarkTidesQuest qs = player.Quest as DarkTidesQuest;

            if (qs == null)
                return true;

            //return !( qs.IsObjectiveInProgress( typeof( RetrieveAbraxusScrollObjective ) ) || qs.IsObjectiveInProgress( typeof( ReadAbraxusScrollObjective ) ) );
            return false;
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            PlayerMobile pm = this.RootParent as PlayerMobile;

            if (pm != null)
            {
                QuestSystem qs = pm.Quest;

                if (qs is DarkTidesQuest)
                {
                    QuestObjective obj = qs.FindObjective(typeof(RetrieveAbraxusScrollObjective));

                    if (obj != null && !obj.Completed)
                        obj.Complete();
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.SendGump(new ScrollOfAbraxusGump());

                PlayerMobile pm = from as PlayerMobile;

                if (pm != null)
                {
                    QuestSystem qs = pm.Quest;

                    if (qs is DarkTidesQuest)
                    {
                        QuestObjective obj = qs.FindObjective(typeof(ReadAbraxusScrollObjective));

                        if (obj != null && !obj.Completed)
                            obj.Complete();
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
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
        }
    }

    public class ScrollOfAbraxusGump : Gump
    {
        public ScrollOfAbraxusGump()
            : base(150, 50)
        {
            this.AddPage(0);

            this.AddImage(0, 0, 1228);
            this.AddImage(340, 255, 9005);

            /* Security at the Crystal Cave<BR><BR>
            * 
            * We have taken great measuresto ensure the safety of the
            * Scroll of Calling, which we have so valiantly taken from
            * the Necromancer Maabus during the battle of the wood
            * nearly 200 years ago.<BR><BR>
            * 
            * The scroll must never fall into the hands of the
            * Necromancers again, lest they use it to summon the ancient
            * daemon Kronus.  The scroll of calling is a necessity in the
            * series of dark rites the Necromancers must perform to once again
            * re-awaken Kronus.<BR><BR>
            * 
            * Should Kronus ever rise again, the days of the Paladins, and
            * indeed humanity as we know it will be numbered.<BR><BR>
            * 
            * For this reason, we have posted the honorable Horus, former
            * General of the Northern Legions to guard the entrance of the
            * Crystal Cave where we keep the Scroll of Calling.  Horus was
            * infused with magical life from the tree Urywen during his last
            * battle.  The power gave him eternal life, but it also,
            * unfortunately, took his eye sight.<BR><BR>
            * 
            * Since Horus cannot see those he admits to the Crystal Cave,
            * he will only allow those that know the secret password to enter.
            * Speak the following word to Horus and he shall grant you passage
            * to the Crystal Cave:<BR><BR>
            * 
            * <I>Urywen</I><BR><BR>
            * 
            * Do not speak this password anywhere except when seeking passage
            * into the Crystal Cave, as our adversaries are lurking in the
            * shadows – they are everywhere.<BR><BR>Go with the light, friend.<BR><BR>
            * 
            * <I>- Frater Melkeer</I>
            */
            this.AddHtmlLocalized(25, 36, 350, 210, 1060116, 1, false, true);
        }
    }
}