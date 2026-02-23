import {useCallback, useEffect, useMemo, useState} from "react";
import {userClient} from "../api-clients.ts";
import type {UpdateUserDto, UserDto} from "../api/ServerApi.ts";

export type EditableUserFields = {
    username: string;
    email: string;
    discordUsername: string;
    role: string;
    password: string;
};

type UseAdminUserDetailsResult = {
    user: UserDto | null;
    form: EditableUserFields;
    loading: boolean;
    saving: boolean;
    deleting: boolean;
    hasLoadedOnce: boolean;
    error: string | null;
    setField: (field: keyof EditableUserFields, value: string) => void;
    refresh: () => Promise<void>;
    updateUser: () => Promise<void>;
    deleteUser: () => Promise<void>;
};

const emptyForm: EditableUserFields = {
    username: "",
    email: "",
    discordUsername: "",
    role: "User",
    password: "",
};

const toForm = (user: UserDto | null): EditableUserFields => ({
    username: user?.username ?? "",
    email: user?.email ?? "",
    discordUsername: user?.discordUsername ?? "",
    role: user?.role ?? "User",
    password: "",
});

const buildUpdateDto = (form: EditableUserFields): UpdateUserDto => {
    const dto: UpdateUserDto = {
        username: form.username || undefined,
        email: form.email || undefined,
        discordUsername: form.discordUsername || undefined,
        role: form.role || undefined,
    };
    if (form.password.trim()) {
        dto.password = form.password.trim();
    }
    return dto;
}

export function useAdminUserDetails(userId: string | undefined): UseAdminUserDetailsResult {
    const [user, setUser] = useState<UserDto | null>(null);
    const [form, setForm] = useState<EditableUserFields>(emptyForm);
    const [loading, setLoading] = useState(false);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [hasLoadedOnce, setHasLoadedOnce] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const canFetch = useMemo(() => Boolean(userId && userId.trim()), [userId]);

    const refresh = useCallback(async () => {
        if (!canFetch) return;
        setLoading(true);
        setError(null);
        try {
            const fetched = await userClient.getById(userId!);
            setUser(fetched ?? null);
            setForm(toForm(fetched ?? null));
        } catch (err) {
            console.error("Failed to load user", err);
            setError("Failed to load user details.");
            setUser(null);
            setForm(emptyForm);
        } finally {
            setLoading(false);
            setHasLoadedOnce(true);
        }
    }, [canFetch, userId]);

    useEffect(() => {
        void refresh();
    }, [refresh]);

    const setField = useCallback((field: keyof EditableUserFields, value: string) => {
        setForm((prev) => ({...prev, [field]: value}));
    }, []);

    const updateUser = useCallback(async () => {
        if (!canFetch) return;
        setSaving(true);
        setError(null);
        try {
            const updated = await userClient.update(userId!, buildUpdateDto(form));
            setUser(updated ?? null);
            setForm(toForm(updated ?? null));
        } catch (err) {
            console.error("Failed to update user", err);
            setError("Failed to update user.");
        } finally {
            setSaving(false);
        }
    }, [canFetch, form, userId]);

    const deleteUser = useCallback(async () => {
        if (!canFetch) return;
        setDeleting(true);
        setError(null);
        try {
            await userClient.delete(userId!);
            setUser(null);
            setForm(emptyForm);
        } catch (err) {
            console.error("Failed to delete user", err);
            setError("Failed to delete user.");
        } finally {
            setDeleting(false);
        }
    }, [canFetch, userId]);

    return {
        user,
        form,
        loading,
        saving,
        deleting,
        hasLoadedOnce,
        error,
        setField,
        refresh,
        updateUser,
        deleteUser,
    };
}
