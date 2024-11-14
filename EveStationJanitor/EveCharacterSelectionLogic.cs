using EveStationJanitor.Authentication;
using Spectre.Console;
using EveStationJanitor.Core.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EveStationJanitor;

internal class EveCharacterSelectionLogic(AppDbContext context, IAuthenticationDataProvider authenticationDataProvider)
{
    public async Task<int?> PromptForCharacterId()
    {
        var characterChoices = await LoadCharacterNameChoices(context);

        var actionChoices = new List<Choice>
        {
            new ActionChoice<int?>(AddNewCharacter, "Add Character"),
            new ActionChoice<int?>(()=>Task.FromResult<int?>(null), "Exit"),
        };

        var prompt = new SelectionPrompt<Choice>()
            .Title("Select a character:");

        if (characterChoices.Count > 0)
        {
            prompt.AddChoiceGroup(new ValueChoice("Characters"), characterChoices);
            actionChoices.Insert(1, new ActionChoice<int?>(RemoveCharacter, "Remove Character"));
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
            var character = await context.Characters.FirstOrDefaultAsync(c => c.Name == characterName);
            return character?.EveCharacterId;
        }
        else
        {
            return null;
        }
    }

    private async Task<int?> AddNewCharacter()
    {
        // Start the authentication flow
        var characterId = await authenticationDataProvider.AuthenticateNewCharacter();
        if (characterId is null)
        {
            AnsiConsole.MarkupLine("[red]Authentication failed[/]");
            return await PromptForCharacterId();
        }

        return characterId;
    }

    private async Task<int?> RemoveCharacter()
    {
        var characterChoices = await LoadCharacterNameChoices(context);

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
            var characterToRemove = context.Characters.FirstOrDefault(f => f.Name == valueChoice.Value);
            if (characterToRemove == null)
            {
                return await PromptForCharacterId();
            }

            await authenticationDataProvider.DeleteCharacter(characterToRemove.EveCharacterId);
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

    private static async Task<List<ValueChoice>> LoadCharacterNameChoices(AppDbContext context)
    {
        var characterNames = await context.Characters
            .OrderBy(c => c.Name)
            .Select(c => c.Name)
            .ToListAsync();

        return characterNames.Select(name => new ValueChoice(name)).ToList();
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

        public static explicit operator ValueChoice(string val) => new(val);
    }

    private abstract class Choice
    {
        public abstract override string ToString();
    }
}
