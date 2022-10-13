using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using WpfMltThrdngSynchronization0.Infrastructure;
using WpfMltThrdngSynchronization0.Models;
/*SP_Practice_Modul_04_cast__01_1589199895 от 09.03.2021
домашние задания по теме многопоточность выполнены в непредвиденном способе.
ввиду требования на воплощение в оконном приложении, большинством своим, задания зиждятся на применении класса Dispatcher,
отрабатывающего через главный поток,
тем не менее, требования многопоточности и применения тематических способов регуляции оных соблюдены.*/
namespace WpfMltThrdngSynchronization0.ViewModels
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public ObservableCollection<string> ThreadsSynchronizationResultArr { get; set; }
        public ICommand Exercises1and6Cmd { get; set; }
        public ICommand Exercises2and3and7Cmd { get; set; }
        public ICommand Exercise5Cmd { get; set; }
        private Random Drbg { get; set; }
        private Mutex Mtx { get; set; }
        private Semaphore Smphre { get; set; }
        public ShellViewModel()
        {
            ThreadsSynchronizationResultArr = new ObservableCollection<string>();
            Exercises1and6Cmd = new RelayCommand(actn => Exercises1and6());
            Exercises2and3and7Cmd = new RelayCommand(actn => Exercises2and3and7());
            Exercise5Cmd = new RelayCommand(actn => Exercise5());
            Drbg = new Random();
            Mtx = new Mutex();
            Smphre = new Semaphore(3, 3);
        }
        /*Задание 1 Создайте приложение, использующее механизм мьютексов. Создайте в коде приложения несколько потоков. 
        Первый поток отображает числа от 0 до 20 в порядке возрастания. Второй поток ожидает, когда завершится 
        первый, после чего отображает числа от 0 до 10 в обратном порядке. Вывод данных необходимо выполнять в консоль.
        Задание 6 Реализуйте первое задание внутри приложения с оконным интерфейсом. Выбор элементов управления остаётся 
        за вами.*/
        private void Exercises1and6()
        {
            Thread[] count2010ThreadsArr = new Thread[2];
            ThreadsSynchronizationResultArr.Clear();
            count2010ThreadsArr[0] = new Thread(() => { Exercise1ThreadsCore(0, 20, value => ++value); });
            count2010ThreadsArr[1] = new Thread(() => { Exercise1ThreadsCore(10, 0, value => --value); });
            for (short i = 0; i < count2010ThreadsArr.Length; i++)
            {
                count2010ThreadsArr[i].IsBackground = true;
                count2010ThreadsArr[i].Start();
            }
            count2010ThreadsArr.Last().Join();
        }
        private void Exercise1ThreadsCore(int i, int finalValue, Func<int, int> calcFuncDelegate)
        {
            Mtx.WaitOne();
            App.Current.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                ThreadsSynchronizationResultArr.Add($"The sequence from {i} to {finalValue}");
                while (i != finalValue)
                {
                    i = calcFuncDelegate(i);
                    ThreadsSynchronizationResultArr.Add($"{i}");
                }
            });
            Mtx.ReleaseMutex();
        }
        /*Задание 2 Создайте приложение, использующее механизм мьютексов. Создайте в коде приложения несколько потоков.Каждый из
        потоков получает массив с данными.Первый поток должен модифицировать элементы массивы, увеличив каждый на
        некоторое случайное число (элемент массива + случайное число). Второй поток ожидает, когда пройдет модификация
        всего массива, после чего находит максимальное значение в этом массиве и отображает его на экран.Вывод данных
        (массив и максимум) необходимо выполнять в консоль.
        Задание 3 Модифицируйте второе задание.Необходимо отображать максимальное значение и модифицированный
        массив вне потоковой функции.Например, отображение можно выполнять в Main.
        Задание 7 Реализуйте второе задание внутри приложения с оконным интерфейсом. Выбор элементов управления остаётся 
        за вами.*/
        private void Exercises2and3and7()
        {
            int[] drbgArr = ArrayFilling();
            Thread[] arrayThreadsArr = new Thread[2];
            arrayThreadsArr[0] = new Thread(() => { Exercises2and3and7ThreadsCore(drbgArr, true); });
            arrayThreadsArr[1] = new Thread(() => { Exercises2and3and7ThreadsCore(drbgArr); });
            for (short i = 0; i < arrayThreadsArr.Length; i++)
            {
                arrayThreadsArr[i].IsBackground = true;
                arrayThreadsArr[i].Start();
            }
            arrayThreadsArr.Last().Join();//
        }
        private void Exercises2and3and7ThreadsCore(int[] drbgArr, bool firstThreadKey = false)
        {
            Mtx.WaitOne();
            App.Current.Dispatcher.BeginInvoke((ThreadStart)delegate//задание 3. отрабатывает в главном потоке.
            {
                if (firstThreadKey)//
                {
                    ThreadsSynchronizationResultArr.Clear();
                    for (short i = 0; i < drbgArr.Length; i++)
                    {
                        drbgArr[i] += drbgArr[0];
                        ThreadsSynchronizationResultArr.Add($"{drbgArr[i]}");
                    }
                }
                else
                {
                    ThreadsSynchronizationResultArr.Add($"Array maximum is\t{drbgArr.Max()}");//задание 1. вывод на экран.
                }
            });
            Mtx.ReleaseMutex();
        }
        private int[] ArrayFilling()
        {
            int[] drbgArr = new int[10];
            for (short i = 0; i < drbgArr.Length; i++)
            {
                drbgArr[i] = Drbg.Next(10);
            }
            return drbgArr;
        }
        /*Задание 5 Создайте приложение, использующее механизм семафоров. Создайте в коде приложения десять потоков. 
        Каждый из потоков выводит набор случайных чисел после чего завершает свою работу. Перед отображением 
        нужно показать идентификатор потока. Одновременно могут исполняться только три потока, остальные потоки 
        выстраиваются в очередь. Как только какой-то поток завершает своё исполнение, новый запускается.*/
        private void Exercise5()
        {
            Thread[] threadsArr = new Thread[10];
            ThreadsSynchronizationResultArr.Clear();
            for (short i = 0; i < threadsArr.Length; i++)
            {
                threadsArr[i] = new Thread(() => { Exercise5ThreadsCore(); }) { IsBackground = true };
                threadsArr[i].Start();
            }
        }
        private void Exercise5ThreadsCore()
        {
            Smphre.WaitOne();
            int threadId = Thread.CurrentThread.ManagedThreadId;
            App.Current.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                for (short i = 0; i < 3; i++)
                {
                    ThreadsSynchronizationResultArr.Add($"{Drbg.Next(10)}");//задание 5. метод вызывается несколькими потоками, тем не менее, механика требует главный поток.
                }
                ThreadsSynchronizationResultArr.Add($"The managed thread id is\t{threadId}");
            });
            Smphre.Release();
        }
    }
}