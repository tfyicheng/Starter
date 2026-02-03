namespace Starter.Models
{
    class BackgroundSettings
    {
        public string Mode { get; set; } = "Color"; // Color / Image
        public string Color { get; set; } = "#33000000";
        public double Opacity { get; set; } = 1;
        public double CornerRadius { get; set; } = 22;
        public bool Blur { get; set; } = false;
        public double BlurRadius { get; set; } = 12;
        public string ImagePath { get; set; } = "";
    }
}
