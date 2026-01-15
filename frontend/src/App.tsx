import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import Home from "./pages/Home";
import Interview from "./pages/Interview";
import History from "./pages/History";
import SessionDetail from "./pages/SessionDetail";
import "./Styles.css";

export default function App() {
  return (
    <BrowserRouter>
      <div className="container">
        <header className="header">
          <Link className="brand" to="/">AI Interview Agent</Link>
          <nav className="nav">
            <Link to="/history">History</Link>
          </nav>
        </header>

        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/interview/:id" element={<Interview />} />
          <Route path="/history" element={<History />} />
          <Route path="/sessions/:id" element={<SessionDetail />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}
