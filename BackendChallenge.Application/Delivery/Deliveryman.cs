using BackendChallenge.CrossCutting.Common;

namespace BackendChallenge.Application.Delivery;
public class Deliveryman : Entity<Guid>
{
    private Deliveryman(string name, string cnpj, DateTime birthdate, string cnhNumber, CnhType cnhType)
    {
        Name = name;
        Cnpj = cnpj;
        Birthdate = birthdate;
        CnhNumber = cnhNumber;
        CnhType = cnhType;
    }

    protected Deliveryman() { }

    public string Name { get; private set; }
    public string Cnpj { get; private set; }
    public DateTime Birthdate { get; private set; }
    public string CnhNumber { get; private set; }
    public CnhType CnhType { get; private set; }
    public string? CnhImageUrl { get; private set; }

    public static Deliveryman Create(string name, string cnpj, DateTime birthdate, string cnhNumber, CnhType cnhType) =>
        new(name, cnpj, birthdate, cnhNumber, cnhType);

    public void UpdateCnhImageUrl(string cnhImageUrl) =>
        CnhImageUrl = cnhImageUrl;

}

