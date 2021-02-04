using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager_redesign.Model.Base
{
    public abstract class NamedEntity :BaseEntity
    {
        public string Name { get; set; }
    }
}
