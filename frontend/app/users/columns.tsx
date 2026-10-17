"use client"

import { Button } from "@/components/ui/button"
import { DataTableFeatures } from "@/components/data-table"
import { createColumnHelper } from "@tanstack/react-table"
import { ArrowUpDown } from "lucide-react"

// This type is used to define the shape of our data.
// You can use a Zod schema here if you want.
export type User = {
    id: string
    name: string
    email: string
    createdAt: string
}

const columnHelper = createColumnHelper<DataTableFeatures, User>()

export const columns = columnHelper.columns([
    columnHelper.accessor("id", {
        header: "ID"
    }),
    columnHelper.accessor("name", {
        header: ({ column }) => {
            return (
                <Button
                    variant="ghost"
                    onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
                >
                    Name
                    <ArrowUpDown className="ml-2 h-4 w-4" />
                </Button>
            )
        }
    }),
    columnHelper.accessor("email", {
        header: ({ column }) => {
            return (
                <Button
                    variant="ghost"
                    onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
                >
                    Email
                    <ArrowUpDown className="ml-2 h-4 w-4" />
                </Button>
            )
        }
    }),
    columnHelper.accessor("createdAt", {
        header: "Created At"
    })
])