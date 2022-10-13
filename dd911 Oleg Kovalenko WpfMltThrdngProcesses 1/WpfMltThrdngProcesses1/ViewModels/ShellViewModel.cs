using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using WpfMltThrdngProcesses1.Infrastructure;
using WpfMltThrdngProcesses1.Models;

namespace WpfMltThrdngProcesses1.ViewModels
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        private Process _selectedProcess;
        public ObservableCollection<Process> Processes{ get; set; }
        public ICommand EndProcessCmd { get; set; }
        public Process SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                NotifyOfPropertyChange("SelectedProcess");
            }
        }
        public ShellViewModel()
        {
            Processes = new ObservableCollection<Process>();
            EndProcessCmd = new RelayCommand(actn => EndTask());
            SelectedProcess = Process.GetCurrentProcess();
            DataGridViewInitialize();
        }
        public void EndTask()
        {
            SelectedProcess.Kill();
            SelectedProcess.WaitForExit();
            SelectedProcess = null;
            DataGridViewInitialize();
        }
        private void DataGridViewInitialize()
        {
            Process[] tempProcessesArr = Process.GetProcesses();
            Processes.Clear();
            foreach (Process prcs in tempProcessesArr)
            {
                Processes.Add(prcs);
            }
            NotifyOfPropertyChange("Processes");
        }
    }
}