export interface InterviewSession {
  id: string;
  role: string;
  level: string;
  topic?: string;
  createdAtUtc: string;
}

export interface InterviewQuestion {
  id: string;
  order: number;
  text: string;
}

export interface Evaluation {
  score: number;
  strengths: string[];
  gaps: string[];
  improvement: string[];
  followUpQuestion: string;
}
