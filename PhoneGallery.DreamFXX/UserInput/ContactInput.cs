﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
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
            var input = AnsiConsole.Ask<string>($"[yellow]{askUser.Trim()}[/]\n[grey]- Press e + ENTER to exit.[/]\n\n").Trim();
            if (input.Equals("e", StringComparison.OrdinalIgnoreCase))
                return null;

            if (validator(input))
                return input;

            AnsiConsole.MarkupLine("[red]Invalid input. Please try again.[/]");

        }
    }

    public static string? GetNameInput()
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        var name = GetValidatedInput("Enter name of the new contact:", InputValidation.IsNameValid);

        if (name == null)
        {
            AnsiConsole.MarkupLine("[yellow]Contact creation cancelled.[/]");
            return null;
        }
        return textInfo.ToTitleCase(name);
    }

    public static string? GetEmailInput()
    {
        return GetValidatedInput("Enter contacts email address\n[grey]- nickname@domain.com[/]", InputValidation.IsEmailValid);
    }

    public static string? GetPhoneNumberInput()
    {
        return GetValidatedInput("Phone number of the new contact:", InputValidation.IsPhoneNumberValid);
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
            .MoreChoicesText("[grey]Move with keyboard arrows to view more categories.[/]")
            .AddChoices(categories)
            .UseConverter(c => c?.Name ?? "Unnamed Category")
            );

        return categorySelected.Id;
    }

    public static Contact? GetSpecificContact(List<Contact> contacts)
    {
        if (contacts?.Any() != true)
        {
            AnsiConsole.MarkupLine("[yellow]No contacts available to select from.[/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        var contactSelected = AnsiConsole.Prompt(
            new SelectionPrompt<Contact>()
                .Title("[yellow]Select contact you want to change:[/]")
                .PageSize(10)
                .AddChoices(contacts)
                .UseConverter(c => c?.Name ?? "Unnamed Contact")
        );
        return contactSelected;
    }

    public static bool ConfirmAction()
    {
        var confirmation = AnsiConsole.Ask<bool>("[green]Are you sure you want to change / delete this contact?\n- options: true/false[/]\n");
        return confirmation;
    }
}

