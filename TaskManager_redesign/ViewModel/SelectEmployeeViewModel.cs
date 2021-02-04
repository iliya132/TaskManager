using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

using TaskManager_redesign.Model;

namespace TaskManager_redesign.ViewModel
{
    public class SelectEmployeeViewModel : INotifyPropertyChanged
    {
        #region MVVM
        private static SelectEmployeeViewModel _instance;
        public static SelectEmployeeViewModel GetInstance()
        {
            if(_instance == null)
            {
                _instance = new SelectEmployeeViewModel();
            }
            return _instance;
        }
        private SelectEmployeeViewModel() 
        {
            PropertyChanged += ViewModelPropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region init
        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(FilterText)))
            {
                RaisePropertyChanged(nameof(AnalyticsFiltered));
            }
        }
        #endregion

        #region data collections
        public ObservableCollection<Analytic> AvailableAnalytics { get; set; }
        private ObservableCollection<Analytic> _analyticsFiltered = new ObservableCollection<Analytic>();
        public ObservableCollection<Analytic> AnalyticsFiltered
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FilterText))
                {
                    return AvailableAnalytics;
                }
                _analyticsFiltered.Clear();
                foreach(Analytic analytic in AvailableAnalytics.Where(a=> $"{a.LastName} {a.FirstName} {a.FatherName}".ToLower().Contains(FilterText.ToLower())))
                {
                    _analyticsFiltered.Add(analytic);
                }
                return _analyticsFiltered;
            }
        }
        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                RaisePropertyChanged(nameof(FilterText));
            }
        }
        #endregion
    }
}
