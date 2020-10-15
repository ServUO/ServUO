using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Spells.Mysticism
{
    public class SpellTriggerSpell : MysticSpell
    {
        public override SpellCircle Circle => SpellCircle.Fifth;

        private static readonly SpellInfo m_Info = new SpellInfo(
                "Spell Trigger", "In Vas Ort Ex ",
                230,
                9022,
                Reagent.Garlic,
                Reagent.MandrakeRoot,
                Reagent.SpidersSilk,
                Reagent.DragonBlood
            );

        public SpellTriggerSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            if (Caster.HasGump(typeof(SpellTriggerGump)))
                Caster.CloseGump(typeof(SpellTriggerGump));

            SpellTriggerGump gump = new SpellTriggerGump(this, Caster);
            int serial = gump.Serial;

            Caster.SendGump(gump);

            Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                {
                    Gump current = Caster.FindGump(typeof(SpellTriggerGump));

                    if (current != null && current.Serial == serial)
                    {
                        Caster.CloseGump(typeof(EnchantSpellGump));
                        FinishSequence();
                    }
                });
        }

        private class SpellTriggerGump : Gump
        {
            private readonly Spell m_Spell;
            private readonly int m_Skill;

            public SpellTriggerGump(Spell spell, Mobile m)
                : base(60, 36)
            {
                m_Spell = spell;

                AddPage(0);

                AddBackground(0, 0, 520, 404, 0x13BE);

                AddImageTiled(10, 10, 500, 20, 0xA40);
                AddImageTiled(10, 40, 500, 324, 0xA40);
                AddImageTiled(10, 374, 500, 20, 0xA40);
                AddAlphaRegion(10, 10, 500, 384);

                AddButton(10, 374, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 376, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL

                AddHtmlLocalized(14, 12, 500, 20, 1080151, 0x7FFF, false, false); // <center>Spell Trigger Selection Menu</center>

                AddPage(1);

                m_Skill = (int)(GetBaseSkill(m) + GetBoostSkill(m));
                int idx = 0;

                for (int i = 0; i < m_Definitions.Length; i++)
                {
                    SpellTriggerDef entry = m_Definitions[i];

                    if (m_Skill >= (entry.Rank * 40))
                    {
                        idx++;

                        if (idx == 11)
                        {
                            AddButton(400, 374, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 2);
                            AddHtmlLocalized(440, 376, 60, 20, 1043353, 0x7FFF, false, false); // Next

                            AddPage(2);

                            AddButton(300, 374, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1);
                            AddHtmlLocalized(340, 376, 60, 20, 1011393, 0x7FFF, false, false); // Back

                            idx = 1;
                        }

                        if ((idx % 2) != 0)
                        {
                            AddImageTiledButton(14, 44 + (64 * (idx - 1) / 2), 0x918, 0x919, 100 + i, GumpButtonType.Reply, 0, entry.ItemId, 0, 15, 20);
                            AddTooltip(entry.Tooltip);
                            AddHtmlLocalized(98, 44 + (64 * (idx - 1) / 2), 170, 60, entry.Cliloc, 0x7FFF, false, false);
                        }
                        else
                        {
                            AddImageTiledButton(264, 44 + (64 * (idx - 2) / 2), 0x918, 0x919, 100 + i, GumpButtonType.Reply, 0, entry.ItemId, 0, 15, 20);
                            AddTooltip(entry.Tooltip);
                            AddHtmlLocalized(348, 44 + (64 * (idx - 2) / 2), 170, 60, entry.Cliloc, 0x7FFF, false, false);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (from.Backpack != null && info.ButtonID >= 100 && info.ButtonID <= 110 && m_Spell.CheckSequence())
                {
                    Item[] stones = from.Backpack.FindItemsByType(typeof(SpellStone));

                    for (int i = 0; i < stones.Length; i++)
                        stones[i].Delete();

                    SpellTriggerDef entry = m_Definitions[info.ButtonID - 100];

                    if (m_Skill >= (entry.Rank * 40))
                    {
                        from.PlaySound(0x659);
                        from.PlaceInBackpack(new SpellStone(entry));

                        from.SendLocalizedMessage(1080165); // A Spell Stone appears in your backpack
                    }
                }

                m_Spell.FinishSequence();
            }
        }

        private static readonly SpellTriggerDef[] m_Definitions = new SpellTriggerDef[]
            {
                new SpellTriggerDef( 677, "Nether Bolt",        1, 1031678, 1095193, 0x2D9E ),
                new SpellTriggerDef( 678, "Healing Stone",      1, 1031679, 1095194, 0x2D9F ),
                new SpellTriggerDef( 679, "Purge Magic",        2, 1031680, 1095195, 0x2DA0 ),
                new SpellTriggerDef( 680, "Enchant",            2, 1031681, 1095196, 0x2DA1 ),
                new SpellTriggerDef( 681, "Sleep",              3, 1031682, 1095197, 0x2DA2 ),
                new SpellTriggerDef( 682, "Eagle Strike",       3, 1031683, 1095198, 0x2DA3 ),
                new SpellTriggerDef( 683, "Animated Weapon",    4, 1031684, 1095199, 0x2DA4 ),
                new SpellTriggerDef( 684, "Stone Form",         4, 1031685, 1095200, 0x2DA5 ),
                new SpellTriggerDef( 686, "Mass Sleep",         5, 1031687, 1095202, 0x2DA7 ),
                new SpellTriggerDef( 687, "Cleansing Winds",    6, 1031688, 1095203, 0x2DA8 ),
                new SpellTriggerDef( 688, "Bombard",            6, 1031689, 1095204, 0x2DA9 )
            };

        public static SpellTriggerDef[] Definitions => m_Definitions;
    }

    public class SpellTriggerDef
    {
        private readonly int m_SpellId;
        private readonly string m_Name;
        private readonly int m_Rank;
        private readonly int m_Cliloc;
        private readonly int m_Tooltip;
        private readonly int m_ItemId;

        public int SpellId => m_SpellId;
        public string Name => m_Name;
        public int Rank => m_Rank;
        public int Cliloc => m_Cliloc;
        public int Tooltip => m_Tooltip;
        public int ItemId => m_ItemId;

        public SpellTriggerDef(int spellId, string name, int rank, int cliloc, int tooltip, int itemId)
        {
            m_SpellId = spellId;
            m_Name = name;
            m_Rank = rank;
            m_Cliloc = cliloc;
            m_Tooltip = tooltip;
            m_ItemId = itemId;
        }
    }

    public class SpellStone : SpellScroll
    {
        private SpellTriggerDef m_SpellDef;

        public override bool Nontransferable => true;

        [Constructable]
        public SpellStone(SpellTriggerDef spellDef)
            : base(spellDef.SpellId, 0x4079, 1)
        {
            m_SpellDef = spellDef;
            LootType = LootType.Blessed;
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            Delete();
            return false;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            return false;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
        }

        private static readonly Dictionary<Mobile, DateTime> m_CooldownTable = new Dictionary<Mobile, DateTime>();

        public override void OnDoubleClick(Mobile from)
        {
            if (m_CooldownTable.ContainsKey(from))
            {
                DateTime next = m_CooldownTable[from];
                int seconds = (int)(next - DateTime.UtcNow).TotalSeconds + 1;

                // You must wait ~1_seconds~ seconds before you can use this item.
                from.SendLocalizedMessage(1079263, seconds.ToString());

                return;
            }

            base.OnDoubleClick(from);
        }

        public void Use(Mobile from)
        {
            m_CooldownTable[from] = DateTime.UtcNow + TimeSpan.FromSeconds(300.0);
            Timer.DelayCall(TimeSpan.FromSeconds(300.0), delegate
            {
                m_CooldownTable.Remove(from);
            });

            Delete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1080166, m_SpellDef.Name); // Use: ~1_spellName~
        }

        public SpellStone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);

            writer.Write(m_SpellDef.SpellId);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        int spellId = reader.ReadInt();

                        for (int i = 0; i < SpellTriggerSpell.Definitions.Length; i++)
                        {
                            SpellTriggerDef def = SpellTriggerSpell.Definitions[i];

                            if (def.SpellId == spellId)
                            {
                                m_SpellDef = def;
                                break;
                            }
                        }

                        if (m_SpellDef == null)
                            Delete();

                        break;
                    }
                case 0:
                    {
                        Delete();
                        break;
                    }
            }
        }
    }
}
