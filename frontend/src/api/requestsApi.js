import { API_URL } from "../config/api";

function getAuthHeaders() {
    const token = localStorage.getItem("token");
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    };
}

export async function getMyRequests() {
    const response = await fetch(`${API_URL}/api/requests/my`, {
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось получить список запросов");
    }

    return await response.json();
}

export async function getAllRequests() {
    const response = await fetch(`${API_URL}/api/requests`, {
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось получить список задач");
    }

    return await response.json();
}

export async function createRequest({ requestTypeId, comment }) {
    const response = await fetch(`${API_URL}/api/requests`, {
        method: "POST",
        headers: getAuthHeaders(),
        body: JSON.stringify({ requestTypeId, comment })
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось создать электронный запрос");
    }

    return response;
}

export async function takeRequest(id) {
    const response = await fetch(`${API_URL}/api/requests/${id}/take`, {
        method: "PATCH",
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось взять запрос в работу");
    }

    return response;
}

export async function completeRequest(id, resolutionComment) {
    const response = await fetch(`${API_URL}/api/requests/${id}/complete`, {
        method: "PATCH",
        headers: getAuthHeaders(),
        body: JSON.stringify({ 
            status: "done", // <-- ДОБАВЛЕНО: Бэкенд ожидает поле Status
            resolutionComment 
        })
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось завершить запрос");
    }

    return response;
}

export async function assignManager(id, managerId) {
    const response = await fetch(`${API_URL}/api/requests/${id}/assign`, {
        method: "PATCH",
        headers: getAuthHeaders(),
        body: JSON.stringify({ managerId })
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось назначить менеджера");
    }

    return response;
}