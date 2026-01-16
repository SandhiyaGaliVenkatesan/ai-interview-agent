public sealed record EvaluationDto(
    int Score,
    string Verdict,
    List<string> Strengths,
    List<string> Improvements,
    string ModelAnswer
);