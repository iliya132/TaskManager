using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using TaskManager_redesign.Model.DataProviders.Interfaces;

namespace TaskManager_redesign.Model.DataProviders.Implementations
{
    public class QuarterReport : IReport
    {
        public DataTable LoadData(object input)
        {
            if(!(input is List<UserTask> tasks))
            {
                throw new FormatException("Получен недопустимый аргумент.");
            }
            DataTable result = new DataTable("ResultTable");
            Dictionary<UserTask, int> TaskLeveled = GenerateTaskLevels(tasks.Where(tsk => tsk.ParentTask == null));
            int maxLevel = TaskLeveled.Values.Max();
            for (int i = 0; i <= maxLevel; i++)
            {
                result.Columns.Add($"Level{i + 1}");
            }
            result.Columns.Add("AssignedTo", typeof(string));
            result.Columns.Add("Start", typeof(DateTime));
            result.Columns.Add("End", typeof(DateTime));
            for (int i = 0; i < 4; i++)
                result.Columns.Add($"Q{i+1}");
            
            
            foreach(var item in TaskLeveled)
            {
                DataRow newRow = result.Rows.Add();
                newRow[item.Value] = item.Key.Name;
                newRow["AssignedTo"] = string.Join("\r\n", item.Key.AssignedTo.Select(i=>$"{ i.Analytic.LastName} {i.Analytic.FirstName} {i.Analytic.FatherName}"));
                newRow["Start"] = item.Key.StartDate;
                
                newRow["End"] = item.Key.DueDate;
                if(item.Key.TaskPlans.Count > 0)
                {
                    foreach(TaskPlan plan in item.Key.TaskPlans.OrderBy(i=>i.DueDate))
                    {
                        short monthN = (short)plan.DueDate.Month;
                        string planText;
                        if (plan.IsDone)
                        {
                            planText = $"{char.ConvertFromUtf32(0x2713)} {plan.DueDate:dd.MM.yyyy}: '{plan.Description}'";
                        }
                        else
                        {
                            planText = $"{char.ConvertFromUtf32(0x29D6)} {plan.DueDate:dd.MM.yyyy}: '{plan.Description}'";
                        }
                     
                        short planQuarter = (short)Math.Ceiling((double)monthN/3);
                    
                        newRow[$"Q{planQuarter}"] += $"{planText}\r\n";

                    }
                }
                else if(item.Key.ChildTasks.Count > 0)
                {
                    foreach(UserTask childTask in item.Key.ChildTasks)
                    {
                        short monthN = (short)childTask.DueDate.Month;
                        string childTaskFinalText;
                        string childTaskToText;
                        string childTaskStatusDot;
                        childTaskToText = string.IsNullOrWhiteSpace(childTask.AwaitedResult) ? childTask.Name : childTask.AwaitedResult;
                        childTaskStatusDot = childTask.Status.Name.Equals("Завершена") ? char.ConvertFromUtf32(0x2713) : char.ConvertFromUtf32(0x29D6);
                        childTaskFinalText = $"{childTaskStatusDot} {childTask.DueDate:dd.MM.yyyy}: '{childTaskToText}'";
                        short planQuarter = (short)Math.Ceiling((double)monthN / 3);
                        newRow[$"Q{planQuarter}"] += $"{childTaskFinalText}\r\n";
                    }
                }
            }
            return result;
        }

        private Dictionary<UserTask, int> GenerateTaskLevels(IEnumerable<UserTask> tasks, int startLevel = 0)
        {
            Dictionary<UserTask, int> result = new Dictionary<UserTask, int>();
            foreach(UserTask task in tasks)
            {
                result.Add(task, startLevel);
                if(task.ChildTasks.Count > 0)
                {
                    foreach(var item in GenerateTaskLevels(task.ChildTasks, startLevel + 1)){
                        result.Add(item.Key, item.Value);
                    }
                }
            }
            return result;
        }

        
    }
}
