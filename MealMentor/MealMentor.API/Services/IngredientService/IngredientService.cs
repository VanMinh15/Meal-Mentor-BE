using AutoMapper;
using MealMentor.API.Repositories.IngredientRepository;
using MealMentor.Core.Domain.Entities;
using MealMentor.Core.DTOs;
using MealMentor.Core.DTOs.ResultModel;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Services.IngredientService
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        public IngredientService(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        public async Task<ResultModel> GetIngredientById(int id)
        {
            var ingredient = await _ingredientRepository.GetIngredientById(id);
            try
            {


                if (ingredient == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        Message = "Ingredient not found",
                        Data = null
                    };
                }
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    Data = ingredient,
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    Message = e.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetIngredientList()
        {
            var ingredients = await _ingredientRepository.GetIngredientList();
            try
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    Data = ingredients,
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    Message = e.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> AddIngredient(Ingredient ingre)
        {
            var ingredient = new Ingredient
            {
                Name = ingre.Name,
                TranslatedName = ingre.TranslatedName,
                Description = ingre.Description,
                Nutrient = ingre.Nutrient,
                Measure = ingre.Measure,
                FoodMatch = ingre.FoodMatch,
                Food = ingre.Food,
                FoodId = ingre.FoodId,
                Weight = ingre.Weight,
                RetainedWeight = ingre.RetainedWeight,
                IsDeleted = ingre.IsDeleted,
                Url = ingre.Url
            };
            var result = await _ingredientRepository.AddIngredient(ingredient);
            try
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    Message = "Ingredient added",
                    Data = _mapper.Map<List<IngredientResponseDTO>>(result)
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    Message = e.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetIngredients(string? searchTerm, List<string>? whitelist, List<string>? blackList, PaginationParams pagingParams)
        {
            var ingredients = await _ingredientRepository.GetIngredientList();
            var query = ingredients.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.FoodMatch.ToLower().Trim().Contains(searchTerm.ToLower().Trim()));
            }

            if (blackList != null && blackList.Any())
            {
                query = query.Where(x => !blackList.Contains(x.FoodId));
            }

            //if (whitelist != null && whitelist.Any())
            //{
            //    query = query.Where(x => whitelist.Contains(x.FoodId));
            //}

            var totalItems = query.Count();
            var items = query.Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
                             .Take(pagingParams.PageSize)
                             .ToList();
            return new ResultModel
            {
                Data = _mapper.Map<List<IngredientResponseDTO>>(items),
                StatusCode = StatusCodes.Status200OK,
                IsSuccess = true,
                Message = "Get Successfully!"
            };
        }

        public async Task<ResultModel> UpdateIngredient(int id, string? description, string? name, string translatedName, string? image)
        {
            try
            {
                var ingr = await _ingredientRepository.GetIngredientById(id);
                if (ingr == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        Message = "Ingredient not found",
                        Data = null
                    };
                }
                ingr.Description = description ?? ingr.Description;
                ingr.Name = name ?? ingr.Name;
                ingr.TranslatedName = translatedName ?? ingr.TranslatedName;
                ingr.Url = image ?? ingr.Url;
                await _ingredientRepository.UpdateIngredient(ingr);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    Message = "Ingredient updated successfully",
                    Data = _mapper.Map<IngredientResponseDTO>(ingr)
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> DeleteIngredient(int id)
        {
            try
            {
                var ingr = await _ingredientRepository.GetIngredientById(id);
                if (ingr == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        Message = "Ingredient not found",
                        Data = null
                    };
                }
                ingr.IsDeleted = 0;
                await _ingredientRepository.UpdateIngredient(ingr);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    Message = "Ingredient deleted",
                    Data = _mapper.Map<IngredientResponseDTO>(ingr)
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
