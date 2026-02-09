using PaymentService.Domain;

namespace PaymentService.Application;

public interface IPaymentRepository
{
    Task AddAsync(Payment p);
    Task<Payment?> GetByIdAsync(Guid id);
    IQueryable<Payment> Query();
    Task SaveChangesAsync();
}
