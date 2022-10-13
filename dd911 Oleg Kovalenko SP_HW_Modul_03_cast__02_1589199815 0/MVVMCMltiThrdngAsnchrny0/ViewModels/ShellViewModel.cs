using MVVMCMltiThrdngAsnchrny0.Infrastructure;
using MVVMCMltiThrdngAsnchrny0.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace MVVMCMltiThrdngAsnchrny0.ViewModels
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        private string _currentExerciseName;
        private ICommand _runExerciseCmd;

        public ObservableCollection<ProgressbarPropertiesStrctre> ExerciseResultsObsrvbleClctn { get; set; }
        public CollectionView ExerciseResultsClctnVw { get; set; }

        public string[] ExerciseNamesArr { get; set; }
        public string CurrentExerciseName
        {
            get => _currentExerciseName;
            set
            {
                _currentExerciseName = value;
                ExercisesDispatcher();
            }
        }

        public ICommand RunExerciseCmd
        {
            get => _runExerciseCmd;
            set
            {
                _runExerciseCmd = value;
                NotifyOfPropertyChange("RunExerciseCmd");
            }
        }
        public string UserInputStr { get; set; }

        private Random PRNG { get; set; }
        private int CompletionOrder { get; set; }

        private int SoughtWordQuantity { get; set; }
        private List<string> FileNamesLst { get; set; }
        public List<string> FullFilePathsLst { get; set; }


        public ShellViewModel()
        {
            ExerciseResultsObsrvbleClctn = new ObservableCollection<ProgressbarPropertiesStrctre>();
            ExerciseResultsClctnVw = (CollectionView)CollectionViewSource.GetDefaultView(ExerciseResultsObsrvbleClctn);

            ExerciseNamesArr = new string[3] { "1 and 2", "3", "4 and 5" };
            CurrentExerciseName = ExerciseNamesArr[0];

            UserInputStr = "10";
            
            PRNG = new Random();

            FileNamesLst = new List<string>();
            FullFilePathsLst = new List<string>();
        }


        private void RandomBarsRaceHandling()
        {
            int barsQuantity;
            CompletionOrder = 0;
            ExerciseResultsObsrvbleClctn.Clear();
            try
            {
                barsQuantity = Convert.ToInt32(UserInputStr);
            }
            catch (Exception excptn)
            {
                System.Windows.MessageBox.Show(excptn.Message, "Caramba!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int j = -1;
            for (int i = 0; i < barsQuantity; i++)
            {
                ExerciseResultsObsrvbleClctn.Add(new ProgressbarPropertiesStrctre() { ProgressbarColorStr = GetPRNGColor() });
                Task.Run(() => RandomBarsRaceCore(++j));
            }
        }
        private string GetPRNGColor()
        {
            switch (PRNG.Next(16))
            {
                case 0:
                    return "Red";
                case 1:
                    return "White";
                case 2:
                    return "Black";
                case 3:
                    return "Blue";
                case 4:
                    return "Purple";
                case 5:
                    return "Cyan";
                case 6:
                    return "Lime";
                case 7:
                    return "Pink";
                case 8:
                    return "Orange";
                case 9:
                    return "Yellow";
                case 10:
                    return "Violet";
                case 11:
                    return "Gold";
                case 12:
                    return "Indigo";
                case 13:
                    return "Chartreuse";
                case 14:
                    return "Aqua";
                case 15:
                    return "Magenta";
                default:
                    return "Coral";
            }
        }

        private void RandomBarsRaceCore(int currentBar)
        {
            int barIndex = currentBar;
            int latency = PRNG.Next(100);
            ProgressbarPropertiesStrctre stringUpdate = ExerciseResultsObsrvbleClctn[currentBar];
            for (int i = 1; i <= 100; i++)
            {
                if (i < 100)
                {
                    stringUpdate.ProgressbarValueStr = $"{i}";
                }
                else
                {
                    stringUpdate.ProgressbarValueStr = $"Completion order\t\t{++CompletionOrder}";
                }
                App.Current.Dispatcher.BeginInvoke((ThreadStart)delegate
                {
                    ExerciseResultsObsrvbleClctn[barIndex] = stringUpdate;
                });
                Thread.Sleep(latency);
            }
        }

        
        private void PingalaSequenceSum()
        {
            int limitValue;
            try
            {
                limitValue = Convert.ToInt32(UserInputStr);
            }
            catch (Exception excptn)
            {
                System.Windows.MessageBox.Show(excptn.Message, "Caramba!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int n_1 = 1;
            int n_2 = 0;
            int sum = 0;
            for (int i = 0; i < limitValue; i++)
            {
                if (i == n_1 + n_2)
                {
                    n_2 = n_1;
                    n_1 = i;
                    sum += i;
                }
            }
            App.Current.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                ExerciseResultsObsrvbleClctn.Clear();
                ExerciseResultsObsrvbleClctn.Add(new ProgressbarPropertiesStrctre { ProgressbarValueStr = $"The sum of Pingala sequence\t{sum}" });
            });
        }


        private void WordSearchHandling()
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                Task.Run(() =>
                {
                    SoughtWordQuantity = 0;
                    FileNamesLst.Clear();
                    FullFilePathsLst.Clear();
                    Recursion(FBD.SelectedPath);
                    PringResults();
                });
            }
        }

        private void Recursion(string currentDirectoryPath)
        {
            string[] innerPaths = Directory.GetDirectories(currentDirectoryPath);
            for (int i = 0; i < innerPaths.Length; i++)
            {
                Recursion(innerPaths[i]);
            }
            innerPaths = Directory.GetFiles(currentDirectoryPath);
            for (int i = 0; i < innerPaths.Length; i++)//
            {
                WordSearch(innerPaths[i]);
            }
        }

        private void WordSearch(string filePath)
        {
            try
            {
                using (StreamReader strmRdr = new StreamReader(filePath))
                {
                    string[] fileContent = strmRdr.ReadToEnd().Split(" !?".ToCharArray());
                    int foundQuantity = Array.FindAll(fileContent, wrd => wrd == UserInputStr).Length;
                    if (foundQuantity > 0)
                    {
                        string[] fullFilePath = filePath.Split('\\');
                        FileNamesLst.Add(fullFilePath[fullFilePath.Length - 1]);
                        FullFilePathsLst.Add(filePath);
                        SoughtWordQuantity += foundQuantity;
                    }
                }
            }
            catch (Exception excptn)
            {
                System.Windows.MessageBox.Show(excptn.Message, "Caramba!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PringResults()
        {
            App.Current.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                ExerciseResultsObsrvbleClctn.Clear();
                for (int i = 0; i < FileNamesLst.Count; i++)
                {
                    ExerciseResultsObsrvbleClctn.Add(new ProgressbarPropertiesStrctre() { ProgressbarValueStr = $"File name\t{FileNamesLst[i]}" });
                    ExerciseResultsObsrvbleClctn.Add(new ProgressbarPropertiesStrctre() { ProgressbarValueStr = $"Path\t\t{FullFilePathsLst[i]}" });
                }
                ExerciseResultsObsrvbleClctn.Add(new ProgressbarPropertiesStrctre()
                {
                    ProgressbarValueStr = $"The sought word occurrences quantity\t{SoughtWordQuantity}"
                });
            });
        }


        private void ExercisesDispatcher()
        {
            switch (CurrentExerciseName)
            {
                case "3":
                    RunExerciseCmd = new RelayCommand(actn => Task.Run(() => PingalaSequenceSum()));
                    break;
                case "4 and 5":
                    RunExerciseCmd = new RelayCommand(actn => WordSearchHandling());
                    break;
                default:
                    RunExerciseCmd = new RelayCommand(actn => RandomBarsRaceHandling());
                    break;
            }
        }
    }
}