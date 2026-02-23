import { TOKEN_KEY } from "./hooks/useAuthToken.ts";
import {
    UserClient, EggAccountClient
} from "./api/ServerApi.ts";

const customFetch = async (url: RequestInfo, init?: RequestInit) => {
    const token = localStorage.getItem(TOKEN_KEY);

    if (token) {
        // Copy of existing init or new object, with copy of existing headers or
        // new object including Bearer token.
        init = {
            ...(init ?? {}),
            headers: {
                ...(init?.headers ?? {}),
                Authorization: `Bearer ${token}`,
            },
        };
    }
    return await fetch(url, init);
};

const baseUrl = undefined;
export const userClient = new UserClient(baseUrl,{fetch: customFetch});
export const eggAccountClient = new EggAccountClient(baseUrl, {fetch: customFetch});
