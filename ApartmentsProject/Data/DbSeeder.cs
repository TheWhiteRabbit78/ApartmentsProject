using Microsoft.AspNetCore.Identity;
using ApartmentsProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartmentsProject.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            const string adminEmail = "admin@fabrikon.hr";
            const string adminPassword = "Admin123!";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    Console.WriteLine($"Admin user created: {adminEmail} / {adminPassword}");
                }
            }
        }

        public static async Task SeedApartments(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Check if apartments already exist
            if (await context.Apartments.AnyAsync())
            {
                Console.WriteLine("Apartments already seeded.");
                return;
            }

            var apartments = new List<Apartment>
            {
                // PRIZEMLJE - A Ulaz
                new() { Title = "Stan A1 - 52.07m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Ulazni hodnik, dnevni boravak s kuhinjom, kupaonica, spremište, soba, lođa i vrt.", Price = 156210, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-180) },
                new() { Title = "Stan A2 - 55.10m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Prostrani dnevni boravak s kuhinjom i blagovaonicom.", Price = 165300, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-179) },
                new() { Title = "Stan A3 - 47.74m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom. Kompaktno uređen stan idealan za mlade.", Price = 143220, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-178) },
                new() { Title = "Stan A4 - 44.96m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom. Funkcionalno uređen prostor.", Price = 134880, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-177) },
                new() { Title = "Stan A5 - 44.99m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom. Optimalno iskorišten prostor.", Price = 134970, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-176) },

                // PRIZEMLJE - B Ulaz
                new() { Title = "Stan B1 - 53.14m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Prostran stan s dodatnim vanjskim prostorom.", Price = 159420, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-175) },
                new() { Title = "Stan B2 - 55.35m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Veliki dnevni boravak.", Price = 166050, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-174) },
                new() { Title = "Stan B3 - 50.15m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Ugodan obiteljski stan.", Price = 150450, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-173) },
                new() { Title = "Stan B4 - 48.37m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Dobro osmišljen raspored.", Price = 145110, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-172) },
                new() { Title = "Stan B5 - 49.44m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i velikim vrtom. Mirna lokacija.", Price = 148320, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-171) },

                // 1. KAT - A Ulaz
                new() { Title = "Stan A6 - 47.77m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Lijep pogled s kata.", Price = 143310, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-170) },
                new() { Title = "Stan A7 - 52.34m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Prostrani dnevni boravak.", Price = 157020, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-169) },
                new() { Title = "Stan A8 - 47.74m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Praktičan raspored.", Price = 143220, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-168) },
                new() { Title = "Stan A9 - 44.96m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Kompaktan i funkcijalan.", Price = 134880, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-167) },
                new() { Title = "Stan A10 - 44.99m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Efikasno iskorišten prostor.", Price = 134970, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-166) },
                new() { Title = "Stan A11 - 47.67m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Ugodan stan za život.", Price = 143010, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-165) },
                new() { Title = "Stan A12 - 77.63m²", Rooms = "3+1 (3 spavaće sobe)", Description = "Trosoban stan na prvom katu s balkonom. Prostran obiteljski stan s više soba i WC-om.", Price = 232890, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-164) },

                // 1. KAT - B Ulaz
                new() { Title = "Stan B6 - 47.72m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Kvalitetan stan.", Price = 143160, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-163) },
                new() { Title = "Stan B7 - 52.41m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Velik dnevni boravak.", Price = 157230, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-162) },
                new() { Title = "Stan B8 - 47.72m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Dobro planiran stan.", Price = 143160, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-161) },
                new() { Title = "Stan B9 - 44.91m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Praktično uređen.", Price = 134730, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-160) },
               new() { Title = "Stan B10 - 45.01m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Funkcionalan raspored.", Price = 135030, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-160) },
               new() { Title = "Stan B11 - 47.62m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Udoban stan za život.", Price = 142860, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-159) },
               new() { Title = "Stan B12 - 77.63m²", Rooms = "3+1 (3 spavaće sobe)", Description = "Trosoban stan na prvom katu s balkonom. Prostran obiteljski stan s tri spavaće sobe, dvije kupaonice i WC.", Price = 232890, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-158) },

               // 2. KAT - A Ulaz
               new() { Title = "Stan A13 - 76.44m²", Rooms = "4+1 (3 spavaće sobe)", Description = "Četverosoban stan na drugom katu s nenatkrivenom terasom. Luksuzni stan s tri spavaće sobe, dvije kupaonice i WC.", Price = 229320, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-157) },
               new() { Title = "Stan A14 - 46.20m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s lođom. Stan s lijepim pogledom.", Price = 138600, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-156) },
               new() { Title = "Stan A15 - 43.52m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s lođom. Kompaktan stan.", Price = 130560, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-155) },
               new() { Title = "Stan A16 - 67.83m²", Rooms = "2+1 (2 spavaće sobe)", Description = "Trosoban stan na drugom katu s nenatkrivenom terasom. Prostran stan s dvije spavaće sobe i WC.", Price = 203490, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-154) },
               new() { Title = "Stan A17 - 56.29m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s nenatkrivenom terasom i balkonom. Jedinstveni stan s dodatnim vanjskim prostorima.", Price = 168870, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-153) },

               // 2. KAT - B Ulaz
               new() { Title = "Stan B13 - 76.44m²", Rooms = "4+1 (3 spavaće sobe)", Description = "Četverosoban stan na drugom katu s nenatkrivenom terasom. Premium stan s tri spavaće sobe, dvije kupaonice i WC.", Price = 229320, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-152) },
               new() { Title = "Stan B14 - 45.95m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s lođom. Kvalitetan stan s pogledom.", Price = 137850, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-151) },
               new() { Title = "Stan B15 - 43.52m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s lođom. Efikashan plan.", Price = 130560, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-150) },
               new() { Title = "Stan B16 - 67.83m²", Rooms = "2+1 (2 spavaće sobe)", Description = "Trosoban stan na drugom katu s nenatkrivenom terasom. Prostran obiteljski stan s dvije spavaće sobe.", Price = 203490, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-149) },
               new() { Title = "Stan B17 - 56.29m²", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s nenatkrivenom terasom i balkonom. Ekskluzivan stan s velikim vanjskim prostorima.", Price = 168870, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-148) }
           };

            await context.Apartments.AddRangeAsync(apartments);
            await context.SaveChangesAsync();

            Console.WriteLine($"Seeded {apartments.Count} apartments successfully.");
        }
    }
}