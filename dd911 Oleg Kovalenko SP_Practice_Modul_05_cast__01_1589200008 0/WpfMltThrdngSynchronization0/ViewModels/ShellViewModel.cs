using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfMltThrdngAsynchrony0.Infrastructure;
using WpfMltThrdngAsynchrony0.Models;
/*SP_Practice_Modul_05_cast__01_1589200008 от 11.03.2021
домашние задания по теме многопоточность выполнены в непредвиденном способе.
ввиду требования на воплощение в оконном приложении, большинством своим, задания зиждятся на применении класса Dispatcher,
отрабатывающего через главный поток,
тем не менее, требования многопоточности и применения тематических способов регуляции оных соблюдены.*/
namespace WpfMltThrdngAsynchrony0.ViewModels
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public ObservableCollection<string> ThreadsSynchronizationResultArr { get; set; }
        public int ThreadsQuantity { get; set; }
        public int FinalValue { get; set; }
        public ICommand Exercise1Cmd { get; set; }
        public ICommand Exercises2and3and4Cmd { get; set; }
        public ICommand Exercise5Cmd { get; set; }
        public ShellViewModel()
        {
            ThreadsQuantity = 1;
            FinalValue = 1000;
            ThreadsSynchronizationResultArr = new ObservableCollection<string>();
            Exercise1Cmd = new RelayCommand(actn => Exercise1());
            Exercises2and3and4Cmd = new RelayCommand(actn => Exercises2and3and4());
            Exercise5Cmd = new RelayCommand(actn => Exercise5());
        }
        /*Задание 1 Создайте приложение, использующее класс Task. Приложение должно отображать текущее время и дату. 
        Запустите задачу три способами:
        ■ Через метод Start класса Task;
        ■ Через метод Task.Factory.StartNew;
        ■ Через метод Task.Run.*/
        private void Exercise1()
        {
            ThreadsSynchronizationResultArr.Clear();
            Task tsk = new Task(() => ThreadsSynchronizationResultArr.Add($"{DateTime.Now}"));
            tsk.Start();
            Task.Factory.StartNew(() => ThreadsSynchronizationResultArr.Add($"{DateTime.Now}"));
            Task.Run(() => ThreadsSynchronizationResultArr.Add($"{DateTime.Now}"));
        }
        /*Задание 2 Приложение должно отображать все простые числа 
        от 0 до 1000. Для отображения чисел необходимо использовать класс Task. Основной поток должен ожидать 
        завершения задачи.
        Задание 3 Модифицируйте второе задание. Необходимо передать задаче границы для генерации простых чисел. Основной поток должен ожидать завершения задачи. После 
        завершения задачи основной поток выводит количество простых чисел.
        Задание 4 Создайте приложение, которое ищет в некотором массиве:
        ■ Минимум;
        ■ Максимум;
        ■ Среднее;
        ■ Сумму.
        Используйте массив Task для решения поставленной задачи.*/
        private void Exercises2and3and4()
        {
            Task[] primeNumbersTasksArr = new Task[5];
            string[] calculationResultsArr = new string[5];
            int i = 0, j = 0;
            ThreadsSynchronizationResultArr.Clear();
            Task<int[]> primeNumbersSearchTsk = new Task<int[]>(() => {
                int[] primeNumbersArr = new int[0];
                for (i = ThreadsQuantity; i < FinalValue; i++)
                {
                    for (j = ThreadsQuantity + 1; j < i; j++)
                    {
                        if (i % j == 0)
                        {
                            break;
                        }
                        if (j == i - 1)
                        {
                            primeNumbersArr = primeNumbersArr.Append(i).ToArray();
                        }
                    }
                }
                return primeNumbersArr;
            });
            primeNumbersSearchTsk.RunSynchronously();//
            i = 0;
            j = 0;
            primeNumbersTasksArr[i++] = new Task(()
                => calculationResultsArr[j++] = $"The minimum {primeNumbersSearchTsk.Result.Min()}");
            primeNumbersTasksArr[i++] = new Task(()
                => calculationResultsArr[j++] = $"The maximum {primeNumbersSearchTsk.Result.Max()}");
            primeNumbersTasksArr[i++] = new Task(()
                => calculationResultsArr[j++] = $"The average {primeNumbersSearchTsk.Result.Average():.00}");
            primeNumbersTasksArr[i++] = new Task(()
                => calculationResultsArr[j++] = $"The sum {primeNumbersSearchTsk.Result.Sum()}");
            primeNumbersTasksArr[i] = new Task(()
                => calculationResultsArr[j] = $"The quantity of prime numbers {primeNumbersSearchTsk.Result.Length}");
            for (i = 0; i < primeNumbersTasksArr.Length; i++)
            {
                primeNumbersTasksArr[i].Start();
            }
            Task.WaitAll(primeNumbersTasksArr);
            for (i = 0; i < primeNumbersSearchTsk.Result.Length + 5; i++)
            {
                if (i > 4)
                {
                    ThreadsSynchronizationResultArr.Add($"{primeNumbersSearchTsk.Result[i - 5]}");
                }
                else
                {
                    ThreadsSynchronizationResultArr.Add(calculationResultsArr[i]);
                }
            }
        }
        /*Задание 5 Создайте приложение для работы с массивом:
        ■ Удаление из массива повторяющихся значений;
        ■ Сортировка массива (стартует после удаления дублей);
        ■ Бинарный поиск некоторого значения (стартует после сортировки).
        Используйте continuation tasks для решения поставленной задачи*/
        private void Exercise5()
        {
            int[] numbersArr = new int[20];
            Random drbg = new Random();
            ThreadsSynchronizationResultArr.Clear();
            Task.Run(() =>
            {
                for (int i = 0; i < numbersArr.Length; i++)
                {
                    numbersArr[i] = drbg.Next(20);
                }
            })
                .ContinueWith(ant => numbersArr = numbersArr.Distinct().ToArray())
                .ContinueWith(ant => Array.Sort(numbersArr))
                .ContinueWith(ant
                => ThreadsSynchronizationResultArr.Add($"The index of search value {Array.BinarySearch(numbersArr, FinalValue)}"))
                .ContinueWith(ant => {
                for (int i = 0; i < numbersArr.Length; i++)
                {
                    ThreadsSynchronizationResultArr.Add($"{numbersArr[i]}");
                }
                });
        }
    }
}