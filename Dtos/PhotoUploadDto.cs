using System;
using Microsoft.AspNetCore.Http;

namespace pro.backend.Dtos
{
    public class PhotoUploadDto
    {

        public string Url { get; set; }
        public IFormFile file { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicID { get; set; }
        public  PhotoUploadDto()
        {
             DateAdded = DateTime.Now;
        }
    }
}