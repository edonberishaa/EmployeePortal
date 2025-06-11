namespace EmployeePortal.Models
{
    public enum EmployeeType
    {
        Permanent = 1,
        Temporary = 2,
        Contract =3,
        Intern=4
    }
    public enum Department
    {
        IT =1,
        HR =2,
        Sales = 3,
        Admin = 4
    }
    public class EmployeeTypeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class DepartmentEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
