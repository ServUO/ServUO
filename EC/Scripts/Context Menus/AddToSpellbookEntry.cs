using System;
using Server.Items;
using Server.Targeting;

namespace Server.ContextMenus
{
    public class AddToSpellbookEntry : ContextMenuEntry
    {
        public AddToSpellbookEntry()
            : base(6144, 3)
        {
        }

        public override void OnClick()
        {
            if (this.Owner.From.CheckAlive() && this.Owner.Target is SpellScroll)
                this.Owner.From.Target = new InternalTarget((SpellScroll)this.Owner.Target);
        }

        private class InternalTarget : Target
        {
            private readonly SpellScroll m_Scroll;
            public InternalTarget(SpellScroll scroll)
                : base(3, false, TargetFlags.None)
            {
                this.m_Scroll = scroll;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Spellbook)
                {
                    if (from.CheckAlive() && !this.m_Scroll.Deleted && this.m_Scroll.Movable && this.m_Scroll.Amount >= 1 && this.m_Scroll.CheckItemUse(from))
                    {
                        Spellbook book = (Spellbook)targeted;

                        SpellbookType type = Spellbook.GetTypeForSpell(this.m_Scroll.SpellID);

                        if (type != book.SpellbookType)
                        {
                        }
                        else if (book.HasSpell(this.m_Scroll.SpellID))
                        {
                            from.SendLocalizedMessage(500179); // That spell is already present in that spellbook.
                        }
                        else
                        {
                            int val = this.m_Scroll.SpellID - book.BookOffset;

                            if (val >= 0 && val < book.BookCount)
                            {
                                book.Content |= (ulong)1 << val;

                                this.m_Scroll.Consume();

                                from.Send(new Network.PlaySound(0x249, book.GetWorldLocation()));
                            }
                        }
                    }
                }
            }
        }
    }
}