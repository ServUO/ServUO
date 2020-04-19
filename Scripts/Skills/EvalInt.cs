using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;

namespace Server.SkillHandlers
{
    public class EvalInt
    {
        public static void Initialize()
        {
            SkillInfo.Table[16].Callback = OnUse;
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.Target = new InternalTarget();

            m.SendLocalizedMessage(500906); // What do you wish to evaluate?

            return TimeSpan.FromSeconds(1.0);
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(8, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == targeted)
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500910); // Hmm, that person looks really silly.
                }
                else if (targeted is TownCrier)
                {
                    ((TownCrier)targeted).PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500907, from.NetState); // He looks smart enough to remember the news.  Ask him about it.
                }
                else if (targeted is BaseVendor && ((BaseVendor)targeted).IsInvulnerable)
                {
                    ((BaseVendor)targeted).PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500909, from.NetState); // That person could probably calculate the cost of what you buy from them.
                }
                else if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;

                    int marginOfError = Math.Max(0, 20 - (int)(from.Skills[SkillName.EvalInt].Value / 5));

                    int intel = targ.Int + Utility.RandomMinMax(-marginOfError, +marginOfError);
                    int mana = ((targ.Mana * 100) / Math.Max(targ.ManaMax, 1)) + Utility.RandomMinMax(-marginOfError, +marginOfError);

                    int intMod = intel / 10;
                    int mnMod = mana / 10;

                    if (intMod > 10)
                        intMod = 10;
                    else if (intMod < 0)
                        intMod = 0;

                    if (mnMod > 10)
                        mnMod = 10;
                    else if (mnMod < 0)
                        mnMod = 0;

                    int body;

                    if (targ.Body.IsHuman)
                        body = targ.Female ? 11 : 0;
                    else
                        body = 22;

                    if (from.CheckTargetSkill(SkillName.EvalInt, targ, 0.0, 120.0))
                    {
                        targ.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1038169 + intMod + body, from.NetState); // He/She/It looks [slighly less intelligent than a rock.]  [Of Average intellect] [etc...]

                        if (from.Skills[SkillName.EvalInt].Base >= 76.0)
                            targ.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1038202 + mnMod, from.NetState); // That being is at [10,20,...] percent mental strength.
                    }
                    else
                    {
                        targ.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1038166 + (body / 11), from.NetState); // You cannot judge his/her/its mental abilities.
                    }
                }
                else if (targeted is Item)
                {
                    ((Item)targeted).SendLocalizedMessageTo(from, 500908, ""); // It looks smarter than a rock, but dumber than a piece of wood.
                }
            }
        }
    }
}
