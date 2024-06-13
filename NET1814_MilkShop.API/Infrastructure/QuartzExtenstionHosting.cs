using NET1814_MilkShop.Services.BackgroundJobs;
using Quartz;

namespace NET1814_MilkShop.API.Infrastructure;

public class QuartzExtenstionHosting
{

    public static void AddQuartzBackgroundJobs(IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();

            //Thêm BackgroundJob ở đây nhé ae

            var checkPaymentStatusJobKey = JobKey.Create(nameof(CheckPaymentStatusJob));
            options.AddJob<CheckPaymentStatusJob>(checkPaymentStatusJobKey)
                .AddTrigger(trigger => trigger.ForJob(checkPaymentStatusJobKey).WithSimpleSchedule(
                    schedule => schedule.WithIntervalInSeconds(60).RepeatForever()));

            //--------------------------------//
        });

        services.AddQuartzHostedService(options =>
            options.WaitForJobsToComplete = true
        );
    }
}