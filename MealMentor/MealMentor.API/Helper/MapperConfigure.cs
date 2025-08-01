using AutoMapper;
using MealMentor.Core.Domain.Entities;
using MealMentor.Core.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MealMentor.API.Helper
{
    public class MapperConfigure : Profile
    {
        public MapperConfigure()
        {
            CreateMap<Recipe, RecipeResponseDTO>()
                .ForMember(dest => dest.TotalNutrients, otp => otp.MapFrom(src => JObject.Parse(src.TotalNutrients).ToObject<TotalNutrients>()))
                .ForMember(dest => dest.TotalDaily, otp => otp.MapFrom(src => JObject.Parse(src.TotalDaily).ToObject<TotalNutrients>()))
                .ForMember(dest => dest.Ingredients, otp => otp.MapFrom(src => !string.IsNullOrWhiteSpace(src.Ingredients)
                ? JObject.Parse(src.Ingredients).ToObject<IngredientGetDTO>().en
                : new IngredientGetDTO().en))
                .ForMember(dest => dest.TranslatedIngredients, otp => otp.MapFrom(src => !string.IsNullOrWhiteSpace(src.Ingredients)
                ? JObject.Parse(src.Ingredients).ToObject<IngredientGetDTO>().vn
                : new IngredientGetDTO().vn))
                .ForMember(dest => dest.IsDeleted, otp => otp.MapFrom(src => src.IsDeleted == 0 ? "True" : "False"))
                .ReverseMap()
                .ForMember(dest => dest.TotalNutrients, otp => otp.MapFrom(src => JsonConvert.SerializeObject(src.TotalNutrients)))
                .ForMember(dest => dest.TotalDaily, otp => otp.MapFrom(src => JsonConvert.SerializeObject(src.TotalDaily)))
                .ForMember(dest => dest.Ingredients, otp => otp.MapFrom(src => JsonConvert.SerializeObject(new IngredientGetDTO { en = src.Ingredients, vn = src.TranslatedIngredients })))
                .ForMember(dest => dest.IsDeleted, otp => otp.MapFrom(src => src.IsDeleted == "True" ? 0 : 1));

            CreateMap<Ingredient, IngredientResponseDTO>()
                .ForMember(dest => dest.Nutrient, otp => otp.MapFrom(src => JObject.Parse(src.Nutrient).ToObject<TotalNutrients>()))
                .ReverseMap()
                .ForMember(dest => dest.Nutrient, otp => otp.MapFrom(src => JsonConvert.SerializeObject(src.Nutrient)));

            CreateMap<Recipe, RecipeUpdateDTO>()
                .ForMember(dest => dest.Ingredients, otp => otp.MapFrom(src => !string.IsNullOrWhiteSpace(src.Ingredients)
                ? JObject.Parse(src.Ingredients).ToObject<IngredientGetDTO>().en
                : new IngredientGetDTO().en))
                .ForMember(dest => dest.TranslatedIngredients, otp => otp.MapFrom(src => !string.IsNullOrWhiteSpace(src.Ingredients)
                ? JObject.Parse(src.Ingredients).ToObject<IngredientGetDTO>().vn
                : new IngredientGetDTO().vn))
                .ReverseMap()
                .ForMember(dest => dest.Ingredients, otp => otp.MapFrom(src => JsonConvert.SerializeObject(new IngredientGetDTO { en = src.Ingredients, vn = src.TranslatedIngredients })));
        }
    }
}
