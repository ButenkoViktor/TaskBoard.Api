using Swashbuckle.AspNetCore.Filters;

namespace Taskboard.Api.Dtos
{
    public class CreateTaskDtoExample : IExamplesProvider<CreateTaskDto>
    {
        public CreateTaskDto GetExamples()
        {
            return new CreateTaskDto
            {
                Title = "Fix Swagger UI",
                Description = "Improve the documentation and theme of the API",
                Priority = "High",
                DueDate = DateTime.UtcNow.AddDays(3)
            };
        }
    }
}