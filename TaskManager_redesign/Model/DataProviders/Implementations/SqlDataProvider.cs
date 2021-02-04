using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;

using TaskManager_redesign.Model.DataProviders.Interfaces;

namespace TaskManager_redesign.Model.DataProviders.Implementations
{
    public class SqlDataProvider : IDataProvider
    {
        private readonly SqlConnection connection;
        private List<Status> Statuses;
        private List<Analytic> Analytics;
        private List<TaskToAnalytic> TaskToAnalytics;
#if development
        private readonly string connectionString = @"Data Source=ILYAHOME\MYDB;Initial Catalog=TaskManager;MultipleActiveResultSets=True;Integrated Security=True";
#else
        private readonly string connectionString = @"Data Source=a105512\a105512;Initial Catalog=TaskManagerRedesigned;MultipleActiveResultSets=true;Integrated Security=True"; //Place Connection String here
#endif
        public SqlDataProvider()
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
            InitializeData();
        }

        private void InitializeData()
        {

            Statuses = new List<Status>(GetStatuses());
            Analytics = new List<Analytic>(GetAnalytics());
            TaskToAnalytics = new List<TaskToAnalytic>(GetTasksToAnalytics());
        }

       

        ~SqlDataProvider(){
            try
            {
                connection.Close();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
            }
        }

