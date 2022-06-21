using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Examples.Polling;

public class Handlers 
{
    static List<long> chats = new List<long>();

    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {

        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var handler = update.Type switch
        {
            UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
            UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
            _ => UnknownUpdateHandlerAsync(botClient, update)
        };
        
        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(botClient, exception, cancellationToken);
        }
    }

    private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
    {
        Console.WriteLine($"Message type: {message.Type}\nMessage text: {message.Text}\nChat id: {message.Chat.Id}");
        if (message.Type != MessageType.Text)
            return;
        if (message.Text == "/start") await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                     text: "Hello",
                                                                     parseMode: null
                                                                    );
        else if (message.Text.StartsWith("/add"))
        {
            long temp;
            long.TryParse(message.Text.Remove(0, 4), out temp);
            chats.Add(temp);                                        //-1001780914140     -568811382     -726946510
        }

        else if(message.Chat.Id == 1030464002)
        {
            for (int i = 0; i < chats.Count(); i++)
            {
                await botClient.SendTextMessageAsync(chatId: chats[i], 
                                                 text: message.Text,
                                                 parseMode: null);
            }
        }
    }

    private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}
