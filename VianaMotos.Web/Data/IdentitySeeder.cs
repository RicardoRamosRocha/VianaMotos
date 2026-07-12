using Microsoft.AspNetCore.Identity;
using VianaMotos.Web.Models;

namespace VianaMotos.Web.Data;

public static class IdentitySeeder
{
    public const string AdministratorRole = "Administrador";

    public static async Task SeedAdministratorAsync(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger logger)
    {
        var roleManager = services
            .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager = services
            .GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync(AdministratorRole))
        {
            var roleResult = await roleManager.CreateAsync(
                new IdentityRole(AdministratorRole));

            ValidarResultado(
                roleResult,
                "Não foi possível criar o perfil Administrador.");
        }

        var email = configuration["ADMIN_EMAIL"];
        var password = configuration["ADMIN_PASSWORD"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "ADMIN_EMAIL ou ADMIN_PASSWORD não configurados. " +
                "O usuário administrador inicial não foi criado.");
            return;
        }

        var administrator = await userManager.FindByEmailAsync(email);

        if (administrator is null)
        {
            administrator = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(
                administrator,
                password);

            ValidarResultado(
                createResult,
                "Não foi possível criar o usuário administrador.");
        }

        if (!await userManager.IsInRoleAsync(
                administrator,
                AdministratorRole))
        {
            var roleResult = await userManager.AddToRoleAsync(
                administrator,
                AdministratorRole);

            ValidarResultado(
                roleResult,
                "Não foi possível vincular o usuário ao perfil Administrador.");
        }
    }

    private static void ValidarResultado(
        IdentityResult result,
        string message)
    {
        if (result.Succeeded)
            return;

        var errors = string.Join(
            "; ",
            result.Errors.Select(error => error.Description));

        throw new InvalidOperationException($"{message} {errors}");
    }
}
