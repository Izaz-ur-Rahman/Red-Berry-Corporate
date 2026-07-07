using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces
{
    public interface IBlueprintRepository
    {
        Task<BlueprintSubmission> CreateAsync(BlueprintSubmission blueprint);

        Task<BlueprintSubmission?> GetByIdAsync(int id);

        Task<(List<BlueprintSubmission>, int)> GetPagedAsync(int pageNumber, int pageSize, string? search);

        Task UpdateAsync(BlueprintSubmission blueprint);

        Task DeleteAsync(BlueprintSubmission blueprint);
    }
}