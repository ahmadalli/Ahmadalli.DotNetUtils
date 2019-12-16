using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ahmadalli.DotNetUtils.IO
{
    public static class FileInfoHelpers
    {
        public static bool IsFileLocked(this FileInfo file)
        {
            try
            {
                using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist
                return true;
            }

            //file is not locked
            return false;
        }
    }
}
