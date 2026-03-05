using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WgCleaningApp.Domain.Entities
{

    public class TaskItem
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool IsCompleted { get; private set; }

        public Guid? AssignedToUserId { get; private set; }
        public User? AssignedToUser { get; private set; }

        public Guid WgId { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly? EndDate { get; private set; }

        private TaskItem() { }

        public TaskItem(
            string title,
            string description,
            Guid wgId,
            DateOnly startDate,
            DateOnly? endDate = null,
            Guid? assignedToUserId = null
        )
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            WgId = wgId;
            StartDate = startDate;
            EndDate = endDate;
            AssignedToUserId = assignedToUserId;
            IsCompleted = false;
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }

        public void AssignTo(Guid userId)
        {
            AssignedToUserId = userId;
        }
    }

}
