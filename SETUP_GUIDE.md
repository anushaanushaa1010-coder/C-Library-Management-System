# AIUB Library Management System - Setup Guide

## ✅ Project Completion Status

### Database Setup
- **Status**: ✅ Complete
- **Database Name**: `library`
- **Server**: `localhost`
- **Tables Created**:
  - Student (Students with enrollment)
  - Librarian (Library staff)
  - Admin (System administrators)
  - Books (Book inventory)
  - BookIssue (Book issue tracking)

### Default Login Credentials

#### Admin Login
- **Username**: admin
- **Password**: admin123

#### Librarian Login
- **Username**: lib001
- **Password**: lib123

#### Student Registration
- Students can self-register via the Registration page
- Default password can be set during registration

### Application Features

#### Student Dashboard (`UserDashboard.cs`)
- ✅ View issued books
- ✅ Browse available books for issuing
- ✅ Track pending book returns
- ✅ View personal information
- ✅ Logout functionality

#### Librarian Dashboard (`LibrarianDashboard.cs`)
- ✅ Add new books to system
- ✅ Issue books to students
- ✅ View dashboard statistics
  - Total students
  - Total books
  - Books issued
- ✅ Logout functionality

#### Admin Dashboard (`AdminDashboard.cs`)
- ✅ View all students with delete option
- ✅ View all books with delete option
- ✅ Add new librarian accounts
- ✅ Dashboard statistics
  - Total students
  - Total books
  - Books issued
- ✅ Logout functionality

### Branding Changes
- ✅ Changed from "KIIT" to "AIUB"
  - Updated LoginForm header
  - Updated Form1 website link to AIUB
- ✅ AIUB Blue color scheme maintained throughout
  - Header: RGB(37, 39, 76) - Dark AIUB Blue
  - Professional design maintained

### Picture/Image Support
- ✅ Created `picture` folder at project root
- ✅ ImageHelper utility class added for image management
- ✅ Support for loading images from `picture` folder
- ✅ Placeholder image generation for missing images

### Security Features
- ✅ Parameterized SQL queries (SQL injection prevention)
- ✅ Centralized DatabaseHelper class
- ✅ Proper resource management using `using` statements
- ✅ Plain text passwords (noted for production hardening)

### File Structure

```
Project Root/
├── AdminDashboard.cs          (Admin dashboard form)
├── LibrarianDashboard.cs      (Librarian dashboard form)
├── UserDashboard.cs           (Student dashboard form)
├── LoginForm.cs               (Main login form - all user types)
├── AddStudent.cs              (Student registration form)
├── DatabaseHelper.cs          (Centralized DB helper)
├── ImageHelper.cs             (Image loading utility)
├── AdminTableScript.sql       (Database schema & initial data)
├── AdminLogin.cs              (Admin login form - legacy, using new LoginForm)
├── Program.cs                 (Application entry point)
├── App.config                 (Application configuration)
└── picture/                   (Image folder)
```

## How to Use

### 1. Database Setup
Run the SQL script to create the database:

```powershell
sqlcmd -S localhost -E -C -i "AdminTableScript.sql"
```

Or manually execute `AdminTableScript.sql` in SQL Server Management Studio.

### 2. Start the Application
1. Open the project in Visual Studio
2. Build the solution (Build → Build Solution)
3. Run the application (F5 or Debug → Start Debugging)
4. The LoginForm will appear

### 3. Login as Different User Types

**As Admin:**
- Select "Admin" from User Type dropdown
- Username: admin
- Password: admin123

**As Librarian:**
- Select "Librarian" from User Type dropdown
- Username: lib001
- Password: lib123

**As Student:**
- Select "Student" from User Type dropdown
- Either register a new student or use existing credentials

### 4. Student Registration
1. Click "Register" button on LoginForm
2. Fill in all required fields:
   - Enrollment Number
   - First Name, Last Name
   - Email, Phone
   - Department, Semester
   - Address
   - Password
3. Click "Save" to register

### 5. Add Images
Place your images in the `picture` folder in the project root directory. The ImageHelper utility will automatically load them.

## Key Classes

### DatabaseHelper.cs
Centralized database operations:
- `GetConnection()` - Creates SQL connection
- `ExecuteNonQuery()` - INSERT, UPDATE, DELETE operations
- `ExecuteScalar()` - Returns single value
- `ExecuteQuery()` - Returns DataTable
- `ExecuteDataSet()` - Returns DataSet

### ImageHelper.cs
Image management utility:
- `LoadImage()` - Load image from picture folder
- `SetPictureBoxImage()` - Set PictureBox image
- `GetPicturePath()` - Get full path to image file
- `EnsurePictureFolderExists()` - Create folder if missing

## Troubleshooting

### Database Connection Issues
1. Ensure SQL Server is running on localhost
2. Verify "library" database exists
3. Check connection string in DatabaseHelper.cs:
   ```
   Data Source=localhost;Initial Catalog=library;Integrated Security=True;
   ```

### SSL Certificate Error
If you get SSL error during sqlcmd:
```powershell
sqlcmd -S localhost -E -C -i "AdminTableScript.sql"
```
The `-C` flag disables encryption for local connections.

### Missing Picture Folder
The application will create the `picture` folder automatically if it doesn't exist. You can also create it manually:
```powershell
mkdir picture
```

## Next Steps / Future Enhancements

- [ ] Implement password hashing (bcrypt or similar)
- [ ] Add email notifications for book return deadlines
- [ ] Implement book fine calculation
- [ ] Add book search and filter functionality
- [ ] Implement user profile editing
- [ ] Add book reservation system
- [ ] Implement PDF generation for issue receipts
- [ ] Add audit logging
- [ ] Implement role-based access control (RBAC)

## Development Notes

- Built with .NET Framework 4.8
- WinForms application
- SQL Server database
- Uses parameterized queries for security
- All 3 user types fully implemented
- Code-first form implementations (no Designer conflicts)

---

**Last Updated**: January 2025
**Status**: Ready for Testing
**Build**: ✅ Successful
