using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnComics.Application.Enums.Comic;
using OnComics.Application.Services.Interfaces;

namespace OnComics.Application.Scheduler
{
    public class OnComicsScheduler : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private DateTime LastDailyRun = DateTime.MinValue;
        private DateTime LastWeeklyRun = DateTime.MinValue;
        private DateTime LastMonthlyRun = DateTime.MinValue;

        public OnComicsScheduler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                if (now.Hour == 0 && now.Minute == 0)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var comicService = scope.ServiceProvider.GetRequiredService<IComicService>();

                    if (LastDailyRun.Date != now.Date)
                    {
                        await comicService.ResetReadNumAsync(ComicReadNumType.DAY);
                        LastDailyRun = now;
                    }

                    if (now.DayOfWeek == DayOfWeek.Monday &&
                        LastWeeklyRun.Date != now.Date)
                    {
                        await comicService.ResetReadNumAsync(ComicReadNumType.WEEK);
                        LastWeeklyRun = now;
                    }

                    if (now.Day == 1 &&
                        LastMonthlyRun.Month != now.Month)
                    {
                        await comicService.ResetReadNumAsync(ComicReadNumType.MONTH);
                        LastMonthlyRun = now;
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
