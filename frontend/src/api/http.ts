import axios from "axios";

export const http = axios.create({
  baseURL: "", // update to backend port shown in console
  headers: { "Content-Type": "application/json" }
});
