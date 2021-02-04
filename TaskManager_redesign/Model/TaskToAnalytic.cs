using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager_redesign.Model
{
    public class TaskToAnalytic :INotifyPropertyChanged
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("task")]
        public int TaskId { get; set; }
        [Column("analytic")]
        public int AnalyticId { get; set; }
        [Column("status")]
        public int StatusId { get; set; }
        private string _comment;
        [Column("comment")]
        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                RaisePropertyChanged(nameof(Comment));
            }
        }

        private Status _status;
        public Status Status
        {
            get => _status;
            set
            {
                _status = value;
                RaisePropertyChanged(nameof(Status));
            }
        }
        public Analytic Analytic { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}