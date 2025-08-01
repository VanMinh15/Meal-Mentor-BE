using Newtonsoft.Json;

namespace MealMentor.Core.DTOs
{
    public class TotalNutrients
    {

        public Nutrients ENERC_KCAL { get; set; }
        public Nutrients FAT { get; set; }
        public Nutrients FASAT { get; set; }
        public Nutrients FATRN { get; set; }
        public Nutrients FAMS { get; set; }
        public Nutrients FAPU { get; set; }
        [JsonProperty("CHOCDF.net")]
        public Nutrients CHOCDF { get; set; }
        [JsonProperty("CHOCDF_net")]
        public Nutrients CHOCDF_net { get; set; }
        public Nutrients FIBTG { get; set; }
        [JsonProperty("SUGAR")]
        public Nutrients SUGAR { get; set; }
        [JsonProperty("SUGAR.added")]
        public Nutrients SUGAR_added { get; set; }
        public Nutrients PROCNT { get; set; }
        public Nutrients CHOLE { get; set; }
        public Nutrients NA { get; set; }
        public Nutrients CA { get; set; }
        public Nutrients MG { get; set; }
        public Nutrients K { get; set; }
        public Nutrients FE { get; set; }
        public Nutrients ZN { get; set; }
        public Nutrients P { get; set; }
        public Nutrients VITA_RAE { get; set; }
        public Nutrients VITC { get; set; }
        public Nutrients THIA { get; set; }
        public Nutrients RIBF { get; set; }
        public Nutrients NIA { get; set; }
        public Nutrients VITB6A { get; set; }
        public Nutrients FOLDFE { get; set; }
        public Nutrients FOLFD { get; set; }
        public Nutrients FOLAC { get; set; }
        public Nutrients VITB12 { get; set; }
        public Nutrients VITD { get; set; }
        public Nutrients TOCPHA { get; set; }
        public Nutrients VITK1 { get; set; }
        public Nutrients WATER { get; set; }
    }
}


