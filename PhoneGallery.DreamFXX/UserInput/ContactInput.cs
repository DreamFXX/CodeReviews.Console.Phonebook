using System.Globalization;
using PhoneGallery.DreamFXX.Models;
using PhoneGallery.DreamFXX.Validation;
using Spectre.Console;

namespace PhoneGallery.DreamFXX.UserInput;

public class ContactInput
{
    public static string? GetValidatedInput(string askUser, Func<string, bool> validator)
    {
        ArgumentNullException.ThrowIfNull(askUser);
        ArgumentNullException.ThrowIfNull(validator);

        while (true)
        {
            var input = AnsiConsole.Ask<string>($"[yellow]{askUser.Trim()} or enter e to exit[/]").Trim();
            if (input.Equals("e", StringComparison.OrdinalIgnoreCase))
                return null;

            if (validator(input))
                return input;
           
            AnsiConsole.MarkupLine("[red]Invalid input. Please try again.[/]");
            
        }
    }

    public static string GetNameInput()
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        var name = GetValidatedInput("Enter name of the contact: ", InputValidation.IsNameValid);
        return name != null ? textInfo.ToTitleCase(name) : throw new InvalidOperationException("Name cannot be null");
    }

    public static string GetEmailInput()
    {
        return GetValidatedInput("Enter contacts email address:", InputValidation.IsEmailValid);
    }

    public static string GetPhoneNumberInput()
    {
        return GetValidatedInput("Enter contacts phone number: ", InputValidation.IsPhoneNumberValid);
    }

    public static int GetCategorySelection(List<Category> categories)
    {
        ArgumentNullException.ThrowIfNull(categories);
        if (!categories.Any())
            throw new ArgumentException("Categories list cannot be empty.", nameof(categories));
        
        var categorySelected = AnsiConsole.Prompt(
            new SelectionPrompt<Category>()
            .Title("Add contact to category:")
            .PageSize(4)
            .MoreChoicesText("[grey](Move cursor up and down to reveal more categories)[/]")
            .AddChoices(categories)
            .UseConverter(c => c?.Name ?? "Unnamed Category")
            );

        return categorySelected.Id;
    }

    public static Contact? GetSpecificContact(List<Contact> contacts)
    {
        ArgumentNullException.ThrowIfNull(contacts);
        if (!contacts.Any())
            return null;

        var contactSelected = AnsiConsole.Prompt(
            new SelectionPrompt<Contact>()
                .Title("[yellow]Select contact you want to modify:[/]")
                .PageSize(10)
                .AddChoices(contacts)
                .UseConverter(c => c?.Name ?? "Unnamed Contact")
        );

        return contactSelected;
    }


    public static bool ConfirmAction()
    {
        var confirmation = AnsiConsole.Ask<bool>("[green]Are you sure you want to change / delete this contact?[/]");
        return confirmation;
    }
}

