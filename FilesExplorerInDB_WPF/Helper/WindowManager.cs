using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace FilesExplorerInDB_WPF.Helper
{
    public static class WindowManager
    {
        private static readonly Hashtable RegisterWindow = new Hashtable();
        private static readonly List<Window> Window = new List<Window>();

        public static void Register<T>(string key)
        {
            if (!RegisterWindow.Contains(key))
                RegisterWindow.Add(key, typeof(T));
        }

        public static void Register(string key, Type t)
        {
            if (!RegisterWindow.Contains(key))
                RegisterWindow.Add(key, t);
        }

        public static void Remove(string key)
        {
            if (!RegisterWindow.ContainsKey(key)) return;
            //RegisterWindow.Remove(key);
            Window window = Window.Find(w => w.Name == key);
            if (window == null) return;
            Window.Remove(window);
            window.Close();
        }

        public static void SetDialogResult(string key, bool result)
        {
            if (!RegisterWindow.ContainsKey(key)) return;
            Window window = Window.Find(w => w.Name == key);
            if (window != null)
                window.DialogResult = result;
        }

        public static bool? Show(string key, bool showDialog = false) //, object vm)
        {
            if (!RegisterWindow.ContainsKey(key)) return null;
            var win = (Window) Activator.CreateInstance((Type) RegisterWindow[key]);
            //win.DataContext = vm;
            win.Name = key;
            Window.Add(win);
            if (showDialog)
                return win.ShowDialog();
            win.Show();
            return null;
        }
    }
}