# AI Interview Agent (React + .NET + LLM (i.e. OpenAPI/Gemini))

An AI-powered interview practice app that generates role-based questions and evaluates answers with structured feedback.

**What it does**
- Creates an interview session by **track** (dotnet / react / system-design) and **difficulty** (easy / medium / hard)
- Generates the **next question** dynamically using Gemini
- Accepts a user answer and returns **evaluation JSON**: score, verdict, strengths, improvements, and a short model answer
- Saves sessions + turns in a database (EF Core)

---

## Demo Flow (API)
1) Create Session  
2) Generate Question  
3) Submit Answer  
4) View Session Summary (all turns)

---

## Tech Stack
**Frontend**
- React + TypeScript + Vite
- Axios API client

**Backend**
- ASP.NET Core Web API
- EF Core + SQLite
- Gemini (Google Generative Language API)

---

## Features
- Session-based interview practice (multi-turn)
- JSON-structured responses from AI (reliable parsing)
- Answer evaluation with actionable feedback
- Persistent storage (session history)
- Clean API design (REST)

---

## Project Structure
