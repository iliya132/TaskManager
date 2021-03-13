using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TaskManager_redesign.Model.DataProviders.Interfaces
{
    public interface IReport
    {
        public DataTable LoadData(object input);
    }
}
