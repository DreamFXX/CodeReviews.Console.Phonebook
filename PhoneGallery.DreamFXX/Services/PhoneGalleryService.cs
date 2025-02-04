// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
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
        _context = context;

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
                new("Add Category", AddCategory),
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
        var categories = GetCategories();
        if (!categories.Any())
        {
            AnsiConsole.MarkupLine("[red]No categories available... Please add categories first![/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        string? name = ContactInput.GetNameInput();
        if (name == null) return;

        string? email = ContactInput.GetEmailInput();
        if (email == null) return;

        string? phoneNumber = ContactInput.GetPhoneNumberInput();
        if (phoneNumber == null) return;

        int categoryId = ContactInput.GetCategorySelection(categories);
        if (!categories.Any(c => c.Id == categoryId))
        {
            AnsiConsole.MarkupLine("[red]Invalid category selected.[/]");
            return;
        }

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
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to save the contact: {ex.Message}[/]");
        }
        AnsiConsole.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }


    public void AddCategory()
    {
        var name = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Enter name of the Category -> [/]")
                  .ValidationErrorMessage("[red]Category name is required[/]")
                  .Validate(name =>
                  {
                      if (string.IsNullOrWhiteSpace(name))
                          return ValidationResult.Error("[red]Category name cannot be empty[/]");
                      if (name.Length > 100)
                          return ValidationResult.Error("[red]Category name must be less than 100 characters[/]");
                      return ValidationResult.Success();
                  }));
        try
        {
            var category = new Category { Name = name };
            _context.Categories.Add(category);
            _context.SaveChanges();
            AnsiConsole.MarkupLine("[green]Category was saved successfully.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to save new category: {ex.Message}[/]");
        }

        AnsiConsole.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public List<Contact>? GetContacts()
    {
        try
        {
            var contacts = _context.Contacts
                .Include(c => c.Category)
                .ToList();

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
            throw;
        }

    }

    private List<Category> GetCategories()
    {
        return _context.Categories.ToList();
    }

    public void ShowContacts()
    {
        var contacts = _context.Contacts
            .Include(c => c.Category)
            .ToList();

        if (!contacts.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No saved contacts found.[/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var table = new Table()
            .AddColumn("[blue]Name[/]")
            .AddColumn("[blue]Phone Number[/]")
            .AddColumn("[blue]Email[/]")
            .AddColumn("[blue]Category[/]");

        foreach (var contact in contacts)
        {
            table.AddRow(
                contact.Name,
                contact.PhoneNumber,
                contact.Email,
                contact.Category?.Name ?? "N/A"
                );
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public void ShowCategories()
    {
        var categories = GetCategories();
        if (!categories.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No categories found.[/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var table = new Table()
            .AddColumn("[blue]ID[/]")
            .AddColumn("[blue]Name[/]");

        foreach (var category in categories)
        {
            table.AddRow(category.Id.ToString(), category.Name);
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public void UpdateContact()
    {
        try
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
                AnsiConsole.MarkupLine("[yellow]Changes were cancelled.[/]");
                return;
            }

            _context.SaveChanges();
            AnsiConsole.MarkupLine("[green]Contact was successfully updated.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to update contact: {ex.Message}[/]");
        }
        finally
        {
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    public void DeleteContact()
    {
        try
        {

            Contact contact = ContactInput.GetSpecificContact(GetContacts());
            if (contact == null)
                return;

            if (!ContactInput.ConfirmAction())
            {
                AnsiConsole.MarkupLine("[yellow]Contact deletion was cancelled.[/]");
                return;
            }
            _context.Remove(contact);
            _context.SaveChanges();
            AnsiConsole.MarkupLine("Contact was deleted.");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to delete contact: {ex.Message}[/]");
        }
        finally
        {
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    public void Exit()
    {
        AnsiConsole.MarkupLine("[red]Goodbye.\n\n- press any key to exit[/]");
        Console.ReadKey();
        Environment.Exit(0);
    }

}



