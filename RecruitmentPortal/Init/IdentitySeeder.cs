using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RecruitmentPortal.Data;

namespace RecruitmentPortal.Init;

/// <summary>
/// Used to initialize the identity database with admin user and role
/// </summary>
public class IdentitySeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;


    public IdentitySeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // This class will be injected as singleton, and the dependencies are scoped, thus we must create a scope here in order to be able to use the scoped classes
        using (var scope = _serviceProvider.CreateScope())
        {
            ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            await Initialize(dbContext, userManager);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task Initialize(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        string[] roles = new string[] { Strings.Auth.Roles.Administrator };

        foreach (string role in roles)
        {
            var roleStore = new RoleStore<IdentityRole>(dbContext);

            if (!dbContext.Roles.Any(r => r.Name == role))
            {
                await roleStore.CreateAsync(new IdentityRole() { Name = role, NormalizedName = role.ToUpperInvariant() });
            }
        }

        var user = new IdentityUser
        {
            Email = "admin@recruitment.com",
            NormalizedEmail = "ADMIN@RECRUITMENT.COM",
            UserName = "admin@recruitment.com",
            NormalizedUserName = "ADMIN@RECRUITMENT.COM",
            PhoneNumber = "+111111111111",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D")
        };

        if (!dbContext.Users.Any(u => u.UserName == user.UserName))
        {
            var password = new PasswordHasher<IdentityUser>();
            var hashed = password.HashPassword(user, "P@ssw0rd");
            user.PasswordHash = hashed;

            var userStore = new UserStore<IdentityUser>(dbContext);
            var result = await userStore.CreateAsync(user);

        }

        await AssignRoles(userManager, user.Email, roles);
        await dbContext.SaveChangesAsync();
    }

    private async Task<IdentityResult> AssignRoles(UserManager<IdentityUser> userManager, string email, string[] roles)
    {
        IdentityUser user = await userManager.FindByEmailAsync(email);
        var result = await userManager.AddToRolesAsync(user, roles.Select(r => r.ToUpperInvariant()));

        return result;
    }
}
