using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _25._03._2022
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Путь к корневому каталогу.
        private const string ROOTDIRPATH = @"C:\Users";

        public MainWindow()
        {
            InitializeComponent();
        }

        // Обработчик нажатия на кнопку "Открыть дерево каталогов".
        private void btnOpenDirectoryMenu_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            ListDirectory(ROOTDIRPATH);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        // Перечисляет все каталоги с помощью рекурсивной ф-ии CreateDirectoryNode.
        private void ListDirectory(string path)
        {
            tvDirectory.Items.Clear();
            DirectoryInfo rootDirectoryName = new DirectoryInfo(path);
            tvDirectory.Items.Add(CreateDirectoryNode(rootDirectoryName));
        }

        // Рекурсивная ф-ия. Создает и возвращает элемент <TreeViewItem>.
        private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var dirNode = new TreeViewItem();
            dirNode.Header = directoryInfo.Name;
            dirNode.Tag = directoryInfo.FullName;
            // Проход по подкаталогам текущего каталога.
            try
            {
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    dirNode.Items.Add(CreateDirectoryNode(directory));
                }
            }
            catch { } // Для игнорирования исключений типа "Доступ к файлу запрещен".

            // Проход по файлам текущего каталога.
            try
            {
                foreach (var file in directoryInfo.GetFiles())
                {
                    var fileNode = new TreeViewItem();
                    fileNode.Header = file.Name;
                    fileNode.Tag = file.FullName;
                    dirNode.Items.Add(fileNode);
                }
            }
            catch { } // Для игнорирования исключений типа "Доступ к файлу запрещен".

            return dirNode;
        }

        // Загружает данные о файлах выбранного каталога в таблицу ListView.
        private void tviShowDirInfo(object sender, RoutedEventArgs e)
        {
            lvDirInfo.ItemsSource = null;
            lvDirInfo.Items.Clear();

            try
            {
                string fullPath = ((TreeViewItem)((TreeView)sender).SelectedItem).Tag.ToString();

                // Проверка, указывает ли путь на каталог или на файл.
                FileAttributes attr = File.GetAttributes(fullPath);
                if (attr.HasFlag(FileAttributes.Directory)) // указывает на каталог
                {
                    DirectoryInfo d = new DirectoryInfo(fullPath);
                    FileInfo[] files = d.GetFiles("*.*");
                    lvDirInfo.ItemsSource = files;
                }
                else // на файл
                {
                    FileInfo fileInfo = new FileInfo(fullPath);
                    lvDirInfo.Items.Add(fileInfo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }
    }
}
