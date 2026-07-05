import { API_URL } from "../config/api";

function getAuthHeaders() {
    const token = localStorage.getItem("token");
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    };
}

export async function getProfile() {
    // URL совпадает с маршрутом [HttpGet] в нашем ProfileController
    const response = await fetch(`${API_URL}/api/profile`, {
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        // Парсим ошибку, которую сформировал твой BaseController (error.Message)
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось получить данные профиля");
    }

    return await response.json();
}

export async function sendProfileUpdateRequest(phone, email) {
    const response = await fetch(`${API_URL}/api/profile/request-change`, {
        method: "POST",
        headers: getAuthHeaders(),
        body: JSON.stringify({ phone, email })
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось отправить запрос на изменение профиля");
    }

    return await response.json(); // Предположим, бэкенд возвращает статус или созданный объект запроса
}