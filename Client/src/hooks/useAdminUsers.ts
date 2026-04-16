import type {UserDto} from "../api/ServerApi.ts";
import {useCallback, useEffect, useMemo, useState} from "react";
import {userClient} from "../api-clients.ts";

const DEFAULT_PAGE_SIZE = 10;
const DEFAULT_SORT_FIELD: SortField = "username";
const DEFAULT_SORT_DIRECTION: SortDirection = "asc";

const sanitizeSearchTerm = (value: string): string | null => {
    const normalized = value.trim().replace(/[|,"]/g, " ").replace(/\s+/g, " ").trim();
    return normalized || null;
}

export type SortField = "username" | "email";
export type RoleFilter = "All" | "Admin" | "User";
export type SortDirection = "asc" | "desc";

type useUsersResult = {
    users: UserDto[];
    total: number;
    page: number;
    pageSize: number;
    totalPages: number;
    visibleStart: number;
    visibleEnd: number;
    loading: boolean;
    hasLoadedOnce: boolean;
    error: string | null;
    searchTerm: string;
    roleFilter: RoleFilter;
    sortField: SortField;
    sortDirection: SortDirection;
    setSearchTerm: (value: string) => void;
    setRoleFilter: (value: RoleFilter) => void;
    setSortField: (value: SortField) => void;
    toggleSortDirection: () => void;
    toggleSort: (field: SortField) => void;
    handlePageChange: (direction: "prev" | "next") => void;
    resetFilters: () => void;
    refresh: () => Promise<void>;
};

const coerceNumber = (value: unknown, fallback = 0): number => {
    const num = Number(value);
    return !Number.isNaN(num) ? num : fallback;
}

const buildSieveFilters = (search: string, role: RoleFilter): string => {
    const filters = [];
    if (search) {
        filters.push(`(username@=*${search}*|email@=*${search}*)`);
    }
    if (role !== "All") {
        filters.push(`role==${role}`);
    }
    return filters.join(",");
}

export function useAdminUsers(): useUsersResult {
    const [users, setUsers] = useState<UserDto[]>([]);
    const [total, setTotal] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
    const [loading, setLoading] = useState(false);
    const [hasLoadedOnce, setHasLoadedOnce] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [searchTerm, setSearchTermState] = useState("");
    const [roleFilter, setRoleFilterState] = useState<RoleFilter>("All");
    const [sortField, setSortFieldState] = useState<SortField>(DEFAULT_SORT_FIELD);
    const [sortDirection, setSortDirection] = useState<SortDirection>(DEFAULT_SORT_DIRECTION);

    const visibleStart = total === 0 ? 0 : (page - 1) * pageSize + 1;
    const visibleEnd = Math.min(page * pageSize, total);

    const filters = useMemo(() => {
        const term = sanitizeSearchTerm(searchTerm) ?? "";
        return buildSieveFilters(term, roleFilter) || undefined;
    }, [roleFilter, searchTerm]);

    const sorts = useMemo(() => (sortDirection === "asc" ? sortField : `-${sortField}`), [sortDirection, sortField]);
    const totalPages = useMemo(() => Math.max(1, Math.ceil(total / pageSize)), [pageSize, total]);

    const fetchUsers = useCallback(async (): Promise<void> => {
        setLoading(true);
        setError(null);

        try {
            const response = await userClient.getAll(filters, sorts, page, pageSize);
            const list = Array.isArray(response?.items) ? response.items : [];
            const totalCount = coerceNumber(response?.totalCount, list.length);
            const serverPageSize = coerceNumber(response?.pageSize, DEFAULT_PAGE_SIZE);
            const serverPage = Math.max(1, coerceNumber(response?.page, page));
            
            setUsers(list);
            setTotal(totalCount);
            setPageSize(serverPageSize);
            setPage(serverPage);
        } catch {
            setError("Failed to load users.");
            setUsers([]);
            setTotal(0);
        } finally {
            setLoading(false);
            setHasLoadedOnce(true);
        }
    }, [filters, page, pageSize, sorts]);

    useEffect(() => {
        void fetchUsers();
    }, [fetchUsers]);

    const setSearchTerm = useCallback((value: string) => {
        setSearchTermState(value);
        setPage(1);
    }, []);

    const setRoleFilter = useCallback((value: RoleFilter) => {
        setRoleFilterState(value);
        setPage(1);
    }, []);

    const setSortField = useCallback((value: SortField) => {
        setSortFieldState(value);
        setPage(1);
    }, []);

    const toggleSortDirection = useCallback(() => {
        setSortDirection((prev) => (prev === "asc" ? "desc" : "asc"));
        setPage(1);
    }, []);

    const toggleSort = useCallback((field: SortField) => {
        if (field === sortField) {
            toggleSortDirection();
        } else {
            setSortFieldState(field);
            setSortDirection("asc");
            setPage(1);
        }
    }, [sortField, toggleSortDirection]);

    const handlePageChange = useCallback((direction: "prev" | "next") => {
        setPage((prev) => {
            if (direction === "prev") return Math.max(prev - 1, 1);
            if (direction === "next") return Math.min(prev + 1, totalPages);
            return prev;
        });
    }, [totalPages]);

    const resetFilters = useCallback(() => {
        setSearchTermState("");
        setRoleFilterState("All");
        setSortFieldState(DEFAULT_SORT_FIELD);
        setSortDirection(DEFAULT_SORT_DIRECTION);
        setPage(1);
    }, []);

    return {
        users,
        total,
        page,
        pageSize,
        totalPages,
        visibleStart,
        visibleEnd,
        loading,
        hasLoadedOnce,
        error,
        searchTerm,
        roleFilter,
        sortField,
        sortDirection,
        setSearchTerm,
        setRoleFilter,
        setSortField,
        toggleSortDirection,
        toggleSort,
        handlePageChange,
        resetFilters,
        refresh: fetchUsers
    }
}
