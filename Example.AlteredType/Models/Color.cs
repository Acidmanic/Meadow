using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.AlteredType.Models
{
    [AlteredType(typeof(long))]
    public class Color
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public static readonly Color Black = new Color
        {
            Description = "The Darkest Color",
            Id = 0,
            Name = "Black"
        };
        public static readonly Color Red = new Color
        {
            Description = "Like Tomatoes",
            Id = 1,
            Name = "Red"
        };
        public static readonly Color Green = new Color
        {
            Description = "Like Grass",
            Id = 2,
            Name = "Green"
        };
        public static readonly Color Blue = new Color
        {
            Description = "Avoid The Glue!",
            Id = 3,
            Name = "Blue"
        };
        public static readonly Color White = new Color
        {
            Description = "The Brightest",
            Id = 4,
            Name = "White"
        };

        public static implicit operator long(Color color)
        {
            return color.Id;
        }

        public static implicit operator Color(long color)
        {
            switch (color)
            {
                case 1: return Red;
                case 2: return Green;
                case 3: return Blue;
                case 4: return White;
            }

            return Black;
        }
        
    }
}