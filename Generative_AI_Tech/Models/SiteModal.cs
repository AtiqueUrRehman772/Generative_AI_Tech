﻿namespace Generative_AI_Tech.Models
{
    public class SiteModal
    {
        public string? Id { get; set; }
        public string? Site_Name { get; set; }
        public string? Summary { get; set; }
        public string? Image_Name { get; set; }
        public string? Anchor_Link { get; set; }
        public int? Likes { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
