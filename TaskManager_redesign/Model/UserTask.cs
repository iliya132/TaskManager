using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TaskManager_redesign.Model
{
    public class UserTask :INotifyPropertyChanged
    {
        [Column("id")]
        public int Id { get; set; }
        private string _name;
        [Column("subject")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        private DateTime _startDate;
        [Column("start_date")]
        public DateTime StartDate { get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }
        private DateTime _dueDate;
        [Column("due_date")]
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                RaisePropertyChanged(nameof(DueDate));
            }
        }
        [Column("created_by")]
        public int CreatedById { get; set; }
        public Analytic CreatedBy { get; set; }
        public ObservableCollection<TaskToAnalytic> AssignedTo { get; set; } = new ObservableCollection<TaskToAnalytic>();
        private string _awaitedResult;
        [Column("awaited_result")]
        public string AwaitedResult
        {
            get => _awaitedResult;
            set
            {
                _awaitedResult = value;
                RaisePropertyChanged(nameof(AwaitedResult));
            }
        }
        private string _description;
        [Column("description")]
        public string TaskDescription
        {
            get => _description;
            set
            {
                _description = value;
                RaisePropertyChanged(nameof(TaskDescription));
            }
        }
        public ObservableCollection<TaskPlan> TaskPlans { get; set; } = new ObservableCollection<TaskPlan>();
        [Column("status")]
        public int StatusId { get; set; }
        public Status Status { get; set; }
        [Column("parent_task")]
        public int? ParentTaskId { get; set; }
        public bool IsHeader { get; set; } = false;
        public ObservableCollection<UserTask> ChildTasks { get; set; } = new ObservableCollection<UserTask>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddChildAndSort(UserTask newChild)
        {
            List<UserTask> tasks = ChildTasks.ToList();
            tasks.Add(newChild);
            tasks = tasks.OrderBy(i => i.ChildTasks.Count).ToList();
            ChildTasks.Clear();
            tasks.ForEach(ChildTasks.Add);
        }

        public bool ContainChild(UserTask task)
        {
            if(ChildTasks.Count == 0)
            {
                return false;
            }
            foreach(UserTask childTask in ChildTasks)
            {
                if(childTask == task || childTask.ContainChild(task))
                {
                    return true;
                }
            }
            return false;
        }

        public UserTask ParentTask { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is UserTask task)
            {
                return task.Id == Id;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if(_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        private bool _isExpanded = false;
        public bool IsExpanded 
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if(_isExpanded != value)
                {
                    _isExpanded = value;
                    RaisePropertyChanged("IsExpanded");
                }
            }
        }
    }
}
