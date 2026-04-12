\# 🎓 Contoso University Clone (ASP.NET Core MVC)

\## 📌 Introduction

This project is a clone of the \*\*Contoso University\*\* sample application by Microsoft. It is built using \*\*ASP.NET Core MVC\*\* and demonstrates fundamental concepts of web development such as CRUD operations, Entity Framework Core, and MVC architecture.
The goal of this project is to practice backend development and understand how real-world web applications are structured.

\---

\## 🚀 Features

\### ✅ Completed

\* Student Management (Create - Read - Update - Delete)

\* Course Management (Create - Read - Update - Delete)

\* Instructor Management (Read - Update - Delete)

\* Enrollment Management

\* Pagination

\* Data Validation with Data Annotations

\* Entity Framework Core integration

\* ViewModel usage for clean data transfer, prevent Over posting

\### ⚠️ In Progress

\* Instructor Management (Create functionality not completed)

\* Department Management (not completed)

\---


\## 🛠️ Technologies Used

\* ASP.NET Core MVC

\* Entity Framework Core

\* SQL Server

\* Razor View Engine

\* Bootstrap (UI styling)

\* LINQ

\---


\## 📖 Key Concepts Learned

\* MVC Pattern (Model - View - Controller)

\* Working with DbContext in EF Core

\* LINQ queries (`Select`, `Include`, `Where`)

\* Handling relationships (One-to-Many, Many-to-Many)

\* Model Binding \& Validation

\* Dependency Injection in ASP.NET Core

\---


\## ⚡ Getting Started

\### 1. Clone repository

```bash

git clone https://github.com/adinngo/StudentManagementWebMVC.git

cd StudentManagementWebMVC

```

\### 2. Setup database

\* Update connection string in `appsettings.json`

\* Run migrations:

```bash

dotnet ef database update

```

\### 3. Run project

```bash

dotnet run

```

\---


\## 📸 Screenshots

\### Home Page

!\[Home](images/home.png)



\### Students Page

!\[Students](images/students.png)



\### Create Student Page

!\[Create Student](images/createStudent.png)



\### Instructors Page

!\[Instructors](images/instructors.png)



\### Edit Instructor Page

!\[Edit Instructor](images/instructors.png)


\---


\## 💡 Note

This project is for learning purposes based on Microsoft's Contoso University tutorial.

