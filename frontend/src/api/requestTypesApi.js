import { API_URL } from "../config/api";

function getAuthHeaders() {
    const token = localStorage.getItem("token");
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    };
}

export async function getAllRequestTypes() {
    const response = await fetch(`${API_URL}/api/requesttypes`, {
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось получить типы запросов");
    }

    return await response.json();
}

export async function createRequestType(data) {
    const response = await fetch(`${API_URL}/api/requesttypes`, {
        method: "POST",
        headers: getAuthHeaders(),
        body: JSON.stringify(data)
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось создать тип запроса");
    }

    return response;
}

export async function updateRequestType(id, data) {
    const response = await fetch(`${API_URL}/api/requesttypes/${id}`, {
        method: "PUT",
        headers: getAuthHeaders(),
        body: JSON.stringify(data)
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось обновить тип запроса");
    }

    return response;
}

export async function deleteRequestType(id) {
    const response = await fetch(`${API_URL}/api/requesttypes/${id}`, {
        method: "DELETE",
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось удалить тип запроса");
    }

    return response;
}