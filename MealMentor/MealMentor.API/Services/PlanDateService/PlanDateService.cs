using AutoMapper;
using MealMentor.API.Repositories.PlanDateRepository;
using MealMentor.API.Repositories.RecipeRepository;
using MealMentor.API.Repositories.UserRepository;
using MealMentor.Core.Domain.Entities;
using MealMentor.Core.DTOs;
using MealMentor.Core.DTOs.ResultModel;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MealMentor.API.Services.PlanDateService
{
    public class PlanDateService : IPlanDateService
    {
        private readonly IPlanDateRepository _planDateRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly HttpClient client = new HttpClient();
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRecipeRepository _recipeRepository;
        public PlanDateService(IPlanDateRepository planDateRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IUserRepository userRepository, IRecipeRepository recipeRepository)
        {
            _planDateRepository = planDateRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _mapper = mapper;
            _recipeRepository = recipeRepository;
        }
        public async Task<ResultModel> CreatePlanDate(PlanDate planDate)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                planDate.CreatedBy = userId;
                var user = await _userRepository.GetUserById(userId);
                if (user == null) return new ResultModel()
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "No user found!",
                    StatusCode = 404
                };
                planDate.Accessibility = "Public";
                var result = await _planDateRepository.CreatePlanDate(planDate);
                List<PlanDateDetailResponseDTO> planDateDetailResponseDTOs = new List<PlanDateDetailResponseDTO>();
                foreach (var item in result.PlanDateDetails)
                {
                    var meal = JsonConvert.DeserializeObject<List<int>>(item.Meal);
                    List<RecipeResponseDTO> recipes = [];
                    foreach (var item1 in meal)
                    {
                        var tmp = await _recipeRepository.GetRecipeById(item1);
                        recipes.Add(_mapper.Map<RecipeResponseDTO>(tmp));
                    }
                    planDateDetailResponseDTOs.Add(new PlanDateDetailResponseDTO()
                    {
                        Meal = recipes,
                        Type = item.Type,
                    });
                }
                PlanDateResponseDTO data = new PlanDateResponseDTO()
                {
                    Id = result.Id,
                    PlanDate = result.CreateDateTime.Value,
                    Details = planDateDetailResponseDTOs
                };


                return new ResultModel()
                {
                    Data = data,
                    IsSuccess = true,
                    StatusCode = 201,
                    Message = "Create Successfully"

                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResultModel> DeletePlanDate(DateTime date)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var planDate = _planDateRepository.GetPlanDateDetail(userId, date.Date);
                if (await _userRepository.GetUserById(userId) == null) return new ResultModel()
                {
                    StatusCode = 400,
                    Data = null,
                    Message = "Invalid user!",
                    IsSuccess = false
                };
                if (planDate == null) return new ResultModel()
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "PlanDate not found!",
                    StatusCode = 404
                };

                _planDateRepository.DeletePlanDate(userId, date);

                return new ResultModel
                {
                    Data = planDate,
                    IsSuccess = true,
                    Message = "Deletion successful.",
                    StatusCode = 204 // Assuming 200 for successful deletion
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResultModel> GetPlanDateByUserId()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new ResultModel
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = "User ID not found.",
                        StatusCode = 404
                    };
                }

                var resultList = await _planDateRepository.GetPlanDatesByUserId(userId);
                List<PlanDateResponseDTO> dataList = new List<PlanDateResponseDTO>();
                foreach (var result in resultList)
                {
                    List<PlanDateDetailResponseDTO> planDateDetailResponseDTOs = new List<PlanDateDetailResponseDTO>();
                    foreach (var item in result.PlanDateDetails)
                    {
                        var meal = JsonConvert.DeserializeObject<List<int>>(item.Meal);
                        List<RecipeResponseDTO> recipes = [];
                        foreach (var item1 in meal)
                        {
                            var tmp = await _recipeRepository.GetRecipeById(item1);
                            recipes.Add(_mapper.Map<RecipeResponseDTO>(tmp));
                        }
                        planDateDetailResponseDTOs.Add(new PlanDateDetailResponseDTO()
                        {
                            Meal = recipes,
                            Type = item.Type,
                        });
                    }
                    PlanDateResponseDTO data = new PlanDateResponseDTO()
                    {
                        Id = result.Id,
                        PlanDate = result.CreateDateTime.Value,
                        Details = planDateDetailResponseDTOs
                    };
                    dataList.Add(data);
                }
                return new ResultModel
                {
                    Data = dataList,
                    IsSuccess = true,
                    Message = "Plan dates retrieved successfully.",
                    StatusCode = 200 // Successful retrieval
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResultModel> UpdatePlanDate(PlanDate planDate)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new ResultModel
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = "User ID not found.",
                        StatusCode = 404
                    };
                }

                planDate.CreatedBy = userId;
                var result = await _planDateRepository.UpdatePlanDate(planDate);
                List<PlanDateDetailResponseDTO> planDateDetailResponseDTOs = new List<PlanDateDetailResponseDTO>();
                foreach (var item in result.PlanDateDetails)
                {
                    var meal = JsonConvert.DeserializeObject<List<int>>(item.Meal);
                    List<RecipeResponseDTO> recipes = [];
                    foreach (var item1 in meal)
                    {
                        var tmp = await _recipeRepository.GetRecipeById(item1);
                        recipes.Add(_mapper.Map<RecipeResponseDTO>(tmp));
                    }
                    planDateDetailResponseDTOs.Add(new PlanDateDetailResponseDTO()
                    {
                        Meal = recipes,
                        Type = item.Type,
                    });
                }
                PlanDateResponseDTO data = new PlanDateResponseDTO()
                {
                    Id = result.Id,
                    PlanDate = result.CreateDateTime.Value,
                    Details = planDateDetailResponseDTOs
                };
                if (result != null)
                {
                    return new ResultModel
                    {
                        Data = data,
                        IsSuccess = true,
                        Message = "Update successful.",
                        StatusCode = 200 // Successful update
                    };
                }
                return new ResultModel
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Update failed.",
                    StatusCode = 500
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }
        public async Task<ResultModel> GetWeekIngredient()
        {
            try
            {
                var date = DateTime.Now;
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                while (date.DayOfWeek != DayOfWeek.Monday) date = date.AddDays(-1);
                var endMonday = date.AddDays(7).Date;
                List<List<string>> listIngr = [];
                do
                {
                    var plan = await _planDateRepository.GetPlanDateDetail(userId, date);
                    if (plan != null)
                    {
                        var ingrForDate = new List<string>();
                        foreach (var item in plan.PlanDateDetails)
                        {
                            var recipeIds = JsonConvert.DeserializeObject<List<string>>(item.Meal);
                            foreach (var id in recipeIds)
                            {
                                var recipe = await _recipeRepository.GetRecipeById(int.Parse(id));
                                var ingre = JsonConvert.DeserializeObject<IngredientGetDTO>(recipe.Ingredients);
                                ingrForDate.AddRange(ingre.vn.Count == 0 ? ingre.en : ingre.vn);
                            }
                        }
                        listIngr.Add(ingrForDate);
                    }
                    else
                    {
                        listIngr.Add([]);
                    }
                    date = date.AddDays(1);
                } while (date.Date != endMonday);
                return new ResultModel()
                {
                    Data = new
                    {
                        Monday = listIngr.ElementAt(0),
                        Tuesday = listIngr.ElementAt(1),
                        Wednesday = listIngr.ElementAt(2),
                        Thursday = listIngr.ElementAt(3),
                        Friday = listIngr.ElementAt(4),
                        Saturday = listIngr.ElementAt(5),
                        Sunday = listIngr.ElementAt(6),
                    },
                    IsSuccess = true,
                    Message = "Get Ingredients List Successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception e)
            {
                return new ResultModel()
                {
                    Data = null,
                    IsSuccess = false,
                    Message = e.Message,
                    StatusCode = 500,
                };
            }
        }

    }
}
