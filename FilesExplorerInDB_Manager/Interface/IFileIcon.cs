using System.Drawing;
using FilesExplorerInDB_Manager.Implements;

namespace FilesExplorerInDB_Manager.Interface
{
    public interface IFileIcon
    {
        Bitmap GetBitmapFromFolderPath(string filePath, FileIcon.IconSizeEnum iconSize);

        Bitmap GetBitmapFromFilePath(string filePath, FileIcon.IconSizeEnum iconSize);

        Bitmap GetBitmapFromPath(string filePath, FileIcon.IconSizeEnum iconSize);
    }
}