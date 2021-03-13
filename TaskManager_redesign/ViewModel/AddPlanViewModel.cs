using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TaskManager_redesign.ViewModel
{
    public class AddPlanViewModel : INotifyPropertyChanged
    {
        #region MVVM
        private static AddPlanViewModel _instance;
        public static AddPlanViewModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new AddPlanViewModel();
            }
            return _instance;
        }
        private AddPlanViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public string Name { get; 
            set; }
        public DateTime DueDate { get; set; } = DateTime.Now.Date;


    }
}
