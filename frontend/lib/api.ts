import axios, { InternalAxiosRequestConfig } from "axios";
import { getSession } from "next-auth/react";
import { ExtendedSession } from "./auth";

const api = axios.create({
    baseURL: "https://68e3c58a8e14f4523dae9daf.mockapi.io/api/latest/", // .NET API
});

api.interceptors.request.use(async (config: InternalAxiosRequestConfig<any>) => {
    const session = await getSession() as ExtendedSession | null;

    if (session?.accessToken) {
        config.headers.set('Authorization', `Bearer ${session.accessToken}`);
    }
    return config;
});

export default api;
