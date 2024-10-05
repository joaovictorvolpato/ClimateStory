using ClimateStory.Models;

namespace ClimateStory.Services;

public interface IUserService
{
    Task<IEnumerable<AppDbContext.User>> GetAllUsers();
    Task<AppDbContext.User> GetUserById(int id);
    Task AddUser(AppDbContext.User user);
    Task<AppDbContext.User> ValidateUser(string email, string password);
    Task UpdateUser(AppDbContext.User user);
    Task DeleteUser(int id);
}