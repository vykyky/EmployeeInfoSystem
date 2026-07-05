import { API_URL } from "../config/api";

export async function login(tabn, password) {
    const response = await fetch(`${API_URL}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ tabn, password })
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Ошибка входа");
    }

    return await response.json(); // { token, role, fio }
}

export function saveAuth(token, role, fio) {
    localStorage.setItem("token", token);
    localStorage.setItem("role", role);
    localStorage.setItem("fio", fio);
}

export function clearAuth() {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    localStorage.removeItem("fio");
}

export function getRole() {
    return localStorage.getItem("role");
}

export function getToken() {
    return localStorage.getItem("token");
}

export function isAuthenticated() {
    return !!localStorage.getItem("token");
}