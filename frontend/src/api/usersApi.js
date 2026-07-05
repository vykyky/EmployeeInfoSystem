import { API_URL } from "../config/api";

function getAuthHeaders() {
    const token = localStorage.getItem("token");
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    };
}

export async function getAllUsers() {
    const response = await fetch(`${API_URL}/api/users`, {
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось получить список пользователей");
    }

    return await response.json();
}

export async function getUserById(id) {
    const response = await fetch(`${API_URL}/api/users/${id}`, {
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось получить пользователя");
    }

    return await response.json();
}

export async function createUser(data) {
    const response = await fetch(`${API_URL}/api/users`, {
        method: "POST",
        headers: getAuthHeaders(),
        body: JSON.stringify(data)
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось создать пользователя");
    }

    return response;
}

export async function deleteUser(id) {
    const response = await fetch(`${API_URL}/api/users/${id}`, {
        method: "DELETE",
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось удалить пользователя");
    }

    return response;
}

export async function changeUserRole(id, role) {
    const response = await fetch(`${API_URL}/api/users/${id}/role`, {
        method: "PATCH",
        headers: getAuthHeaders(),
        body: JSON.stringify(role)
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось изменить роль");
    }

    return response;
}