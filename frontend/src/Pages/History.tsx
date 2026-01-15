import { useEffect, useState } from "react";
import { listSessions } from "../api/sessionsApi";
import { Link } from "react-router-dom";

export default function History() {
  const [items, setItems] = useState<any[]>([]);

  useEffect(() => {
    listSessions().then(setItems);
  }, []);

  return (
    <div className="card">
      <h2>Past sessions</h2>
      <div className="list">
        {items.map((s) => (
          <Link key={s.id} className="listItem" to={`/sessions/${s.id}`}>
            <div>
              <strong>{s.track}</strong> • {s.difficulty}
            </div>
            <div className="muted">Turns: {s.turnsCount}</div>
          </Link>
        ))}
      </div>
    </div>
  );
}
