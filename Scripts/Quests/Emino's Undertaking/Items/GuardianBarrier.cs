using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
    public class GuardianBarrier : Item
    {
        [Constructable]
        public GuardianBarrier()
            : base(0x3967)
        {
            this.Movable = false;
            this.Visible = false; 
        }

        public GuardianBarrier(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.IsStaff())
                return true;

            // If the mobile is to the north of the barrier, allow him to pass
            if (this.Y >= m.Y)
                return true;

            if (m is BaseCreature)
            {
                Mobile master = ((BaseCreature)m).GetMaster();

                // Allow creatures to cross from the south to the north only if their master is near to the north
                if (master != null && this.Y >= master.Y && master.InRange(this, 4))
                    return true;
                else
                    return false;
            }

            PlayerMobile pm = m as PlayerMobile;

            if (pm != null)
            {
                EminosUndertakingQuest qs = pm.Quest as EminosUndertakingQuest;

                if (qs != null)
                {
                    SneakPastGuardiansObjective obj = qs.FindObjective(typeof(SneakPastGuardiansObjective)) as SneakPastGuardiansObjective;

                    if (obj != null)
                    {
                        if (m.Hidden)
                            return true; // Hidden ninjas can pass

                        if (!obj.TaughtHowToUseSkills)
                        {
                            obj.TaughtHowToUseSkills = true;
                            qs.AddConversation(new NeedToHideConversation());
                        }
                    }
                }
            }

            return false;
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
}