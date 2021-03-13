using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager_redesign.Model
{
    public class TaskPlan :INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int CreatedById { get; set; }
        [Column("is_done")]
        public bool IsDone { get; set; }
        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                RaisePropertyChanged(nameof(Description));
                RaisePropertyChanged(nameof(DescriptionWithDueDate));
            }
        }
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public UserTask Task { get; set; }
        public Analytic CreatedBy { get; set; }
        [NotMapped()]
        public string DescriptionWithDueDate { get => ToString(); }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return $"{DueDate:dd.MM.yyyy} - {Description}";
        }
    }
}