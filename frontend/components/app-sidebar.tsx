"use client"

import { Home, Search, Settings, Users } from "lucide-react"

import {
    Sidebar,
    SidebarContent,
    SidebarFooter,
    SidebarGroup,
    SidebarGroupContent,
    SidebarGroupLabel,
    SidebarMenu,
    SidebarMenuButton,
    SidebarMenuItem,
    SidebarRail
} from "@/components/ui/sidebar"
import { ModeToggle } from "./mode-toggle"
import { NavUser } from "./nav-user"
import { useSession } from "next-auth/react"

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
        icon: Users,
    }
]

export function AppSidebar() {
    const { data: session } = useSession();

    if (!session || !session.user)
        return null;

    console.log(session.user);

    return (
        <Sidebar collapsible="icon">
            <SidebarContent>
                <SidebarGroup>
                    <SidebarGroupLabel>Charon UI</SidebarGroupLabel>
                    <SidebarGroupContent>
                        <SidebarMenu>
                            {items.map((item) => (
                                <SidebarMenuItem key={item.title}>
                                    <SidebarMenuButton asChild>
                                        <a href={item.url}>
                                            <item.icon />
                                            <span>{item.title}</span>
                                        </a>
                                    </SidebarMenuButton>
                                </SidebarMenuItem>
                            ))}
                        </SidebarMenu>
                    </SidebarGroupContent>
                </SidebarGroup>
            </SidebarContent>
            <SidebarFooter>
                <ModeToggle />
                <NavUser user={session.user} />
            </SidebarFooter>
            <SidebarRail />
        </Sidebar>
    )
}