        public IEnumerable<UserTask> GetTasks()
        {
            List<UserTask> result = new List<UserTask>();
            
            string query = "select id, created_at, due_date, created_by, awaited_result, description, status, parent_task, subject from Tasks;";
            SqlCommand getTasksCommand = new SqlCommand(query, connection);
            using (SqlDataReader reader = getTasksCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    UserTask readedTask = new UserTask
                    {
                        Id = reader.GetInt32(0),
                        CreatedAt = reader.GetDateTime(1),
                        DueDate = reader.GetDateTime(2),
                        CreatedById = reader.GetInt32(3),
                        AwaitedResult = reader.GetString(4),
                        TaskDescription = reader.GetString(5),
                        StatusId = reader.GetInt32(6),
                        Name = reader.GetString(8)
                    };
                    if (!reader.IsDBNull(7))
                    {
                        readedTask.ParentTaskId = reader.GetInt32(7);
                    }
                    readedTask.CreatedBy = Analytics.SingleOrDefault(a => a.Id == readedTask.CreatedById);
                    readedTask.Status = Statuses.SingleOrDefault(a => a.Id == readedTask.StatusId);

                    readedTask.TaskPlans = new ObservableCollection<TaskPlan>(GetTaskPlans(readedTask.Id));
                    foreach (TaskPlan plan in readedTask.TaskPlans)
                    {
                        plan.Task = readedTask;
                    }



                    result.Add(readedTask);
                }
                foreach(UserTask task in result)
                {
                    if (task.ParentTaskId.HasValue)
                    {
                        task.ParentTask = result.SingleOrDefault(tsk => tsk.Id == task.ParentTaskId);
                    }
                    task.ChildTasks = new ObservableCollection<UserTask>(result.Where(tsk => tsk.ParentTaskId == task.Id));
                    task.AssignedTo = new ObservableCollection<TaskToAnalytic>(TaskToAnalytics.Where(i => i.TaskId == task.Id));
                }
            }
            return result;
        }

        public IEnumerable<UserTask> GetAssignedToTasks(Analytic user)
        {
            List<UserTask> result = new List<UserTask>();

            string query = "select t.id, created_at, due_date, created_by, awaited_result, description, t.status, parent_task, subject from Tasks as t"+
                            "left join TasksToAnalytics as tta on tta.task = t.id" +
                            "where tta.analytic = @user;";
            SqlCommand getTasksCommand = new SqlCommand(query, connection);
            getTasksCommand.Parameters.AddWithValue("@user", user.Id);
            using (SqlDataReader reader = getTasksCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    UserTask readedTask = new UserTask
                    {
                        Id = reader.GetInt32(0),
                        CreatedAt = reader.GetDateTime(1),
                        DueDate = reader.GetDateTime(2),
                        CreatedById = reader.GetInt32(3),
                        AwaitedResult = reader.GetString(4),
                        TaskDescription = reader.GetString(5),
                        StatusId = reader.GetInt32(6),
                        Name = reader.GetString(8)
                    };
                    if (!reader.IsDBNull(7))
                    {
                        readedTask.ParentTaskId = reader.GetInt32(7);
                    }
                    readedTask.CreatedBy = Analytics.SingleOrDefault(a => a.Id == readedTask.CreatedById);
                    readedTask.Status = Statuses.SingleOrDefault(a => a.Id == readedTask.StatusId);

                    readedTask.TaskPlans = new ObservableCollection<TaskPlan>(GetTaskPlans(readedTask.Id));
                    foreach (TaskPlan plan in readedTask.TaskPlans)
                    {
                        plan.Task = readedTask;
                    }
                    result.Add(readedTask);
                }
                foreach (UserTask task in result)
                {
                    if (task.ParentTaskId.HasValue)
                    {
                        task.ParentTask = result.SingleOrDefault(tsk => tsk.Id == task.ParentTaskId);
                    }
                    task.ChildTasks = new ObservableCollection<UserTask>(result.Where(tsk => tsk.ParentTaskId == task.Id));
                    task.AssignedTo = new ObservableCollection<TaskToAnalytic>(TaskToAnalytics.Where(i => i.TaskId == task.Id));
                }
            }
            return result;
        }

        public IEnumerable<Status> GetStatuses()
        {
            List<Status> result = new List<Status>();
            string query = "select * from Statuses;";
            SqlCommand getStatuses = new SqlCommand(query, connection);
            using (SqlDataReader reader = getStatuses.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new Status
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
            return result;
        }

        public IEnumerable<Analytic> GetAnalytics()
        {
            List<Analytic> result = new List<Analytic>();
            string query = "select * from TimeSheet.dbo.Analytic;";
            SqlCommand getAnalytics = new SqlCommand(query, connection);
            using (SqlDataReader reader = getAnalytics.ExecuteReader())
            {
                while (reader.Read())
                {
                    Analytic readedAnalytic = new Analytic
                    {
                        Id = reader.GetInt32(0),
                        UserName = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        LastName = reader.GetString(3),
                        FatherName = reader.GetString(4),
                        DepartmentId = reader.GetInt32(5),
                        DirectionId = reader.GetInt32(6),
                        UpravlenieId = reader.GetInt32(7),
                        OtdelId = reader.GetInt32(8),
                        PositionsId = reader.GetInt32(9),
                        RoleTableId = reader.GetInt32(10)
                    };
                    if (!reader.IsDBNull(11))
                    {
                        readedAnalytic.HeadAdmId = reader.GetInt32(11);
                    }
                    if (!reader.IsDBNull(12))
                    {
                        readedAnalytic.HeadFuncId = reader.GetInt32(12);
                    }
                    if (!reader.IsDBNull(13))
                    {
                        readedAnalytic.Deleted_Flag = reader.GetBoolean(13);
                    }
                    result.Add(readedAnalytic);
                }
                foreach(Analytic analytic in result)
                {
                    if (analytic.HeadAdmId.HasValue)
                    {
                        analytic.AdminHead = result.SingleOrDefault(a => a.Id == analytic.HeadAdmId);
                    }
                    if (analytic.HeadFuncId.HasValue)
                    {
                        analytic.FunctionHead = result.SingleOrDefault(a => a.Id == analytic.HeadFuncId);
                    }
                }
            }
            return result;
        }

        private IEnumerable<TaskToAnalytic> GetTasksToAnalytics()
        {
            List<TaskToAnalytic> result = new List<TaskToAnalytic>();
            string query = "select id, task, analytic, comment, status from TasksToAnalytics;";
            SqlCommand getTasksToAnalytics = new SqlCommand(query, connection);
            using (SqlDataReader reader = getTasksToAnalytics.ExecuteReader())
            {

                while (reader.Read())
                {
                    TaskToAnalytic newTTA = new TaskToAnalytic
                    {
                        Id = reader.GetInt32(0),
                        TaskId = reader.GetInt32(1),
                        AnalyticId = reader.GetInt32(2),
                        StatusId = reader.GetInt32(4)

                    };
                    newTTA.Status = Statuses.Single(i => i.Id == newTTA.StatusId);
                    newTTA.Analytic = Analytics.Single(i => i.Id == newTTA.AnalyticId);
                    if (!reader.IsDBNull(3))
                    {
                        newTTA.Comment = reader.GetString(3);
                    }

                    result.Add(newTTA);
                }
            }
            return result;
        }

        public void UpdateField(string field, object newValue, int TaskId)
        {
            string sqlQuery = $"update Tasks set {field} = @newValue Where id = @TaskId;";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@newValue", newValue);
            command.Parameters.AddWithValue("@TaskId", TaskId);
            command.ExecuteNonQuery();
        }

        public void AddNewTask(UserTask newTask)
        {
            string sqlQuery = "insert into Tasks (subject, due_date, created_by, awaited_result, description, parent_task) values (@subj, @duedate, @createdby, @awaitedresult, @description, @parent_task);SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@subj", newTask.Name);
            command.Parameters.AddWithValue("@duedate", newTask.DueDate);
            command.Parameters.AddWithValue("@createdby", newTask.CreatedById);
            command.Parameters.AddWithValue("@awaitedresult", newTask.AwaitedResult);
            command.Parameters.AddWithValue("@description", newTask.TaskDescription);
            command.Parameters.AddWithValue("@parent_task", (object)newTask.ParentTaskId ?? SqlInt32.Null);
            int newId = Convert.ToInt32(command.ExecuteScalar());
            newTask.Id = newId;
        }

        public Analytic GetUser(string userName)
        {
            return Analytics.SingleOrDefault(a => a.UserName.ToLower().Equals(userName.ToLower()));
        }

        public void UpdateTaskToAnalytic(int analyticId, int taskId, string comment, Status status)
        {
            string sqlQuery = "update TasksToAnalytics set comment = @comment, status = @status where analytic = @analytic and task = @task";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@comment", comment);
            command.Parameters.AddWithValue("@status", status.Id);
            command.Parameters.AddWithValue("@analytic", analyticId);
            command.Parameters.AddWithValue("@task", taskId);
            command.ExecuteNonQuery();
        }

        public void DeleteAssignedAnalytic(TaskToAnalytic obj)
        {
            string sqlQuery = "delete TasksToAnalytics where id = @id";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@id", obj.Id);
            command.ExecuteNonQuery();
        }

        public TaskToAnalytic AddNewAssignedAnalytic(Analytic analytic, UserTask relatedTask)
        {
            string checkConstraint = "select count(*) from TasksToAnalytics where task = @task_id and analytic = @analytic_id;";
            SqlCommand command = new SqlCommand(checkConstraint, connection);
            command.Parameters.AddWithValue("@task_id", relatedTask.Id);
            command.Parameters.AddWithValue("@analytic_id", analytic.Id);
            int sameRecordCount = Convert.ToInt32(command.ExecuteScalar());
            if (sameRecordCount > 0)
            {
                throw new SqlAlreadyFilledException("Same record already exists");
            }

            string sqlQuery = "insert TasksToAnalytics (task, analytic) values (@task_id, @analytic_id); Select id, task, analytic from TasksToAnalytics where id = (Select SCOPE_IDENTITY());";
            command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@task_id", relatedTask.Id);
            command.Parameters.AddWithValue("@analytic_id", analytic.Id);
            TaskToAnalytic tta = new TaskToAnalytic();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tta.Id = reader.GetInt32(0);
                    tta.TaskId = reader.GetInt32(1);
                    tta.AnalyticId = reader.GetInt32(2);
                    tta.Analytic = Analytics.SingleOrDefault(i => i.Id == tta.AnalyticId);
                    tta.StatusId = 1;
                    tta.Status = Statuses[0];
                }
            }
            return tta;
        }

        public void RemoveTask(UserTask task)
        {
            RemoveTaskRecoursive(task);
        }

        private void RemoveTaskRecoursive(UserTask task)
        {
            while (task.ChildTasks.Count > 0)
            {
                RemoveTaskRecoursive(task.ChildTasks[0]);
                task.ChildTasks.RemoveAt(0);
            }
            RemoveTasksPlansByTaskId(task.Id);
            RemoveTasksToAnalyticsByTaskId(task.Id);
            string sqlQuery = "Delete Tasks where id=@task";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@task", task.Id);
            command.ExecuteNonQuery();

        }

        private void RemoveTasksToAnalyticsByTaskId(int task_id)
        {
            string sqlQuery = "Delete TasksToAnalytics where task= @task;";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@task", task_id);
            command.ExecuteNonQuery();
        }

        private void RemoveTasksPlansByTaskId(int task_id)
        {
            string sqlQuery = "Delete TaskPlan where task= @task;";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@task", task_id);
            command.ExecuteNonQuery();
        }

        public TaskPlan AddPlanStep(UserTask task, Analytic created_by, string name, DateTime dueDate)
        {
            string sqlQuery = "insert into TaskPlan (task, description, created_by, due_date) values (@task, @description, @created_by, @due_date); Select SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@task", task.Id);
            command.Parameters.AddWithValue("@description", name);
            command.Parameters.AddWithValue("@created_by", created_by.Id);
            command.Parameters.AddWithValue("@due_date", dueDate);
            int newId = Convert.ToInt32(command.ExecuteScalar());
            TaskPlan newTP = new TaskPlan
            {
                Id = newId,
                CreatedAt = DateTime.Now,
                CreatedBy = created_by,
                CreatedById = created_by.Id,
                Description = name,
                DueDate = dueDate,
                Task = task,
                TaskId = task.Id
            };
            return newTP;
        }

        private IEnumerable<TaskPlan> GetTaskPlans(int task_id)
        {
            List<TaskPlan> result = new List<TaskPlan>();
            string query = "select id, task, description, created_at, created_by, due_date, is_done from TaskPlan where task = @task order by due_date;";
            SqlCommand getPlans = new SqlCommand(query, connection);
            getPlans.Parameters.AddWithValue("@task", task_id);
            using (SqlDataReader reader = getPlans.ExecuteReader())
            {

            
                while (reader.Read())
                {
                    TaskPlan newPlan = new TaskPlan
                    {
                        Id = reader.GetInt32(0),
                        TaskId = reader.GetInt32(1),
                        Description = reader.GetString(2),
                        CreatedAt = reader.GetDateTime(3),
                        CreatedById = reader.GetInt32(4),
                        DueDate = reader.GetDateTime(5),
                        IsDone = reader.GetBoolean(6)

                    };
                    newPlan.CreatedBy = Analytics.SingleOrDefault(i => i.Id == newPlan.CreatedById);
                    result.Add(newPlan);
                }
            }
            return result;
        }

        public void ChangePlanStatus(TaskPlan plan)
        {
            string sqlQuery = "update TaskPlan set is_done = @is_done where id = @id;";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@is_done", plan.IsDone);
            command.Parameters.AddWithValue("@id", plan.Id);
            command.ExecuteNonQuery();
        }

        public void RemovePlan(TaskPlan plan)
        {
            string sqlQuery = "delete from TaskPlan where id = @id;";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@id", plan.Id);
            command.ExecuteNonQuery();
        }

        public void OrderByDate(ObservableCollection<TaskPlan> collection)
        {
            List<TaskPlan> tempPlans = new List<TaskPlan>(collection);
            tempPlans = tempPlans.OrderBy(i => i.DueDate).ToList();
            collection.Clear();
            tempPlans.ForEach(collection.Add);
        }
    }
}
