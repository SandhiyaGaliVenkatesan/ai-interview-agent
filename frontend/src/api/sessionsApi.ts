import { http } from "./http";

export async function createSession(track: string, difficulty: string) {
  const res = await http.post("/api/sessions", { track, difficulty });
  return res.data;
}
//export const createSession = (payload: any) => http.post("/api/sessions", payload);
export async function listSessions() {
  const res = await http.get("/api/sessions");
  return res.data;
}

export async function getSession(id: string) {
  const res = await http.get(`/api/sessions/${id}`);
  return res.data;
}

export async function generateQuestion(id: string, context?: string) {
  const res = await http.post(`/api/sessions/${id}/question`, { context: context ?? "" });
  return res.data;
}

export async function submitAnswer(id: string, turnNumber: number, answer: string) {
  const res = await http.post(`/api/sessions/${id}/turns/${turnNumber}/answer`, { answer });
  return res.data;
}
