using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TaskManager_redesign.Model.DataProviders.Implementations;

namespace TaskManager_redesign
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App() : base()
		{
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
            HandleUnobservedException(sender, e.Exception);
		}

		private void HandleUnobservedException(object sender, Exception e)
        {
            Window window = new Window();
            window.Show();
            window.Visibility = Visibility.Collapsed;
            MessageBox.Show("В работе приложения возникло необработанное исключение.\r\nПриложение направит нужную информацию в ОРППА для анализа.\r\nВозможно последнее совершенное вами действие не сохранится.\r\nПожалуйста перезапустите приложение и проверьте внесенные изменения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Exception exOfInterest = GetLastException(e);
            StackTrace stackTrace = new StackTrace(GetLastException(e), true);
            string lineNumber = string.Join("###", stackTrace.GetFrames().Select(i=>$"{i.GetMethod().Name}, {i.GetFileName()}, {i.GetFileLineNumber()}"));
            SqlDataProvider.WriteLog(sender, exOfInterest.Message, e.StackTrace, $"Error at line {lineNumber} - {e.Source}", e.TargetSite.Name);
            window.Close();
        }

        private Exception GetLastException(Exception e)
        {
            if(e.InnerException == null)
            {
                return e;
            }
            else
            {
                return GetLastException(e.InnerException);
            }
        }
	}
}
