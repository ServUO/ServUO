using Server.Items;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class JournalDrSpector1 : BaseJournal
    {
        public override TextDefinition Body => 1157036;
        /*Week 1<br><br>As I begin my journey across this strange land, I am not completely sure how to proceed. My supplies are limited, 
         * so I will likely need to search for food or find people with whom to trade. I also worry about becoming lost in the wilds. This 
         * is dangerous, especially without any weapons.<br> <br>I have managed to identify several fixed constellations by which to align
         * my sextant. It should give me the means to track my movements and map this new world. I plan to leave copies of my notes for 
         * others who may come after me. My journey started at 21° South 42° West. I plan to move north toward a settlement I saw from a 
         * tree at 8° South 45° West.<br> <br>Week 2<br> <br>The Barako Tribe worship the strength of the Great Apes of this land of Eodon. 
         * They are a martial people; both sexes train as hunters and warriors. While they prefer to use bludgeoning weapons, one of their 
         * greatest heroes, Halawa, used a bow to rescue her infant daughter from a silverback gorilla. The tribe has built its home in the 
         * high trees.<br> <br>It seems that there is great animosity between the Barako and another tribe known as the Kurak. I am going to 
         * leave before I am caught in the middle of their conflict. I have learned of another settlement at 3° North 62° West which may have 
         * answers.<br> <br>Week 3<br> <br>The Urali are a strange people. Their skin has a green tint, likely because of diet and dyes. The 
         * color matches the turtles that are common in the area. Their warriors carry shields made from discarded turtle shells. Like the 
         * Dragon Turtle they worship, all of their chieftains are women, the strongest shield duelists in the tribe. All of the shamans of 
         * the tribe are the male descendants of a Shaman named Wamap, who created the spells of protection they still use.<br><br>The Dragon
         * Turtle did not seem as intelligent as the draconic creatures of legend, and will thus give me no answers, so I will head out in a 
         * few days to 17° North 54° West.*/

        public override TextDefinition Title => 1157034;  // The Journal of Dr. Spector - Collection I

        [Constructable]
        public JournalDrSpector1()
        {
        }

        public JournalDrSpector1(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class JournalDrSpector2 : BaseJournal
    {
        public override TextDefinition Body => 1157037;
        /*Week 4<br><br>The Sakkhra both imitate and hunt the dinosaurs that thunder constantly in their section of Eodon. Given the great 
         * size of these creatures, the Sakkhra have chosen to use powerful bows to bring down the creatures from a distance. When the 
         * dinosaurs wander into their camp, the tribe forces the creature back with spears dipped in poison. In cases of stampede or 
         * attack, the tribe will also destroy the bridges across the rivers around their settlement. The tribe is the first I’ve seen 
         * that grows crops, but they do not show the advancement needed to get me home.<br> <br>While I enjoyed my visit with the Sakkhra,
         * I have decided to meet with the next of Eodon’s tribes. The Sakkhra have warned me about the “spiritless tribe,” but I believe 
         * I should have no problems. I will travel to 17° South 81° West.<br> <br>Week 5<br> <br>The Barrab are very much like the insects 
         * in desert below: organized, ruthless, and complicated. The Barrab worship the Myrmidex, a terrifying insect species that lives
         * underneath Eodon. The visible entrances to their hive are gigantic. Even though the Barrab worship the insects and dress themselves
         * in discarded carapaces, the Myrmidex are still aggressive to most humans. The worst criminals of the Barrab tribe are sacrificed 
         * to the drones in the sand pits below. Like the Myrmidex, the Barrab is a strict matriarchy. I did hear one story of a male shaman
         * and chief named Balakai who ruled with the help of magic and alchemy, but I was unable to translate the ending of his story to my
         * satisfaction. I did learn that female Barrab shamans get their power from something in the Myrmidex tunnels, though they seemed
         * offended when I asked for more information.<br> <br>While the Myrmidex may have answers to my questions, or possibly a Moonstone 
         * to return me home, I do not wish to be eaten alive by such creatures. I will move on to 20° South 67° West.*/

        public override TextDefinition Title => 1157035;  // The Journal of Dr. Spector - Collection II

        [Constructable]
        public JournalDrSpector2()
        {
        }

        public JournalDrSpector2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class JournalDrSpector3 : BaseJournal
    {
        public override TextDefinition Body => 1157054;
        /*Week 6<br><br>The Kurak may be the rivals of the Barako tribe, but I did not feel any hostility directed at me. While they are 
         * proud of their tiger totem and the animal they emulate, they were not as cruel as these predator cats. I did manage to see a 
         * demonstration of the strange claws they use for hunting and combat, and I am glad I stayed on their good side. I learned about
         * a strange ruin they avoid, something to do with their ancestors. It is to the east. I plan to visit it after I meet with the 
         * final tribe of Eodon at 40° South 41° West.<br><br>Week 7<br><br>The Jukari live among the lava flows in the shadow of a great 
         * volcanic range that blocks travel to the southeast. The tribe uses the fire and obsidian shards produced by the volcano as their
         * weapons, believing these to be gifts from the spirits. I asked if any had traveled into the volcanic range, and they mentioned
         * that one of their greatest chieftains walked into the mountains with a sacred hide that protected him from the volcano’s poisonous
         * gases. I don’t believe the answers I seek can be found within the volcano. That leaves only the pyramid at 20°South 58° West.
         * <br><br>Week 8<br><br>I have explored every ruin I have come across and spoken with all of the tribes. None can help me return
         * through the Ethereal Void to my home. The cracked Moonstone I used to travel did not come with me, and unless I can find another
         * one, I will be trapped here. From the ruins and people, I have learned a bit about various Eodon spirits in my time here:<br>Aphazz,
         * spirit of emotion and strength<br>Motazz, spirit of battle<Br>Heluzz, spirit of knowledge and visions<br>Fabozz, spirit of nature 
         * and animals<br>Kukuzz, spirit of healing<br>What is strange is that the Pyramid does not show the symbols or images of any of 
         * these spirits. It is almost as if someone or something else built it. I must investigate further. I hope to find some way to the
         * lower chambers of the Pyramid. */

        public override TextDefinition Title => 1157052;  // The Journal of Dr. Spector - Collection III

        [Constructable]
        public JournalDrSpector3()
        {
        }

        public JournalDrSpector3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class JournalDrSpector4 : BaseJournal
    {
        public override TextDefinition Body => 1157055;
        /*Week 9<br><br>In my travels around Eodon, I heard stories of the city of the World-Builders. While the passage at the top of the
         * Pyramid was sealed, I found a crack near the base of the structure. As I shimmied into the narrow tunnel, I disturbed a stone 
         * that was holding up a wall. While I was able to avoid injury, the collapse blocked my way out of the Pyramid. With only a single 
         * torch, I started my exploration of the lost city. The city was filled with the bones and dried remains of different creatures. I
         * recognized human and Myrmidex remains, but in all my time in Eodon, I saw nothing that matches the bipedal reptile skeletons in 
         * the tunnels. Are these what remain of the World-Builders? In the entire ruined city, only one large crystal had not been smashed
         * when the city fell. The vibrant crystal gave off a shifting spectrum of light, and the air around it was charged with energy. It
         * was hypnotic, and with no other choice, I put my hand against it. The light of the crystal became brighter and brighter. Even when 
         * my hand started to burn, I could not pull myself away. I heard a voice in my mind, speaking rapidly in a strange language. As I 
         * lost consciousness, one word stuck in my mind: Zipactriotl. I woke in a burning jungle clearing. Around me were dead or dying
         * Myrmidex, and my hands were burning with a strange blue flame. I did not remember slaughtering those creatures, but I knew I had.
         * The tribes had a saying, “Killing one Myrmidex brings the wrath of its thousand kin.” How much wrath have I earned by killing so
         * many?<br><br>Week 10<br><br>I am different now. My bones have reshaped themselves, and my skin is like armor. There is a charge of
         * energy around me, constantly buzzing in my ears. I am blacking out more and more, and I am losing more and more of myself. With 
         * the changes, hidden parts of the city now allow me access. I have found what appears to be a sort of stasis chamber. I will put 
         * myself inside and hope someone can help me. */

        public override TextDefinition Title => 1157053;  // The Journal of Dr. Spector - Collection IV

        [Constructable]
        public JournalDrSpector4()
        {
        }

        public JournalDrSpector4(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class HistoryOfTheGreatWok1 : BaseJournal
    {
        public override TextDefinition Body => 1157060;
        /*The Great Moonstone and the Destruction of the First Home<br><br>Spoken by the Memory of Katalkotl<br><br>Transcribed by Professor
         * Ellie Rafkin<br><br>Eodon was not the cradle of the Kotl. Before, there was Akaktolan, the First Home. While much is forgotten,
         * it was there where the Kotl learned the fundamentals of magic and lore. The First Home was struck by a great meteorite, and the 
         * world slipped into an ice age. Those Kotl who survived traveled across the wastes, seeking the source of their suffering. In the 
         * center of a huge crater, they found a large tenebrous moonstone. The blackrock artifact bent the very shadows around it. All the 
         * Kotl recognized the raw magic before them. Some shunned the moonstone, turning and walking back into the cold, never to be seen
         * again. They were the Mixkotl, the Lost. The rest remained with the moonstone, hoping to unlock its secrets before the end came. 
         * In the frozen crater, each day of study claimed another life, but eventually, the Kotl learned to wield the moonstone’s power. 
         * They used it to grasp and manipulate frayed threads of existence. They knit together pieces from before the First Home’s destruction
         * into a new world - Eodon. Eodon presented refuge from the cold desolation Akaktolan had become, so the Kotl stepped across the 
         * threshold into the new reality. When they left, they took the Great Moonstone with them, thus closing the door to the First Home
         * forever. */

        public override TextDefinition Title => 1157056;  // History of the Great Work I

        [Constructable]
        public HistoryOfTheGreatWok1()
        {
        }

        public HistoryOfTheGreatWok1(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class HistoryOfTheGreatWok2 : BaseJournal
    {
        public override TextDefinition Body => 1157061;
        /*The Founding of Kotlan and the Myrmidex<br><br>Spoken by the Memory of Katalkotl<br><br>Transcribed by Professor Ellie Rafkin<br><br>
         * While the valley of Eodon was warm and filled with bountiful life and food, it was still untamed and wild. The Kotl wished to r
         * ebuild their civilization, so they bored into the earth to tap veins of iron and gold. These tunnels became the oldest parts of 
         * their city, running for miles under Eodon.  To help with the work, they constructed huge golden golems. Powered by the Great 
         * Moonstone, the Kotl automatons were tireless workers. Soon, the city of Kotlan was a shining marvel at the heart of Eodon. After
         * the city was completed, the Kotl settled into a life made easier by the magic of the moonstone. However, they wished to be free 
         * of all toil. They could never construct or empower enough automatons to take care of every task in the city, so they searched for
         * new servants. Finding no intelligent creatures in the world, they decided to create a servitor race. Using the Nowlotl crystal to
         * tap into the moonstone’s power and select distant future variations, the Kotl bred such servants from ants. The Myrmidex were this
         * failed experiment. The Kotl thought that the Myrmidex might inherit the industrious nature of the ants, but instead, they gained 
         * only the ants’ warlike nature. The Myrmidex broke free of their captivity and fled the city of the Kotl. Thus, the World-Builders
         * were forced to look elsewhere for their labor.*/

        public override TextDefinition Title => 1157057;  // History of the Great Work II

        [Constructable]
        public HistoryOfTheGreatWok2()
        {
        }

        public HistoryOfTheGreatWok2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class HistoryOfTheGreatWok3 : BaseJournal
    {
        public override TextDefinition Body => 1157062;
        /*The Worldwalker and Humanity<br><br>Spoken by the Memory of Katalkotl<br><br>Transcribed by Professor Ellie Rafkin<br><br>After 
         * the failure of the Myrmidex, the Kotl took more care in their manipulation of living things. They turned the power of the
         * moonstone outward beyond Eodon. They constructed the Moon Chamber as a focal point for all of Eodon’s mana. The artificial origin 
         * of Eodon made travel across the void between worlds difficult. However, with the awesome power of the Great Moonstone, it was
         * possible. The Kotl sent their bravest general out across the Ethereal Void: Katalkotl the Worldwalker. He and his warriors traveled
         * to dozens of worlds, bringing treasures and curiosities back to Eodon. Eventually, Katalkotl found several worlds with an 
         * intelligent, but less civilized, species called humans. With unrivalled magic and martial power, Katalkotl’s forces brought back
         * humans to be servants to the Kotl. Because of the mercurial connection between Eodon and the outer worlds, the humans were from
         * many different times and places. Since the Kotl language could not be spoken by the human voice box (and so the Kotl could keep 
         * the secrets of their power), the Kotl created a single language from various human tongues to communicate with their new servants.
         * Out of awe and fear, most of the humans revered and served the World-Builders without complaint. Those of exceptional courage and 
         * honor even served as defenders of Kotlan. Still some humans rebelled against servitude. Many managed to flee the city of the Kotl. 
         * Some tried to resist openly and directly, only to be stopped by the automatons. A few succeeded in undermining the rulers of Eodon 
         * in secret.*/

        public override TextDefinition Title => 1157058;  // History of the Great Work III

        [Constructable]
        public HistoryOfTheGreatWok3()
        {
        }

        public HistoryOfTheGreatWok3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class HistoryOfTheGreatWok4 : BaseJournal
    {
        public override TextDefinition Body => 1157063;
        /*The Fall of the World-Builders<br><br>Spoken by the Memory of Katalkotl<br><br>Transcribed by Professor Ellie Rafkin<br><br>Initially,
         * the Kotl viewed the escaped Myrmidex as a minor nuisance, like any other beast of Eodon. The Myrmidex should have been no threat to
         * the Kotl, who protected their settlements with energy fields powered by the Great Moonstone. Still, the Myrmidex were persistent in 
         * their aggression. Eventually, the Kotl began planning to exterminate the ant hybrids. The Kotl attempted to grow a new version of
         * the Myrmidex, hoping to breed passivity into the species. They planned to send empowered humans directly against the insects. The 
         * first of these was called Zipactriotl. While the weapon initially killed a countless number of the insects, the human mind inside 
         * grew dangerously unstable. Still, it was believed that beyond a few scattered insects in the wilds of Eodon, the species was extinct.
         * Unfortunately, the Myrmidex queen had simply gone into hiding. In a subterranean hive, she produced a million eggs that hatched into
         * an army. The intelligent insects had studied the Kotl and knew the source of their power. Mysteriously, the Myrmidex raided inside 
         * Kotlan and stole the Great Moonstone. If this initial strike disarmed the Kotl, the Myrmidex then proceeded to break the back of Kotl
         * civilization. The surface portions of Kotlan were destroyed, so the World-Builders barricaded themselves in the city’s tunnels. 
         * Expecting to withstand the Myrmidex for months, the Kotl found themselves facing the Myrmidex in days. The insects destroyed 
         * everything in their path: every crystal, every relic, and any trace of Kotl magic. The Myrmidex killed everyone in the city, Kotl
         * and human. Then the Myrmidex left, and the city closed up behind them.*/

        public override TextDefinition Title => 1157059;  // History of the Great Work IV

        [Constructable]
        public HistoryOfTheGreatWok4()
        {
        }

        public HistoryOfTheGreatWok4(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}