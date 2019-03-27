using System.Collections.Generic;

namespace LirycAnalise
{
    class Song
    {
        public string title = "";
        public string artist = "";
        public int lines = 0;
        public int verse = 0;
        public int word = 0;
        public Dictionary<string, int> lyrics = new Dictionary<string, int>();
        public Song()
        {

        }
    }
}
