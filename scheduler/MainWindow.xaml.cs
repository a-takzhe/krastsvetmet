using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Data;
using System.Collections.ObjectModel;

namespace scheduler
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Scheduler scheduler;//Экземпляр основного класса 
        private ExcelWorker excel_worker; //Экземпляр класса записи в документ

        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Обработчик нажатия кнопки (Выбрать папку с файлами)
        /// </summary>
        private void BtnSelectDir_Click(object sender, RoutedEventArgs e)
        {
            //Скрываем и обнуляем некоторые компоненты
            ProgressBar.Value = 0;
            LStatus.Content = "";
            ProgressBar.Visibility = Visibility.Visible;
            IconCheck.Visibility = Visibility.Collapsed;
            IconWarning.Visibility = Visibility.Collapsed;

            //После выбора папки запускается дочерный процесс, который выполняет 
            //функцию worker_SelectDir
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += worker_SelectDir;
                worker.RunWorkerAsync(dialog.FileName);
            }
            else if(scheduler == null)
            {
                LStatus.Content = "Данные не загружены!";
                ProgressBar.Visibility = Visibility.Collapsed;
                IconWarning.Visibility = Visibility.Visible;
            }
            else
            {
                LStatus.Content = $"Данные загружены";
                ProgressBar.Visibility = Visibility.Collapsed; 
                IconCheck.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки (Спланировать рассписание)
        /// </summary>
        private void BtnSchedule_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            IconCheck.Visibility = Visibility.Collapsed;
            IconWarning.Visibility = Visibility.Collapsed;

            string type = string.Empty;
            if (RB_PS.IsChecked.Value)
            {
                type = "johnson";
            }
            else if (RB_Jhns.IsChecked.Value)
            {
                type = "petrov-sokolitsin";
            }

            //запускается дочерный процесс, который выполняет функцию worker_Schedule
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_Schedule;
            worker.RunWorkerAsync(type);
        }

        /// <summary>
        /// Обработчик нажатия кнопки (Вывести результаты планирования в Excel)
        /// </summary>
        private void BtnCreateExcel_Click(object sender, RoutedEventArgs e)
        {
            //Создаем файл -> заполнение отчета -> сихранение -> закрытие
            CommonSaveFileDialog dialog = new CommonSaveFileDialog();
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                excel_worker = new ExcelWorker();
                excel_worker.CreateReport(scheduler.ScheduleResult, scheduler.GanttTree);
                excel_worker.SaveAs(dialog.FileName);
                excel_worker.Close();
            }

        }

        //Функция в которой происходит инициализация класса-планировщика и
        //создание двух сводных таблиц
        void worker_SelectDir(object sender, DoWorkEventArgs e)
        {
            string path = (string)e.Argument;

            try
            {
                Dispatcher.BeginInvoke(new ThreadStart(delegate { LStatus.Content = $"Загрузка данных..."; }));
                scheduler = new Scheduler(path);

                Dispatcher.BeginInvoke(new ThreadStart(delegate { LStatus.Content = $"Вычисление вспомогательных параметров..."; }));
                scheduler.CreateTimeMaps();
                CreateTableTimeMap(scheduler.StringTimeMap);

                Dispatcher.BeginInvoke(new ThreadStart(delegate { DGV_Parties.ItemsSource = scheduler.CreateListPartiesWithNomenclature(); ; }));
                Dispatcher.BeginInvoke(new ThreadStart(delegate { LStatus.Content = $"Данные загружены"; }));
                Dispatcher.BeginInvoke(new ThreadStart(delegate { ProgressBar.Visibility = Visibility.Collapsed; }));
                Dispatcher.BeginInvoke(new ThreadStart(delegate { IconCheck.Visibility = Visibility.Visible; }));
                Dispatcher.BeginInvoke(new ThreadStart(delegate { BtnSchedule.IsEnabled = true; }));

                MessageBox.Show("Далее необходимо выбрать один из методов планирования и нажать кнопку 'Спланировать рассписание'");
            }
            catch
            {
                MessageBox.Show("В выбранной директории нет нужных файлов \n (parties.xlsx, nomenclatures.xlsx, machine_tools.xlsx, times.xlsx)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Dispatcher.BeginInvoke(new ThreadStart(delegate { LStatus.Content = $"Данные не загружены"; }));
                Dispatcher.BeginInvoke(new ThreadStart(delegate { ProgressBar.Visibility = Visibility.Collapsed; }));
                Dispatcher.BeginInvoke(new ThreadStart(delegate { IconWarning.Visibility = Visibility.Visible; }));
            }
        }

        //В функции вызывается метод планирования загрузки и метод заполнения дерева для диаграммы Гантта
        void worker_Schedule(object sender, DoWorkEventArgs e)
        {
            List<int> PlanningResult = null;
            string type = (string)e.Argument;

            if (scheduler != null)
            {
                Dispatcher.BeginInvoke(new ThreadStart(delegate { LStatus.Content = $"Идёт планирование..."; }));
                PlanningResult = scheduler.Schedule(type);

                Dispatcher.BeginInvoke(new ThreadStart(delegate { LStatus.Content = $"Создание ключевого представления..."; }));
                if (PlanningResult != null)
                {
                    var gantt = scheduler.CreateGanttTree(PlanningResult);
                    Dispatcher.BeginInvoke(new ThreadStart(delegate { TreeViewGantt_String.ItemsSource = gantt; }));
                    Dispatcher.BeginInvoke(new ThreadStart(delegate { TreeViewGantt_Bar.ItemsSource = gantt; }));
                }

                Dispatcher.BeginInvoke(new ThreadStart(delegate { LStatus.Content = $"Загрузка спланирована"; }));
                Dispatcher.BeginInvoke(new ThreadStart(delegate { ProgressBar.Visibility = Visibility.Collapsed; }));
                Dispatcher.BeginInvoke(new ThreadStart(delegate { IconCheck.Visibility = Visibility.Visible; }));
                Dispatcher.BeginInvoke(new ThreadStart(delegate { BtnCreateExcel.IsEnabled = true; }));

                MessageBox.Show("Планирование окончено! Теперь можно перейти на вкладку 'Результаты планирования' для ознакомления или нажать на кнопку 'Вывести результаты планирования в Excel'");
            }
        }

        ///Функция заполнения DGV динамическими данными
        private void CreateTableTimeMap(List<List<string>> map)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < map[0].Count; i++)
            {
                if (i == 0)
                {
                    foreach(var Col in map)
                    {
                        dt.Columns.Add(new DataColumn(Col[0]));
                    }
                }
                else
                {
                    var row = dt.NewRow();
                    foreach (var Col in map)
                    {
                        row[Col[0]] = Col[i];   
                    }
                    dt.Rows.Add(row);
                }
            }
            Dispatcher.BeginInvoke(new ThreadStart(delegate { DGV_Times.DataContext = dt.DefaultView; }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Для начала работы нужно выбрать папку в которой хроняться файлы БД");
        }
    }
}