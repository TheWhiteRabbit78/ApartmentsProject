using Microsoft.AspNetCore.Identity;
using ApartmentsProject.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

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
                new() { Title = "Stan A1", SurfaceArea = "52.07", Floor ="Prizemlje - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Ulazni hodnik, dnevni boravak s kuhinjom, kupaonica, spremište, soba, lođa i vrt.", Price = 156210, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-180) },
                new() { Title = "Stan A2", SurfaceArea = "55.10", Floor ="Prizemlje - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Prostrani dnevni boravak s kuhinjom i blagovaonicom.", Price = 165300, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-179) },
                new() { Title = "Stan A3", SurfaceArea = "47.74", Floor ="Prizemlje - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom. Kompaktno uređen stan idealan za mlade.", Price = 143220, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-178) },
                new() { Title = "Stan A4", SurfaceArea = "44.96", Floor ="Prizemlje - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom. Funkcionalno uređen prostor.", Price = 134880, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-177) },
                new() { Title = "Stan A5", SurfaceArea = "44.99", Floor ="Prizemlje - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom. Optimalno iskorišten prostor.", Price = 134970, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-176) },
                
                // PRIZEMLJE - B Ulaz
                new() { Title = "Stan B1", SurfaceArea = "53.14", Floor ="Prizemlje - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Prostran stan s dodatnim vanjskim prostorom.", Price = 159420, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-175) },
                new() { Title = "Stan B2", SurfaceArea = "55.35", Floor ="Prizemlje - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Veliki dnevni boravak.", Price = 166050, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-174) },
                new() { Title = "Stan B3", SurfaceArea = "50.15", Floor ="Prizemlje - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Ugodan obiteljski stan.", Price = 150450, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-173) },
                new() { Title = "Stan B4", SurfaceArea = "48.37", Floor ="Prizemlje - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i vrtom. Dobro osmišljen raspored.", Price = 145110, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-172) },
                new() { Title = "Stan B5", SurfaceArea = "49.44", Floor ="Prizemlje - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan u prizemlju s lođom i velikim vrtom. Mirna lokacija.", Price = 148320, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-171) },
                
                // 1. Kat - A Ulaz
                new() { Title = "Stan A6", SurfaceArea = "47.77", Floor ="1. Kat - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Lijep pogled s kata.", Price = 143310, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-170) },
                new() { Title = "Stan A7", SurfaceArea = "52.34", Floor ="1. Kat - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Prostrani dnevni boravak.", Price = 157020, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-169) },
                new() { Title = "Stan A8", SurfaceArea = "47.74", Floor ="1. Kat - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Praktičan raspored.", Price = 143220, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-168) },
                new() { Title = "Stan A9", SurfaceArea = "44.96", Floor ="1. Kat - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Kompaktan i funkcijalan.", Price = 134880, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-167) },
                new() { Title = "Stan A10", SurfaceArea = "44.99", Floor ="1. Kat - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Efikasno iskorišten prostor.", Price = 134970, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-166) },
                new() { Title = "Stan A11", SurfaceArea = "47.67", Floor ="1. Kat - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Ugodan stan za život.", Price = 143010, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-165) },
                new() { Title = "Stan A12", SurfaceArea = "77.63", Floor ="1. Kat - A Ulaz", Rooms = "3+1 (3 spavaće sobe)", Description = "Trosoban stan na prvom katu s balkonom. Prostran obiteljski stan s više soba i WC-om.", Price = 232890, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-164) },
                
                // 1. Kat - B Ulaz
                new() { Title = "Stan B6", SurfaceArea = "47.72", Floor ="1. Kat - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Kvalitetan stan.", Price = 143160, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-163) },
                new() { Title = "Stan B7", SurfaceArea = "52.41", Floor ="1. Kat - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Velik dnevni boravak.", Price = 157230, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-162) },
                new() { Title = "Stan B8", SurfaceArea = "47.72", Floor ="1. Kat - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Dobro planiran stan.", Price = 143160, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-161) },
                new() { Title = "Stan B9", SurfaceArea = "44.91", Floor ="1. Kat - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Praktično uređen.", Price = 134730, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-160) },
                new() { Title = "Stan B10", SurfaceArea = "45.01", Floor ="1. Kat - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Funkcionalan raspored.", Price = 135030, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-160) },
                new() { Title = "Stan B11", SurfaceArea = "47.62", Floor ="1. Kat - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na prvom katu s lođom. Udoban stan za život.", Price = 142860, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-159) },
                new() { Title = "Stan B12", SurfaceArea = "77.63", Floor ="1. Kat - B Ulaz", Rooms = "3+1 (3 spavaće sobe)", Description = "Trosoban stan na prvom katu s balkonom. Prostran obiteljski stan s tri spavaće sobe, dvije kupaonice i WC.", Price = 232890, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-158) },
                
                // 2. Kat - A Ulaz
                new() { Title = "Stan A13", SurfaceArea = "76.44", Floor ="2. Kat - A Ulaz", Rooms = "4+1 (3 spavaće sobe)", Description = "Četverosoban stan na drugom katu s nenatkrivenom terasom. Luksuzni stan s tri spavaće sobe, dvije kupaonice i WC.", Price = 229320, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-157) },
                new() { Title = "Stan A14", SurfaceArea = "46.20", Floor ="2. Kat - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s lođom. Stan s lijepim pogledom.", Price = 138600, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-156) },
                new() { Title = "Stan A15", SurfaceArea = "43.52", Floor ="2. Kat - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s lođom. Kompaktan stan.", Price = 130560, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-155) },
                new() { Title = "Stan A16", SurfaceArea = "67.83", Floor ="2. Kat - A Ulaz", Rooms = "2+1 (2 spavaće sobe)", Description = "Trosoban stan na drugom katu s nenatkrivenom terasom. Prostran stan s dvije spavaće sobe i WC.", Price = 203490, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-154) },
                new() { Title = "Stan A17", SurfaceArea = "56.29", Floor ="2. Kat - A Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s nenatkrivenom terasom i balkonom. Jedinstveni stan s dodatnim vanjskim prostorima.", Price = 168870, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-153) },
                  
                // 2. Kat - B Ulaz
                new() { Title = "Stan B13", SurfaceArea = "76.44", Floor ="2. Kat - B Ulaz", Rooms = "4+1 (3 spavaće sobe)", Description = "Četverosoban stan na drugom katu s nenatkrivenom terasom. Premium stan s tri spavaće sobe, dvije kupaonice i WC.", Price = 229320, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-152) },
                new() { Title = "Stan B14", SurfaceArea = "45.95", Floor ="2. Kat - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s lođom. Kvalitetan stan s pogledom.", Price = 137850, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-151) },
                new() { Title = "Stan B15", SurfaceArea = "43.52", Floor ="2. Kat - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s lođom. Efikashan plan.", Price = 130560, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-150) },
                new() { Title = "Stan B16", SurfaceArea = "67.83", Floor ="2. Kat - B Ulaz", Rooms = "2+1 (2 spavaće sobe)", Description = "Trosoban stan na drugom katu s nenatkrivenom terasom. Prostran obiteljski stan s dvije spavaće sobe.", Price = 203490, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-149) },
                new() { Title = "Stan B17", SurfaceArea = "56.29", Floor ="2. Kat - B Ulaz", Rooms = "1+1 (1 spavaća soba)", Description = "Dvosoban stan na drugom katu s nenatkrivenom terasom i balkonom. Ekskluzivan stan s velikim vanjskim prostorima.", Price = 168870, IsAvailable = true, CreatedAt = DateTime.Now.AddDays(-148) }
            };
            await context.Apartments.AddRangeAsync(apartments);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {apartments.Count} apartments successfully.");

            var imageMapping = new Dictionary<string, string[]>
            {
                ["Stan A1"] = ["A1_tlocrt.jpg", "A1.png"],
                ["Stan A2"] = ["A2_tlocrt.jpg", "A2.png"],
                ["Stan A3"] = ["A3_tlocrt.jpg", "A3.png"],
                ["Stan A4"] = ["A4_tlocrt.jpg", "A4.png"],
                ["Stan A5"] = ["A5_tlocrt.jpg", "A5.png"],
                ["Stan A6"] = ["A6_tlocrt.jpg", "A6.png"],
                ["Stan A7"] = ["A7_tlocrt.jpg", "A7.png"],
                ["Stan A8"] = ["A8_tlocrt.jpg", "A8.png"],
                ["Stan A9"] = ["A9_tlocrt.jpg", "A9.png"],
                ["Stan A10"] = ["A10_tlocrt.jpg", "A10.png"],
                ["Stan A11"] = ["A11_tlocrt.jpg", "A11.png"],
                ["Stan A12"] = ["A12_tlocrt.jpg", "A12.png"],
                ["Stan A13"] = ["A13_tlocrt.jpg", "A13.png"],
                ["Stan A14"] = ["A14_tlocrt.jpg", "A14.png"],
                ["Stan A15"] = ["A15_tlocrt.jpg", "A15.png"],
                ["Stan A16"] = ["A16_tlocrt.jpg", "A16.png"],
                ["Stan A17"] = ["A17_tlocrt.jpg", "A17.png"],
                ["Stan B1"] = ["B1_tlocrt.jpg", "B1.png"],
                ["Stan B2"] = ["B2_tlocrt.jpg", "B2.png"],
                ["Stan B3"] = ["B3_tlocrt.jpg", "B3.png"],
                ["Stan B4"] = ["B4_tlocrt.jpg", "B4.png"],
                ["Stan B5"] = ["B5_tlocrt.jpg", "B5.png"],
                ["Stan B6"] = ["B6_tlocrt.jpg", "B6.png"],
                ["Stan B7"] = ["B7_tlocrt.jpg", "B7.png"],
                ["Stan B8"] = ["B8_tlocrt.jpg", "B8.png"],
                ["Stan B9"] = ["B9_tlocrt.jpg", "B9.png"],
                ["Stan B10"] = ["B10_tlocrt.jpg", "B10.png"],
                ["Stan B11"] = ["B11_tlocrt.jpg", "B11.png"],
                ["Stan B12"] = ["B12_tlocrt.jpg", "B12.png"],
                ["Stan B13"] = ["B13_tlocrt.jpg", "B13.png"],
                ["Stan B14"] = ["B14_tlocrt.jpg", "B14.png"],
                ["Stan B15"] = ["B15_tlocrt.jpg", "B15.png"],
                ["Stan B16"] = ["B16_tlocrt.jpg", "B16.png"],
                ["Stan B17"] = ["B17_tlocrt.jpg", "B17.png"]
            };
            var images = new List<ApartmentImage>();
            foreach (var apartment in apartments)
            {
                if (imageMapping.TryGetValue(apartment.Title, out var fileNames))
                {
                    var apartmentImages = fileNames.Select((fileName, index) => new ApartmentImage
                    {
                        ApartmentId = apartment.Id,
                        FileName = fileName,
                        UploadedAt = DateTime.Now.AddMinutes(index)
                    });

                    images.AddRange(apartmentImages);
                }
            }

            await context.ApartmentImages.AddRangeAsync(images);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {images.Count} apartment images successfully.");

        }
    }
}