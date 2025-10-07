"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { SessionProvider } from "next-auth/react";
import { ReactNode, useState } from "react";

export function Providers({ children }: { children: ReactNode }) {
    // Create the QueryClient once per app render
    const [queryClient] = useState(() => new QueryClient());

    return <QueryClientProvider client={queryClient}><SessionProvider>{children}</SessionProvider></QueryClientProvider>;
}
