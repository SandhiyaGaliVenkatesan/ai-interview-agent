namespace InterviewAgent.Api.Services.Ai;

public class PromptLoader
{
    public string Load(string promptFileName)
    {
        var basePath = AppContext.BaseDirectory;
        var path = Path.Combine(basePath, "Prompts", promptFileName);

        // In development, prompts may be in project folder. Fallback:
        if (!File.Exists(path))
        {
            var devPath = Path.Combine(Directory.GetCurrentDirectory(), "Prompts", promptFileName);
            if (File.Exists(devPath)) return File.ReadAllText(devPath);
        }

        return File.ReadAllText(path);
    }
}
