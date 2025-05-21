using System.ComponentModel.DataAnnotations;

namespace SHDomain.Models
{
    public class Audit
    {
        [Key]
        public int? Id { get; set; }

        public string? EmployeeId { get; set; }

        public string? OperationName { get; set; }

        public string? TableName { get; set; }

        public string? ColumnName { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? Description { get; set; }

        public DateTime Created { get; set; }
    }

















    //public class Audit
    //{
    //    [Key]
    //    public int? Id { get; set; }
    //    public string? EmployeeId { get; set; }
    //    public string? Description { get; set; }
    //    public DateTime Created { get; set; }
    //    public AuditExt? AuditExt { get; set; }
    //}

    //public class AuditExt
    //{
    //    [Key]
    //    public int? AuditExtId { get; set; }
    //    public int? AuditId { get; set; }
    //    public string? OperationName { get; set; }
    //    public string? TableName { get; set; }
    //    public string? ColumnName { get; set; }
    //    public string? OldValue { get; set; }
    //    public string? NewValue { get; set; }
    //}

    //Audit auditBrand = new()
    //{
    //    Id = newAuditId.ToString(),
    //    EmployeeId = sessionId,
    //    Description = "Update existing brand record",
    //    Created = DateTime.Now,
    //    AuditExt = new AuditExt
    //    {
    //        AuditExtId = ObjectId.GenerateNewId().ToString(),
    //        AuditId = newAuditId.ToString(),
    //        OperationName = "Update",
    //        TableName = "AssetBrand",
    //        ColumnName = "BrandName",
    //        OldValue = auditOldBrand.BrandName,
    //        NewValue = auditNewBrand.BrandName
    //    }
    //};
}
