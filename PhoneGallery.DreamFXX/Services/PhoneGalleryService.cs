using PhoneGallery.DreamFXX.Data;
using PhoneGallery.DreamFXX.Models;
using PhoneGallery.DreamFXX.UserInput;
using Spectre.Console;

namespace PhoneGallery.DreamFXX.Services;

public class PhoneGalleryService
{
    private readonly PhoneGalleryContext _context;

    public PhoneGalleryService(PhoneGalleryContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        try
        {
            _context.Database.EnsureCreated();
        }
        catch (Exception dbEx)
        {
            AnsiConsole.MarkupLine($"[red]Database initialization error: {dbEx.Message}[/]");
            throw;
        }
    }

    public void Start()
    {
        while (true)
        {
            Console.Clear();

            var menuSelections = new List<MenuSelection>
            {
                new("Add Contact", AddContact),
                new("Show Contact List", ShowContacts),
                new("Update Contact", UpdateContact),
                new("Delete Contact", DeleteContact),
                new("Show Categories", ShowCategories),
                new("Exit", Exit)
            };

            var userSelection = AnsiConsole.Prompt(
                new SelectionPrompt<MenuSelection>()
                .Title(
                "[yellow]Welcome in your personal phonebook[/]\n[grey]-Select an option-[/]")
                .PageSize(6)
                .AddChoices(menuSelections)
                .UseConverter(n => n.Name));

            userSelection.Action.Invoke();

            if (userSelection.Name == "Exit")
                break;

        }
    }

    public void AddContact()
    {
        string? name = ContactInput.GetNameInput();
        if (name == null)
            return;

        string? email = ContactInput.GetEmailInput();
        if (email == null)
            return;

        string? phoneNumber = ContactInput.GetPhoneNumberInput();
        if (phoneNumber == null)
            return;

        int categoryId = ContactInput.GetCategorySelection(GetCategories());

        var contact = new Contact
        {
            Name = name,
            Email = email,
            PhoneNumber = phoneNumber,
            CategoryId = categoryId
        };

        try
        {
            _context.Contacts.Add(contact);
            _context.SaveChanges();
            AnsiConsole.MarkupLine("[green]Contact was saved successfully.[/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to save the contact: {ex.Message}[/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            throw;
        }
    }

    public List<Contact>? GetContacts()
    {
        try
        {
            var contacts = _context.Contacts.ToList();
            if (!contacts.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No saved contacts found.[/]");
                AnsiConsole.WriteLine("Press any key to go back.");
                Console.ReadKey();
                return null;
            }
            return contacts;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Database error: {ex.Message}[/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return new List<Contact>();
        }

    }

    private List<Category> GetCategories()
    {
        return _context.Categories.ToList();
    }

    public void ShowContacts()
    {
        var contacts = _context.Contacts.ToList();
        if (!contacts.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No contacts found in the database.[/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        foreach (var contact in contacts)
        {
            AnsiConsole.MarkupLine($"[green]Name[/] - {contact.Name}");
            AnsiConsole.MarkupLine($"[green]Phone Number[/] - {contact.PhoneNumber}");
            AnsiConsole.MarkupLine($"[green]Email[/] - {contact.Email}");
            AnsiConsole.MarkupLine($"[green]Category[/] - {contact.Category?.Name ?? "Uncategorized"}");
        }

        AnsiConsole.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public void ShowCategories()
    {
        var categories = GetCategories();
        if (categories == null) return;

        foreach (var category in categories)
        {
            Console.WriteLine($"{category.Id}: {category.Name}");
            Console.WriteLine();
        }

    }

    public void UpdateContact()
    {
        Contact contact = ContactInput.GetSpecificContact(GetContacts());
        if (contact == null)
            return;

        contact.PhoneNumber = ContactInput.GetPhoneNumberInput();
        if (contact.PhoneNumber == null)
            return;

        contact.Email = ContactInput.GetEmailInput();
        if (contact.Email == null)
            return;

        contact.CategoryId = ContactInput.GetCategorySelection(GetCategories());

        if (!ContactInput.ConfirmAction())
        {
            Console.WriteLine("Changes were cancelled.");
            return;
        }

        _context.SaveChanges();
        Console.WriteLine("Contact was updated.");
    }

    public void DeleteContact()
    {
        Contact contact = ContactInput.GetSpecificContact(GetContacts());
        if (contact == null)
            return;

        if (!ContactInput.ConfirmAction())
        {
            Console.WriteLine("Contact was not deleted.");
            return;
        }
        _context.Remove(contact);
        _context.SaveChanges();
        Console.WriteLine("Contact was deleted.");
    }


    public void Exit()
    {
        AnsiConsole.MarkupLine("[red]Goodbye.\n\n- press any key to exit[/]");
        Console.ReadKey();
        Environment.Exit(0);
    }

}



