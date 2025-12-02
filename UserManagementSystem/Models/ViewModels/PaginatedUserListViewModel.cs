namespace UserManagementSystem.Models.ViewModels
{
    public class PaginatedUserListViewModel
    {
        public List<UserListViewModel> Users { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string SearchTerm { get; set; }
    }
}
