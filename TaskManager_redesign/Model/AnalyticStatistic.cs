using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager_redesign.Model
{
    public class AnalyticStatistic
    {
        public string Name { get; set; }
        public int TasksCount { get; set; }
        public int TaskPlansCount { get; set; }
        public int TasksDone { get; set; }
        public int TaskPlansDone { get; set; }
        public int TasksExpired { get; set; }
        public int TaskPlansExpired { get; set; }
        public int q1TasksCount { get; set; }
        public int q1TaskPlansCount { get; set; }
        public int q2TasksCount { get; set; }
        public int q2TaskPlansCount { get; set; }
        public int q3TasksCount { get; set; }
        public int q3TaskPlansCount { get; set; }
        public int q4TasksCount { get; set; }
        public int q4TaskPlansCount { get; set; }
        public double TasksCompletionPercent { get => TasksCount == 0 ? 0 : Math.Round((double)TasksDone / TasksCount * 100, 2); }
        public double TaskPlansCompletionPercent { get => TaskPlansCount == 0 ? 0 : Math.Round((double)TaskPlansDone / TaskPlansCount * 100, 2); }
        public double TasksExpiredPercent{ get => TasksCount == 0 ? 0 : Math.Round((double)TasksExpired / TasksCount * 100, 2); }
        public double TaskPlansExpiredPercent{ get => TaskPlansCount == 0 ? 0 : Math.Round((double)TaskPlansExpired / TaskPlansCount * 100, 2); }
    }
}
