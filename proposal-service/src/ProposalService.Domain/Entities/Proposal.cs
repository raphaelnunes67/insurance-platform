namespace ProposalService.Domain.Entities;

public class Proposal
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; }
    public decimal InsuredAmount { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Proposal(string customerName, decimal insuredAmount)
    {
        Id = Guid.NewGuid();
        CustomerName = customerName;
        InsuredAmount = insuredAmount;

        Status = "InAnalysis";

        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void Approve()
    {
        if (Status != "InAnalysis")
            throw new InvalidOperationException("Just proposals in analysis can be approved!");

        Status = "Approved";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != "InAnalysis")
            throw new InvalidOperationException("Just proposals in analysis can be rejected!");

        Status = "Rejected";
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInformation(string newCustomerName, decimal newInsuredAmount)
    {

        if (Status == "Approved" || Status == "Rejected")
        {
            throw new InvalidOperationException($"It is not possible set status: '{Status}'.");
        }

        CustomerName = newCustomerName;
        InsuredAmount = newInsuredAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    protected Proposal()
    {
        CustomerName = default!;
        Status = default!;
    }
}