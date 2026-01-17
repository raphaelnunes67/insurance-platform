namespace ContractingService.Domain.Entities;

public class Contract
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; }
    public decimal InsuredAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; } 
    public DateTime? ContractedAt { get; private set; } 

    public Contract(Guid id, string customerName, decimal insuredAmount)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id not be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Need client name.", nameof(customerName));

        if (insuredAmount <= 0)
            throw new ArgumentException("The ValueTask must be bigger than zero.", nameof(insuredAmount));

        Id = id;
        CustomerName = customerName;
        InsuredAmount = insuredAmount;
        
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void SignContract(DateTime contractedDate)
    {
        if (contractedDate > DateTime.UtcNow)
             throw new ArgumentException("The contract date will not be in future");

        ContractedAt = contractedDate;
        UpdatedAt = DateTime.UtcNow;
    }
    protected Contract() 
    { 
        CustomerName = default!;
    }
}