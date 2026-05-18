-- ============================================================
-- AIUB Library Management System - Complete Database Script
-- Run this in SQL Server Management Studio (SSMS)
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'library')
    CREATE DATABASE library;
GO

USE library;
GO

-- ============================================================
-- STEP 1: DROP ALL FOREIGN KEYS FIRST
-- ============================================================
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(TABLE_NAME) +
               ' DROP CONSTRAINT ' + QUOTENAME(CONSTRAINT_NAME) + '; '
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'
  AND TABLE_SCHEMA = 'dbo';
EXEC sp_executesql @sql;
GO

-- ============================================================
-- STEP 2: DROP ALL TABLES
-- ============================================================
IF OBJECT_ID('BorrowRequest', 'U') IS NOT NULL DROP TABLE BorrowRequest;
IF OBJECT_ID('BookIssue',     'U') IS NOT NULL DROP TABLE BookIssue;
IF OBJECT_ID('Books',         'U') IS NOT NULL DROP TABLE Books;
IF OBJECT_ID('Category',      'U') IS NOT NULL DROP TABLE Category;
IF OBJECT_ID('Student',       'U') IS NOT NULL DROP TABLE Student;
IF OBJECT_ID('Librarian',     'U') IS NOT NULL DROP TABLE Librarian;
IF OBJECT_ID('Admin',         'U') IS NOT NULL DROP TABLE Admin;
-- Old tables cleanup
IF OBJECT_ID('NewStudent',    'U') IS NOT NULL DROP TABLE NewStudent;
IF OBJECT_ID('IRBook',        'U') IS NOT NULL DROP TABLE IRBook;
IF OBJECT_ID('NewBook',       'U') IS NOT NULL DROP TABLE NewBook;
IF OBJECT_ID('logintable',    'U') IS NOT NULL DROP TABLE logintable;
GO

-- ============================================================
-- STEP 3: CREATE TABLES
-- ============================================================

-- 1. Admin
CREATE TABLE Admin (
    AdminID       INT PRIMARY KEY IDENTITY(1,1),
    Username      VARCHAR(50)  NOT NULL UNIQUE,
    FirstName     VARCHAR(50)  NOT NULL,
    LastName      VARCHAR(50),
    Email         VARCHAR(100),
    Password      VARCHAR(255) NOT NULL,
    CreatedDate   DATETIME     DEFAULT GETDATE(),
    LastLogin     DATETIME,
    IsActive      BIT          DEFAULT 1
);
GO

-- 2. Librarian
CREATE TABLE Librarian (
    LibrarianID   INT PRIMARY KEY IDENTITY(1,1),
    Username      VARCHAR(50)  NOT NULL UNIQUE,
    FirstName     VARCHAR(50)  NOT NULL,
    LastName      VARCHAR(50),
    Email         VARCHAR(100),
    PhoneNo       VARCHAR(15),
    Password      VARCHAR(255) NOT NULL,
    CreatedDate   DATETIME     DEFAULT GETDATE(),
    LastLogin     DATETIME,
    IsActive      BIT          DEFAULT 1
);
GO

-- 3. Student
CREATE TABLE Student (
    StudentID      INT PRIMARY KEY IDENTITY(1,1),
    EnrollmentNo   VARCHAR(50)  NOT NULL UNIQUE,
    FirstName      VARCHAR(50)  NOT NULL,
    LastName       VARCHAR(50),
    Email          VARCHAR(100),
    PhoneNo        VARCHAR(15),
    Department     VARCHAR(50),
    Semester       INT,
    Address        VARCHAR(255),
    Password       VARCHAR(255) NOT NULL,
    CreatedDate    DATETIME     DEFAULT GETDATE(),
    IsActive       BIT          DEFAULT 1
);
GO

-- 4. Category
CREATE TABLE Category (
    CategoryID    INT PRIMARY KEY IDENTITY(1,1),
    CategoryName  VARCHAR(100) NOT NULL UNIQUE,
    Description   VARCHAR(255),
    CreatedDate   DATETIME     DEFAULT GETDATE(),
    IsActive      BIT          DEFAULT 1
);
GO

-- 5. Books
CREATE TABLE Books (
    BookID             INT PRIMARY KEY IDENTITY(1,1),
    BookName           VARCHAR(255) NOT NULL,
    Author             VARCHAR(100) NOT NULL,
    ISBN               VARCHAR(50),
    Publication        VARCHAR(100),
    PublicationDate    DATE,
    CategoryID         INT,
    TotalQuantity      INT          DEFAULT 0,
    AvailableQuantity  INT          DEFAULT 0,
    Price              DECIMAL(10,2),
    AddedBy            INT,
    CreatedDate        DATETIME     DEFAULT GETDATE(),
    LastModified       DATETIME,
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID),
    FOREIGN KEY (AddedBy)    REFERENCES Admin(AdminID)
);
GO

