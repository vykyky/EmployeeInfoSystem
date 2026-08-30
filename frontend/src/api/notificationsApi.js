import { API_URL } from "../config/api";

function getAuthHeaders() {
    const token = localStorage.getItem("token");
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    };
}

export async function getMyNotifications() {
    const response = await fetch(`${API_URL}/api/notifications/my`, {
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось получить уведомления");
    }

    return await response.json();
}

export async function markNotificationRead(id) {
    const response = await fetch(`${API_URL}/api/notifications/${id}/read`, {
        method: "PATCH",
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось отметить уведомление");
    }

    return response;
}

// Новое: удаление уведомления
export async function deleteNotification(id) {
    const response = await fetch(`${API_URL}/api/notifications/${id}`, {
        method: "DELETE",
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось удалить уведомление");
    }

    return response;
}