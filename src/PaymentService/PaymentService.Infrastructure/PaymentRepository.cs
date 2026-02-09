using PaymentService.Application;
using PaymentService.Domain;
using PaymentService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace PaymentService.Infrastructure;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _db;
    public PaymentRepository(PaymentDbContext db) => _db = db;

    public Task AddAsync(Payment p)
    {
        _db.Payments.Add(new PaymentEntity { Id = p.Id, OrderId = p.OrderId, Amount = p.Amount, Status = p.Status.ToString(), Method = p.Method, TransactionId = p.TransactionId, CreatedAt = p.CreatedAt });
        return Task.CompletedTask;
    }

    public IQueryable<Payment> Query()
    {
        return _db.Payments.AsEnumerable().Select(e => Payment.Load(
            e.Id,
            e.OrderId,
            e.Amount,
            Enum.TryParse<PaymentStatus>(e.Status, out var s) ? s : PaymentStatus.Pending,
            e.Method,
            e.TransactionId,
            e.CreatedAt)).AsQueryable();
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        var e = await _db.Payments.FirstOrDefaultAsync(p => p.Id == id);
        if (e == null) return null;
        return Payment.Load(
            e.Id,
            e.OrderId,
            e.Amount,
            Enum.TryParse<PaymentStatus>(e.Status, out var s) ? s : PaymentStatus.Pending,
            e.Method,
            e.TransactionId,
            e.CreatedAt);
    }
}
