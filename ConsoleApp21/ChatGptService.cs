using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace ConsoleApp21;

public class ChatGptService:IChatGptService
{
    private readonly OpenAIService _openAiService;
    private readonly string _prompt;
    
    public async Task<string?> GetAnswerFromGpt(string question)
    {
        try
        {
            var completionResult = await _openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest
                {
                    Model = Models.Gpt_3_5_Turbo_16k,
                    Messages = new List<ChatMessage>()
                    {
                        new(StaticValues.ChatMessageRoles.System, _prompt),
                        new(StaticValues.ChatMessageRoles.User, question)
                    },
                    Temperature = 0.1f
                });
            if (completionResult.Successful)
            {
                var request = completionResult.Choices.First().Message.Content;
                return request;
            }
            var errorMessage = $"OpenAi API Error:\n{completionResult.Error.Code}\n{completionResult.Error.Message}";
            Console.WriteLine(errorMessage);
            return "Ответ";
        }
        catch (Exception e)
        {
            await ErrorNotificationService.Instance.SendErrorNotification(e);
            return null;
        }
    }

    public ChatGptService(string openAiApiKey, AppSettings appSettings)
    {
        _prompt = appSettings.GptPrompt;
        _openAiService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = openAiApiKey
        });
    }
}