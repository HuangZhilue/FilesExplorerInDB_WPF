using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using Sikiro.Nosql.Mongo;

namespace FilesExplorerInDB_MongoDb.Implments
{
    public class FilesDbMongoDbService : IFilesDbService
    {
        private readonly MongoRepository _mongoRepository = new MongoRepository(ConfigurationManager.ConnectionStrings["FilesDB_MongoDB"].ConnectionString);

        public Files FilesAdd(Files entity)
        {
            entity.FileId = _mongoRepository.Count<Files>(f => f.FileId != -1) + 1;
            return _mongoRepository.Add(entity) ? entity : null;
        }

        public Folders FoldersAdd(Folders entity)
        {
            entity.FolderId = _mongoRepository.Count<Folders>(f => f.FolderId != -1) + 1;
            return _mongoRepository.Add(entity) ? entity : null;
        }

        public Files FilesFind(params object[] keyValue)
        {
            return _mongoRepository.Get<Files>(f => f.FileId == Convert.ToInt32(keyValue[0]));
        }

        public Folders FoldersFind(params object[] keyValue)
        {
            return _mongoRepository.Get<Folders>(f => f.FolderId == Convert.ToInt32(keyValue[0]));
        }

        public void FilesModified(Files entity)
        {
            _mongoRepository.Update(entity);
        }

        public void FoldersModified(Folders entity)
        {
            _mongoRepository.Update(entity);
        }

        public void FilesRemove(Files entity)
        {
            _mongoRepository.Delete<Files>(f => f.FileId == entity.FileId);
        }

        public void FoldersRemove(Folders entity)
        {
            _mongoRepository.Delete<Folders>(f => f.FolderId == entity.FolderId);
        }

        public IQueryable<Files> LoadFilesEntites(Expression<Func<Files, bool>> where)
        {
            return _mongoRepository.ToList(where).AsQueryable();
        }

        public IQueryable<Folders> LoadFoldersEntites(Expression<Func<Folders, bool>> where)
        {
            //_mongoRepository = new MongoRepository("mongodb://127.0.0.1:27017");

            //var folder2 = new Folders
            //{
            //    CreationTime = DateTime.Now,
            //    FileIncludeCount = 0,
            //    FolderId = 1,
            //    FolderIncludeCount = 0,
            //    FolderLocalId = 0,
            //    FolderName = "Name",
            //    IsDelete = false,
            //    ModifyTime = DateTime.Now,
            //    Size = 0
            //};
            ////_mongoRepository.Add(folder2);



            //var b = _mongoRepository.ToList<Folders2>(ff => ff.FolderId != -1);


            //var a = _mongoRepository.ToList<Folders>(folder => folder.FolderId != -1);
            //var l = _mongoRepository.GetCollection<Folders>();
            //var l2 = l.AsQueryable();
            //var l3 = l2.Where(where);
            //var l4 = l3.ToList();


            //List<Folders> f = _mongoRepository.ToList(where);
            return _mongoRepository.ToList(where).AsQueryable();
        }

        public int SaveChanges()
        {
            return 1; 
        }
    }

    //[Mongo("FilesExplorerDB", "Folders")]
    //public class Folders2 : MongoEntity
    //{
    //    public int FolderId { get; set; }

    //    public string FolderName { get; set; }

    //    public int FolderLocalId { get; set; }

    //    public long Size { get; set; }

    //    public DateTime CreationTime { get; set; }

    //    public DateTime ModifyTime { get; set; }

    //    public int FolderIncludeCount { get; set; }

    //    public int FileIncludeCount { get; set; }

    //    public bool IsDelete { get; set; }
    //}
}
