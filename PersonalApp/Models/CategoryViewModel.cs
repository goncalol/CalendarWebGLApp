using PersonalApp.Data;
using System.Linq;

namespace PersonalApp.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }


        internal static CategoryViewModel Create(Category category)
            => new CategoryViewModel
            {
                Id = category?.Id ?? 0,
                Name = category?.Name,
                Color = category?.Color,
            };

        internal static CategoryViewModel[] Create(Category[] categories)
            => categories?.Select(Create).ToArray() ?? new CategoryViewModel[] { };
    }
}
