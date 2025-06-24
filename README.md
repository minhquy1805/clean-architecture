# 📰 **CommercialNews — User Module**

Welcome to **CommercialNews** — a modern, production-ready news platform designed with **Clean Architecture**, secure authentication, and flexible user management.

This module handles all **User Management** concerns — from secure registration to admin-level user lifecycle operations.

---

## ✅ 1️⃣ Overview

The **User Module** includes:

- Secure **email registration** with verification
- **JWT**-based authentication with **Refresh Token** support
- **Password recovery** with secure reset token
- **Self-service** profile updates & password change
- **Admin-level** CRUD, soft delete & restore
- **Rate Limiting** to prevent abuse
- **Audit Logs** for user actions
- **Login History** for security insights
- Clean separation: **Domain / Application / Infrastructure / API**

---

## ✅ 2️⃣ API Summary

### 🔑 Auth APIs

| API | Route | Method | Purpose |
|-----|-------|--------|---------|
| Register | `/api/v1/auth/register` | POST | Create new user & send verification email |
| Verify Email | `/api/v1/auth/verify` | GET | Verify email with token |
| Resend Verification | `/api/v1/auth/resend-verification` | POST | Resend verification link |
| Login | `/api/v1/auth/login` | POST | Authenticate & receive tokens |
| Refresh Token | `/api/v1/auth/refresh-token` | POST | Renew AccessToken |
| Logout | `/api/v1/auth/logout` | POST | Revoke RefreshToken (optional) |

### 👤 User Self Management

| API | Route | Method | Purpose |
|-----|-------|--------|---------|
| Get Profile | `/api/v1/users/me` | GET | View logged-in user profile |
| Update Profile | `/api/v1/users/me` | PUT | Update profile fields |
| Change Password | `/api/v1/users/change-password` | POST | Update password |
| Forgot Password | `/api/v1/users/forgot-password` | POST | Request reset link |
| Reset Password | `/api/v1/users/reset-password` | POST | Reset with token |

### 🔑 Admin User Management

| API | Route | Method | Purpose |
|-----|-------|--------|---------|
| Get All | `/api/v1/admin/users/all` | GET | Get all users |
| Paging & Filter | `/api/v1/admin/users/paging` | GET | Search & filter users |
| Get by Id | `/api/v1/admin/users/{id}` | GET | View user detail |
| Update | `/api/v1/admin/users/{id}` | PUT | Admin update |
| Soft Delete | `/api/v1/admin/users/{id}/soft-delete` | PUT | Deactivate user |
| Restore | `/api/v1/admin/users/{id}/restore` | PUT | Reactivate user |

---

## ✅ 3️⃣ Database Overview

| Table | Purpose |
|-------|---------|
| **User** | Stores user data, IsActive + Flag fields |
| **UserVerification** | Manages email verify / reset tokens |
| **RefreshToken** | Stores refresh sessions |
| **UserAudit** | Tracks updates & sensitive actions |
| **LoginHistory** | Logs every login attempt (success & fail) |

---

## ✅ 4️⃣ Security & Best Practices

- **Email verification (Flag)**: must be `T` to log in.
- **IsActive**: Soft delete disables user access.
- **Rate Limit**: Configured for register, forgot-password, etc.
- **Audit**: Changes tracked (Update, ChangePassword).
- **Login History**: IP, UserAgent stored for every attempt.
- **JWT**: Secure short-lived AccessToken + long-lived RefreshToken.

---

## ✅ 5️⃣ Structure

| Layer | Content |
|-------|---------|
| **Domain** | Entities: User, UserVerification, RefreshToken, Audit, LoginHistory |
| **Application** | DTOs, Validators, Interfaces, Services |
| **Infrastructure** | ADO.NET repositories, stored procedure calls |
| **API** | AuthController, UserController, AdminUserController |
| **Email** | MailKit + Razor/HTML templates |

---

## ✅ 6️⃣ Additional Features Implemented

✅ **Soft Delete**: via `IsActive`  
✅ **Audit Logs**: for admin edits, self edits, password change  
✅ **Login History**: for security monitoring  
✅ **Rate Limit**: per IP per route (AspNetCoreRateLimit)  
✅ **ExceptionMiddleware**: global error handling  
✅ **FluentValidation**: robust input validation  
✅ **ApiResponse**: unified success/error format  
✅ **BaseController**: DRY pattern for consistent response

---

## ✅ 7️⃣ Suggested Next Steps

| Idea | Note |
|------|------|
| 2FA / OTP | Add phone/email code at login |
| ABAC / RBAC | Dynamic RoleClaims for modular permission |
| Session Management UI | Admin can force logout devices |
| Logging & Monitoring | Use Serilog + ELK |
| CI/CD | Secrets vault + pipeline |
| Swagger JWT Auth | Enable JWT in Swagger UI |
| Unit Test | Increase coverage to 80%+ |

---

## ✅ 8️⃣ How To Start

✅ Clone repo  
✅ Configure `appsettings.json`:
- Database
- SMTP (Gmail)
- JWT settings

✅ Run migrations or scripts for:
- `User`
- `UserVerification`
- `RefreshToken`
- `UserAudit`
- `LoginHistory`

✅ Test with Swagger or Postman.

---

## ✅ 9️⃣ Contact

Any ideas or PRs welcome!
**CommercialNews** — flexible, secure & ready to scale 🚀

---

## ✅ 10️⃣ One More Thing!

Check `docs/UserModule_Phase2_Roadmap.pdf` for your next upgrade!

---

**Happy coding!**
