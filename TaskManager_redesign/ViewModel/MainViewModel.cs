﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager_redesign.Miscellaneous;
using TaskManager_redesign.Model;
using TaskManager_redesign.Model.DataProviders.Implementations;
using TaskManager_redesign.Model.DataProviders.Interfaces;
using TaskManager_redesign.ViewModel.Commands;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace TaskManager_redesign.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region MVVM_realisation
        public static MainViewModel _instance;
        public event PropertyChangedEventHandler PropertyChanged;

        public static MainViewModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MainViewModel();
            }
            return _instance;
        }
        private MainViewModel()
        {
            dataProvider = new SqlDataProvider();
            InitializeData();
            InitializeCommands();
            InitializeViewModels();
            InitializeEvents();
        }
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region DataCollections
        public IDataProvider dataProvider;
        public List<UserTask> AllTasks { get; set; } = new List<UserTask>();
        public ObservableCollection<UserTask> UserTasks { get; set; }
        public List<Status> Statuses { get; set; }
        public Dictionary<int, List<Analytic>> AnalyticsAtStructures;
        #endregion

        #region Data collections methods
        private void MarkSelectedTask(int id, IEnumerable<UserTask> childTasks)
        {
            foreach (UserTask tsk in childTasks)
            {
                if (tsk.Id == id && tsk.IsSelected == false)
                {
                    tsk.IsSelected = true;
                    Dispatcher.Yield();
                }
                else if (tsk.Id != id && tsk.IsSelected == true)
                {
                    tsk.IsSelected = false;
                    Dispatcher.Yield();
                }
                MarkSelectedTask(id, tsk.ChildTasks);
            }
        }

        private UserTask GetSelectedTask(IEnumerable<UserTask> childTasks)
        {
            foreach (UserTask tsk in childTasks)
            {
                if (tsk.IsSelected)
                {
                    return tsk;
                }
                else
                {
                    return GetSelectedTask(tsk.ChildTasks);
                }
            }
            return null;
        }
        #endregion

        #region technical_variables
        private bool SelectedItemChangedCodeBehind { get; set; }
        bool IsRedrawActive { get; set; }
        public bool CreateOutside { get; set; }
        public enum TaskFilter
        {
            AllTasks,
            AssignedToMe,
            ReportedByMe,
            MyStructure
        }
        public TaskFilter CurrentTaskFilter { get; set; } = TaskFilter.AllTasks;
        #endregion

        #region ViewProperties
        public Visibility FinishButtonVisibility
        {
            get
            {
                if (SelectedItem == null || SelectedItem.AssignedTo == null)
                    return Visibility.Collapsed;
                if(SelectedItem.AssignedTo.Any(i=>i.Analytic.Id == CurrentAnalytic.Id && i.Status.Name.Equals("В работе")))
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility PropertiesVisibility
        {
            get
            {
                if (SelectedItem == null)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }

        public Visibility AssignBtnVisibility
        {
            get
            {
                if(SelectedItem.CreatedBy.Id == CurrentAnalytic.Id)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility TasksFilteredVisibility { get; set; } = Visibility.Collapsed;
        public Visibility AllTaskTreeVisibility { get; set; }



        public string CurrentViewName { get; set; } = "Все задачи";
        #endregion

        #region currentValues
        private UserTask _selectedItem;
        public UserTask SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (!IsRedrawActive && value!=_selectedItem && value != null)
                {
                    _selectedItem = value;
                    RaisePropertyChanged(nameof(SelectedItem));
                }
                
                IsRedrawActive = false;
            }
        }
        public Analytic CurrentAnalytic { get; set; }
        public bool IsCreateAsParentModeActive { get; set; }
        #region Статистика
        public int OverallTasks
        {
            get
            {
                if(CurrentViewName.Equals("Все задачи"))
                {
                    return AllTasks.Count;
                }
                else
                {
                    return UserTasks.Count;
                }
            }
        }

        public int OverallTaskPlans
        {
            get
            {
                if (CurrentViewName.Equals("Все задачи"))
                {
                    return AllTasks.Select(i=>i.TaskPlans.Count).Sum();
                }
                else
                {
                    return UserTasks.Select(i => i.TaskPlans.Count).Sum();
                }
            }
        }

        public int TasksComplete
        {
            get
            {
                if(CurrentViewName.Equals("Все задачи"))
                {
                    return AllTasks.Where(i => i.StatusId == 2).Count();
                }
                else
                {
                    return UserTasks.Where(i => i.StatusId == 2).Count();
                }
            }
        }

        public int TaskPlansComplete
        {
            get
            {
                if (CurrentViewName.Equals("Все задачи"))
                {
                    return AllTasks.Select(i=>i.TaskPlans.Where(j=>j.IsDone).Count()).Sum();
                }
                else
                {
                    return UserTasks.Select(i => i.TaskPlans.Where(j => j.IsDone).Count()).Sum();
                }
            }
        }

        public int TasksExpired
        {
            get
            {
                if(CurrentViewName.Equals("Все задачи"))
                {
                    return AllTasks.Where(i => i.DueDate.CompareTo(DateTime.Today) < 0 && i.StatusId == 1).Count();
                }
                else
                {
                    return UserTasks.Where(i => i.DueDate.CompareTo(DateTime.Today) < 0 && i.StatusId == 1).Count();
                }
            }
        }

        public int TaskPlansExpired
        {
            get
            {
                if (CurrentViewName.Equals("Все задачи"))
                {
                    return AllTasks.Select(i => i.TaskPlans.Where(j => j.DueDate.CompareTo(DateTime.Today) < 0 && !j.IsDone).Count()).Sum();
                }
                else
                {
                    return UserTasks.Select(i => i.TaskPlans.Where(j => j.DueDate.CompareTo(DateTime.Today) < 0 && !j.IsDone).Count()).Sum();
                }
            }
        }

        private List<AnalyticStatistic> _tasksOnAnalytics;
        public List<AnalyticStatistic> TasksOnAnalytics
        {
            get => _tasksOnAnalytics;
            set => _tasksOnAnalytics = value;
        }

        private void UpdateStatistic()
        {
            List<AnalyticStatistic> result = new List<AnalyticStatistic>();
            List<Analytic> analyticsOfInterest = GetCurrentFilterAnalytics();
            List<UserTask> tasks;
            
            if (CurrentTaskFilter == TaskFilter.AllTasks)
            {
                tasks = AllTasks;
            }
            else
            {
                tasks = AllTasks.Intersect(UserTasks).ToList();
            }



            foreach (UserTask tsk in tasks)
            {
                int quarter = (int)Math.Ceiling((double)tsk.DueDate.Month / 3);
                int taskPlansExpired = tsk.TaskPlans.Where(i => i.DueDate.CompareTo(DateTime.Today) < 0 && !i.IsDone).Count(); ;
                
                
                foreach (Analytic analytic in tsk.AssignedTo.Select(i => i.Analytic))
                {
                    if (!analyticsOfInterest.Contains(analytic))
                    {
                        continue;
                    }
                    if (!result.Any(i => i.Name.Equals(analytic.ToString())))
                    {
                        result.Add(new AnalyticStatistic
                        {
                            Name = analytic.ToString()
                        });
                    }
                    AnalyticStatistic record = result.Single(i => i.Name.Equals(analytic.ToString()));
                    if (tsk.StatusId == 2)
                    {
                        record.TasksDone++;
                    }
                    record.TasksCount++;
                    record.TaskPlansCount += tsk.TaskPlans.Count;
                    record.TaskPlansDone += tsk.TaskPlans.Where(i => i.UpdatedById == CurrentAnalytic.Id && i.IsDone).Count();
                    switch (quarter)
                    {
                        case 1:
                            record.q1TasksCount++;
                            record.q1TaskPlansCount += tsk.TaskPlans.Count;
                            break;
                        case 2:
                            record.q2TasksCount++;
                            record.q2TaskPlansCount += tsk.TaskPlans.Count;
                            break;
                        case 3:
                            record.q3TasksCount++;
                            record.q3TaskPlansCount += tsk.TaskPlans.Count;
                            break;
                        case 4:
                            record.q4TasksCount++;
                            record.q4TaskPlansCount += tsk.TaskPlans.Count;
                            break;
                    }
                    if (tsk.DueDate.CompareTo(DateTime.Today) < 0 && tsk.AssignedTo.Single(i=>i.AnalyticId==analytic.Id).StatusId == 1)
                    {
                        record.TasksExpired++;
                    }
                    record.TaskPlansExpired += taskPlansExpired;
                }
            }

            TasksOnAnalytics = result.OrderBy(i => i.Name).ToList();
        }

        private List<Analytic> GetCurrentFilterAnalytics()
        {
            List<Analytic> result = new List<Analytic>();
            if(CurrentTaskFilter == TaskFilter.AllTasks)
            {
                foreach (var item in AllTasks.Select(i => i.AssignedTo))
                {
                    result.AddRange(item.Select(j => j.Analytic));
                }
            }else if (CurrentTaskFilter == TaskFilter.AssignedToMe)
            {
                result.Add(CurrentAnalytic);
            }else if (CurrentTaskFilter == TaskFilter.ReportedByMe)
            {
                foreach(UserTask reportedByMeTask in AllTasks.Where(i=>i.CreatedById == CurrentAnalytic.Id))
                {
                    foreach(var item in AllTasks.Select(i => i.AssignedTo))
                    {
                        result.AddRange(item.Select(j => j.Analytic));
                    }
                }
            }else if(CurrentTaskFilter == TaskFilter.MyStructure)
            {
                foreach(var item in AnalyticsAtStructures.Where(j=>StructuresFiltered.Any(i=>i==j.Key)))
                {
                    result.AddRange(item.Value);
                }
            }
            return result;
        }

        private double CalculateSumRecoursive(Func<UserTask, double> func, UserTask task)
        {
            double result = func(task);
            foreach (UserTask childTask in task.ChildTasks)
            {
                result += CalculateSumRecoursive(func, childTask);
            }
            return result;
        }



        private void OnStatisticChanged()
        {
            RaisePropertyChanged(nameof(OverallTasks));
            RaisePropertyChanged(nameof(OverallTaskPlans));
            RaisePropertyChanged(nameof(TasksComplete));
            RaisePropertyChanged(nameof(TaskPlansComplete));
            RaisePropertyChanged(nameof(TasksExpired));
            RaisePropertyChanged(nameof(TaskPlansExpired));
            RaisePropertyChanged(nameof(TasksOnAnalytics));
        }
        #endregion
        #endregion

        #region Commands
        public TriggerCommand<string> FieldUpdated { get; set; }
        public TriggerCommand<TaskPlan> PlanStatusChanged { get; set; }
        public TriggerCommand<TaskPlan> RemovePlan { get; set; }
        public TriggerCommand<string> AddNewTask { get; set; }
        public TriggerCommand<UserTask> SelectedTaskChanged { get; set; }
        public TriggerCommand<string> CompleteTheTask { get; set; }
        public TriggerCommand<TaskToAnalytic> DeleteAssignedEmployee { get; set; }
        public TriggerCommand<Analytic> AddAssignedAnalytic { get; set; }
        public TriggerCommand<UserTask> RemoveTask { get; set; }
        public TriggerCommand AddNewPlanStep { get; set; }
        public TriggerCommand<string> SetFilterState { get; set; }
        public TriggerCommand<(UserTask, UserTask)> DragNDrop { get; set; }
        public TriggerCommand<TaskPlan> UpdatePlan { get; set; }
        public TriggerCommand<string> ChangeStructureFilter { get; set; }
        public TriggerCommand TreeReportCommand { get; set; }
        public TriggerCommand ShowReportWindow { get; set; }
        public TriggerCommand<UserTask> CopyTask { get; set; }
        public TriggerCommand<UserTask> PasteTask { get; set; }
        public TriggerCommand CollapseAllCommand { get; set; }
        #endregion

        #region initialize MVVM
        private void InitializeCommands()
        {
            FieldUpdated = new TriggerCommand<string>(HandleFieldUpdate);
            PlanStatusChanged = new TriggerCommand<TaskPlan>(HandlePlanChangedStatus);
            RemovePlan = new TriggerCommand<TaskPlan>(HandleRemovePlan);
            AddNewTask = new TriggerCommand<string>(HandleAddTaskFieldUpdate);
            SelectedTaskChanged = new TriggerCommand<UserTask>(HandleSelectedTaskChanged);
            CompleteTheTask = new TriggerCommand<string>(HandleFinishCommandAction);
            DeleteAssignedEmployee = new TriggerCommand<TaskToAnalytic>(HandleDeleteCommandAction);
            AddAssignedAnalytic = new TriggerCommand<Analytic>(HandleAddAssignedAnalyticAction);
            RemoveTask = new TriggerCommand<UserTask>(HandleDeleteTaskAction);
            AddNewPlanStep = new TriggerCommand(HandleAddPlanStepAction);
            SetFilterState = new TriggerCommand<string>(HandleTaskFilterStateChanged);
            DragNDrop = new TriggerCommand<(UserTask, UserTask)>(HandleDragAndDropAction);
            UpdatePlan = new TriggerCommand<TaskPlan>(HandleTaskPlanUpdate);
            ChangeStructureFilter = new TriggerCommand<string>(HandleStructFilterChanged);
            TreeReportCommand = new TriggerCommand(HandleTreeReport);
            ShowReportWindow = new TriggerCommand(HandleReportBtnClicked);
            CopyTask = new TriggerCommand<UserTask>(HandleCopyTask);
            PasteTask = new TriggerCommand<UserTask>(HandlePasteTask);
            CollapseAllCommand = new TriggerCommand(HandleCollapseAll);
        }

        private void HandleCollapseAll()
        {
            foreach(UserTask task in AllTasks)
            {
                CollapseRecoursive(task);
            }
        }

        private void CollapseRecoursive(UserTask task)
        {
            if (task.IsExpanded)
            {
                task.IsExpanded = false;
            }
            foreach(UserTask childTask in task.ChildTasks)
            {
                CollapseRecoursive(childTask);
            }
        }

        private void HandlePasteTask(UserTask obj)
        {
            if (CopiedTask == null)
            {
                return;
            }
            HandleDragAndDropAction((CopiedTask, obj));
        }

        UserTask CopiedTask;
        private void HandleCopyTask(UserTask obj)
        {
            CopiedTask = obj;
        }

        private void HandleReportBtnClicked()
        {
            throw new NotImplementedException();
        }

        private void HandleTreeReport()
        {
            IReport report = new QuarterReport();
            System.Data.DataTable reportResult = report.LoadData(AllTasks);
            IExporter exporter = new ExcelExporter();
            string reportFileName = exporter.Export(reportResult);
            //TODO вынести запуск процесса в другое место
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo(reportFileName)
            {
                UseShellExecute = true
            };
            proc.Start();
        }

        private void InitializeData()
        {
            this.PropertyChanged += MainViewModel_PropertyChanged;
            AllTasks = new List<UserTask>(dataProvider.GetTasks());
            UserTasks = new ObservableCollection<UserTask>(AllTasks.Where(tsk => tsk.ParentTask == null));
            SelectedItem = UserTasks.Count > 0 ? UserTasks[0] : null;
            Statuses = new List<Status>(dataProvider.GetStatuses());
            CurrentAnalytic = dataProvider.GetUser(Environment.UserName);
            AnalyticsAtStructures = dataProvider.GenerateAnalyticsTostructures();
            UpdateStatistic();
        }

        private void InitializeViewModels()
        {
            SelectEmployeeViewModel sevm = SelectEmployeeViewModel.GetInstance();
            sevm.AvailableAnalytics = new ObservableCollection<Analytic>(dataProvider.GetAnalytics());
        }

        private void InitializeEvents()
        {
            AfterTaskFilteredEvent += () => UpdateStatistic();

        }

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(SelectedItem)))
            {
                RaisePropertyChanged(nameof(FinishButtonVisibility));
                RaisePropertyChanged(nameof(PropertiesVisibility));
                RaisePropertyChanged(nameof(AssignBtnVisibility));
            }
        }
        #endregion

        #region Commands implementations
        private void HandleDeleteCommandAction(TaskToAnalytic obj)
        {
            if(MessageBox.Show("Вы уверены что хотите удалить ответственного?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                dataProvider.DeleteAssignedAnalytic(obj);
                SelectedItem.AssignedTo.Remove(obj);
                RaisePropertyChanged(nameof(FinishButtonVisibility));
            }
        }

        private void HandleFinishCommandAction(string comment)
        {
            Status doneStatus = Statuses.SingleOrDefault(i => i.Name.Equals("Завершена"));
            Status newStatus = dataProvider.UpdateTaskToAnalytic(CurrentAnalytic.Id, SelectedItem.Id, comment, doneStatus);
            TaskToAnalytic tta = SelectedItem.AssignedTo.SingleOrDefault(i => i.AnalyticId == CurrentAnalytic.Id);
            tta.Status = doneStatus;
            tta.StatusId = doneStatus.Id;
            tta.Comment = comment;
            if(SelectedItem.Status != newStatus)
            {
                SelectedItem.Status = newStatus;
            }
        }

        private void HandleSelectedTaskChanged(UserTask obj)
        {
            SelectedItem = obj;
        }

        private void HandleFieldUpdate(string field)
        {
            if (SelectedItem != null)
            {
                Type taskType = typeof(UserTask);
                PropertyInfo fieldInfo = taskType.GetProperty(field);
                string attributeValue = fieldInfo.GetCustomAttribute<ColumnAttribute>().Name;
                object newValue = fieldInfo.GetValue(SelectedItem);
                dataProvider.UpdateField(attributeValue, newValue, SelectedItem.Id);
            }
        }

        private void HandleAddTaskFieldUpdate(string newTaskSubject)
        {
            if (string.IsNullOrWhiteSpace(newTaskSubject))
            {
                return;
            }
            SelectedItemChangedCodeBehind = true;
            UserTask oldTask = SelectedItem;
            DateTime createdDT = DateTime.Now;
            UserTask newTask = new UserTask
            {
                Name = newTaskSubject,
                CreatedBy = CurrentAnalytic,
                AwaitedResult = string.Empty,
                TaskDescription = string.Empty,
                CreatedById = CurrentAnalytic.Id,
                CreatedAt = createdDT,
                DueDate = createdDT,
                StartDate = createdDT,
                Status = Statuses.Single(i => i.Name.Equals("В работе")),
                StatusId = Statuses.Single(i => i.Name.Equals("В работе")).Id
            };
            if (CreateOutside || SelectedItem == null || IsCreateAsParentModeActive)
            {
                
                UserTasks.Add(newTask);
            }
            else
            {
                oldTask.IsExpanded = true;
                newTask.ParentTask = oldTask;
                newTask.ParentTaskId = oldTask.Id;
                oldTask.AddChildAndSort(newTask);
            }
            SelectedItem = newTask;
            AllTasks.Add(newTask);
            dataProvider.AddNewTask(newTask);
         
        }

        private void HandleAddAssignedAnalyticAction(Analytic analytic)
        {
            try
            {
                TaskToAnalytic tta = dataProvider.AddNewAssignedAnalytic(analytic, SelectedItem);
                SelectedItem.AssignedTo.Add(tta);
            }
            catch (SqlAlreadyFilledException)
            {
                MessageBox.Show("Этот аналитик уже ответственен за эту задачу", "Ошибка при добавлении", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception)
            {
                MessageBox.Show("При добавлении ответственного возникла непредвиденная ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void HandleDeleteTaskAction(UserTask task)
        {
            if(task.ChildTasks.Count > 0)
            {
                MessageBoxResult boxResult = MessageBox.Show("Удаляемая задача содержит подзадачи. Они будут удалены вместе с удаляемой задачей!\r\nВы уверены что хотите продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (boxResult == MessageBoxResult.No)
                {
                    return;
                }
            }
            else
            {
                MessageBoxResult boxResult = MessageBox.Show($"Вы собираетесь удалить задачу {task.Name}\r\nУверены что хотите продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (boxResult == MessageBoxResult.No)
                {
                    return;
                }
            }
            dataProvider.RemoveTask(task);
            AllTasks.Remove(task);
            if (task.ParentTask == null)
            {
                UserTasks.Remove(task);
                if (UserTasks.Count == 0)
                    SelectedItem = null;
            }
            else
            {
                task.ParentTask.ChildTasks.Remove(task);
            }
        }

        private void HandleAddPlanStepAction()
        {
            AddPlanViewModel apvm = AddPlanViewModel.GetInstance();
            string name = apvm.Name;
            DateTime dueDate = apvm.DueDate;
            SelectedItem.TaskPlans.Add(dataProvider.AddPlanStep(SelectedItem, CurrentAnalytic, name, dueDate));
            dataProvider.OrderByDate(SelectedItem.TaskPlans);
            apvm.Name = string.Empty;
        }
        
        private void HandlePlanChangedStatus(TaskPlan plan)
        {
            dataProvider.ChangePlanStatus(plan);
        }
        private void HandleRemovePlan(TaskPlan plan)
        {
            dataProvider.RemovePlan(plan);
            SelectedItem.TaskPlans.Remove(plan);
        }

        private void HandleTaskFilterStateChanged(string newState)
        {
            bool IsFilterUnchanged = CurrentTaskFilter.ToString().Equals(newState) && !newState.Equals("MyStructure");


            if (!IsFilterUnchanged)
            {
                IsRedrawActive = true;
                CurrentTaskFilter = (TaskFilter)Enum.Parse(typeof(TaskFilter), newState);
                UpdateTaskCollection();
                switch (newState)
                {
                    case ("AllTasks"):
                        CurrentViewName = "Все задачи";
                        TasksFilteredVisibility = Visibility.Collapsed;
                        AllTaskTreeVisibility = Visibility.Visible;
                        RaisePropertyChanged(nameof(TasksFilteredVisibility));
                        RaisePropertyChanged(nameof(AllTaskTreeVisibility));
                        break;
                    case ("AssignedToMe"):
                        CurrentViewName = "Задачи назначенные мне";
                        TasksFilteredVisibility = Visibility.Visible;
                        AllTaskTreeVisibility = Visibility.Collapsed;
                        RaisePropertyChanged(nameof(TasksFilteredVisibility));
                        RaisePropertyChanged(nameof(AllTaskTreeVisibility));
                        break;
                    case ("ReportedByMe"):
                        TasksFilteredVisibility = Visibility.Visible;
                        AllTaskTreeVisibility = Visibility.Collapsed;
                        RaisePropertyChanged(nameof(TasksFilteredVisibility));
                        RaisePropertyChanged(nameof(AllTaskTreeVisibility));
                        CurrentViewName = "Задачи инициированные мной";
                        break;
                    case ("MyStructure"):
                        TasksFilteredVisibility = Visibility.Visible;
                        AllTaskTreeVisibility = Visibility.Collapsed;
                        RaisePropertyChanged(nameof(TasksFilteredVisibility));
                        RaisePropertyChanged(nameof(AllTaskTreeVisibility));
                        CurrentViewName = "Фильтр по подразделению";
                        break;
                }
                RaisePropertyChanged(nameof(CurrentViewName));
                OnStatisticChanged();
            }
        }

        public delegate void AfterTaskFiltered();
        public event AfterTaskFiltered AfterTaskFilteredEvent;
        private void UpdateTaskCollection()
        {
            List<UserTask> newValues = new List<UserTask>();
            #region фильтр по общему пулу задач
            switch (CurrentTaskFilter)
            {
                case (TaskFilter.AllTasks):
                    AllTasks.Where(i => i.ParentTaskId == null).OrderBy(i=>i.BaseParentTask.Name).ToList().ForEach(newValues.Add);
                    break;
                case (TaskFilter.AssignedToMe):
                    AllTasks.Where(i => i.AssignedTo.Any(anl=>anl.AnalyticId == CurrentAnalytic.Id)).OrderBy(i => i.BaseParentTask.Name).ToList().ForEach(newValues.Add);
                    break;
                case (TaskFilter.ReportedByMe):
                    AllTasks.Where(i => i.CreatedById == CurrentAnalytic.Id).OrderBy(i=>i.BaseParentTask.Name).ToList().ForEach(newValues.Add);
                    break;
                case (TaskFilter.MyStructure):
                    AllTasks.Where(i => i.AssignedTo.Select(j => j.Analytic).
                    Any(k => currentFilterAnalyticsSet.Any(m => m.Id == k.Id))).OrderBy(i => i.BaseParentTask.Name).
                ToList().ForEach(newValues.Add);
                    break;
            }
            #endregion
            UserTasks.Clear();
            foreach(UserTask task in newValues)
            {
                UserTasks.Add(task);
            }
            AfterTaskFilteredEvent?.Invoke();
        }

        private void HandleDragAndDropAction((UserTask, UserTask) FromTo)
        {
            UserTask from = FromTo.Item1;
            UserTask to = FromTo.Item2;
            if(to != null && from != null && to.Id == from.Id)
            {
                return;
            }
            if(to==null && from.ParentTask == null)
            {
                return;
            }
            if(to == null && from.ParentTask != null)
            {
                from.ParentTask.ChildTasks.Remove(from);
                UserTasks.Add(from);
                from.ParentTask = null;
                from.ParentTaskId = null;
                dataProvider.MoveChildTask(from, to);
                return;
            }
            if (from.ContainChild(to))
            {
                MessageBox.Show("Невозможно переместить родительскую задачу внутрь дочерней", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            else if (MessageBox.Show($"Действительно переместить задачу \r\n\"{from.Name}\"\r\n внутрь \r\n\"{to.Name}\"?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                to.AddChildAndSort(from);
                if (from.ParentTask != null)
                {
                    from.ParentTask.ChildTasks.Remove(from);
                }
                else
                {
                    UserTasks.Remove(from);
                }
                from.ParentTask = to;
                dataProvider.MoveChildTask(from, to);
            }
            
        }

        private void HandleTaskPlanUpdate(TaskPlan plan)
        {
            dataProvider.UpdateTaskPlan(plan);
        }

        readonly List<int> StructuresFiltered = new List<int>();

        private void HandleStructFilterChanged(string structureID)
        {
            int stuctId = Convert.ToInt32(structureID);
            if (StructuresFiltered.Contains(stuctId))
            {
                StructuresFiltered.Remove(stuctId);
            }
            else
            {
                StructuresFiltered.Add(stuctId);
            }
            ApplyStructureFilter();
        }

        List<Analytic> currentFilterAnalyticsSet = new List<Analytic>();
        private void ApplyStructureFilter()
        {
            currentFilterAnalyticsSet.Clear();
            if (StructuresFiltered.Count == 0)
            {
                HandleTaskFilterStateChanged("AllTasks");
            }
            else
            {
                foreach (int i in StructuresFiltered)
                {
                    currentFilterAnalyticsSet.AddRange(AnalyticsAtStructures.Single(j => j.Key == i).Value);
                }
                HandleTaskFilterStateChanged("MyStructure");
            }
            
        }
        #endregion
    }
}
