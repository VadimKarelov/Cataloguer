namespace Cataloguer.Server.ArrivingModels
{
    public class BrochureCreationModel
    {
        public string Name { get; set; }
        public int Edition { get; set; }
        public DateTime Date { get; set; }
        public (int GoodId, decimal Price)[] Positions { get; set; }
    }
}
