using System;
using Server.Items;

namespace Server.Engines.Craft
{
    public class DefBasketweaving : CraftSystem
    {
        private static CraftSystem m_CraftSystem;
        private DefBasketweaving()
            : base(1, 1, 1.25)// base( 1, 2, 1.7 )
        {
        }

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefBasketweaving();

                return m_CraftSystem;
            }
        }
        public override SkillName MainSkill
        {
            get
            {
                return SkillName.Tinkering;
            }
        }
        public override string GumpTitleString
        {
            get
            {
                return "<basefont color=#FFFFFF><CENTER>BASKET WEAVING MENU</CENTER></basefont>";
            }
        }
        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x1C6); 
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038);

            if (failed)
            {
                if (lostMaterial)
                    return 1044043;
                else
                    return 1044157;
            }
            else
            {
                from.PlaySound(0x1c6);

                if (quality == 0)
                    return 502785;
                else if (makersMark && quality == 2)
                    return 1044156;
                else if (quality == 2)
                    return 1044155;
                else
                    return 1044154;
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            index = this.AddCraft(typeof(RoundBasket), 1112335, 1112293, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 3, 1112251);
            this.AddSkill(index, SkillName.Carpentry, 75.0, 80.0);
            this.AddRes(index, typeof(Shaft), 1112247, 2, 1112246);
            //SetUseSubRes2(index, true);

            index = this.AddCraft(typeof(RoundBasketHandles), 1112335, "Round basket w/handles", 75.0, 100.0, typeof(SoftenedReeds), 1112249, 3, 1112251);
            this.AddSkill(index, SkillName.Carpentry, 75.0, 80.0);
            this.AddRes(index, typeof(Shaft), 1112247, 2, 1112246);
            //SetUseSubRes2(index, true);

            index = this.AddCraft(typeof(SmallBushel), 1112335, 1112337, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 2, 1112251);
            this.AddSkill(index, SkillName.Carpentry, 75.0, 80.0);
            this.AddRes(index, typeof(Shaft), 1112247, 1, 1112246);
            // SetUseSubRes2(index, true);

            index = this.AddCraft(typeof(PicnicBasket), 1112335, 1112356, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 2, 1112251);
            this.AddSkill(index, SkillName.Carpentry, 75.0, 80.0);
            this.AddRes(index, typeof(Shaft), 1112247, 1, 1112246);
            //SetUseSubRes2(index, true);

            index = this.AddCraft(typeof(WinnowingBasket), 1112335, 1112355, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 5, 1112251);
            this.AddSkill(index, SkillName.Carpentry, 75.0, 80.0);
            this.AddRes(index, typeof(Shaft), 1112247, 4, 1112246);
            // SetUseSubRes2(index, true);
            
            index = this.AddCraft(typeof(SquareBasket), 1112335, 1112295, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 3, 1112251);
            this.AddSkill(index, SkillName.Carpentry, 75.0, 80.0);
            this.AddRes(index, typeof(Shaft), 1112247, 2, 1112246);
            //SetUseSubRes2(index, true);
            
            index = this.AddCraft(typeof(Basket), 1112335, 1112294, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 2, 1112251);
            this.AddSkill(index, SkillName.Carpentry, 75.0, 80.0);
            this.AddRes(index, typeof(Shaft), 1112247, 1, 1112246);

            index = this.AddCraft(typeof(TallRoundBasket), 1112335, 1112297, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 4, 1112251);
            this.AddSkill(index, SkillName.Carpentry, 75.0, 80.0);
            this.AddRes(index, typeof(Shaft), 1112247, 3, 1112246);

            index = this.AddCraft(typeof(SmallSquareBasket), 1112335, 1112296, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 2, 1112251);
            this.AddSkill(index, SkillName.Carpentry, 75.0, 80.0);
            this.AddRes(index, typeof(Shaft), 1112247, 1, 1112246);

            index = this.AddCraft(typeof(TallBasket), 1112335, 1112299, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 4, 1112251);
            this.AddSkill(index, SkillName.Carpentry, 75.0, 80.0);
            this.AddRes(index, typeof(Shaft), 1112247, 3, 1112246);

            index = this.AddCraft(typeof(SmallRoundBasket), 1112335, 1112298, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 2, 1112251);
            this.AddSkill(index, SkillName.Carpentry, 75.0, 80.0);
            this.AddRes(index, typeof(Shaft), 1112247, 1, 1112246);
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;
            public InternalTimer(Mobile from)
                : base(TimeSpan.FromSeconds(0.7))
            {
                this.m_From = from;
            }

            protected override void OnTick()
            {
                this.m_From.PlaySound(0x1C6);
            }
        }
    }
}