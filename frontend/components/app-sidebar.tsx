"use client"

import { Home, Search, Settings, Users } from "lucide-react"

import {
    Sidebar,
    SidebarContent,
    SidebarFooter,
    SidebarRail
} from "@/components/ui/sidebar"
import { ModeToggle } from "./mode-toggle"
import { NavUser } from "./nav-user"
import { useSession } from "next-auth/react"
import { NavMain } from "./nav-main"

const items = [
    {
        title: "Home",
        url: "#",
        icon: Home,
    },
    {
        title: "Search",
        url: "#",
        icon: Search,
    },
    {
        title: "Settings",
        url: "#",
        icon: Settings,
    },
    {
        title: "Users",
        url: "users",
        icon: Users
    }
]

export function AppSidebar() {
    const { data: session } = useSession();

    if (!session?.user)
        return null;

    return (
        <Sidebar collapsible="icon">
            <SidebarContent>
                <NavMain items={items} />
            </SidebarContent>
            <SidebarFooter>
                <ModeToggle />
                <NavUser user={session.user} />
            </SidebarFooter>
            <SidebarRail />
        </Sidebar>
    )
}
