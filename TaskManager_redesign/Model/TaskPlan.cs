using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager_redesign.Model
{
    public class TaskPlan
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int CreatedById { get; set; }
        [Column("is_done")]
        public bool IsDone { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public UserTask Task { get; set; }
        public Analytic CreatedBy { get; set; }
        public override string ToString()
        {
            return $"{DueDate:dd.MM.yyyy} - {Description}";
        }
    }
}