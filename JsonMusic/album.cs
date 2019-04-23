using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonMusic
{
    class album
    {
        /*
         * "id": 1187,
					"mid": "004AnyL73LCOJP",
					"name": "哈林天堂",
					"subtitle": "《背叛爱情》韩剧片头曲",
					"title": "哈林天堂",
					"title_hilight": "哈林天堂"
         */
        
        public string mid { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public string subtitle { get; set; }
        public string title { get; set; }
        public string title_hilight { get; set; }

    }
}
