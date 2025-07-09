using Microsoft.EntityFrameworkCore;
using ComputerTypingWebApp.Models;


namespace ComputerTypingWebApp.Models
{
    public class dbContext: DbContext
    {
        public dbContext(DbContextOptions<dbContext> options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Institute> Institute { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<Instructor> Instructor { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<Handicap> Handicap { get; set; } = null!;
        public DbSet<Course> Course { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<IdentityDoc>IdentityDoc { get; set; }
        public DbSet<CoursesUpload> CoursesUpload { get; set; }
        public DbSet<coursepractices> coursepractices { get; set; }
        public DbSet<Notices> Notices { get; set; }
        public DbSet<InstituteSessions> InstituteSessions { get; set; }
        public DbSet<InstituteTimings> InstituteTimings { get; set; }
        public DbSet<Coursefee> Coursefee { get; set; }
        public DbSet<GrNumber> GrNumber { get; set; }
        public DbSet<HallTicket> HallTicket { get; set; }
        public DbSet<studenttype> studenttype { get; set; }
        public DbSet<Receipts> Receipts { get; set; } = null!;
        public DbSet<Payment> Payment { get; set; }
        public DbSet<EnrolledSubject> EnrolledSubject { get; set; }
        public DbSet<TypingResult> TypingResult { get; set; }
        public DbSet<UserLogins> UserLogins { get; set; }
        public DbSet<speedPracticeUpload> speedPracticeUpload { get; set; }
        public DbSet<section> section { get; set; }
        public DbSet<mcqsection> mcqsection { get; set; }
        public DbSet<mcqquestions> mcqquestions { get; set; }
        public DbSet<typingtestschedule> typingtestschedule { get; set; }
        public DbSet<mcqtestschedule> mcqtestschedule { get; set; }
        public DbSet<mcqtestquestpaper> mcqtestquestpaper { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Use Fluent API to configure

            // Map entities to tables
            modelBuilder.Entity<Users>().ToTable("users");
            modelBuilder.Entity<Roles>().ToTable("roles");
            modelBuilder.Entity<Institute>().ToTable("Institute");
            modelBuilder.Entity<Students>().ToTable("Students");
            modelBuilder.Entity<Instructor>().ToTable("Instructor");
            modelBuilder.Entity<Gender>().ToTable("Gender");
            modelBuilder.Entity<Handicap>().ToTable("hadicap");
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Subject>().ToTable("Subject");
            modelBuilder.Entity<IdentityDoc>().ToTable("IdentityDoc");
            modelBuilder.Entity<CoursesUpload>().ToTable("CoursesUpload");
            modelBuilder.Entity<coursepractices>().ToTable("coursepractices");
            modelBuilder.Entity<Notices>().ToTable("Notices");
            modelBuilder.Entity<InstituteSessions>().ToTable("InstituteSessions");
            modelBuilder.Entity<InstituteTimings>().ToTable("InstituteTimings");
            modelBuilder.Entity<Coursefee>().ToTable("Coursefee");
            modelBuilder.Entity<GrNumber>().ToTable("GrNumber");
            modelBuilder.Entity<HallTicket>().ToTable("HallTicket");
            modelBuilder.Entity<studenttype>().ToTable("studenttype");
            modelBuilder.Entity<Receipts>().ToTable("Receipts");
            modelBuilder.Entity<Payment>().ToTable("Payment");
            modelBuilder.Entity<EnrolledSubject>().ToTable("EnrolledSubject");
            modelBuilder.Entity<TypingResult>().ToTable("TypingResult");
            modelBuilder.Entity<speedPracticeUpload>().ToTable("speedPracticeUpload");
            modelBuilder.Entity<section>().ToTable("section");
            modelBuilder.Entity<mcqsection>().ToTable("mcqsection");
            modelBuilder.Entity<mcqquestions>().ToTable("mcqquestions");
            modelBuilder.Entity<typingtestschedule>().ToTable("typingtestschedule");
            modelBuilder.Entity<mcqtestschedule>().ToTable("mcqtestschedule");
            modelBuilder.Entity<mcqtestquestpaper>().ToTable("mcqtestquestpaper");
        }
    }
}
