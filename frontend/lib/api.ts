import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5000/api", // .NET API
});

export default api;
