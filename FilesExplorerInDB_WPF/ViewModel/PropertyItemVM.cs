using FilesExplorerInDB_Models.Models;
using FilesExplorerInDB_WPF.Models;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public class PropertyItemVM : BaseViewModel
    {
        #region 字段

        #region 公共字段

        public PropertyItems PropertyItems { get; } = PropertyItems.GetInstance;

        #endregion

        #region 非公共字段



        #endregion

        #endregion

        #region 构造函数

        public static PropertyItemVM GetInstance { get; } = new PropertyItemVM();

        private PropertyItemVM()
        {

        }

        #endregion

        public void SetProperty(ExplorerProperty property)
        {
            PropertyItems.GetPropertyFromExplorerItems(property);
        }

        public void SetProperty(int nowFolderId)
        {
            PropertyItems.GetPropertyFromExplorerBlock(nowFolderId);
        }
    }
}