using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class UntanglingTheWebQuest : BaseQuest
    {
        public UntanglingTheWebQuest()
            : base()
        {
            AddObjective(new AcidCreaturesObjective(typeof(IAcidCreature), "acid creatures", 12));

            AddReward(new BaseReward(typeof(AcidPopper), 1095058));
        }

        /* Untangling the Web */
        public override object Title => 1095050;
        /* Kill Acid Slugs and Acid Elementals to fill Vernix's jars.  Return to Vernix with the filled jars for
        your reward.<br><center>-----</center><br>Vernix say, stranger has proven big power.  You now ready to 
        help Green Goblins big time.  Green Gobin and outsider not need to be enemy.  Need to be friend against 
        common enemy.  You help Green Goblins with important mission.  We tell you important information.  Help 
        your people not be eaten.<br><br>Go find acid slugs and acid elementals.  They very dangerous, but me 
        think you can handle it.  Fill these jars with acid from these. */
        public override object Description => 1095052;
        /* Hmm... Perhaps you are afraid.  Hmm... Very good to know.  Ok, you go and do.  You come back.
        Let me know if you stop being afraid of acid. */
        public override object Refuse => 1095053;
        /* Acid very important to stopping master plan of Gray Goblins.  You get acid, ok? */
        public override object Uncomplete => 1095054;
        /* This very good.  Now King Vernix tell you valuable secret.  Acid good for melting wolf spider webs.
        Webs very strong, but not stronger than acid.  Vernix gives to you pay for good work. */
        public override object Complete => 1095057;
        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1095056, null, 0x23); // Vernix's Jars are now full.							
            Owner.PlaySound(CompleteSound);
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

        private class AcidCreaturesObjective : SlayObjective
        {
            public AcidCreaturesObjective(Type creature, string name, int amount)
                : base(creature, name, amount)
            {
            }

            public override void OnKill(Mobile killed)
            {
                base.OnKill(killed);

                if (!Completed)
                    Quest.Owner.SendLocalizedMessage(1095055); // You collect acid from the creature into the jar.
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

    public class GreenWithEnvyQuest : BaseQuest
    {
        public GreenWithEnvyQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(EyeOfNavrey), "eye of Navrey", 1, 0x1F1C));

            AddReward(new BaseReward(typeof(RewardBox), 1072584));
        }

        /* Green with Envy */
        public override object Title => 1095118;
        /* Kill the queen spider, Navrey Night-Eyes.  Bring proof of her death to King Vernix.<br><center>-----</center>
        <br>Vernix plan is now ready.  Man thing from outside make good champion for Green Goblins.  King Vernix will 
        let him in on plan.  Gray goblin power comes from their god, Navrey Night-Eyes.  Navery is great spider. 
        Very nasty.  Gray Goblins and Green Goblins used to be one tribe, but Gray Goblins gain power from Navery and 
        make Green Goblins slaves.  Green Goblins escape tribe and find own place.<br><br>Navery Night-Eyes has big hunger. 
        Always need more blood.  Gray Goblins want to sacrifice outside kind to Navery so she not eat them.  You kill Navery, 
        you solve big problem for outside kind and goblin kind.<br><br>If you do this, you take big risk so Vernix make it 
        worth your while.  Kill Navery Night-Eyes. */
        public override object Description => 1095120;
        /* You no kill her, then many outside people disappear at night when others sleep.  You think about it, then
        come back when you ready to make deal. */
        public override object Refuse => 1095121;
        /* You not have much time.  Navery Night-Eyes is hungry. */
        public override object Uncomplete => 1095122;
        /* You do good service to your people.  Now Green Goblins will do the rest.  Without power from Navery Night-Eyes,
        we will have our revenge.  Vernix keep Green Goblin end of deal. */
        public override object Complete => 1095123;
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

    public class Vernix : MondainQuester
    {
        [Constructable]
        public Vernix()
            : base("Vernix")
        {
        }

        public Vernix(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(UntanglingTheWebQuest),
                    typeof(GreenWithEnvyQuest),
                };

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 723;

            Frozen = true;
            Direction = Direction.East;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
        }

        public override void Advertise()
        {
            Say(Utility.RandomBool() ? 1095119 : 1095051);
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

            Frozen = true;
            Direction = Direction.East;
        }
    }
}