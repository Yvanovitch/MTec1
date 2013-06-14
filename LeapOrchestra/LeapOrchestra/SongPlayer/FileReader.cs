using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapOrchestra.SongPlayer
{
    abstract class FileReader
    {
        private string FilePath;

        public FileReader(string path)
        {
            FilePath = path;
        }
    }
}
