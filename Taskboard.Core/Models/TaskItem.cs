using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskboard.Core.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string? Description { get; set; }
        //Status can be "Open", "In Progress", "Completed"
        public string Status { get; set; } = "Open";
        //Priority can be "Low", "Medium", "High"
        public string Priority { get; set; } = "Medium"; 
        public DateTime? DueDate { get; set; }
        public int? AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }
    }
}
