using BackendChallenge.CrossCutting.Common;

namespace BackendChallenge.Application.Bikes;
public class Bike : Entity<Guid>
{
    public int Year { get; private set; }
    public string Model { get; private set; }
    public string LicensePlate { get; private set; }

    private Bike(int year, string model, string licensePlate) : base(Guid.NewGuid())
    {
        Year = year;
        Model = model;
        LicensePlate = licensePlate.ToUpper();
    }

    protected Bike() { }

    public static Bike Create(int year, string model, string licensePlate)
    {
        Bike bike = new(year, model, licensePlate);

        return bike;
    }

    public void UpdateLicensePlate(string licensePlate) =>
        LicensePlate = licensePlate.ToUpper();
}
