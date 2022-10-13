using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using WpfMltThrdngSynchronization1.Infrastructure;
using WpfMltThrdngSynchronization1.Models;
/*SP_Practice_Modul_04_cast__02_1589199950 от 10.03.2021
домашние задания по теме многопоточность выполнены в непредвиденном способе.
ввиду требования на воплощение в оконном приложении, большинством своим, задания зиждятся на применении класса Dispatcher,
отрабатывающего через главный поток,
тем не менее, требования многопоточности и применения тематических способов регуляции оных соблюдены.*/
namespace WpfMltThrdngSynchronization1.ViewModels
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public ObservableCollection<string> ThreadsSynchronizationResultArr { get; set; }
        public string ThreadsQuantity { get; set; }
        public ICommand Exercises1and3Cmd { get; set; }
        public ICommand Exercise2Cmd { get; set; }
        private AutoResetEvent Are { get; set; }
        private object SynchronizationObj { get; set; }
        public ShellViewModel()
        {
            ThreadsQuantity = $"{10}";
            ThreadsSynchronizationResultArr = new ObservableCollection<string>();
            Are = new AutoResetEvent(true);
            SynchronizationObj = new object();
            Exercises1and3Cmd = new RelayCommand(actn => Exercises1and3());
            Exercise2Cmd = new RelayCommand(actn => Exercise2());
        }
        /*Задание 1 Создайте приложение, использующее механизм событий. Создайте в коде приложения несколько потоков. 
        Первый поток генерирует 1000 чисел в диапазоне от 0 до 5000. Три потока ожидают, когда генерирование будет 
        завершено. Когда все числа сгенерированы, три потока стартуют процесс анализа полученных данных. Первый 
        поток находит максимум среди чисел. Второй поток находит минимум. Третий поток высчитывает среднее 
        арифметическое.
        Задание 3 Создайте приложение, использующее механизм критических секций. Создайте в коде приложения несколько 
        потоков. Первый поток получает в качестве аргумента 
        массив данных и сортирует массив по возрастанию. Второй поток ожидает, пока первый закончит свою работу и 
        проверяет есть ли некоторое число в отсортированном массиве. Число передаётся внутрь потоковой функции в 
        качестве параметра.*/
        private void Exercises1and3()
        {
            int[] drbgArr = ArrayFilling();//задание 1. неточность "Первый поток", как результат генерация чисел главным потоком.
            Thread[] arrayThreadsArr = new Thread[5];
            short i = 0;
            ThreadsSynchronizationResultArr.Clear();
            arrayThreadsArr[i++] = new Thread(() => { Exercises1and3ThreadsCore(()
                => { ThreadsSynchronizationResultArr.Add($"The maximum is\t{drbgArr.Max()}"); }); });

            arrayThreadsArr[i++] = new Thread(() => { Exercises1and3ThreadsCore(()
                => { ThreadsSynchronizationResultArr.Add($"The minimum is\t{drbgArr.Min()}"); }); });

            arrayThreadsArr[i++] = new Thread(() => { Exercises1and3ThreadsCore(()
                => { ThreadsSynchronizationResultArr.Add($"The average is\t{drbgArr.Average():.00}"); }); });
            //нарушена очерёдность выполнения потоков с целью вывода результата поиска числа в обозреваемой области окна.
            arrayThreadsArr[i++] = new Thread(() => { Exercises1and3ThreadsCore(()
                => { ThreadsSynchronizationResultArr.Add($"The array contains {ThreadsQuantity}\t{drbgArr.Contains(Int32.Parse(ThreadsQuantity))}"); }); });

            arrayThreadsArr[i] = new Thread(() => { Exercises1and3ThreadsCore(()
                => {
                        drbgArr = drbgArr.OrderBy(value => value).ToArray();
                        for (short j = 0; j < drbgArr.Length; j++)
                        {
                            ThreadsSynchronizationResultArr.Add($"{drbgArr[j]}");//
                        }
                }); });

            for (i = 0; i < arrayThreadsArr.Length; i++)
            {
                arrayThreadsArr[i].IsBackground = true;
                arrayThreadsArr[i].Start();
            }
            arrayThreadsArr.Last().Join();//
        }
        private void Exercises1and3ThreadsCore(Action actn)
        {
            Are.WaitOne();
            App.Current.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                actn.Invoke();
            });
            Are.Set();
        }
        private int[] ArrayFilling()
        {
            int[] drbgArr = new int[1000];
            Random drbg = new Random();
            for (short i = 0; i < drbgArr.Length; i++)
            {
                drbgArr[i] = drbg.Next(5000);
            }
            return drbgArr;
        }
        /*Задание 2 Создайте приложение, использующее механизм критических секций. Создайте в коде приложения несколько 
        потоков. Первый поток получает в качестве аргумента путь к файлу, считывает содержимое файла и подсчитывает 
        количество предложений. Второй поток ожидает, пока первый закончит свою работу и модифицирует файл 
        (производится замена всех ! на #).*/
        private void Exercise2()
        {
            Thread[] count2010ThreadsArr = new Thread[2];
            //string path = String.Empty;
            int symbolCnt;
            ThreadsSynchronizationResultArr.Clear();
            count2010ThreadsArr[0] = new Thread(() => {
                lock (SynchronizationObj)//
                {
                    using (StreamReader strmRdr = new StreamReader(ThreadsQuantity))
                    {
                        symbolCnt = strmRdr.ReadToEnd()
                                     .Where(symb => symb == '.' || symb == '?' || symb == '!').Count();
                    }
                    App.Current.Dispatcher.BeginInvoke((ThreadStart)delegate {
                        ThreadsSynchronizationResultArr.Add($"The quantity of sentences in the file {symbolCnt}");
                    });
                }
            });
            count2010ThreadsArr[1] = new Thread(() => {
                lock (SynchronizationObj)//
                {
                    string fileContent;
                    using (StreamReader strmRdr = new StreamReader(ThreadsQuantity))
                    {
                        fileContent = strmRdr.ReadToEnd();
                        fileContent = fileContent.Replace('!', '#');
                    }
                    using (StreamWriter strmWrtr = new StreamWriter(ThreadsQuantity))
                    {
                        strmWrtr.Write(fileContent);
                    }
                    App.Current.Dispatcher.BeginInvoke((ThreadStart)delegate {
                        ThreadsSynchronizationResultArr.Add("The remplacement of symbol '!' by '#' is done");
                    });
                }
            });
            for (short i = 0; i < count2010ThreadsArr.Length; i++)
            {
                count2010ThreadsArr[i].IsBackground = true;
                count2010ThreadsArr[i].Start();
            }
            count2010ThreadsArr.Last().Join();
        }
    }
}