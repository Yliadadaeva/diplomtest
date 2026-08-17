using System;

namespace NovosibirskForestRegistry
{
    public class ForestPlot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CadastralNumber { get; set; }
        public double Area { get; set; }
        public string Category { get; set; }
        public string District { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}