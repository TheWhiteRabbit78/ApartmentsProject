namespace ApartmentsProject.ViewModels
{
    public class ApartmentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Rooms { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string Status => IsAvailable ? "Dostupan" : "Prodan";
        public string FormattedPrice => $"€{Price:N0}";

        // Replace the old image properties with proper lists
        public List<ApartmentImageViewModel> AllImages { get; set; } = new List<ApartmentImageViewModel>();
        public List<ApartmentImageViewModel> MainImages => AllImages.Where(i => i.Type == "main").ToList();
        public List<ApartmentImageViewModel> FloorplanImages => AllImages.Where(i => i.Type == "floorplan").ToList();
        public List<ApartmentImageViewModel> InteriorImages => AllImages.Where(i => i.Type == "interior").ToList();
        public List<ApartmentImageViewModel> ExteriorImages => AllImages.Where(i => i.Type == "exterior").ToList();

        // Helper properties for easy access
        public string MainImageUrl => MainImages.FirstOrDefault()?.Url;
        public bool HasImages => AllImages.Any();
    }
}