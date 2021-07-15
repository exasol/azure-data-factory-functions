using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exasol.Utilities
{
    class FilterUtilities
    {
        public static List<string> FilterFilenameListOnDirectoryPath(List<string> filenameList, String path)
        {
            if (!String.IsNullOrEmpty(path)){
                var filteredFilenames = filenameList.Where(filename => filename.StartsWith(path));
                return filteredFilenames.ToList();
            } else
            {
                return filenameList;
            }
        }
    }
}
