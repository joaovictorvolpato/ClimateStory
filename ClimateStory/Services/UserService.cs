using ClimateStory.Models;
using Microsoft.EntityFrameworkCore;

namespace ClimateStory.Services;


public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IHashService _hashService;

        // Inject the IHashService via constructor
        public UserService(AppDbContext context, IHashService hashService)
        {
            _context = context;
            _hashService = hashService;
        }

        public async Task<IEnumerable<AppDbContext.User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<AppDbContext.User> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddUser(AppDbContext.User user)
        {
            // Hash the user's password using IHashService
            user.password = _hashService.HashPassword(user.password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUser(AppDbContext.User user)
        {
            // If the password has been modified, hash the new password
            if (!string.IsNullOrWhiteSpace(user.password))
            {
                user.password = _hashService.HashPassword(user.password);
            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        // Validate email and password for login
        public async Task<AppDbContext.User> ValidateUser(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.email == email);
            if (user == null)
            {
                return null;
            }

            // Use IHashService to verify the password
            bool isPasswordValid = _hashService.VerifyPassword(password, user.password);
            return isPasswordValid ? user : null;
        }
}