using System;

namespace Library_Management_System
{
    /// <summary>
    /// Stores the currently logged-in user's info.
    /// Set on login, clear on logout.
    /// </summary>
    public static class SessionManager
    {
        public static int    CurrentUserID   { get; set; }
        public static string CurrentUserName { get; set; }  // login username
        public static string CurrentUser     { get; set; }  // alias — same as CurrentUserName
        public static string CurrentFullName { get; set; }
        public static string CurrentEmail    { get; set; }
        public static string CurrentRole     { get; set; }  // "Admin" | "Librarian" | "Student"
        public static string EnrollmentNo    { get; set; }  // Student only

        public static bool IsLoggedIn => CurrentUserID > 0;

        public static void Login(int id, string username, string fullName, string email, string role, string enrollment = "")
        {
            CurrentUserID   = id;
            CurrentUserName = username;
            CurrentUser     = username;
            CurrentFullName = fullName;
            CurrentEmail    = email;
            CurrentRole     = role;
            EnrollmentNo    = enrollment;
        }

        public static void Logout()
        {
            CurrentUserID   = 0;
            CurrentUserName = string.Empty;
            CurrentUser     = string.Empty;
            CurrentFullName = string.Empty;
            CurrentEmail    = string.Empty;
            CurrentRole     = string.Empty;
            EnrollmentNo    = string.Empty;
        }

        public static bool IsAdmin     => CurrentRole == "Admin";
        public static bool IsLibrarian => CurrentRole == "Librarian";
        public static bool IsStudent   => CurrentRole == "Student";
    }
}
