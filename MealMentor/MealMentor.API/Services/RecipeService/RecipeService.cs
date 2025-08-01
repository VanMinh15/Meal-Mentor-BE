using AutoMapper;
using MealMentor.API.Pagination;
using MealMentor.API.Repositories.IngredientRepository;
using MealMentor.API.Repositories.RecipeRepository;
using MealMentor.API.Repositories.UserRepository;
using MealMentor.API.Services.IngredientService;
using MealMentor.Core.Domain.Entities;
using MealMentor.Core.Domain.Enums;
using MealMentor.Core.DTOs;
using MealMentor.Core.DTOs.ResultModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Services.RecipeService
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly HttpClient client = new HttpClient();
        private readonly IIngredientService _ingredientService;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public RecipeService(IRecipeRepository recipeRepository, IHttpContextAccessor httpContextAccessor, IIngredientService ingredientService, IIngredientRepository ingredientRepository, IMapper mapper,
            IUserRepository userRepository)
        {
            _recipeRepository = recipeRepository;
            _httpContextAccessor = httpContextAccessor;
            _ingredientService = ingredientService;
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<ResultModel> CreateRecipe(RecipeDTO recipeDTO)
        {
            ResultModel result = new ResultModel();
            try
            {
                //var username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

                //if (string.IsNullOrEmpty(username))
                //{
                //    result.StatusCode = StatusCodes.Status401Unauthorized;
                //    result.IsSuccess = false;
                //    result.Message = "Unauthorized";
                //    return result;
                //}

                string edamamUrl = "https://api.edamam.com/api/nutrition-details?app_id=1cbc4d56&app_key=c5fd320b67bc74176b0a910cec834ddc";

                var json = JsonConvert.SerializeObject(new { ingr = recipeDTO.Ingredients });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(edamamUrl, content);
                response.EnsureSuccessStatusCode();
                var jsonObject = JObject.Parse(await response.Content.ReadAsStringAsync());
                List<IngredientsDTO> list = jsonObject["ingredients"].ToObject<List<IngredientsDTO>>();
                var listIngr = await _ingredientRepository.GetIngredientList();
                foreach (var item in list)
                {
                    if (item.Parsed != null)
                        foreach (var ingr in item.Parsed)
                        {
                            if (listIngr.FirstOrDefault(x => x.FoodId == ingr.FoodId) == null)
                                await _ingredientRepository.AddIngredient(new()
                                {
                                    FoodId = ingr.FoodId,
                                    FoodMatch = ingr.FoodMatch,
                                    Url = ingr.Image,
                                    Nutrient = JsonConvert.SerializeObject(ingr.Nutrients),
                                    IsDeleted = 1,
                                    Measure = ingr.Measure,
                                    Weight = ingr.Weight,
                                    Food = ingr.Food,
                                    RetainedWeight = ingr.RetainedWeight,
                                    Quantity = ingr.Quantity
                                });
                        }
                }
                var recipe = new Recipe
                {
                    TotalWeight = (double)(jsonObject["totalWeight"] ?? 0),
                    Name = recipeDTO.Name,
                    TranslatedName = recipeDTO.TranslatedName,
                    Description = recipeDTO.Description,
                    Instruction = recipeDTO.Instruction,
                    Preparation = recipeDTO.Preparation,
                    Url = new Guid().ToString(),
                    Image = recipeDTO.Image,
                    TotalNutrients = jsonObject["totalNutrients"].ToString(),
                    TotalDaily = jsonObject["totalDaily"].ToString(),
                    Ingredients = JsonConvert.SerializeObject(new { en = recipeDTO.Ingredients, vn = recipeDTO.TranslatedIngredients }),
                    IsDeleted = (int)RecipeStatus.False,
                    Accessibility = "Private",
                    Calories = (double)(jsonObject["calories"] ?? 0),
                    LikeQuantity = 0,
                    CreateDateTime = DateTime.Now,
                    CreatedBy = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                };
                var newRecipe = await _recipeRepository.AddRecipe(recipe);
                result.StatusCode = StatusCodes.Status201Created;
                result.IsSuccess = true;
                result.Message = "Recipe created";
                result.Data = _mapper.Map<RecipeResponseDTO>(newRecipe);
            }
            catch (Exception e)
            {
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.IsSuccess = false;
                result.Message = e.Message;
                result.Data = null;
            }
            return result;
        }

        public async Task<ResultModel> DeleteRecipe(int id)
        {
            try
            {
                var recipe = await _recipeRepository.GetRecipeById(id);
                if (recipe == null)
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "No recipe found matched",
                        Data = null
                    };
                }
                else
                {
                    await _recipeRepository.DeleteRecipe(id);
                    recipe.IsDeleted = 0;
                    return new ResultModel()
                    {
                        IsSuccess = true,
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Recipe deleted",
                        Data = _mapper.Map<RecipeResponseDTO>(recipe)
                    };
                }
            }
            catch (Exception e)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = e.Message,
                    Data = null
                };
            }
        }

        // API Edamam
        public async Task<ResultModel> SearchRecipe(string keyword, PaginationParams pagingParams)
        {
            ResultModel result = new ResultModel();
            try
            {
                string edamamUrl = "https://api.edamam.com/api/recipes/v2?type=any&beta=true&app_id=9d914e48&app_key=675592435664a1da4a4f51bcd9477017&health=vegetarian&random=true&q=" + keyword;

                HttpResponseMessage response = await client.GetAsync(edamamUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);
                var hits = json["hits"].ToObject<List<JObject>>();
                var list = hits.Select(hit => hit["recipe"].ToObject<RecipeJsonDTO>()).ToList();
                var tmpList = await _recipeRepository.GetAllRecipes();
                foreach (var item in list)
                {
                    string apiUrl = "https://api.edamam.com/api/nutrition-details?app_id=174f7bdb&app_key=56ab01541420c6456c7a24d321f9d712";

                    var json1 = JsonConvert.SerializeObject(new { ingr = item.IngredientLines });
                    var content = new StringContent(json1, Encoding.UTF8, "application/json");
                    HttpResponseMessage response1 = await client.PostAsync(apiUrl, content);
                    response1.EnsureSuccessStatusCode();
                    var jsonObject = JObject.Parse(await response1.Content.ReadAsStringAsync());
                    List<IngredientsDTO> list1 = jsonObject["ingredients"].ToObject<List<IngredientsDTO>>();
                    var listIngr = await _ingredientRepository.GetIngredientList();
                    foreach (var item1 in list1)
                    {
                        if (item1.Parsed != null)
                            foreach (var ingr in item1.Parsed)
                            {
                                if (listIngr.FirstOrDefault(x => x.FoodId == ingr.FoodId) == null)
                                    await _ingredientRepository.AddIngredient(new()
                                    {
                                        FoodId = ingr.FoodId,
                                        FoodMatch = ingr.FoodMatch,
                                        Url = ingr.Image,
                                        Nutrient = JsonConvert.SerializeObject(ingr.Nutrients),
                                        IsDeleted = 1,
                                        Measure = ingr.Measure,
                                        Weight = ingr.Weight,
                                        Food = ingr.Food,
                                        RetainedWeight = ingr.RetainedWeight,
                                        Quantity = ingr.Quantity
                                    });
                            }
                    }
                    if (tmpList.FirstOrDefault(x => x.Url == item.Url) == null)
                        await _recipeRepository.AddRecipe(new()
                        {
                            Name = item.Name,
                            Description = "",
                            Instruction = "",
                            Preparation = "",
                            Accessibility = "Public",
                            Calories = item.Calories,
                            CreateDateTime = DateTime.Now,
                            IsDeleted = 1,
                            TotalNutrients = JsonConvert.SerializeObject(item.TotalNutrients),
                            TotalDaily = JsonConvert.SerializeObject(item.TotalDaily),
                            TotalWeight = item.TotalWeight,
                            Url = item.Url,
                            Image = item.Image,
                            LikeQuantity = 0,
                            Ingredients = JsonConvert.SerializeObject(new { en = item.IngredientLines, vn = new List<string>() })
                        });
                }
                var recipe = _recipeRepository.GetRecipeByName(keyword);

                if (recipe != null && recipe.Any())
                {
                    result.IsSuccess = true;
                    result.StatusCode = StatusCodes.Status200OK;
                    result.Message = "Recipe found";
                    var pagedResult = await recipe.AsQueryable().ToPagedResultAsync(pagingParams);
                    result.Data = new { pagedResult.CurrentPage, pagedResult.TotalPages, pagedResult.PageSize, pagedResult.TotalCount, Items = _mapper.Map<List<RecipeResponseDTO>>(pagedResult.Items) };
                }
                else
                {
                    result.IsSuccess = false;
                    result.StatusCode = StatusCodes.Status400BadRequest;
                    result.Message = "No recipe found matched";
                    result.Data = null;
                }
            }
            catch (Exception e)
            {
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.IsSuccess = false;
                result.Message = e.Message;
                result.Data = null;
            }
            return result;
        }

        public async Task<ResultModel> UpdateRecipe(int id, RecipeUpdateDTO recipeDTO)
        {
            try
            {
                var recipe = await _recipeRepository.GetRecipeById(id);
                if (recipe == null)
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "No recipe found matched",
                        Data = null
                    };
                }
                else
                {
                    //string edamamUrl = "https://api.edamam.com/api/nutrition-details?app_id=f5166b49&app_key=04db7f991e2cf45fd23e3d40ff522ad8";

                    //var json = JsonConvert.SerializeObject(recipeDTO.Ingredients);
                    //var content = new StringContent(json, Encoding.UTF8, "application/json");
                    //HttpResponseMessage response = await client.PostAsync(edamamUrl, content);
                    //response.EnsureSuccessStatusCode();
                    //var jsonObject = JObject.Parse(await response.Content.ReadAsStringAsync());
                    recipe.Name = recipeDTO.Name;
                    recipe.TranslatedName = recipeDTO.TranslatedName;
                    recipe.Description = recipeDTO.Description;
                    recipe.Instruction = recipeDTO.Instruction;
                    recipe.Preparation = recipeDTO.Preparation;
                    recipe.Ingredients = _mapper.Map<Recipe>(recipeDTO).Ingredients;
                    recipe.Url = recipeDTO.Url;
                    recipe.Image = recipeDTO.Image;

                    await _recipeRepository.UpdateRecipe(id, recipe);
                    return new ResultModel()
                    {
                        IsSuccess = true,
                        StatusCode = StatusCodes.Status200OK,
                        Message = "No recipe deleted",
                        Data = _mapper.Map<RecipeResponseDTO>(recipe)
                    };
                }
            }
            catch (Exception e)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = e.Message,
                    Data = null
                };
            }
        }
        public async Task<ResultModel> GetRecipesByName(string keyword, PaginationParams pagingParams)
        {
            try
            {
                var pagedResult = await _recipeRepository.GetRecipeByName(keyword).AsQueryable().ToPagedResultAsync(pagingParams);
                if (pagedResult == null || pagedResult.Items == null || pagedResult.Items.Count == 0)
                    return new ResultModel()
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = "No recipe found!",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                return new ResultModel()
                {
                    Data = new { pagedResult.CurrentPage, pagedResult.TotalPages, pagedResult.PageSize, pagedResult.TotalCount, Items = _mapper.Map<List<RecipeResponseDTO>>(pagedResult.Items) },
                    IsSuccess = true,
                    Message = "Get Successfully",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel()
                {
                    Data = null,
                    IsSuccess = false,
                    Message = e.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
        }

        public async Task<ResultModel> GetAllRecipes(PaginationParams paginationParams, string? orderBy)
        {
            var recipes = _recipeRepository.GetRecipeList().Where(x => x.IsDeleted == 1);
            var query = recipes.AsQueryable();
            try
            {
                if (!string.IsNullOrEmpty(orderBy))
                {
                    query = orderBy switch
                    {
                        "CreateDateTime" => query.OrderByDescending(x => x.CreateDateTime),
                        "LikeQuantity" => query.OrderByDescending(x => x.LikeQuantity),
                        _ => query
                    };
                }

                var totalItems = query.Count();
                var items = query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                                 .Take(paginationParams.PageSize)
                                 .ToList();

                var pagedResult = await query.ToPagedResultAsync(paginationParams);

                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get recipes successfully",
                    Data = new
                    {
                        pagedResult.CurrentPage,
                        pagedResult.TotalPages,
                        pagedResult.PageSize,
                        pagedResult.TotalCount,
                        Items = _mapper.Map<List<RecipeResponseDTO>>(pagedResult.Items)
                    }
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = e.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> LikeRecipe(string userId, int recipeId)
        {

            try
            {
                var existingRecipe = await _recipeRepository.GetRecipeById(recipeId);
                if (existingRecipe == null)
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "No recipe found matched",
                        Data = null
                    };
                }
                var existRecipeInList = _userRepository.ExistRecipeInRecipeList(userId, recipeId);
                if (existRecipeInList)
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Recipe already liked",
                        Data = null
                    };
                }
                else
                {
                    var recipe = await _recipeRepository.LikeRecipe(recipeId);
                    await _userRepository.AddRecipeToUser(userId, recipeId);
                    return new ResultModel()
                    {
                        IsSuccess = true,
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Recipe liked",
                        Data = recipe
                    };
                }
            }
            catch (Exception e)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = e.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> UnlikeRecipe(string userId, int recipeId)
        {
            try
            {
                var existingRecipe = await _recipeRepository.GetRecipeById(recipeId);
                if (existingRecipe == null)
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "No recipe found matched",
                        Data = null
                    };
                }
                var existRecipeInList = _userRepository.ExistRecipeInRecipeList(userId, recipeId);
                if (!existRecipeInList)
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Recipe unliked already",
                        Data = null
                    };
                }
                else
                {
                    var recipe = await _recipeRepository.DislikeRecipe(recipeId);
                    await _userRepository.RemoveRecipeFromUser(userId, recipeId);
                    return new ResultModel()
                    {
                        IsSuccess = true,
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Recipe disliked",
                        Data = _mapper.Map<RecipeResponseDTO>(recipe)
                    };
                }
            }
            catch (Exception e)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = e.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetRecipeCountEveryMonth()
        {
            try
            {
                var recipeCounts = await _recipeRepository.GetRecipeCountByMonth();
                return new ResultModel()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get recipe count by month successfully",
                    Data = recipeCounts
                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetMostLikedRecipe()
        {
            try
            {
                var recipe = await _recipeRepository.GetMostLikeRecipes();
                return new ResultModel()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get most liked recipe successfully",
                    Data = recipe
                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetRecipeTotalCount()
        {
            try
            {
                var recipeCount = await _recipeRepository.GetRecipeTotalCount();
                return new ResultModel()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get recipe total count successfully",
                    Data = recipeCount
                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetRecipeById(int id)
        {
            try
            {
                var recipe = await _recipeRepository.GetRecipeById(id);
                if (recipe == null)
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "No recipe found matched",
                        Data = null
                    };
                }
                return new ResultModel()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get recipe successfully",
                    Data = _mapper.Map<RecipeResponseDTO>(recipe)
                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}

