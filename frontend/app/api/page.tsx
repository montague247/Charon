"use client";

import { useQuery } from "@tanstack/react-query";
import api from "../../lib/api";

export default function ApiPage() {
    const { data, isLoading, error } = useQuery({
        queryKey: ["weather"],
        queryFn: async () => {
            const res = await api.get<string[]>("/weather");
            return res.data;
        },
    });

    if (isLoading) {
        return <p className="p-6">Loading...</p>;
    }

    if (error) {
        return <p className="p-6 text-red-500">Failed to load weather data.</p>;
    }

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold">Weather Forecast</h1>
            <ul className="mt-4 list-disc pl-5">
                {data?.map((w, i) => (
                    <li key={i}>{w}</li>
                ))}
            </ul>
        </div>
    );
}
