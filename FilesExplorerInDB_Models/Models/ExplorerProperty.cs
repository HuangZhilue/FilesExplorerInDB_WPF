using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using FilesExplorerInDB_Models.Interface;

namespace FilesExplorerInDB_Models.Models
{
    public class ExplorerProperty : IProperty
    {
        public int Id { get; set; }

        public int FolderLocalId { get; set; }

        public bool IsFolder { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public long Size { get; set; }

        public string CreationTime { get; set; }

        public string ModifyTime { get; set; }

        public string AccessTime { get; set; }

        public ImageSource ImageSource { get; set; }

        public Thickness BorderThickness { get; set; } = new Thickness(0);
        public bool IsReadOnly { get; set; } = true;
        public Brush Background { get; set; } = null;
        public Brush BorderBrush { get; set; } = null;
        public Cursor Cursor { get; set; } = Cursors.Arrow;
        public bool Focusable { get; set; } = false;
    }
}