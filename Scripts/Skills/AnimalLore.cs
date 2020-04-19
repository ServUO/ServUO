using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using System;

namespace Server.SkillHandlers
{
    public class AnimalLore
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.AnimalLore].Callback = OnUse;
        }

        public static TimeSpan OnUse(Mobile m)
        {
            if (m.HasGump(typeof(NewAnimalLoreGump)))
            {
                m.SendLocalizedMessage(500118); // You must wait a few moments to use another skill.
            }
            else
            {
                m.Target = new InternalTarget();
                m.SendLocalizedMessage(500328); // What animal should I look at?
            }

            return TimeSpan.FromSeconds(1.0);
        }

        private class InternalTarget : Target
        {
            private static void SendGump(Mobile from, BaseCreature c)
            {
                from.CheckTargetSkill(SkillName.AnimalLore, c, 0.0, 120.0);

                if (from is PlayerMobile)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                        {
                            BaseGump.SendGump(new NewAnimalLoreGump((PlayerMobile)from, c));
                        });
                }
            }

            private static void Check(Mobile from, BaseCreature c, double min)
            {
                if (from.CheckTargetSkill(SkillName.AnimalLore, c, min, 120.0))
                    SendGump(from, c);
                else
                    from.SendLocalizedMessage(500334); // You can't think of anything you know offhand.
            }

            public InternalTarget()
                : base(8, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!from.Alive)
                {
                    from.SendLocalizedMessage(500331); // The spirits of the dead are not the province of animal lore.
                }
                else if (targeted is BaseCreature)
                {
                    BaseCreature c = (BaseCreature)targeted;

                    if (!c.IsDeadPet)
                    {
                        if (c.Body.IsAnimal || c.Body.IsMonster || c.Body.IsSea)
                        {
                            double skill = from.Skills[SkillName.AnimalLore].Value;
                            if (skill < 100.0)
                            {
                                if (c.Controlled)
                                    SendGump(from, c);
                                else
                                    from.SendLocalizedMessage(1049674); // At your skill level, you can only lore tamed creatures.
                            }
                            else if (skill < 110.0)
                            {
                                if (c.Controlled)
                                    SendGump(from, c);
                                else if (c.Tamable)
                                    Check(from, c, 80.0);
                                else
                                    from.SendLocalizedMessage(1049675); // At your skill level, you can only lore tamed or tameable creatures.
                            }
                            else
                            {
                                if (c.Controlled)
                                    SendGump(from, c);
                                else if (c.Tamable)
                                    Check(from, c, 80.0);
                                else
                                    Check(from, c, 100.0);
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(500329); // That's not an animal!
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(500331); // The spirits of the dead are not the province of animal lore.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(500329); // That's not an animal!
                }
            }
        }
    }
}
