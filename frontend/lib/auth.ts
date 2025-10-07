import { NextAuthOptions } from "next-auth"
import { Provider } from "next-auth/providers/index"
import GoogleProvider from "next-auth/providers/google"
import GitHubProvider from "next-auth/providers/github"
import AppleProvider from "next-auth/providers/apple"
import BattleNetProvider from "next-auth/providers/battlenet"

const providers: Provider[] = [];

if (process.env.GOOGLE_CLIENT_ID && process.env.GOOGLE_CLIENT_SECRET)
    providers.push(GoogleProvider({
        clientId: process.env.GOOGLE_CLIENT_ID!,
        clientSecret: process.env.GOOGLE_CLIENT_SECRET!
    }));

if (process.env.GITHUB_CLIENT_ID && process.env.GITHUB_CLIENT_SECRET)
    providers.push(GitHubProvider({
        clientId: process.env.GITHUB_CLIENT_ID!,
        clientSecret: process.env.GITHUB_CLIENT_SECRET!
    }));

if (process.env.APPLE_CLIENT_ID && process.env.APPLE_CLIENT_SECRET)
    providers.push(AppleProvider({
        clientId: process.env.APPLE_CLIENT_ID!,
        clientSecret: process.env.APPLE_CLIENT_SECRET!
    }));

if (process.env.BATTLENET_CLIENT_ID && process.env.BATTLENET_CLIENT_SECRET)
    providers.push(BattleNetProvider({
        clientId: process.env.BATTLENET_CLIENT_ID!,
        clientSecret: process.env.BATTLENET_CLIENT_SECRET!,
        issuer: "https://eu.battle.net/oauth"
    }));

export const authOptions: NextAuthOptions = {
    providers: providers,
    secret: process.env.NEXTAUTH_SECRET,
    pages: {
        signIn: "/api/auth/signin"
    },
    session: {
        strategy: "jwt"
    },
    callbacks: {
        async signIn({ user, account, profile, email, credentials }) {
            console.log("callback: signIn");
            return true
        },
        async redirect({ url, baseUrl }) {
            console.log("callback: redirect " + baseUrl);
            return baseUrl
        },
        async jwt({ token, user, account, profile }) {
            console.log("callback: jwt");
            return token
        },
        async session({ session, user, token }) {
            console.log("callback: session");
            return session
        }
    }
}
