using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager_redesign.Model
{
    public class AnalyticStatistic
    {
        public string Name { get; set; }
        public int TasksCount { get; set; }
        public int TasksDone { get; set; }
        public int TasksExpired { get; set; }
        public int q1TasksCount { get; set; }
        public int q2TasksCount { get; set; }
        public int q3TasksCount { get; set; }
        public int q4TasksCount { get; set; }
        public double TasksCompletionPercent { get => (double)TasksDone / TasksCount; }
        public double TasksExpiredPercent{ get => (double)TasksExpired / TasksCount; }
    }
}
