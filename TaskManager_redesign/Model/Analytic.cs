using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskManager_redesign.Model
{
    [Table("Analytic")]
    public class Analytic
    {
        [Key]
        public int Id { get; set; }
        [Column("userName")]
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public int DepartmentId { get; set; }
        public int DirectionId { get; set; }
        public int UpravlenieId { get; set; }
        public int OtdelId { get; set; }
        public int PositionsId { get; set; }
        public int RoleTableId { get; set; }
        public int? HeadAdmId { get; set; }
        public int? HeadFuncId { get; set; }
        public bool? Deleted_Flag { get; set; }

        public virtual Analytic AdminHead { get; set; }
        public virtual Analytic FunctionHead { get; set; }

        public override string ToString()
        {
            return $"{LastName} {FirstName} {FatherName}";
        }


    }
}
