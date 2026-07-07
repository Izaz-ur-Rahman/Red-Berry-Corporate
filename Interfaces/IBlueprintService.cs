using RedBerryCorporate.DTOs.Blueprint;

namespace RedBerryCorporate.Interfaces
{
    public interface IBlueprintService
    {
        Task<BlueprintResponseDto> CreateAsync(BlueprintCreateDto dto);

        Task<BlueprintResponseDto?> GetByIdAsync(int id);

        Task<(List<BlueprintResponseDto> Data, int TotalRecords)> GetPagedAsync(BlueprintListRequestDto dto);

        Task<bool> UpdateAsync(BlueprintUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}