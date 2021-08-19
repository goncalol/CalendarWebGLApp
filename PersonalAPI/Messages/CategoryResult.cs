using PersonalAPI.Data;
using System.Linq;

namespace PersonalAPI.Messages
{
    public class CategoryResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }


        internal static CategoryResult Create(Category category)
            => new CategoryResult
            {
                Id = category?.Id ?? 0,
                Name = category?.Name,
                Color = category?.Color,
            };

        internal static CategoryResult[] Create(Category[] categories)
            => categories?.Select(Create).ToArray() ?? new CategoryResult[] { };
    }
}