-- 6. BookIssue
CREATE TABLE BookIssue (
    IssueID        INT PRIMARY KEY IDENTITY(1,1),
    BookID         INT           NOT NULL,
    StudentID      INT           NOT NULL,
    IssuedDate     DATETIME      DEFAULT GETDATE(),
    DueDate        DATE          NOT NULL,
    ReturnedDate   DATETIME      NULL,
    FineAmount     DECIMAL(10,2) DEFAULT 0,
    Status         VARCHAR(20)   DEFAULT 'Issued',
    Remarks        VARCHAR(500),
    IssuedBy       INT,
    FOREIGN KEY (BookID)    REFERENCES Books(BookID),
    FOREIGN KEY (StudentID) REFERENCES Student(StudentID),
    FOREIGN KEY (IssuedBy)  REFERENCES Librarian(LibrarianID)
);
GO

-- 7. BorrowRequest
CREATE TABLE BorrowRequest (
    RequestID      INT PRIMARY KEY IDENTITY(1,1),
    BookID         INT          NOT NULL,
    StudentID      INT          NOT NULL,
    RequestDate    DATETIME     DEFAULT GETDATE(),
    Status         VARCHAR(20)  DEFAULT 'Pending',
    ProcessedBy    INT          NULL,
    ProcessedDate  DATETIME     NULL,
    Remarks        VARCHAR(500),
    FOREIGN KEY (BookID)      REFERENCES Books(BookID),
    FOREIGN KEY (StudentID)   REFERENCES Student(StudentID),
    FOREIGN KEY (ProcessedBy) REFERENCES Librarian(LibrarianID)
);
GO

-- ============================================================
-- STEP 4: INSERT DEFAULT DATA
-- ============================================================

-- Admin: admin / admin123
INSERT INTO Admin (Username, FirstName, LastName, Email, Password, IsActive)
VALUES ('admin', 'System', 'Admin', 'admin@aiub.edu', 'admin123', 1);
GO

-- Librarian: librarian1 / lib123
INSERT INTO Librarian (Username, FirstName, LastName, Email, PhoneNo, Password, IsActive)
VALUES ('librarian1', 'Fatima', 'Rahman', 'fatima@aiub.edu', '01711111111', 'lib123', 1);
GO

-- Categories
INSERT INTO Category (CategoryName, Description) VALUES
('Computer Science',  'Programming, algorithms, data structures'),
('Mathematics',       'Calculus, algebra, statistics'),
('Physics',           'Classical and modern physics'),
('Business',          'Management, economics, finance'),
('Literature',        'Fiction, poetry, drama'),
('Engineering',       'Civil, electrical, mechanical engineering'),
('Science Fiction',   'Sci-fi novels and stories'),
('Reference',         'Dictionaries, encyclopedias, atlases');
GO

-- Sample Books
INSERT INTO Books (BookName, Author, ISBN, Publication, CategoryID, TotalQuantity, AvailableQuantity, Price, AddedBy)
VALUES
('Introduction to Algorithms',      'Cormen, Leiserson',  '978-0262033848', 'MIT Press',     1, 5, 5,  850.00, 1),
('Clean Code',                       'Robert C. Martin',   '978-0132350884', 'Prentice Hall', 1, 3, 3,  650.00, 1),
('C# Programming in Depth',         'Jon Skeet',          '978-1617294532', 'Manning',        1, 4, 4,  720.00, 1),
('Calculus: Early Transcendentals', 'James Stewart',      '978-1285741550', 'Cengage',        2, 6, 6,  980.00, 1),
('University Physics',              'Young & Freedman',   '978-0133969290', 'Pearson',        3, 4, 4,  870.00, 1),
('Principles of Management',        'Robbins & Coulter',  '978-0134527604', 'Pearson',        4, 3, 3,  750.00, 1),
('The Great Gatsby',                'F. Scott Fitzgerald','978-0743273565', 'Scribner',       5, 5, 5,  350.00, 1),
('Database System Concepts',        'Silberschatz et al', '978-0078022159', 'McGraw Hill',    1, 4, 4,  920.00, 1),
('Data Structures Using C',         'Reema Thareja',      '978-0199459155', 'Oxford',         1, 5, 5,  580.00, 1),
('Artificial Intelligence',         'Stuart Russell',     '978-0136042594', 'Pearson',        1, 3, 3, 1100.00, 1);
GO

-- Sample Student: 21-41234-1 / student123
INSERT INTO Student (EnrollmentNo, FirstName, LastName, Email, PhoneNo, Department, Semester, Password, IsActive)
VALUES ('21-41234-1', 'Rahel', 'Ahmed', 'rahel@aiub.edu', '01800000000', 'CSE', 6, 'student123', 1);
GO

-- ============================================================
-- VERIFY
-- ============================================================
SELECT 'Admin'     AS [Table], COUNT(*) AS Records FROM Admin     UNION ALL
SELECT 'Librarian',             COUNT(*)            FROM Librarian UNION ALL
SELECT 'Student',               COUNT(*)            FROM Student   UNION ALL
SELECT 'Category',              COUNT(*)            FROM Category  UNION ALL
SELECT 'Books',                 COUNT(*)            FROM Books;
GO

PRINT '========================================';
PRINT 'Database setup COMPLETE!';
PRINT 'Admin     -> admin / admin123';
PRINT 'Librarian -> librarian1 / lib123';
PRINT 'Student   -> 21-41234-1 / student123';
PRINT '========================================';
