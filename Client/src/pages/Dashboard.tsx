import {useAuthToken} from "../hooks/useAuthToken.ts";
import MainPage from "./MainPage.tsx";
import AdminPage from "./AdminPage.tsx";

function getRoleFromJwt(jwt: string | null | undefined): string | null {
    if (!jwt) return null;
    try {
        const payloadBase64 = jwt.split(".")[1];
        const payloadJson = atob(payloadBase64);
        const payload = JSON.parse(payloadJson);
        return payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? null;
    } catch {
        return null;
    }
}

export default function Dashboard() {
    const jwt = useAuthToken();
    const role = getRoleFromJwt(jwt);
    const isAdmin = role === "Admin";

    return (
        <div className="flex flex-col min-h-screen w-full">
            {isAdmin ? <AdminPage /> : <MainPage />}
        </div>
    );


}