# RideShareApp

A .NET 8 solution implementing an event-driven architecture for a ride-sharing application using MassTransit and RabbitMQ.

## Solution Structure

The solution consists of the following projects:

### 1. **RideShareApp.Api**
Main API gateway with REST controllers:
- `UserController` - User management (create, update)
- `RiderController` - Ride requests and management
- `DriverController` - Driver actions (accept rides)

### 2. **RideShareApp.NotificationService**
Service that handles notifications for various events:
- User creation
- Ride requests
- Ride acceptance
- Ride completion
- Ride cancellations

### 3. **RideShareApp.RatingService**
Service for managing ratings:
- Listens for ride completion events
- Accepts rating submissions
- Provides user rating queries

### 4. **RideShareApp.PaymentsService**
Service for processing payments:
- Processes payments when rides are completed
- Handles payment success/failure scenarios
- Provides payment history queries

### 5. **RideShareApp.Contracts**
Shared library containing event contracts:
- User events (UserCreated, UserUpdated)
- Ride events (RideRequested, RideAccepted, RideCompleted, RideCancelled)
- Payment events (PaymentProcessed, PaymentFailed)
- Rating events (RatingSubmitted)
- Notification events (NotificationSent)

## Event-Driven Architecture

The application uses **MassTransit** with **RabbitMQ** as the message broker to implement an event-driven architecture:

1. **API publishes events** when actions occur (e.g., user created, ride requested)
2. **Services consume events** they're interested in and perform their specific operations
3. **Services can publish new events** based on their processing (e.g., payment processed)

### Event Flow Example

```
1. API receives request → POST /api/user
2. API publishes UserCreatedEvent
3. NotificationService consumes event → Sends welcome email
4. Ride requested → RideRequestedEvent published
5. NotificationService notifies available drivers
6. Driver accepts → RideAcceptedEvent published
7. NotificationService notifies rider
8. Ride completed → RideCompletedEvent published
9. PaymentsService processes payment → PaymentProcessedEvent
10. RatingService enables rating submission
```

## Prerequisites

- .NET 8 SDK
- RabbitMQ server (see installation below)

## Getting Started

### 1. Install RabbitMQ

**Windows:**
```powershell
# Using Chocolatey
choco install rabbitmq

# Or download from: https://www.rabbitmq.com/download.html
```

**Docker:**
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

The management UI will be available at: http://localhost:15672 (username: guest, password: guest)

### 2. Configure RabbitMQ

Update `appsettings.json` in each project if your RabbitMQ instance uses different credentials:

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run the Services

Run each service in separate terminal windows:

**Terminal 1 - API:**
```bash
cd RideShareApp.Api
dotnet run
```

**Terminal 2 - Notification Service:**
```bash
cd RideShareApp.NotificationService
dotnet run
```

**Terminal 3 - Rating Service:**
```bash
cd RideShareApp.RatingService
dotnet run
```

**Terminal 4 - Payments Service:**
```bash
cd RideShareApp.PaymentsService
dotnet run
```

### 5. Access Swagger UI

Each service exposes Swagger UI for testing:
- API: https://localhost:7xxx/swagger (check console for exact port)
- Notification Service: https://localhost:7xxx/swagger
- Rating Service: https://localhost:7xxx/swagger
- Payments Service: https://localhost:7xxx/swagger

## API Endpoints

### User Controller
- `POST /api/user` - Create a new user
- `PUT /api/user/{userId}` - Update user information

### Rider Controller
- `POST /api/rider/request-ride` - Request a new ride
- `POST /api/rider/{rideId}/cancel` - Cancel a ride
- `POST /api/rider/{rideId}/complete` - Complete a ride

### Driver Controller
- `POST /api/driver/{rideId}/accept` - Accept a ride request

### Rating Controller
- `POST /api/rating` - Submit a rating
- `GET /api/rating/user/{userId}` - Get user ratings

### Payment Controller
- `GET /api/payment/{paymentId}` - Get payment status
- `GET /api/payment/user/{userId}` - Get user payment history

## Architecture Benefits

1. **Loose Coupling**: Services communicate via events, not direct dependencies
2. **Scalability**: Services can scale independently
3. **Resilience**: Message queue provides reliability and retry mechanisms
4. **Event Sourcing**: All events are captured and can be replayed
5. **Asynchronous Processing**: Services don't block each other

## Next Steps

To make this production-ready, consider:

1. **Database Integration**: Add Entity Framework Core or similar for data persistence
2. **Authentication/Authorization**: Implement JWT or OAuth2
3. **Error Handling**: Add comprehensive error handling and dead letter queues
4. **Monitoring**: Add Application Insights or similar
5. **Docker**: Containerize services with Docker Compose
6. **Testing**: Add unit and integration tests
7. **API Gateway**: Consider adding an API Gateway pattern

## License

This project is for demonstration purposes.


