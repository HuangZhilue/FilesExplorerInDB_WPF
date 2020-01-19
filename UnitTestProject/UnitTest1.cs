using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Command;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Manager.Implements;
using FilesExplorerInDB_Manager.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            for (int i = 0; i < 100; i++)
            {
                Files f1 = new Files();
            }

            Debug.WriteLine("F1 :" + s.ElapsedTicks);
            s.Restart();

            for (int i = 0; i < 100; i++)
            {
                Files f2 = UnityContainerHelp.GetServer<Files>();
            }

            Debug.WriteLine("F2 :" + s.ElapsedTicks);
            s.Restart();

            for (int i = 0; i < 100; i++)
            {
                Files f3 = (Files) Activator.CreateInstance(typeof(Files));
            }

            Debug.WriteLine("F3 :" + s.ElapsedTicks);
            s.Stop();
        }

        [TestMethod]
        public void TestMethod2()
        {
            //IFilesDbManager filesDbManager  = UnityContainerHelp.GetServer<IFilesDbManager>();
            //Files f1 = filesDbManager.FilesFind("5b0961b7-19fb-4ca3-8c36-9af9d3975f26");
            ////f1.FileId = Guid.NewGuid().ToString();
            //filesDbManager.FilesAdd(f1,true,true);

        }
    }
}