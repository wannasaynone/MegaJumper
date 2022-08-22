using KahaGameCore.Static;
using ProjectBS.Data;

namespace ProjectBS
{
    public class ContextConverter 
    {
        public static ContextConverter Instance 
        {
            get
            {
                if(m_instance == null)
                {
                    m_instance = new ContextConverter();
                }
                return m_instance;
            }
        }
        private static ContextConverter m_instance = null;

        public enum Area
        {
            zh_tw,
            en_us,
            ja_jp
        }

        public Area area = Area.zh_tw;

        private ContextConverter() { }

        public string GetContext(int ID)
        {
            ContextData _context = GameDataManager.GetGameData<ContextData>(ID);
            if(_context == null)
            {
                throw new System.Exception("[ContextConverter][GetContext] Can't find context id=" + ID);
            }

            switch (area)
            {
                case Area.zh_tw:
                    {
                        if (string.IsNullOrEmpty(_context.zh_tw)) return "[MissingTranslation<zh_tw>" + ID + "]";
                        return _context.zh_tw;
                    }
                case Area.en_us:
                    {
                        if (string.IsNullOrEmpty(_context.en_us)) return "[MissingTranslation<en_us>" + ID + "]";
                        return _context.en_us;
                    }
                case Area.ja_jp:
                    {
                        if (string.IsNullOrEmpty(_context.ja_jp)) return "[MissingTranslation<ja_jp>" + ID + "]";
                        return _context.ja_jp;
                    }
                default:
                    {
                        throw new System.Exception("[ContextConverter][GetContext] Invaild Area=" + area.ToString());
                    }
            }
        }
    }
}
