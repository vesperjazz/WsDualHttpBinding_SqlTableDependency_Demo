using System;

namespace WsDualHttpBinding_SqlTableDependency_PoC.WCF.Entities
{
    public class Patient
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int SortOrder { get; set; }
    }
}