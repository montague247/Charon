import { NextApiRequest, NextApiResponse } from "next";
import { NextAuthOptions, Session, TokenSet } from "next-auth"
import { JWT } from "next-auth/jwt";
import NextAuth from "next-auth/next";
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

export interface ExtendedJWT extends JWT {
    accessToken?: string;
}
export interface ExtendedSession extends Session {
    accessToken?: string;
}

export const authOptions: NextAuthOptions = {
    providers: providers,
    secret: process.env.NEXTAUTH_SECRET,
    session: {
        strategy: "jwt"
    },
    callbacks: {
        async jwt({ token, account }): Promise<JWT> {
            if (account) {
                token.accessToken = (account as TokenSet).access_token;
            }

            return token as ExtendedJWT;
        },
        async session({ session, token }): Promise<Session> {
            const extendedSession: ExtendedSession = {
                ...session, accessToken: (token as ExtendedJWT).accessToken,
            };

            return extendedSession;
        }
    }
}

export default function NextAuthHandler(req: NextApiRequest, res: NextApiResponse) {
    return NextAuth(req, res, authOptions);
}
