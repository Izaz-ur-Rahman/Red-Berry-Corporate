namespace RedBerryCorporate.DTOs.Blueprint
{
    public class BlueprintListRequestDto
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }
    }
}