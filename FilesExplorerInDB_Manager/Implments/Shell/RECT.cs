using System.Runtime.InteropServices;

namespace FilesExplorerInDB_Manager.Implments.Shell
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
