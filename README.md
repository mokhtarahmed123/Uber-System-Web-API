# 🚖 Uber System Web API

A **clean, modular Web API** for an Uber-like system, built with **ASP.NET Core** and following a layered architecture. This project is designed with scalability, maintainability, and clarity in mind.

---

## 📂 Repository Structure

* **`Uber.API`** → ASP.NET Core Web API project (entry point, controllers, Swagger, middleware).
* **`Uber.Application`** → Application services, DTOs, and use cases.
* **`Uber.Domain`** → Domain entities and business rules.
* **`Uber.Infrastructure`** → Data access layer, EF Core, and external services.
* **`Migrations`** → Database migrations managed with EF Core.
* **Config files** → `appsettings.json`, `appsettings.Development.json` for environment-specific settings.

---

## ✨ Features

✅ Clean Architecture (API / Application / Domain / Infrastructure)
✅ Entity Framework Core with migrations
✅ ASP.NET Core Web API ready to run
✅ Configurable via `appsettings.json`
✅ Swagger/OpenAPI support for API documentation

---

## 🛠 Tech Stack

* **.NET 7/8 (C#)**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server 
* **Swagger (Swashbuckle)** for API docs
* Caching With Redis
* Auto Mapping

---

## 🚀 Getting Started

### 1️⃣ Clone the repository

```bash
git clone https://github.com/mokhtarahmed123/Uber-System-Web-API.git
cd Uber-System-Web-API
```

### 2️⃣ Configure the database

Update **`appsettings.json`** with your connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=UberDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 3️⃣ Apply migrations

```bash
cd Uber.API
dotnet ef database update --project ../Uber.Infrastructure --startup-project .
```

### 4️⃣ Run the API

```bash
dotnet run --project Uber.API
```

The API will be available at:

* `https://localhost:5001`
* `http://localhost:5000`

Swagger docs: [http://localhost:5105/swagger](http://localhost:5105/swagger)

---

## 📖 API Documentation

* All endpoints are available via **Swagger UI** when the project is running.
* Controllers are located in the `Uber.API/Controllers` folder.
* DTOs can be found in the `Uber.Application/DTOs` folder.

---

## 🧪 Testing

* Add unit/integration tests under a separate test project (e.g., `Uber.Tests`).
* Suggested frameworks: **xUnit**, **NUnit**, **Moq**.

---

## 🤝 Contributing

1. Fork the repo
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit & push changes: `git push origin feature/your-feature`
4. Open a Pull Request 🎉

---

## 📌 Roadmap / TODO

* [ ] Add unit and integration tests
* [ ] Improve Swagger documentation with examples
* [ ] Setup CI/CD pipeline
* [ ] Add Docker support
* [ ] Add role-based authentication & authorization

---

## 📬 Contact

👤 **Mokhtar Ahmed**
GitHub: [mokhtarahmed123](https://github.com/mokhtarahmed123)
<img width="1675" height="878" alt="Screenshot 2025-09-12 232339" src="https://github.com/user-attachments/assets/50615bd1-65b6-4dfb-9486-cc281d11b747" />
<img width="1368" height="897" alt="Screenshot 2025-09-12 232413" src="https://github.com/user-attachments/assets/34ff6c4e-6c49-4181-a8ae-fbcef32f52a1" />
<img width="1311" height="920" alt="Screenshot 2025-09-12 232510" src="https://github.com/user-attachments/assets/b8ed5008-9044-460b-b857-20ebf45ad9cb" />

---<img width="1476" height="930" alt="Screenshot 2025-09-12 232440" src="https://github.com/user-attachments/assets/19e0a54f-b913-4f6a-942c-ead2e7bb2b94" />
<img width="1266" height="937" alt="Screenshot 2025-09-12 232628" src="https://github.com/user-attachments/assets/496a04b9-768d-476a-9a0f-a8ac7ba298b8" />
<img width="1266" height="937" alt="Screenshot 2025-09-12 232628" src="https://github.com/user-attachments/assets/5697fe32-cafa-40f5-bfc4-361b0b8c60d3" />




🚀 *Ready to scale your Uber-like system with ASP.NET Core!*
