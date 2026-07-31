using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.DTOs.Dashboard;
using RedBerryCorporate.Enums;
using RedBerryCorporate.Interfaces.Dashboard;

namespace RedBerryCorporate.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardResponseDto> GetDashboardAsync()
        {
            var contactCount =
    await _context.Contacts.CountAsync(x => !x.IsDeleted);

            var blueprintCount =
                await _context.BlueprintSubmissions.CountAsync();

            return new DashboardResponseDto
            {
                PublishedBlogs =
                    await _context.Blogs.CountAsync(x =>
                        x.Status == BlogStatus.Published &&
                        !x.IsDeleted),

                DraftBlogs =
                    await _context.Blogs.CountAsync(x =>
                        x.Status == BlogStatus.Draft &&
                        !x.IsDeleted),

                ScheduledBlogs =
                    await _context.Blogs.CountAsync(x =>
                        x.Status == BlogStatus.Scheduled &&
                        !x.IsDeleted),

                ArchivedBlogs =
                    await _context.Blogs.CountAsync(x =>
                        x.Status == BlogStatus.Archived &&
                        !x.IsDeleted),

                //ContactSubmissions =
                //    await _context.Contacts.CountAsync(x =>
                //        !x.IsDeleted),

                //BlueprintSubmissions =
                //    await _context.BlueprintSubmissions.CountAsync()
                ContactSubmissions = contactCount,

                BlueprintSubmissions = blueprintCount,

                AllSubmissions = contactCount + blueprintCount
            };
        }
    }
}