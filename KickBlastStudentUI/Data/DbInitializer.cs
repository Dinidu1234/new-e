using KickBlastStudentUI.Models;
using Microsoft.Extensions.Configuration;

namespace KickBlastStudentUI.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext db, IConfiguration configuration)
    {
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Username = "rashmika",
                PasswordPlain = "123456",
                CreatedAt = DateTime.Now
            });
        }

        if (!db.TrainingPlans.Any())
        {
            var beginner = configuration.GetValue<decimal>("Pricing:BeginnerWeeklyFee");
            var intermediate = configuration.GetValue<decimal>("Pricing:IntermediateWeeklyFee");
            var elite = configuration.GetValue<decimal>("Pricing:EliteWeeklyFee");

            db.TrainingPlans.AddRange(
                new TrainingPlan { Name = "Beginner", WeeklyFee = beginner },
                new TrainingPlan { Name = "Intermediate", WeeklyFee = intermediate },
                new TrainingPlan { Name = "Elite", WeeklyFee = elite }
            );
            db.SaveChanges();
        }

        if (!db.Athletes.Any())
        {
            var plans = db.TrainingPlans.ToDictionary(x => x.Name, x => x.Id);
            db.Athletes.AddRange(
                new Athlete { Name = "Nimal Perera", TrainingPlanId = plans["Beginner"], CurrentWeightKg = 63, CompetitionCategoryKg = 60, CreatedAt = DateTime.Now },
                new Athlete { Name = "Kasun Silva", TrainingPlanId = plans["Intermediate"], CurrentWeightKg = 70, CompetitionCategoryKg = 66, CreatedAt = DateTime.Now },
                new Athlete { Name = "Amila Fernando", TrainingPlanId = plans["Elite"], CurrentWeightKg = 75, CompetitionCategoryKg = 73, CreatedAt = DateTime.Now },
                new Athlete { Name = "Sajini Jayasuriya", TrainingPlanId = plans["Beginner"], CurrentWeightKg = 53, CompetitionCategoryKg = 52, CreatedAt = DateTime.Now },
                new Athlete { Name = "Dinuka Samaranayake", TrainingPlanId = plans["Intermediate"], CurrentWeightKg = 81, CompetitionCategoryKg = 81, CreatedAt = DateTime.Now },
                new Athlete { Name = "Ishara Madushani", TrainingPlanId = plans["Elite"], CurrentWeightKg = 58, CompetitionCategoryKg = 57, CreatedAt = DateTime.Now }
            );
        }

        db.SaveChanges();
    }
}
