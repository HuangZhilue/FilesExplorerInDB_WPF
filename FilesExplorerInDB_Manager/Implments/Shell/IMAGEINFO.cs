using System;
using System.Runtime.InteropServices;

namespace FilesExplorerInDB_Manager.Implments.Shell
{
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGEINFO
    {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public int Unused1;
        public int Unused2;
        public RECT rcImage;
    }
}
