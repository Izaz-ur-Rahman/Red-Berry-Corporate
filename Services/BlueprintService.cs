using Newtonsoft.Json;
using RedBerryCorporate.DTOs.Blueprint;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Services
{
    public class BlueprintService : IBlueprintService
    {
        private readonly IBlueprintRepository _repository;

        public BlueprintService(IBlueprintRepository repository)
        {
            _repository = repository;
        }

        #region Create

        public async Task<BlueprintResponseDto> CreateAsync(BlueprintCreateDto dto)
        {
            BlueprintSubmission entity = new BlueprintSubmission
            {
                Name = dto.Name,
                Email = dto.Email,
                Whatsapp = dto.Whatsapp,
                Company = dto.Company,
                Location = dto.Location,
                BusinessStage = dto.BusinessStage,
                ReviewMethod = dto.ReviewMethod,
                Message = dto.Message,

                Ambition = dto.Result.Ambition,

                TotalScore = dto.Result.TotalScore,

                CorporateScore = dto.Result.CorporateScore,

                FinancialScore = dto.Result.FinancialScore,

                MarketScore = dto.Result.MarketScore,

                LegacyScore = dto.Result.LegacyScore,

                StrongestLayer = dto.Result.StrongestLayer,

                ExposedLayer = dto.Result.ExposedLayer,

                RecommendedPathway = dto.Result.RecommendedPathway,

                OverallStatus = dto.Result.OverallStatus,

                ImprovementLayers = JsonConvert.SerializeObject(dto.Result.ImprovementLayers),

                PatternsJson = JsonConvert.SerializeObject(dto.Result.Patterns)
            };

            entity = await _repository.CreateAsync(entity);

            return MapToResponse(entity);
        }

        #endregion

        #region GetById

        public async Task<BlueprintResponseDto?> GetByIdAsync(int id)
        {
            BlueprintSubmission? entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                return null;

            return MapToResponse(entity);
        }

        #endregion

        #region List

        public async Task<(List<BlueprintResponseDto> Data, int TotalRecords)>
            GetPagedAsync(BlueprintListRequestDto dto)
        {
            var result = await _repository.GetPagedAsync(
                dto.PageNumber,
                dto.PageSize,
                dto.Search);

            return
            (
                result.Item1
                    .Select(MapToResponse)
                    .ToList(),

                result.Item2
            );
        }

        #endregion

        #region Update

        public async Task<bool> UpdateAsync(BlueprintUpdateDto dto)
        {
            BlueprintSubmission? entity =
                await _repository.GetByIdAsync(dto.Id);

            if (entity == null)
                return false;

            entity.Name = dto.Name;
            entity.Email = dto.Email;
            entity.Whatsapp = dto.Whatsapp;
            entity.Company = dto.Company;
            entity.Location = dto.Location;
            entity.BusinessStage = dto.BusinessStage;
            entity.ReviewMethod = dto.ReviewMethod;
            entity.Message = dto.Message;

            entity.Ambition = dto.Result.Ambition;
            entity.TotalScore = dto.Result.TotalScore;
            entity.CorporateScore = dto.Result.CorporateScore;
            entity.FinancialScore = dto.Result.FinancialScore;
            entity.MarketScore = dto.Result.MarketScore;
            entity.LegacyScore = dto.Result.LegacyScore;
            entity.StrongestLayer = dto.Result.StrongestLayer;
            entity.ExposedLayer = dto.Result.ExposedLayer;
            entity.RecommendedPathway = dto.Result.RecommendedPathway;
            entity.OverallStatus = dto.Result.OverallStatus;

            entity.ImprovementLayers =
                JsonConvert.SerializeObject(dto.Result.ImprovementLayers);

            entity.PatternsJson =
                JsonConvert.SerializeObject(dto.Result.Patterns);

            await _repository.UpdateAsync(entity);

            return true;
        }

        #endregion

        #region Delete

        public async Task<bool> DeleteAsync(int id)
        {
            BlueprintSubmission? entity =
                await _repository.GetByIdAsync(id);

            if (entity == null)
                return false;

            await _repository.DeleteAsync(entity);

            return true;
        }

        #endregion

        #region Mapper

        private BlueprintResponseDto MapToResponse(BlueprintSubmission entity)
        {
            return new BlueprintResponseDto
            {
                Id = entity.Id,

                Name = entity.Name,
                Email = entity.Email,
                Whatsapp = entity.Whatsapp,
                Company = entity.Company,
                Location = entity.Location,

                BusinessStage = entity.BusinessStage,
                ReviewMethod = entity.ReviewMethod,

                Message = entity.Message,

                CreatedAt = entity.CreatedAt,

                Result = new BlueprintResultDto
                {
                    Ambition = entity.Ambition,

                    TotalScore = entity.TotalScore,

                    CorporateScore = entity.CorporateScore,

                    FinancialScore = entity.FinancialScore,

                    MarketScore = entity.MarketScore,

                    LegacyScore = entity.LegacyScore,

                    StrongestLayer = entity.StrongestLayer,

                    ExposedLayer = entity.ExposedLayer,

                    RecommendedPathway = entity.RecommendedPathway,

                    OverallStatus = entity.OverallStatus,

                    ImprovementLayers =
                        string.IsNullOrEmpty(entity.ImprovementLayers)
                        ? new List<string>()
                        : JsonConvert.DeserializeObject<List<string>>(entity.ImprovementLayers)!,

                    Patterns =
                        string.IsNullOrEmpty(entity.PatternsJson)
                        ? new List<BlueprintPatternDto>()
                        : JsonConvert.DeserializeObject<List<BlueprintPatternDto>>(entity.PatternsJson)!
                }
            };
        }

        #endregion
    }
}