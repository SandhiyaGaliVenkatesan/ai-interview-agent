import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getSession } from "../api/sessionsApi";

export default function SessionDetail() {
  const { id } = useParams();
  const [data, setData] = useState<any>(null);

  useEffect(() => {
    getSession(id as string).then(setData);
  }, [id]);

  if (!data) return <div className="card">Loading…</div>;

  return (
    <div className="card">
      <h2>Session</h2>
      <p className="muted">{data.track} • {data.difficulty}</p>

      {(data.turns ?? []).map((t: any) => {
        const q = safeJson(t.questionJson);
        const e = safeJson(t.evaluationJson);
        return (
          <div key={t.turnNumber} className="turn">
            <h3>Turn {t.turnNumber}</h3>
            <p><strong>Q:</strong> {q?.question ?? "—"}</p>
            <p><strong>A:</strong> {t.userAnswer || "—"}</p>
            <p><strong>Follow-up:</strong> {e?.followUpQuestion ?? "—"}</p>
          </div>
        );
      })}
    </div>
  );
}

function safeJson(txt: string) {
  try { return JSON.parse(txt); } catch { return null; }
}
