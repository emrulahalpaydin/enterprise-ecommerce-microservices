# Architecture

## Türkçe

### Genel Bakış

```
                 +------------------+
                 |    ApiGateway    |
                 |     (Ocelot)     |
                 +--------+---------+
                          |
                          v
  +----------------+  +----------------+  +----------------+  +----------------+
  | IdentityService|  | CatalogService |  |  OrderService  |  | PaymentService |
  +--------+-------+  +--------+-------+  +--------+-------+  +--------+-------+
           |                   |                   |                   |
           +-------------------+---------+---------+-------------------+
                                       |
                                       v
                              +----------------+
                              |   RabbitMQ     |
                              |  Event Bus     |
                              +----------------+
```

### Mimari Kararlar

- **Mikroservis + servis başına veritabanı**: Her servis kendi PostgreSQL’ini yönetir ve HTTP + event ile işlev sunar.
- **Clean Architecture + DDD**: Domain katmanı izole; uygulama katmanı MediatR handler’ları ile orkestre edilir.
- **CQRS yaklaşımı**: Command/Query ayrımı handler seviyesinde uygulanır.
- **Event-driven entegrasyon**: RabbitMQ topic exchange, routing key = event class adı.
- **At-least-once teslimat**: Başarılı işleme `Ack`, hatada `Nack` + `requeue`.
- **API Gateway**: Ocelot tek giriş noktası ve routing sağlar, auth servis bazında uygulanır.

### Event Akışı (gerçek abonelikler)

```
OrderService    -> OrderCreatedEvent    -> PaymentService
PaymentService  -> PaymentCompletedEvent / PaymentFailedEvent -> OrderService
```

### Entegrasyon Sözleşmeleri

Event tipleri `shared/Contracts/Events.cs` içindedir. Varsayılan exchange `microservices.exchange` (topic).  
Queue isim formatı: `{ServiceName}.{EventName}` (örn. `order-service.PaymentFailedEvent`).

### Servis Sınırları

- IdentityService: auth, JWT, refresh token
- CatalogService: ürün, kategori, stok, variant yönetimi
- OrderService: sepet ve sipariş yaşam döngüsü
- PaymentService: ödeme simülasyonu
- ApiGateway: routing + auth enforcement

### Trade-off / Bilinen Eksikler

- **Outbox/Inbox yok**: Event’ler doğrudan yayınlanır; broker erişilebilirliğine bağlıdır.
- **Handler bazlı retry policy yok**: RabbitMQ requeue dışında özel retry yok; idempotency handler sorumluluğundadır.
- **Saga/Orkestrasyon yok**: servisler arası akış basit event zinciridir.



## English

### High-level

```
                 +------------------+
                 |    ApiGateway    |
                 |     (Ocelot)     |
                 +--------+---------+
                          |
                          v
  +----------------+  +----------------+  +----------------+  +----------------+
  | IdentityService|  | CatalogService |  |  OrderService  |  | PaymentService |
  +--------+-------+  +--------+-------+  +--------+-------+  +--------+-------+
           |                   |                   |                   |
           +-------------------+---------+---------+-------------------+
                                       |
                                       v
                              +----------------+
                              |   RabbitMQ     |
                              |  Event Bus     |
                              +----------------+
```

### Architectural Decisions

- **Microservices + Database per service**: Each service owns its data (PostgreSQL) and exposes business capabilities via HTTP and events.
- **Clean Architecture + DDD**: Domain models are isolated; application layer orchestrates commands/queries (MediatR).
- **CQRS-ish split**: Commands and queries are separated at the handler level; not a hard split with separate read models.
- **Event-driven integration**: Services integrate via RabbitMQ topic exchange. Routing key is the event class name.
- **At-least-once delivery**: Consumers ack on success and Nack with requeue on failure.
- **API Gateway**: Ocelot provides routing and single entry point; auth enforced per service.

### Event Flow (actual subscriptions)

```
OrderService    -> OrderCreatedEvent    -> PaymentService
PaymentService  -> PaymentCompletedEvent / PaymentFailedEvent -> OrderService
```

### Integration Contracts

Events are in `shared/Contracts/Events.cs`. The default exchange is `microservices.exchange` (topic).  
Queue naming pattern: `{ServiceName}.{EventName}` (e.g., `order-service.PaymentFailedEvent`).

### Service Boundaries

- IdentityService: auth, JWT, refresh tokens
- CatalogService: products, categories, stock, variants
- OrderService: basket + order lifecycle
- PaymentService: simulated payment processing
- ApiGateway: routing + auth enforcement

### Trade-offs / Known Gaps

- **No Outbox/Inbox**: Events are published directly; eventual consistency relies on broker availability.
- **No retry policy per handler** beyond RabbitMQ requeue; idempotency is the handler’s responsibility.
- **No saga/orchestration**: cross-service workflows are simple event chains.

---
