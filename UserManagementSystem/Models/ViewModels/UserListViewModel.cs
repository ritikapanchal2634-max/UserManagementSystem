namespace UserManagementSystem.Models.ViewModels
{
    public class UserListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
