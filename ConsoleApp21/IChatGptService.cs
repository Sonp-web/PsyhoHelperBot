namespace ConsoleApp21;

public interface IChatGptService
{
    public Task<string?> GetAnswerFromGpt(string question);
}