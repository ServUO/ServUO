using Server;
using System;

namespace Server.Gumps
{
    public class BasicInfoGump : Gump
    {
        public BasicInfoGump(TextDefinition body)
            : this(body, null)
        {
        }

        public BasicInfoGump(TextDefinition body, TextDefinition title)
            : base(20, 20)
        {
            AddBackground(0, 0, 300, 450, 9200);

            if (title != null)
            {
                AddImageTiled(10, 10, 280, 20, 2702);
                AddImageTiled(10, 40, 280, 400, 2702);

                if (title.Number > 0)
                    AddHtmlLocalized(12, 10, 275, 20, title.Number, 0xFFFFFF, false, false);
                else if (title.String != null)
                    AddHtml(12, 10, 275, 20, String.Format("<BASEFONT COLOR=WHITE>{0}</BASEFONT>", title.String), false, false);

                if (body.Number > 0)
                    AddHtmlLocalized(12, 40, 275, 390, body.Number, 0xFFFFFF, false, false);
                else if (body.String != null)
                    AddHtml(12, 40, 275, 390, String.Format("<BASEFONT COLOR=WHITE>{0}</BASEFONT>", body.String), false, false);
            }
            else
            {
                AddImageTiled(10, 10, 280, 430, 2702);

                if (body.Number > 0)
                    AddHtmlLocalized(12, 10, 275, 425, (int)body, 0xFFFFFF, false, false);
                else if (body.String != null)
                    AddHtml(12, 10, 275, 425, String.Format("<BASEFONT COLOR=WHITE>{0}</BASEFONT>", body.String), false, false);
            }
        }
    }
}
