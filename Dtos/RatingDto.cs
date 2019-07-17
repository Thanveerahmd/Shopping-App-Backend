namespace pro.backend.Dtos
{
    public class RatingDto
    {
        public int Id {get; set;}
        public string UserId {get;set;}
        public string UserFullName {get;set;}
        public int ProductId {get;set;}
        public int RatingValue {get;set;}
        public string Comment {get;set;}
    }
}