using System;
using Server.Mobiles;
using Server.Engines.Craft;

namespace Server.Items
{
    [FlipableAttribute(0x9DB1, 0x9DB2)]
    public class KotlAutomatonHead : Item, ICraftable
    {
        private bool _Activated;
        private CraftResource _Resource;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource { get { return _Resource; } set { _Resource = value; Hue = CraftResources.GetHue(this._Resource); InvalidateProperties(); } }

        [Constructable]
        public KotlAutomatonHead()
            : base(0x9DB1)
        {
            LootType = LootType.Blessed;

            Resource = CraftResource.Iron;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1157022, String.Format("#{0}", CraftResources.GetLocalizationNumber(_Resource))); // Rebuilt ~1_MATTYPE~ Automaton Head
        }

        public override void OnDoubleClick(Mobile from)
        {
            int skill = (int)from.Skills[SkillName.Tinkering].Base;

            if (skill < 100.0)
            {
                from.SendLocalizedMessage(1157006); // You must be a Grandmaster Tinker to activate an Automaton.
            }
            else if (_Activated)
            {
                from.SendLocalizedMessage(1157007); // The Automaton is already being activated.
            }
            else
            {
                _Activated = true;

                Timer.DelayCall(TimeSpan.FromSeconds(3), () =>
                    {
                        KotlAutomaton automaton = GetAutomaton(from);

                        if (automaton.SetControlMaster(from))
                        {
                            automaton.IsBonded = true;

                            Delete();

                            automaton.MoveToWorld(from.Location, from.Map);
                            from.PlaySound(0x23B);
                        }
                        else
                        {
                            automaton.Delete();
                        }
                    });
            }
        }

        public virtual KotlAutomaton GetAutomaton(Mobile master)
        {
            KotlAutomaton automaton = new KotlAutomaton();
            automaton.Resource = _Resource;

            return automaton;
        }

        #region ICraftable Members
        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            if (typeRes == null)
                typeRes = craftItem.Resources.GetAt(0).ItemType;

            Resource = CraftResources.GetFromType(typeRes);

            return quality;
        }
        #endregion

        public KotlAutomatonHead(Serial serial)
            : base(serial)
        {
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
