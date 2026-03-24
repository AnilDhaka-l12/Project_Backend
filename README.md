ProjectBackend API
Overview
.NET 9 Web API with Firebase Firestore and JWT authentication for React applications.

Tech Stack
.NET 9

Firebase Firestore

JWT Authentication with BCrypt

Swagger UI

Key Features
User Management
POST /api/users - Create user (public)

PUT /api/users/{id} - Update user (public)

GET /api/users - Get all users (admin only)

DELETE /api/users/{id} - Delete user (admin only)

Admin Authentication
POST /api/AdminLogin - Login with credentials to receive JWT token

Security
BCrypt password hashing

JWT tokens (8-hour expiry)

Automatic logging of all admin API calls to Firestore

Setup
1. Install Dependencies
bash
dotnet restore
2. Configure Firebase
Create Firebase project

Enable Firestore

Download service account key

Save as firebase-service-account.json in project root

3. Run Application
bash
dotnet run
Access Swagger UI: http://localhost:5082/swagger