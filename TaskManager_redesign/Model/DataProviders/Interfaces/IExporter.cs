using System.Data;

namespace TaskManager_redesign.Model.DataProviders.Interfaces
{
    public interface IExporter
    {
        public string Export(DataTable input);
    }
}
