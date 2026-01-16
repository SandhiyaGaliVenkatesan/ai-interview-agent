namespace InterviewAgent.Api.Services.Ai;

public class PromptLoader
{
    public string Load(string promptFileName)
    {
        // 1) Try project folder first (most reliable in dev)
        var devPath = Path.Combine(Directory.GetCurrentDirectory(), "Prompts", promptFileName);
        if (File.Exists(devPath)) return File.ReadAllText(devPath);

        // 2) Try output folder (bin/Debug/.../Prompts)
        var basePath = AppContext.BaseDirectory;
        var outPath = Path.Combine(basePath, "Prompts", promptFileName);
        if (File.Exists(outPath)) return File.ReadAllText(outPath);

        throw new FileNotFoundException(
            $"Prompt file not found. Looked in:\n{devPath}\n{outPath}",
            outPath);
    }
}
