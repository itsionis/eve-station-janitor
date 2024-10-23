using Spectre.Console;
using EveStationJanitor.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using EveStationJanitor.Authentication;
using EveStationJanitor.Authentication.Persistence;

namespace EveStationJanitor;

internal class EveCharacterSelectionLogic(AppDbContext context, IAuthenticationClient authenticationClient, IAuthenticationDataProvider authenticationDataProvider)
{
    private async Task<int?> AddNewCharacter()
    {
        // Start the authentication flow
        var authenticationResult = await authenticationClient.Authenticate();
        if (authenticationResult is null)
        {
            AnsiConsole.MarkupLine("[red]Authentication failed[/]");
            return null;
        }

        var (token, characterInfo) = authenticationResult.Value;
        await authenticationDataProvider.WriteCharacterAuthData(characterInfo.CharacterId, token, characterInfo);
        return characterInfo.CharacterId;
    }

    private async Task<int?> RemoveCharacter()
    {
        var characterChoices = await context.Characters.Select(c => new ValueChoice(c.Name)).ToListAsync();

        var prompt = new SelectionPrompt<Choice>()
            .Title("Remove a character:")
            .AddChoiceGroup(new ValueChoice("Characters"), characterChoices)
            .AddChoiceGroup(new ValueChoice("Actions"), new List<Choice>
        {
            new ActionChoice<int?>(PromptForCharacterId, "Go back"),
        });

        var choice = AnsiConsole.Prompt(prompt);

        if (choice is ValueChoice valueChoice)
        {
            var acharacter = context.Characters.FirstOrDefault(f => f.Name == valueChoice.Value);
            if (acharacter == null) return await PromptForCharacterId();
            await authenticationDataProvider.RemoveCharacter(acharacter.EveCharacterId);
            return await PromptForCharacterId();
        }
        else if (choice is ActionChoice<int?> actionChoice)
        {
            return await actionChoice.Invoke();
        }
        else
        {
            return null;
        }
    }

    public async Task<int?> PromptForCharacterId()
    {
        var characterChoices = await context.Characters.Select(c => new ValueChoice(c.Name)).Order().ToListAsync();

        var actionChoices = new List<Choice>
        {
            new ActionChoice<int?>(AddNewCharacter, "Add New Character"),
        };

        var prompt = new SelectionPrompt<Choice>()
            .Title("Select a character:");

        if (characterChoices.Count > 0)
        {
            prompt.AddChoiceGroup(new ValueChoice("Characters"), characterChoices);
            actionChoices.Add(new ActionChoice<int?>(RemoveCharacter, "Remove Character"));
        }

        prompt.AddChoiceGroup(new ValueChoice("Actions"), actionChoices);

        var choice = AnsiConsole.Prompt(prompt);

        if (choice is ActionChoice<int?> actionChoice)
        {
            return await actionChoice.Invoke();
        }
        else if (choice is ValueChoice valueChoice)
        {
            var characterName = valueChoice.Value;
            if (characterName is null) return null;

            var character = await context.Characters.FirstOrDefaultAsync(c => c.Name == characterName);
            if (character is null) return null;
            return character.EveCharacterId;
        }
        else
        {
            return null;
        }
    }

    private class ActionChoice<T>(Func<Task<T>> action, string label) : Choice
    {
        public async Task<T> Invoke()
        {
            return await action.Invoke();
        }

        public override string ToString()
        {
            return label;
        }
    }

    private class ValueChoice(string value) : Choice
    {
        public override string ToString()
        {
            return value;
        }

        public string Value => value;

        public static explicit operator ValueChoice(string val) => new ValueChoice(val);
    }

    private abstract class Choice
    {
        public override abstract string ToString();
    }
}
