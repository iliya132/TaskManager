using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TaskManager_redesign.Model.DataProviders.Interfaces
{
    public interface IDataProvider
    {
        IEnumerable<UserTask> GetTasks();
        IEnumerable<Status> GetStatuses();
        IEnumerable<Analytic> GetAnalytics();
        Analytic GetUser(string userName);
        void UpdateField(string field, object newValue, int TaskId);
        void AddNewTask(UserTask newTask);
        void UpdateTaskToAnalytic(int id1, int id2, string comment, Status status);
        void DeleteAssignedAnalytic(TaskToAnalytic obj);
        TaskToAnalytic AddNewAssignedAnalytic(Analytic analytic, UserTask selectedItem);
        void RemoveTask(UserTask task);
        TaskPlan AddPlanStep(UserTask task, Analytic created_by, string name, DateTime dueDate);
        void ChangePlanStatus(TaskPlan plan);
        void RemovePlan(TaskPlan plan);
        void OrderByDate(ObservableCollection<TaskPlan> collection);
    }
}
