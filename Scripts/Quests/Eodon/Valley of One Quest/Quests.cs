using Server.Engines.Harvest;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Engines.Quests
{
    public class TimeIsOfTheEssenceQuest : BaseQuest
    {
        public override QuestChain ChainID => QuestChain.ValleyOfOne;
        public override Type NextQuest => typeof(UnitingTheTribesQuest);

        public override object Title => 1156512;             /*The Valley of One*/

        public override object Description => 1156458;   /*Hawkwind: I'm afraid I have foreseen a most unfortunate timeline of events...
																		<br><br>Blackthorn: Tell me, what is it that troubles you?<br><br>Hawkwind: I 
																		brought the Britannians to Shadowguard to aid in the battle against Minax, in 
																		doing so I fear I may have placed the native peoples in grave peril. The 
																		Britannians are a curious and brave people, I fear that curiosity has put the 
																		Valley of Eodon at risk. Britannians seek to take advantage of the natural 
																		resources in Eodon.<br><br>Blackthorn: Such is the nature of all humans. Surely 
																		you knew of this risk?<br><br>Hawkwind: Indeed, for every action there is an 
																		equal and opposite reaction. Cause and effect are linked at the most fundamental
																		levels.<br><br>Blackthorn: And so what is the effect of the Britannian's 
																		adventurism?<br><br>Hawkwind: Scores of Dragon Turtles lay dead, tiger carcasses 
																		litter the jungle floor. Native tribes people have been murdered at the hands of 
																		what they only know as foreign invaders.<br><br>Blackthorn: *Blackthorn looks down 
																		solemnly* One must ask whether the ends justify the means...<br><br>Hawkwind: We 
																		could not allow Minax to take hold in Eodon.  As any other threat, I am certain the 
																		Britannians will answer the call.<br><br>Blackthorn: Tis the very nature of my Kingdom 
																		and its peoples to rise in a time of need.<br><br>Hawkwind: With so much death the 
																		Myrmidex have grown increasingly bold. The abundance of carrion has lured them from 
																		their dark pits to feed and bolster their numbers. The Barrab Tribe has seen this as 
																		an omen and increased the fervor of their zealotism. If the Myrmidex are not held in 
																		check, I fear the entire Valley will be overrun.<br><br>Blackthorn: Is there no way to
																		stop the invasion?<br><br>Hawkwind: There is always another path on the Time Line. 
																		We must unite the Tribes of Eodon against the Barrab. Only then can they stop the 
																		Myrmidex invasion.<br><br>Blackthorn: *King Blackthorn smiles at you* Well then!  
																		Take these orders to Sir Geoffrey at his camp near the Barrab Tribe in the Valley of 
																		Eodon.  Godspeed!*/

        public override object Refuse => 1156460;            /* Tis unfortunate, in this time of great need we must look to our inner courage... */

        public override object Uncomplete => 1156461;        /*There is little time to delay. Deliver the orders to Sir Geoffrey at his camp near 
																		the Barrab Tribe in the Valley of Eodon.*/

        public override object Complete => 1156462;      /* Ahh, King Blackthorn sent you did he? */

        public TimeIsOfTheEssenceQuest() : base()
        {
            AddObjective(new DeliverObjective(typeof(KingBlackthornOrders), "Orders from King Blackthorn to Sir Geoffrey", 1, typeof(SirGeoffery), "Sir Geoffery", 360));

            AddReward(new BaseReward(1156459)); // A step closer to quelling the Myrmidex threat...
        }

        public override bool RenderObjective(MondainQuestGump g, bool offer)
        {
            if (offer)
                g.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                g.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

            g.AddButton(130, 430, 0x2EEF, 0x2EF1, (int)Buttons.PreviousPage, GumpButtonType.Reply, 0);
            g.AddButton(275, 430, 0x2EE9, 0x2EEB, (int)Buttons.NextPage, GumpButtonType.Reply, 0);

            g.AddHtmlObject(160, 70, 330, 16, Title, BaseQuestGump.DarkGreen, false, false);
            g.AddHtmlLocalized(98, 140, 312, 16, 1049073, 0x2710, false, false); // Objective:
            g.AddHtmlLocalized(98, 156, 312, 16, 1072208, 0x2710, false, false); // All of the following	
            g.AddHtmlLocalized(98, 1, 40, 16, 1072207, 0x15F90, false, false); // Deliver
            g.AddHtmlLocalized(143, 172, 300, 16, 1156516, 0xFFFF, false, false); // Orders from King Blackthorn to Sir Geoffrey

            return true;
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

    public class UnitingTheTribesQuest : BaseQuest
    {
        public override QuestChain ChainID => QuestChain.ValleyOfOne;

        public override object Title => 1156463;             /* Uniting the Tribes */

        public override object Description => 1156464;   /* *Sir Geoffrey looks up at you* What's this? Ah! Here to join the effort then?
																		We've been trying for weeks to push through to the Barrab and get them to give 
																		us the secrets to entering the Myrmidex pits, but alas we have had little luck.  
																		King Blackthorn seems to think our only hope is to pressure the Barrab via the 
																		other Eodonian tribes.  We tried to approach them initially but were met with 
																		aggression.  It wasn't until Professor Rafkin arrived that we were able to peacefully 
																		communicate with them.  In order for them to join our cause you'll need to gain their 
																		trust.  The Sakkhra worship these overgrown Lizardmen, "Di-no-saur" is the word they 
																		use, bunch of rubbish if you ask me, but you should find their High Chief in the 
																		Sakkhra walled village to the North beyond the rivers.  The Urali worship the 
																		Dragon Turtle, shucks, to be honest I thought such a creature was myth until one 
																		came crashing up on a beach during one of our early expeditions.  You can find the 
																		Urali just north of the beach lined streams that are home to the Dragon Turtles.  The 
																		Jukari worship the great Volcano.  Nasty bunch too, took out a company of my men with 
																		pickaxes to the skull.  In any case, they can be found at the base of the Volcano to 
																		the Southeast.  The Kurak are lightning fast and worship the jungle cats they call 
																		"Ti-gers"  Cute little things until one rips you apart and drags you back to its den 
																		*chuckles a bit*  Their tribe is in the lowlands to the Southwest.  The Barako live 
																		high up in the trees and worship the gorillas that populate that region.  Professor 
																		Rafkin says something about a mythical giant ape, bunch of baloney if you ask me.  
																		Their tribe sits in the forest canopy to the East.  Bring me back proof that the 
																		tribes have agreed to join our cause and you'll be rewarded handsomely.  Or this jungle 
																		will tear you apart, whichever happens first.  Good luck!*/

        public override object Refuse => 1156472;            /* Well then stop wasting my bloody time! Haven't you see we've got a war to win?! */

        public override object Uncomplete => 1156518;        /* These tribes aren't going to unite themselves, best get on with it and visit the Sakkhra, 
																		Urali, Jukari, Kurak, and Barako tribes!  We've got a war to win! */

        public override object Complete => 1156519;      /*Well done! I don't know how you managed to pull it off but this is the break we've 
																		been waiting for! With the support of the other tribes we can begin to plan an offensive
																		to quell the Myrmidex threat!  It will likely take some time before our troops and the
																		Tribes are ready.  I will send word when the time to attack comes!*/

        public UnitingTheTribesQuest() : base()
        {
            AddObjective(new ObtainObjective(typeof(MosaicOfHeluzz), "Trust of the Sakkhra Tribe", 1));
            AddObjective(new ObtainObjective(typeof(TotemOfFabozz), "Trust of the Urali Tribe", 1));
            AddObjective(new ObtainObjective(typeof(FiresOfKukuzz), "Trust of the Jukari Tribe", 1));
            AddObjective(new ObtainObjective(typeof(SkullOfMotazz), "Trust of the Kurak Tribe", 1));
            AddObjective(new ObtainObjective(typeof(SkullOfAphazz), "Trust of the Barako Tribe", 1));

            AddReward(new BaseReward(typeof(UniqueTreasureBag), 1, 1156581)); // A bag with a unique treasure
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

    public class TheGreatHuntQuest : BaseQuest
    {
        public override object Title => 1156521;             /* The Great Hunt */

        public override object Description => 1156522;   /* *The Sakkhra High Chieftess looks at you apprehensively, 
																		as the greeting Professor Rafkin's book taught you rolls from your tongue The 
																		High Chief smiles and warms to you*  Rafkin say you can help Sakkhra.  Sakkhra 
																		do not trust you.  You want Sakkhra trust, then you fight biggest of all dinosaur, 
																		T Rex!  You survive the T Rex hunt, then Sakkhra trust you.  *The Chief motions you
																		west to the nearby dinosaur plains* */

        public override object Refuse => 1156550;            /* *The Chieftess looks at you...* */

        public override object Uncomplete => 1156531;        /* You not kill giant dino yet? Sakkhra will not help until you kill big dino! */

        public override object Complete => 1156557;      /* Ahh-Ooo! Ahh-Ooo!  Britainnian kill big dino! Show courage and strength!  Sakkrah trust you now! */

        public TheGreatHuntQuest() : base()
        {
            AddObjective(new SlayObjective(typeof(TRex), "Slay the Tyrannosaurus Rex", 1));
            AddReward(new BaseReward(typeof(MosaicOfHeluzz), 1, 1156551)); // Trust of the Sakkhra Tribe
        }

        public override bool RenderObjective(MondainQuestGump g, bool offer)
        {
            if (offer)
                g.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                g.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

            g.AddButton(130, 430, 0x2EEF, 0x2EF1, (int)Buttons.PreviousPage, GumpButtonType.Reply, 0);
            g.AddButton(275, 430, 0x2EE9, 0x2EEB, (int)Buttons.NextPage, GumpButtonType.Reply, 0);

            g.AddHtmlObject(160, 70, 330, 16, Title, BaseQuestGump.DarkGreen, false, false);
            g.AddHtmlLocalized(98, 140, 312, 16, 1049073, 0x2710, false, false); // Objective:
            g.AddHtmlLocalized(98, 156, 312, 16, 1072208, 0x2710, false, false); // All of the following	
            g.AddHtmlLocalized(98, 172, 300, 16, 1156533, 0xFFFF, false, false); // Slay the Tyrannosaurus Rex

            return true;
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

    public class EmptyNestQuest : BaseQuest
    {
        public override object Title => 1156523;             /* Empty Nest */

        public override object Description => 1156524;   /* *The Urali High Chieftess looks at you apprehensively, as the greeting Professor 
																		Rafkin's book taught you rolls from your tongue The Chieftess smiles and warms to you*  
																		Rafkin say you can help Urali.  Urali do not trust you.  You want Urali trust, then you
																		stop them taking Dragon Turtle Egg.  You stop Dragon Turtle Egg theft, then Urali trust you.  
																		*The Chieftess motions you south towards the nearby nesting beach* */


        public override object Refuse => 1156550;            /* *The Chieftess looks at you...* */

        public override object Uncomplete => 1156536;    /* You not save Dragon Turtle Hatchlings yet? Urali will not help until you save Dragon Turtle Hatchlings! */

        public override object Complete => 1156537;          /* Ahhh-OOO! Ahhh-OOO!  Dragon Turtle dig-dig and swim-swim! You good to Urali! Urali trust you! */

        public EmptyNestQuest() : base()
        {
            AddObjective(new InternalObjective());
            AddReward(new BaseReward(typeof(TotemOfFabozz), 1, 1156552)); // Trust of the Urali Tribe
        }

        public override bool RenderObjective(MondainQuestGump g, bool offer)
        {
            if (offer)
                g.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                g.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

            g.AddButton(130, 430, 0x2EEF, 0x2EF1, (int)Buttons.PreviousPage, GumpButtonType.Reply, 0);
            g.AddButton(275, 430, 0x2EE9, 0x2EEB, (int)Buttons.NextPage, GumpButtonType.Reply, 0);

            g.AddHtmlObject(160, 70, 330, 16, Title, BaseQuestGump.DarkGreen, false, false);
            g.AddHtmlLocalized(98, 140, 312, 16, 1049073, 0x2710, false, false); // Objective:
            g.AddHtmlLocalized(98, 156, 312, 16, 1072208, 0x2710, false, false); // All of the following
            g.AddHtmlLocalized(98, 172, 300, 16, 1156534, 0xFFFF, false, false); // Rescue 5 Dragon Turtle Hatchlings from poachers

            return true;
        }

        public void Update(object o)
        {
            foreach (BaseObjective obj in Objectives)
            {
                obj.Update(o);

                if (!Completed)
                {
                    Owner.PlaySound(UpdateSound);
                    Owner.SendLocalizedMessage(1156535, string.Format("{0}\t{1}", obj.CurProgress.ToString(), obj.MaxProgress.ToString())); // You have rescued ~1_count~ of ~2_max~ Dragon Turtle Hatchlings.
                }
                else
                {
                    OnCompleted();
                }
            }
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1046258, null, 0x23); // Your quest is complete.												
            Owner.PlaySound(CompleteSound);
        }

        public class InternalObjective : BaseObjective
        {
            public InternalObjective() : base(5)
            {
            }

            public override bool Update(object o)
            {
                CurProgress++;
                return true;
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

    public class TheGreatVolcanoQuest : BaseQuest
    {
        public override object Title => 1156525;             /* The Great Volcano */

        public override object Description => 1156526;   /* *The Jukari High Chief looks at you apprehensively, as the greeting Professor 
																		Rafkin's book taught you rolls from your tongue The Chief smiles and warms to you* 
																		Rafkin say you can help Jukari. Jukari do not trust you. You want Jukari trust, then 
																		you mine Jukari Lavastone from Volcano.  Volcano no eat you, then Jukari trust you. 
																		*The chief motions you east toward the Volcano* */


        public override object Refuse => 1156532;            /* *The Chief looks at you...* */

        public override object Uncomplete => 1156540;    /* You afraid of Great Volcano? Ha Ha! Britannian afraid! */

        public override object Complete => 1156541;          /* OOO!  You bring Jukari lava rock, mean Volcano no like your taste HA HA! Jukari help you! */

        public TheGreatVolcanoQuest() : base()
        {
            //AddObjective( new ObtainObjective( typeof(LavaStone), "Recover 5 lava rocks from the Caldera of the Great Volcano", 5 ) );
            AddObjective(new InternalObjective());
            AddReward(new BaseReward(typeof(FiresOfKukuzz), 1, 1156553)); // Trust of the Jukari Tribe
        }

        public static Rectangle2D VolcanoMineBounds = new Rectangle2D(879, 1568, 95, 95);

        public static bool OnHarvest(Mobile m, Item tool)
        {
            if (!(m is PlayerMobile) || m.Map != Map.TerMur)
                return false;

            PlayerMobile pm = m as PlayerMobile;

            if ((pm.ToggleMiningStone || pm.ToggleStoneOnly) && VolcanoMineBounds.Contains(m.Location))
            {
                object locked = tool;

                if (!m.BeginAction(locked))
                    return false;

                m.Animate(AnimationType.Attack, 3);

                Timer.DelayCall(Mining.System.OreAndStone.EffectSoundDelay, () =>
                {
                    m.PlaySound(Utility.RandomList(Mining.System.OreAndStone.EffectSounds));
                });

                Timer.DelayCall(Mining.System.OreAndStone.EffectDelay, () =>
                    {
                        TheGreatVolcanoQuest quest = QuestHelper.GetQuest(pm, typeof(TheGreatVolcanoQuest)) as TheGreatVolcanoQuest;

                        if (quest != null && !quest.Completed && 0.05 > Utility.RandomDouble())
                        {
                            if (m.CheckSkill(SkillName.Mining, 90, 100))
                            {
                                double chance = Utility.RandomDouble();

                                if (0.08 > chance)
                                {
                                    BaseCreature spawn = new VolcanoElemental();
                                    Point3D p = m.Location;

                                    for (int i = 0; i < 10; i++)
                                    {
                                        int x = Utility.RandomMinMax(p.X - 1, p.X + 1);
                                        int y = Utility.RandomMinMax(p.Y - 1, p.Y + 1);
                                        int z = Map.TerMur.GetAverageZ(x, y);

                                        if (Map.TerMur.CanSpawnMobile(x, y, z))
                                        {
                                            p = new Point3D(x, y, z);
                                            break;
                                        }
                                    }

                                    spawn.OnBeforeSpawn(p, Map.TerMur);
                                    spawn.MoveToWorld(p, Map.TerMur);
                                    spawn.OnAfterSpawn();

                                    spawn.Combatant = m;

                                    m.SendLocalizedMessage(1156508);  // Uh oh...that doesn't look like a lava rock!
                                }
                                else if (0.55 > chance)
                                {
                                    m.PrivateOverheadMessage(MessageType.Regular, 1154, 1156507, m.NetState); // *You uncover a lava rock and carefully store it for later!*
                                    quest.Update(m);
                                }
                                else
                                    m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1156509); // You loosen some dirt but fail to find anything.
                            }
                            else
                                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1156509); // You loosen some dirt but fail to find anything.
                        }
                        else
                            m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1156509); // You loosen some dirt but fail to find anything.

                        if (tool is IUsesRemaining)
                        {
                            ((IUsesRemaining)tool).UsesRemaining--;

                            if (((IUsesRemaining)tool).UsesRemaining <= 0)
                            {
                                m.SendLocalizedMessage(1044038); // You have worn out your tool!
                                tool.Delete();
                            }
                        }

                        m.EndAction(locked);
                    });

                return true;
            }

            return false;
        }

        public bool Update(object o)
        {
            foreach (BaseObjective obj in Objectives)
            {
                obj.Update(o);

                if (!Completed)
                {
                    Owner.PlaySound(UpdateSound);
                    Owner.SendLocalizedMessage(1156539, string.Format("{0}\t{1}", obj.CurProgress.ToString(), obj.MaxProgress.ToString())); // You have recovered ~1_count~ of ~2_max~ Lava Rocks.
                }
                else
                {
                    OnCompleted();
                }
            }

            return true;
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription => 1156538;

            public InternalObjective()
                : base(5)
            {
            }

            public override bool Update(object o)
            {
                CurProgress++;
                return true;
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

    public class PrideOfTheAmbushQuest : BaseQuest
    {
        public override object Title => 1156527;             /* The Pride of the Ambush */

        public override object Description => 1156528;   /* *The Kurak High Chief looks at you apprehensively, as the greeting Professor 
																		Rafkin's book taught you rolls from your tongue The Chieftess smiles and warms 
																		to you*  Rafkin say you can help Kurak.  Kurak do not trust you.  You want Kurak
																		trust, then you free cubs from trappers.  You free cubs from trappers, then 
																		Kurak trust you. *The chieftess motions you south to the lowlands* */


        public override object Refuse => 1156532;            /* *The Chief looks at you...* */

        public override object Uncomplete => 1156545;    /* You save tiger cubs! They little and helpless! Kurak will not help until you save tiger cubs! */

        public override object Complete => 1156546;      /* You save tiger cubs! Kurak thank you and help you! */

        public PrideOfTheAmbushQuest() : base()
        {
            AddObjective(new InternalObjective());
            AddReward(new BaseReward(typeof(SkullOfMotazz), 1, 1156554));  // Trust of the Kurak Tribe
        }

        public override bool RenderObjective(MondainQuestGump g, bool offer)
        {
            if (offer)
                g.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                g.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

            g.AddButton(130, 430, 0x2EEF, 0x2EF1, (int)Buttons.PreviousPage, GumpButtonType.Reply, 0);
            g.AddButton(275, 430, 0x2EE9, 0x2EEB, (int)Buttons.NextPage, GumpButtonType.Reply, 0);

            g.AddHtmlObject(160, 70, 330, 16, Title, BaseQuestGump.DarkGreen, false, false);
            g.AddHtmlLocalized(98, 140, 312, 16, 1049073, 0x2710, false, false); // Objective:
            g.AddHtmlLocalized(98, 156, 312, 16, 1072208, 0x2710, false, false); // All of the following	
            g.AddHtmlLocalized(98, 172, 300, 16, 1156544, 0xFFFF, false, false); // Rescue 5 tiger cubs from the trapper enclosures.

            return true;
        }

        public bool Update(object o)
        {
            foreach (BaseObjective obj in Objectives)
            {
                obj.Update(o);

                if (!Completed)
                {
                    Owner.PlaySound(UpdateSound);
                    Owner.SendLocalizedMessage(1156543, string.Format("{0}\t{1}", obj.CurProgress.ToString(), obj.MaxProgress.ToString())); // You have rescued ~1_count~ of ~2_max~ tiger cubs.
                }
                else
                {
                    OnCompleted();
                }
            }

            return true;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1046258, null, 0x23); // Your quest is complete.												
            Owner.PlaySound(CompleteSound);
        }

        public class InternalObjective : BaseObjective
        {
            public InternalObjective()
                : base(5)
            {
            }

            public override bool Update(object o)
            {
                CurProgress++;
                return true;
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

    public class TheGreatApeQuest : BaseQuest
    {
        public override object Title => 1156529;             /* The Great Ape */

        public override object Description => 1156530;   /* *The Barako High Chief looks at you apprehensively, as the greeting 
																		Professor Rafkin's book taught you rolls from your tongue The Chief smiles 
																		and warms to you*  Rafkin say you can help Barako.  Barako do not trust you.
																		You want Barako trust, then you battle Great Ape.  You go to banana cave and
																		ape no eat you, then Barako trust you.  *The Chief motions you northeast toward
																		the nearby banana cave* */


        public override object Refuse => 1156532;            /* *The Chief looks at you...* */

        public override object Uncomplete => 1156548;    /* *Thumps chest!* You battle Great Ape or Barako will not trust you! */

        public override object Complete => 1156549;          /* *Thumps chest* You battle Great Ape and win! You strong and Barako respect and trust you! */

        public TheGreatApeQuest() : base()
        {
            AddObjective(new SlayObjective(typeof(GreatApe), "Defeat the Great Ape", 1)); // Defeat the Great Ape    
            AddReward(new BaseReward(typeof(SkullOfAphazz), 1, 1156555)); // Trust of the Barako Tribe
        }

        public override bool RenderObjective(MondainQuestGump g, bool offer)
        {
            if (offer)
                g.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                g.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

            g.AddButton(130, 430, 0x2EEF, 0x2EF1, (int)Buttons.PreviousPage, GumpButtonType.Reply, 0);
            g.AddButton(275, 430, 0x2EE9, 0x2EEB, (int)Buttons.NextPage, GumpButtonType.Reply, 0);

            g.AddHtmlObject(160, 70, 330, 16, Title, BaseQuestGump.DarkGreen, false, false);
            g.AddHtmlLocalized(98, 140, 312, 16, 1049073, 0x2710, false, false); // Objective:
            g.AddHtmlLocalized(98, 156, 312, 16, 1072208, 0x2710, false, false); // All of the following	
            g.AddHtmlLocalized(98, 172, 300, 16, 1156547, 0xFFFF, false, false); // Defeat the Great Ape

            return true;
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