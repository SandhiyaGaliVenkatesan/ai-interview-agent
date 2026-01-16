import { useEffect, useRef, useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import { generateQuestion, submitAnswer } from "../api/sessionsApi";

export default function Interview() {
  const { id } = useParams();
  const sessionId = useMemo(() => id as string, [id]);

  const [turnNumber, setTurnNumber] = useState<number | null>(null);
  const [questionJson, setQuestionJson] = useState<any>(null);
  const [answer, setAnswer] = useState("");
  const [evaluationJson, setEvaluationJson] = useState<any>(null);
  const [loading, setLoading] = useState(false);

const didRun = useRef(false);
  async function nextQuestion() {
    setLoading(true);
    setEvaluationJson(null);
    setAnswer("");
    const res = await generateQuestion(sessionId);
    setTurnNumber(res.turnNumber);
    setQuestionJson(JSON.parse(res.questionJson));
    setLoading(false);
  }

  async function evaluate() {
    if (!turnNumber) return;
    setLoading(true);
    const res = await submitAnswer(sessionId, turnNumber, answer);
    setEvaluationJson(JSON.parse(res.evaluationJson));
    setLoading(false);
  }

  useEffect(() => {
     if (didRun.current) return;
     didRun.current = true;
    nextQuestion();
    // eslint-disable-next-line
  }, []);

  return (
    <div className="grid">
      <div className="card">
        <h2>Question</h2>
        {questionJson ? (
          <>
            <div className="pillRow">
              <span className="pill">{questionJson.difficulty}</span>
              {(questionJson.tags ?? []).map((t: string) => (
                <span key={t} className="pill">{t}</span>
              ))}
            </div>
            <p className="question">{questionJson.question}</p>
          </>
        ) : (
          <p>Loading question…</p>
        )}

        <label>Your answer</label>
        <textarea value={answer} onChange={(e) => setAnswer(e.target.value)} rows={8} />

        <div className="row">
          <button onClick={evaluate} disabled={loading || !answer.trim()}>
            {loading ? "Evaluating…" : "Submit Answer"}
          </button>
          <button onClick={nextQuestion} disabled={loading}>
            Next Question
          </button>
        </div>
      </div>

      <div className="card">
  <h2>Evaluation</h2>

  {evaluationJson ? (
    <>
      <div className="scores">
        <div className="score">
          <div className="scoreKey">Overall</div>
          <div className="scoreVal">{evaluationJson.score}/10</div>
        </div>
      </div>

      <h3>Verdict</h3>
      <p>{evaluationJson.verdict}</p>

      <h3>Feedback</h3>
      <ul>
        {[...(evaluationJson.strengths ?? []), ...(evaluationJson.improvements ?? [])]
          .filter(Boolean)
          .map((f: string, i: number) => (
            <li key={i}>{f}</li>
          ))}
      </ul>

      <h3>Better answer</h3>
      <pre style={{ whiteSpace: "pre-wrap" }}>
        {evaluationJson.modelAnswer}
      </pre>

      <h3>Follow-up</h3>
      <p>
        {evaluationJson.improvements?.[0]
          ? `Can you improve your answer by addressing: ${evaluationJson.improvements[0]}?`
          : "Add a concrete example and explain tradeoffs."}
      </p>
    </>
  ) : (
    <p>Submit an answer to see scoring.</p>
  )}
</div>
    </div>
  );
}
