import axios from "axios";

const api = axios.create({
    baseURL: "https://68e3c58a8e14f4523dae9daf.mockapi.io/api/latest/", // .NET API
});

export default api;
