using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Win32;

namespace NEditor
{
    public partial class MainWindow : Window
    {
        /* Так как интерфейс редактора однодокументный, 
         * можно записать путь к текущему файлу в поле
         */
        String currentFilePath = null;
        
        public MainWindow()
        {
            InitializeComponent();
            //Текстовое поле невидимо, пока не создан или открыт файл
            rtxtText.Visibility = Visibility.Collapsed;
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            if (openFileDlg.ShowDialog() ?? false) //Если результат открытия диалога null, блок не выполнится
            {
                //Загрузка из файла
                TextRange textRange;
                System.IO.FileStream fileStream;
                textRange = new TextRange(rtxtText.Document.ContentStart, rtxtText.Document.ContentEnd);
                using (fileStream = new System.IO.FileStream(openFileDlg.FileName, System.IO.FileMode.OpenOrCreate))
                {
                    textRange.Load(fileStream, System.Windows.DataFormats.Rtf);
                    currentFilePath = openFileDlg.FileName;
                }
                rtxtText.Visibility = Visibility.Visible;
            }
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Перезапись или создание файла
            TextRange textRange;
            System.IO.FileStream fileStream;
            textRange = new TextRange(rtxtText.Document.ContentStart, rtxtText.Document.ContentEnd);
            using (fileStream = new System.IO.FileStream(currentFilePath, System.IO.FileMode.Create))
            {
               textRange.Save(fileStream, System.Windows.DataFormats.Rtf);
            }
        }
        
        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Закрытие файла и сброс параметров
            currentFilePath = null;
            TextRange textRange = new TextRange(rtxtText.Document.ContentStart, rtxtText.Document.ContentEnd);
            textRange.Text = "";
            rtxtText.Visibility = Visibility.Collapsed;
        }

        private void ToolbarShowHide_Click(object sender, RoutedEventArgs e)
        {
            //Видимость панели инструментов
            MenuItem mItem = sender as MenuItem;
            toolBar1.Visibility = mItem.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            //Выход из приложения
            Application.Current.Shutdown(0);
        }

        private void CloseCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //Команду закрытия файла можно вызвать только если таков загружен
            e.CanExecute = currentFilePath != null;
        }

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Указание местоположения файла при создании
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            if (saveFileDlg.ShowDialog() ?? false)
            {
                currentFilePath = saveFileDlg.FileName;
            }
            rtxtText.Visibility = Visibility.Visible;
        }

        private void SaveCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //Команду сохранения файла можно вызвать только если таков загружен
            e.CanExecute = currentFilePath != null;
        }
    }
}
