using BackendChallenge.Application.Accounts;
using BackendChallenge.CrossCutting.Common;

namespace BackendChallenge.Application.Delivery;
public class Deliveryman : Entity<Guid>
{
    private Deliveryman(
       string accountId, string name, string cnpj, DateTime birthdate, string cnhNumber, CnhType cnhType)
    {
        AccountId = accountId;
        Name = name;
        Cnpj = cnpj;
        Birthdate = birthdate;
        CnhNumber = cnhNumber;
        CnhType = cnhType;
    }

    protected Deliveryman() { }

    public string AccountId { get; private set; }
    public string Name { get; private set; }
    public string Cnpj { get; private set; }
    public DateTime Birthdate { get; private set; }
    public string CnhNumber { get; private set; }
    public CnhType CnhType { get; private set; }
    public string? CnhImageUrl { get; private set; }

    public Account Account { get; private set; }

    public static Deliveryman Create(string accountId, string name, string cnpj, DateTime birthdate, string cnhNumber, CnhType cnhType) =>
        new(accountId, name, cnpj, birthdate, cnhNumber, cnhType);

    public void UpdateCnhImageUrl(string cnhImageUrl) =>
        CnhImageUrl = cnhImageUrl;

}

