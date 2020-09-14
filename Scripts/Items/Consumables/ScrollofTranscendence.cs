using Server.Accounting;
using Server.Mobiles;
using System;

namespace Server.Items
{
    [TypeAlias("Server.Items.ScrollofTranscendence")]
    public class ScrollOfTranscendence : SpecialScroll, IAccountRestricted
    {
        public override int LabelNumber => 1094934;// Scroll of Transcendence

        public override int Message => 1094933;/*Using a Scroll of Transcendence for a given skill will permanently increase your current 
        *level in that skill by the amount of points displayed on the scroll.
        *As you may not gain skills beyond your maximum skill cap, any excess points will be lost.*/

        public override string DefaultTitle => string.Format("<basefont color=#FFFFFF>Scroll of Transcendence ({0} Skill):</basefont>", Value);

        public static ScrollOfTranscendence CreateRandom(int min, int max)
        {
            SkillName skill = (SkillName)Utility.Random(SkillInfo.Table.Length);

            return new ScrollOfTranscendence(skill, Utility.RandomMinMax(min, max) * 0.1);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Account { get; set; }

        public ScrollOfTranscendence()
            : this(SkillName.Alchemy, 0.0)
        {
        }

        [Constructable]
        public ScrollOfTranscendence(SkillName skill, double value)
            : base(skill, value)
        {
            ItemID = 0x14EF;
            Hue = 0x490;
        }

        public ScrollOfTranscendence(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1076759, "{0}\t{1:0.0} Skill Points", GetName(), Value);

            if (!string.IsNullOrEmpty(Account))
                list.Add(1155526); // Account Bound
        }

        public override bool CanUse(Mobile from)
        {
            if (!base.CanUse(from))
                return false;

            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return false;

            #region Scroll of Alacrity
            if (pm.AcceleratedStart > DateTime.UtcNow)
            {
                from.SendLocalizedMessage(1077951); // You are already under the effect of an accelerated skillgain scroll.
                return false;
            }
            #endregion

            if (!string.IsNullOrEmpty(Account))
            {
                Account acct = pm.Account as Account;

                if (acct == null || acct.Username != Account)
                {
                    from.SendLocalizedMessage(1151920); // This item is Account Bound, you are not permitted to take this action.
                    return false;
                }
            }

            return true;
        }

        public override void Use(Mobile from)
        {
            if (!CanUse(from))
                return;

            double tskill = from.Skills[Skill].Base; // value of skill without item bonuses etc
            double tcap = from.Skills[Skill].Cap; // maximum value permitted
            bool canGain = false;

            double newValue = Value;

            if ((tskill + newValue) > tcap)
                newValue = tcap - tskill;

            if (tskill < tcap && from.Skills[Skill].Lock == SkillLock.Up)
            {
                if ((from.SkillsTotal + newValue * 10) > from.SkillsCap)
                {
                    int ns = from.Skills.Length; // number of items in from.Skills[]

                    for (int i = 0; i < ns; i++)
                    {
                        // skill must point down and its value must be enough
                        if (from.Skills[i].Lock == SkillLock.Down && from.Skills[i].Base >= newValue)
                        {
                            from.Skills[i].Base -= newValue;
                            canGain = true;
                            break;
                        }
                    }
                }
                else
                    canGain = true;
            }

            if (!canGain)
            {
                from.SendLocalizedMessage(1094935);	/*You cannot increase this skill at this time. The skill may be locked or set to lower in your skill menu.
                *If you are at your total skill cap, you must use a Powerscroll to increase your current skill cap.*/
                return;
            }

            from.SendLocalizedMessage(1049513, GetNameLocalized()); // You feel a surge of magic as the scroll enhances your ~1_type~!

            from.Skills[Skill].Base += newValue;

            Effects.PlaySound(from.Location, from.Map, 0x1F7);
            Effects.SendTargetParticles(from, 0x373A, 35, 45, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);
            Effects.SendTargetParticles(from, 0x376A, 35, 45, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(Account);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = (InheritsItem ? 0 : reader.ReadInt()); //Required for SpecialScroll insertion

            if (version > 0)
                Account = reader.ReadString();

            LootType = LootType.Cursed;
            Insured = false;

            if (Hue == 0x7E)
                Hue = 0x490;
        }
    }
}