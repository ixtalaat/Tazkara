using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tazkara.Domain.Entities;
using Tazkara.Domain.Enums;

namespace Tazkara.Infrastructure.Data;

/// <summary>
/// Creates the minimum development/demo data required to exercise the app.
/// Every operation is lookup-first, so running the API repeatedly is safe.
/// </summary>
public static class DatabaseSeeder
{
    public sealed class Options
    {
        public bool Enabled { get; set; }
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;
        public string OrganizerEmail { get; set; } = string.Empty;
        public string OrganizerPassword { get; set; } = string.Empty;
    }

    public static async Task SeedAsync(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<Role> roleManager,
        Options options)
    {
        if (!options.Enabled) return;
        if (string.IsNullOrWhiteSpace(options.AdminEmail) || string.IsNullOrWhiteSpace(options.AdminPassword) ||
            string.IsNullOrWhiteSpace(options.OrganizerEmail) || string.IsNullOrWhiteSpace(options.OrganizerPassword))
            throw new InvalidOperationException("DatabaseSeed credentials must be configured when DatabaseSeed:Enabled is true.");

        await db.Database.MigrateAsync();

        foreach (var roleName in new[] { "Admin", "Organizer", "Customer" })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new Role { Name = roleName });
                EnsureSucceeded(result, $"create role '{roleName}'");
            }
        }

        await EnsureUserAsync(
            userManager, options.AdminEmail, options.AdminPassword, "Talaat", "Tazkara", "Admin");
        var organizer = await EnsureUserAsync(
            userManager, options.OrganizerEmail, options.OrganizerPassword, "Mariam", "Egypt Events", "Organizer");

        var categories = new Dictionary<string, Category>();
        foreach (var name in new[]
        {
            "Music & Concerts",
            "Theatre & Performing Arts",
            "Sports",
            "Culture & Heritage",
            "Festivals & Family"
        })
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.Name == name);
            if (category is null)
            {
                category = new Category { Name = name };
                db.Categories.Add(category);
            }

            categories[name] = category;
        }

        await db.SaveChangesAsync();

        var seededEvents = new[]
        {
            new SeedEvent("Cairo Jazz Nights", "Live jazz under the stars with Egypt's finest musicians.", "AUC Tahrir Square", "Music & Concerts", 12, 450, 180),
            new SeedEvent("Nile Festival of Lights", "An evening of music, food and family activities beside the Nile.", "Al Manial Island, Cairo", "Festivals & Family", 20, 250, 350),
            new SeedEvent("Pharaohs Cup Final", "Experience the excitement of a night football final in Cairo.", "Cairo International Stadium", "Sports", 28, 300, 1200),
            new SeedEvent("Aida at the Cairo Opera House", "Verdi's classic opera performed by an international cast.", "Cairo Opera House", "Theatre & Performing Arts", 35, 650, 700),
            new SeedEvent("Khan el-Khalili Heritage Walk", "A guided cultural evening through historic Islamic Cairo.", "Khan el-Khalili, Cairo", "Culture & Heritage", 42, 180, 90),
            new SeedEvent("Alexandria Mediterranean Film Week", "A celebration of Egyptian and Mediterranean cinema.", "Bibliotheca Alexandrina", "Culture & Heritage", 55, 220, 160)
        };

        foreach (var item in seededEvents)
        {
            if (await db.Events.AnyAsync(e => e.Title == item.Title))
                continue;

            var start = DateTime.UtcNow.Date.AddDays(item.DaysFromNow).AddHours(18);
            db.Events.Add(new Event
            {
                Title = item.Title,
                Description = item.Description,
                Location = item.Location,
                StartDate = start,
                EndDate = start.AddHours(3),
                Capacity = item.Capacity,
                AvailableTickets = item.Capacity,
                Price = item.Price,
                Status = EventStatus.Published,
                OrganizerId = organizer.Id,
                CategoryId = categories[item.Category].Id
            });
        }

        await db.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string firstName,
        string lastName,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await userManager.CreateAsync(user, password);
            EnsureSucceeded(result, $"create user '{email}'");
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var result = await userManager.AddToRoleAsync(user, role);
            EnsureSucceeded(result, $"assign role '{role}' to '{email}'");
        }

        return user;
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Unable to {operation}: {errors}");
        }
    }

    private sealed record SeedEvent(
        string Title,
        string Description,
        string Location,
        string Category,
        int DaysFromNow,
        decimal Price,
        int Capacity);
}
