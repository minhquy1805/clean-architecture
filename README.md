# 📰 **CommercialNews — User Module**

Welcome to **CommercialNews** — a modern, production-ready news platform designed to deliver powerful user authentication, secure sessions, and a clean architecture structure for scalability.

This module focuses on everything related to **user management**, ensuring that each user is securely registered, verified, authenticated, and empowered to manage their account.

---

## ✅ 1️⃣ Overview

The **User Module** provides a secure, production-ready set of endpoints for user authentication, account management, and session handling.

**Core features include:**
- Register with email verification
- JWT login with Refresh Token support
- Session revocation (Logout)
- Password recovery (Forgot/Reset)
- Self-profile management (view, update, change password)
- Admin-ready CRUD for users

---

## ✅ 2️⃣ API Summary

### 🔑 Auth APIs

| API | Route | Method | Purpose |
|-----|-------|--------|---------|
| Register | /api/v1/auth/register | POST | Create user, send email verification |
| Verify Email | /api/v1/auth/verify | GET | Verify account via token |
| Resend Verification | /api/v1/auth/resend-verification | POST | Resend a new verification link |
| Login | /api/v1/auth/login | POST | Login, get AccessToken + RefreshToken |
| Refresh Token | /api/v1/auth/refresh-token | POST | Get a new AccessToken when old one expires |
| Logout | /api/v1/auth/logout | POST | Revoke Refresh Token (logout) |

### 👤 User Self Management

| API | Route | Method | Purpose |
|-----|-------|--------|---------|
| Get Own Profile | /api/v1/users/me | GET | Retrieve logged-in user’s profile |
| Update Own Profile | /api/v1/users/me | PUT | Update profile details |
| Change Password | /api/v1/users/change-password | POST | Change current password |
| Forgot Password | /api/v1/users/forgot-password | POST | Request password reset link |
| Reset Password | /api/v1/users/reset-password | POST | Reset password using reset token |

---

## ✅ 3️⃣ Database Overview

| Table | Purpose |
|-------|---------|
| User | Stores user data, flag for verified status |
| UserVerification | Stores verification/reset tokens |
| RefreshToken | Manages refresh token sessions |

---

## ✅ 4️⃣ Typical Flows

### Register & Verify
1. POST /auth/register → creates user with Flag = F
2. System sends email with link: /auth/verify?token=...
3. User clicks link → GET /auth/verify → sets Flag = T

### Login & Session
1. POST /auth/login → checks Flag == T → returns Access & Refresh Token.
2. FE uses Access Token for protected endpoints.
3. On expiry → FE calls POST /auth/refresh-token.
4. User logs out → POST /auth/logout → revokes Refresh Token.

### Forgot/Reset Password
1. POST /users/forgot-password → sends /reset-password?token=...
2. User opens link → FE shows form → sends POST /users/reset-password.

### Resend Verification
User hasn’t verified yet? Call POST /auth/resend-verification to get a fresh link.

---

## ✅ 5️⃣ Security Best Practices

- Access Token: Short-lived (15–30 mins), signed JWT.
- Refresh Token: Long-lived (7–30 days), stored in DB, supports revocation.
- Email Verification: Required (Flag = T) to allow login.
- Mail: Uses MailKit + HTML templates in /EmailTemplates/.

---

## ✅ 6️⃣ Project Structure

| Layer | Description |
|-------|--------------|
| Domain | Core entities: User, UserVerification, RefreshToken |
| Application | Interfaces, DTOs, core services |
| Infrastructure | Repositories for data access |
| API | Controllers: AuthController, UserController |
| Email Service | MailKit + HTML templates (VerificationEmailTemplate.html, ResetPasswordEmailTemplate.html) |

---

## ✅ 7️⃣ Configuration

| Key | Where | Purpose |
|-----|-------|---------|
| Connection String | Settings:ConnectionString | Database |
| JWT Settings | Jwt:Key, Jwt:Issuer, Jwt:Audience | Token signing |
| SMTP Settings | Email:From, Email:SmtpHost, Email:SmtpPort, Email:SmtpUser, Email:SmtpPass | Email delivery |

---

## ✅ 8️⃣ Suggested Enhancements

| Idea | Notes |
|------|-------|
| Resend Verification | ✅ Implemented |
| Admin CRUD | Use /users/{id} for admin operations |
| 2FA/OTP | Optional for stronger security |
| Audit Logging | Recommended for user activity tracking |
| Rate Limiting | Prevent brute-force attacks |

---

## ✅ 9️⃣ Getting Started

- Clone the repo
- Configure appsettings.json:
  - Database connection
  - JWT secret & issuer
  - SMTP for email
- Run migrations for User, UserVerification, RefreshToken
- Test with Swagger or Postman

---

## ✅ 10️⃣ Contacts

For contributions, suggestions, or to expand with Google OAuth or Social Login — feel free to reach out or open an issue!

🚀 **Ready to scale your user authentication with confidence!**
