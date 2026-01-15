import { useState } from "react";
import { createSession } from "../api/sessionsApi";
import { useNavigate } from "react-router-dom";

export default function Home() {
  const [track, setTrack] = useState("dotnet");
  const [difficulty, setDifficulty] = useState("medium");
  const [loading, setLoading] = useState(false);
  const nav = useNavigate();

  async function start() {
    setLoading(true);
   try {
  const s = await createSession(track, difficulty);
  nav(`/interview/${s.id}`);
} catch (e: any) {
  console.error("API error:", e?.response?.status, e?.response?.data || e.message);
  alert("Backend call failed. Check console.");
}
    setLoading(false);
  }
  

  return (
    <div className="card">
      <h2>Start a session</h2>

      <label>Track</label>
      <select value={track} onChange={(e) => setTrack(e.target.value)}>
        <option value="dotnet">.NET</option>
        <option value="react">React</option>
        <option value="system-design">System Design</option>
      </select>

      <label>Difficulty</label>
      <select value={difficulty} onChange={(e) => setDifficulty(e.target.value)}>
        <option value="easy">Easy</option>
        <option value="medium">Medium</option>
        <option value="hard">Hard</option>
      </select>

      <button onClick={start} disabled={loading}>
        {loading ? "Starting..." : "Start Interview"}
      </button>
    </div>
  );
}
