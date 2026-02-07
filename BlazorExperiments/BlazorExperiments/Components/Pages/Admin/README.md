# ASP.NET Identity Admin Management Pages

This folder contains comprehensive admin pages for managing ASP.NET Identity tables in your Blazor application.

## Features

### 1. **User Management** (`/admin/users`)
- View all users in a searchable table
- Create new users with username, email, password, and phone number
- Edit existing users (username, email, phone, email confirmation, 2FA settings)
- Delete users
- Navigate to user roles and user claims management

### 2. **Role Management** (`/admin/roles`)
- View all roles
- Create new roles
- Edit role names
- Delete roles
- Navigate to role claims management

### 3. **User Roles Management** (`/admin/users/{userId}/roles`)
- View all available roles
- Assign roles to specific users
- Remove roles from users
- Visual indication of assigned roles

### 4. **User Claims Management** (`/admin/users/{userId}/claims`)
- View all claims for a specific user
- Add new claims (type and value)
- Remove claims from users

### 5. **Role Claims Management** (`/admin/roles/{roleId}/claims`)
- View all claims for a specific role
- Add new claims to roles
- Remove claims from roles

### 6. **Admin Dashboard** (`/admin`)
- Central hub for all admin operations
- Quick access to all management pages
- Feature overview

## Setup Instructions

### 1. Role Configuration

The admin pages require users to have the "Admin" role. You need to:

1. Create the "Admin" role in your database
2. Assign the "Admin" role to your admin users

You can do this via a migration, seed data, or manually through SQL:

```csharp
// Example seed data in ApplicationDbContext.cs
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    // Seed Admin Role
    var adminRoleId = "admin-role-id";
    builder.Entity<IdentityRole>().HasData(new IdentityRole
    {
        Id = adminRoleId,
        Name = "Admin",
        NormalizedName = "ADMIN"
    });

    // Optionally seed an admin user and assign the role
}
```

### 2. Authorization

All admin pages are protected with `[Authorize(Roles = "Admin")]`. Users without the Admin role will be denied access.

### 3. Navigation

Add links to these pages in your navigation menu for easy access:

```razor
<MudNavLink Href="/admin" Icon="@Icons.Material.Filled.AdminPanelSettings">
    Admin
</MudNavLink>
```

## Technology Stack

- **Blazor Server** with Interactive Server render mode
- **MudBlazor** for UI components
- **ASP.NET Identity** for user management
- **Entity Framework Core** for database operations

## Pages Overview

| Page | Route | Description |
|------|-------|-------------|
| Dashboard | `/admin` | Admin landing page with quick actions |
| Users | `/admin/users` | Manage all users |
| Roles | `/admin/roles` | Manage all roles |
| User Roles | `/admin/users/{userId}/roles` | Manage roles for a specific user |
| User Claims | `/admin/users/{userId}/claims` | Manage claims for a specific user |
| Role Claims | `/admin/roles/{roleId}/claims` | Manage claims for a specific role |

## Key Features

- **Search Functionality**: Search users and roles by name/email
- **MudBlazor Dialogs**: Clean modal dialogs for create/edit operations
- **Breadcrumb Navigation**: Easy navigation back to parent pages
- **Visual Feedback**: Snackbar notifications for success/error messages
- **Responsive Design**: Works on all screen sizes
- **Role-based Authorization**: Secure access control

## Identity Tables Managed

These pages provide full CRUD operations for the following ASP.NET Identity tables:

1. **AspNetUsers** - User accounts
2. **AspNetRoles** - Roles
3. **AspNetUserRoles** - User-Role relationships
4. **AspNetUserClaims** - User-specific claims
5. **AspNetRoleClaims** - Role-based claims

## Usage Examples

### Creating a User
1. Navigate to `/admin/users`
2. Click "Create User" button
3. Fill in username, email, password, and optional phone number
4. Optionally mark email as confirmed
5. Click "Create"

### Assigning Roles to Users
1. Navigate to `/admin/users`
2. Click the shield icon next to a user
3. Click on role chips to assign/remove roles
4. Changes are saved immediately

### Managing Claims
1. Navigate to user or role claims page
2. Click "Add Claim"
3. Enter claim type (e.g., "Permission", "Department")
4. Enter claim value (e.g., "CanEdit", "HR")
5. Click "Add"

## Security Considerations

- All pages require "Admin" role
- Password creation follows Identity password policy
- Consider implementing audit logging for admin actions
- Implement proper backup procedures before bulk deletions

## Customization

You can customize these pages by:
- Modifying MudBlazor theme colors
- Adding additional user properties
- Implementing custom validation rules
- Adding audit logging
- Extending with additional features (user lockout management, password reset, etc.)

## Dependencies

Ensure these packages are installed in your project:
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `MudBlazor`
- `Microsoft.EntityFrameworkCore.SqlServer`

These are already included in your project based on the `.csproj` file.
