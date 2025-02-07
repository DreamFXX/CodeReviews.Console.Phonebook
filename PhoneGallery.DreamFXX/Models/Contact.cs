using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneGallery.DreamFXX.Models;

public class Contact
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Contact name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email format. Example: user@example.com")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\+?(\d{1,3})?[-. ]?\(?\d{3}\)?[-. ]?\d{3}[-. ]?\d{4}$",
        ErrorMessage = "Invalid phone number format. Accepted formats: +1-234-567-8900, (123) 456-7890, 1234567890")]
    public required string PhoneNumber { get; set; }

    [Required]
    [ForeignKey("CategoryId")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
