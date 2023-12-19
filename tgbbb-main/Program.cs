using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("6929536785:AAF0kaRmvJzoezZ668qBYLrG1oN87oZ790s");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    if (messageText.ToLower() == "помощь")
    {
        await botClient.SendTextMessageAsync(chatId: chatId,
            text: "Доступные Команды: привет, картинка, видео, стикер, кнопки", cancellationToken: cancellationToken);
    }

    if (messageText.ToLower() == "привет")
    {
        await botClient.SendTextMessageAsync(chatId: chatId,
            text: "Привет,я бот помощник по игре Escape From Tarkov", cancellationToken: cancellationToken);
    }
    if (messageText.ToLower() == "картинка")
    { 
        await botClient.SendPhotoAsync(
        chatId: chatId,
        photo: InputFile.FromUri("https://upload.wikimedia.org/wikipedia/ru/archive/7/7a/20170106090512%21Escape_from_Tarkov_logo.png"),
        caption: "<b>Это логотип игры</b>.",
        parseMode: ParseMode.Html,
        cancellationToken: cancellationToken);
    }

    if (messageText.ToLower() == "видео")
    {
        await botClient.SendVideoAsync(
        chatId: chatId,
        video: InputFile.FromUri("https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-countdown.mp4"),
        caption: "<b>Могло бы быть видео геймплея, но там очень страшные звуки xD</b>.",
        parseMode: ParseMode.Html,
        cancellationToken: cancellationToken);
    }

    if (messageText.ToLower() == "стикер")
    {
        await botClient.SendStickerAsync(
        chatId: chatId,
        sticker: InputFile.FromUri("https://github.com/TelegramBots/book/raw/master/src/docs/sticker-fred.webp"),
        cancellationToken: cancellationToken);
    }

    if (messageText.ToLower() == "кнопки") 
    {
        await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "Полезные сайты",
        parseMode: ParseMode.MarkdownV2,
    disableNotification: true,
    replyToMessageId: update.Message.MessageId,
    replyMarkup: new InlineKeyboardMarkup(new[]
    {
        InlineKeyboardButton.WithUrl(
                    text: "тарков хелп",
                    url: "https://tarkov.help/"),
                InlineKeyboardButton.WithUrl(
                    text: "тарков вики",
                    url: "https://escapefromtarkov.fandom.com/ru/wiki/Escape_from_Tarkov_Wiki")
    }
        ),
    cancellationToken: cancellationToken);
    }



}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
