export { default } from "next-auth/middleware"

export const config = {
    matcher: [
        "/users"//"/((?!api/auth|_next/static|_next/image|__nextjs_font|favicon.ico).*)"
    ]
}
