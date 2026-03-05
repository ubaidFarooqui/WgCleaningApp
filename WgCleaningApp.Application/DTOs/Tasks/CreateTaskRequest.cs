using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WgCleaningApp.Application.DTOs.Tasks
{
    public record CreateTaskRequest(
    string Title,
    string Description,
    Guid? AssignedToUserId,
    DateOnly StartDate,
    DateOnly? EndDate
);
}
