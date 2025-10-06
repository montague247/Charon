"use client";

import { useQuery } from "@tanstack/react-query";
import api from "../../lib/api";
import { columns, User } from "./columns";
import { DataTable } from "@/components/data-table";

export default function UsersPage() {
    const { data, isLoading, error } = useQuery({
        queryKey: ["users"],
        queryFn: async () => {
            const res = await api.get<User[]>("users");
            return res.data;
        },
    });

    if (isLoading) {
        return <p className="p-6">Loading...</p>;
    }

    if (error) {
        return <p className="p-6 text-red-500">Failed to load users data.</p>;
    }

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold">Users</h1>
            <div className="container mx-auto py-10">
                <DataTable columns={columns} data={data!} />
            </div>
        </div>
    );
}
