namespace Server.Items
{
    public class BookContent
    {
        private readonly string m_Title;
        private readonly string m_Author;
        private readonly BookPageInfo[] m_Pages;
        public BookContent(string title, string author, params BookPageInfo[] pages)
        {
            m_Title = title;
            m_Author = author;
            m_Pages = pages;
        }

        public string Title => m_Title;
        public string Author => m_Author;
        public BookPageInfo[] Pages => m_Pages;
        public BookPageInfo[] Copy()
        {
            BookPageInfo[] copy = new BookPageInfo[m_Pages.Length];

            for (int i = 0; i < copy.Length; ++i)
                copy[i] = new BookPageInfo(m_Pages[i].Lines);

            return copy;
        }

        public bool IsMatch(BookPageInfo[] cmp)
        {
            if (cmp.Length != m_Pages.Length)
                return false;

            for (int i = 0; i < cmp.Length; ++i)
            {
                string[] a = m_Pages[i].Lines;
                string[] b = cmp[i].Lines;

                if (a.Length != b.Length)
                {
                    return false;
                }
                else if (a != b)
                {
                    for (int j = 0; j < a.Length; ++j)
                    {
                        if (a[j] != b[j])
                            return false;
                    }
                }
            }

            return true;
        }
    }
}