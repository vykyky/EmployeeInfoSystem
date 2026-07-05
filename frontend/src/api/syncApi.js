import { API_URL } from "../config/api";

function getAuthHeaders() {
    const token = localStorage.getItem("token");
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    };
}

export async function syncProfileByTabn(tabn) {
    const response = await fetch(`${API_URL}/api/sync/profile?tabn=${encodeURIComponent(tabn)}`, {
        method: "POST",
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось синхронизировать профиль сотрудника");
    }

    return response;
}

export async function syncAllProfiles() {
    const response = await fetch(`${API_URL}/api/sync/profile`, {
        method: "POST",
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось синхронизировать профили всех сотрудников");
    }

    return response;
}

export async function syncPpeByTabn(tabn) {
    const response = await fetch(`${API_URL}/api/sync/ppe?tabn=${encodeURIComponent(tabn)}`, {
        method: "POST",
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось синхронизировать спец. одежду сотрудника");
    }

    return response;
}

export async function syncAllPpe() {
    const response = await fetch(`${API_URL}/api/sync/ppe`, {
        method: "POST",
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось синхронизировать спец. одежду всех сотрудников");
    }

    return response;
}