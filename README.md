# Stock Trading Application

## 1. Project Description

Stock Trading Application is a high-performance stock trading simulator, built with **.NET Core 8**, **Redis caching**, and **optimized SQL** for fast execution. The project focuses on **periodic stock updates (every minute), historical tracking, and transaction management** while maintaining scalability and performance.

> **Note:** This project does not implement real-time stock price changes upon buying/selling stocks. In a real-world scenario, stock values would update dynamically based on market activity, but this feature is not included in the current implementation.

---

## 2. Purpose

The purpose of this application is to **simulate stock trading** with a focus on performance optimization and scalable design. This is a **demo project**, and some aspects have been **simplified**:

- **Limited stock data** from the external provider.
- **User creation is allowed**, but authentication is **not secured via JWT** (which would be required in a real-world scenario).
- **Designed for learning & demonstration**, but follows best practices to be expandable into a production-grade system.

---

## 3. Tech Stack

### **Primary Technologies:**

- **Backend:** .NET Core 8, C#
- **Database:** PostgreSQL (running in Docker), Entity Framework Core (ORM)
- **Caching & Performance:** Redis for caching (running in Docker), Redis Rate Limiting
- **Job Scheduling:** Quartz for periodic stock data updates
- **Testing:** xUnit, Moq for unit testing, test containers for integration tests
- **Database Management:** pgAdmin (running in Docker)

### **Design & Architectural Considerations:**

- **Repository Pattern** - Clean architecture, separation of concerns, easy testability.
- **Unit of Work Pattern** - Transaction management with rollback support.
- **Factory Pattern** - Used for general error handling responses, ensuring consistent API behavior.
- **Decorator Pattern** - Applied for extending functionality like caching and logging without modifying base implementations.
- **Options Pattern** - Industry-standard way of reading configurations from `appsettings.json`.
- **Reflection for Dynamic Entity Registration** - Used to register entities dynamically in **Entity Framework Core**.
- **Inheritance with Reflection for Concurrency Control** - Implemented a base class `ConcurrentParentClass`, which automatically applies the **[Timestamp] attribute** (using PostgreSQL's `xmin` column for optimistic concurrency control) to all derived entities.

### **SOLID Principles & Best Practices:**

- **Dependency Inversion & OOP** - Inheritance and interface-based abstractions.
- **On-Delete Cascade Consistency** - Ensures that when a **Portfolio** is deleted, its related **Transactions & Holdings** are also removed.

> **Note:** While the database, Redis, and pgAdmin are containerized in Docker, the application itself is currently **not** Dockerized and is intended to be run manually.

---

## 4. How It Started - Development Process

This project follows a **Code-First Approach**:

- The **DB schema was first drafted** but generated dynamically using `EnsureCreated()`.
- **Key database entities:**
  - **AppUser**: Stores user details.
  - **Stock**: Represents a tradeable stock.
  - **Stock History**: Retains stock price changes over time.
  - **Portfolio**: A user’s portfolio and cash balance.
  - **Portfolio Holding**: Tracks how many units of a stock a user owns.
  - **Portfolio Transaction**: Logs every buy/sell action for historical accuracy.
- **On-Delete Cascade Rules** ensure automatic cleanup when an entity is removed.

---

## 5. Core Features & User Actions

### **Automated Scheduled Job:**

- Runs a background job that **fetches updated stock values** and **writes them to the database** every minute.
- Implements the **Decorator Pattern** to update the **cache dynamically** for optimized performance.

### **User & Portfolio Management:**

- **Create a user** with a unique username.
- **Create a portfolio** (unique and linked to a user ID).

### **Stock Trading Features:**

- **Buy stock** - Adds stock to portfolio holdings.
- **Sell stock** - Removes stock from portfolio holdings.
- **Check portfolio balance** (total cash and stock valuation).
- **View transaction history** - Keeps records of all trades for auditing.

---

## 6. Key Technical Implementations & Architectural Considerations

### **Security & Performance Optimization:**

- **No JWT authentication for this demo**. 
- **Native SQL queries for performance optimization:**
  - **MERGE INTO** statements to update bulk stock data.
  - **Bulk inserts** to minimize database calls for stock historical updates.
- **Caching Mechanism:**
  - **Avoids blindly updating all stock prices**, only updating **changed values**.

### **RESTful API Design:**

- Follows **RESTful principles** for endpoint structuring.
- **Rate Limiting via Redis:**
  - **Restricts each IP to 10 requests per minute** (configurable for enhanced security).
- **Health Checks & Monitoring:**
  - Monitors **database, cache, and core application** health status.

### **Locking & Concurrency Control:**

- Implements **locking mechanisms** to prevent race conditions in **buying/selling stocks** where cash balances are critical.
- **Reflection-Based Concurrency Control:**
  - Any entity inheriting from `ConcurrentParentClass` is automatically marked for concurrency control using PostgreSQL’s `xmin`.

---

## 7. Unit Testing & Integration Tests

### **Unit Tests:**

- Focus on **business logic in service classes** to validate core behaviors.

### **Integration Tests:**

- Ensure that **On-Delete Cascade** works correctly for relational data integrity.

---

## Conclusion

This project effectively demonstrates **.NET engineering skills**, following best practices for **performance, scalability, clean architecture, and real-world considerations**. While simplified for demo purposes, it is structured to be expandable into a **production-ready** stock trading system.

---
