﻿using backend.Domain.Cores;
namespace backend.Models
{
    public class FeedbackModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
    }
}
