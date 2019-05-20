namespace FilesExplorerInDB_EF.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Monitor")]
    public partial class Monitor
    {
        public int Id { get; set; }

        [Required]
        public string OperationType { get; set; }

        public string ObjectName { get; set; }

        [Required]
        public string Operator { get; set; }

        public string Message { get; set; }
    }
}
