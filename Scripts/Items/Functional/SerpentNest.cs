using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Items
{
    public class SerpentNest : Item
    {
        public override int LabelNumber => 1112582;  // a serpent's nest

        [Constructable]
        public SerpentNest()
            : base(0x2233)
        {
            Hue = 0x456;
            Movable = false;
        }

        public SerpentNest(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 1))
            {
                SendLocalizedMessageTo(from, 1076766); // That is too far away.
                return;
            }

            from.RevealingAction();

            if (0.025 > Utility.RandomDouble())
            {
                from.AddToBackpack(new RareSerpentEgg());
                from.SendLocalizedMessage(1112581); // You reach in and find a rare serpent egg!!
            }
            else
            {
                switch (Utility.Random(3))
                {
                    case 0:
                        {
                            from.SendLocalizedMessage(1112578); // You try to reach the eggs, but the hole is too deep.

                            break;
                        }
                    case 1:
                        {
                            from.SendLocalizedMessage(1112579); // You reach in but clumsily destroy the eggs inside the nest.
                            Collapse(from);

                            break;
                        }
                    case 2:
                        {
                            from.SendLocalizedMessage(1112580); // Beware! You've hatched the eggs!!
                            HatchEggs(from);

                            from.PrivateOverheadMessage(MessageType.Regular, 33, 1112940, from.NetState); // Your hand remains stuck!!!
                            from.Frozen = true;

                            Timer.DelayCall(TimeSpan.FromSeconds(5.0), delegate
                            {
                                from.Frozen = false;
                                from.PrivateOverheadMessage(MessageType.Regular, 65, 1112941, from.NetState); // You manage to free your hand!
                            });

                            break;
                        }
                }
            }
        }

        public void Collapse(Mobile from)
        {
            from.SendLocalizedMessage(1112583); // The nest collapses.

            Delete();
        }

        public void HatchEggs(Mobile from)
        {
            from.SendLocalizedMessage(1112577); // A swarm of snakes springs forth from the nest and attacks you!!!

            for (int i = 0; i < 4; i++)
            {
                try
                {
                    BaseCreature snake = (BaseCreature)Activator.CreateInstance(m_SnakeTypes[Utility.Random(m_SnakeTypes.Length)]);

                    snake.RemoveOnSave = true;
                    snake.MoveToWorld(Map.GetSpawnPosition(Location, 1), Map);
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
                }
            }

            Collapse(from);
        }

        private static readonly Type[] m_SnakeTypes = new Type[]
            {
                typeof( LavaSnake ),    typeof( Snake ),
                typeof( CoralSnake ),   typeof( GiantSerpent )
            };

        public override bool OnMoveOver(Mobile m)
        {
            BaseCreature snake = m as BaseCreature;

            if (snake != null && snake.CharmMaster != null)
            {
                snake.CharmMaster.SendLocalizedMessage(1112588); // The snake begins searching for rare eggs.
                snake.Frozen = true;

                Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(1, 5)), delegate
                {
                    if (!snake.Alive)
                        return;

                    snake.Frozen = false;

                    Mobile from = snake.CharmMaster;

                    if (from == null || Deleted)
                        return;

                    if (0.1 > Utility.RandomDouble())
                    {
                        from.SendLocalizedMessage(1112586); // The snake finds a rare egg and drags it out of the nest!
                        new RareSerpentEgg().MoveToWorld(Location, Map);

                        Collapse(from);
                    }
                    else
                    {
                        switch (Utility.Random(3))
                        {
                            case 0:
                            {
                                from.SendLocalizedMessage(1112585); // Beware! The snake has hatched some of the eggs!!
                                HatchEggs(from);

                                break;
                            }
                            case 1:
                            {
                                from.SendLocalizedMessage(1112584); // The snake searches the nest and finds nothing.

                                break;
                            }
                            case 2:
                            {
                                from.SendLocalizedMessage(1112584); // The snake searches the nest and finds nothing.
                                Collapse(from);

                                break;
                            }
                        }
                    }

                    snake.EndCharm();
                });
            }

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

            /*int version = */
            reader.ReadInt();
        }
    }
}
