namespace Server.Items
{
    public class SpellFocusingSash : BodySash
    {
        public override int LabelNumber { get { return 1150059; } } // Spell Focusing Sash

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override double DefaultWeight { get { return 1; } }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Mobile SpellCastTarget { get; set; }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public int SpellCastCount { get; set; }

        [CommandProperty(AccessLevel.Counselor, true)]
        public int SpellDamageOffset
        {
            get
            {
                if (SpellCastCount <= 5)
                {
                    return -(SpellCastCount * 6);
                }

                return (SpellCastCount - 6) * 2;
            }
        }

        [Constructable]
        public SpellFocusingSash()
            : this(0)
        {
        }

        [Constructable]
        public SpellFocusingSash(int hue)
        {
            Hue = hue;

            Attributes.BonusMana = 1;
            Attributes.DefendChance = 5;

            NegativeAttributes.Brittle = 1;
        }

        public SpellFocusingSash(Serial serial)
            : base(serial)
        {
        }

        public override void AddWeightProperty(ObjectPropertyList list)
        {
            base.AddWeightProperty(list);

            list.Add(1150058); // Spell Focusing
        }

        private void RefreshBuffInfo(Mobile caster)
        {
            var offset = SpellDamageOffset;

            if (offset < 0)
            {
                BuffInfo.RemoveBuff(caster, BuffIcon.SpellFocusingBuff);

                if (caster == Parent)
                {
                    // Spell Focusing
                    // Target: ~1_val~<br>Damage Modifier: ~2_val~%
                    BuffInfo.AddBuff(caster, new BuffInfo(BuffIcon.SpellFocusingDebuff, 1151391, 1151392, $"{SpellCastTarget?.Name ?? "None"}\t{offset}"));
                }
            }
            else
            {
                BuffInfo.RemoveBuff(caster, BuffIcon.SpellFocusingDebuff);

                if (caster == Parent)
                {
                    // Spell Focusing
                    // Target: ~1_val~<br>Damage Modifier: ~2_val~%
                    BuffInfo.AddBuff(caster, new BuffInfo(BuffIcon.SpellFocusingBuff, 1151391, 1151392, $"{SpellCastTarget?.Name ?? "None"}\t{offset}"));
                }
            }
        }

        public void ValidateTarget(Mobile caster, Mobile target, out int sdiOffset)
        {
            sdiOffset = 0;

            try
            {
                if (caster == null || caster.Deleted || !caster.Alive || caster != Parent)
                {
                    SpellCastTarget = null;
                    SpellCastCount = 0;

                    return;
                }

                if (SpellCastTarget != target)
                {
                    SpellCastTarget = target;
                    SpellCastCount = 0;

                    caster.SendLocalizedMessage(1150117); // Spell focusing damage has reset.
                }

                if (SpellCastTarget == null)
                {
                    SpellCastTarget = null;
                    SpellCastCount = 0;

                    return;
                }

                if (!SpellCastTarget.Alive || SpellCastTarget.Deleted)
                {
                    SpellCastTarget = null;
                    SpellCastCount = 0;

                    caster.SendLocalizedMessage(1150117); // Spell focusing damage has reset.

                    return;
                }

                // offset range is -30 to +30
                // first 5 casts are multiples of -6
                // 6th cast is negated
                // every cast after is +2

                if (++SpellCastCount <= 21)
                {
                    sdiOffset = SpellDamageOffset;

                    if (sdiOffset == 0)
                    {
                        caster.SendLocalizedMessage(1150118); // Spell focusing damage has now been tuned to your opponent.
                    }
                }

                if (SpellCastCount >= 21)
                {
                    SpellCastTarget = null;
                    SpellCastCount = 0;

                    caster.SendLocalizedMessage(1150116); // Spell focusing damage has peaked.
                }
            }
            finally
            {
                RefreshBuffInfo(caster);
            }
        }

        protected override void OnParentChanged(object oldParent)
        {
            base.OnParentChanged(oldParent);

            SpellCastTarget = null;
            SpellCastCount = 0;

            RefreshBuffInfo(oldParent as Mobile);
            RefreshBuffInfo(Parent as Mobile);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadEncodedInt();
        }
    }
}
